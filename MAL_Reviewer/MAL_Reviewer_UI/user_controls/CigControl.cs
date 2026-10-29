using System.Windows.Forms;
using System.Drawing;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// A mini control that serves as a category lable.
    /// </summary>
    public partial class CigControl : UserControl
    {
        /// <summary>
        /// The fonts of the close label, (the bold one is shown while hovered and created once).
        /// </summary>
        private Font regularCloseFont, boldCloseFont;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CigControl()
        {
            InitializeComponent();

            this.regularCloseFont = this.closeLabel.Font;

            this.cigLabel.MouseEnter += CigControl_MouseEnter;
            this.cigLabel.MouseLeave += CigControl_MouseLeave;

            this.closeLabel.MouseEnter += CigControl_MouseEnter;
            this.closeLabel.MouseLeave += CigControl_MouseLeave;

            this.Disposed += (s, e) => this.boldCloseFont?.Dispose();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="label"></param>
        public CigControl(string label) : this()
        {
            this.cigLabel.Text = label;
        }

        /// <summary>
        /// Gets the label content.
        /// </summary>
        public string Label
        {
            get => this.cigLabel.Text;
        }

        private void CloseLabel_MouseEnter(object sender, System.EventArgs e)
        {
            this.boldCloseFont ??= new Font(this.regularCloseFont, FontStyle.Bold);
            this.closeLabel.Font = this.boldCloseFont;
        }

        private void CloseLabel_MouseLeave(object sender, System.EventArgs e)
        {
            this.closeLabel.Font = this.regularCloseFont;
        }

        private void CloseLabel_Click(object sender, System.EventArgs e)
        {
            // Removing the label control once the click is handled, since a control can't dispose itself from its own event.
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.Parent?.Controls.Remove(this);
                this.Dispose();
            }));
        }

        private void CigControl_MouseEnter(object sender, System.EventArgs e)
        {
            this.closeLabel.ForeColor = Color.Black;
            this.BackColor = SystemColors.ControlLight;
        }

        private void CigControl_MouseLeave(object sender, System.EventArgs e)
        {
            this.closeLabel.ForeColor = SystemColors.ControlDark;
            this.BackColor = SystemColors.Control;
        }
    }
}
