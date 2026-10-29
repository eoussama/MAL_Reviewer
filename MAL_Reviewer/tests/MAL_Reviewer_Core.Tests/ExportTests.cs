using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.services;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class ListExportTests
    {
        private static readonly AnimelistEntryModel[] Anime =
        {
            new AnimelistEntryModel { Mal_id = 21, Title = "One Piece", Type = "TV", Score = 9, Watching_status = 1, Watched_episodes = 623, Total_episodes = null, Tags = "action, pirates", Url = "https://myanimelist.net/anime/21/One_Piece" },
            new AnimelistEntryModel { Mal_id = 48, Title = ".hack//Sign", Type = "TV", Score = 0, Watching_status = 2, Watched_episodes = 26, Total_episodes = 26 },
            new AnimelistEntryModel { Mal_id = 99, Title = "Unknown status", Watching_status = 5 },
            null,
            new AnimelistEntryModel { Mal_id = 1, Title = "Tom & \"Jerry\", <again>", Watching_status = 6 }
        };

        private static readonly MangalistEntryModel[] Manga =
        {
            new MangalistEntryModel { Mal_id = 14090, Title = "All Rounder Meguru", Type = "Manga", Score = 8, Reading_status = 1, Read_chapters = 28, Read_volumes = 0, Total_chapters = 178, Total_volumes = 19 },
            new MangalistEntryModel { Mal_id = 2, Title = "Berserk", Reading_status = 3 },
            new MangalistEntryModel { Mal_id = 3, Title = "Dropped one", Reading_status = 4 }
        };

        [Fact]
        public void AnimeXml_FollowsMyAnimeListsExport()
        {
            byte[] file = ListExport.AnimeList("Xinil", Anime, ListFileFormat.MalXml);

            Assert.NotEqual(0xEF, file[0]);
            XElement root = XDocument.Parse(Encoding.UTF8.GetString(file)).Root;

            Assert.Equal("myanimelist", root.Name.LocalName);
            XElement info = root.Element("myinfo");
            Assert.Equal("Xinil", (string)info.Element("user_name"));
            Assert.Equal(1, (int)info.Element("user_export_type"));
            Assert.Equal(3, (int)info.Element("user_total_anime"));
            Assert.Equal(1, (int)info.Element("user_total_watching"));
            Assert.Equal(1, (int)info.Element("user_total_completed"));
            Assert.Equal(0, (int)info.Element("user_total_onhold"));
            Assert.Equal(1, (int)info.Element("user_total_plantowatch"));

            XElement[] entries = root.Elements("anime").ToArray();
            Assert.Equal(3, entries.Length);

            Assert.Equal(21, (int)entries[0].Element("series_animedb_id"));
            Assert.Equal("One Piece", (string)entries[0].Element("series_title"));
            Assert.Equal(0, (int)entries[0].Element("series_episodes"));
            Assert.Equal(623, (int)entries[0].Element("my_watched_episodes"));
            Assert.Equal(9, (int)entries[0].Element("my_score"));
            Assert.Equal("Watching", (string)entries[0].Element("my_status"));
            Assert.Equal("action, pirates", (string)entries[0].Element("my_tags"));
            Assert.Equal("0000-00-00", (string)entries[0].Element("my_start_date"));
            Assert.Equal(1, (int)entries[0].Element("update_on_import"));

            Assert.Equal("Completed", (string)entries[1].Element("my_status"));
            Assert.Equal(0, (int)entries[1].Element("my_score"));
            Assert.Equal("Tom & \"Jerry\", <again>", (string)entries[2].Element("series_title"));
            Assert.Equal("Plan to Watch", (string)entries[2].Element("my_status"));
        }

        [Fact]
        public void MangaXml_FollowsMyAnimeListsExport()
        {
            XElement root = XDocument.Parse(Encoding.UTF8.GetString(ListExport.MangaList("Xinil", Manga, ListFileFormat.MalXml))).Root;

            XElement info = root.Element("myinfo");
            Assert.Equal(2, (int)info.Element("user_export_type"));
            Assert.Equal(3, (int)info.Element("user_total_manga"));
            Assert.Equal(1, (int)info.Element("user_total_reading"));
            Assert.Equal(1, (int)info.Element("user_total_onhold"));
            Assert.Equal(1, (int)info.Element("user_total_dropped"));
            Assert.Equal(0, (int)info.Element("user_total_plantoread"));

            XElement meguru = root.Elements("manga").First();
            Assert.Equal(14090, (int)meguru.Element("manga_mangadb_id"));
            Assert.Equal(19, (int)meguru.Element("manga_volumes"));
            Assert.Equal(178, (int)meguru.Element("manga_chapters"));
            Assert.Equal(28, (int)meguru.Element("my_read_chapters"));
            Assert.Equal("Reading", (string)meguru.Element("my_status"));
            Assert.Equal(new[] { "Reading", "On-Hold", "Dropped" }, root.Elements("manga").Select(entry => (string)entry.Element("my_status")));
        }

        [Fact]
        public void AnimeCsv_HasOneRowPerEntry_QuotedWhenNeeded()
        {
            byte[] file = ListExport.AnimeList("Xinil", Anime, ListFileFormat.Csv);

            Assert.Equal(Encoding.UTF8.GetPreamble(), file.Take(3));
            string[] lines = Encoding.UTF8.GetString(file.Skip(3).ToArray()).Split("\r\n");

            Assert.Equal("MAL id,Title,Type,Status,Score,Episodes watched,Episodes,Tags,Link", lines[0]);
            Assert.Equal("21,One Piece,TV,Watching,9,623,,\"action, pirates\",https://myanimelist.net/anime/21/One_Piece", lines[1]);
            Assert.Equal("48,.hack//Sign,TV,Completed,,26,26,,", lines[2]);
            Assert.Equal("99,Unknown status,,,,,,,", lines[3]);
            Assert.Equal("1,\"Tom & \"\"Jerry\"\", <again>\",,Plan to Watch,,,,,", lines[4]);
            Assert.Equal("", lines[5]);
            Assert.Equal(6, lines.Length);
        }

        [Fact]
        public void MangaCsv_HasTheMangaColumns()
        {
            string[] lines = Encoding.UTF8.GetString(ListExport.MangaList("Xinil", Manga, ListFileFormat.Csv).Skip(3).ToArray()).Split("\r\n");

            Assert.Equal("MAL id,Title,Type,Status,Score,Chapters read,Chapters,Volumes read,Volumes,Tags,Link", lines[0]);
            Assert.Equal("14090,All Rounder Meguru,Manga,Reading,8,28,178,0,19,,", lines[1]);
            Assert.Equal("2,Berserk,,On Hold,,,,,,,", lines[2]);
        }

        [Fact]
        public void EmptyLists_GiveEmptyFiles()
        {
            XElement root = XDocument.Parse(Encoding.UTF8.GetString(ListExport.AnimeList(null, null, ListFileFormat.MalXml))).Root;

            Assert.Equal("", (string)root.Element("myinfo").Element("user_name"));
            Assert.Empty(root.Elements("anime"));
            Assert.Single(Encoding.UTF8.GetString(ListExport.MangaList("Xinil", null, ListFileFormat.Csv).Skip(3).ToArray()).Split("\r\n", StringSplitOptions.RemoveEmptyEntries));
        }

        [Theory]
        [InlineData(TargetType.Anime, ListFileFormat.MalXml, "Xinil_animelist.xml")]
        [InlineData(TargetType.Manga, ListFileFormat.Csv, "Xinil_mangalist.csv")]
        public void FileName_NamesTheUserAndTheList(TargetType type, ListFileFormat format, string expected)
        {
            Assert.Equal(expected, ListExport.FileName("Xinil", type, format));
        }

        [Theory]
        [InlineData("list.CSV", ListFileFormat.MalXml, ListFileFormat.Csv)]
        [InlineData("list.xml", ListFileFormat.Csv, ListFileFormat.MalXml)]
        [InlineData("list.txt", ListFileFormat.Csv, ListFileFormat.Csv)]
        [InlineData("list", ListFileFormat.MalXml, ListFileFormat.MalXml)]
        [InlineData(null, ListFileFormat.Csv, ListFileFormat.Csv)]
        public void FormatOf_FollowsTheExtension(string path, ListFileFormat chosen, ListFileFormat expected)
        {
            Assert.Equal(expected, ListExport.FormatOf(path, chosen));
        }
    }

    public class SettingsExportTests
    {
        [Fact]
        public void ExportedSettings_AreImportedBack()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings exported = new Settings(storage.Config);
                exported.LoadSettings();
                exported.UserSettings.DefaultUser = "Xinil";
                exported.SearchSettings.Delay = 900;
                exported.ThemeSettings.Theme = ThemeKind.Dark;
                exported.ReviewTemplatesSettings.ReviewTemplates.RemoveAt(0);

                storage.Config.PrepareStorageFolder();
                string file = Path.Combine(storage.Root, "backup.json");
                exported.ExportSettings(file);

                Settings imported = new Settings(storage.Config);
                imported.LoadSettings();
                imported.ImportSettings(file);

                Assert.Equal("Xinil", imported.UserSettings.DefaultUser);
                Assert.Equal(900, imported.SearchSettings.Delay);
                Assert.Equal(ThemeKind.Dark, imported.ThemeSettings.Theme);
                Assert.Single(imported.ReviewTemplatesSettings.ReviewTemplates);

                // The import is saved.
                Settings reloaded = new Settings(storage.Config);
                reloaded.LoadSettings();
                Assert.Equal("Xinil", reloaded.UserSettings.DefaultUser);
            }
        }

        [Fact]
        public void PartialFiles_AreRepaired()
        {
            using (TempStorage storage = new TempStorage())
            {
                string file = Path.Combine(storage.Root, "partial.json");
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(file, "{\"formatVersion\":1,\"search\":{\"delay\":5}}");

                Settings settings = new Settings(storage.Config);
                settings.ImportSettings(file);

                Assert.Equal(SearchController.MinDelay, settings.SearchSettings.Delay);
                Assert.Equal(string.Empty, settings.UserSettings.DefaultUser);
                Assert.Empty(settings.ReviewTemplatesSettings.ReviewTemplates);
                Assert.Equal(ThemeKind.Light, settings.ThemeSettings.Theme);
            }
        }

        [Theory]
        [InlineData("not json", "readable")]
        [InlineData("[1, 2]", "readable")]
        [InlineData("null", "exported by MAL Reviewer")]
        [InlineData("{}", "exported by MAL Reviewer")]
        [InlineData("{\"formatVersion\":0}", "exported by MAL Reviewer")]
        [InlineData("{\"formatVersion\":2}", "newer version")]
        public void OtherFiles_AreRefused_KeepingTheSettings(string content, string reason)
        {
            using (TempStorage storage = new TempStorage())
            {
                string file = Path.Combine(storage.Root, "other.json");
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(file, content);

                Settings settings = new Settings(storage.Config);
                settings.UserSettings.DefaultUser = "Xinil";

                InvalidDataException error = Assert.Throws<InvalidDataException>(() => settings.ImportSettings(file));

                Assert.Contains(reason, error.Message);
                Assert.Equal("Xinil", settings.UserSettings.DefaultUser);
                Assert.False(File.Exists(storage.File(Settings.StorageFile)));
            }
        }
    }
}
