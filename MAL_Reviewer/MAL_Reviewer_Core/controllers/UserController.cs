using System;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.interfaces;

namespace MAL_Reviewer_Core.controllers
{
    /// <summary>
    /// The controller of the user.
    /// </summary>
    public class UserController : ISettings
    {
        #region Properties

        /// <summary>
        /// The default MAL user.
        /// </summary>
        public string DefaultUser { get; set; } = string.Empty;

        /// <summary>
        /// Whether the default user's MAL account is loaded when the application starts.
        /// </summary>
        public bool LoadOnStart { get; set; } = true;

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
        }

        #endregion
    }
}
