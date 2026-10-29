namespace MAL_Reviewer_API.models.TargetModels
{
    /// <summary>
    /// The information regarding an individual Manga.
    /// </summary>
    public class MangaModel : TargetModel
    {
        /// <summary>
        /// The publishing status of the Manga.
        /// </summary>
        public bool Publishing { get; set; }

        /// <summary>
        /// The number of volumes in the Manga.
        /// </summary>
        public int? Volumes { get; set; }

        /// <summary>
        /// The number of chapters in the Manga.
        /// </summary>
        public int? Chapters { get; set; }
    }
}
