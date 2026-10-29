using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API.http;
using RichardSzalay.MockHttp;
using Xunit;

namespace MAL_Reviewer_API.Tests
{
    public class ApiHttpTests
    {
        private const string Url = "https://source.test/data";

        private class Payload
        {
            public int Value { get; set; }
        }

        [Fact]
        public async Task Get_DeserializesTheAnswer()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond("application/json", "{\"value\": 42}");

            Payload payload = await TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken);

            Assert.Equal(42, payload.Value);
        }

        [Fact]
        public async Task Get_ReadsNumbersWrittenAsStrings()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond("application/json", "{\"value\": \"7\"}");

            Payload payload = await TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken);

            Assert.Equal(7, payload.Value);
        }

        [Fact]
        public async Task Get_ReturnsDefault_On404ByDefault()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond(HttpStatusCode.NotFound);

            Assert.Null(await TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task Get_UsesThePassedNotFoundStatuses()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond(HttpStatusCode.BadRequest);

            Assert.Null(await TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken, HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Get_Throws_When404IsNotANotFoundStatus()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond(HttpStatusCode.NotFound);

            await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken, HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Get_Throws_OnServerErrors_WithTheSourceName()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond(HttpStatusCode.InternalServerError);

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock, source: "Jikan").GetAsync<Payload>(Url, TestContext.Current.CancellationToken));

            Assert.Equal("Jikan", error.DataSource);
            Assert.Contains("500", error.Message);
        }

        [Fact]
        public async Task Get_Throws_OnMalformedJson()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond("application/json", "<html>maintenance</html>");

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken));

            Assert.IsAssignableFrom<System.Text.Json.JsonException>(error.InnerException);
        }

        [Fact]
        public async Task Get_Throws_WhenTheSourceCannotBeReached()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Throw(new HttpRequestException("connection refused"));

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken));

            Assert.IsType<HttpRequestException>(error.InnerException);
        }

        [Fact]
        public async Task Get_TreatsAnUnrequestedCancellationAsATimeout()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Throw(new TaskCanceledException("timeout"));

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock).GetAsync<Payload>(Url, TestContext.Current.CancellationToken));

            Assert.Contains("timed out", error.Message);
        }

        [Fact]
        public async Task Get_HonorsTheCallersCancellation()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond("application/json", "{}");

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => TestSupport.Http(mock).GetAsync<Payload>(Url, cts.Token));
            }
        }

        [Fact]
        public async Task Get_RetriesRateLimitedRequests_HonoringRetryAfter()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(Url).Respond(_ =>
            {
                HttpResponseMessage response = TestSupport.Status(429);
                response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(3));
                return response;
            });
            mock.Expect(Url).Respond(HttpStatusCode.ServiceUnavailable);
            mock.Expect(Url).Respond("application/json", "{\"value\": 1}");

            List<TimeSpan> delays = new List<TimeSpan>();
            Payload payload = await TestSupport.Http(mock, delays).GetAsync<Payload>(Url, TestContext.Current.CancellationToken);

            Assert.Equal(1, payload.Value);
            Assert.Equal(new[] { TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(2) }, delays);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Get_GivesUp_AfterTheMaximumRetries()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.When(Url).Respond(_ => TestSupport.Status(429));

            List<TimeSpan> delays = new List<TimeSpan>();

            DataSourceUnavailableException error = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => TestSupport.Http(mock, delays, maxRetries: 2).GetAsync<Payload>(Url, TestContext.Current.CancellationToken));

            Assert.Equal(2, delays.Count);
            Assert.Contains("3 attempts", error.Message);
        }

        [Fact]
        public async Task Post_SendsTheBodyAsJson()
        {
            MockHttpMessageHandler mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, Url)
                .WithContent("{\"query\":\"q\",\"variables\":{\"id\":5}}")
                .Respond("application/json", "{\"value\": 5}");

            Payload payload = await TestSupport.Http(mock).PostAsync<Payload>(Url, new { query = "q", variables = new { id = 5 } }, TestContext.Current.CancellationToken);

            Assert.Equal(5, payload.Value);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void Constructor_RequiresAClient()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiHttp(null, "Test", 1));
        }

        [Fact]
        public void RetryDelay_UsesTheRetryAfterDelta()
        {
            HttpResponseMessage response = TestSupport.Status(429);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(4));

            Assert.Equal(TimeSpan.FromSeconds(4), ApiHttp.GetRetryDelay(response, 0));
        }

        [Fact]
        public void RetryDelay_UsesTheRetryAfterDate()
        {
            HttpResponseMessage response = TestSupport.Status(429);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(5));

            TimeSpan wait = ApiHttp.GetRetryDelay(response, 0);

            Assert.InRange(wait, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void RetryDelay_NeverWaitsForAPastDate()
        {
            HttpResponseMessage response = TestSupport.Status(429);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(-1));

            Assert.Equal(TimeSpan.Zero, ApiHttp.GetRetryDelay(response, 0));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        public void RetryDelay_BacksOffExponentially_WithoutRetryAfter(int attempt, int seconds)
        {
            Assert.Equal(TimeSpan.FromSeconds(seconds), ApiHttp.GetRetryDelay(TestSupport.Status(429), attempt));
        }

        [Fact]
        public void RetryDelay_IsCapped()
        {
            HttpResponseMessage response = TestSupport.Status(429);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromHours(1));

            Assert.Equal(ApiHttp.MaxRetryDelay, ApiHttp.GetRetryDelay(response, 0));
        }
    }
}
