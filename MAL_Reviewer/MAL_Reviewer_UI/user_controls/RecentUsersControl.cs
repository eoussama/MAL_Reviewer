using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MAL_Reviewer_Core.models.UserModels;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// A drop-down list of recently loaded users, to load one again or to forget it.
    /// </summary>
    public class RecentUsersControl : Panel
    {
        #region Events

        /// <summary>
        /// Invoked with the name of the user picked to be loaded.
        /// </summary>
        public event EventHandler<string> UserChosen;

        /// <summary>
        /// Invoked with the name of the user to forget.
        /// </summary>
        public event EventHandler<string> UserForgotten;

        #endregion

        #region Fields

        private const int RowHeight = 34, VisibleRows = 5;

        private static readonly Font NameFont = new Font("Bahnschrift", 9.75F);
        private static readonly Font DateFont = new Font("Bahnschrift Light", 8.25F);

        private readonly ToolTip tips = new ToolTip();

        private readonly List<Panel> rows = new List<Panel>();
        private int highlighted = -1;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the highlighted user, or null.
        /// </summary>
        public string HighlightedUser => this.highlighted >= 0 ? (string)this.rows[this.highlighted].Tag : null;

        #endregion

        #region Constructors

        public RecentUsersControl()
        {
            this.AutoScroll = true;
            this.BackColor = SystemColors.ControlLightLight;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Visible = false;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows users, or hides the list when there are none.
        /// </summary>
        /// <param name="users"> The users to show, in order. </param>
        public void ShowUsers(IReadOnlyList<RecentUserModel> users)
        {
            this.SuspendLayout();

            this.rows.Clear();
            this.highlighted = -1;
            this.Controls.ClearAndDispose();

            for (int i = 0; i < users.Count; i++)
            {
                Panel row = CreateRow(users[i], i);
                this.rows.Add(row);
                this.Controls.Add(row);
            }

            // Leaving room for the scroll bar once there are more users than rows shown.
            int width = this.ClientSize.Width - (users.Count > VisibleRows ? SystemInformation.VerticalScrollBarWidth : 0);
            this.rows.ForEach(row => row.Width = width);

            this.Height = Math.Min(users.Count, VisibleRows) * RowHeight + 2;
            this.Visible = users.Count > 0;

            this.ResumeLayout();
        }

        /// <summary>
        /// Moves the highlight up or down the list.
        /// </summary>
        /// <param name="step"> 1 to go down, -1 to go up. </param>
        public void MoveHighlight(int step)
        {
            if (this.rows.Count == 0)
            {
                return;
            }

            Highlight(Math.Clamp(this.highlighted + step, 0, this.rows.Count - 1));
            this.ScrollControlIntoView(this.rows[this.highlighted]);
        }

        #endregion

        #region Protected methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tips.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private methods

        private Panel CreateRow(RecentUserModel user, int index)
        {
            Panel row = new Panel
            {
                BackColor = SystemColors.ControlLightLight,
                Cursor = Cursors.Hand,
                Location = new Point(0, index * RowHeight),
                Name = $"RecentUser{ index }",
                Size = new Size(this.ClientSize.Width, RowHeight),
                Tag = user.Username
            };

            PictureBox picture = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(6, 5),
                Size = new Size(24, 24),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            Label name = new Label
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = NameFont,
                ForeColor = SystemColors.ActiveCaptionText,
                Location = new Point(36, 2),
                Name = "NameLabel",
                Text = user.Username
            };

            Label date = new Label
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = DateFont,
                ForeColor = SystemColors.ControlDark,
                Location = new Point(37, 18),
                Text = $"Loaded { user.LoadedAt.LocalDateTime:MMM d, yyyy}"
            };

            Label forget = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent,
                Font = NameFont,
                ForeColor = SystemColors.ControlDark,
                Location = new Point(row.Width - 24, 8),
                Name = "ForgetLabel",
                Size = new Size(18, 18),
                Text = "×",
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.tips.SetToolTip(forget, "Forget this user");
            _ = picture.LoadImageAsync(user.ImageUrl, Icons.User);

            foreach (Control part in new Control[] { row, picture, name, date })
            {
                part.MouseEnter += (sender, e) => Highlight(index);
                part.MouseClick += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        UserChosen?.Invoke(this, user.Username);
                    }
                };
            }

            forget.MouseEnter += (sender, e) => { Highlight(index); forget.SetThemedForeColor(Color.Black); };
            forget.MouseLeave += (sender, e) => forget.SetThemedForeColor(SystemColors.ControlDark);
            forget.Click += (sender, e) => BeginInvoke(new MethodInvoker(() => UserForgotten?.Invoke(this, user.Username)));

            row.Controls.AddRange(new Control[] { forget, picture, name, date });

            return row;
        }

        private void Highlight(int index)
        {
            for (int i = 0; i < this.rows.Count; i++)
            {
                this.rows[i].SetThemedBackColor(i == index ? SystemColors.Control : SystemColors.ControlLightLight);
            }

            this.highlighted = index;
        }

        #endregion
    }
}
