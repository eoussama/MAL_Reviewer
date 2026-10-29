namespace MAL_Reviewer_UI.forms.sub_forms
{
    partial class SettingsThemeForm
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
            this.ThemeLabel = new System.Windows.Forms.Label();
            this.SystemRadioButton = new System.Windows.Forms.RadioButton();
            this.LightRadioButton = new System.Windows.Forms.RadioButton();
            this.DarkRadioButton = new System.Windows.Forms.RadioButton();
            this.CustomRadioButton = new System.Windows.Forms.RadioButton();
            this.CustomColorsLabel = new System.Windows.Forms.Label();
            this.CustomColorsHintLabel = new System.Windows.Forms.Label();
            this.ColorsPanel = new System.Windows.Forms.Panel();
            this.StartFromLightLinkLabel = new System.Windows.Forms.LinkLabel();
            this.StartFromDarkLinkLabel = new System.Windows.Forms.LinkLabel();
            this.linePanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.ThemeSettingsResetButton = new System.Windows.Forms.Button();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // contentPanel
            //
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Controls.Add(this.ThemeLabel);
            this.contentPanel.Controls.Add(this.SystemRadioButton);
            this.contentPanel.Controls.Add(this.LightRadioButton);
            this.contentPanel.Controls.Add(this.DarkRadioButton);
            this.contentPanel.Controls.Add(this.CustomRadioButton);
            this.contentPanel.Controls.Add(this.CustomColorsLabel);
            this.contentPanel.Controls.Add(this.CustomColorsHintLabel);
            this.contentPanel.Controls.Add(this.ColorsPanel);
            this.contentPanel.Controls.Add(this.StartFromLightLinkLabel);
            this.contentPanel.Controls.Add(this.StartFromDarkLinkLabel);
            this.contentPanel.Location = new System.Drawing.Point(12, 94);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(512, 366);
            this.contentPanel.TabIndex = 26;
            //
            // ThemeLabel
            //
            this.ThemeLabel.AutoSize = true;
            this.ThemeLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThemeLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ThemeLabel.Location = new System.Drawing.Point(4, 2);
            this.ThemeLabel.Name = "ThemeLabel";
            this.ThemeLabel.Size = new System.Drawing.Size(56, 19);
            this.ThemeLabel.TabIndex = 0;
            this.ThemeLabel.Text = "Theme";
            //
            // SystemRadioButton
            //
            this.SystemRadioButton.AutoSize = true;
            this.SystemRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SystemRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SystemRadioButton.Location = new System.Drawing.Point(9, 28);
            this.SystemRadioButton.Name = "SystemRadioButton";
            this.SystemRadioButton.Size = new System.Drawing.Size(104, 17);
            this.SystemRadioButton.TabIndex = 1;
            this.SystemRadioButton.TabStop = true;
            this.SystemRadioButton.Text = "Same as Windows";
            this.SystemRadioButton.UseVisualStyleBackColor = true;
            this.SystemRadioButton.CheckedChanged += new System.EventHandler(this.ThemeRadioButton_CheckedChanged);
            //
            // LightRadioButton
            //
            this.LightRadioButton.AutoSize = true;
            this.LightRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LightRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.LightRadioButton.Location = new System.Drawing.Point(129, 28);
            this.LightRadioButton.Name = "LightRadioButton";
            this.LightRadioButton.Size = new System.Drawing.Size(48, 17);
            this.LightRadioButton.TabIndex = 2;
            this.LightRadioButton.Text = "Light";
            this.LightRadioButton.UseVisualStyleBackColor = true;
            this.LightRadioButton.CheckedChanged += new System.EventHandler(this.ThemeRadioButton_CheckedChanged);
            //
            // DarkRadioButton
            //
            this.DarkRadioButton.AutoSize = true;
            this.DarkRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DarkRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DarkRadioButton.Location = new System.Drawing.Point(194, 28);
            this.DarkRadioButton.Name = "DarkRadioButton";
            this.DarkRadioButton.Size = new System.Drawing.Size(48, 17);
            this.DarkRadioButton.TabIndex = 3;
            this.DarkRadioButton.Text = "Dark";
            this.DarkRadioButton.UseVisualStyleBackColor = true;
            this.DarkRadioButton.CheckedChanged += new System.EventHandler(this.ThemeRadioButton_CheckedChanged);
            //
            // CustomRadioButton
            //
            this.CustomRadioButton.AutoSize = true;
            this.CustomRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CustomRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CustomRadioButton.Location = new System.Drawing.Point(259, 28);
            this.CustomRadioButton.Name = "CustomRadioButton";
            this.CustomRadioButton.Size = new System.Drawing.Size(60, 17);
            this.CustomRadioButton.TabIndex = 4;
            this.CustomRadioButton.Text = "Custom";
            this.CustomRadioButton.UseVisualStyleBackColor = true;
            this.CustomRadioButton.CheckedChanged += new System.EventHandler(this.ThemeRadioButton_CheckedChanged);
            //
            // CustomColorsLabel
            //
            this.CustomColorsLabel.AutoSize = true;
            this.CustomColorsLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomColorsLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CustomColorsLabel.Location = new System.Drawing.Point(4, 62);
            this.CustomColorsLabel.Name = "CustomColorsLabel";
            this.CustomColorsLabel.Size = new System.Drawing.Size(111, 19);
            this.CustomColorsLabel.TabIndex = 4;
            this.CustomColorsLabel.Text = "Custom colors";
            //
            // CustomColorsHintLabel
            //
            this.CustomColorsHintLabel.AutoSize = true;
            this.CustomColorsHintLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CustomColorsHintLabel.Location = new System.Drawing.Point(6, 86);
            this.CustomColorsHintLabel.Name = "CustomColorsHintLabel";
            this.CustomColorsHintLabel.Size = new System.Drawing.Size(281, 13);
            this.CustomColorsHintLabel.TabIndex = 5;
            this.CustomColorsHintLabel.Text = "Click a color to change it, which switches to the custom theme.";
            //
            // ColorsPanel
            //
            this.ColorsPanel.BackColor = System.Drawing.Color.Transparent;
            this.ColorsPanel.Location = new System.Drawing.Point(9, 110);
            this.ColorsPanel.Name = "ColorsPanel";
            this.ColorsPanel.Size = new System.Drawing.Size(490, 160);
            this.ColorsPanel.TabIndex = 6;
            //
            // StartFromLightLinkLabel
            //
            this.StartFromLightLinkLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StartFromLightLinkLabel.AutoSize = true;
            this.StartFromLightLinkLabel.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartFromLightLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.StartFromLightLinkLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.StartFromLightLinkLabel.Location = new System.Drawing.Point(6, 280);
            this.StartFromLightLinkLabel.Name = "StartFromLightLinkLabel";
            this.StartFromLightLinkLabel.Size = new System.Drawing.Size(150, 13);
            this.StartFromLightLinkLabel.TabIndex = 7;
            this.StartFromLightLinkLabel.TabStop = true;
            this.StartFromLightLinkLabel.Text = "Start again from the light colors";
            this.StartFromLightLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StartFromLightLinkLabel_LinkClicked);
            //
            // StartFromDarkLinkLabel
            //
            this.StartFromDarkLinkLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StartFromDarkLinkLabel.AutoSize = true;
            this.StartFromDarkLinkLabel.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartFromDarkLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.StartFromDarkLinkLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.StartFromDarkLinkLabel.Location = new System.Drawing.Point(200, 280);
            this.StartFromDarkLinkLabel.Name = "StartFromDarkLinkLabel";
            this.StartFromDarkLinkLabel.Size = new System.Drawing.Size(147, 13);
            this.StartFromDarkLinkLabel.TabIndex = 8;
            this.StartFromDarkLinkLabel.TabStop = true;
            this.StartFromDarkLinkLabel.Text = "Start again from the dark colors";
            this.StartFromDarkLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StartFromDarkLinkLabel_LinkClicked);
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
            this.titleLabel.Size = new System.Drawing.Size(153, 25);
            this.titleLabel.TabIndex = 24;
            this.titleLabel.Text = "Theme settings";
            //
            // ThemeSettingsResetButton
            //
            this.ThemeSettingsResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ThemeSettingsResetButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ThemeSettingsResetButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ThemeSettingsResetButton.FlatAppearance.BorderSize = 0;
            this.ThemeSettingsResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ThemeSettingsResetButton.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThemeSettingsResetButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ThemeSettingsResetButton.Location = new System.Drawing.Point(432, 12);
            this.ThemeSettingsResetButton.Name = "ThemeSettingsResetButton";
            this.ThemeSettingsResetButton.Size = new System.Drawing.Size(92, 33);
            this.ThemeSettingsResetButton.TabIndex = 28;
            this.ThemeSettingsResetButton.Text = "Reset settings";
            this.ThemeSettingsResetButton.UseVisualStyleBackColor = false;
            this.ThemeSettingsResetButton.Click += new System.EventHandler(this.ThemeSettingsResetButton_Click);
            //
            // SettingsThemeForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(536, 473);
            this.Controls.Add(this.ThemeSettingsResetButton);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.linePanel);
            this.Controls.Add(this.titleLabel);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsThemeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Theme";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Label ThemeLabel;
        private System.Windows.Forms.RadioButton SystemRadioButton;
        private System.Windows.Forms.RadioButton LightRadioButton;
        private System.Windows.Forms.RadioButton DarkRadioButton;
        private System.Windows.Forms.RadioButton CustomRadioButton;
        private System.Windows.Forms.Label CustomColorsLabel;
        private System.Windows.Forms.Label CustomColorsHintLabel;
        private System.Windows.Forms.Panel ColorsPanel;
        private System.Windows.Forms.LinkLabel StartFromLightLinkLabel;
        private System.Windows.Forms.LinkLabel StartFromDarkLinkLabel;
        private System.Windows.Forms.Panel linePanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button ThemeSettingsResetButton;
    }
}
