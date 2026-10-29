using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.forms.sub_forms;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms
{
    public partial class SettingsForm : Form
    {
        #region Fields

        private Form
            GeneralSubForm,
            UserSubForm,
            ThemeSubForm,
            ReviewTemplateSubForm,
            SearchSubForm,
            InfoSubForm;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            // Instanciating the sub forms.
            GeneralSubForm = new SettingsGeneralForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            UserSubForm = new SettingsUserForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            ThemeSubForm = new SettingsThemeForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            ReviewTemplateSubForm = new SettingsTemplateForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            SearchSubForm = new SettingsSearchForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            InfoSubForm = new SettingsInfoForm()
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };

            // Adding the sub forms to the content panel.
            ContentPanel.Controls.AddRange(new Form[] {
                GeneralSubForm,
                UserSubForm,
                ThemeSubForm,
                ReviewTemplateSubForm,
                SearchSubForm,
                InfoSubForm
            });

            GeneralButton_Click(GeneralButton, EventArgs.Empty);

            // Showing the form in the chosen theme.
            Theme.Attach(this);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Opens the “General” settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneralButton_Click(object sender, EventArgs e) => GeneralSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        /// <summary>
        /// Opens the “User”'s settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserButton_Click(object sender, EventArgs e) => UserSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        /// <summary>
        /// Opens the “Theme”'s settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeButton_Click(object sender, EventArgs e) => ThemeSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        /// <summary>
        /// Opens the “Review Templates”'s settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateButton_Click(object sender, EventArgs e) => ReviewTemplateSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        /// <summary>
        /// Opens the “Search”'s settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, EventArgs e) => SearchSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        /// <summary>
        /// Opens the “Info”'s settings sub form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoButton_Click(object sender, EventArgs e) => InfoSubForm.ToggleSubForm((Button)sender, ContentPanel, SidePanel);

        #endregion
    }
}
