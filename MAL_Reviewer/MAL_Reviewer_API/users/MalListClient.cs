using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using MAL_Reviewer_API.models.ListEntryModels;

namespace MAL_Reviewer_API.users
{
    /// <summary>
    /// Reads public Anime and Manga lists straight from MyAnimeList's list pages,
    /// which need no API key. (Jikan discontinued its user list endpoints in 2022.)
    /// </summary>
    internal class MalListClient
    {
        #region Fields

        /// <summary>
        /// The number of entries MyAnimeList returns per page.
        /// </summary>
        internal const int PageSize = 300;

        /// <summary>
        /// A safety net against endless paging; 100 pages are 30,000 entries.
        /// </summary>
        internal const int MaxPages = 100;

        /// <summary>
        /// The "all statuses" filter of the list pages.
        /// </summary>
        private const int AllStatuses = 7;

        private readonly ApiHttp http;
        private readonly string malUrl;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="http"> The transport bound to MyAnimeList. </param>
        /// <param name="malUrl"> The base URL of MyAnimeList. </param>
        public MalListClient(ApiHttp http, string malUrl)
        {
            this.http = http;
            this.malUrl = malUrl;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the Anime list of the passed user, or null if the user does not exist or the list is private.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        public async Task<List<AnimelistEntryModel>> GetAnimeListAsync(string username, CancellationToken ct)
        {
            List<MalAnimeEntry> entries = await GetAllPagesAsync<MalAnimeEntry>("animelist", username, ct);

            return entries?.ConvertAll(entry => new AnimelistEntryModel
            {
                Mal_id = entry.Id,
                Title = entry.Title,
                Url = ToAbsoluteUrl(entry.Url),
                Image_url = entry.ImagePath,
                Type = entry.MediaType,
                Score = ToScore(entry.Score),
                Tags = entry.Tags,
                Watching_status = ToShort(entry.Status),
                Watched_episodes = ToShort(entry.WatchedEpisodes),
                Total_episodes = ToKnownCount(entry.TotalEpisodes)
            });
        }

        /// <summary>
        /// Returns the Manga list of the passed user, or null if the user does not exist or the list is private.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        public async Task<List<MangalistEntryModel>> GetMangaListAsync(string username, CancellationToken ct)
        {
            List<MalMangaEntry> entries = await GetAllPagesAsync<MalMangaEntry>("mangalist", username, ct);

            return entries?.ConvertAll(entry => new MangalistEntryModel
            {
                Mal_id = entry.Id,
                Title = entry.Title,
                Url = ToAbsoluteUrl(entry.Url),
                Image_url = entry.ImagePath,
                Type = entry.MediaType,
                Score = ToScore(entry.Score),
                Tags = entry.Tags,
                Reading_status = ToShort(entry.Status),
                Read_chapters = ToShort(entry.ReadChapters),
                Read_volumes = ToShort(entry.ReadVolumes),
                Total_chapters = ToKnownCount(entry.TotalChapters),
                Total_volumes = ToKnownCount(entry.TotalVolumes)
            });
        }

        #endregion

        #region Private methods

        private async Task<List<T>> GetAllPagesAsync<T>(string list, string username, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            string user = Uri.EscapeDataString(username.Trim());
            List<T> entries = new List<T>();

            for (int page = 0; page < MaxPages; page++)
            {
                string url = ApiOptions.Combine(this.malUrl, $"{ list }/{ user }/load.json?status={ AllStatuses }&offset={ page * PageSize }");

                // MyAnimeList answers 400 for unknown users and private lists.
                T[] batch = await this.http.GetAsync<T[]>(url, ct, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);

                if (batch == null)
                {
                    return page == 0 ? null : entries;
                }

                entries.AddRange(batch);

                if (batch.Length < PageSize)
                {
                    break;
                }
            }

            return entries;
        }

        private string ToAbsoluteUrl(string path)
        {
            if (string.IsNullOrEmpty(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return path;
            }

            return ApiOptions.Combine(this.malUrl, path);
        }

        /// <summary>
        /// MyAnimeList uses 0 for "not scored".
        /// </summary>
        internal static byte? ToScore(int? score)
        {
            return score.HasValue && score.Value > 0 && score.Value <= 10 ? (byte)score.Value : (byte?)null;
        }

        /// <summary>
        /// MyAnimeList uses 0 for "unknown" total counts, (ex: ongoing series).
        /// </summary>
        internal static short? ToKnownCount(int? count)
        {
            return count.HasValue && count.Value > 0 ? ToShort(count) : null;
        }

        internal static short? ToShort(int? value)
        {
            return value.HasValue ? (short)Math.Clamp(value.Value, short.MinValue, short.MaxValue) : (short?)null;
        }

        #endregion
    }

    #region MyAnimeList list DTOs

    internal class MalListEntry
    {
        [JsonPropertyName("tags")]
        [JsonConverter(typeof(TextConverter))]
        public string Tags { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("score")]
        public int? Score { get; set; }
    }

    internal class MalAnimeEntry : MalListEntry
    {
        [JsonPropertyName("anime_id")]
        public int? Id { get; set; }

        [JsonPropertyName("anime_title")]
        [JsonConverter(typeof(TextConverter))]
        public string Title { get; set; }

        [JsonPropertyName("anime_url")]
        public string Url { get; set; }

        [JsonPropertyName("anime_image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("anime_media_type_string")]
        public string MediaType { get; set; }

        [JsonPropertyName("num_watched_episodes")]
        public int? WatchedEpisodes { get; set; }

        [JsonPropertyName("anime_num_episodes")]
        public int? TotalEpisodes { get; set; }
    }

    internal class MalMangaEntry : MalListEntry
    {
        [JsonPropertyName("manga_id")]
        public int? Id { get; set; }

        [JsonPropertyName("manga_title")]
        [JsonConverter(typeof(TextConverter))]
        public string Title { get; set; }

        [JsonPropertyName("manga_url")]
        public string Url { get; set; }

        [JsonPropertyName("manga_image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("manga_media_type_string")]
        public string MediaType { get; set; }

        [JsonPropertyName("num_read_chapters")]
        public int? ReadChapters { get; set; }

        [JsonPropertyName("num_read_volumes")]
        public int? ReadVolumes { get; set; }

        [JsonPropertyName("manga_num_chapters")]
        public int? TotalChapters { get; set; }

        [JsonPropertyName("manga_num_volumes")]
        public int? TotalVolumes { get; set; }
    }

    /// <summary>
    /// Reads text that MyAnimeList may send as a number, (a title or tags made of digits only, like the Anime "86").
    /// </summary>
    internal class TextConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    return System.Text.Encoding.UTF8.GetString(reader.ValueSpan);
                default:
                    reader.Skip();
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    #endregion
}
