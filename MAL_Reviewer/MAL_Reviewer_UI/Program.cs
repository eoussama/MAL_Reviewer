using System;
using System.Threading;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Applies the visual styles, default font and DPI mode set in the project file.
            ApplicationConfiguration.Initialize();

            // Routing every unexpected error to one place instead of letting it close the application.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Initializing the application's core.
            Core.Init();

            // Showing the windows in the chosen theme.
            Theme.Use(Core.Settings.ThemeSettings.ActivePalette);

            // Application shutdown event
            Application.ApplicationExit += (s, e) =>
            {
                // Saving data.
                TrySave();
            };

            Application.Run(new forms.WelcomeForm());
        }

        /// <summary>
        /// Handles the errors raised on the interface's thread; the application keeps running.
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string saved = TrySave() ? "Your settings were saved." : "Your settings could not be saved.";

            MessageBox.Show($"Something went wrong. { saved }\n\n{ e.Exception.Message }", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Handles the errors raised on other threads, after which the process ends.
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string saved = TrySave() ? "Your settings were saved." : "Your settings could not be saved.";

            MessageBox.Show($"A fatal error occurred and the application has to close. { saved }\n\n{ (e.ExceptionObject as Exception)?.Message }", "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Saves the settings, without letting a failing save hide the original problem.
        /// Returns whether the save succeeded.
        /// </summary>
        private static bool TrySave()
        {
            try
            {
                Core.Save();
                return true;
            }
            catch (Exception)
            {
                // Nothing more can be done here; the settings file is never left half-written.
                return false;
            }
        }
    }
}
