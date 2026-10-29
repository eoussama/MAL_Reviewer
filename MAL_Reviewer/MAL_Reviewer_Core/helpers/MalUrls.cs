using System;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Builds links to MyAnimeList pages.
    /// </summary>
    public static class MalUrls
    {
        #region Fields

        /// <summary>
        /// The MyAnimeList website.
        /// </summary>
        public const string Base = "https://myanimelist.net/";

        /// <summary>
        /// The account creation page.
        /// </summary>
        public const string Register = Base + "register.php";

        #endregion

        #region Public methods

        /// <summary>
        /// The user's profile page.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        public static string Profile(string username) => $"{ Base }profile/{ Encode(username) }";

        /// <summary>
        /// The user's friends page.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        public static string Friends(string username) => $"{ Profile(username) }/friends";

        /// <summary>
        /// The user's Anime list page.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        public static string AnimeList(string username) => $"{ Base }animelist/{ Encode(username) }";

        /// <summary>
        /// The user's Manga list page.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        public static string MangaList(string username) => $"{ Base }mangalist/{ Encode(username) }";

        #endregion

        #region Private methods

        private static string Encode(string username) => Uri.EscapeDataString(username?.Trim() ?? string.Empty);

        #endregion
    }
}
