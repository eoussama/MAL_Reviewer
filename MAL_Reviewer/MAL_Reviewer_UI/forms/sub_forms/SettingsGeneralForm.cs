using System;
using System.IO;
using System.Windows.Forms;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.services;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsGeneralForm : Form, ISubForm
    {
        #region Fields

        private const string SettingsFilter = "MAL Reviewer settings (*.json)|*.json";

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public SettingsGeneralForm()
        {
            InitializeComponent();

            this.VisibleChanged += SettingsGeneralForm_VisibleChanged;
            this.StoragePathBrowserControl.BrowserOpenSubmitEventHander += StoragePathBrowserControl_BrowserOpenSubmitEventHander;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the sub form's visibility change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsGeneralForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitDisplay();
            }
        }

        /// <summary>
        /// Handles the click button of the browser on open mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoragePathBrowserControl_BrowserOpenSubmitEventHander(object sender, EventArgs e) => Shell.Open(StoragePathBrowserControl.BrowserPath);

        /// <summary>
        /// Handles the reset all button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneralSettingsResetButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show($"Are you sure you want to reset all the application's settings?\nAll current configurations will be permanently lost.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                // Resetting the application's settings.
                Core.Settings.ResetSettings();
                Core.Save();

                // Showing the reset theme.
                WindowsTheme.UseSettingsTheme();

                // Update the displayed information.
                InitDisplay();

                MessageBox.Show("Application's settings have been successfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportSettingsButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog { Filter = SettingsFilter, FileName = "MAL_Reviewer_settings.json", Title = "Export settings" })
            {
                if (dialog.ShowDialog(this.TopLevelControl) == DialogResult.OK)
                {
                    WriteFile(() => Core.Settings.ExportSettings(dialog.FileName), "The settings were exported.");
                }
            }
        }

        private void ImportSettingsButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog { Filter = SettingsFilter, Title = "Import settings" })
            {
                if (dialog.ShowDialog(this.TopLevelControl) != DialogResult.OK
                    || DialogResult.Yes != MessageBox.Show("Replace all the current settings with those of this file?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    return;
                }

                try
                {
                    Core.Settings.ImportSettings(dialog.FileName);
                    Core.Save();
                }
                catch (Exception ex) when (ex is InvalidDataException || ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"The settings could not be imported, the current ones are kept.\n\n{ ex.Message }", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Showing the imported theme.
                WindowsTheme.UseSettingsTheme();
                InitDisplay();

                MessageBox.Show("The settings were imported.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportAnimeListButton_Click(object sender, EventArgs e)
        {
            ExportList(TargetType.Anime, format => ListExport.AnimeList(Core.UserData.Username, Core.UserData.AnimeList, format));
        }

        private void ExportMangaListButton_Click(object sender, EventArgs e)
        {
            ExportList(TargetType.Manga, format => ListExport.MangaList(Core.UserData.Username, Core.UserData.MangaList, format));
        }

        /// <summary>
        /// Asks where to save a list of the loaded user, and in which format.
        /// </summary>
        /// <param name="type"> Whether it is the Anime or the Manga list. </param>
        /// <param name="write"> Writes the list in a format. </param>
        private void ExportList(TargetType type, Func<ListFileFormat, byte[]> write)
        {
            string username = Core.UserData?.Username;

            if (username == null)
            {
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "MAL XML, which MyAnimeList and AniList can import (*.xml)|*.xml|CSV, for spreadsheets (*.csv)|*.csv",
                FileName = Path.GetFileNameWithoutExtension(ListExport.FileName(username, type, ListFileFormat.MalXml)),
                AddExtension = true,
                Title = $"Export the { type } list"
            })
            {
                if (dialog.ShowDialog(this.TopLevelControl) == DialogResult.OK)
                {
                    ListFileFormat format = ListExport.FormatOf(dialog.FileName, dialog.FilterIndex == 2 ? ListFileFormat.Csv : ListFileFormat.MalXml);

                    WriteFile(() => File.WriteAllBytes(dialog.FileName, write(format)), $"The { type } list was exported.");
                }
            }
        }

        /// <summary>
        /// Writes an exported file, telling whether it worked.
        /// </summary>
        private static void WriteFile(Action write, string done)
        {
            try
            {
                write();
                MessageBox.Show(done, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show($"The file could not be written.\n\n{ ex.Message }", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Public methods

        // Displays initial information on the sub form.
        public void InitDisplay()
        {
            // Resetting the storage folder path control.
            StoragePathBrowserControl.BrowserPath = Core.Configurations.StorageDirectory;

            // The lists can be exported once a MAL account is loaded, unless they are private.
            MALUserModel user = Core.UserData;

            ExportAnimeListButton.Enabled = user?.AnimeList != null;
            ExportMangaListButton.Enabled = user?.MangaList != null;

            if (user == null)
            {
                ListsHintLabel.Text = "Load a MAL account to export its lists.";
            }
            else
            {
                string privateLists = user.AnimeList == null && user.MangaList == null ? "Both lists are private."
                    : user.AnimeList == null ? "The Anime list is private."
                    : user.MangaList == null ? "The Manga list is private."
                    : string.Empty;

                ListsHintLabel.Text = $"The lists of “{ user.Username }”, as MAL XML to import them on MyAnimeList or AniList, or as CSV for spreadsheets. { privateLists }".TrimEnd();
            }
        }

        #endregion
    }
}
