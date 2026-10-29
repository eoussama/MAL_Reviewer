using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace MAL_Reviewer_UI.extensions
{
    /// <summary>
    /// Opens links, folders and email addresses with the user's default programs.
    /// </summary>
    public static class Shell
    {
        /// <summary>
        /// Opens the passed target, (ex: a URL, a folder or a mailto link).
        /// Nothing happens for empty targets; failures are reported to the user.
        /// </summary>
        /// <param name="target"> The target to open. </param>
        public static void Open(object target)
        {
            string path = target?.ToString();

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                // .NET no longer goes through the shell by default, which is needed for anything but executables.
                using (Process.Start(new ProcessStartInfo(path) { UseShellExecute = true })) { }
            }
            catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException)
            {
                MessageBox.Show($"“{ path }” could not be opened.\n\n{ ex.Message }", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
