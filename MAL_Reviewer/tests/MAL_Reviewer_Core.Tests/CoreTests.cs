using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core.services;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class UserLoaderTests
    {
        [Fact]
        public async Task LoadAsync_CombinesTheProfileAndLists()
        {
            FakeMalApi api = new FakeMalApi
            {
                User = new MALUserModel { Username = "Xinil" },
                AnimeList = new[] { new AnimelistEntryModel { Title = "Bebop" } },
                MangaList = null
            };

            MALUserModel user = await new UserLoader(api).LoadAsync("  Xinil ", TestContext.Current.CancellationToken);

            Assert.Equal("Xinil", user.Username);
            Assert.Equal("Bebop", Assert.Single(user.AnimeList).Title);
            Assert.Null(user.MangaList);
            Assert.Equal(new[] { "Xinil" }, api.RequestedUsers);
        }

        [Fact]
        public async Task LoadAsync_ReturnsNull_ForUnknownUsers()
        {
            Assert.Null(await new UserLoader(new FakeMalApi()).LoadAsync("nobody", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task LoadAsync_RejectsInvalidNames_WithoutCallingTheApi()
        {
            FakeMalApi api = new FakeMalApi();

            await Assert.ThrowsAsync<ArgumentException>(() => new UserLoader(api).LoadAsync("x", TestContext.Current.CancellationToken));
            Assert.Empty(api.RequestedUsers);
        }

        [Fact]
        public async Task LoadAsync_ReportsUnavailableServices()
        {
            FakeMalApi api = new FakeMalApi
            {
                User = new MALUserModel { Username = "Xinil" },
                ListError = new DataSourceUnavailableException("MyAnimeList", "down")
            };

            await Assert.ThrowsAsync<DataSourceUnavailableException>(() => new UserLoader(api).LoadAsync("Xinil", TestContext.Current.CancellationToken));
        }

        [Fact]
        public void RequiresAnApi()
        {
            Assert.Throws<ArgumentNullException>(() => new UserLoader(null));
        }
    }

    public class ConfigTests
    {
        [Fact]
        public void StoresDataInItsOwnFolder()
        {
            using (TempStorage storage = new TempStorage())
            {
                Assert.Equal(Path.Combine(storage.Root, "MAL_Reviewer"), storage.Config.StorageDirectory);
                Assert.Equal(Path.Combine(storage.Root, "MAL_Reviewer", "a.json"), storage.Config.GetStorageFilePath("a.json"));
                Assert.False(storage.Config.DoesStorageFileExist("a.json"));

                storage.Config.PrepareStorageFolder();
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File("a.json"), "{}");

                Assert.True(storage.Config.DoesStorageFileExist("a.json"));
            }
        }

        [Fact]
        public void DefaultsToTheApplicationDataFolder()
        {
            Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), new Config().StoragePath);
        }
    }

    [Collection(nameof(CoreState))]
    public class CoreTests
    {
        [Fact]
        public void Init_LoadsTheSettings_AndUsesThePassedApi()
        {
            using (TempStorage storage = new TempStorage())
            {
                FakeMalApi api = new FakeMalApi();

                Core.Init(storage.Config, api);

                Assert.Same(storage.Config, Core.Configurations);
                Assert.Equal(SettingsLoadResult.Seeded, Core.LoadResult);
                Assert.Equal(2, Core.Settings.ReviewTemplatesSettings.Count);
                Assert.Same(api, MALHelper.Api);
                Assert.Null(Core.User);
            }
        }

        [Fact]
        public void Save_PersistsTheSettings()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config, new FakeMalApi());
                Core.Settings.UserSettings.DefaultUser = "Xinil";

                Core.Save();
                Core.Init(storage.Config, new FakeMalApi());

                Assert.Equal(SettingsLoadResult.Loaded, Core.LoadResult);
                Assert.Equal("Xinil", Core.Settings.UserSettings.DefaultUser);
            }
        }

        [Fact]
        public void Init_WithoutAnApi_UsesTheFreeServices()
        {
            using (TempStorage storage = new TempStorage())
            {
                Core.Init(storage.Config);

                Assert.IsType<MalApi>(MALHelper.Api);
            }
        }

        [Fact]
        public void SetUser_KeepsTheProfile_WithPlaceholdersForMissingFields()
        {
            Core.SetUser(new MALUserModel { Username = "Xinil", Joined = new DateTime(2004, 11, 5) });

            Assert.Equal("Xinil", Core.User.Username);
            Assert.Equal("?", Core.User.Gender);
            Assert.Equal("?", Core.User.Url);
            Assert.Equal("Such empty!", Core.User.About);
            Assert.Equal(new DateTime(2004, 11, 5), Core.User.Joined);

            Core.UnloadUser();
            Assert.Null(Core.User);
        }

        [Fact]
        public void SetUser_WithNull_Unloads()
        {
            Core.SetUser(new MALUserModel { Username = "Xinil" });
            Core.SetUser(null);

            Assert.Null(Core.User);
        }
    }
}
