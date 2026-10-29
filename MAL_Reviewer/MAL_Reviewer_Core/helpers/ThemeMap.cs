using System;
using System.Collections.Generic;
using System.Drawing;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.models.ThemeModels;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Translates the colors the interface was designed with, (the light ones), into the colors of a theme.
    /// Only the named colors the interface uses are translated; any other color is kept.
    /// </summary>
    public static class ThemeMap
    {
        #region Fields

        private static readonly Dictionary<KnownColor, ThemeRole> BackRoles = new Dictionary<KnownColor, ThemeRole>
        {
            [KnownColor.Control] = ThemeRole.Background,
            [KnownColor.ButtonFace] = ThemeRole.Background,
            [KnownColor.ControlLight] = ThemeRole.Panel,
            [KnownColor.ControlLightLight] = ThemeRole.Highlight,
            [KnownColor.Window] = ThemeRole.Highlight,
            [KnownColor.HighlightText] = ThemeRole.Highlight,
            [KnownColor.White] = ThemeRole.Highlight,
            [KnownColor.WhiteSmoke] = ThemeRole.Highlight,
            [KnownColor.Gainsboro] = ThemeRole.Header,
            [KnownColor.LightGray] = ThemeRole.Header,
            [KnownColor.ScrollBar] = ThemeRole.Line,
            [KnownColor.ControlDark] = ThemeRole.Line,
            [KnownColor.ButtonShadow] = ThemeRole.Line,
            [KnownColor.DarkGray] = ThemeRole.Line
        };

        private static readonly Dictionary<KnownColor, ThemeRole> ForeRoles = new Dictionary<KnownColor, ThemeRole>
        {
            [KnownColor.ControlText] = ThemeRole.Text,
            [KnownColor.WindowText] = ThemeRole.Text,
            [KnownColor.ActiveCaptionText] = ThemeRole.Text,
            [KnownColor.Black] = ThemeRole.Text,
            [KnownColor.ControlDarkDark] = ThemeRole.SecondaryText,
            [KnownColor.ControlDark] = ThemeRole.FaintText,
            [KnownColor.ButtonShadow] = ThemeRole.FaintText,
            [KnownColor.AppWorkspace] = ThemeRole.FaintText,
            [KnownColor.DarkGray] = ThemeRole.FaintText,
            [KnownColor.GrayText] = ThemeRole.FaintText,
            [KnownColor.HotTrack] = ThemeRole.Link,

            // The windows' own text color, which every label replaces.
            [KnownColor.ControlLight] = ThemeRole.Panel
        };

        #endregion

        #region Public methods

        /// <summary>
        /// The role of a background color, if it is one the interface uses.
        /// </summary>
        /// <param name="color"> The designed color. </param>
        public static ThemeRole? BackRole(Color color) => RoleOf(BackRoles, color);

        /// <summary>
        /// The role of a text color, if it is one the interface uses.
        /// </summary>
        /// <param name="color"> The designed color. </param>
        public static ThemeRole? ForeRole(Color color) => RoleOf(ForeRoles, color);

        /// <summary>
        /// The theme's version of a designed background color.
        /// </summary>
        /// <param name="color"> The designed color. </param>
        /// <param name="palette"> The theme's colors; null keeps the designed colors. </param>
        public static Color Back(Color color, ThemePalette palette) => Translate(BackRole(color), color, palette);

        /// <summary>
        /// The theme's version of a designed text color.
        /// </summary>
        /// <param name="color"> The designed color. </param>
        /// <param name="palette"> The theme's colors; null keeps the designed colors. </param>
        public static Color Fore(Color color, ThemePalette palette) => Translate(ForeRole(color), color, palette);

        /// <summary>
        /// Moves a pixel drawn on one background onto another, keeping its difference from the background.
        /// Between light and dark backgrounds the difference in brightness is reversed and the tint kept,
        /// so dark blue marks on light become light blue marks on dark.
        /// </summary>
        /// <param name="pixel"> The pixel. </param>
        /// <param name="oldBackground"> The background it was drawn on. </param>
        /// <param name="newBackground"> The background to move it to. </param>
        public static Color MoveToBackground(Color pixel, Color oldBackground, Color newBackground)
        {
            int red = pixel.R - oldBackground.R, green = pixel.G - oldBackground.G, blue = pixel.B - oldBackground.B;

            // Removing the difference in brightness twice reverses it.
            int reverse = ThemePalette.IsDarkColor(oldBackground) == ThemePalette.IsDarkColor(newBackground) ? 0 : 2 * (red + green + blue) / 3;

            return Color.FromArgb(pixel.A,
                Math.Clamp(newBackground.R + red - reverse, 0, 255),
                Math.Clamp(newBackground.G + green - reverse, 0, 255),
                Math.Clamp(newBackground.B + blue - reverse, 0, 255));
        }

        #endregion

        #region Private methods

        private static ThemeRole? RoleOf(Dictionary<KnownColor, ThemeRole> roles, Color color)
        {
            return color.IsKnownColor && roles.TryGetValue(color.ToKnownColor(), out ThemeRole role) ? role : null;
        }

        private static Color Translate(ThemeRole? role, Color color, ThemePalette palette)
        {
            return palette != null && role.HasValue ? palette[role.Value] : color;
        }

        #endregion
    }
}
