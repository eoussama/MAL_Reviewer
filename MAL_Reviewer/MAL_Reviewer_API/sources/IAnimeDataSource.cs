using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// A source of Anime and Manga information, keyed by MyAnimeList ids.
    /// </summary>
    /// <remarks>
    /// Missing data is reported with an empty array or null;
    /// a failing source throws <see cref="DataSourceUnavailableException"/>.
    /// </remarks>
    public interface IAnimeDataSource
    {
        /// <summary>
        /// The display name of the source, (ex: Jikan).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the entries matching the passed title, best matches first.
        /// </summary>
        /// <param name="kind"> The kind of entries to search. </param>
        /// <param name="query"> The title to search for. </param>
        /// <param name="limit"> The most results to return, within <see cref="SearchLimits"/>. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct);

        /// <summary>
        /// Returns the Anime with the passed MAL id, or null if it does not exist.
        /// </summary>
        /// <param name="malId"> The MAL id of the Anime. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct);

        /// <summary>
        /// Returns the Manga with the passed MAL id, or null if it does not exist.
        /// </summary>
        /// <param name="malId"> The MAL id of the Manga. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct);
    }
}
