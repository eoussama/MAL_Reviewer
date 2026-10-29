namespace MAL_Reviewer_API.models.ListEntryModels
{
    /// <summary>
    /// The individual Manga information, (Part of the Manga list).
    /// </summary>
    public class MangalistEntryModel : ListEntryModel
    {
        /// <summary>
        /// The Manga's reading status, (ex; Reading, Completed, On Hold, Dropped, Plant to Read).
        /// </summary>
        public short? Reading_status { get; set; }

        /// <summary>
        /// The number of read chapters.
        /// </summary>
        public short? Read_chapters { get; set; }

        /// <summary>
        /// The number of read volumes.
        /// </summary>
        public short? Read_volumes { get; set; }

        /// <summary>
        /// The number of chapters in a given Manga.
        /// </summary>
        public short? Total_chapters { get; set; }

        /// <summary>
        /// The number of volumes in a given Manga.
        /// </summary>
        public short? Total_volumes { get; set; }
    }
}
