using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using RichardSzalay.MockHttp;

namespace MAL_Reviewer_API.Tests
{
    /// <summary>
    /// Shared helpers of the API tests. No test touches the network.
    /// </summary>
    internal static class TestSupport
    {
        public const string JikanUrl = "https://jikan.test/v4/";
        public const string AniListUrl = "https://anilist.test/";
        public const string MalUrl = "https://mal.test/";

        public static string Fixture(string name)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));
        }

        public static ApiOptions Options(int maxRetries = 2)
        {
            return new ApiOptions
            {
                JikanUrl = JikanUrl,
                AniListUrl = AniListUrl,
                MalUrl = MalUrl,
                MaxRetries = maxRetries,
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        /// <summary>
        /// A transport whose retry waits are recorded instead of awaited.
        /// </summary>
        public static ApiHttp Http(MockHttpMessageHandler handler, List<TimeSpan> delays = null, int maxRetries = 2, string source = "Test")
        {
            return new ApiHttp(handler.ToHttpClient(), source, maxRetries, RecordDelay(delays));
        }

        public static Func<TimeSpan, CancellationToken, Task> RecordDelay(List<TimeSpan> delays)
        {
            return (wait, ct) =>
            {
                delays?.Add(wait);
                return Task.CompletedTask;
            };
        }

        public static MalApi Api(MockHttpMessageHandler handler, List<TimeSpan> delays = null)
        {
            return new MalApi(Options(), handler, RecordDelay(delays));
        }

        public static HttpResponseMessage Status(int status)
        {
            return new HttpResponseMessage((System.Net.HttpStatusCode)status);
        }
    }
}
