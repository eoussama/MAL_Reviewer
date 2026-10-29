using System.ComponentModel;
using System;
using System.IO;
using System.Windows.Forms;
using MAL_Reviewer_Core.helpers;

namespace MAL_Reviewer_UI.user_controls
{
    /// <summary>
    /// Path browser.
    /// </summary>
    public partial class PathBrowserControl : UserControl
    {
        #region Events

        /// <summary>
        /// The event of submitting the folder browser on the browse mode.
        /// </summary>
        public event EventHandler<string> BrowserBrowseSubmitEventHander;

        /// <summary>
        /// The event of submitting the folder browser on the open mode.
        /// </summary>
        public event EventHandler BrowserOpenSubmitEventHander;

        #endregion

        #region Fields

        /// <summary>
        /// The core of the control that handles the folder explorer.
        /// </summary>
        private FolderBrowserDialog FolderBrowser;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the browser's path.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string BrowserPath
        {
            get => FolderBrowser.SelectedPath;
            set
            {
                FolderBrowser.SelectedPath = value;
                PathTextbox.Text = value;
            }
        }

        /// <summary>
        /// Gets and sets the browser's button text.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string BrowserText
        {
            get => PathButton.Text;
            set => PathButton.Text = value;
        }

        /// <summary>
        /// Whether or not the path browser's button opens a folder dialog or
        /// the selected path in the file explorer.
        /// true means it will open the folder browser.
        /// false means it will open the file explorer.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BrowseOrOpen { get; set; } = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public PathBrowserControl()
        {
            InitializeComponent();

            // Instantiating the FolderBrowser field.
            FolderBrowser = new FolderBrowserDialog
            {
                Description = $"{ AppInfo.Title }'s storage folder"
            };
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the browse button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathButton_Click(object sender, EventArgs e)
        {
            // If it's set on browsing.
            if (BrowseOrOpen)
            {
                if (DialogResult.OK == this.FolderBrowser.ShowDialog())
                {
                    BrowserPath = FolderBrowser.SelectedPath;

                    // Invoking the submit event.
                    BrowserBrowseSubmitEventHander?.Invoke(this, BrowserPath);
                }
            }

            // If it's set on opening the filer explorer.
            else
            {
                // Invoking the submit event.
                BrowserOpenSubmitEventHander?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
