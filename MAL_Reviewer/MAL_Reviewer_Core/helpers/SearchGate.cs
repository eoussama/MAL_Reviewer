using System;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Decides when a search box should send a new search: the text must be long enough,
    /// and the text or the search kind must differ from the last successful search.
    /// </summary>
    public class SearchGate
    {
        #region Fields

        private string lastText;
        private int? lastKind;

        #endregion

        #region Properties

        /// <summary>
        /// The shortest text that can be searched.
        /// </summary>
        public int MinLength { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="minLength"> The shortest text that can be searched. </param>
        public SearchGate(int minLength = 3)
        {
            this.MinLength = minLength;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Whether a search for the passed text and kind should be sent.
        /// </summary>
        /// <param name="text"> The searched text; surrounding spaces are ignored. </param>
        /// <param name="kind"> The kind of search, (ex: the selected Anime/Manga option). </param>
        public bool ShouldSubmit(string text, int kind)
        {
            string query = text?.Trim() ?? string.Empty;

            if (query.Length < this.MinLength)
            {
                return false;
            }

            return !string.Equals(query, this.lastText, StringComparison.Ordinal) || kind != this.lastKind;
        }

        /// <summary>
        /// Remembers a search that succeeded, so the same search is not sent again.
        /// Failed searches must not be recorded, so they can be retried.
        /// </summary>
        /// <param name="text"> The searched text. </param>
        /// <param name="kind"> The kind of search. </param>
        public void MarkSucceeded(string text, int kind)
        {
            this.lastText = text?.Trim() ?? string.Empty;
            this.lastKind = kind;
        }

        /// <summary>
        /// Forgets the last search, so the next one is always sent.
        /// </summary>
        public void Reset()
        {
            this.lastText = null;
            this.lastKind = null;
        }

        #endregion
    }
}
