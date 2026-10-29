using MAL_Reviewer_API;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core.extension;
using MAL_Reviewer_Core.models.UserModels;

namespace MAL_Reviewer_Core
{
    /// <summary>
    /// The heart of the application.
    /// </summary>
    public static class Core
    {
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
            User = null;

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
        public static void SetUser(MALUserModel user)
        {
            if (user == null)
            {
                UnloadUser();
                return;
            }

            UserModel model = new UserModel();
            model.Load(user);

            User = model;
        }

        /// <summary>
        /// Disconnects the current MAL user.
        /// </summary>
        public static void UnloadUser()
        {
            User = null;
        }

        #endregion
    }
}
