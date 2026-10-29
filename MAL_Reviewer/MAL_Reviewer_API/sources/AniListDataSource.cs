using System;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// Fetches Anime and Manga information from the AniList GraphQL API (https://anilist.co),
    /// mapped back to MyAnimeList ids. Entries without a MAL id are skipped.
    /// </summary>
    internal class AniListDataSource : IAnimeDataSource
    {
        #region Fields

        internal const string SearchQuery =
            "query ($search: String, $type: MediaType, $perPage: Int) { Page(perPage: $perPage) { media(search: $search, type: $type, sort: SEARCH_MATCH) { idMal title { romaji english } format coverImage { medium large } } } }";

        internal const string DetailsQuery =
            "query ($idMal: Int, $type: MediaType) { Media(idMal: $idMal, type: $type) { idMal title { romaji english } format status episodes chapters volumes averageScore description(asHtml: false) coverImage { large } } }";

        private static readonly Regex LineBreaks = new Regex(@"<br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Tags = new Regex(@"<[^>]+>", RegexOptions.Compiled);

        private readonly ApiHttp http;
        private readonly string url;
        private readonly string malUrl;

        #endregion

        #region Properties

        public string Name => "AniList";

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="http"> The transport bound to AniList. </param>
        /// <param name="url"> The URL of the AniList GraphQL API. </param>
        /// <param name="malUrl"> The base URL of MyAnimeList, used to build entry links. </param>
        public AniListDataSource(ApiHttp http, string url, string malUrl)
        {
            this.http = http;
            this.url = url;
            this.malUrl = malUrl;
        }

        #endregion

        #region Public methods

        public async Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<SearchResultsModel>();
            }

            object body = new
            {
                query = SearchQuery,
                variables = new { search = query.Trim(), type = ToAniListType(kind), perPage = SearchLimits.Clamp(limit) }
            };

            AniListResponse<AniListPageData> response = await this.http.PostAsync<AniListResponse<AniListPageData>>(this.url, body, ct);

            return (response?.Data?.Page?.Media ?? Array.Empty<AniListMedia>())
                .Where(media => media.IdMal.HasValue)
                .Select(media => new SearchResultsModel
                {
                    Mal_id = media.IdMal.Value,
                    Title = media.Title?.Romaji ?? media.Title?.English,
                    Type = ToMalType(media.Format),
                    // Search cards are small, so the lighter cover is enough.
                    Image_url = media.CoverImage?.Medium ?? media.CoverImage?.Large
                })
                .ToArray();
        }

        public async Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct)
        {
            AniListMedia media = await GetMediaAsync(MediaKind.Anime, malId, ct);

            if (media == null)
            {
                return null;
            }

            AnimeModel anime = Fill(new AnimeModel(), media, MediaKind.Anime, malId);
            anime.Airing = media.Status == "RELEASING";
            anime.Episodes = media.Episodes;

            return anime;
        }

        public async Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct)
        {
            AniListMedia media = await GetMediaAsync(MediaKind.Manga, malId, ct);

            if (media == null)
            {
                return null;
            }

            MangaModel manga = Fill(new MangaModel(), media, MediaKind.Manga, malId);
            manga.Publishing = media.Status == "RELEASING";
            manga.Volumes = media.Volumes;
            manga.Chapters = media.Chapters;

            return manga;
        }

        /// <summary>
        /// Converts an AniList format to the type names MyAnimeList uses.
        /// </summary>
        /// <param name="format"> The AniList format, (ex: TV_SHORT, ONE_SHOT). </param>
        internal static string ToMalType(string format)
        {
            switch (format)
            {
                case "TV":
                case "TV_SHORT": return "TV";
                case "MOVIE": return "Movie";
                case "SPECIAL": return "Special";
                case "OVA": return "OVA";
                case "ONA": return "ONA";
                case "MUSIC": return "Music";
                case "MANGA": return "Manga";
                case "NOVEL": return "Light Novel";
                case "ONE_SHOT": return "One-shot";
                default: return format;
            }
        }

        /// <summary>
        /// Turns an AniList description into plain text.
        /// </summary>
        /// <param name="description"> The description, which may contain HTML. </param>
        internal static string ToPlainText(string description)
        {
            if (description == null)
            {
                return null;
            }

            string text = LineBreaks.Replace(description, "\n");
            text = Tags.Replace(text, string.Empty);
            text = WebUtility.HtmlDecode(text);

            // AniList usually follows a <br> with its own newline.
            return Regex.Replace(text, @"\n{3,}", "\n\n").Trim();
        }

        #endregion

        #region Private methods

        private async Task<AniListMedia> GetMediaAsync(MediaKind kind, int malId, CancellationToken ct)
        {
            object body = new
            {
                query = DetailsQuery,
                variables = new { idMal = malId, type = ToAniListType(kind) }
            };

            AniListResponse<AniListMediaData> response = await this.http.PostAsync<AniListResponse<AniListMediaData>>(this.url, body, ct);

            return response?.Data?.Media;
        }

        private T Fill<T>(T target, AniListMedia media, MediaKind kind, int malId) where T : TargetModel
        {
            target.Mal_id = media.IdMal ?? malId;
            target.Url = ApiOptions.Combine(this.malUrl, $"{ kind.ToPathSegment() }/{ target.Mal_id }");
            target.Image_url = media.CoverImage?.Large;
            target.Title = media.Title?.Romaji ?? media.Title?.English;
            target.Type = ToMalType(media.Format);
            target.Score = media.AverageScore.HasValue ? media.AverageScore.Value / 10f : (float?)null;
            target.Rank = null;
            target.Synopsis = ToPlainText(media.Description);

            return target;
        }

        private static string ToAniListType(MediaKind kind)
        {
            return kind == MediaKind.Anime ? "ANIME" : "MANGA";
        }

        #endregion
    }

    #region AniList DTOs

    internal class AniListResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    internal class AniListPageData
    {
        [JsonPropertyName("Page")]
        public AniListPage Page { get; set; }
    }

    internal class AniListPage
    {
        [JsonPropertyName("media")]
        public AniListMedia[] Media { get; set; }
    }

    internal class AniListMediaData
    {
        [JsonPropertyName("Media")]
        public AniListMedia Media { get; set; }
    }

    internal class AniListTitle
    {
        [JsonPropertyName("romaji")]
        public string Romaji { get; set; }

        [JsonPropertyName("english")]
        public string English { get; set; }
    }

    internal class AniListCover
    {
        [JsonPropertyName("medium")]
        public string Medium { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }
    }

    internal class AniListMedia
    {
        [JsonPropertyName("idMal")]
        public int? IdMal { get; set; }

        [JsonPropertyName("title")]
        public AniListTitle Title { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        [JsonPropertyName("chapters")]
        public int? Chapters { get; set; }

        [JsonPropertyName("volumes")]
        public int? Volumes { get; set; }

        [JsonPropertyName("averageScore")]
        public int? AverageScore { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("coverImage")]
        public AniListCover CoverImage { get; set; }
    }

    #endregion
}
