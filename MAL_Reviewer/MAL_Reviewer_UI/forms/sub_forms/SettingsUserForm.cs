using System;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_UI.interfaces;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsUserForm : Form, ISubForm
    {
        #region Constructors

        public SettingsUserForm()
        {
            InitializeComponent();

            DefaultUserTextboxControl.MaxLength = UsernameRule.MaxLength;
            this.VisibleChanged += SettingsUserForm_VisibleChanged;
        }

        #endregion

        #region Private methods

        private void SettingsUserForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitDisplay();
            }
        }

        private void SetDefaultUserButton_Click(object sender, EventArgs e)
        {
            string username = UsernameRule.Normalize(DefaultUserTextboxControl.InnerText);

            // An empty box removes the default user.
            if (username.Length == 0)
            {
                if (Core.Settings.UserSettings.DefaultUser.Length > 0
                    && DialogResult.Yes == MessageBox.Show("Do you want to remove the default user?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    Core.Settings.UserSettings.ClearDefaultUser();
                    Core.Save();

                    MessageBox.Show("The default user was removed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            try
            {
                Core.Settings.UserSettings.SetDefaultUser(username);
                Core.Save();

                MessageBox.Show($"Default user set to “{ username }”.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void UserSettingsResetButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to reset the user settings?\nThe default user will be removed.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                Core.Settings.UserSettings.ResetSettings();
                Core.Save();

                InitDisplay();

                MessageBox.Show("User settings were successfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Public methods

        public void InitDisplay()
        {
            DefaultUserTextboxControl.InnerText = Core.Settings.UserSettings.DefaultUser;
        }

        #endregion
    }
}
