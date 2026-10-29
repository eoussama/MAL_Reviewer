using System;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// The bounds of the number of search results asked from the data sources.
    /// </summary>
    public static class SearchLimits
    {
        /// <summary>
        /// The number of results asked when none is given.
        /// </summary>
        public const int Default = 10;

        /// <summary>
        /// The fewest results that can be asked.
        /// </summary>
        public const int Min = 1;

        /// <summary>
        /// The most results that can be asked; Jikan serves at most 25 per page.
        /// </summary>
        public const int Max = 25;

        /// <summary>
        /// Keeps a requested number of results within the bounds.
        /// </summary>
        /// <param name="limit"> The requested number of results. </param>
        public static int Clamp(int limit) => Math.Clamp(limit, Min, Max);
    }
}
