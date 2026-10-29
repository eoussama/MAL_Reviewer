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

namespace MAL_Reviewer_Core.Tests
{
    public class RecentUsersTests
    {
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

        [Fact]
        public void LoadedUsers_AreRemembered_MostRecentFirst_OnceEach()
        {
            UserController settings = new UserController();

            Assert.True(settings.RememberUser("Xinil", "https://cdn/x.jpg", Now));
            Assert.True(settings.RememberUser("  Someone ", null, Now.AddMinutes(1)));
            Assert.True(settings.RememberUser("xinil", null, Now.AddMinutes(2)));

            Assert.Equal(new[] { "xinil", "Someone" }, settings.RecentUsers.Select(user => user.Username));
            Assert.Equal(Now.AddMinutes(2), settings.RecentUsers[0].LoadedAt);
            Assert.Null(settings.RecentUsers[0].ImageUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("not a name!")]
        public void InvalidNames_AreNotRemembered(string username)
        {
            UserController settings = new UserController();

            Assert.False(settings.RememberUser(username, null, Now));
            Assert.Empty(settings.RecentUsers);
        }

        [Fact]
        public void OnlyTheMostRecentUsers_AreKept()
        {
            UserController settings = new UserController();

            for (int i = 0; i < UserController.MaxRecentUsers + 3; i++)
            {
                settings.RememberUser($"user{ i }", null, Now.AddMinutes(i));
            }

            Assert.Equal(UserController.MaxRecentUsers, settings.RecentUsers.Count);
            Assert.Equal($"user{ UserController.MaxRecentUsers + 2 }", settings.RecentUsers[0].Username);
            Assert.Equal("user3", settings.RecentUsers.Last().Username);
        }

        [Fact]
        public void FindRecentUsers_PutsNamesStartingWithTheSearchFirst()
        {
            UserController settings = new UserController();
            settings.RememberUser("Kuroneko", null, Now);
            settings.RememberUser("NekoChan", null, Now);
            settings.RememberUser("Xinil", null, Now);
            settings.RememberUser("AnotherNeko", null, Now);

            Assert.Equal(new[] { "NekoChan", "AnotherNeko", "Kuroneko" }, settings.FindRecentUsers(" neko ").Select(user => user.Username));
            Assert.Equal(4, settings.FindRecentUsers("").Count);
            Assert.Equal(4, settings.FindRecentUsers(null).Count);
            Assert.Empty(settings.FindRecentUsers("zzz"));
        }

        [Fact]
        public void ForgetAndReset_RemoveUsers()
        {
            UserController settings = new UserController();
            settings.RememberUser("Xinil", null, Now);
            settings.RememberUser("Someone", null, Now);

            settings.ForgetUser(" XINIL ");
            Assert.Equal("Someone", settings.RecentUsers.Single().Username);

            settings.ResetSettings();
            Assert.Empty(settings.RecentUsers);

            settings.RecentUsers = null;
            Assert.NotNull(settings.RecentUsers);
        }

        [Fact]
        public void RecentUsers_AreSaved_AndRepairedWhenLoaded()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings saved = new Settings(storage.Config);
                saved.UserSettings.RememberUser("Xinil", "https://cdn/x.jpg", Now);
                saved.SaveSettings();

                string json = File.ReadAllText(storage.File(Settings.StorageFile));
                Assert.Contains("\"recentUsers\"", json);
                Assert.Contains("\"imageUrl\": \"https://cdn/x.jpg\"", json);

                Settings loaded = new Settings(storage.Config);
                loaded.LoadSettings();
                Assert.Equal(Now, loaded.UserSettings.RecentUsers.Single().LoadedAt);

                string users = string.Join(",", Enumerable.Range(0, 12).Select(i => $"{{\"username\":\"user{ i }\"}}"));
                File.WriteAllText(storage.File(Settings.StorageFile), $"{{\"formatVersion\":1,\"user\":{{\"recentUsers\":[null,{{\"username\":\"!\"}},{{}},{ users }]}}}}");

                Settings repaired = new Settings(storage.Config);
                Assert.Equal(SettingsLoadResult.Loaded, repaired.LoadSettings());
                Assert.Equal(UserController.MaxRecentUsers, repaired.UserSettings.RecentUsers.Count);
                Assert.Equal("user0", repaired.UserSettings.RecentUsers[0].Username);

                File.WriteAllText(storage.File(Settings.StorageFile), "{\"formatVersion\":1,\"user\":{\"recentUsers\":null}}");

                Settings empty = new Settings(storage.Config);
                empty.LoadSettings();
                Assert.Empty(empty.UserSettings.RecentUsers);
            }
        }
    }
}
