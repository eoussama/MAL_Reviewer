using System;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Decides when a search box should send a new search: the text must be long enough,
    /// and the text or the search kind must differ from the search running, or else from the last successful search.
    /// </summary>
    public class SearchGate
    {
        #region Fields

        private string lastText;
        private int? lastKind;

        /// <summary>
        /// The search sent and not finished yet, if any.
        /// </summary>
        private string runningText;
        private int? runningKind;

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
        /// While a search runs, only the same search is skipped: going back to the last successful search must replace
        /// the running one, or its results would show for the wrong search, (ex: Manga results with Anime selected).
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

            return this.runningText != null
                ? !IsSame(query, kind, this.runningText, this.runningKind)
                : !IsSame(query, kind, this.lastText, this.lastKind);
        }

        /// <summary>
        /// Remembers a search that was sent, until it is completed.
        /// </summary>
        /// <param name="text"> The searched text. </param>
        /// <param name="kind"> The kind of search. </param>
        public void MarkSent(string text, int kind)
        {
            this.runningText = text?.Trim() ?? string.Empty;
            this.runningKind = kind;
        }

        /// <summary>
        /// Ends the search sent last. A successful search is not sent again; a failed one can be retried.
        /// </summary>
        /// <param name="succeeded"> Whether the search succeeded. </param>
        public void Complete(bool succeeded)
        {
            if (succeeded && this.runningText != null)
            {
                this.lastText = this.runningText;
                this.lastKind = this.runningKind;
            }

            this.runningText = null;
            this.runningKind = null;
        }

        /// <summary>
        /// Remembers a search that succeeded, so the same search is not sent again.
        /// Failed searches must not be recorded, so they can be retried.
        /// </summary>
        /// <param name="text"> The searched text. </param>
        /// <param name="kind"> The kind of search. </param>
        public void MarkSucceeded(string text, int kind)
        {
            MarkSent(text, kind);
            Complete(true);
        }

        /// <summary>
        /// Forgets the last and the running search, so the next one is always sent.
        /// </summary>
        public void Reset()
        {
            this.lastText = null;
            this.lastKind = null;
            this.runningText = null;
            this.runningKind = null;
        }

        #endregion

        #region Private methods

        private static bool IsSame(string text, int kind, string otherText, int? otherKind)
        {
            return string.Equals(text, otherText, StringComparison.Ordinal) && kind == otherKind;
        }

        #endregion
    }
}
