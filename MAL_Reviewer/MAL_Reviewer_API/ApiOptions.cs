using System;

namespace MAL_Reviewer_API
{
    /// <summary>
    /// The endpoints and network settings used to talk to the remote data sources.
    /// </summary>
    public class ApiOptions
    {
        #region Fields

        /// <summary>
        /// The environment variable that overrides the Jikan base URL.
        /// </summary>
        public const string JikanUrlVariable = "MAL_REVIEWER_JIKAN_URL";

        /// <summary>
        /// The environment variable that overrides the AniList GraphQL URL.
        /// </summary>
        public const string AniListUrlVariable = "MAL_REVIEWER_ANILIST_URL";

        #endregion

        #region Properties

        /// <summary>
        /// The base URL of the Jikan v4 API.
        /// </summary>
        public string JikanUrl { get; set; } = "https://api.jikan.moe/v4/";

        /// <summary>
        /// The URL of the AniList GraphQL API.
        /// </summary>
        public string AniListUrl { get; set; } = "https://graphql.anilist.co/";

        /// <summary>
        /// The base URL of MyAnimeList, used for the public list endpoints.
        /// </summary>
        public string MalUrl { get; set; } = "https://myanimelist.net/";

        /// <summary>
        /// The time allowed for a single request.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// How many times a rate limited (429) or busy (503) request is retried.
        /// </summary>
        public int MaxRetries { get; set; } = 2;

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the default options, overridden by the environment variables when they are set.
        /// </summary>
        public static ApiOptions FromEnvironment()
        {
            ApiOptions options = new ApiOptions();

            string jikanUrl = Environment.GetEnvironmentVariable(JikanUrlVariable);
            string aniListUrl = Environment.GetEnvironmentVariable(AniListUrlVariable);

            if (!string.IsNullOrWhiteSpace(jikanUrl))
            {
                options.JikanUrl = jikanUrl;
            }

            if (!string.IsNullOrWhiteSpace(aniListUrl))
            {
                options.AniListUrl = aniListUrl;
            }

            return options;
        }

        /// <summary>
        /// Joins a base URL and a relative path, regardless of trailing or leading slashes.
        /// </summary>
        /// <param name="baseUrl"> The base URL. </param>
        /// <param name="path"> The relative path, with its query string if any. </param>
        public static string Combine(string baseUrl, string path)
        {
            return $"{ baseUrl.TrimEnd('/') }/{ path.TrimStart('/') }";
        }

        #endregion
    }
}
