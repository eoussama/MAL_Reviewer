using System;

namespace MAL_Reviewer_API
{
    /// <summary>
    /// Thrown when a remote data source could not be reached or answered with an error,
    /// as opposed to a successful answer that simply contains no data.
    /// </summary>
    public class DataSourceUnavailableException : Exception
    {
        /// <summary>
        /// The name of the data source that failed, (ex: Jikan, AniList, MyAnimeList).
        /// </summary>
        public string DataSource { get; }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="dataSource"> The name of the data source that failed. </param>
        /// <param name="message"> The reason of the failure. </param>
        /// <param name="innerException"> The underlying error, if any. </param>
        public DataSourceUnavailableException(string dataSource, string message, Exception innerException = null)
            : base($"{ dataSource } is unavailable: { message }", innerException)
        {
            this.DataSource = dataSource;
        }
    }
}
