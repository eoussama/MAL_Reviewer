using System;
using System.Threading;
using System.Windows.Forms;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.services;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms
{
    public partial class LoadUserForm : Form
    {
        #region Events

        /// <summary>
        /// The event that's invoked when a user is loaded.
        /// </summary>
        public event EventHandler<MALUserModel> UserLoadedEvent;

        #endregion

        #region Fields

        private bool
            // Whether the form is ready for input or not.
            ready = true,

            // Whether or not the form was automatically opened or not.
            auto = false;

        /// <summary>
        /// The optional username passed.
        /// </summary>
        private string username = string.Empty;

        /// <summary>
        /// The option to set the loaded username as the default one.
        /// </summary>
        private bool setDefault = false;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        private readonly CancellationTokenSource cts;

        #endregion

        #region Properties

        /// <summary>
        /// When the loaded user was shown from their saved data instead of MAL, the date of that data.
        /// </summary>
        public DateTimeOffset? SavedAt { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public LoadUserForm()
        {
            InitializeComponent();

            this.ActiveControl = searchControl.Controls["inputTextBox"];
            this.searchControl.MaxLength = UsernameRule.MaxLength;
            cts = new CancellationTokenSource();

            // Showing the form in the chosen theme.
            Theme.Attach(this);
        }

        /// <summary>
        /// Constructor with username.
        /// </summary>
        /// <param name="username"></param>
        public LoadUserForm(string username) : this()
        {
            this.auto = true;
            this.username = username;
        }

        /// <summary>
        /// Constructor with default user.
        /// </summary>
        /// <param name="setDefault"> Whether or not to set the loaded user as the default one. </param>
        public LoadUserForm(bool setDefault) : this()
        {
            this.setDefault = setDefault;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the user's data loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BLoad_Click(object sender, EventArgs e)
        {
            if (!this.ready)
            {
                return;
            }

            string username = UsernameRule.Normalize(searchControl.InnerText);

            if (!UsernameRule.IsValid(username))
            {
                MessageBox.Show(UsernameRule.InvalidMessage, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            this.bLoad.Enabled = false;
            this.searchControl.ToggleLoading(true);
            this.ready = false;

            MALUserModel userModel = null;
            string error = null;
            bool closeOnError = false;

            try
            {
                // Get the data of the user, along with their lists.
                userModel = await new UserLoader(MALHelper.Api).LoadAsync(username, cts.Token);

                if (userModel == null)
                {
                    error = $"No user under the username “{ username }” was found!";
                }
            }
            catch (OperationCanceledException)
            {
                // The form was closed while loading.
                return;
            }
            catch (DataSourceUnavailableException ex)
            {
                error = $"The user's data could not be fetched right now, please try again later.\n\n{ ex.Message }";

                // At start-up, showing the data saved the last time instead, or starting without it.
                if (this.auto)
                {
                    if (Core.UserCache.TryLoad(username, out MALUserModel saved, out DateTimeOffset savedAt))
                    {
                        userModel = saved;
                        this.SavedAt = savedAt;
                        error = null;
                    }
                    else
                    {
                        error = $"MAL can't be reached right now, and no data of “{ username }” was saved on this computer yet, so the application starts without it.\n\n{ ex.Message }";
                        closeOnError = true;
                    }
                }
            }

            if (this.IsDisposed || cts.IsCancellationRequested)
            {
                return;
            }

            this.ready = true;

            if (error != null)
            {
                MessageBox.Show(error, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                if (closeOnError)
                {
                    this.Close();
                    return;
                }

                this.bLoad.Enabled = true;
                this.searchControl.ToggleLoading(false);
                this.ActiveControl = searchControl.Controls["inputTextBox"];
                return;
            }

            if (this.setDefault)
            {
                // Setting the loaded user's name as the default one.
                Core.Settings.UserSettings.DefaultUser = username;
                Core.Save();
            }

            this.Close();
            UserLoadedEvent?.Invoke(this, userModel);
        }

        /// <summary>
        /// Handles the event of when the form is first shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadUserForm_Shown(object sender, EventArgs e)
        {
            if (this.auto)
            {
                searchControl.InnerText = this.username;
                BLoad_Click(sender, e);
            }
        }

        /// <summary>
        /// Redirects to the MAL account creation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LlNoAcc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(MalUrls.Register);

        /// <summary>
        /// Handles the closing of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FLoadUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.ready)
            {
                cts.Cancel();
            }
        }

        /// <summary>
        /// Releases the cancellation source with the form.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            cts.Dispose();
        }

        #endregion
    }
}
