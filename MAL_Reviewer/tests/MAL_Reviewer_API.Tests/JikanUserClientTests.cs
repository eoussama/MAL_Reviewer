using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.users;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class JikanUserClientTests
    {
        private static JikanUserClient Client(MockHttpMessageHandler mock) => new JikanUserClient(TestSupport.Http(mock), TestSupport.JikanUrl);

        [Fact]
        public async Task GetUser_MapsTheProfile_AndFetchesFavoritesSeparately()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "users/Xinil/full").Respond("application/json", TestSupport.Fixture("jikan_user_full.json"));
            mock.Expect(TestSupport.JikanUrl + "users/Xinil/favorites").Respond("application/json", TestSupport.Fixture("jikan_user_favorites.json"));

            MALUserModel user = await Client(mock).GetUserAsync(" Xinil ", TestContext.Current.CancellationToken);

            Assert.Equal("Xinil", user.Username);
            Assert.Equal("https://myanimelist.net/profile/Xinil", user.Url);
            Assert.Equal("https://cdn.myanimelist.net/images/userimages/1.jpg", user.Image_url);
            Assert.Equal("Male", user.Gender);
            Assert.Equal("California", user.Location);
            Assert.Null(user.About);
            Assert.Equal(new DateTime(1985, 3, 4), user.Birthday);
            Assert.Equal(new DateTime(2004, 11, 5), user.Joined);

            Assert.Equal(297, user.Anime_stats.Total_entries);
            Assert.Equal(120.5, user.Anime_stats.Days_watched);
            Assert.Equal(7.42, user.Anime_stats.Mean_score);
            Assert.Equal(3, user.Anime_stats.Watching);
            Assert.Equal(250, user.Anime_stats.Completed);
            Assert.Equal(4, user.Anime_stats.On_hold);
            Assert.Equal(10, user.Anime_stats.Dropped);
            Assert.Equal(30, user.Anime_stats.Plan_to_watch);
            Assert.Equal(5, user.Anime_stats.Rewatched);
            Assert.Equal(7000, user.Anime_stats.Episodes_watched);

            Assert.Equal(76, user.Manga_stats.Total_entries);
            Assert.Equal(12.25, user.Manga_stats.Days_read);
            Assert.Equal(2, user.Manga_stats.Reading);
            Assert.Equal(10, user.Manga_stats.Plan_to_read);
            Assert.Equal(0, user.Manga_stats.Reread);
            Assert.Equal(4000, user.Manga_stats.Chapters_read);
            Assert.Equal(300, user.Manga_stats.Volumes_read);

            Assert.Equal(new[] { "Cowboy Bebop", "Naruto" }, Array.ConvertAll(user.Favorites.Anime, favorite => favorite.Name));
            Assert.Equal(1, user.Favorites.Anime[0].Mal_id);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/4/19644.jpg", user.Favorites.Anime[0].Image_url);
            Assert.Single(user.Favorites.Manga);
            Assert.Equal("Spiegel, Spike", user.Favorites.Characters[0].Name);
            Assert.Empty(user.Favorites.People);

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetUser_UsesEmbeddedFavorites_AndKeepsMissingPartsEmpty()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "users/someone/full").Respond("application/json",
                "{\"data\":{\"username\":\"someone\",\"about\":\"Hi\",\"favorites\":{\"anime\":[{\"mal_id\":5,\"title\":\"A\"}]}}}");

            MALUserModel user = await Client(mock).GetUserAsync("someone", TestContext.Current.CancellationToken);

            Assert.Equal("Hi", user.About);
            Assert.Equal("A", Assert.Single(user.Favorites.Anime).Name);
            Assert.Empty(user.Favorites.Manga);
            Assert.Empty(user.Favorites.Characters);
            Assert.NotNull(user.Anime_stats);
            Assert.Null(user.Anime_stats.Total_entries);
            Assert.NotNull(user.Manga_stats);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetUser_ToleratesMissingFavorites()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(TestSupport.JikanUrl + "users/someone/full").Respond("application/json", "{\"data\":{\"username\":\"someone\"}}");
            mock.When(TestSupport.JikanUrl + "users/someone/favorites").Respond(HttpStatusCode.NotFound);

            MALUserModel user = await Client(mock).GetUserAsync("someone", TestContext.Current.CancellationToken);

            Assert.Empty(user.Favorites.Anime);
            Assert.Empty(user.Favorites.People);
        }

        [Fact]
        public async Task GetUser_EncodesTheUsername()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "users/a%2Fb/full").Respond(HttpStatusCode.NotFound);

            Assert.Null(await Client(mock).GetUserAsync("a/b", TestContext.Current.CancellationToken));
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetUser_ReturnsNull_ForUnknownOrEmptyUsers()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When("*").Respond(HttpStatusCode.NotFound);

            Assert.Null(await Client(mock).GetUserAsync("nobody", TestContext.Current.CancellationToken));
            Assert.Null(await Client(mock).GetUserAsync("  ", TestContext.Current.CancellationToken));
            Assert.Null(await Client(mock).GetUserAsync(null, TestContext.Current.CancellationToken));
        }
    }
}
