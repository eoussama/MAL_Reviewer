using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;

namespace MAL_Reviewer_UI.user_controls
{
    public partial class LoaderControl : UserControl, IThemedControl
    {
        /// <summary>
        /// Get and set the user control's back color.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color @Color
        {
            get => this.BackColor;
            set
            {
                this.BackColor = value;
                this.LoaderPictureBox.BackColor = value;
                this.LoaderPictureBox.Image = Icons.Loading(value);
            }
        }

        public LoaderControl()
        {
            InitializeComponent();
            this.Visible = false;
        }

        /// <summary>
        /// Recolors the loading animation for the theme's background.
        /// </summary>
        public void ApplyTheme()
        {
            this.LoaderPictureBox.Image = Icons.Loading(this.BackColor);
        }
    }
}
