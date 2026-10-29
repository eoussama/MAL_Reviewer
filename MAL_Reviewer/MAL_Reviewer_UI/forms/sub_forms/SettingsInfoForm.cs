using System;
using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_UI.extensions;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    /// <summary>
    /// The info sub form.
    /// </summary>
    public partial class SettingsInfoForm : Form
    {
        #region Constructors

        public SettingsInfoForm()
        {
            InitializeComponent();

            // Displaying the application's title.
            ApplicationTitleLabel.Text = AppInfo.Title;

            // Displaying the application's version.
            ApplicationVersionLabel.Text = AppInfo.Version;

            // Displaying the application's description.
            ApplicationDescriptionLabel.Text = AppInfo.Description;

            // Displaying the author's name.
            ApplicationAuthorLinkLabel.Text = AppInfo.Author;

            // Displaying the application's license.
            ApplicationLicenseLinkLabel.Text = AppInfo.License;

            // Assigning the author's link.
            ApplicationAuthorLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = AppInfo.AuthorLink });

            // Assigning the license's link.
            ApplicationLicenseLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = AppInfo.LicenseLink });

            // Assigning the repository's link.
            ApplicationRepoLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = AppInfo.RepoLink });

            // Assigning the bug report's link.
            ApplicationBugReportLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = AppInfo.BugReportLink });

            // Assigning the contact email.
            ApplicationContactLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = AppInfo.ContactLink });

            // Moving the version's label next to the title's.
            ApplicationVersionLabel.Left = ApplicationTitleLabel.Right;

            // Wiring click events
            AddClickEvent(JikanPanel, JikanPanel_Click);
            AddClickEvent(FreepikPanel, FreepikPanel_Click);
            AddClickEvent(FontAwesomePanel, FontAwesomePanel_Click);

            // Wiring mouse enter events
            AddMouseEnterEvent(JikanPanel, JikanPanel_MouseEnter);
            AddMouseEnterEvent(FreepikPanel, FreepikPanel_MouseEnter);
            AddMouseEnterEvent(FontAwesomePanel, FontAwesomePanel_MouseEnter);

            // Wiring mouse leave events
            AddMouseLeaveEvent(JikanPanel, JikanPanel_MouseLeave);
            AddMouseLeaveEvent(FreepikPanel, FreepikPanel_MouseLeave);
            AddMouseLeaveEvent(FontAwesomePanel, FontAwesomePanel_MouseLeave);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Redirects to the author's page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationAuthorLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(e.Link.LinkData);

        /// <summary>
        /// Redirects to the license's page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationLicenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(e.Link.LinkData);

        /// <summary>
        /// Redirects to the application's repository.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationRepoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(e.Link.LinkData);

        /// <summary>
        /// Redirects to the bug reporting page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBugReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(e.Link.LinkData);

        /// <summary>
        /// Opens the default mail client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationContactLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Shell.Open(e.Link.LinkData);

        /// <summary>
        /// Redirects to the Jikan project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JikanPanel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Shell.Open(AppInfo.JikanLink);
            }
        }

        /// <summary>
        /// Redirects to the Freepik's website.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreepikPanel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Shell.Open(AppInfo.FreepikLink);
            }
        }

        /// <summary>
        /// Redirects to the Font Awesome pack page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontAwesomePanel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Shell.Open(AppInfo.FontAwesomeLink);
            }
        }

        /// <summary>
        /// Highlights the jikan groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JikanPanel_MouseEnter(object sender, EventArgs e) => SetSectionColor(JikanPanel, SystemColors.Control);

        /// <summary>
        /// Highlights the freepik groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontAwesomePanel_MouseEnter(object sender, EventArgs e) => SetSectionColor(FontAwesomePanel, SystemColors.Control);

        /// <summary>
        /// Highlights the fontawesome groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreepikPanel_MouseEnter(object sender, EventArgs e) => SetSectionColor(FreepikPanel, SystemColors.Control);

        /// <summary>
        /// Resets the jikan groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JikanPanel_MouseLeave(object sender, EventArgs e) => SetSectionColor(JikanPanel, Color.Transparent);

        /// <summary>
        /// Resets the freepik groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontAwesomePanel_MouseLeave(object sender, EventArgs e) => SetSectionColor(FontAwesomePanel, Color.Transparent);

        /// <summary>
        /// Resets the fontawesome groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreepikPanel_MouseLeave(object sender, EventArgs e) => SetSectionColor(FreepikPanel, Color.Transparent);

        /// <summary>
        /// Adds a click event to an element and all its children.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="event"></param>
        private void AddClickEvent(Control parent, MouseEventHandler @event)
        {
            parent.MouseClick += @event;

            foreach (Control control in parent.Controls)
            {
                control.MouseClick += @event;
            }
        }

        /// <summary>
        /// Adds a mouse enter to an element and all its children.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="event"></param>
        private void AddMouseEnterEvent(Control parent, EventHandler @event)
        {
            parent.MouseEnter += @event;

            foreach (Control control in parent.Controls)
            {
                control.MouseEnter += @event;
            }
        }

        /// <summary>
        /// Adds a mouse leave to an element and all its children.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="event"></param>
        private void AddMouseLeaveEvent(Control parent, EventHandler @event)
        {
            parent.MouseLeave += @event;

            foreach (Control control in parent.Controls)
            {
                control.MouseLeave += @event;
            }
        }

        /// <summary>
        /// Sets an element and all its children's backcolor.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="color"></param>
        private void SetSectionColor(Panel panel, Color color)
        {
            panel.BackColor = color;

            foreach (Control control in panel.Controls)
            {
                control.BackColor = color;
            }
        }

        #endregion
    }
}
