using System;
using System.Collections.Generic;
using System.Linq;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.interfaces;
using MAL_Reviewer_Core.models.UserModels;

namespace MAL_Reviewer_Core.controllers
{
    /// <summary>
    /// The controller of the user.
    /// </summary>
    public class UserController : ISettings
    {
        #region Fields

        /// <summary>
        /// The most recently loaded users remembered.
        /// </summary>
        public const int MaxRecentUsers = 10;

        private List<RecentUserModel> recentUsers = new List<RecentUserModel>();

        #endregion

        #region Properties

        /// <summary>
        /// The default MAL user.
        /// </summary>
        public string DefaultUser { get; set; } = string.Empty;

        /// <summary>
        /// Whether the default user's MAL account is loaded when the application starts.
        /// </summary>
        public bool LoadOnStart { get; set; } = true;

        /// <summary>
        /// The recently loaded users, the most recent first.
        /// </summary>
        public List<RecentUserModel> RecentUsers
        {
            get => this.recentUsers;
            set => this.recentUsers = value ?? new List<RecentUserModel>();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public UserController() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Validates and sets the default MAL user.
        /// </summary>
        /// <param name="username"> The username; surrounding spaces are ignored. </param>
        /// <exception cref="ArgumentException"> When the username is not a valid MAL username or already the default. </exception>
        public void SetDefaultUser(string username)
        {
            string name = UsernameRule.Normalize(username);

            if (!UsernameRule.IsValid(name))
            {
                throw new ArgumentException(UsernameRule.InvalidMessage);
            }

            if (string.Equals(name, DefaultUser, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The default user is already “{ DefaultUser }”.");
            }

            DefaultUser = name;
        }

        /// <summary>
        /// Removes the default MAL user.
        /// </summary>
        public void ClearDefaultUser()
        {
            DefaultUser = string.Empty;
        }

        /// <summary>
        /// Remembers a loaded user as the most recent one.
        /// </summary>
        /// <param name="username"> The user's name, as written on MAL. </param>
        /// <param name="imageUrl"> The url of the user's picture, if any. </param>
        /// <param name="loadedAt"> When the user was loaded. </param>
        /// <returns> Whether the user was remembered; invalid usernames are not. </returns>
        public bool RememberUser(string username, string imageUrl, DateTimeOffset loadedAt)
        {
            string name = UsernameRule.Normalize(username);

            if (!UsernameRule.IsValid(name))
            {
                return false;
            }

            ForgetUser(name);
            RecentUsers.Insert(0, new RecentUserModel { Username = name, ImageUrl = imageUrl, LoadedAt = loadedAt });

            if (RecentUsers.Count > MaxRecentUsers)
            {
                RecentUsers.RemoveRange(MaxRecentUsers, RecentUsers.Count - MaxRecentUsers);
            }

            return true;
        }

        /// <summary>
        /// Removes a user from the recently loaded ones.
        /// </summary>
        /// <param name="username"> The user's name, in any case. </param>
        public void ForgetUser(string username)
        {
            string name = UsernameRule.Normalize(username);

            RecentUsers.RemoveAll(user => string.Equals(user?.Username, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The recently loaded users matching a search: those whose name starts with it first, then those containing it,
        /// the most recent first in each. An empty search matches everyone.
        /// </summary>
        /// <param name="search"> The searched text. </param>
        public List<RecentUserModel> FindRecentUsers(string search)
        {
            string text = UsernameRule.Normalize(search);
            List<RecentUserModel> users = RecentUsers.Where(user => user?.Username != null).ToList();

            return users.Where(user => user.Username.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                .Concat(users.Where(user => !user.Username.StartsWith(text, StringComparison.OrdinalIgnoreCase) && user.Username.Contains(text, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /// <summary>
        /// Resets the user's settings.
        /// </summary>
        public void ResetSettings()
        {
            SeedSettings();
        }

        /// <summary>
        /// Generate default values for the user's settings.
        /// </summary>
        public void SeedSettings()
        {
            DefaultUser = string.Empty;
            LoadOnStart = true;
            RecentUsers.Clear();
        }

        #endregion
    }
}
