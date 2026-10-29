namespace MAL_Reviewer_API.models.ListEntryModels
{
    /// <summary>
    /// The individual Anime/Manga information, (Part of Anime/Manga list).
    /// </summary>
    public class ListEntryModel
    {
        /// <summary>
        /// The title of a given entry; (Anime/Manga).
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The url of the manga page on MAL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The url of the manga's cover.
        /// </summary>
        public string Image_url { get; set; }

        /// <summary>
        /// The type of a given entry, (ex; Anime, Movie, OVA, ONA, Special, Manga, Manwah, Novel, One Shot...).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The score of the entry in MAL.
        /// </summary>
        public byte? Score { get; set; }
    }
}
