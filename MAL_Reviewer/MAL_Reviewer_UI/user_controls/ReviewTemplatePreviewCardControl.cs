using System;
using System.Windows.Forms;
using System.Drawing;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// The review template card preview.
    /// Responsible on displaying brief information on a review template
    /// for the purpose of previewing them in the NewReviewForm.
    /// </summary>
    public partial class ReviewTemplatePreviewCardControl : UserControl
    {
        public event EventHandler ControlCheckedEventHandler;
        private ReviewTemplateModel reviewTemplateModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReviewTemplatePreviewCardControl(ReviewTemplateModel reviewTemplateModel)
        {
            InitializeComponent();

            this.reviewTemplateModel = reviewTemplateModel;
            this.selectRadioButton.Text = TemplateSummary.CardName(this.reviewTemplateModel);
            this.aspectsLabel.Text = TemplateSummary.Aspects(this.reviewTemplateModel);
            this.introLabel.Text = TemplateSummary.Intro(this.reviewTemplateModel);
            this.tldrLabel.Text = TemplateSummary.Tldr(this.reviewTemplateModel);
            this.extraInfoTooltip.ToolTipTitle = this.reviewTemplateModel.TemplateName;

            // Tooltip content.
            string aspectString = this.reviewTemplateModel.TemplateAspects.Count > 0 ? string.Empty : " ";
            this.reviewTemplateModel.TemplateAspects.ForEach(aspect => aspectString += $"{ aspect.AspectName }\n");
            this.extraInfoTooltip.SetToolTip(this, aspectString);
            this.extraInfoTooltip.SetToolTip(this.containerPanel, aspectString);

            // Wiring the mouse events to the control.
            this.MouseEnter += Control_MouseEnter;
            this.MouseLeave += Control_MouseLeave;
            this.MouseClick += Control_MouseClick;

            // Wiring the mouse events to the container panel.
            this.containerPanel.MouseEnter += Control_MouseEnter;
            this.containerPanel.MouseLeave += Control_MouseLeave;
            this.containerPanel.MouseClick += Control_MouseClick;

            // Wiring the mouse events to the container panel's children.
            foreach (Control control in this.containerPanel.Controls)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
                control.MouseClick += Control_MouseClick;

                this.extraInfoTooltip.SetToolTip(control, aspectString);
            }
        }        

        /// <summary>
        /// Gets and sets the background color of the control.
        /// </summary>
        private Color BackgroundColor
        {
            get => this.BackColor;
            set
            {
                this.SetThemedBackColor(value);
                this.containerPanel.SetThemedBackColor(value);

                // Changing the backcolor of all children of the container panel.
                foreach (Control control in this.containerPanel.Controls)
                {
                    control.SetThemedBackColor(value);
                }
            }
        }

        /// <summary>
        /// Check and Unchecks the control.
        /// </summary>
        /// <param name="state"></param>
        public void Check(bool state)
        {
            this.selectRadioButton.Checked = state;

            if (state)
            {
                this.ControlCheckedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Control_MouseEnter(object sender, System.EventArgs e)
        {
            this.BackgroundColor = SystemColors.ScrollBar;
        }

        private void Control_MouseLeave(object sender, System.EventArgs e)
        {
            this.BackgroundColor = SystemColors.ControlLight;
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Check(true);
            }
        }
    }
}
