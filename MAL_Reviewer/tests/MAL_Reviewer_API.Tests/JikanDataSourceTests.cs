using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.sources;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class JikanDataSourceTests
    {
        private static JikanDataSource Source(MockHttpMessageHandler mock) => new JikanDataSource(TestSupport.Http(mock), TestSupport.JikanUrl);

        [Fact]
        public async Task Search_MapsTheResults()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "anime")
                .WithExactQueryString("q=naruto&limit=10")
                .Respond("application/json", TestSupport.Fixture("jikan_anime_search.json"));

            SearchResultsModel[] results = await Source(mock).SearchAsync(MediaKind.Anime, "  naruto ", 10, TestContext.Current.CancellationToken);

            Assert.Equal(2, results.Length);
            Assert.Equal(20, results[0].Mal_id);
            Assert.Equal("Naruto", results[0].Title);
            Assert.Equal("TV", results[0].Type);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/1141/142503.jpg", results[0].Image_url);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Search_EncodesTheQuery_AndUsesTheMangaEndpoint()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "manga")
                .WithQueryString("q", "Fullmetal & Alchemist?")
                .Respond("application/json", "{\"data\": []}");

            SearchResultsModel[] results = await Source(mock).SearchAsync(MediaKind.Manga, "Fullmetal & Alchemist?", 10, TestContext.Current.CancellationToken);

            Assert.Empty(results);
            mock.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Search_SendsNothing_ForAnEmptyQuery(string query)
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();

            Assert.Empty(await Source(mock).SearchAsync(MediaKind.Anime, query, 10, TestContext.Current.CancellationToken));
            Assert.Equal(0, mock.GetMatchCount(mock.Fallback));
        }

        [Fact]
        public async Task Search_ToleratesAMissingDataArray()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "anime*").Respond("application/json", "{}");

            Assert.Empty(await Source(mock).SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task GetAnime_MapsTheDetails()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "anime/1").Respond("application/json", TestSupport.Fixture("jikan_anime.json"));

            AnimeModel anime = await Source(mock).GetAnimeAsync(1, TestContext.Current.CancellationToken);

            Assert.Equal(1, anime.Mal_id);
            Assert.Equal("Cowboy Bebop", anime.Title);
            Assert.Equal("https://myanimelist.net/anime/1/Cowboy_Bebop", anime.Url);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/4/19644.jpg", anime.Image_url);
            Assert.Equal("TV", anime.Type);
            Assert.Equal(8.75f, anime.Score);
            Assert.Equal(46, anime.Rank);
            Assert.Equal(26, anime.Episodes);
            Assert.False(anime.Airing);
            Assert.StartsWith("Crime is timeless", anime.Synopsis);
        }

        [Fact]
        public async Task GetManga_MapsTheDetails()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "manga/2").Respond("application/json", TestSupport.Fixture("jikan_manga.json"));

            MangaModel manga = await Source(mock).GetMangaAsync(2, TestContext.Current.CancellationToken);

            Assert.Equal("Berserk", manga.Title);
            Assert.True(manga.Publishing);
            Assert.Null(manga.Chapters);
            Assert.Null(manga.Volumes);
            Assert.Equal(1, manga.Rank);
        }

        [Fact]
        public async Task GetAnimeAndManga_ReturnNull_WhenNotFound()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When("*").Respond(HttpStatusCode.NotFound);

            Assert.Null(await Source(mock).GetAnimeAsync(999, TestContext.Current.CancellationToken));
            Assert.Null(await Source(mock).GetMangaAsync(999, TestContext.Current.CancellationToken));
        }

        [Fact]
        public void Name_IsJikan()
        {
            Assert.Equal("Jikan", Source(new MockHttpMessageHandler()).Name);
        }
    }
}
