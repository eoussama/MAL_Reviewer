using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MAL_Reviewer_API.http
{
    /// <summary>
    /// Sends JSON requests to a single data source, retrying when it is rate limited
    /// and translating every failure into a <see cref="DataSourceUnavailableException"/>.
    /// </summary>
    internal class ApiHttp
    {
        #region Fields

        /// <summary>
        /// The longest wait honored from a Retry-After header.
        /// </summary>
        internal static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The serializer options shared by all of the data sources.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private readonly HttpClient client;
        private readonly string source;
        private readonly int maxRetries;
        private readonly Func<TimeSpan, CancellationToken, Task> delay;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="client"> The HTTP client to send the requests with. </param>
        /// <param name="source"> The name of the data source, used in error messages. </param>
        /// <param name="maxRetries"> How many times a rate limited request is retried. </param>
        /// <param name="delay"> Waits between retries; replaceable for tests. </param>
        public ApiHttp(HttpClient client, string source, int maxRetries, Func<TimeSpan, CancellationToken, Task> delay = null)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.source = source;
            this.maxRetries = Math.Max(0, maxRetries);
            this.delay = delay ?? Task.Delay;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sends a GET request and deserializes the JSON answer.
        /// </summary>
        /// <param name="url"> The absolute URL to request. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <param name="notFoundStatuses"> The statuses that mean "no such data"; 404 when none are given. </param>
        /// <returns> The deserialized answer, or the default value when the data does not exist. </returns>
        public Task<T> GetAsync<T>(string url, CancellationToken ct, params HttpStatusCode[] notFoundStatuses)
        {
            return SendAsync<T>(() => new HttpRequestMessage(HttpMethod.Get, url), ct, notFoundStatuses);
        }

        /// <summary>
        /// Sends a POST request with a JSON body and deserializes the JSON answer.
        /// </summary>
        /// <param name="url"> The absolute URL to request. </param>
        /// <param name="body"> The object to send as JSON. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <param name="notFoundStatuses"> The statuses that mean "no such data"; 404 when none are given. </param>
        /// <returns> The deserialized answer, or the default value when the data does not exist. </returns>
        public Task<T> PostAsync<T>(string url, object body, CancellationToken ct, params HttpStatusCode[] notFoundStatuses)
        {
            return SendAsync<T>(() => new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body, options: JsonOptions) }, ct, notFoundStatuses);
        }

        #endregion

        #region Private methods

        private async Task<T> SendAsync<T>(Func<HttpRequestMessage> createRequest, CancellationToken ct, HttpStatusCode[] notFoundStatuses)
        {
            if (notFoundStatuses == null || notFoundStatuses.Length == 0)
            {
                notFoundStatuses = new[] { HttpStatusCode.NotFound };
            }

            for (int attempt = 0; ; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                using (HttpRequestMessage request = createRequest())
                using (HttpResponseMessage response = await SendOnceAsync(request, ct))
                {
                    if (notFoundStatuses.Contains(response.StatusCode))
                    {
                        return default;
                    }

                    if (IsRetryable(response.StatusCode))
                    {
                        if (attempt >= this.maxRetries)
                        {
                            throw new DataSourceUnavailableException(this.source, $"still answering { (int)response.StatusCode } after { attempt + 1 } attempts");
                        }

                        await this.delay(GetRetryDelay(response, attempt), ct);
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new DataSourceUnavailableException(this.source, $"answered { (int)response.StatusCode } { response.ReasonPhrase }");
                    }

                    try
                    {
                        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
                    }
                    catch (JsonException e)
                    {
                        throw new DataSourceUnavailableException(this.source, "returned an unexpected answer", e);
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SendOnceAsync(HttpRequestMessage request, CancellationToken ct)
        {
            try
            {
                return await this.client.SendAsync(request, ct);
            }
            catch (HttpRequestException e)
            {
                throw new DataSourceUnavailableException(this.source, "could not be reached", e);
            }
            catch (TaskCanceledException e) when (!ct.IsCancellationRequested)
            {
                // A cancellation that the caller did not ask for is the client's timeout.
                throw new DataSourceUnavailableException(this.source, "timed out", e);
            }
        }

        private static bool IsRetryable(HttpStatusCode status)
        {
            return status == HttpStatusCode.TooManyRequests || status == HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Uses the server's Retry-After when it gives one, an exponential backoff otherwise.
        /// </summary>
        /// <param name="response"> The rate limited response. </param>
        /// <param name="attempt"> The zero-based number of the attempt that was rate limited. </param>
        internal static TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
        {
            TimeSpan? retryAfter = response.Headers.RetryAfter?.Delta;

            if (retryAfter == null && response.Headers.RetryAfter?.Date is DateTimeOffset date)
            {
                retryAfter = date - DateTimeOffset.UtcNow;
            }

            TimeSpan wait = retryAfter ?? TimeSpan.FromSeconds(Math.Pow(2, attempt));

            if (wait < TimeSpan.Zero)
            {
                wait = TimeSpan.Zero;
            }

            return wait > MaxRetryDelay ? MaxRetryDelay : wait;
        }

        #endregion
    }
}
