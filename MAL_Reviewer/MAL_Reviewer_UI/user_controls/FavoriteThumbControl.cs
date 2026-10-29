using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.user_controls
{
    public partial class FavoriteThumbControl : UserControl
    {
        public FavoriteThumbControl(string title, string image_url, string type)
        {
            InitializeComponent();

            this.lTitle.Text = title;

            // The thumbnail is downloaded in the background.
            _ = this.pbImage.LoadImageAsync(image_url);

            // Tooltip initialization.
            this.ttTitle.ToolTipTitle = $"{ type } name";

            this.ttTitle.SetToolTip(this.lTitle, title);
            this.ttTitle.SetToolTip(this.pbImage, title);
            this.ttTitle.SetToolTip(this, title);

            // Wiring up the mouse click event.
            this.lTitle.MouseClick += UcFavoriteThumb_MouseClick;
            this.pbImage.MouseClick += UcFavoriteThumb_MouseClick;

            this.lTitle.MouseEnter += UcFavoriteThumb_MouseEnter;
            this.pbImage.MouseEnter += UcFavoriteThumb_MouseEnter;

            this.lTitle.MouseLeave += UcFavoriteThumb_MouseLeave;
            this.pbImage.MouseLeave += UcFavoriteThumb_MouseLeave;
        }

        private void UcFavoriteThumb_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Shell.Open(this.Tag);
        }

        private void UcFavoriteThumb_MouseEnter(object sender, System.EventArgs e) => this.SetThemedBackColor(SystemColors.Control);

        private void UcFavoriteThumb_MouseLeave(object sender, System.EventArgs e) => this.SetThemedBackColor(SystemColors.ControlLight);
    }
}
