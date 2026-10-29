using System;

namespace MAL_Reviewer_API.sources
{
    /// <summary>
    /// The kinds of entries that can be searched and reviewed.
    /// </summary>
    public enum MediaKind
    {
        Anime,
        Manga
    }

    /// <summary>
    /// Conversions of <see cref="MediaKind"/> values.
    /// </summary>
    public static class MediaKindExtensions
    {
        /// <summary>
        /// Parses "anime" or "manga", in any letter case.
        /// </summary>
        /// <param name="value"> The text to parse. </param>
        public static MediaKind Parse(string value)
        {
            if (Enum.TryParse(value?.Trim(), true, out MediaKind kind) && Enum.IsDefined(kind))
            {
                return kind;
            }

            throw new ArgumentException($"Unknown media kind “{ value }”, expected anime or manga.", nameof(value));
        }

        /// <summary>
        /// Returns the lowercase path segment used by the APIs, (ex: anime, manga).
        /// </summary>
        /// <param name="kind"> The media kind. </param>
        public static string ToPathSegment(this MediaKind kind)
        {
            return kind == MediaKind.Anime ? "anime" : "manga";
        }
    }
}
