namespace MAL_Reviewer_Core.enumerations
{
    /// <summary>
    /// The application's color themes.
    /// </summary>
    public enum ThemeKind
    {
        /// <summary>
        /// The original light colors.
        /// </summary>
        Light,

        /// <summary>
        /// Dark colors.
        /// </summary>
        Dark,

        /// <summary>
        /// Colors picked by the user.
        /// </summary>
        Custom,

        /// <summary>
        /// The light or dark colors, following the apps mode chosen in the Windows settings.
        /// </summary>
        System
    }
}
