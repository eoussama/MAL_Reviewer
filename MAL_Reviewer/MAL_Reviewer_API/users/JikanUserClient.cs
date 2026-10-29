using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.sources;

namespace MAL_Reviewer_API.users
{
    /// <summary>
    /// Fetches MyAnimeList user profiles, statistics and favorites from Jikan v4.
    /// </summary>
    internal class JikanUserClient
    {
        #region Fields

        private readonly ApiHttp http;
        private readonly string baseUrl;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="http"> The transport bound to Jikan. </param>
        /// <param name="baseUrl"> The base URL of the Jikan v4 API. </param>
        public JikanUserClient(ApiHttp http, string baseUrl)
        {
            this.http = http;
            this.baseUrl = baseUrl;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the profile of the passed MAL user, or null if the user does not exist.
        /// The Anime and Manga lists are not filled here.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        public async Task<MALUserModel> GetUserAsync(string username, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            string user = Uri.EscapeDataString(username.Trim());
            JikanEnvelope<JikanUser> envelope = await this.http.GetAsync<JikanEnvelope<JikanUser>>(ApiOptions.Combine(this.baseUrl, $"users/{ user }/full"), ct);
            JikanUser profile = envelope?.Data;

            if (profile == null)
            {
                return null;
            }

            // Older and newer Jikan versions differ on whether the full profile embeds the favorites.
            JikanFavorites favorites = profile.Favorites;

            if (favorites == null)
            {
                JikanEnvelope<JikanFavorites> favoritesEnvelope = await this.http.GetAsync<JikanEnvelope<JikanFavorites>>(ApiOptions.Combine(this.baseUrl, $"users/{ user }/favorites"), ct);
                favorites = favoritesEnvelope?.Data;
            }

            return new MALUserModel
            {
                Username = profile.Username,
                Url = profile.Url,
                Image_url = profile.Images?.Jpg?.ImageUrl,
                Gender = profile.Gender,
                Location = profile.Location,
                About = profile.About,
                Birthday = profile.Birthday?.DateTime,
                Joined = profile.Joined?.DateTime,
                Anime_stats = MapAnimeStats(profile.Statistics?.Anime),
                Manga_stats = MapMangaStats(profile.Statistics?.Manga),
                Favorites = MapFavorites(favorites)
            };
        }

        #endregion

        #region Private methods

        private static AnimeStatsModel MapAnimeStats(JikanAnimeStats stats)
        {
            if (stats == null)
            {
                return new AnimeStatsModel();
            }

            return new AnimeStatsModel
            {
                Days_watched = stats.DaysWatched,
                Mean_score = stats.MeanScore,
                Watching = stats.Watching,
                Completed = stats.Completed,
                On_hold = stats.OnHold,
                Dropped = stats.Dropped,
                Plan_to_watch = stats.PlanToWatch,
                Total_entries = stats.TotalEntries,
                Rewatched = stats.Rewatched,
                Episodes_watched = stats.EpisodesWatched
            };
        }

        private static MangaStatsModel MapMangaStats(JikanMangaStats stats)
        {
            if (stats == null)
            {
                return new MangaStatsModel();
            }

            return new MangaStatsModel
            {
                Days_read = stats.DaysRead,
                Mean_score = stats.MeanScore,
                Reading = stats.Reading,
                Completed = stats.Completed,
                On_hold = stats.OnHold,
                Dropped = stats.Dropped,
                Plan_to_read = stats.PlanToRead,
                Total_entries = stats.TotalEntries,
                Reread = stats.Reread,
                Chapters_read = stats.ChaptersRead,
                Volumes_read = stats.VolumesRead
            };
        }

        private static FavoritesModel MapFavorites(JikanFavorites favorites)
        {
            return new FavoritesModel
            {
                Anime = Map<FavAnimeModel>(favorites?.Anime),
                Manga = Map<FavMangaModel>(favorites?.Manga),
                Characters = Map<FavCharactersModel>(favorites?.Characters),
                People = Map<FavPeopleModel>(favorites?.People)
            };
        }

        private static T[] Map<T>(JikanFavorite[] favorites) where T : FavTargetModel, new()
        {
            return (favorites ?? Array.Empty<JikanFavorite>())
                .Select(favorite => new T
                {
                    Mal_id = favorite.MalId,
                    Url = favorite.Url,
                    Image_url = favorite.Images?.Jpg?.ImageUrl,
                    // Anime and Manga have a title, characters and people have a name.
                    Name = favorite.Title ?? favorite.Name
                })
                .ToArray();
        }

        #endregion
    }

    #region Jikan user DTOs

    internal class JikanUser
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("images")]
        public JikanImages Images { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("about")]
        public string About { get; set; }

        [JsonPropertyName("birthday")]
        public DateTimeOffset? Birthday { get; set; }

        [JsonPropertyName("joined")]
        public DateTimeOffset? Joined { get; set; }

        [JsonPropertyName("statistics")]
        public JikanStatistics Statistics { get; set; }

        [JsonPropertyName("favorites")]
        public JikanFavorites Favorites { get; set; }
    }

    internal class JikanStatistics
    {
        [JsonPropertyName("anime")]
        public JikanAnimeStats Anime { get; set; }

        [JsonPropertyName("manga")]
        public JikanMangaStats Manga { get; set; }
    }

    internal class JikanTargetStats
    {
        [JsonPropertyName("mean_score")]
        public double? MeanScore { get; set; }

        [JsonPropertyName("completed")]
        public int? Completed { get; set; }

        [JsonPropertyName("on_hold")]
        public int? OnHold { get; set; }

        [JsonPropertyName("dropped")]
        public int? Dropped { get; set; }

        [JsonPropertyName("total_entries")]
        public int? TotalEntries { get; set; }
    }

    internal class JikanAnimeStats : JikanTargetStats
    {
        [JsonPropertyName("days_watched")]
        public double? DaysWatched { get; set; }

        [JsonPropertyName("watching")]
        public int? Watching { get; set; }

        [JsonPropertyName("plan_to_watch")]
        public int? PlanToWatch { get; set; }

        [JsonPropertyName("rewatched")]
        public int? Rewatched { get; set; }

        [JsonPropertyName("episodes_watched")]
        public int? EpisodesWatched { get; set; }
    }

    internal class JikanMangaStats : JikanTargetStats
    {
        [JsonPropertyName("days_read")]
        public double? DaysRead { get; set; }

        [JsonPropertyName("reading")]
        public int? Reading { get; set; }

        [JsonPropertyName("plan_to_read")]
        public int? PlanToRead { get; set; }

        [JsonPropertyName("reread")]
        public int? Reread { get; set; }

        [JsonPropertyName("chapters_read")]
        public int? ChaptersRead { get; set; }

        [JsonPropertyName("volumes_read")]
        public int? VolumesRead { get; set; }
    }

    internal class JikanFavorites
    {
        [JsonPropertyName("anime")]
        public JikanFavorite[] Anime { get; set; }

        [JsonPropertyName("manga")]
        public JikanFavorite[] Manga { get; set; }

        [JsonPropertyName("characters")]
        public JikanFavorite[] Characters { get; set; }

        [JsonPropertyName("people")]
        public JikanFavorite[] People { get; set; }
    }

    internal class JikanFavorite
    {
        [JsonPropertyName("mal_id")]
        public int? MalId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("images")]
        public JikanImages Images { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    #endregion
}
