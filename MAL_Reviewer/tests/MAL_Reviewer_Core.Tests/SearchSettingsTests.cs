using System.IO;
using MAL_Reviewer_API.sources;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class SearchControllerTests
    {
        [Fact]
        public void Defaults_FavorFastSearches()
        {
            SearchController settings = new SearchController();

            Assert.True(settings.SearchWhileTyping);
            Assert.Equal(300, settings.Delay);
            Assert.Equal(10, settings.ResultsLimit);
            Assert.Equal(SearchMode.Title, settings.DefaultMode);
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(250, 250)]
        [InlineData(99999, 2000)]
        public void Delay_StaysInRange(int delay, int expected)
        {
            Assert.Equal(expected, new SearchController { Delay = delay }.Delay);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(5, 5)]
        [InlineData(50, 10)]
        public void ResultsLimit_StaysInRange(int limit, int expected)
        {
            Assert.Equal(expected, new SearchController { ResultsLimit = limit }.ResultsLimit);
        }

        [Fact]
        public void DefaultMode_FallsBackToTitle_ForUnknownValues()
        {
            Assert.Equal(SearchMode.Link, new SearchController { DefaultMode = SearchMode.Link }.DefaultMode);
            Assert.Equal(SearchMode.Title, new SearchController { DefaultMode = (SearchMode)42 }.DefaultMode);
        }

        [Fact]
        public void SeedAndReset_RestoreTheDefaults()
        {
            SearchController settings = new SearchController { SearchWhileTyping = false, Delay = 1500, ResultsLimit = 3, DefaultMode = SearchMode.Link };
            settings.SeedSettings();
            Assert.Equal(300, settings.Delay);

            settings.ResultsLimit = 4;
            settings.ResetSettings();
            Assert.True(settings.SearchWhileTyping);
            Assert.Equal(10, settings.ResultsLimit);
            Assert.Equal(SearchMode.Title, settings.DefaultMode);
        }
    }

    public class SearchSettingsFileTests
    {
        [Fact]
        public void SearchSettings_AreSavedAndLoaded()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings saved = new Settings(storage.Config);
                saved.SearchSettings.SearchWhileTyping = false;
                saved.SearchSettings.Delay = 750;
                saved.SearchSettings.ResultsLimit = 5;
                saved.SearchSettings.DefaultMode = SearchMode.Link;
                saved.SaveSettings();

                Assert.Contains("\"defaultMode\": \"Link\"", File.ReadAllText(storage.File(Settings.StorageFile)));

                Settings loaded = new Settings(storage.Config);
                loaded.LoadSettings();

                Assert.False(loaded.SearchSettings.SearchWhileTyping);
                Assert.Equal(750, loaded.SearchSettings.Delay);
                Assert.Equal(5, loaded.SearchSettings.ResultsLimit);
                Assert.Equal(SearchMode.Link, loaded.SearchSettings.DefaultMode);
            }
        }

        [Fact]
        public void FilesWithoutSearchSettings_GetTheDefaults()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), "{\"formatVersion\":1,\"reviewTemplates\":[]}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(300, settings.SearchSettings.Delay);
            }
        }

        [Theory]
        [InlineData("\"link\"", SearchMode.Link)]
        [InlineData("1", SearchMode.Link)]
        [InlineData("\"sideways\"", SearchMode.Title)]
        [InlineData("7", SearchMode.Title)]
        [InlineData("true", SearchMode.Title)]
        [InlineData("{\"x\":1}", SearchMode.Title)]
        public void UnknownValues_DontMakeTheFileUnreadable(string mode, SearchMode expected)
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), $"{{\"search\":{{\"delay\":-1,\"resultsLimit\":99,\"defaultMode\":{ mode },\"searchWhileTyping\":false}}}}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(expected, settings.SearchSettings.DefaultMode);
                Assert.Equal(SearchController.MinDelay, settings.SearchSettings.Delay);
                Assert.Equal(SearchController.MaxResults, settings.SearchSettings.ResultsLimit);
                Assert.False(settings.SearchSettings.SearchWhileTyping);
            }
        }

        [Fact]
        public void ResetAndMigration_IncludeTheSearchSettings()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);
                settings.SearchSettings.Delay = 2000;

                settings.ResetSettings();

                Assert.Equal(300, settings.SearchSettings.Delay);

                storage.Config.PrepareStorageFolder();
                File.WriteAllBytes(storage.File(Settings.LegacyStorageFile), LegacyPayload.Write("someone"));

                Settings migrated = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Migrated, migrated.LoadSettings());
                Assert.Equal(10, migrated.SearchSettings.ResultsLimit);
            }
        }
    }

    public class MalLinkTests
    {
        [Theory]
        [InlineData("https://myanimelist.net/anime/1/Cowboy_Bebop", MediaKind.Anime, 1)]
        [InlineData("  http://www.myanimelist.net/manga/2/Berserk  ", MediaKind.Manga, 2)]
        [InlineData("myanimelist.net/anime/5114", MediaKind.Anime, 5114)]
        [InlineData("https://MyAnimeList.net/ANIME/20/Naruto/reviews?p=2", MediaKind.Anime, 20)]
        [InlineData("https://myanimelist.net/anime/21/", MediaKind.Anime, 21)]
        [InlineData("https://myanimelist.net/anime/21#comments", MediaKind.Anime, 21)]
        [InlineData("https://myanimelist.net/anime.php?id=30", MediaKind.Anime, 30)]
        [InlineData("https://myanimelist.net/manga.php?id=11&x=1", MediaKind.Manga, 11)]
        public void TryParse_ReadsAnimeAndMangaLinks(string text, MediaKind kind, int malId)
        {
            Assert.True(MalLink.TryParse(text, out MediaKind parsedKind, out int parsedId));
            Assert.Equal(kind, parsedKind);
            Assert.Equal(malId, parsedId);
        }

        [Theory]
        [InlineData("Cowboy Bebop")]
        [InlineData("https://myanimelist.net/character/1/Spike_Spiegel")]
        [InlineData("https://myanimelist.net/anime/")]
        [InlineData("https://myanimelist.net/anime/0")]
        [InlineData("https://myanimelist.net/anime/12abc")]
        [InlineData("https://myanimelist.net/anime/99999999999")]
        [InlineData("https://notmyanimelist.net/anime/1")]
        [InlineData("https://myanimelist.net.evil.test/anime/1")]
        [InlineData("https://anilist.co/anime/1")]
        [InlineData("")]
        [InlineData(null)]
        public void TryParse_RejectsAnythingElse(string text)
        {
            Assert.False(MalLink.TryParse(text, out _, out int malId));
            Assert.Equal(0, malId);
        }

        [Fact]
        public void Example_IsAValidLink()
        {
            Assert.True(MalLink.TryParse(MalLink.Example, out _, out _));
        }
    }
}
