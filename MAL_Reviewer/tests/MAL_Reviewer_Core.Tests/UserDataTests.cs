using System;
using System.IO;
using System.Linq;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.services;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class UserCacheTests
    {
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 30, 9, 15, 0, TimeSpan.FromHours(1));

        private static MALUserModel User(string name = "Xinil") => new MALUserModel
        {
            Username = name,
            Joined = new DateTime(2004, 11, 5),
            Anime_stats = new AnimeStatsModel { Completed = 12, Mean_score = 7.5 },
            Favorites = new FavoritesModel { Anime = new[] { new FavAnimeModel { Mal_id = 1, Name = "Cowboy Bebop" } } },
            AnimeList = new[] { new AnimelistEntryModel { Title = "Cowboy Bebop", Watching_status = 2, Score = 9 } },
            MangaList = null
        };

        [Fact]
        public void SavedData_IsReadBack_WithItsDate()
        {
            using (TempStorage storage = new TempStorage())
            {
                UserCache cache = new UserCache(storage.Config, () => Now);

                cache.Save(User());

                Assert.True(cache.TryLoad("  xinil ", out MALUserModel user, out DateTimeOffset savedAt));
                Assert.Equal("Xinil", user.Username);
                Assert.Equal(new DateTime(2004, 11, 5), user.Joined);
                Assert.Equal(12, user.Anime_stats.Completed);
                Assert.Equal("Cowboy Bebop", user.Favorites.Anime.Single().Name);
                Assert.Equal((short)2, user.AnimeList.Single().Watching_status);
                Assert.Null(user.MangaList);
                Assert.Equal(Now, savedAt);
                Assert.False(File.Exists(storage.File(UserCache.StorageFile) + ".tmp"));
            }
        }

        [Fact]
        public void Save_KeepsAPassedDate()
        {
            using (TempStorage storage = new TempStorage())
            {
                UserCache cache = new UserCache(storage.Config, () => Now);
                DateTimeOffset fetched = Now.AddDays(-3);

                cache.Save(User(), fetched);

                Assert.True(cache.TryLoad("Xinil", out _, out DateTimeOffset savedAt));
                Assert.Equal(fetched, savedAt);
                Assert.Contains("\"formatVersion\":1", File.ReadAllText(storage.File(UserCache.StorageFile)));
            }
        }

        [Fact]
        public void TryLoad_IgnoresOtherUsers_AndMissingData()
        {
            using (TempStorage storage = new TempStorage())
            {
                UserCache cache = new UserCache(storage.Config);

                Assert.False(cache.TryLoad("Xinil", out _, out _));

                cache.Save(User());

                Assert.False(cache.TryLoad("Someone", out MALUserModel user, out DateTimeOffset savedAt));
                Assert.Null(user);
                Assert.Equal(default, savedAt);
                Assert.False(cache.TryLoad("", out _, out _));
                Assert.False(cache.TryLoad(null, out _, out _));
            }
        }

        [Theory]
        [InlineData("not json")]
        [InlineData("{}")]
        [InlineData("{\"user\":null}")]
        [InlineData("{\"user\":{\"username\":null}}")]
        [InlineData("[1]")]
        public void TryLoad_RefusesUnreadableData(string content)
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(UserCache.StorageFile), content);

                Assert.False(new UserCache(storage.Config).TryLoad("Xinil", out MALUserModel user, out _));
                Assert.Null(user);
            }
        }

        [Fact]
        public void Clear_DeletesTheSavedData_EvenWhenThereIsNone()
        {
            using (TempStorage storage = new TempStorage())
            {
                UserCache cache = new UserCache(storage.Config);

                cache.Clear();
                cache.Save(User());
                cache.Clear();

                Assert.False(File.Exists(storage.File(UserCache.StorageFile)));
                Assert.False(cache.TryLoad("Xinil", out _, out _));
            }
        }

        [Fact]
        public void Arguments_AreChecked()
        {
            using (TempStorage storage = new TempStorage())
            {
                Assert.Throws<ArgumentNullException>(() => new UserCache(null));
                Assert.Throws<ArgumentNullException>(() => new UserCache(storage.Config).Save(null));
            }
        }
    }

    [Collection(nameof(CoreState))]
    public class CoreUserDataTests
    {
        [Fact]
        public void TheDefaultUser_IsSavedOnce_WhenFetched()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.Settings.UserSettings.DefaultUser = "xinil";
                DateTimeOffset before = DateTimeOffset.Now;

                Core.SetUser(new MALUserModel { Username = "Xinil" });
                Core.SaveUserData();

                Assert.True(Core.UserCache.TryLoad("Xinil", out _, out DateTimeOffset savedAt));
                Assert.InRange(savedAt, before, DateTimeOffset.Now);
                Assert.Equal(savedAt, Core.UserFetchedAt);

                // Saving again doesn't write the same data twice.
                Core.UserCache.Clear();
                Core.Save();
                Assert.False(File.Exists(storage.File(UserCache.StorageFile)));
            }
        }

        [Fact]
        public void OtherUsers_AreNotSaved()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.Settings.UserSettings.DefaultUser = "Xinil";

                Core.SetUser(new MALUserModel { Username = "Someone" });
                Core.Save();

                Assert.False(File.Exists(storage.File(UserCache.StorageFile)));
            }
        }

        [Fact]
        public void AUserMadeDefaultAfterLoading_IsSavedWithTheSettings()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.SetUser(new MALUserModel { Username = "Xinil" });

                Core.Settings.UserSettings.SetDefaultUser("Xinil");
                Core.Save();

                Assert.True(Core.UserCache.TryLoad("Xinil", out _, out _));
            }
        }

        [Fact]
        public void DataShownFromTheSavedCopy_KeepsItsDate()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.Settings.UserSettings.DefaultUser = "Xinil";
                DateTimeOffset savedAt = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
                Core.UserCache.Save(new MALUserModel { Username = "Xinil" }, savedAt);

                Assert.True(Core.UserCache.TryLoad("Xinil", out MALUserModel saved, out DateTimeOffset date));
                Core.SetUser(saved, date);
                Core.Save();

                Assert.Equal(savedAt, Core.UserFetchedAt);
                Assert.True(Core.UserCache.TryLoad("Xinil", out _, out DateTimeOffset kept));
                Assert.Equal(savedAt, kept);
            }
        }

        [Fact]
        public void WithoutADefaultUser_TheSavedDataIsDeleted()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.UserCache.Save(new MALUserModel { Username = "Xinil" });

                Core.Settings.UserSettings.ResetSettings();
                Core.Save();

                Assert.False(File.Exists(storage.File(UserCache.StorageFile)));
            }
        }

        [Fact]
        public void Unloading_ForgetsTheLoadedUser()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.Settings.UserSettings.DefaultUser = "Xinil";
                Core.SetUser(new MALUserModel { Username = "Xinil" });

                Core.UnloadUser();
                Core.Save();

                Assert.Null(Core.UserFetchedAt);
                Assert.False(File.Exists(storage.File(UserCache.StorageFile)));
            }
        }
    }

    public class LoadOnStartTests
    {
        [Fact]
        public void LoadOnStart_IsOnByDefault_AndAfterAReset()
        {
            UserController settings = new UserController();
            Assert.True(settings.LoadOnStart);

            settings.LoadOnStart = false;
            settings.ResetSettings();

            Assert.True(settings.LoadOnStart);
        }

        [Fact]
        public void LoadOnStart_IsSaved_AndOnForOlderFiles()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings saved = new Settings(storage.Config);
                saved.UserSettings.LoadOnStart = false;
                saved.SaveSettings();

                Assert.Contains("\"loadOnStart\": false", File.ReadAllText(storage.File(Settings.StorageFile)));

                Settings loaded = new Settings(storage.Config);
                loaded.LoadSettings();
                Assert.False(loaded.UserSettings.LoadOnStart);

                File.WriteAllText(storage.File(Settings.StorageFile), "{\"formatVersion\":1,\"user\":{\"defaultUser\":\"Xinil\"}}");

                Settings older = new Settings(storage.Config);
                Assert.Equal(SettingsLoadResult.Loaded, older.LoadSettings());
                Assert.True(older.UserSettings.LoadOnStart);
                Assert.Equal("Xinil", older.UserSettings.DefaultUser);
            }
        }
    }
}
