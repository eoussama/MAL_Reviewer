using System;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core.extension;
using MAL_Reviewer_Core.models.UserModels;
using MAL_Reviewer_Core.services;

namespace MAL_Reviewer_Core
{
    /// <summary>
    /// The heart of the application.
    /// </summary>
    public static class Core
    {
        #region Fields

        /// <summary>
        /// The data of the connected MAL user, as fetched.
        /// </summary>
        private static MALUserModel loadedUser;

        /// <summary>
        /// Whether the data of the connected MAL user is already saved.
        /// </summary>
        private static bool loadedUserSaved;

        #endregion

        #region Properties

        /// <summary>
        /// The application's configurations.
        /// </summary>
        public static Config Configurations { get; set; }

        /// <summary>
        /// The application's settings.
        /// </summary>
        public static Settings Settings { get; set; }

        /// <summary>
        /// The currently connected MAL user.
        /// </summary>
        public static UserModel User { get; set; } = null;

        /// <summary>
        /// How the settings were obtained at start-up.
        /// </summary>
        public static SettingsLoadResult LoadResult { get; private set; }

        /// <summary>
        /// The saved data of the default MAL user, shown when MAL can't be reached.
        /// </summary>
        public static UserCache UserCache { get; private set; }

        /// <summary>
        /// The data of the connected MAL user, with their lists.
        /// </summary>
        public static MALUserModel UserData => loadedUser;

        /// <summary>
        /// When the data of the connected MAL user was fetched from MAL.
        /// </summary>
        public static DateTimeOffset? UserFetchedAt { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the intern functionalities of the application.
        /// </summary>
        /// <param name="configurations"> Where the data is stored; the user's application data folder when null. </param>
        /// <param name="api"> The MAL API to use; the default free services when null. </param>
        public static void Init(Config configurations = null, IMalApi api = null)
        {
            // Instantiation
            Configurations = configurations ?? new Config();
            Settings = new Settings(Configurations);
            UserCache = new UserCache(Configurations);
            UnloadUser();

            // Loading the data.
            Load();

            // Initializing the MAL API helper.
            if (api == null)
            {
                MALHelper.Init();
            }
            else
            {
                MALHelper.Use(api);
            }
        }

        /// <summary>
        /// Saves all of the application's data.
        /// </summary>
        public static void Save()
        {
            // Saving the settings, (the storage folder is created if it doesn't exist).
            Settings?.SaveSettings();

            // Saving the default user's data.
            SaveUserData();
        }

        /// <summary>
        /// Loads all of the application's data.
        /// </summary>
        public static void Load()
        {
            // Create the storage folder if it doesn't exist.
            Configurations.PrepareStorageFolder();

            // Loading the settings.
            LoadResult = Settings.LoadSettings();
        }

        /// <summary>
        /// Sets the currently connected MAL user.
        /// </summary>
        /// <param name="user"> The loaded MAL user. </param>
        /// <param name="savedAt"> When the user was loaded from their saved data, the date of that data; null when fetched from MAL now. </param>
        public static void SetUser(MALUserModel user, DateTimeOffset? savedAt = null)
        {
            if (user == null)
            {
                UnloadUser();
                return;
            }

            UserModel model = new UserModel();
            model.Load(user);

            User = model;
            loadedUser = user;
            loadedUserSaved = savedAt.HasValue;
            UserFetchedAt = savedAt ?? DateTimeOffset.Now;
        }

        /// <summary>
        /// Saves the data of the connected user when they are the default user, so it can be shown when MAL can't be reached.
        /// Without a default user, the saved data is deleted.
        /// </summary>
        public static void SaveUserData()
        {
            string defaultUser = Settings?.UserSettings.DefaultUser;

            if (UserCache == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(defaultUser))
            {
                UserCache.Clear();
                return;
            }

            if (loadedUser != null && !loadedUserSaved && string.Equals(loadedUser.Username, defaultUser, StringComparison.OrdinalIgnoreCase))
            {
                UserCache.Save(loadedUser, UserFetchedAt);
                loadedUserSaved = true;
            }
        }

        /// <summary>
        /// Disconnects the current MAL user.
        /// </summary>
        public static void UnloadUser()
        {
            User = null;
            loadedUser = null;
            loadedUserSaved = false;
            UserFetchedAt = null;
        }

        #endregion
    }
}
