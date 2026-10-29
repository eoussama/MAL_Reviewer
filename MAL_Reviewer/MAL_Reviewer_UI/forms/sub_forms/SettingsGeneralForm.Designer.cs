namespace MAL_Reviewer_UI.forms.sub_forms
{
    partial class SettingsGeneralForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GeneralSettingsResetButton = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.StoragePathBrowserControl = new MAL_Reviewer_UI.user_controls.PathBrowserControl();
            this.StorageLabel = new System.Windows.Forms.Label();
            this.SettingsFileLabel = new System.Windows.Forms.Label();
            this.ExportSettingsButton = new System.Windows.Forms.Button();
            this.ImportSettingsButton = new System.Windows.Forms.Button();
            this.SettingsFileHintLabel = new System.Windows.Forms.Label();
            this.ListsLabel = new System.Windows.Forms.Label();
            this.ExportAnimeListButton = new System.Windows.Forms.Button();
            this.ExportMangaListButton = new System.Windows.Forms.Button();
            this.ListsHintLabel = new System.Windows.Forms.Label();
            this.linePanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneralSettingsResetButton
            // 
            this.GeneralSettingsResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralSettingsResetButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.GeneralSettingsResetButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GeneralSettingsResetButton.FlatAppearance.BorderSize = 0;
            this.GeneralSettingsResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GeneralSettingsResetButton.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GeneralSettingsResetButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.GeneralSettingsResetButton.Location = new System.Drawing.Point(432, 12);
            this.GeneralSettingsResetButton.Name = "GeneralSettingsResetButton";
            this.GeneralSettingsResetButton.Size = new System.Drawing.Size(92, 33);
            this.GeneralSettingsResetButton.TabIndex = 31;
            this.GeneralSettingsResetButton.Text = "Reset all";
            this.GeneralSettingsResetButton.UseVisualStyleBackColor = false;
            this.GeneralSettingsResetButton.Click += new System.EventHandler(this.GeneralSettingsResetButton_Click);
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Controls.Add(this.StoragePathBrowserControl);
            this.contentPanel.Controls.Add(this.StorageLabel);
            this.contentPanel.Controls.Add(this.SettingsFileLabel);
            this.contentPanel.Controls.Add(this.ExportSettingsButton);
            this.contentPanel.Controls.Add(this.ImportSettingsButton);
            this.contentPanel.Controls.Add(this.SettingsFileHintLabel);
            this.contentPanel.Controls.Add(this.ListsLabel);
            this.contentPanel.Controls.Add(this.ExportAnimeListButton);
            this.contentPanel.Controls.Add(this.ExportMangaListButton);
            this.contentPanel.Controls.Add(this.ListsHintLabel);
            this.contentPanel.Location = new System.Drawing.Point(12, 94);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(512, 366);
            this.contentPanel.TabIndex = 30;
            // 
            // StoragePathBrowserControl
            // 
            this.StoragePathBrowserControl.BackColor = System.Drawing.SystemColors.Control;
            this.StoragePathBrowserControl.BrowseOrOpen = false;
            this.StoragePathBrowserControl.BrowserPath = "";
            this.StoragePathBrowserControl.BrowserText = "Open";
            this.StoragePathBrowserControl.Location = new System.Drawing.Point(9, 26);
            this.StoragePathBrowserControl.MinimumSize = new System.Drawing.Size(275, 40);
            this.StoragePathBrowserControl.Name = "StoragePathBrowserControl";
            this.StoragePathBrowserControl.Size = new System.Drawing.Size(494, 40);
            this.StoragePathBrowserControl.TabIndex = 2;
            // 
            // StorageLabel
            // 
            this.StorageLabel.AutoSize = true;
            this.StorageLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StorageLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StorageLabel.Location = new System.Drawing.Point(4, 2);
            this.StorageLabel.Name = "StorageLabel";
            this.StorageLabel.Size = new System.Drawing.Size(102, 19);
            this.StorageLabel.TabIndex = 1;
            this.StorageLabel.Text = "Storage path";
            //
            // SettingsFileLabel
            //
            this.SettingsFileLabel.AutoSize = true;
            this.SettingsFileLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsFileLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SettingsFileLabel.Location = new System.Drawing.Point(4, 82);
            this.SettingsFileLabel.Name = "SettingsFileLabel";
            this.SettingsFileLabel.Size = new System.Drawing.Size(66, 19);
            this.SettingsFileLabel.TabIndex = 3;
            this.SettingsFileLabel.Text = "Settings";
            //
            // ExportSettingsButton
            //
            this.ExportSettingsButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExportSettingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportSettingsButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ExportSettingsButton.Location = new System.Drawing.Point(9, 108);
            this.ExportSettingsButton.Name = "ExportSettingsButton";
            this.ExportSettingsButton.Size = new System.Drawing.Size(130, 30);
            this.ExportSettingsButton.TabIndex = 4;
            this.ExportSettingsButton.Text = "Export settings...";
            this.ExportSettingsButton.UseVisualStyleBackColor = false;
            this.ExportSettingsButton.Click += new System.EventHandler(this.ExportSettingsButton_Click);
            //
            // ImportSettingsButton
            //
            this.ImportSettingsButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ImportSettingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ImportSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImportSettingsButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ImportSettingsButton.Location = new System.Drawing.Point(147, 108);
            this.ImportSettingsButton.Name = "ImportSettingsButton";
            this.ImportSettingsButton.Size = new System.Drawing.Size(130, 30);
            this.ImportSettingsButton.TabIndex = 5;
            this.ImportSettingsButton.Text = "Import settings...";
            this.ImportSettingsButton.UseVisualStyleBackColor = false;
            this.ImportSettingsButton.Click += new System.EventHandler(this.ImportSettingsButton_Click);
            //
            // SettingsFileHintLabel
            //
            this.SettingsFileHintLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SettingsFileHintLabel.Location = new System.Drawing.Point(6, 144);
            this.SettingsFileHintLabel.Name = "SettingsFileHintLabel";
            this.SettingsFileHintLabel.Size = new System.Drawing.Size(490, 28);
            this.SettingsFileHintLabel.TabIndex = 6;
            this.SettingsFileHintLabel.Text = "The default user, review templates, and the search and theme settings, in one file to keep or to move to another computer.";
            //
            // ListsLabel
            //
            this.ListsLabel.AutoSize = true;
            this.ListsLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListsLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ListsLabel.Location = new System.Drawing.Point(4, 184);
            this.ListsLabel.Name = "ListsLabel";
            this.ListsLabel.Size = new System.Drawing.Size(117, 19);
            this.ListsLabel.TabIndex = 7;
            this.ListsLabel.Text = "Anime and Manga lists";
            //
            // ExportAnimeListButton
            //
            this.ExportAnimeListButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExportAnimeListButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportAnimeListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportAnimeListButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ExportAnimeListButton.Location = new System.Drawing.Point(9, 210);
            this.ExportAnimeListButton.Name = "ExportAnimeListButton";
            this.ExportAnimeListButton.Size = new System.Drawing.Size(130, 30);
            this.ExportAnimeListButton.TabIndex = 8;
            this.ExportAnimeListButton.Text = "Export Anime list...";
            this.ExportAnimeListButton.UseVisualStyleBackColor = false;
            this.ExportAnimeListButton.Click += new System.EventHandler(this.ExportAnimeListButton_Click);
            //
            // ExportMangaListButton
            //
            this.ExportMangaListButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExportMangaListButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportMangaListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportMangaListButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ExportMangaListButton.Location = new System.Drawing.Point(147, 210);
            this.ExportMangaListButton.Name = "ExportMangaListButton";
            this.ExportMangaListButton.Size = new System.Drawing.Size(130, 30);
            this.ExportMangaListButton.TabIndex = 9;
            this.ExportMangaListButton.Text = "Export Manga list...";
            this.ExportMangaListButton.UseVisualStyleBackColor = false;
            this.ExportMangaListButton.Click += new System.EventHandler(this.ExportMangaListButton_Click);
            //
            // ListsHintLabel
            //
            this.ListsHintLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ListsHintLabel.Location = new System.Drawing.Point(6, 246);
            this.ListsHintLabel.Name = "ListsHintLabel";
            this.ListsHintLabel.Size = new System.Drawing.Size(490, 30);
            this.ListsHintLabel.TabIndex = 10;
            this.ListsHintLabel.Text = "Load a MAL account to export its lists.";
            //
            // linePanel
            // 
            this.linePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linePanel.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.linePanel.Location = new System.Drawing.Point(12, 51);
            this.linePanel.Name = "linePanel";
            this.linePanel.Size = new System.Drawing.Size(512, 5);
            this.linePanel.TabIndex = 29;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Bahnschrift", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.titleLabel.Location = new System.Drawing.Point(12, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(165, 25);
            this.titleLabel.TabIndex = 28;
            this.titleLabel.Text = "General settings";
            // 
            // SettingsGeneralForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(536, 473);
            this.Controls.Add(this.GeneralSettingsResetButton);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.linePanel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsGeneralForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General settings";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GeneralSettingsResetButton;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Label StorageLabel;
        private System.Windows.Forms.Panel linePanel;
        private System.Windows.Forms.Label titleLabel;
        private user_controls.PathBrowserControl StoragePathBrowserControl;
        private System.Windows.Forms.Label SettingsFileLabel;
        private System.Windows.Forms.Button ExportSettingsButton;
        private System.Windows.Forms.Button ImportSettingsButton;
        private System.Windows.Forms.Label SettingsFileHintLabel;
        private System.Windows.Forms.Label ListsLabel;
        private System.Windows.Forms.Button ExportAnimeListButton;
        private System.Windows.Forms.Button ExportMangaListButton;
        private System.Windows.Forms.Label ListsHintLabel;
    }
}