using System.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_Core.helpers;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// Custom textbox control.
    /// </summary>
    public partial class TextboxControl : UserControl
    {
        public event EventHandler<string> TextboxSubmitEvent;

        private bool toggleIcon = true;
        private const int searchInterval = 500;

        // Decides whether a submit is new; only successful submits are remembered.
        private readonly SearchGate gate = new SearchGate();
        private string pendingText;
        private int pendingKind;

        LoaderControl loaderControl;
        private System.Windows.Forms.Timer inputDelay;

        /// <summary>
        /// Gets and sets the text in inputTextBox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string InnerText
        {
            get => this.inputTextBox.Text;
            set
            {
                this.inputTextBox.Text = value ?? string.Empty;
                this.placeholderLabel.Visible = this.inputTextBox.Text.Trim().Length == 0;
            }
        }

        /// <summary>
        /// Gets and sets the icon of the iconPictureBox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Icon
        {
            get => iconPictureBox.Image;
            set => iconPictureBox.Image = value;
        }

        /// <summary>
        /// Enables and disables the use of an icon for the textbox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ToggleIcon
        {
            get => this.toggleIcon;
            set
            {
                this.toggleIcon = value;
                this.iconPictureBox.Visible = value;

                if (value)
                {
                    this.inputTextBox.Width = this.Width - (this.inputTextBox.Left * 2) - this.iconPictureBox.Width - (this.iconPictureBox.Left - this.inputTextBox.Right);
                    this.placeholderLabel.Width = this.inputTextBox.Width;
                }
                else
                {
                    this.inputTextBox.Width = this.Width - (this.inputTextBox.Left * 2);
                    this.placeholderLabel.Width = this.inputTextBox.Width;
                }
            }
        }

        /// <summary>
        /// Gets and sets the background color of the textbox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get => this.BackColor;
            set
            {
                this.BackColor = value;
                this.iconPictureBox.BackColor = value;
                this.loaderControl.Color = value;

                if (value != Color.Transparent)
                {
                    this.inputTextBox.BackColor = value;
                }
            }
        }

        /// <summary>
        /// Sets the maximum input length of the textbox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxLength
        {
            set => this.inputTextBox.MaxLength = value;
        }

        /// <summary>
        /// Gets and sets the minimum length required to submit the textbox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SubmitMin
        {
            get => this.gate.MinLength;
            set => this.gate.MinLength = value;
        }

        /// <summary>
        /// Gets and sets the ability to activates the loading animation whne finishing typing.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AllowLoad { get; set; } = false;

        /// <summary>
        /// Gets and sets the ability to auto submit the textbox when finished typing.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AutoSubmit { get; set; } = false;

        /// <summary>
        /// Gets and sets the control's placeholder.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Placeholder
        {
            get => this.placeholderLabel.Text;
            set => this.placeholderLabel.Text = value;
        }

        /// <summary>
        /// Gets and sets the control's placeholder's forecolor.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PlaceholderColor
        {
            get => this.placeholderLabel.ForeColor;
            set => this.placeholderLabel.ForeColor = value;
        }

        public TextboxControl()
        {
            InitializeComponent();

            this.loaderControl = new LoaderControl()
            {
                Color = this.BackgroundColor,
                Bounds = this.iconPictureBox.Bounds
            };
            this.Controls.Add(loaderControl);
            this.loaderControl.BringToFront();
            this.inputDelay = new System.Windows.Forms.Timer
            {
                Interval = searchInterval
            };
            this.inputDelay.Tick += InputDelay_Tick;
            this.Disposed += (s, e) => this.inputDelay.Dispose();

            TextboxControl_Leave(this, EventArgs.Empty);
        }

        private void InputTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.inputDelay.Stop();
            this.inputDelay.Start();
        }

        private void InputDelay_Tick(object sender, System.EventArgs e)
        {
            this.inputDelay.Stop();

            // Check if the ability to submit the textbox when finished typing is on.
            if (this.AutoSubmit)
            {
                this.Submit();
            }
        }

        /// <summary>
        /// Toggles the loading animation for the textbox.
        /// </summary>
        public void ToggleLoading(bool state)
        {
            this.inputTextBox.Enabled = !state;
            this.iconPictureBox.Visible = !state && this.ToggleIcon;
            this.loaderControl.Visible = state && this.ToggleIcon;

            this.Cursor = (state ? Cursors.WaitCursor : Cursors.Default);
        }

        /// <summary>
        /// Submit the textbox.
        /// </summary>
        public void Submit()
        {
            string text = this.inputTextBox.Text.Trim();
            int kind = GetSearchKind();

            // Check if the text is long enough and differs from the last successful submit, (or its kind does).
            if (!this.gate.ShouldSubmit(text, kind))
            {
                return;
            }

            this.pendingText = text;
            this.pendingKind = kind;

            // Check if the loading animation is allowed; it's turned on before the handler runs,
            // so a handler that finishes immediately can still turn it off.
            if (this.AllowLoad)
            {
                this.ToggleLoading(true);
            }

            this.TextboxSubmitEvent?.Invoke(this, text);
        }

        /// <summary>
        /// Tells the textbox how the last submit ended. Only successful submits are remembered,
        /// so a failed one can be retried with the same text.
        /// </summary>
        /// <param name="succeeded"> Whether the submit succeeded. </param>
        public void CompleteSubmit(bool succeeded)
        {
            if (succeeded)
            {
                this.gate.MarkSucceeded(this.pendingText, this.pendingKind);
            }
        }

        /// <summary>
        /// The kind of search, stored in the control's tag, (ex: the selected Anime/Manga option).
        /// </summary>
        private int GetSearchKind()
        {
            return int.TryParse(this.Tag?.ToString(), out int kind) ? kind : 0;
        }


        /// <summary>
        /// Clear the textbox.
        /// </summary>
        public void Clear() => this.inputTextBox.Clear();

        private void PlaceholderLabel_Click(object sender, EventArgs e)
        {
            this.placeholderLabel.Visible = false;
            this.ActiveControl = this.inputTextBox;
        }

        private void TextboxControl_Enter(object sender, EventArgs e)
        {
            this.placeholderLabel.Visible = false;
        }

        private void TextboxControl_Leave(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text.Length == 0)
                this.placeholderLabel.Visible = true;
        }
    }
}
