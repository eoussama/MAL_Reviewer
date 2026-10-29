using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.services;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.theming;
using MAL_Reviewer_UI.user_controls;

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

        /// <summary>
        /// The drop-down of recently loaded users.
        /// </summary>
        private readonly RecentUsersControl recentUsers;

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

            // The recently loaded users, shown under the username box as a drop-down.
            this.recentUsers = new RecentUsersControl
            {
                Location = new Point(searchControl.Left, searchControl.Bottom),
                Name = "recentUsers",
                Width = searchControl.Width
            };

            this.recentUsers.UserChosen += RecentUsers_UserChosen;
            this.recentUsers.UserForgotten += RecentUsers_UserForgotten;
            this.Controls.Add(this.recentUsers);
            this.recentUsers.BringToFront();

            Control input = searchControl.Controls["inputTextBox"];
            input.Enter += (sender, e) => ShowRecentUsers();
            input.TextChanged += (sender, e) => ShowRecentUsers();
            input.Leave += (sender, e) => this.recentUsers.Visible = false;
            input.KeyDown += Input_KeyDown;

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
            this.recentUsers.Visible = false;

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

            // Retries after the automatic load are made by hand.
            this.auto = false;

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

            // Remembering the user for the search of this form.
            Core.Settings.UserSettings.RememberUser(userModel.Username ?? username, userModel.Image_url, DateTimeOffset.Now);

            if (this.setDefault)
            {
                // Setting the loaded user's name as the default one.
                Core.Settings.UserSettings.DefaultUser = username;
            }

            try
            {
                Core.Settings.SaveSettings();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // The settings are saved again when the application closes.
            }

            this.Close();
            UserLoadedEvent?.Invoke(this, userModel);
        }

        /// <summary>
        /// Shows the recently loaded users matching the typed name, unless a user is being loaded.
        /// </summary>
        private void ShowRecentUsers()
        {
            if (!this.ready || this.auto)
            {
                this.recentUsers.Visible = false;
                return;
            }

            this.recentUsers.ShowUsers(Core.Settings.UserSettings.FindRecentUsers(searchControl.InnerText));
        }

        /// <summary>
        /// Loads a recently loaded user again.
        /// </summary>
        private void RecentUsers_UserChosen(object sender, string name)
        {
            searchControl.InnerText = name;
            BLoad_Click(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Forgets a recently loaded user.
        /// </summary>
        private void RecentUsers_UserForgotten(object sender, string name)
        {
            Core.Settings.UserSettings.ForgetUser(name);

            try
            {
                Core.Settings.SaveSettings();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // The settings are saved again when the application closes.
            }

            ShowRecentUsers();
            this.ActiveControl = searchControl.Controls["inputTextBox"];
        }

        /// <summary>
        /// Browses the recently loaded users with the arrow keys, closes them with Escape, and loads with Enter.
        /// </summary>
        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    if (!this.recentUsers.Visible)
                    {
                        ShowRecentUsers();
                    }

                    this.recentUsers.MoveHighlight(e.KeyCode == Keys.Down ? 1 : -1);
                    break;

                case Keys.Escape when this.recentUsers.Visible:
                    this.recentUsers.Visible = false;
                    break;

                case Keys.Return:
                    string highlighted = this.recentUsers.Visible ? this.recentUsers.HighlightedUser : null;

                    if (highlighted != null)
                    {
                        RecentUsers_UserChosen(sender, highlighted);
                    }
                    else
                    {
                        BLoad_Click(sender, EventArgs.Empty);
                    }

                    break;

                default:
                    return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
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
            else
            {
                // The username box has the focus from the start, so the recent users show right away.
                ShowRecentUsers();
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
