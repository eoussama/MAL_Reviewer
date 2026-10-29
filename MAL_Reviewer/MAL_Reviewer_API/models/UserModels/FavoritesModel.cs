namespace MAL_Reviewer_API.models.UserModels
{
    /// <summary>
    /// The user's favorite Anime/Manga/characters/people.
    /// </summary>
    public class FavoritesModel
    {
        /// <summary>
        /// List of the user's favorite Anime.
        /// </summary>
        public FavAnimeModel[] Anime { get; set; }

        /// <summary>
        /// List of the user's favorite Manga.
        /// </summary>
        public FavMangaModel[] Manga { get; set; }

        /// <summary>
        /// List of the user's favorite characters.
        /// </summary>
        public FavCharactersModel[] Characters { get; set; }

        /// <summary>
        /// List of the user's favorite people.
        /// </summary>
        public FavPeopleModel[] People { get; set; }
    }

    /// <summary>
    /// The Anime/Manga/characters/people's information.
    /// </summary>
    public class FavTargetModel
    {
        /// <summary>
        /// The id of the Anime/Manga/Character/Person on MAL.
        /// </summary>
        public int? Mal_id { get; set; }

        /// <summary>
        /// The url of the page of Anime/Manga/Character/Person on MAL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The url of the image of Anime/Manga/Character/Person.
        /// </summary>
        public string Image_url { get; set; }

        /// <summary>
        /// The name of Anime/Manga/Character/Person.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// The favorite Anime information.
    /// </summary>
    public class FavAnimeModel : FavTargetModel { }

    /// <summary>
    /// The favorite Manga information.
    /// </summary>
    public class FavMangaModel : FavTargetModel { }

    /// <summary>
    /// The favorite character information.
    /// </summary>
    public class FavCharactersModel : FavTargetModel { }

    /// <summary>
    /// The favorite person information.
    /// </summary>
    public class FavPeopleModel : FavTargetModel { }
}
