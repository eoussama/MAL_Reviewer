using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.exceptions;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.user_controls;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsTemplateForm : Form, ISubForm
    {
        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public SettingsTemplateForm()
        {
            InitializeComponent();

            // Styling the aspect add button, once, so its events are not subscribed again on every display.
            StyleAspectAdd();

            this.VisibleChanged += SettingsTemplateForm_VisibleChanged;
        }

        #endregion

        #region CRUD management

        /// <summary>
        /// Handles the operation of setting a review template back to its default
        /// configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateDefaultButton_Click(object sender, EventArgs e)
        {
            DisplayReviewTemplateInfo();
        }

        /// <summary>
        /// Updates a selected review template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateUpdateButton_Click(object sender, EventArgs e)
        {
            // Getting the selected index of the listbox.
            short selectedIndex = (short)this.templateListBox.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            try
            {
                // Saving the review template's name that's about to be updated.
                string oldName = templateListBox.Text;

                // Creating the updated review template, keeping its creation date.
                ReviewTemplateModel reviewTemplateModel = TemplateEditor.BuildUpdated(
                    Core.Settings.ReviewTemplatesSettings.Get(selectedIndex),
                    this.templatePreviewRichTextBox.Text,
                    GetAspectNamesFromUI(),
                    templateIntroCheckBox.Checked,
                    templateTLDRCheckBox.Checked,
                    DateTime.Now);

                Core.Settings.ReviewTemplatesSettings.Update(selectedIndex, reviewTemplateModel);
                Core.Save();
                LoadReviewTemplates(selectedIndex);

                MessageBox.Show($"The review template “{ oldName }” was successfully updated!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex) when (ex.ParamName == "name")
            {
                MessageBox.Show(TemplateEditor.MissingNameMessage, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (InvalidReviewTemplateException ex)
            {
                MessageBox.Show(ex.Message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Deletes the selected review template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateDeleteButton_Click(object sender, EventArgs e)
        {
            short selectedIndex = (short)this.templateListBox.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            // Getting the selecte review template's name and saving it in a temporary variable.
            string reviewTemplateName = this.templateListBox.Text;

            if (DialogResult.Yes == MessageBox.Show($"Do you really want to delete the “{ reviewTemplateName }” review template?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                try
                {
                    Core.Settings.ReviewTemplatesSettings.Delete(selectedIndex);
                    Core.Save();
                    LoadReviewTemplates(selectedIndex);

                    MessageBox.Show($"The review template “{ reviewTemplateName }” was successfully removed!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (InvalidReviewTemplateException ex)
                {
                    MessageBox.Show(ex.Message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// Creates a new review template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateCreateButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Getting the appropriate review template name.
                string reviewTemplateName = Core.Settings.ReviewTemplatesSettings.GetDuplicateName();

                // Creating a new empty review template.
                Core.Settings.ReviewTemplatesSettings.Add(new ReviewTemplateModel(reviewTemplateName, false, false, DateTime.Now, DateTime.Now, new List<ReviewAspectModel>()));
                Core.Save();

                // Refreshing the UI, selecting the new review template.
                LoadReviewTemplates(Core.Settings.ReviewTemplatesSettings.Count - 1);

                MessageBox.Show("A new review template was successfully created!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MaximumReviewTemplatesException ex)
            {
                MessageBox.Show(ex.Message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Clear all review templates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateClearButton_Click(object sender, EventArgs e)
        {
            short reviewTemplatesCount = Core.Settings.ReviewTemplatesSettings.Count;

            if (DialogResult.Yes == MessageBox.Show($"Do you really want to clear all { reviewTemplatesCount } review template(s)?\nOnce deleted they are permanently lost.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                Core.Settings.ReviewTemplatesSettings.Clear();
                Core.Save();

                LoadReviewTemplates();
                MessageBox.Show($"All { reviewTemplatesCount } review template(s) were successfully deleted!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Resets the review template settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewTemplateSettingsResetButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show($"Are you sure you want to reset the template settings?\nAll existing templates with be deleted in the process.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                // Resetting the review template's settings.
                Core.Settings.ReviewTemplatesSettings.ResetSettings();
                Core.Save();

                // Refreshing the UI, selecting the first review template.
                LoadReviewTemplates(0);

                MessageBox.Show("Review template settings were seccessfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region UI control

        /// <summary>
        /// Loads the review templates from the memory and updates the UI of the form.
        /// </summary>
        /// <param name="selectedIndex"> The review template to select; kept within the list's bounds. </param>
        private void LoadReviewTemplates(int selectedIndex = 0)
        {
            List<ReviewTemplateModel> reviewTemplates = Core.Settings.ReviewTemplatesSettings.ReviewTemplates;

            this.templateLabel.Text = TemplateSummary.Counter(reviewTemplates.Count);

            this.templateListBox.DataSource = null;
            this.templateListBox.DataSource = reviewTemplates;
            this.templateListBox.DisplayMember = "TemplateName";

            // Rebinding selects the first item, so the wanted one is selected again.
            if (reviewTemplates.Count > 0)
            {
                this.templateListBox.SelectedIndex = Math.Clamp(selectedIndex, 0, reviewTemplates.Count - 1);
            }

            // Toggle the review template management buttons.
            this.templateDeleteButton.Enabled = this.templateListBox.Items.Count > 0;
            this.templateUpdateButton.Enabled = this.templateListBox.Items.Count > 0;
            this.templateDefaultButton.Enabled = this.templateListBox.Items.Count > 0;
            this.reviewTemplateClearButton.Enabled = this.templateListBox.Items.Count > 0;

            DisplayEmptyUI();
        }

        /// <summary>
        /// Updates the preview UI with the selected review template's information.
        /// </summary>
        private void DisplayReviewTemplateInfo()
        {
            if (this.templateListBox.DataSource != null && this.templateListBox.SelectedIndex >= 0)
            {
                // Fetching the selected review template.
                ReviewTemplateModel reviewTemplateModel = Core.Settings.ReviewTemplatesSettings.Get((short)this.templateListBox.SelectedIndex);

                // Updating the title.
                this.templatePreviewRichTextBox.Text = reviewTemplateModel.TemplateName;

                // Updating the aspects.
                this.templateAspectsFlowPanel.Controls.ClearAndDispose();
                reviewTemplateModel.TemplateAspects?.ForEach(aspect => SubmitAspect(aspect.AspectName));

                // Updating the checkboxes.
                this.templateIntroCheckBox.Checked = reviewTemplateModel.UseIntro;
                this.templateTLDRCheckBox.Checked = reviewTemplateModel.UserTLDR;

                // Updating the dates.
                this.creationDateLabel.Text = $"Created on { reviewTemplateModel.CreationDate.ToShortDateString() }";
                this.editDateLabel.Text = $"Last modified on { reviewTemplateModel.ModificationDate.ToShortDateString() }";
                this.reviewTemplateTooltip.SetToolTip(this.creationDateLabel, $"{ reviewTemplateModel.CreationDate.ToLongDateString() } - { reviewTemplateModel.CreationDate.ToLongTimeString() }");
                this.reviewTemplateTooltip.SetToolTip(this.editDateLabel, $"{ reviewTemplateModel.ModificationDate.ToLongDateString() } - { reviewTemplateModel.ModificationDate.ToLongTimeString() }");

                //Aligning the richtextbox content.
                this.templatePreviewRichTextBox.SelectAll();
                this.templatePreviewRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Displays an appropriate UI when there are no review templates created.
        /// </summary>
        private void DisplayEmptyUI()
        {
            if (Core.Settings.ReviewTemplatesSettings.ReviewTemplates.Count > 0 && this.noPreviewLabel.Visible == true)
            {
                // Showing all the preview panel's children but the noPreview label.
                foreach (Control control in this.templatePreviewPanel.Controls)
                {
                    control.Visible = (control != this.noPreviewLabel);
                }

                // Hidding the noPreview label.
                this.noPreviewLabel.Visible = false;

            }
            else if (Core.Settings.ReviewTemplatesSettings.ReviewTemplates.Count == 0 && this.noPreviewLabel.Visible == false)
            {
                // Hiding all the preview panel's children but the noPreview label.
                foreach (Control control in this.templatePreviewPanel.Controls)
                {
                    control.Visible = !(control != this.noPreviewLabel);
                }

                // Showing the noPreview label.
                this.noPreviewLabel.Visible = true;

                // Centering the noPreview label.
                this.noPreviewLabel.Location = new Point((this.templatePreviewPanel.Width / 2) - (this.noPreviewLabel.Width / 2), (this.templatePreviewPanel.Height / 2) - (this.noPreviewLabel.Height / 2));
            }
        }

        /// <summary>
        /// Puts the first building blocks of a sub form.
        /// </summary>
        public void InitDisplay()
        {
            // Loading the review templates.
            LoadReviewTemplates(this.templateListBox.SelectedIndex);
        }

        /// <summary>
        /// Handles the listbox' items selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayReviewTemplateInfo();
        }

        /// <summary>
        /// Handles the sub form's visibility change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsTemplateForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                InitDisplay();
            }
        }

        #endregion

        #region Aspect management

        /// <summary>
        /// Handles the keydown event on the aspects' textbox.
        /// Used to detect the return key press so that the typed
        /// aspect can be submit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AspectsAdd_KeyDown(object sender, KeyEventArgs e)
        {
            string label = this.aspectsTextBox.InnerText.Trim();

            if (e.KeyCode == Keys.Return && label.Length > 0)
            {
                SubmitAspect(label);
            }
        }

        /// <summary>
        /// Handles the mouse clicks on aspects' textbox add button.
        /// Used to submit the typed value on the textbox as CigControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AspectsAdd_MouseClick(object sender, MouseEventArgs e)
        {
            string label = this.aspectsTextBox.InnerText.Trim();

            if (e.Button == MouseButtons.Left && label.Length > 0)
            {
                SubmitAspect(label);
            }
        }

        /// <summary>
        /// Adds visual updates to the add button of the aspects
        /// and assigns a submit event.
        /// </summary>
        private void StyleAspectAdd()
        {
            // Add the hand cursor.
            aspectsTextBox.Controls["iconPictureBox"].Cursor = Cursors.Hand;

            // Click event.
            aspectsTextBox.Controls["iconPictureBox"].MouseClick += AspectsAdd_MouseClick;

            // Key event.
            aspectsTextBox.Controls["inputTextBox"].KeyDown += AspectsAdd_KeyDown;
        }

        /// <summary>
        /// Creates an aspect label and submits it.
        /// </summary>
        /// <param name="label"></param>
        private void SubmitAspect(string label)
        {
            this.templateAspectsFlowPanel.Controls.Add(new CigControl(label));
            this.aspectsTextBox.Clear();
        }

        /// <summary>
        /// Reads the aspect names from the content of the UI.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetAspectNamesFromUI()
        {
            return templateAspectsFlowPanel.Controls.OfType<CigControl>().Select(cigControl => cigControl.Label).ToList();
        }

        #endregion
    }
}
