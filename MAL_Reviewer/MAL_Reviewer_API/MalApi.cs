using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.sources;
using MAL_Reviewer_API.users;

namespace MAL_Reviewer_API
{
    /// <summary>
    /// Reads MyAnimeList data from free services that need no API key:
    /// AniList (falling back to Jikan v4) for searches, which it answers several times faster,
    /// Jikan (falling back to AniList) for entry details, Jikan for user profiles,
    /// and MyAnimeList's public list pages for user lists. Searches and details are cached for a while.
    /// </summary>
    public class MalApi : IMalApi, IDisposable
    {
        #region Fields

        private readonly HttpClient client;
        private readonly IAnimeDataSource searches;
        private readonly IAnimeDataSource entries;
        private readonly JikanUserClient users;
        private readonly MalListClient lists;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="options"> The endpoints and network settings; the environment-aware defaults when null. </param>
        /// <param name="handler"> The HTTP handler to send requests with; replaceable for tests. </param>
        /// <param name="delay"> Waits between retries of rate limited requests; replaceable for tests. </param>
        /// <param name="now"> The clock of the caches and the fallbacks; replaceable for tests. </param>
        public MalApi(ApiOptions options = null, HttpMessageHandler handler = null, Func<TimeSpan, CancellationToken, Task> delay = null, Func<DateTimeOffset> now = null)
        {
            options ??= ApiOptions.FromEnvironment();

            this.client = handler == null ? new HttpClient() : new HttpClient(handler);
            this.client.Timeout = options.Timeout;
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MAL_Reviewer", GetVersion()));

            ApiHttp jikan = new ApiHttp(this.client, "Jikan", options.MaxRetries, delay);
            ApiHttp aniList = new ApiHttp(this.client, "AniList", options.MaxRetries, delay);
            ApiHttp mal = new ApiHttp(this.client, "MyAnimeList", options.MaxRetries, delay);

            JikanDataSource jikanEntries = new JikanDataSource(jikan, options.JikanUrl);
            AniListDataSource aniListEntries = new AniListDataSource(aniList, options.AniListUrl, options.MalUrl);

            this.searches = new CachedDataSource(new FallbackDataSource(aniListEntries, jikanEntries, now: now), now: now);
            this.entries = new CachedDataSource(new FallbackDataSource(jikanEntries, aniListEntries, now: now), now: now);
            this.users = new JikanUserClient(jikan, options.JikanUrl);
            this.lists = new MalListClient(mal, options.MalUrl);
        }

        #endregion

        #region Public methods

        public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string title, int limit, CancellationToken ct)
        {
            return this.searches.SearchAsync(kind, title, limit, ct);
        }

        public Task<AnimeModel> GetAnimeAsync(int animeId, CancellationToken ct)
        {
            return this.entries.GetAnimeAsync(animeId, ct);
        }

        public Task<MangaModel> GetMangaAsync(int mangaId, CancellationToken ct)
        {
            return this.entries.GetMangaAsync(mangaId, ct);
        }

        public Task<MALUserModel> GetUserAsync(string username, CancellationToken ct)
        {
            return this.users.GetUserAsync(username, ct);
        }

        public async Task<IEnumerable<AnimelistEntryModel>> GetAnimeListAsync(string username, CancellationToken ct)
        {
            return await this.lists.GetAnimeListAsync(username, ct);
        }

        public async Task<IEnumerable<MangalistEntryModel>> GetMangaListAsync(string username, CancellationToken ct)
        {
            return await this.lists.GetMangaListAsync(username, ct);
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        #endregion

        #region Private methods

        private static string GetVersion()
        {
            Version version = typeof(MalApi).Assembly.GetName().Version;

            return version == null ? "0.0.0" : $"{ version.Major }.{ version.Minor }.{ version.Build }";
        }

        #endregion
    }
}
