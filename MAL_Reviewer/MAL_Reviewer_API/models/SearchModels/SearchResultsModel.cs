namespace MAL_Reviewer_API.models.SearchModels
{
    /// <summary>
    /// The information regarding a search entry.
    /// </summary>
    public class SearchResultsModel
    {
        /// <summary>
        /// The id of an Anime/Manga on MAL.
        /// </summary>
        public int Mal_id { get; set; }

        /// <summary>
        /// The title of an Anime/Manga.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The type of an Anime/Manga.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The url of the cover of an Anime/Manga.
        /// </summary>
        public string Image_url { get; set; }
    }
}
