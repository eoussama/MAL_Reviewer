using System;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.sources;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class FallbackDataSourceTests
    {
        /// <summary>
        /// A data source that either answers with fixed data or is unavailable.
        /// </summary>
        private class FakeSource : IAnimeDataSource
        {
            public FakeSource(string name, bool available, SearchResultsModel[] results = null)
            {
                Name = name;
                Available = available;
                Results = results ?? Array.Empty<SearchResultsModel>();
            }

            public string Name { get; }

            public bool Available { get; set; }

            public SearchResultsModel[] Results { get; }

            public int Calls { get; private set; }

            public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string query, CancellationToken ct) => Answer(Results);

            public Task<AnimeModel> GetAnimeAsync(int malId, CancellationToken ct) => Answer(new AnimeModel { Mal_id = malId, Title = Name });

            public Task<MangaModel> GetMangaAsync(int malId, CancellationToken ct) => Answer(new MangaModel { Mal_id = malId, Title = Name });

            private Task<T> Answer<T>(T value)
            {
                Calls++;

                if (!Available)
                {
                    throw new DataSourceUnavailableException(Name, "down");
                }

                return Task.FromResult(value);
            }
        }

        [Fact]
        public async Task UsesThePrimary_WhenItAnswers()
        {
            FakeSource primary = new FakeSource("Primary", true, new[] { new SearchResultsModel { Mal_id = 1 } });
            FakeSource secondary = new FakeSource("Secondary", true);

            SearchResultsModel[] results = await new FallbackDataSource(primary, secondary).SearchAsync(MediaKind.Anime, "x", TestContext.Current.CancellationToken);

            Assert.Single(results);
            Assert.Equal(0, secondary.Calls);
        }

        [Fact]
        public async Task TrustsAnEmptyAnswerFromThePrimary()
        {
            FakeSource secondary = new FakeSource("Secondary", true, new[] { new SearchResultsModel() });

            SearchResultsModel[] results = await new FallbackDataSource(new FakeSource("Primary", true), secondary).SearchAsync(MediaKind.Anime, "x", TestContext.Current.CancellationToken);

            Assert.Empty(results);
            Assert.Equal(0, secondary.Calls);
        }

        [Fact]
        public async Task SwitchesToTheSecondary_WhenThePrimaryIsUnavailable()
        {
            FallbackDataSource source = new FallbackDataSource(new FakeSource("Primary", false), new FakeSource("Secondary", true));

            Assert.Equal("Secondary", (await source.GetAnimeAsync(1, TestContext.Current.CancellationToken)).Title);
            Assert.Equal("Secondary", (await source.GetMangaAsync(2, TestContext.Current.CancellationToken)).Title);
        }

        [Fact]
        public async Task ReportsBothFailures_WhenEverySourceIsUnavailable()
        {
            FallbackDataSource source = new FallbackDataSource(new FakeSource("Primary", false), new FakeSource("Secondary", false));

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => source.GetAnimeAsync(1, TestContext.Current.CancellationToken));

            AggregateException causes = Assert.IsType<AggregateException>(error.InnerException);
            Assert.Equal(2, causes.InnerExceptions.Count);
            Assert.Equal("Primary / Secondary", error.DataSource);
        }

        [Fact]
        public async Task SkipsTheFailedPrimary_DuringTheCooldown_ThenTriesItAgain()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 10, 29, 12, 0, 0, TimeSpan.Zero);
            FakeSource primary = new FakeSource("Primary", false);
            FakeSource secondary = new FakeSource("Secondary", true);
            FallbackDataSource source = new FallbackDataSource(primary, secondary, TimeSpan.FromMinutes(2), () => now);

            await source.GetAnimeAsync(1, TestContext.Current.CancellationToken);
            now = now.AddMinutes(1);
            await source.GetAnimeAsync(1, TestContext.Current.CancellationToken);

            Assert.Equal(1, primary.Calls);
            Assert.Equal(2, secondary.Calls);

            primary.Available = true;
            now = now.AddMinutes(1);

            Assert.Equal("Primary", (await source.GetAnimeAsync(1, TestContext.Current.CancellationToken)).Title);
            Assert.Equal(2, primary.Calls);
        }

        [Fact]
        public async Task ReportsTheSecondaryFailure_WhileThePrimaryIsSkipped()
        {
            DateTimeOffset now = DateTimeOffset.UnixEpoch;
            FakeSource secondary = new FakeSource("Secondary", true);
            FallbackDataSource source = new FallbackDataSource(new FakeSource("Primary", false), secondary, TimeSpan.FromMinutes(2), () => now);

            await source.GetMangaAsync(1, TestContext.Current.CancellationToken);
            secondary.Available = false;

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => source.GetMangaAsync(1, TestContext.Current.CancellationToken));

            Assert.Equal("Secondary", Assert.IsType<DataSourceUnavailableException>(error.InnerException).DataSource);
        }

        [Fact]
        public void RequiresBothSources()
        {
            Assert.Throws<ArgumentNullException>(() => new FallbackDataSource(null, new FakeSource("S", true)));
            Assert.Throws<ArgumentNullException>(() => new FallbackDataSource(new FakeSource("P", true), null));
        }
    }
}
