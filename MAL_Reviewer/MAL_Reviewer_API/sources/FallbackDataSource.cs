using System;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// Asks a primary data source first and switches to a secondary one only when the primary is unavailable.
    /// A "not found" answer from the primary is final.
    /// After a failure, the primary is skipped for a cooldown period, so a service that is down
    /// doesn't make every request wait for its timeout.
    /// </summary>
    internal class FallbackDataSource : IAnimeDataSource
    {
        #region Fields

        /// <summary>
        /// How long the primary is skipped after it failed.
        /// </summary>
        internal static readonly TimeSpan DefaultCooldown = TimeSpan.FromMinutes(2);

        private readonly IAnimeDataSource primary;
        private readonly IAnimeDataSource secondary;
        private readonly TimeSpan cooldown;
        private readonly Func<DateTimeOffset> now;
        private readonly object gate = new object();

        /// <summary>
        /// When the primary may be asked again.
        /// </summary>
        private DateTimeOffset primaryRetryAt = DateTimeOffset.MinValue;

        #endregion

        #region Properties

        public string Name => $"{ this.primary.Name } / { this.secondary.Name }";

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="primary"> The source asked first. </param>
        /// <param name="secondary"> The source asked when the primary is unavailable. </param>
        /// <param name="cooldown"> How long the primary is skipped after a failure; two minutes when null. </param>
        /// <param name="now"> The clock; replaceable for tests. </param>
        public FallbackDataSource(IAnimeDataSource primary, IAnimeDataSource secondary, TimeSpan? cooldown = null, Func<DateTimeOffset> now = null)
        {
            this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
            this.secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));
            this.cooldown = cooldown ?? DefaultCooldown;
            this.now = now ?? (() => DateTimeOffset.UtcNow);
        }

        #endregion

        #region Public methods

        public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct)
        {
            return RunAsync(source => source.SearchAsync(kind, query, limit, ct));
        }

        public Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct)
        {
            return RunAsync(source => source.GetAnimeAsync(malId, ct));
        }

        public Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct)
        {
            return RunAsync(source => source.GetMangaAsync(malId, ct));
        }

        #endregion

        #region Private methods

        private async Task<T> RunAsync<T>(Func<IAnimeDataSource, Task<T>> call)
        {
            DataSourceUnavailableException primaryError = null;

            if (IsPrimaryAllowed())
            {
                try
                {
                    return await call(this.primary);
                }
                catch (DataSourceUnavailableException e)
                {
                    primaryError = e;

                    lock (this.gate)
                    {
                        this.primaryRetryAt = this.now() + this.cooldown;
                    }
                }
            }

            try
            {
                return await call(this.secondary);
            }
            catch (DataSourceUnavailableException secondaryError)
            {
                Exception cause = primaryError == null ? (Exception)secondaryError : new AggregateException(primaryError, secondaryError);

                throw new DataSourceUnavailableException(this.Name, "every data source failed", cause);
            }
        }

        private bool IsPrimaryAllowed()
        {
            lock (this.gate)
            {
                return this.now() >= this.primaryRetryAt;
            }
        }

        #endregion
    }
}
