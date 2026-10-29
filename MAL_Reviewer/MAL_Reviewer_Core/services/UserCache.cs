using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MAL_Reviewer_API.models.UserModels;

namespace MAL_Reviewer_Core.services
{
    /// <summary>
    /// Keeps the data of the default MAL user on disk, so it can be shown when MAL can't be reached.
    /// </summary>
    public class UserCache
    {
        #region Fields

        /// <summary>
        /// The file of the saved user data.
        /// </summary>
        internal const string StorageFile = "user.json";

        /// <summary>
        /// The version of the file's layout.
        /// </summary>
        internal const int FormatVersion = 1;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly Config config;
        private readonly Func<DateTimeOffset> now;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="config"> The configurations that tell where the data is stored. </param>
        /// <param name="now"> The clock that dates the saved data; the system's clock when null. </param>
        public UserCache(Config config, Func<DateTimeOffset> now = null)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.now = now ?? (() => DateTimeOffset.Now);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves a user's data, replacing any saved before.
        /// </summary>
        /// <param name="user"> The user, with their lists. </param>
        /// <param name="savedAt"> When the data was fetched from MAL; now when null. </param>
        public void Save(MALUserModel user, DateTimeOffset? savedAt = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            this.config.PrepareStorageFolder();

            string path = this.config.GetStorageFilePath(StorageFile);
            string temporaryPath = path + ".tmp";

            SavedUser saved = new SavedUser
            {
                FormatVersion = FormatVersion,
                SavedAt = savedAt ?? this.now(),
                User = user
            };

            // Writing next to the target first, so an interrupted save never leaves a half-written file.
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(saved, JsonOptions));
            File.Move(temporaryPath, path, true);
        }

        /// <summary>
        /// Reads the saved data of a user.
        /// </summary>
        /// <param name="username"> The user's name; saved data of anyone else is ignored. </param>
        /// <param name="user"> The saved user, or null. </param>
        /// <param name="savedAt"> When the data was fetched from MAL. </param>
        /// <returns> Whether data of that user was saved and could be read. </returns>
        public bool TryLoad(string username, out MALUserModel user, out DateTimeOffset savedAt)
        {
            user = null;
            savedAt = default;

            string path = this.config.GetStorageFilePath(StorageFile);

            if (string.IsNullOrWhiteSpace(username) || !File.Exists(path))
            {
                return false;
            }

            SavedUser saved;

            try
            {
                saved = JsonSerializer.Deserialize<SavedUser>(File.ReadAllText(path), JsonOptions);
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException)
            {
                return false;
            }

            if (saved?.User == null || !string.Equals(saved.User.Username, username.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            user = saved.User;
            savedAt = saved.SavedAt;

            return true;
        }

        /// <summary>
        /// Deletes the saved user data.
        /// </summary>
        public void Clear()
        {
            string path = this.config.GetStorageFilePath(StorageFile);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

        #region File layout

        internal class SavedUser
        {
            [JsonPropertyName("formatVersion")]
            public int FormatVersion { get; set; }

            [JsonPropertyName("savedAt")]
            public DateTimeOffset SavedAt { get; set; }

            [JsonPropertyName("user")]
            public MALUserModel User { get; set; }
        }

        #endregion
    }
}
