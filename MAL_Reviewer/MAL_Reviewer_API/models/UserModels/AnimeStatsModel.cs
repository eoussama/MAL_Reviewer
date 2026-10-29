namespace MAL_Reviewer_API.models.UserModels
{
    /// <summary>
    /// The Anime stats of the user.
    /// </summary>
    public class AnimeStatsModel : TargetStatsModel
    {
        /// <summary>
        /// The time spent watching Anime in days by the user.
        /// </summary>
        public double? Days_watched { get; set; }

        /// <summary>
        /// The total number of Anime the user is currently watching.
        /// </summary>
        public int? Watching { get; set; }

        /// <summary>
        /// The total number of Anime the user plans to watch.
        /// </summary>
        public int? Plan_to_watch { get; set; }

        /// <summary>
        /// The total number of rewatches of the user.
        /// </summary>
        public int? Rewatched { get; set; }

        /// <summary>
        /// The total number of watched episodes by the user.
        /// </summary>
        public int? Episodes_watched { get; set; }
    }
}
