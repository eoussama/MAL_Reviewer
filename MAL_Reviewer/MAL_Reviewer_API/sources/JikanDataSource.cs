using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// Fetches Anime and Manga information from the Jikan v4 API (https://jikan.moe).
    /// </summary>
    internal class JikanDataSource : IAnimeDataSource
    {
        #region Fields

        private readonly ApiHttp http;
        private readonly string baseUrl;

        #endregion

        #region Properties

        public string Name => "Jikan";

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="http"> The transport bound to Jikan. </param>
        /// <param name="baseUrl"> The base URL of the Jikan v4 API. </param>
        public JikanDataSource(ApiHttp http, string baseUrl)
        {
            this.http = http;
            this.baseUrl = baseUrl;
        }

        #endregion

        #region Public methods

        public async Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<SearchResultsModel>();
            }

            string url = ApiOptions.Combine(this.baseUrl, $"{ kind.ToPathSegment() }?q={ Uri.EscapeDataString(query.Trim()) }&limit={ SearchLimits.Clamp(limit) }");
            JikanEnvelope<JikanEntry[]> envelope = await this.http.GetAsync<JikanEnvelope<JikanEntry[]>>(url, ct);

            return (envelope?.Data ?? Array.Empty<JikanEntry>())
                .Select(entry => new SearchResultsModel
                {
                    Mal_id = entry.MalId,
                    Title = entry.Title,
                    Type = entry.Type,
                    Image_url = entry.Images?.Jpg?.ImageUrl
                })
                .ToArray();
        }

        public async Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct)
        {
            JikanEntry entry = await GetEntryAsync(MediaKind.Anime, malId, ct);

            if (entry == null)
            {
                return null;
            }

            AnimeModel anime = Fill(new AnimeModel(), entry);
            anime.Airing = entry.Airing ?? false;
            anime.Episodes = entry.Episodes;

            return anime;
        }

        public async Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct)
        {
            JikanEntry entry = await GetEntryAsync(MediaKind.Manga, malId, ct);

            if (entry == null)
            {
                return null;
            }

            MangaModel manga = Fill(new MangaModel(), entry);
            manga.Publishing = entry.Publishing ?? false;
            manga.Volumes = entry.Volumes;
            manga.Chapters = entry.Chapters;

            return manga;
        }

        #endregion

        #region Private methods

        private async Task<JikanEntry> GetEntryAsync(MediaKind kind, int malId, CancellationToken ct)
        {
            string url = ApiOptions.Combine(this.baseUrl, $"{ kind.ToPathSegment() }/{ malId }");
            JikanEnvelope<JikanEntry> envelope = await this.http.GetAsync<JikanEnvelope<JikanEntry>>(url, ct);

            return envelope?.Data;
        }

        private static T Fill<T>(T target, JikanEntry entry) where T : TargetModel
        {
            target.Mal_id = entry.MalId;
            target.Url = entry.Url;
            target.Image_url = entry.Images?.Jpg?.ImageUrl;
            target.Title = entry.Title;
            target.Type = entry.Type;
            target.Score = entry.Score;
            target.Rank = entry.Rank;
            target.Synopsis = entry.Synopsis;

            return target;
        }

        #endregion
    }

    #region Jikan DTOs

    internal class JikanEnvelope<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    internal class JikanImages
    {
        [JsonPropertyName("jpg")]
        public JikanImage Jpg { get; set; }
    }

    internal class JikanImage
    {
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }

    internal class JikanEntry
    {
        [JsonPropertyName("mal_id")]
        public int MalId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("images")]
        public JikanImages Images { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("score")]
        public float? Score { get; set; }

        [JsonPropertyName("rank")]
        public int? Rank { get; set; }

        [JsonPropertyName("synopsis")]
        public string Synopsis { get; set; }

        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        [JsonPropertyName("airing")]
        public bool? Airing { get; set; }

        [JsonPropertyName("chapters")]
        public int? Chapters { get; set; }

        [JsonPropertyName("volumes")]
        public int? Volumes { get; set; }

        [JsonPropertyName("publishing")]
        public bool? Publishing { get; set; }
    }

    #endregion
}
