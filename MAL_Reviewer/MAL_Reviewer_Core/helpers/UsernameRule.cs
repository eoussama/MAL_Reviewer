using System.Text.RegularExpressions;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// The rules of MyAnimeList usernames: 2 to 16 letters, digits, underscores or hyphens.
    /// </summary>
    public static class UsernameRule
    {
        #region Fields

        /// <summary>
        /// The shortest username allowed.
        /// </summary>
        public const int MinLength = 2;

        /// <summary>
        /// The longest username allowed.
        /// </summary>
        public const int MaxLength = 16;

        /// <summary>
        /// The message shown for invalid usernames.
        /// </summary>
        public const string InvalidMessage = "Please input a valid MAL username (2 to 16 letters, digits, “_” or “-”).";

        private static readonly Regex Pattern = new Regex($"^[A-Za-z0-9_-]{{{ MinLength },{ MaxLength }}}$", RegexOptions.Compiled);

        #endregion

        #region Public methods

        /// <summary>
        /// Removes the surrounding spaces of a username.
        /// </summary>
        /// <param name="username"> The raw username. </param>
        public static string Normalize(string username)
        {
            return username?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Whether the passed text is a valid MyAnimeList username.
        /// </summary>
        /// <param name="username"> The username to check, already normalized. </param>
        public static bool IsValid(string username)
        {
            return username != null && Pattern.IsMatch(username);
        }

        #endregion
    }
}
