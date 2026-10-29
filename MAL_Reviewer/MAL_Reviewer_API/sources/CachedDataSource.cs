using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// Remembers the answers of another data source for a while, so repeating a search
    /// or previewing an entry again is answered instantly, without a request.
    /// Failures and missing entries are not remembered.
    /// </summary>
    internal class CachedDataSource : IAnimeDataSource
    {
        #region Fields

        /// <summary>
        /// How long an answer is kept.
        /// </summary>
        internal static readonly TimeSpan DefaultLifetime = TimeSpan.FromMinutes(10);

        /// <summary>
        /// The most answers kept; the oldest ones are forgotten first.
        /// </summary>
        internal const int DefaultCapacity = 200;

        private readonly IAnimeDataSource source;
        private readonly TimeSpan lifetime;
        private readonly int capacity;
        private readonly Func<DateTimeOffset> now;
        private readonly Dictionary<string, (DateTimeOffset Stored, object Value)> answers = new Dictionary<string, (DateTimeOffset, object)>();

        #endregion

        #region Properties

        public string Name => this.source.Name;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="source"> The data source whose answers are kept. </param>
        /// <param name="lifetime"> How long an answer is kept; ten minutes when null. </param>
        /// <param name="capacity"> The most answers kept. </param>
        /// <param name="now"> The clock; replaceable for tests. </param>
        public CachedDataSource(IAnimeDataSource source, TimeSpan? lifetime = null, int capacity = DefaultCapacity, Func<DateTimeOffset> now = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.lifetime = lifetime ?? DefaultLifetime;
            this.capacity = Math.Max(1, capacity);
            this.now = now ?? (() => DateTimeOffset.UtcNow);
        }

        #endregion

        #region Public methods

        public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct)
        {
            // Searches differing only by letter case or surrounding spaces give the same results.
            string key = $"search|{ kind }|{ SearchLimits.Clamp(limit) }|{ query?.Trim().ToLowerInvariant() }";

            return GetOrFetchAsync(key, () => this.source.SearchAsync(kind, query, limit, ct));
        }

        public Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct)
        {
            return GetOrFetchAsync($"anime|{ malId }", () => this.source.GetAnimeAsync(malId, ct));
        }

        public Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct)
        {
            return GetOrFetchAsync($"manga|{ malId }", () => this.source.GetMangaAsync(malId, ct));
        }

        #endregion

        #region Private methods

        private async Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> fetch) where T : class
        {
            lock (this.answers)
            {
                if (this.answers.TryGetValue(key, out (DateTimeOffset Stored, object Value) answer))
                {
                    if (this.now() - answer.Stored < this.lifetime)
                    {
                        return (T)answer.Value;
                    }

                    this.answers.Remove(key);
                }
            }

            T value = await fetch();

            if (value != null)
            {
                lock (this.answers)
                {
                    this.answers[key] = (this.now(), value);

                    if (this.answers.Count > this.capacity)
                    {
                        this.answers.Remove(this.answers.OrderBy(entry => entry.Value.Stored).First().Key);
                    }
                }
            }

            return value;
        }

        #endregion
    }
}
