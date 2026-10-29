namespace MAL_Reviewer_Core.enumerations
{
    /// <summary>
    /// What a color of a theme is used for.
    /// </summary>
    public enum ThemeRole
    {
        /// <summary>
        /// The background of the windows.
        /// </summary>
        Background,

        /// <summary>
        /// The background of panels, cards and settings pages.
        /// </summary>
        Panel,

        /// <summary>
        /// The background of selected and hovered items, lists and text boxes.
        /// </summary>
        Highlight,

        /// <summary>
        /// The background of headers and of hovered buttons.
        /// </summary>
        Header,

        /// <summary>
        /// Separator lines, shadows and buttons.
        /// </summary>
        Line,

        /// <summary>
        /// The main text.
        /// </summary>
        Text,

        /// <summary>
        /// Labels and secondary text.
        /// </summary>
        SecondaryText,

        /// <summary>
        /// Hints, dates and placeholders.
        /// </summary>
        FaintText,

        /// <summary>
        /// Links.
        /// </summary>
        Link
    }
}
