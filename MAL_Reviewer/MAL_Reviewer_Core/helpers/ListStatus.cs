using System;
using System.Collections.Generic;
using System.Linq;
using MAL_Reviewer_Core.enumerations;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// The statuses of MyAnimeList list entries, in the order the lists are displayed.
    /// </summary>
    public static class ListStatus
    {
        #region Fields

        /// <summary>
        /// The MyAnimeList status codes: watching/reading, completed, on hold, dropped, plan to watch/read.
        /// (MyAnimeList does not use 5.)
        /// </summary>
        public static readonly IReadOnlyList<short> Codes = new short[] { 1, 2, 3, 4, 6 };

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the display names of the lists, in the order of <see cref="Codes"/>.
        /// </summary>
        /// <param name="type"> The type of the list. </param>
        public static IReadOnlyList<string> Labels(TargetType type)
        {
            return type == TargetType.Anime
                ? new[] { "Watching", "Completed", "On Hold", "Dropped", "Plan to Watch" }
                : new[] { "Reading", "Completed", "On Hold", "Dropped", "Plan to Read" };
        }

        /// <summary>
        /// Splits entries into one list per status, in the order of <see cref="Codes"/>.
        /// Entries with an unknown status are left out.
        /// </summary>
        /// <param name="entries"> The entries to split; null is treated as empty. </param>
        /// <param name="status"> Reads the status code of an entry. </param>
        public static List<T>[] Split<T>(IEnumerable<T> entries, Func<T, short?> status)
        {
            List<T>[] buckets = Codes.Select(_ => new List<T>()).ToArray();

            foreach (T entry in entries ?? Enumerable.Empty<T>())
            {
                short? code = status(entry);
                int index = code.HasValue ? IndexOf(code.Value) : -1;

                if (index >= 0)
                {
                    buckets[index].Add(entry);
                }
            }

            return buckets;
        }

        #endregion

        #region Private methods

        private static int IndexOf(short code)
        {
            for (int i = 0; i < Codes.Count; i++)
            {
                if (Codes[i] == code)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
