using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.sources;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class AniListDataSourceTests
    {
        private static AniListDataSource Source(MockHttpMessageHandler mock) => new AniListDataSource(TestSupport.Http(mock), TestSupport.AniListUrl, TestSupport.MalUrl);

        [Fact]
        public async Task Search_PostsTheQuery_AndSkipsEntriesWithoutAMalId()
        {
            string body = null;
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).Respond(request =>
            {
                body = request.Content.ReadAsStringAsync().Result;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TestSupport.Fixture("anilist_search.json")) };
            });

            SearchResultsModel[] results = await Source(mock).SearchAsync(MediaKind.Anime, " naruto ", TestContext.Current.CancellationToken);

            Assert.Equal(2, results.Length);
            Assert.Equal(20, results[0].Mal_id);
            Assert.Equal("NARUTO", results[0].Title);
            Assert.Equal("TV", results[0].Type);
            Assert.Equal("OVA", results[1].Type);
            Assert.StartsWith("https://s4.anilist.co/", results[0].Image_url);

            using (JsonDocument sent = JsonDocument.Parse(body))
            {
                JsonElement variables = sent.RootElement.GetProperty("variables");

                Assert.Equal("naruto", variables.GetProperty("search").GetString());
                Assert.Equal("ANIME", variables.GetProperty("type").GetString());
                Assert.Equal(10, variables.GetProperty("perPage").GetInt32());
                Assert.Equal(AniListDataSource.SearchQuery, sent.RootElement.GetProperty("query").GetString());
            }
        }

        [Fact]
        public async Task Search_SendsNothing_ForAnEmptyQuery()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();

            Assert.Empty(await Source(mock).SearchAsync(MediaKind.Manga, " ", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task Search_ToleratesAnEmptyAnswer()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).Respond("application/json", "{\"data\": {\"Page\": null}}");

            Assert.Empty(await Source(mock).SearchAsync(MediaKind.Manga, "x", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task GetAnime_MapsTheDetails_ToMalConventions()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl)
                .WithPartialContent("\"idMal\":1")
                .WithPartialContent("\"type\":\"ANIME\"")
                .Respond("application/json", TestSupport.Fixture("anilist_anime.json"));

            AnimeModel anime = await Source(mock).GetAnimeAsync(1, TestContext.Current.CancellationToken);

            Assert.Equal(1, anime.Mal_id);
            Assert.Equal("Cowboy Bebop", anime.Title);
            Assert.Equal(TestSupport.MalUrl + "anime/1", anime.Url);
            Assert.Equal(8.6f, anime.Score);
            Assert.Null(anime.Rank);
            Assert.Equal(26, anime.Episodes);
            Assert.False(anime.Airing);
            Assert.DoesNotContain("<br>", anime.Synopsis);
            Assert.StartsWith("Enter a world in the distant future", anime.Synopsis);
        }

        [Fact]
        public async Task GetManga_MapsTheDetails()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl)
                .WithPartialContent("\"type\":\"MANGA\"")
                .Respond("application/json", TestSupport.Fixture("anilist_manga.json"));

            MangaModel manga = await Source(mock).GetMangaAsync(2, TestContext.Current.CancellationToken);

            Assert.Equal("Berserk", manga.Title);
            Assert.Equal("Manga", manga.Type);
            Assert.True(manga.Publishing);
            Assert.Equal(TestSupport.MalUrl + "manga/2", manga.Url);
            Assert.EndsWith("& more.", manga.Synopsis);
        }

        [Fact]
        public async Task GetAnimeAndManga_ReturnNull_WhenNotFound()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).Respond(HttpStatusCode.NotFound, "application/json", "{\"errors\":[{\"message\":\"Not Found.\",\"status\":404}],\"data\":{\"Media\":null}}");

            Assert.Null(await Source(mock).GetAnimeAsync(999, TestContext.Current.CancellationToken));
            Assert.Null(await Source(mock).GetMangaAsync(999, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task GetAnime_KeepsTheRequestedId_WhenTheAnswerHasNone()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).Respond("application/json", "{\"data\":{\"Media\":{\"title\":{\"english\":\"Only English\"},\"status\":\"RELEASING\"}}}");

            AnimeModel anime = await Source(mock).GetAnimeAsync(77, TestContext.Current.CancellationToken);

            Assert.Equal(77, anime.Mal_id);
            Assert.Equal("Only English", anime.Title);
            Assert.True(anime.Airing);
            Assert.Null(anime.Score);
            Assert.Null(anime.Synopsis);
        }

        [Theory]
        [InlineData("TV", "TV")]
        [InlineData("TV_SHORT", "TV")]
        [InlineData("MOVIE", "Movie")]
        [InlineData("SPECIAL", "Special")]
        [InlineData("OVA", "OVA")]
        [InlineData("ONA", "ONA")]
        [InlineData("MUSIC", "Music")]
        [InlineData("MANGA", "Manga")]
        [InlineData("NOVEL", "Light Novel")]
        [InlineData("ONE_SHOT", "One-shot")]
        [InlineData("SOMETHING_NEW", "SOMETHING_NEW")]
        [InlineData(null, null)]
        public void ToMalType_ConvertsAniListFormats(string format, string expected)
        {
            Assert.Equal(expected, AniListDataSource.ToMalType(format));
        }

        [Theory]
        [InlineData("One<br>Two", "One\nTwo")]
        [InlineData("One<br><br>\nTwo", "One\n\nTwo")]
        [InlineData("<i>Tom &amp; Jerry</i> ", "Tom & Jerry")]
        [InlineData(null, null)]
        public void ToPlainText_RemovesHtml(string description, string expected)
        {
            Assert.Equal(expected, AniListDataSource.ToPlainText(description));
        }

        [Fact]
        public void Name_IsAniList()
        {
            Assert.Equal("AniList", Source(new MockHttpMessageHandler()).Name);
        }
    }
}
