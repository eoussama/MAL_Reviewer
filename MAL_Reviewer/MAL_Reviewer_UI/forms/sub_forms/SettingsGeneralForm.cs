using System;
using System.IO;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsGeneralForm : Form, ISubForm
    {
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
                Theme.Use(Core.Settings.ThemeSettings.ActivePalette);

                // Update the displayed information.
                InitDisplay();

                MessageBox.Show("Application's settings have been successfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Public methods

        // Displays initial information on the sub form.
        public void InitDisplay()
        {
            // Resetting the storage folder path control.
            StoragePathBrowserControl.BrowserPath = Core.Configurations.StorageDirectory;
        }

        #endregion
    }
}
