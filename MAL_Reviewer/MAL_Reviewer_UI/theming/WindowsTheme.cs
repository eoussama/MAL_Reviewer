using System;
using System.Threading;
using Microsoft.Win32;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.enumerations;

namespace MAL_Reviewer_UI.theming
{
    /// <summary>
    /// The light or dark mode chosen for apps in the Windows settings, (Personalization → Colors), which the system theme follows.
    /// </summary>
    public static class WindowsTheme
    {
        #region Fields

        private const string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        /// <summary>
        /// The thread of the windows, where the theme is changed.
        /// </summary>
        private static SynchronizationContext interfaceThread;

        private static bool wasDark;

        #endregion

        #region Properties

        /// <summary>
        /// Whether Windows shows apps in dark mode; light when the setting can't be read.
        /// </summary>
        public static bool IsDark
        {
            get
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(PersonalizeKey))
                    {
                        return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
                    }
                }
                catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
                {
                    return false;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows the windows in the theme of the settings, (see <see cref="Theme.Use"/>).
        /// </summary>
        public static void UseSettingsTheme()
        {
            wasDark = IsDark;
            Theme.Use(Core.Settings.ThemeSettings.PaletteFor(wasDark));
        }

        /// <summary>
        /// Follows the changes of the Windows mode while the system theme is chosen.
        /// To call once, from the thread of the windows.
        /// </summary>
        public static void Follow()
        {
            interfaceThread = SynchronizationContext.Current;
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        /// <summary>
        /// Stops following the changes of the Windows mode.
        /// </summary>
        public static void StopFollowing()
        {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        }

        #endregion

        #region Private methods

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            // Windows tells about many changes; the mode is among the general ones.
            if (e.Category != UserPreferenceCategory.General || interfaceThread == null)
            {
                return;
            }

            interfaceThread.Post(_ =>
            {
                if (Core.Settings?.ThemeSettings.Theme == ThemeKind.System && IsDark != wasDark)
                {
                    UseSettingsTheme();
                }
            }, null);
        }

        #endregion
    }
}
