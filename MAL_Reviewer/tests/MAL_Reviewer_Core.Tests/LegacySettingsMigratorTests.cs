using System;
using System.IO;
using MAL_Reviewer_Core.migration;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class LegacySettingsMigratorTests
    {
        private static readonly DateTime Created = new DateTime(2018, 9, 1, 12, 30, 0);
        private static readonly DateTime Modified = new DateTime(2018, 10, 2, 8, 15, 0);

        private static byte[] SamplePayload()
        {
            return LegacyPayload.Write("Eoussama",
                LegacyPayload.Template("Classic MAL review", Created, Modified,
                    LegacyPayload.Aspect("Story", "Well paced", 8.5),
                    LegacyPayload.Aspect("Art")),
                LegacyPayload.Template("Lazy MAL review", Created, Created));
        }

        [Fact]
        public void Read_RecoversTheUserAndTemplates()
        {
            using (MemoryStream stream = new MemoryStream(SamplePayload()))
            {
                LegacySettings settings = LegacySettingsMigrator.Read(stream);

                Assert.Equal("Eoussama", settings.DefaultUser);
                Assert.Equal(2, settings.ReviewTemplates.Count);

                ReviewTemplateModel classic = settings.ReviewTemplates[0];
                Assert.Equal("Classic MAL review", classic.TemplateName);
                Assert.True(classic.UseIntro);
                Assert.False(classic.UserTLDR);
                Assert.Equal(Created, classic.CreationDate);
                Assert.Equal(Modified, classic.ModificationDate);
                Assert.Equal(2, classic.TemplateAspects.Count);
                Assert.Equal("Story", classic.TemplateAspects[0].AspectName);
                Assert.Equal("Well paced", classic.TemplateAspects[0].AspectReview);
                Assert.Equal(8.5, classic.TemplateAspects[0].AspectRating);
                Assert.Equal("Art", classic.TemplateAspects[1].AspectName);

                Assert.Empty(settings.ReviewTemplates[1].TemplateAspects);
            }
        }

        [Fact]
        public void Read_HandlesNullValues()
        {
            Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewTemplateModel template = LegacyPayload.Template(null, Created, Modified);
            template.TemplateAspects = null;

            using (MemoryStream stream = new MemoryStream(LegacyPayload.Write(null, template)))
            {
                LegacySettings settings = LegacySettingsMigrator.Read(stream);

                Assert.Equal(string.Empty, settings.DefaultUser);
                Assert.Equal(ReviewTemplateModel.DefaultReviewTemplateName, settings.ReviewTemplates[0].TemplateName);
                Assert.Empty(settings.ReviewTemplates[0].TemplateAspects);
            }
        }

        [Fact]
        public void Read_HandlesMissingControllers()
        {
            using (MemoryStream stream = new MemoryStream(LegacyPayload.Write(new Legacy.MAL_Reviewer_Core.Settings())))
            {
                LegacySettings settings = LegacySettingsMigrator.Read(stream);

                Assert.Equal(string.Empty, settings.DefaultUser);
                Assert.Empty(settings.ReviewTemplates);
            }
        }

        [Fact]
        public void Read_RejectsPayloadsThatAreNotSettings()
        {
            using (MemoryStream stream = new MemoryStream(LegacyPayload.Write(new Legacy.Unrelated())))
            {
                Assert.Null(LegacySettingsMigrator.Read(stream));
            }
        }

        [Fact]
        public void TryRead_ReturnsFalse_ForDamagedOrMissingFiles()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                string path = storage.File("settings.dat");
                File.WriteAllBytes(path, new byte[] { 1, 2, 3, 4 });

                Assert.False(LegacySettingsMigrator.TryRead(path, out LegacySettings damaged));
                Assert.Null(damaged);
                Assert.False(LegacySettingsMigrator.TryRead(storage.File("missing.dat"), out _));
            }
        }

        [Fact]
        public void Load_MigratesTheLegacyFile_AndSetsItAside()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllBytes(storage.File(Settings.LegacyStorageFile), SamplePayload());

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Migrated, settings.LoadSettings());
                Assert.Equal("Eoussama", settings.UserSettings.DefaultUser);
                Assert.Equal(2, settings.ReviewTemplatesSettings.Count);
                Assert.True(File.Exists(storage.File(Settings.StorageFile)));
                Assert.False(File.Exists(storage.File(Settings.LegacyStorageFile)));
                Assert.True(File.Exists(storage.File(Settings.LegacyStorageFile + ".migrated")));

                // The next start reads the new file.
                Settings reloaded = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, reloaded.LoadSettings());
                Assert.Equal(Modified, reloaded.ReviewTemplatesSettings.ReviewTemplates[0].ModificationDate);
            }
        }

        [Fact]
        public void Load_LeavesAnUnreadableLegacyFileAlone()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.LegacyStorageFile), "garbage");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Seeded, settings.LoadSettings());
                Assert.True(File.Exists(storage.File(Settings.LegacyStorageFile)));
            }
        }

        [Fact]
        public void Load_PrefersTheJsonFile_OverALegacyFile()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings current = new Settings(storage.Config);
                current.UserSettings.DefaultUser = "current";
                current.SaveSettings();
                File.WriteAllBytes(storage.File(Settings.LegacyStorageFile), SamplePayload());

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal("current", settings.UserSettings.DefaultUser);
                Assert.True(File.Exists(storage.File(Settings.LegacyStorageFile)));
            }
        }
    }
}
