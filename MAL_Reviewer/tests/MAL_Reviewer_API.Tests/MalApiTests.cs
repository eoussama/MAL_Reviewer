using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.sources;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class MalApiTests
    {
        [Fact]
        public async Task Search_FallsBackToAniList_WhenJikanIsDown()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "*").Throw(new HttpRequestException("refused"));
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).Respond("application/json", TestSupport.Fixture("anilist_search.json"));

            using (MalApi api = TestSupport.Api(mock))
            {
                SearchResultsModel[] results = await api.SearchAsync(MediaKind.Anime, "naruto", TestContext.Current.CancellationToken);

                Assert.Equal(20, results[0].Mal_id);
            }
        }

        [Fact]
        public async Task Details_FallBackToAniList_WhenJikanIsRateLimited()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "*").Respond(_ => TestSupport.Status(429));
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).WithPartialContent("ANIME").Respond("application/json", TestSupport.Fixture("anilist_anime.json"));
            mock.When(HttpMethod.Post, TestSupport.AniListUrl).WithPartialContent("MANGA").Respond("application/json", TestSupport.Fixture("anilist_manga.json"));

            using (MalApi api = TestSupport.Api(mock))
            {
                Assert.Equal("Cowboy Bebop", (await api.GetAnimeAsync(1, TestContext.Current.CancellationToken)).Title);
                Assert.Equal("Berserk", (await api.GetMangaAsync(2, TestContext.Current.CancellationToken)).Title);
            }
        }

        [Fact]
        public async Task SendsAUserAgent_AndAsksForJson()
        {
            HttpRequestMessage seen = null;
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "anime/1").Respond(request =>
            {
                seen = request;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TestSupport.Fixture("jikan_anime.json")) };
            });

            using (MalApi api = TestSupport.Api(mock))
            {
                await api.GetAnimeAsync(1, TestContext.Current.CancellationToken);
            }

            Assert.Equal("MAL_Reviewer", seen.Headers.UserAgent.Single().Product.Name);
            Assert.Contains(seen.Headers.Accept, accept => accept.MediaType == "application/json");
        }

        [Fact]
        public async Task User_AndLists_ComeFromJikanAndMyAnimeList()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "users/Xinil/full").Respond("application/json", TestSupport.Fixture("jikan_user_full.json"));
            mock.When(TestSupport.JikanUrl + "users/Xinil/favorites").Respond("application/json", TestSupport.Fixture("jikan_user_favorites.json"));
            mock.When(TestSupport.MalUrl + "animelist/Xinil/load.json").Respond("application/json", TestSupport.Fixture("mal_animelist.json"));
            mock.When(TestSupport.MalUrl + "mangalist/Xinil/load.json").Respond("application/json", TestSupport.Fixture("mal_mangalist.json"));

            using (MalApi api = TestSupport.Api(mock))
            {
                MALUserModel user = await api.GetUserAsync("Xinil", TestContext.Current.CancellationToken);

                Assert.Equal("Xinil", user.Username);
                Assert.Equal(3, (await api.GetAnimeListAsync("Xinil", TestContext.Current.CancellationToken)).Count());
                Assert.Equal(2, (await api.GetMangaListAsync("Xinil", TestContext.Current.CancellationToken)).Count());
            }
        }

        [Fact]
        public void CanBeCreatedWithDefaults()
        {
            using (MalApi api = new MalApi())
            {
                Assert.NotNull(api);
            }
        }
    }

    public class MediaKindTests
    {
        [Theory]
        [InlineData("anime", MediaKind.Anime)]
        [InlineData(" Manga ", MediaKind.Manga)]
        [InlineData("ANIME", MediaKind.Anime)]
        public void Parse_AcceptsAnyCase(string value, MediaKind expected)
        {
            Assert.Equal(expected, MediaKindExtensions.Parse(value));
        }

        [Theory]
        [InlineData("novel")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("5")]
        public void Parse_RejectsUnknownKinds(string value)
        {
            Assert.Throws<ArgumentException>(() => MediaKindExtensions.Parse(value));
        }

        [Fact]
        public void ToPathSegment_IsLowercase()
        {
            Assert.Equal("anime", MediaKind.Anime.ToPathSegment());
            Assert.Equal("manga", MediaKind.Manga.ToPathSegment());
        }
    }

    public class ApiOptionsTests
    {
        [Theory]
        [InlineData("https://a.test/v4/", "anime/1", "https://a.test/v4/anime/1")]
        [InlineData("https://a.test/v4", "/anime/1", "https://a.test/v4/anime/1")]
        [InlineData("https://a.test/", "", "https://a.test/")]
        public void Combine_JoinsWithASingleSlash(string baseUrl, string path, string expected)
        {
            Assert.Equal(expected, ApiOptions.Combine(baseUrl, path));
        }

        [Fact]
        public void Defaults_PointAtThePublicServices()
        {
            ApiOptions options = new ApiOptions();

            Assert.Equal("https://api.jikan.moe/v4/", options.JikanUrl);
            Assert.Equal("https://graphql.anilist.co/", options.AniListUrl);
            Assert.Equal("https://myanimelist.net/", options.MalUrl);
            Assert.True(options.MaxRetries > 0);
        }
    }

    /// <summary>
    /// Tests that change process-wide state (environment variables, the static helper) run one at a time.
    /// </summary>
    [CollectionDefinition(nameof(ProcessState), DisableParallelization = true)]
    public class ProcessState { }

    [Collection(nameof(ProcessState))]
    public class ApiOptionsEnvironmentTests
    {
        [Fact]
        public void FromEnvironment_UsesTheOverrides()
        {
            try
            {
                Environment.SetEnvironmentVariable(ApiOptions.JikanUrlVariable, "http://127.0.0.1:9/");
                Environment.SetEnvironmentVariable(ApiOptions.AniListUrlVariable, "http://127.0.0.1:8/");

                ApiOptions options = ApiOptions.FromEnvironment();

                Assert.Equal("http://127.0.0.1:9/", options.JikanUrl);
                Assert.Equal("http://127.0.0.1:8/", options.AniListUrl);
            }
            finally
            {
                Environment.SetEnvironmentVariable(ApiOptions.JikanUrlVariable, null);
                Environment.SetEnvironmentVariable(ApiOptions.AniListUrlVariable, null);
            }
        }

        [Fact]
        public void FromEnvironment_KeepsTheDefaults_WithoutOverrides()
        {
            Assert.Equal(new ApiOptions().JikanUrl, ApiOptions.FromEnvironment().JikanUrl);
        }
    }

    [Collection(nameof(ProcessState))]
    public class MALHelperTests
    {
        [Fact]
        public async Task ForwardsEveryCall_ToTheConfiguredApi()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "anime").Respond("application/json", TestSupport.Fixture("jikan_anime_search.json"));
            mock.When(TestSupport.JikanUrl + "anime/1").Respond("application/json", TestSupport.Fixture("jikan_anime.json"));
            mock.When(TestSupport.JikanUrl + "manga/2").Respond("application/json", TestSupport.Fixture("jikan_manga.json"));
            mock.When(TestSupport.JikanUrl + "users/Xinil/full").Respond("application/json", TestSupport.Fixture("jikan_user_full.json"));
            mock.When(TestSupport.JikanUrl + "users/Xinil/favorites").Respond("application/json", TestSupport.Fixture("jikan_user_favorites.json"));
            mock.When(TestSupport.MalUrl + "animelist/Xinil/load.json").Respond("application/json", TestSupport.Fixture("mal_animelist.json"));
            mock.When(TestSupport.MalUrl + "mangalist/Xinil/load.json").Respond("application/json", TestSupport.Fixture("mal_mangalist.json"));

            MALHelper.Use(TestSupport.Api(mock));

            Assert.Equal(2, (await MALHelper.Search("Anime", "naruto", TestContext.Current.CancellationToken)).Length);
            Assert.Equal(1, (await MALHelper.GetAnime(1, TestContext.Current.CancellationToken)).Mal_id);
            Assert.Equal(2, (await MALHelper.GetManga(2, TestContext.Current.CancellationToken)).Mal_id);
            Assert.Equal("Xinil", (await MALHelper.GetUser("Xinil", TestContext.Current.CancellationToken)).Username);
            Assert.Equal(3, (await MALHelper.GetAnimeList("Xinil", TestContext.Current.CancellationToken)).Count());
            Assert.Equal(2, (await MALHelper.GetMangaList("Xinil", TestContext.Current.CancellationToken)).Count());
        }

        [Fact]
        public async Task Search_RejectsUnknownTypes()
        {
            MALHelper.Init(new MockHttpMessageHandler());

            await Assert.ThrowsAsync<ArgumentException>(() => MALHelper.Search("novel", "x", TestContext.Current.CancellationToken));
        }

        [Fact]
        public void Use_RejectsNull_AndKeepsTheSameInstance()
        {
            Assert.Throws<ArgumentNullException>(() => MALHelper.Use(null));

            MALHelper.Init();
            IMalApi api = MALHelper.Api;
            MALHelper.Use(api);

            Assert.Same(api, MALHelper.Api);
        }
    }
}
