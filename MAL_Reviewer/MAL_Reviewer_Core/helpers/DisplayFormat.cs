using System;
using System.Globalization;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Turns data into the text shown by the interface.
    /// </summary>
    public static class DisplayFormat
    {
        #region Fields

        /// <summary>
        /// Shown in place of unknown values.
        /// </summary>
        public const string Unknown = "?";

        /// <summary>
        /// Shown when a user has no "about" text.
        /// </summary>
        public const string EmptyAbout = "Such empty!";

        /// <summary>
        /// Appended to shortened texts.
        /// </summary>
        public const string Ellipsis = "...";

        #endregion

        #region Public methods

        /// <summary>
        /// Shortens a text to the passed length, adding an ellipsis when it is cut.
        /// </summary>
        /// <param name="text"> The text to shorten; null gives an empty text. </param>
        /// <param name="maxLength"> The number of characters kept before the ellipsis. </param>
        public static string Truncate(string text, int maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Length > maxLength ? text.Substring(0, maxLength) + Ellipsis : text;
        }

        /// <summary>
        /// Formats the progress of an entry, (ex: 5/12, or 5/? when the total is unknown).
        /// </summary>
        /// <param name="done"> The watched episodes or read chapters/volumes. </param>
        /// <param name="total"> The total, null or 0 when unknown. </param>
        public static string Progress(int? done, int? total)
        {
            string totalText = total.HasValue && total.Value > 0 ? total.Value.ToString(CultureInfo.CurrentCulture) : Unknown;

            return $"{ (done ?? 0).ToString(CultureInfo.CurrentCulture) }/{ totalText }";
        }

        /// <summary>
        /// Formats a count, or "?" when it is unknown. Zero is a real value and shown as such.
        /// </summary>
        /// <param name="count"> The count. </param>
        public static string Count(int? count)
        {
            return count?.ToString(CultureInfo.CurrentCulture) ?? Unknown;
        }

        /// <summary>
        /// Formats a decimal statistic with two decimals, or "?" when it is unknown.
        /// </summary>
        /// <param name="value"> The value, (ex: mean score, days watched). </param>
        public static string Decimal(double? value)
        {
            return value?.ToString("0.00", CultureInfo.CurrentCulture) ?? Unknown;
        }

        /// <summary>
        /// Returns the text, or "?" when it is empty.
        /// </summary>
        /// <param name="text"> The text. </param>
        public static string OrUnknown(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? Unknown : text;
        }

        /// <summary>
        /// Returns the user's "about" text, or a placeholder when it is empty.
        /// </summary>
        /// <param name="about"> The "about" text. </param>
        public static string About(string about)
        {
            return string.IsNullOrWhiteSpace(about) ? EmptyAbout : about;
        }

        /// <summary>
        /// Formats a date in the long format, or "?" when it is unknown.
        /// </summary>
        /// <param name="date"> The date. </param>
        public static string LongDate(DateTime? date)
        {
            return date?.ToString("D", CultureInfo.CurrentCulture) ?? Unknown;
        }

        /// <summary>
        /// The status of an Anime.
        /// </summary>
        /// <param name="airing"> Whether the Anime is airing. </param>
        public static string AnimeStatus(bool airing) => airing ? "Airing" : "Finished";

        /// <summary>
        /// The status of a Manga.
        /// </summary>
        /// <param name="publishing"> Whether the Manga is publishing. </param>
        public static string MangaStatus(bool publishing) => publishing ? "Publishing" : "Finished";

        /// <summary>
        /// The tooltip of a list state, (ex: Eoussama's Anime list is public.).
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="cardTitle"> The title of the card, whose first word names the list, (ex: Anime Stats). </param>
        /// <param name="state"> The state of the list, (ex: Public). </param>
        public static string ListStateTooltip(string username, string cardTitle, string state)
        {
            string list = (cardTitle ?? string.Empty).Split(' ')[0];

            return $"{ username }'s { list } list is { (state ?? string.Empty).ToLowerInvariant() }.";
        }

        #endregion
    }
}
