using System.Collections.Generic;
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
    /// Everything the application reads from MyAnimeList, whichever service actually answers.
    /// </summary>
    /// <remarks>
    /// Missing data is reported with an empty array or null;
    /// a failing service throws <see cref="DataSourceUnavailableException"/>.
    /// </remarks>
    public interface IMalApi
    {
        /// <summary>
        /// Returns up to <paramref name="limit"/> entries matching the passed title, (see <see cref="SearchLimits"/>).
        /// </summary>
        Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string title, int limit, CancellationToken ct);

        /// <summary>
        /// Returns the Anime with the passed MAL id, or null if it does not exist.
        /// </summary>
        Task<AnimeModel> GetAnimeAsync(int animeId, CancellationToken ct);

        /// <summary>
        /// Returns the Manga with the passed MAL id, or null if it does not exist.
        /// </summary>
        Task<MangaModel> GetMangaAsync(int mangaId, CancellationToken ct);

        /// <summary>
        /// Returns the profile, statistics and favorites of a MAL user, or null if the user does not exist.
        /// </summary>
        Task<MALUserModel> GetUserAsync(string username, CancellationToken ct);

        /// <summary>
        /// Returns the user's Anime list, or null if the user does not exist or the list is private.
        /// </summary>
        Task<IEnumerable<AnimelistEntryModel>> GetAnimeListAsync(string username, CancellationToken ct);

        /// <summary>
        /// Returns the user's Manga list, or null if the user does not exist or the list is private.
        /// </summary>
        Task<IEnumerable<MangalistEntryModel>> GetMangaListAsync(string username, CancellationToken ct);
    }
}
