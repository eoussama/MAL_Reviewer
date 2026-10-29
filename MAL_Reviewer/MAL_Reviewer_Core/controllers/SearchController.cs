using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.interfaces;

namespace MAL_Reviewer_Core.controllers
{
    /// <summary>
    /// The settings of the Anime/Manga search. Values out of range are brought back within it.
    /// </summary>
    public class SearchController : ISettings
    {
        #region Fields

        /// <summary>
        /// The shortest typing delay, in milliseconds.
        /// </summary>
        public const int MinDelay = 100;

        /// <summary>
        /// The longest typing delay, in milliseconds.
        /// </summary>
        public const int MaxDelay = 2000;

        /// <summary>
        /// The default typing delay, in milliseconds.
        /// </summary>
        public const int DefaultDelay = 300;

        /// <summary>
        /// The fewest search results shown.
        /// </summary>
        public const int MinResults = 3;

        /// <summary>
        /// The most search results shown.
        /// </summary>
        public const int MaxResults = 10;

        private int delay = DefaultDelay;
        private int resultsLimit = MaxResults;
        private SearchMode defaultMode = SearchMode.Title;

        #endregion

        #region Properties

        /// <summary>
        /// Whether a search starts by itself once typing stops, (otherwise Enter starts it).
        /// </summary>
        public bool SearchWhileTyping { get; set; } = true;

        /// <summary>
        /// How long to wait after the last key press before searching, in milliseconds.
        /// </summary>
        public int Delay
        {
            get => this.delay;
            set => this.delay = Math.Clamp(value, MinDelay, MaxDelay);
        }

        /// <summary>
        /// The number of search results shown.
        /// </summary>
        public int ResultsLimit
        {
            get => this.resultsLimit;
            set => this.resultsLimit = Math.Clamp(value, MinResults, MaxResults);
        }

        /// <summary>
        /// Whether the search starts by title or by link.
        /// </summary>
        [JsonConverter(typeof(LenientSearchModeConverter))]
        public SearchMode DefaultMode
        {
            get => this.defaultMode;
            set => this.defaultMode = Enum.IsDefined(value) ? value : SearchMode.Title;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public SearchController() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Resets the search settings.
        /// </summary>
        public void ResetSettings()
        {
            SeedSettings();
        }

        /// <summary>
        /// Generate default values for the search settings.
        /// </summary>
        public void SeedSettings()
        {
            SearchWhileTyping = true;
            Delay = DefaultDelay;
            ResultsLimit = MaxResults;
            DefaultMode = SearchMode.Title;
        }

        #endregion
    }

    /// <summary>
    /// Writes the search mode as its name, and reads names or numbers; an unknown value means
    /// <see cref="SearchMode.Title"/> rather than making the whole settings file unreadable.
    /// </summary>
    internal class LenientSearchModeConverter : JsonConverter<SearchMode>
    {
        public override SearchMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), true, out SearchMode mode) && Enum.IsDefined(mode))
            {
                return mode;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number) && Enum.IsDefined((SearchMode)number))
            {
                return (SearchMode)number;
            }

            reader.Skip();

            return SearchMode.Title;
        }

        public override void Write(Utf8JsonWriter writer, SearchMode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
