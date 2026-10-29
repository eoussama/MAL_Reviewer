using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class SettingsTests
    {
        [Fact]
        public void Load_SeedsTheDefaults_WhenNothingIsStored()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Seeded, settings.LoadSettings());
                Assert.Equal(string.Empty, settings.UserSettings.DefaultUser);
                Assert.Equal(new[] { "Classic MAL review", "Lazy MAL review" }, settings.ReviewTemplatesSettings.ReviewTemplates.Select(template => template.TemplateName));
                Assert.Equal(5, settings.ReviewTemplatesSettings.ReviewTemplates[0].TemplateAspects.Count);
            }
        }

        [Fact]
        public void SaveThenLoad_KeepsEverything()
        {
            using (TempStorage storage = new TempStorage())
            {
                DateTime created = new DateTime(2020, 1, 2, 3, 4, 5);
                DateTime modified = new DateTime(2021, 6, 7, 8, 9, 10);

                Settings saved = new Settings(storage.Config);
                saved.UserSettings.DefaultUser = "Xinil";
                saved.ReviewTemplatesSettings.ReviewTemplates.Add(new ReviewTemplateModel("Mine", true, false, created, modified, new List<ReviewAspectModel>
                {
                    new ReviewAspectModel("Story", "Great", 9.5)
                }));
                saved.SaveSettings();

                Settings loaded = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, loaded.LoadSettings());
                Assert.Equal("Xinil", loaded.UserSettings.DefaultUser);

                ReviewTemplateModel template = Assert.Single(loaded.ReviewTemplatesSettings.ReviewTemplates);
                Assert.Equal("Mine", template.TemplateName);
                Assert.True(template.UseIntro);
                Assert.False(template.UserTLDR);
                Assert.Equal(created, template.CreationDate);
                Assert.Equal(modified, template.ModificationDate);

                ReviewAspectModel aspect = Assert.Single(template.TemplateAspects);
                Assert.Equal("Story", aspect.AspectName);
                Assert.Equal("Great", aspect.AspectReview);
                Assert.Equal(9.5, aspect.AspectRating);
            }
        }

        [Fact]
        public void Save_WritesReadableJson_AndLeavesNoTemporaryFile()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);
                settings.SeedSettings();
                settings.SaveSettings();

                string json = File.ReadAllText(storage.File(Settings.StorageFile));

                Assert.Contains("\"formatVersion\": 1", json);
                Assert.Contains("\"reviewTemplates\"", json);
                Assert.DoesNotContain("\"count\"", json);
                Assert.False(File.Exists(storage.File(Settings.StorageFile) + ".tmp"));
            }
        }

        [Fact]
        public void Save_ReplacesALongerPreviousFile()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);
                settings.SeedSettings();
                settings.SaveSettings();

                // Fewer templates make a shorter file; the old content must not survive past its end.
                settings.ReviewTemplatesSettings.Clear();
                settings.SaveSettings();

                Settings loaded = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, loaded.LoadSettings());
                Assert.Empty(loaded.ReviewTemplatesSettings.ReviewTemplates);
            }
        }

        [Theory]
        [InlineData("{ not json")]
        [InlineData("null")]
        [InlineData("")]
        public void Load_RecoversFromAnUnreadableFile(string content)
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), content);

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Recovered, settings.LoadSettings());
                Assert.Equal(2, settings.ReviewTemplatesSettings.Count);
                Assert.False(File.Exists(storage.File(Settings.StorageFile)));
                Assert.True(File.Exists(settings.RecoveredFilePath));
                Assert.Equal(content, File.ReadAllText(settings.RecoveredFilePath));
            }
        }

        [Fact]
        public void Load_RepairsMissingParts()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile),
                    "{\"formatVersion\":1,\"user\":{\"defaultUser\":null},\"reviewTemplates\":[null,{\"templateName\":null,\"templateAspects\":null},{\"templateName\":\"A\",\"templateAspects\":[null,{\"aspectName\":\"Art\"}]}]}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(string.Empty, settings.UserSettings.DefaultUser);
                Assert.Equal(2, settings.ReviewTemplatesSettings.Count);
                Assert.Equal(ReviewTemplateModel.DefaultReviewTemplateName, settings.ReviewTemplatesSettings.ReviewTemplates[0].TemplateName);
                Assert.Empty(settings.ReviewTemplatesSettings.ReviewTemplates[0].TemplateAspects);
                Assert.Equal("Art", Assert.Single(settings.ReviewTemplatesSettings.ReviewTemplates[1].TemplateAspects).AspectName);
            }
        }

        [Fact]
        public void Load_ToleratesAFileWithoutUserOrTemplates()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), "{}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(string.Empty, settings.UserSettings.DefaultUser);
                Assert.Empty(settings.ReviewTemplatesSettings.ReviewTemplates);
            }
        }

        [Fact]
        public void Reset_GoesBackToTheDefaults()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);
                settings.LoadSettings();
                settings.UserSettings.DefaultUser = "someone";
                settings.ReviewTemplatesSettings.Clear();

                settings.ResetSettings();

                Assert.Equal(string.Empty, settings.UserSettings.DefaultUser);
                Assert.Equal(2, settings.ReviewTemplatesSettings.Count);
            }
        }

        [Fact]
        public void Constructor_RequiresConfigurations()
        {
            Assert.Throws<ArgumentNullException>(() => new Settings(null));
        }
    }
}
