namespace MAL_Reviewer_UI.forms.sub_forms
{
    partial class SettingsUserForm
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.DefaultUserLabel = new System.Windows.Forms.Label();
            this.DefaultUserPanel = new System.Windows.Forms.Panel();
            this.SetDefaultUserButton = new System.Windows.Forms.Button();
            this.linePanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.UserSettingsResetButton = new System.Windows.Forms.Button();
            this.DefaultUserTextboxControl = new MAL_Reviewer_UI.user_controls.TextboxControl();
            this.contentPanel.SuspendLayout();
            this.DefaultUserPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Controls.Add(this.DefaultUserLabel);
            this.contentPanel.Controls.Add(this.DefaultUserPanel);
            this.contentPanel.Location = new System.Drawing.Point(12, 94);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(512, 366);
            this.contentPanel.TabIndex = 26;
            // 
            // DefaultUserLabel
            // 
            this.DefaultUserLabel.AutoSize = true;
            this.DefaultUserLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultUserLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DefaultUserLabel.Location = new System.Drawing.Point(4, 2);
            this.DefaultUserLabel.Name = "DefaultUserLabel";
            this.DefaultUserLabel.Size = new System.Drawing.Size(98, 19);
            this.DefaultUserLabel.TabIndex = 22;
            this.DefaultUserLabel.Text = "Default user";
            // 
            // DefaultUserPanel
            // 
            this.DefaultUserPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultUserPanel.BackColor = System.Drawing.Color.Transparent;
            this.DefaultUserPanel.Controls.Add(this.DefaultUserTextboxControl);
            this.DefaultUserPanel.Controls.Add(this.SetDefaultUserButton);
            this.DefaultUserPanel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DefaultUserPanel.Location = new System.Drawing.Point(9, 26);
            this.DefaultUserPanel.Name = "DefaultUserPanel";
            this.DefaultUserPanel.Size = new System.Drawing.Size(494, 42);
            this.DefaultUserPanel.TabIndex = 20;
            // 
            // SetDefaultUserButton
            // 
            this.SetDefaultUserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SetDefaultUserButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SetDefaultUserButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SetDefaultUserButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SetDefaultUserButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SetDefaultUserButton.Location = new System.Drawing.Point(384, 3);
            this.SetDefaultUserButton.Name = "SetDefaultUserButton";
            this.SetDefaultUserButton.Size = new System.Drawing.Size(110, 36);
            this.SetDefaultUserButton.TabIndex = 19;
            this.SetDefaultUserButton.Text = "Set default user";
            this.SetDefaultUserButton.UseVisualStyleBackColor = false;
            this.SetDefaultUserButton.Click += new System.EventHandler(this.SetDefaultUserButton_Click);
            // 
            // linePanel
            // 
            this.linePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linePanel.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.linePanel.Location = new System.Drawing.Point(12, 51);
            this.linePanel.Name = "linePanel";
            this.linePanel.Size = new System.Drawing.Size(512, 5);
            this.linePanel.TabIndex = 25;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Bahnschrift", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.titleLabel.Location = new System.Drawing.Point(12, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(136, 25);
            this.titleLabel.TabIndex = 24;
            this.titleLabel.Text = "User settings";
            // 
            // UserSettingsResetButton
            // 
            this.UserSettingsResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UserSettingsResetButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.UserSettingsResetButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UserSettingsResetButton.FlatAppearance.BorderSize = 0;
            this.UserSettingsResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UserSettingsResetButton.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserSettingsResetButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.UserSettingsResetButton.Location = new System.Drawing.Point(432, 12);
            this.UserSettingsResetButton.Name = "UserSettingsResetButton";
            this.UserSettingsResetButton.Size = new System.Drawing.Size(92, 33);
            this.UserSettingsResetButton.TabIndex = 28;
            this.UserSettingsResetButton.Text = "Reset settings";
            this.UserSettingsResetButton.UseVisualStyleBackColor = false;
            this.UserSettingsResetButton.Click += new System.EventHandler(this.UserSettingsResetButton_Click);
            // 
            // DefaultUserTextboxControl
            // 
            this.DefaultUserTextboxControl.AllowLoad = false;
            this.DefaultUserTextboxControl.AutoSubmit = false;
            this.DefaultUserTextboxControl.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.DefaultUserTextboxControl.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.DefaultUserTextboxControl.Icon = global::MAL_Reviewer_UI.Properties.Resources.icon_user;
            this.DefaultUserTextboxControl.InnerText = "";
            this.DefaultUserTextboxControl.Location = new System.Drawing.Point(3, 3);
            this.DefaultUserTextboxControl.Name = "DefaultUserTextboxControl";
            this.DefaultUserTextboxControl.Placeholder = "Default MAL user...";
            this.DefaultUserTextboxControl.PlaceholderColor = System.Drawing.SystemColors.ControlDark;
            this.DefaultUserTextboxControl.Size = new System.Drawing.Size(375, 36);
            this.DefaultUserTextboxControl.SubmitMin = 3;
            this.DefaultUserTextboxControl.TabIndex = 23;
            this.DefaultUserTextboxControl.Tag = "0";
            this.DefaultUserTextboxControl.ToggleIcon = true;
            // 
            // SettingsUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(536, 473);
            this.Controls.Add(this.UserSettingsResetButton);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.linePanel);
            this.Controls.Add(this.titleLabel);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Info";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.DefaultUserPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel linePanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button UserSettingsResetButton;
        private System.Windows.Forms.Button SetDefaultUserButton;
        private System.Windows.Forms.Panel DefaultUserPanel;
        private System.Windows.Forms.Label DefaultUserLabel;
        private user_controls.TextboxControl DefaultUserTextboxControl;
    }
}