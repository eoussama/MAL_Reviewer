using System;

namespace MAL_Reviewer_Core.models.UserModels
{
    /// <summary>
    /// A recently loaded MAL user, as remembered for the search of the user loading form.
    /// </summary>
    public class RecentUserModel
    {
        #region Properties

        /// <summary>
        /// The user's name, as written on MAL.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The url of the user's picture, if they have one.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// When the user was last loaded.
        /// </summary>
        public DateTimeOffset LoadedAt { get; set; }

        #endregion
    }
}
