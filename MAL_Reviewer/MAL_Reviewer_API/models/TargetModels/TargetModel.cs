namespace MAL_Reviewer_API.models.TargetModels
{
    /// <summary>
    /// The information regarding an individual Anime/Manga.
    /// </summary>
    public class TargetModel
    {
        /// <summary>
        /// The id of an Anime/Manga on MAL.
        /// </summary>
        public int Mal_id { get; set; }
        
        /// <summary>
        /// The url of the page of an Anime/Manga on MAL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The url of the image of the Anime/Manga's cover.
        /// </summary>
        public string Image_url { get; set; }

        /// <summary>
        /// The entry's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The entry's type; (Anime, OVA, ONA, Movie, Special, Manga, Manwha, One Shot, Nover...).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The score of the entry on MAL.
        /// </summary>
        public float? Score { get; set; }

        /// <summary>
        /// The rank of the entry on MAL.
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// The synopsis of the entry on MAL.
        /// </summary>
        public string Synopsis { get; set; }
    }
}
