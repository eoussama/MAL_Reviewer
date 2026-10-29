using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.users;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class MalListClientTests
    {
        private static MalListClient Client(MockHttpMessageHandler mock) => new MalListClient(TestSupport.Http(mock), TestSupport.MalUrl);

        private static string Page(int count, string list = "anime")
        {
            return JsonSerializer.Serialize(Enumerable.Range(1, count).Select(i => new Dictionary<string, object>
            {
                ["status"] = 1,
                [$"{ list }_title"] = $"Entry { i }"
            }));
        }

        [Fact]
        public async Task GetAnimeList_MapsTheEntries()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.MalUrl + "animelist/Xinil/load.json")
                .WithExactQueryString("status=7&offset=0")
                .Respond("application/json", TestSupport.Fixture("mal_animelist.json"));

            List<AnimelistEntryModel> list = await Client(mock).GetAnimeListAsync("Xinil", TestContext.Current.CancellationToken);

            Assert.Equal(3, list.Count);

            AnimelistEntryModel onePiece = list[0];
            Assert.Equal("One Piece", onePiece.Title);
            Assert.Equal(TestSupport.MalUrl + "anime/21/One_Piece", onePiece.Url);
            Assert.StartsWith("https://cdn.myanimelist.net/", onePiece.Image_url);
            Assert.Equal("TV", onePiece.Type);
            Assert.Equal((byte)9, onePiece.Score);
            Assert.Equal((short)1, onePiece.Watching_status);
            Assert.Equal((short)623, onePiece.Watched_episodes);
            Assert.Null(onePiece.Total_episodes);

            Assert.Equal((short)2, list[1].Watching_status);
            Assert.Equal((short)6, list[2].Watching_status);
            Assert.Null(list[2].Score);
            Assert.Equal((short)12, list[2].Total_episodes);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetMangaList_MapsTheEntries()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.MalUrl + "mangalist/Xinil/load.json").Respond("application/json", TestSupport.Fixture("mal_mangalist.json"));

            List<MangalistEntryModel> list = await Client(mock).GetMangaListAsync("Xinil", TestContext.Current.CancellationToken);

            Assert.Equal(2, list.Count);
            Assert.Equal("All Rounder Meguru", list[0].Title);
            Assert.Equal((short)28, list[0].Read_chapters);
            Assert.Equal((short)0, list[0].Read_volumes);
            Assert.Equal((short)178, list[0].Total_chapters);
            Assert.Equal((short)19, list[0].Total_volumes);
            Assert.Equal((short)1, list[0].Reading_status);
            Assert.Equal("Manga", list[0].Type);
            Assert.Equal((short)4, list[1].Reading_status);
            Assert.Null(list[1].Total_chapters);
            Assert.Null(list[1].Total_volumes);
        }

        [Fact]
        public async Task GetAnimeList_ReadsEveryPage()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.MalUrl + "animelist/big/load.json").WithQueryString("offset", "0").Respond("application/json", Page(MalListClient.PageSize));
            mock.Expect(TestSupport.MalUrl + "animelist/big/load.json").WithQueryString("offset", "300").Respond("application/json", Page(MalListClient.PageSize));
            mock.Expect(TestSupport.MalUrl + "animelist/big/load.json").WithQueryString("offset", "600").Respond("application/json", Page(5));

            List<AnimelistEntryModel> list = await Client(mock).GetAnimeListAsync("big", TestContext.Current.CancellationToken);

            Assert.Equal(605, list.Count);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetAnimeList_StopsOnAnEmptyPage()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.MalUrl + "animelist/exact/load.json").WithQueryString("offset", "0").Respond("application/json", Page(MalListClient.PageSize));
            mock.Expect(TestSupport.MalUrl + "animelist/exact/load.json").WithQueryString("offset", "300").Respond("application/json", "[]");

            Assert.Equal(300, (await Client(mock).GetAnimeListAsync("exact", TestContext.Current.CancellationToken)).Count);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetAnimeList_KeepsTheReadPages_WhenALaterPageIsRefused()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.MalUrl + "animelist/u/load.json").WithQueryString("offset", "0").Respond("application/json", Page(MalListClient.PageSize));
            mock.Expect(TestSupport.MalUrl + "animelist/u/load.json").WithQueryString("offset", "300").Respond(HttpStatusCode.BadRequest);

            Assert.Equal(300, (await Client(mock).GetAnimeListAsync("u", TestContext.Current.CancellationToken)).Count);
        }

        [Fact]
        public async Task GetLists_ReturnNull_ForPrivateListsOrUnknownUsers()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When("*").Respond(HttpStatusCode.BadRequest);

            Assert.Null(await Client(mock).GetAnimeListAsync("private", TestContext.Current.CancellationToken));
            Assert.Null(await Client(mock).GetMangaListAsync("private", TestContext.Current.CancellationToken));
            Assert.Null(await Client(mock).GetMangaListAsync(" ", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task GetAnimeList_KeepsAbsoluteUrls_AndEncodesTheUsername()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.MalUrl + "animelist/a%20b/load.json")
                .Respond("application/json", "[{\"status\":2,\"anime_url\":\"https://elsewhere.test/anime/1\"},{\"status\":2,\"anime_url\":null}]");

            List<AnimelistEntryModel> list = await Client(mock).GetAnimeListAsync("a b", TestContext.Current.CancellationToken);

            Assert.Equal("https://elsewhere.test/anime/1", list[0].Url);
            Assert.Null(list[1].Url);
            mock.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(0, null)]
        [InlineData(11, null)]
        [InlineData(1, (byte)1)]
        [InlineData(10, (byte)10)]
        public void ToScore_TreatsZeroAsNotScored(int? score, byte? expected)
        {
            Assert.Equal(expected, MalListClient.ToScore(score));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(0, null)]
        [InlineData(24, (short)24)]
        [InlineData(100000, short.MaxValue)]
        public void ToKnownCount_TreatsZeroAsUnknown_AndClamps(int? count, short? expected)
        {
            Assert.Equal(expected, MalListClient.ToKnownCount(count));
        }
    }
}
