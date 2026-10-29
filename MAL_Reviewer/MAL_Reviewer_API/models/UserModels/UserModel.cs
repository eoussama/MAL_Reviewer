using System;
using System.Collections.Generic;
using MAL_Reviewer_API.models.ListEntryModels;

namespace MAL_Reviewer_API.models.UserModels
{
    /// <summary>
    /// The user's information on MAL.
    /// </summary>
    public class MALUserModel
    {
        /// <summary>
        /// The user's name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The user's MAL page's url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The user's image url.
        /// </summary>
        public string Image_url { get; set; }

        /// <summary>
        /// The user's gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// The user's location.
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// The user's about.
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// The user's birthdate.
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// The user's joining date.
        /// </summary>
        public DateTime? Joined { get; set; }

        /// <summary>
        /// The user's Anime stats.
        /// </summary>
        public AnimeStatsModel Anime_stats { get; set; }

        /// <summary>
        /// The user's Manga stats.
        /// </summary>
        public MangaStatsModel Manga_stats { get; set; }

        /// <summary>
        /// The user's favorite Anime/Manga/characters/people list.
        /// </summary>
        public FavoritesModel Favorites { get; set; }

        /// <summary>
        /// The user's Anime list.
        /// </summary>
        public IEnumerable<AnimelistEntryModel> AnimeList { get; set; }

        /// <summary>
        /// The user's Manga list.
        /// </summary>
        public IEnumerable<MangalistEntryModel> MangaList { get; set; }
    }
}
