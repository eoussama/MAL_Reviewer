namespace MAL_Reviewer_API.models.TargetModels
{
    /// <summary>
    /// The information regarding an individual Anime.
    /// </summary>
    public class AnimeModel : TargetModel
    {
        /// <summary>
        /// The airing status of an Anime.
        /// </summary>
        public bool Airing { get; set; }

        /// <summary>
        /// The number of episodes in an Anime.
        /// </summary>
        public int? Episodes { get; set; }
    }
}
