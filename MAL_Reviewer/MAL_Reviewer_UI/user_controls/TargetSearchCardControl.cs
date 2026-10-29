using System.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// Search card control.
    /// </summary>
    public partial class TargetSearchCardControl : UserControl, IThemedControl
    {
        public event EventHandler<int> CardMouseClickEvent;
        private int targetId;

        /// <summary>
        /// The loading animation shown until the cover is downloaded.
        /// </summary>
        private Image placeholder;

        public TargetSearchCardControl()
        {
            InitializeComponent();

            this.placeholder = this.pbTargetImage.Image;

            // Wiring up the click event.
            this.MouseClick += UcTargetSearchCard_MouseClick;
            this.pbTargetImage.MouseClick += UcTargetSearchCard_MouseClick;
            this.lTargetTitle.MouseClick += UcTargetSearchCard_MouseClick;
            this.lTargetType.MouseClick += UcTargetSearchCard_MouseClick;

            // Wiring up the hover event.
            this.MouseEnter += CardMouseEnter;
            this.pbTargetImage.MouseEnter += CardMouseEnter;
            this.lTargetTitle.MouseEnter += CardMouseEnter;
            this.lTargetType.MouseEnter += CardMouseEnter;

            this.MouseLeave += CardMouseLeave;
            this.pbTargetImage.MouseLeave += CardMouseLeave;
            this.lTargetTitle.MouseLeave += CardMouseLeave;
            this.lTargetType.MouseLeave += CardMouseLeave;
        }

        /// <summary>
        /// Gets and sets the MAL id of the target assigned to the card.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TargetId { get => targetId; set => targetId = value; }

        /// <summary>
        /// Updates the card's UI.
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetTitle"></param>
        /// <param name="targetType"></param>
        /// <param name="targetImageUrl"></param>
        /// <param name="searchType"></param>
        public void UpdateUI (int targetId, string targetTitle, string targetType, string targetImageUrl, string searchType)
        {
            this.TargetId = targetId;
            lTargetTitle.Text = DisplayFormat.Truncate(targetTitle, 17);
            lTargetType.Text = DisplayFormat.OrUnknown(targetType);
            _ = pbTargetImage.LoadImageAsync(targetImageUrl);

            ttTitle.ToolTipTitle = $"{ searchType } information";
            ttTitle.SetToolTip(this, targetTitle);
            ttTitle.SetToolTip(this.lTargetTitle, targetTitle);
            ttTitle.SetToolTip(this.lTargetType, targetTitle);
            ttTitle.SetToolTip(this.pbTargetImage, targetTitle);
        }

        private void UcTargetSearchCard_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CardMouseClickEvent?.Invoke(sender, TargetId);
        }

        private void CardMouseEnter(object sender, EventArgs e) => this.SetThemedBackColor(SystemColors.Control);

        private void CardMouseLeave(object sender, EventArgs e) => this.SetThemedBackColor(SystemColors.ControlLight);

        /// <summary>
        /// Recolors the loading animation for the theme, unless the cover is already shown.
        /// </summary>
        public void ApplyTheme()
        {
            if (this.pbTargetImage.Image == this.placeholder)
            {
                this.placeholder = Icons.Loading(Theme.Back(SystemColors.ControlLight));
                this.pbTargetImage.Image = this.placeholder;
            }
        }
    }
}
