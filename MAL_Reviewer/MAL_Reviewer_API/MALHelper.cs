using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.sources;

namespace MAL_Reviewer_API
{
    /// <summary>
    /// This is the entry point that enables fetching
    /// data from MAL (MyAnimeList), see <see cref="MalApi"/>.
    /// </summary>
    public static class MALHelper
    {
        #region Properties

        /// <summary>
        /// The API every call is forwarded to.
        /// </summary>
        public static IMalApi Api { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializing the API client.
        /// </summary>
        /// <param name="handler"> The HTTP handler to send requests with; replaceable for tests. </param>
        public static void Init(HttpMessageHandler handler = null)
        {
            Use(new MalApi(handler: handler));
        }

        /// <summary>
        /// Replaces the API every call is forwarded to.
        /// </summary>
        /// <param name="api"> The API to use. </param>
        public static void Use(IMalApi api)
        {
            IMalApi previous = Api;
            Api = api ?? throw new ArgumentNullException(nameof(api));

            if (!ReferenceEquals(previous, api))
            {
                (previous as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Returns the search results from MAL given the passed target.
        /// </summary>
        /// <param name="type"> The type of the target; (anime or manga). </param>
        /// <param name="title"> The title of the target; (the Anime/Manga's title/name). </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> Up to 10 search results; empty when nothing matches. </returns>
        public static Task<SearchResultsModel[]> Search(string type, string title, CancellationToken ct)
        {
            return Search(type, title, SearchLimits.Default, ct);
        }

        /// <summary>
        /// Returns the search results from MAL given the passed target.
        /// </summary>
        /// <param name="type"> The type of the target; (anime or manga). </param>
        /// <param name="title"> The title of the target; (the Anime/Manga's title/name). </param>
        /// <param name="limit"> The most results to return. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> Up to <paramref name="limit"/> search results; empty when nothing matches. </returns>
        public static Task<SearchResultsModel[]> Search(string type, string title, int limit, CancellationToken ct)
        {
            return GetApi().SearchAsync(MediaKindExtensions.Parse(type), title, limit, ct);
        }

        /// <summary>
        /// Returns an Anime with the given passed MAL id.
        /// </summary>
        /// <param name="animeId"> The MAL id of the Anime. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> The Anime's information, or null if it does not exist. </returns>
        public static Task<AnimeModel> GetAnime(int animeId, CancellationToken ct = default)
        {
            return GetApi().GetAnimeAsync(animeId, ct);
        }

        /// <summary>
        /// Returns a Manga with the given passed MAL id.
        /// </summary>
        /// <param name="mangaId"> The MAL id of the Manga. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> The Manga's information, or null if it does not exist. </returns>
        public static Task<MangaModel> GetManga(int mangaId, CancellationToken ct = default)
        {
            return GetApi().GetMangaAsync(mangaId, ct);
        }

        /// <summary>
        /// Returns a user with the given passed MAL username.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> A MALUserModel containing information about the user, or null if the user does not exist. </returns>
        public static Task<MALUserModel> GetUser(string username, CancellationToken ct)
        {
            return GetApi().GetUserAsync(username, ct);
        }

        /// <summary>
        /// Returns the Anime list of the given passed username.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> The user's Anime entries, or null if the list could not be found. </returns>
        public static Task<IEnumerable<AnimelistEntryModel>> GetAnimeList(string username, CancellationToken ct)
        {
            return GetApi().GetAnimeListAsync(username, ct);
        }

        /// <summary>
        /// Returns the Manga list of the given passed username.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> The user's Manga entries, or null if the list could not be found. </returns>
        public static Task<IEnumerable<MangalistEntryModel>> GetMangaList(string username, CancellationToken ct)
        {
            return GetApi().GetMangaListAsync(username, ct);
        }

        #endregion

        #region Private methods

        private static IMalApi GetApi()
        {
            return Api ?? throw new InvalidOperationException($"{ nameof(MALHelper) }.{ nameof(Init) } must be called first.");
        }

        #endregion
    }
}
