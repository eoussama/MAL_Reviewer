namespace MAL_Reviewer_API.models.UserModels
{
    /// <summary>
    /// The Manga stats of the user.
    /// </summary>
    public class MangaStatsModel : TargetStatsModel
    {
        /// <summary>
        /// The time spent reading Manga in days by the user.
        /// </summary>
        public double? Days_read { get; set; }

        /// <summary>
        /// The total number of Manga the user is currently reading.
        /// </summary>
        public int? Reading { get; set; }

        /// <summary>
        /// The total number of Manga the user plans to read.
        /// </summary>
        public int? Plan_to_read { get; set; }

        /// <summary>
        /// The total number of rereads of the user.
        /// </summary>
        public int? Reread { get; set; }

        /// <summary>
        /// The total number of chapters read by the user.
        /// </summary>
        public int? Chapters_read { get; set; }

        /// <summary>
        /// The total number of volumes read by the user.
        /// </summary>
        public int? Volumes_read { get; set; }
    }
}
