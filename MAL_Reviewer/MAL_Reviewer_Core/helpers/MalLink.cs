using System.Globalization;
using System.Text.RegularExpressions;
using MAL_Reviewer_API.sources;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Reads MyAnimeList Anime and Manga links, (ex: https://myanimelist.net/anime/1/Cowboy_Bebop).
    /// </summary>
    public static class MalLink
    {
        #region Fields

        /// <summary>
        /// An example of a link, shown to the user.
        /// </summary>
        public const string Example = "https://myanimelist.net/anime/1/Cowboy_Bebop";

        /// <summary>
        /// Matches the current links (/anime/1/Title) and the old ones (/anime.php?id=1),
        /// with or without the scheme and www, followed by anything.
        /// </summary>
        private static readonly Regex Pattern = new Regex(
            @"^(?:https?://)?(?:www\.)?myanimelist\.net/(?<kind>anime|manga)(?:/(?<id>\d+)|\.php\?id=(?<id>\d+))(?:[/?#&].*)?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        #endregion

        #region Public methods

        /// <summary>
        /// Reads the kind and the MAL id of an Anime or Manga link.
        /// </summary>
        /// <param name="text"> The text that may be a link; surrounding spaces are ignored. </param>
        /// <param name="kind"> Whether the link is of an Anime or a Manga. </param>
        /// <param name="malId"> The MAL id of the entry. </param>
        /// <returns> Whether the text is an Anime or Manga link. </returns>
        public static bool TryParse(string text, out MediaKind kind, out int malId)
        {
            kind = MediaKind.Anime;
            malId = 0;

            Match match = Pattern.Match(text?.Trim() ?? string.Empty);

            if (!match.Success || !int.TryParse(match.Groups["id"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out malId) || malId <= 0)
            {
                malId = 0;
                return false;
            }

            kind = MediaKindExtensions.Parse(match.Groups["kind"].Value);

            return true;
        }

        #endregion
    }
}
