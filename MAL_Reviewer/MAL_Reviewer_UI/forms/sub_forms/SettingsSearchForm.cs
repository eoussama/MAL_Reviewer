using System;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_UI.interfaces;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsSearchForm : Form, ISubForm
    {
        #region Fields

        /// <summary>
        /// Set while the controls are filled from the settings, so filling them isn't taken as a change.
        /// </summary>
        private bool displaying = false;

        #endregion

        #region Constructors

        public SettingsSearchForm()
        {
            InitializeComponent();

            // The allowed ranges come from the settings themselves.
            // Changing a range moves the value into it, which must not be taken as the user's change.
            this.displaying = true;
            DelayNumericUpDown.Minimum = SearchController.MinDelay;
            DelayNumericUpDown.Maximum = SearchController.MaxDelay;
            ResultsNumericUpDown.Minimum = SearchController.MinResults;
            ResultsNumericUpDown.Maximum = SearchController.MaxResults;
            this.displaying = false;

            ResultsUnitLabel.Text = $"results per search ({ SearchController.MinResults } to { SearchController.MaxResults })";
            DelayUnitLabel.Text = $"milliseconds after the last key press ({ SearchController.MinDelay } to { SearchController.MaxDelay })";

            this.VisibleChanged += SettingsSearchForm_VisibleChanged;
        }

        #endregion

        #region Private methods

        private void SettingsSearchForm_VisibleChanged(object sender, System.EventArgs e)
        {
            if (this.Visible)
            {
                InitDisplay();
            }
        }

        private void SearchWhileTypingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // The delay only matters when searching while typing.
            DelayNumericUpDown.Enabled = SearchWhileTypingCheckBox.Checked;

            if (!this.displaying)
            {
                Core.Settings.SearchSettings.SearchWhileTyping = SearchWhileTypingCheckBox.Checked;
            }
        }

        private void DelayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!this.displaying)
            {
                Core.Settings.SearchSettings.Delay = (int)DelayNumericUpDown.Value;
            }
        }

        private void ResultsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!this.displaying)
            {
                Core.Settings.SearchSettings.ResultsLimit = (int)ResultsNumericUpDown.Value;
            }
        }

        private void ModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.displaying && ((RadioButton)sender).Checked)
            {
                Core.Settings.SearchSettings.DefaultMode = LinkModeRadioButton.Checked ? SearchMode.Link : SearchMode.Title;
            }
        }

        private void SearchSettingsResetButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to reset the search settings?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                Core.Settings.SearchSettings.ResetSettings();
                Core.Save();

                InitDisplay();

                MessageBox.Show("Search settings were successfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Public methods

        // Displays initial information on the sub form.
        public void InitDisplay()
        {
            SearchController settings = Core.Settings.SearchSettings;

            this.displaying = true;

            SearchWhileTypingCheckBox.Checked = settings.SearchWhileTyping;
            DelayNumericUpDown.Value = settings.Delay;
            DelayNumericUpDown.Enabled = settings.SearchWhileTyping;
            ResultsNumericUpDown.Value = settings.ResultsLimit;
            TitleModeRadioButton.Checked = settings.DefaultMode == SearchMode.Title;
            LinkModeRadioButton.Checked = settings.DefaultMode == SearchMode.Link;

            this.displaying = false;
        }

        #endregion
    }
}
