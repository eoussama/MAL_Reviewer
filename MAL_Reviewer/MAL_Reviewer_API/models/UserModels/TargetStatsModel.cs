namespace MAL_Reviewer_API.models.UserModels
{
    /// <summary>
    /// The Anime/Manga stats of the user.
    /// </summary>
    public class TargetStatsModel
    {
        /// <summary>
        /// The average Anime/Manga score of a user.
        /// </summary>
        public double? Mean_score { get; set; }

        /// <summary>
        /// The number of completed Anime/Manga of a user.
        /// </summary>
        public int? Completed { get; set; }

        /// <summary>
        /// The number of on hold Anime/Manga of a user.
        /// </summary>
        public int? On_hold { get; set; }

        /// <summary>
        /// The number of dropped Anime/Manga of a user.
        /// </summary>
        public int? Dropped { get; set; }

        /// <summary>
        /// The number Anime/Manga of a user.
        /// </summary>
        public int? Total_entries { get; set; }
    }
}
