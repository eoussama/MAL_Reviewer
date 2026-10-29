using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.sources;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class CachedDataSourceTests
    {
        /// <summary>
        /// A data source counting its calls, answering with fresh objects each time.
        /// </summary>
        private class CountingSource : IAnimeDataSource
        {
            public string Name => "Counting";

            public int Searches { get; private set; }

            public int Details { get; private set; }

            public bool Fail { get; set; }

            public bool Missing { get; set; }

            public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, int limit, CancellationToken ct)
            {
                Searches++;
                ThrowIfFailing();
                return Task.FromResult(new[] { new SearchResultsModel { Title = query } });
            }

            public Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct)
            {
                Details++;
                ThrowIfFailing();
                return Task.FromResult(Missing ? null : new AnimeModel { Mal_id = malId });
            }

            public Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct)
            {
                Details++;
                ThrowIfFailing();
                return Task.FromResult(Missing ? null : new MangaModel { Mal_id = malId });
            }

            private void ThrowIfFailing()
            {
                if (Fail)
                {
                    throw new DataSourceUnavailableException(Name, "down");
                }
            }
        }

        [Fact]
        public async Task Search_IsCached_IgnoringCaseAndSpaces()
        {
            CountingSource source = new CountingSource();
            CachedDataSource cache = new CachedDataSource(source);

            SearchResultsModel[] first = await cache.SearchAsync(MediaKind.Anime, "Naruto", 10, TestContext.Current.CancellationToken);
            SearchResultsModel[] second = await cache.SearchAsync(MediaKind.Anime, "  naruto ", 10, TestContext.Current.CancellationToken);

            Assert.Same(first, second);
            Assert.Equal(1, source.Searches);
            Assert.Equal("Counting", cache.Name);
        }

        [Fact]
        public async Task Search_KeepsKindsAndLimitsApart()
        {
            CountingSource source = new CountingSource();
            CachedDataSource cache = new CachedDataSource(source);

            await cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken);
            await cache.SearchAsync(MediaKind.Manga, "x", 10, TestContext.Current.CancellationToken);
            await cache.SearchAsync(MediaKind.Anime, "x", 3, TestContext.Current.CancellationToken);

            Assert.Equal(3, source.Searches);
        }

        [Fact]
        public async Task Details_AreCachedPerKindAndId()
        {
            CountingSource source = new CountingSource();
            CachedDataSource cache = new CachedDataSource(source);

            await cache.GetAnimeAsync(1, TestContext.Current.CancellationToken);
            await cache.GetAnimeAsync(1, TestContext.Current.CancellationToken);
            await cache.GetMangaAsync(1, TestContext.Current.CancellationToken);
            await cache.GetMangaAsync(1, TestContext.Current.CancellationToken);

            Assert.Equal(2, source.Details);
        }

        [Fact]
        public async Task Answers_Expire()
        {
            DateTimeOffset now = DateTimeOffset.UnixEpoch;
            CountingSource source = new CountingSource();
            CachedDataSource cache = new CachedDataSource(source, TimeSpan.FromMinutes(10), now: () => now);

            await cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken);
            now = now.AddMinutes(9);
            await cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken);
            Assert.Equal(1, source.Searches);

            now = now.AddMinutes(2);
            await cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken);
            Assert.Equal(2, source.Searches);
        }

        [Fact]
        public async Task TheOldestAnswer_IsForgottenFirst()
        {
            DateTimeOffset now = DateTimeOffset.UnixEpoch;
            CountingSource source = new CountingSource();
            CachedDataSource cache = new CachedDataSource(source, capacity: 2, now: () => now);

            await cache.SearchAsync(MediaKind.Anime, "a", 10, TestContext.Current.CancellationToken);
            now = now.AddSeconds(1);
            await cache.SearchAsync(MediaKind.Anime, "b", 10, TestContext.Current.CancellationToken);
            now = now.AddSeconds(1);
            await cache.SearchAsync(MediaKind.Anime, "c", 10, TestContext.Current.CancellationToken);

            await cache.SearchAsync(MediaKind.Anime, "c", 10, TestContext.Current.CancellationToken);
            await cache.SearchAsync(MediaKind.Anime, "b", 10, TestContext.Current.CancellationToken);
            Assert.Equal(3, source.Searches);

            await cache.SearchAsync(MediaKind.Anime, "a", 10, TestContext.Current.CancellationToken);
            Assert.Equal(4, source.Searches);
        }

        [Fact]
        public async Task FailuresAndMissingEntries_AreNotCached()
        {
            CountingSource source = new CountingSource { Fail = true };
            CachedDataSource cache = new CachedDataSource(source);

            await Assert.ThrowsAsync<DataSourceUnavailableException>(() => cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken));
            source.Fail = false;
            await cache.SearchAsync(MediaKind.Anime, "x", 10, TestContext.Current.CancellationToken);
            Assert.Equal(2, source.Searches);

            source.Missing = true;
            Assert.Null(await cache.GetAnimeAsync(5, TestContext.Current.CancellationToken));
            source.Missing = false;
            Assert.NotNull(await cache.GetAnimeAsync(5, TestContext.Current.CancellationToken));
            Assert.Equal(2, source.Details);
        }

        [Fact]
        public void RequiresASource()
        {
            Assert.Throws<ArgumentNullException>(() => new CachedDataSource(null));
        }
    }

    public class SearchLimitsTests
    {
        [Theory]
        [InlineData(-5, 1)]
        [InlineData(0, 1)]
        [InlineData(7, 7)]
        [InlineData(99, 25)]
        public void Clamp_KeepsLimitsInRange(int limit, int expected)
        {
            Assert.Equal(expected, SearchLimits.Clamp(limit));
        }

        [Fact]
        public async Task Jikan_ReceivesTheClampedLimit()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(TestSupport.JikanUrl + "anime").WithExactQueryString("q=x&limit=25").Respond("application/json", "{\"data\":[]}");

            await new JikanDataSource(TestSupport.Http(mock), TestSupport.JikanUrl).SearchAsync(MediaKind.Anime, "x", 99, TestContext.Current.CancellationToken);

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task AniList_UsesTheMediumCover_ForSearchResults()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Post, TestSupport.AniListUrl)
                .WithPartialContent("\"perPage\":3")
                .Respond("application/json", "{\"data\":{\"Page\":{\"media\":[{\"idMal\":1,\"coverImage\":{\"medium\":\"m.jpg\",\"large\":\"l.jpg\"}}]}}}");

            SearchResultsModel[] results = await new AniListDataSource(TestSupport.Http(mock), TestSupport.AniListUrl, TestSupport.MalUrl).SearchAsync(MediaKind.Anime, "x", 3, TestContext.Current.CancellationToken);

            Assert.Equal("m.jpg", results[0].Image_url);
        }
    }
}
