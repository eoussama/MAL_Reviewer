using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.interfaces;
using MAL_Reviewer_Core.migration;
using MAL_Reviewer_Core.models.ReviewTemplateModels;

namespace MAL_Reviewer_Core
{
    /// <summary>
    /// How the settings were obtained when loading them.
    /// </summary>
    public enum SettingsLoadResult
    {
        /// <summary>
        /// Read from the settings file.
        /// </summary>
        Loaded,

        /// <summary>
        /// No settings existed, the defaults were generated.
        /// </summary>
        Seeded,

        /// <summary>
        /// Converted from the settings file of older versions.
        /// </summary>
        Migrated,

        /// <summary>
        /// The settings file was unreadable; it was set aside and the defaults were generated.
        /// </summary>
        Recovered
    }

    /// <summary>
    /// The application's settings.
    /// </summary>
    public class Settings : ISettings
    {
        #region Fields

        /// <summary>
        /// The settings storage file's name.
        /// </summary>
        internal const string StorageFile = "settings.json";

        /// <summary>
        /// The binary settings file written by versions before 0.2.0.
        /// </summary>
        internal const string LegacyStorageFile = "settings.dat";

        /// <summary>
        /// The version of the settings file's layout.
        /// </summary>
        internal const int FormatVersion = 1;

        internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        private readonly Config config;

        #endregion

        #region Properties

        /// <summary>
        /// The user's settings.
        /// </summary>
        public UserController UserSettings { get; set; }

        /// <summary>
        /// The review template's settings.
        /// </summary>
        public ReviewTemplatesController ReviewTemplatesSettings { get; set; }

        /// <summary>
        /// The file the unreadable settings were moved to by the last load, if any.
        /// </summary>
        public string RecoveredFilePath { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="config"> The configurations that tell where the settings are stored. </param>
        public Settings(Config config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.UserSettings = new UserController();
            this.ReviewTemplatesSettings = new ReviewTemplatesController();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads stored settings into the memory.
        /// </summary>
        public SettingsLoadResult LoadSettings()
        {
            this.RecoveredFilePath = null;

            string path = this.config.GetStorageFilePath(StorageFile);

            if (File.Exists(path))
            {
                try
                {
                    SettingsDocument document = JsonSerializer.Deserialize<SettingsDocument>(File.ReadAllText(path), JsonOptions)
                        ?? throw new JsonException("The settings file is empty.");

                    Apply(document);

                    return SettingsLoadResult.Loaded;
                }
                catch (JsonException)
                {
                    // Keeping the unreadable file for inspection instead of overwriting it.
                    this.RecoveredFilePath = this.config.GetStorageFilePath($"settings.corrupt-{ DateTime.Now:yyyyMMddHHmmss}.json");
                    File.Move(path, this.RecoveredFilePath, true);

                    ClearAndSeed();

                    return SettingsLoadResult.Recovered;
                }
            }

            string legacyPath = this.config.GetStorageFilePath(LegacyStorageFile);

            if (File.Exists(legacyPath) && LegacySettingsMigrator.TryRead(legacyPath, out LegacySettings legacy))
            {
                this.UserSettings = new UserController { DefaultUser = legacy.DefaultUser ?? string.Empty };
                this.ReviewTemplatesSettings = new ReviewTemplatesController { ReviewTemplates = legacy.ReviewTemplates };

                // Saving in the new format first, so the old file is only set aside once its data is safe.
                SaveSettings();
                File.Move(legacyPath, legacyPath + ".migrated", true);

                return SettingsLoadResult.Migrated;
            }

            ClearAndSeed();

            return SettingsLoadResult.Seeded;
        }

        /// <summary>
        /// Saves the settings in the memory into the settings file.
        /// </summary>
        public void SaveSettings()
        {
            this.config.PrepareStorageFolder();

            string path = this.config.GetStorageFilePath(StorageFile);
            string temporaryPath = path + ".tmp";

            SettingsDocument document = new SettingsDocument
            {
                FormatVersion = FormatVersion,
                User = this.UserSettings,
                ReviewTemplates = this.ReviewTemplatesSettings.ReviewTemplates
            };

            // Writing next to the target first, so an interrupted save never leaves a half-written file.
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(document, JsonOptions));
            File.Move(temporaryPath, path, true);
        }

        /// <summary>
        /// Generates default setting values for when the application first runs,
        /// also when a global reset is needed.
        /// </summary>
        public void SeedSettings()
        {
            // Seeding the user's settings.
            this.UserSettings.SeedSettings();

            // Seeding review template settings.
            this.ReviewTemplatesSettings.SeedSettings();
        }

        /// <summary>
        /// Resets the application's settings
        /// </summary>
        public void ResetSettings()
        {
            // Reseting the user's settings.
            this.UserSettings.ResetSettings();

            // Resetting the review template's settings.
            this.ReviewTemplatesSettings.ResetSettings();
        }

        #endregion

        #region Private methods

        private void ClearAndSeed()
        {
            this.UserSettings = new UserController();
            this.ReviewTemplatesSettings = new ReviewTemplatesController();

            SeedSettings();
        }

        /// <summary>
        /// Copies a loaded document into memory, repairing missing parts instead of failing on them.
        /// </summary>
        private void Apply(SettingsDocument document)
        {
            this.UserSettings = document.User ?? new UserController();
            this.UserSettings.DefaultUser ??= string.Empty;

            List<ReviewTemplateModel> templates = document.ReviewTemplates ?? new List<ReviewTemplateModel>();
            templates.RemoveAll(template => template == null);
            templates.ForEach(template =>
            {
                template.TemplateName ??= ReviewTemplateModel.DefaultReviewTemplateName;
                template.TemplateAspects ??= new List<ReviewAspectModel>();
                template.TemplateAspects.RemoveAll(aspect => aspect == null);
            });

            this.ReviewTemplatesSettings = new ReviewTemplatesController { ReviewTemplates = templates };
        }

        #endregion

        #region Settings file layout

        internal class SettingsDocument
        {
            [JsonPropertyName("formatVersion")]
            public int FormatVersion { get; set; }

            [JsonPropertyName("user")]
            public UserController User { get; set; }

            [JsonPropertyName("reviewTemplates")]
            public List<ReviewTemplateModel> ReviewTemplates { get; set; }
        }

        #endregion
    }
}
