using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using MAL_Reviewer_UI.extensions;

namespace MAL_Reviewer_UI.user_controls
{
    public partial class LoaderControl : UserControl
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
                this.LoaderPictureBox.Image = GetProperImage();
            }
        }

        public LoaderControl()
        {
            InitializeComponent();
            this.Visible = false;
        }

        /// <summary>
        /// Get the apropriate loading gif given the background color.
        /// </summary>
        /// <returns></returns>
        private Image GetProperImage()
        {
            Image _image = null;

            switch (this.Color.Name)
            {
                case "White":
                    _image = Icons.LoadingWhite;
                    break;
                case "ControlLight":
                    _image = Icons.LoadingControlLight;
                    break;
                default:
                    _image = Icons.LoadingControl;
                    break;
            }

            return _image;
        }
    }
}
