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

        private ThemeKind theme = ThemeKind.Light;
        private ThemePalette customColors = ThemePalette.Light;

        #endregion

        #region Properties

        /// <summary>
        /// The chosen theme.
        /// </summary>
        [JsonConverter(typeof(LenientEnumConverter<ThemeKind>))]
        public ThemeKind Theme
        {
            get => this.theme;
            set => this.theme = Enum.IsDefined(value) ? value : ThemeKind.Light;
        }

        /// <summary>
        /// The colors of the custom theme.
        /// </summary>
        public ThemePalette CustomColors
        {
            get => this.customColors;
            set => this.customColors = value ?? ThemePalette.Light;
        }

        /// <summary>
        /// The colors of the chosen theme, or null for the light theme, which keeps the colors the interface was designed with.
        /// </summary>
        [JsonIgnore]
        public ThemePalette ActivePalette => this.Theme switch
        {
            ThemeKind.Dark => ThemePalette.Dark,
            ThemeKind.Custom => new ThemePalette(this.CustomColors),
            _ => null
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ThemeController() { }

        #endregion

        #region Public methods

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
            Theme = ThemeKind.Light;
            CustomColors = ThemePalette.Light;
        }

        #endregion
    }
}
