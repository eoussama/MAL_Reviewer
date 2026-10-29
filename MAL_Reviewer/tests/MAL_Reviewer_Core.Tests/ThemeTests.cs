using System;
using System.Drawing;
using System.IO;
using System.Linq;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ThemeModels;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class ThemePaletteTests
    {
        [Fact]
        public void Light_HoldsTheDesignedColors()
        {
            ThemePalette light = ThemePalette.Light;

            Assert.Equal(0xF0F0F0, light[ThemeRole.Background].ToArgb() & 0xFFFFFF);
            Assert.Equal(0xE3E3E3, light[ThemeRole.Panel].ToArgb() & 0xFFFFFF);
            Assert.Equal(0x000000, light[ThemeRole.Text].ToArgb() & 0xFFFFFF);
            Assert.Equal(0x0066CC, light[ThemeRole.Link].ToArgb() & 0xFFFFFF);
            Assert.False(light.IsDark);
            Assert.True(ThemePalette.Dark.IsDark);
        }

        [Fact]
        public void EveryRoleHasAColor()
        {
            Assert.Equal(Enum.GetValues<ThemeRole>(), ThemePalette.Roles);
            Assert.All(ThemePalette.Roles, role => Assert.Equal(255, ThemePalette.Dark[role].A));
            Assert.True(new ThemePalette().SameColors(ThemePalette.Light));
        }

        [Fact]
        public void Colors_AreStoredOpaqueAndUnnamed()
        {
            ThemePalette palette = new ThemePalette();

            palette[ThemeRole.Text] = Color.FromArgb(10, 1, 2, 3);
            palette[ThemeRole.Link] = Color.Red;

            Assert.Equal(Color.FromArgb(255, 1, 2, 3), palette[ThemeRole.Text]);
            Assert.False(palette[ThemeRole.Link].IsKnownColor);
            Assert.Equal(Color.Red.ToArgb(), palette[ThemeRole.Link].ToArgb());
        }

        [Fact]
        public void Copies_AndPresets_AreIndependent()
        {
            ThemePalette light = ThemePalette.Light;
            light[ThemeRole.Background] = Color.Black;

            ThemePalette copy = new ThemePalette(light);
            copy[ThemeRole.Background] = Color.White;

            Assert.Equal(Color.Black.ToArgb(), light[ThemeRole.Background].ToArgb());
            Assert.Equal(0xF0F0F0, ThemePalette.Light[ThemeRole.Background].ToArgb() & 0xFFFFFF);
            Assert.False(copy.SameColors(light));
            Assert.False(copy.SameColors(null));
            Assert.Throws<ArgumentNullException>(() => new ThemePalette(null));
        }

        [Theory]
        [InlineData("#1F2023", 0x1F2023)]
        [InlineData("1f2023", 0x1F2023)]
        [InlineData("  #FFFFFF ", 0xFFFFFF)]
        public void TryParseHex_ReadsColors(string text, int rgb)
        {
            Assert.True(ThemePalette.TryParseHex(text, out Color color));
            Assert.Equal(rgb, color.ToArgb() & 0xFFFFFF);
            Assert.Equal(255, color.A);
            Assert.Equal(text.Trim().TrimStart('#').ToUpperInvariant(), ThemePalette.ToHex(color).TrimStart('#'));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("#FFF")]
        [InlineData("#GGGGGG")]
        [InlineData("#1234567")]
        public void TryParseHex_RejectsAnythingElse(string text)
        {
            Assert.False(ThemePalette.TryParseHex(text, out Color color));
            Assert.Equal(Color.Empty, color);
        }

        [Fact]
        public void IsDarkColor_UsesPerceivedBrightness()
        {
            Assert.True(ThemePalette.IsDarkColor(Color.Black));
            Assert.True(ThemePalette.IsDarkColor(Color.Blue));
            Assert.False(ThemePalette.IsDarkColor(Color.Yellow));
            Assert.False(ThemePalette.IsDarkColor(Color.White));
        }
    }

    public class ThemeMapTests
    {
        [Theory]
        [InlineData(KnownColor.Control, ThemeRole.Background)]
        [InlineData(KnownColor.ButtonFace, ThemeRole.Background)]
        [InlineData(KnownColor.ControlLight, ThemeRole.Panel)]
        [InlineData(KnownColor.ControlLightLight, ThemeRole.Highlight)]
        [InlineData(KnownColor.Window, ThemeRole.Highlight)]
        [InlineData(KnownColor.White, ThemeRole.Highlight)]
        [InlineData(KnownColor.WhiteSmoke, ThemeRole.Highlight)]
        [InlineData(KnownColor.Gainsboro, ThemeRole.Header)]
        [InlineData(KnownColor.LightGray, ThemeRole.Header)]
        [InlineData(KnownColor.ScrollBar, ThemeRole.Line)]
        [InlineData(KnownColor.ControlDark, ThemeRole.Line)]
        public void Backgrounds_GetTheirRole(KnownColor designed, ThemeRole role)
        {
            Assert.Equal(role, ThemeMap.BackRole(Color.FromKnownColor(designed)));
            Assert.Equal(ThemePalette.Dark[role], ThemeMap.Back(Color.FromKnownColor(designed), ThemePalette.Dark));
        }

        [Theory]
        [InlineData(KnownColor.ControlText, ThemeRole.Text)]
        [InlineData(KnownColor.WindowText, ThemeRole.Text)]
        [InlineData(KnownColor.ActiveCaptionText, ThemeRole.Text)]
        [InlineData(KnownColor.Black, ThemeRole.Text)]
        [InlineData(KnownColor.ControlDarkDark, ThemeRole.SecondaryText)]
        [InlineData(KnownColor.ControlDark, ThemeRole.FaintText)]
        [InlineData(KnownColor.AppWorkspace, ThemeRole.FaintText)]
        [InlineData(KnownColor.DarkGray, ThemeRole.FaintText)]
        [InlineData(KnownColor.HotTrack, ThemeRole.Link)]
        [InlineData(KnownColor.ControlLight, ThemeRole.Panel)]
        public void Texts_GetTheirRole(KnownColor designed, ThemeRole role)
        {
            Assert.Equal(role, ThemeMap.ForeRole(Color.FromKnownColor(designed)));
            Assert.Equal(ThemePalette.Dark[role], ThemeMap.Fore(Color.FromKnownColor(designed), ThemePalette.Dark));
        }

        [Fact]
        public void OtherColors_AreKept()
        {
            Color[] others = { Color.Transparent, Color.Red, Color.FromArgb(1, 2, 3), Color.Empty, SystemColors.Highlight };

            Assert.All(others, color =>
            {
                Assert.Null(ThemeMap.BackRole(color));
                Assert.Equal(color, ThemeMap.Back(color, ThemePalette.Dark));
                Assert.Equal(color, ThemeMap.Fore(color, ThemePalette.Dark));
            });
        }

        [Fact]
        public void WithoutAPalette_TheDesignedColorsAreKept()
        {
            Assert.Equal(SystemColors.Control, ThemeMap.Back(SystemColors.Control, null));
            Assert.Equal(Color.Black, ThemeMap.Fore(Color.Black, null));
        }

        [Fact]
        public void MoveToBackground_KeepsColorsOnTheSameBackground()
        {
            Color background = Color.FromArgb(0xF0, 0xF0, 0xF0);
            Color mark = Color.FromArgb(0x4C, 0x66, 0xAC);

            Assert.Equal(mark.ToArgb(), ThemeMap.MoveToBackground(mark, background, background).ToArgb());
            Assert.Equal(background.ToArgb(), ThemeMap.MoveToBackground(background, background, background).ToArgb());
        }

        [Fact]
        public void MoveToBackground_KeepsTheDifference_BetweenLightBackgrounds()
        {
            Color moved = ThemeMap.MoveToBackground(Color.FromArgb(0xC0, 0xC0, 0xC0), Color.FromArgb(0xF0, 0xF0, 0xF0), Color.FromArgb(0xE0, 0xE0, 0xE0));

            Assert.Equal(Color.FromArgb(0xB0, 0xB0, 0xB0).ToArgb(), moved.ToArgb());
        }

        [Fact]
        public void MoveToBackground_TurnsDarkMarksLight_KeepingTheirTint()
        {
            Color light = Color.FromArgb(0xF0, 0xF0, 0xF0), dark = Color.FromArgb(0x1F, 0x20, 0x23);
            Color blueMark = Color.FromArgb(128, 0x4C, 0x66, 0xAC);

            Color moved = ThemeMap.MoveToBackground(blueMark, light, dark);

            Assert.Equal(dark.ToArgb(), ThemeMap.MoveToBackground(light, light, dark).ToArgb());
            Assert.Equal(128, moved.A);
            Assert.True(moved.GetBrightness() > dark.GetBrightness());
            Assert.True(moved.B > moved.G && moved.G > moved.R);
        }

        [Fact]
        public void MoveToBackground_StaysWithinRange()
        {
            Color moved = ThemeMap.MoveToBackground(Color.Black, Color.White, Color.FromArgb(0x30, 0x30, 0x30));

            Assert.Equal(Color.FromArgb(255, 255, 255).ToArgb(), moved.ToArgb());
        }
    }

    public class GifPaletteTests
    {
        // A 1x1 GIF with a global color table of 2 colors, a graphic control extension,
        // and an image with a local color table of 2 colors.
        private static readonly byte[] Gif =
        {
            (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a',
            1, 0, 1, 0, 0x80, 0, 0,
            10, 20, 30, 40, 50, 60,
            0x21, 0xF9, 4, 0, 10, 0, 0, 0,
            0x2C, 0, 0, 0, 0, 1, 0, 1, 0, 0x80,
            70, 80, 90, 100, 110, 120,
            2, 2, 0x4C, 0x01, 0,
            0x3B
        };

        [Fact]
        public void Recolor_ChangesEveryColorTable_AndNothingElse()
        {
            byte[] original = (byte[])Gif.Clone();

            byte[] recolored = GifPalette.Recolor(Gif, color => Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B));

            Assert.Equal(original, Gif);
            Assert.Equal(new byte[] { 245, 235, 225, 215, 205, 195 }, recolored.Skip(13).Take(6));
            Assert.Equal(new byte[] { 185, 175, 165, 155, 145, 135 }, recolored.Skip(37).Take(6));

            int[] tables = Enumerable.Range(13, 6).Concat(Enumerable.Range(37, 6)).ToArray();
            Assert.All(Enumerable.Range(0, Gif.Length).Except(tables), i => Assert.Equal(Gif[i], recolored[i]));
        }

        [Fact]
        public void Recolor_HandlesImagesWithoutColorTables()
        {
            byte[] gif = (byte[])Gif.Clone();
            gif[10] = 0;

            byte[] withoutGlobalTable = gif.Take(13).Concat(gif.Skip(19)).ToArray();
            int calls = 0;

            GifPalette.Recolor(withoutGlobalTable, color => { calls++; return color; });

            Assert.Equal(2, calls);
        }

        [Fact]
        public void Recolor_RejectsOtherFiles()
        {
            Assert.Throws<FormatException>(() => GifPalette.Recolor(new byte[] { 1, 2, 3 }, color => color));
            Assert.Throws<FormatException>(() => GifPalette.Recolor(new byte[20], color => color));
            Assert.Throws<FormatException>(() => GifPalette.Recolor(Gif.Take(Gif.Length - 1).ToArray(), color => color));
            Assert.Throws<FormatException>(() => GifPalette.Recolor(Gif.Take(16).ToArray(), color => color));

            byte[] unknownBlock = (byte[])Gif.Clone();
            unknownBlock[19] = 0x99;
            Assert.Throws<FormatException>(() => GifPalette.Recolor(unknownBlock, color => color));

            Assert.Throws<ArgumentNullException>(() => GifPalette.Recolor(null, color => color));
            Assert.Throws<ArgumentNullException>(() => GifPalette.Recolor(Gif, null));
        }
    }

    public class ThemeControllerTests
    {
        [Fact]
        public void Defaults_FollowWindows()
        {
            ThemeController settings = new ThemeController();

            Assert.Equal(ThemeKind.System, settings.Theme);
            Assert.Null(settings.PaletteFor(windowsIsDark: false));
            Assert.True(settings.PaletteFor(windowsIsDark: true).SameColors(ThemePalette.Dark));
            Assert.True(settings.CustomColors.SameColors(ThemePalette.Light));
        }

        [Theory]
        [InlineData(ThemeKind.Light, false)]
        [InlineData(ThemeKind.Light, true)]
        public void TheLightTheme_KeepsTheDesignedColors_WhateverWindowsUses(ThemeKind theme, bool windowsIsDark)
        {
            Assert.Null(new ThemeController { Theme = theme }.PaletteFor(windowsIsDark));
        }

        [Fact]
        public void PaletteFor_FollowsTheTheme()
        {
            ThemeController settings = new ThemeController { Theme = ThemeKind.Dark };
            Assert.True(settings.PaletteFor(windowsIsDark: false).SameColors(ThemePalette.Dark));

            settings.Theme = ThemeKind.Custom;
            settings.CustomColors[ThemeRole.Text] = Color.Red;

            ThemePalette active = settings.PaletteFor(windowsIsDark: true);
            Assert.Equal(Color.Red.ToArgb(), active[ThemeRole.Text].ToArgb());

            // The palette in use is a copy.
            active[ThemeRole.Text] = Color.Blue;
            Assert.Equal(Color.Red.ToArgb(), settings.CustomColors[ThemeRole.Text].ToArgb());
        }

        [Fact]
        public void UnknownValues_FallBack()
        {
            ThemeController settings = new ThemeController { Theme = (ThemeKind)42, CustomColors = null };

            Assert.Equal(ThemeKind.System, settings.Theme);
            Assert.True(settings.CustomColors.SameColors(ThemePalette.Light));
        }

        [Fact]
        public void SeedAndReset_RestoreTheDefaults()
        {
            ThemeController settings = new ThemeController { Theme = ThemeKind.Custom, CustomColors = ThemePalette.Dark };
            settings.SeedSettings();
            Assert.Equal(ThemeKind.System, settings.Theme);

            settings.CustomColors = ThemePalette.Dark;
            settings.ResetSettings();
            Assert.True(settings.CustomColors.SameColors(ThemePalette.Light));
        }
    }

    public class ThemeSettingsFileTests
    {
        [Fact]
        public void ThemeSettings_AreSavedAndLoaded()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings saved = new Settings(storage.Config);
                saved.ThemeSettings.Theme = ThemeKind.Custom;
                saved.ThemeSettings.CustomColors = ThemePalette.Dark;
                saved.ThemeSettings.CustomColors[ThemeRole.Link] = Color.FromArgb(0x12, 0x34, 0x56);
                saved.SaveSettings();

                string json = File.ReadAllText(storage.File(Settings.StorageFile));
                Assert.Contains("\"theme\": \"Custom\"", json);
                Assert.Contains("\"link\": \"#123456\"", json);
                Assert.Contains("\"secondaryText\": \"#B4B8BE\"", json);

                Settings loaded = new Settings(storage.Config);
                loaded.LoadSettings();

                Assert.Equal(ThemeKind.Custom, loaded.ThemeSettings.Theme);
                Assert.True(loaded.ThemeSettings.CustomColors.SameColors(saved.ThemeSettings.CustomColors));
            }
        }

        [Fact]
        public void FilesWithoutThemeSettings_FollowWindows()
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), "{\"formatVersion\":1,\"reviewTemplates\":[]}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(ThemeKind.System, settings.ThemeSettings.Theme);
            }
        }

        [Theory]
        [InlineData("\"dark\"", ThemeKind.Dark)]
        [InlineData("2", ThemeKind.Custom)]
        [InlineData("\"light\"", ThemeKind.Light)]
        [InlineData("\"system\"", ThemeKind.System)]
        [InlineData("3", ThemeKind.System)]
        [InlineData("\"sideways\"", ThemeKind.System)]
        [InlineData("7", ThemeKind.System)]
        [InlineData("[1]", ThemeKind.System)]
        public void UnknownThemes_DontMakeTheFileUnreadable(string theme, ThemeKind expected)
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), $"{{\"theme\":{{\"theme\":{ theme },\"customColors\":{{\"text\":\"#FF0000\"}}}}}}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());
                Assert.Equal(expected, settings.ThemeSettings.Theme);
                Assert.Equal(Color.Red.ToArgb(), settings.ThemeSettings.CustomColors[ThemeRole.Text].ToArgb());
            }
        }

        [Theory]
        [InlineData("{\"text\":\"red\",\"link\":12,\"nothing\":\"#000000\",\"panel\":{\"x\":[1]},\"background\":\"#101010\"}")]
        [InlineData("\"#101010\"")]
        [InlineData("[\"#101010\"]")]
        [InlineData("null")]
        public void UnreadableColors_AreTakenFromTheLightTheme(string colors)
        {
            using (TempStorage storage = new TempStorage())
            {
                storage.Config.PrepareStorageFolder();
                File.WriteAllText(storage.File(Settings.StorageFile), $"{{\"theme\":{{\"theme\":\"Custom\",\"customColors\":{ colors }}}}}");

                Settings settings = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Loaded, settings.LoadSettings());

                ThemePalette custom = settings.ThemeSettings.CustomColors;
                Assert.Equal(ThemePalette.Light[ThemeRole.Text], custom[ThemeRole.Text]);
                Assert.Equal(ThemePalette.Light[ThemeRole.Link], custom[ThemeRole.Link]);
                Assert.Equal(ThemePalette.Light[ThemeRole.Panel], custom[ThemeRole.Panel]);

                int background = colors.StartsWith("{") ? 0x101010 : 0xF0F0F0;
                Assert.Equal(background, custom[ThemeRole.Background].ToArgb() & 0xFFFFFF);
            }
        }

        [Fact]
        public void ResetAndMigration_IncludeTheThemeSettings()
        {
            using (TempStorage storage = new TempStorage())
            {
                Settings settings = new Settings(storage.Config);
                settings.ThemeSettings.Theme = ThemeKind.Dark;

                settings.ResetSettings();

                Assert.Equal(ThemeKind.System, settings.ThemeSettings.Theme);

                storage.Config.PrepareStorageFolder();
                File.WriteAllBytes(storage.File(Settings.LegacyStorageFile), LegacyPayload.Write("someone"));

                Settings migrated = new Settings(storage.Config);

                Assert.Equal(SettingsLoadResult.Migrated, migrated.LoadSettings());
                Assert.Equal(ThemeKind.System, migrated.ThemeSettings.Theme);
            }
        }
    }
}
