using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MAL_Reviewer_Core.enumerations;

namespace MAL_Reviewer_Core.models.ThemeModels
{
    /// <summary>
    /// The colors of a theme, one per <see cref="ThemeRole"/>.
    /// </summary>
    [JsonConverter(typeof(ThemePaletteConverter))]
    public class ThemePalette
    {
        #region Fields

        private readonly Dictionary<ThemeRole, Color> colors = new Dictionary<ThemeRole, Color>();

        #endregion

        #region Properties

        /// <summary>
        /// Every role, in the order they are shown.
        /// </summary>
        public static IReadOnlyList<ThemeRole> Roles { get; } = Enum.GetValues<ThemeRole>();

        /// <summary>
        /// The original colors of the application.
        /// </summary>
        public static ThemePalette Light => new ThemePalette(0xF0F0F0, 0xE3E3E3, 0xFFFFFF, 0xDCDCDC, 0xC8C8C8, 0x000000, 0x696969, 0xA0A0A0, 0x0066CC);

        /// <summary>
        /// The dark colors.
        /// </summary>
        public static ThemePalette Dark => new ThemePalette(0x1F2023, 0x2A2C30, 0x3A3D42, 0x33363B, 0x4A4D53, 0xEDEEF0, 0xB4B8BE, 0x878B92, 0x6CB4FF);

        /// <summary>
        /// Gets and sets the color of a role. Colors are stored opaque.
        /// </summary>
        /// <param name="role"> The role of the color. </param>
        public Color this[ThemeRole role]
        {
            get => this.colors[role];
            set => this.colors[role] = Color.FromArgb(value.R, value.G, value.B);
        }

        /// <summary>
        /// Whether the background is dark, which calls for dark title bars and scroll bars.
        /// </summary>
        public bool IsDark => IsDarkColor(this[ThemeRole.Background]);

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor, with the light colors.
        /// </summary>
        public ThemePalette() : this(Light) { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"> The palette to copy. </param>
        public ThemePalette(ThemePalette source)
        {
            ArgumentNullException.ThrowIfNull(source);

            foreach (ThemeRole role in Roles)
            {
                this[role] = source[role];
            }
        }

        private ThemePalette(params int[] rgb)
        {
            for (int i = 0; i < Roles.Count; i++)
            {
                this[Roles[i]] = Color.FromArgb(rgb[i]);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Whether two palettes have the same colors.
        /// </summary>
        /// <param name="other"> The other palette. </param>
        public bool SameColors(ThemePalette other)
        {
            return other != null && Roles.All(role => this[role].ToArgb() == other[role].ToArgb());
        }

        /// <summary>
        /// Whether a color is dark, (its perceived brightness is under half).
        /// </summary>
        /// <param name="color"> The color. </param>
        public static bool IsDarkColor(Color color)
        {
            return (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255 < 0.5;
        }

        /// <summary>
        /// Writes a color as "#RRGGBB".
        /// </summary>
        /// <param name="color"> The color. </param>
        public static string ToHex(Color color)
        {
            return $"#{ color.R:X2}{ color.G:X2}{ color.B:X2}";
        }

        /// <summary>
        /// Reads a "#RRGGBB" color, (the "#" is optional).
        /// </summary>
        /// <param name="text"> The text to read. </param>
        /// <param name="color"> The color read, or <see cref="Color.Empty"/>. </param>
        public static bool TryParseHex(string text, out Color color)
        {
            color = Color.Empty;
            string hex = text?.Trim().TrimStart('#');

            if (hex == null || hex.Length != 6 || !int.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int rgb))
            {
                return false;
            }

            color = Color.FromArgb(255, Color.FromArgb(rgb));

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Writes a palette as { "background": "#RRGGBB", ... }. Missing or unreadable colors are taken from the light palette,
    /// rather than making the whole settings file unreadable.
    /// </summary>
    internal class ThemePaletteConverter : JsonConverter<ThemePalette>
    {
        public override ThemePalette Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ThemePalette palette = ThemePalette.Light;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                reader.Skip();
                return palette;
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                string name = reader.GetString();
                reader.Read();

                if (reader.TokenType == JsonTokenType.String
                    && Enum.TryParse(name, true, out ThemeRole role) && Enum.IsDefined(role)
                    && ThemePalette.TryParseHex(reader.GetString(), out Color color))
                {
                    palette[role] = color;
                }
                else
                {
                    reader.Skip();
                }
            }

            return palette;
        }

        public override void Write(Utf8JsonWriter writer, ThemePalette value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (ThemeRole role in ThemePalette.Roles)
            {
                writer.WriteString(JsonNamingPolicy.CamelCase.ConvertName(role.ToString()), ThemePalette.ToHex(value[role]));
            }

            writer.WriteEndObject();
        }
    }
}
