using System;
using System.Text.Json.Serialization;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.interfaces;
using MAL_Reviewer_Core.models.ThemeModels;

namespace MAL_Reviewer_Core.controllers
{
    /// <summary>
    /// The theme settings.
    /// </summary>
    public class ThemeController : ISettings
    {
        #region Fields

        /// <summary>
        /// The theme of new installations and of a reset.
        /// </summary>
        public const ThemeKind DefaultTheme = ThemeKind.System;

        private ThemeKind theme = DefaultTheme;
        private ThemePalette customColors = ThemePalette.Light;

        #endregion

        #region Properties

        /// <summary>
        /// The chosen theme.
        /// </summary>
        [JsonConverter(typeof(LenientThemeKindConverter))]
        public ThemeKind Theme
        {
            get => this.theme;
            set => this.theme = Enum.IsDefined(value) ? value : DefaultTheme;
        }

        /// <summary>
        /// The colors of the custom theme.
        /// </summary>
        public ThemePalette CustomColors
        {
            get => this.customColors;
            set => this.customColors = value ?? ThemePalette.Light;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ThemeController() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The colors of the chosen theme, or null for the light theme, which keeps the colors the interface was designed with.
        /// </summary>
        /// <param name="windowsIsDark"> Whether Windows is set to show apps in dark mode, which the system theme follows. </param>
        public ThemePalette PaletteFor(bool windowsIsDark) => this.Theme switch
        {
            ThemeKind.Dark => ThemePalette.Dark,
            ThemeKind.Custom => new ThemePalette(this.CustomColors),
            ThemeKind.System when windowsIsDark => ThemePalette.Dark,
            _ => null
        };

        /// <summary>
        /// Resets the theme settings.
        /// </summary>
        public void ResetSettings()
        {
            SeedSettings();
        }

        /// <summary>
        /// Generate default values for the theme settings.
        /// </summary>
        public void SeedSettings()
        {
            Theme = DefaultTheme;
            CustomColors = ThemePalette.Light;
        }

        #endregion
    }

    /// <summary>
    /// Reads the theme, unknown values meaning the default theme.
    /// </summary>
    internal class LenientThemeKindConverter : LenientEnumConverter<ThemeKind>
    {
        protected override ThemeKind Fallback => ThemeController.DefaultTheme;
    }
}
