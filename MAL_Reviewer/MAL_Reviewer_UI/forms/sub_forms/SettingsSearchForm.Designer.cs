namespace MAL_Reviewer_UI.forms.sub_forms
{
    partial class SettingsSearchForm
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
            this.SearchWhileTypingLabel = new System.Windows.Forms.Label();
            this.SearchWhileTypingCheckBox = new System.Windows.Forms.CheckBox();
            this.DelayLabel = new System.Windows.Forms.Label();
            this.DelayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DelayUnitLabel = new System.Windows.Forms.Label();
            this.ResultsLabel = new System.Windows.Forms.Label();
            this.ResultsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ResultsUnitLabel = new System.Windows.Forms.Label();
            this.DefaultModeLabel = new System.Windows.Forms.Label();
            this.TitleModeRadioButton = new System.Windows.Forms.RadioButton();
            this.LinkModeRadioButton = new System.Windows.Forms.RadioButton();
            this.linePanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.SearchSettingsResetButton = new System.Windows.Forms.Button();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DelayNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsNumericUpDown)).BeginInit();
            this.SuspendLayout();
            //
            // contentPanel
            //
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Controls.Add(this.SearchWhileTypingLabel);
            this.contentPanel.Controls.Add(this.SearchWhileTypingCheckBox);
            this.contentPanel.Controls.Add(this.DelayLabel);
            this.contentPanel.Controls.Add(this.DelayNumericUpDown);
            this.contentPanel.Controls.Add(this.DelayUnitLabel);
            this.contentPanel.Controls.Add(this.ResultsLabel);
            this.contentPanel.Controls.Add(this.ResultsNumericUpDown);
            this.contentPanel.Controls.Add(this.ResultsUnitLabel);
            this.contentPanel.Controls.Add(this.DefaultModeLabel);
            this.contentPanel.Controls.Add(this.TitleModeRadioButton);
            this.contentPanel.Controls.Add(this.LinkModeRadioButton);
            this.contentPanel.Location = new System.Drawing.Point(12, 94);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(512, 366);
            this.contentPanel.TabIndex = 26;
            //
            // SearchWhileTypingLabel
            //
            this.SearchWhileTypingLabel.AutoSize = true;
            this.SearchWhileTypingLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchWhileTypingLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SearchWhileTypingLabel.Location = new System.Drawing.Point(4, 2);
            this.SearchWhileTypingLabel.Name = "SearchWhileTypingLabel";
            this.SearchWhileTypingLabel.Size = new System.Drawing.Size(150, 19);
            this.SearchWhileTypingLabel.TabIndex = 0;
            this.SearchWhileTypingLabel.Text = "Search while typing";
            //
            // SearchWhileTypingCheckBox
            //
            this.SearchWhileTypingCheckBox.AutoSize = true;
            this.SearchWhileTypingCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchWhileTypingCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SearchWhileTypingCheckBox.Location = new System.Drawing.Point(9, 28);
            this.SearchWhileTypingCheckBox.Name = "SearchWhileTypingCheckBox";
            this.SearchWhileTypingCheckBox.Size = new System.Drawing.Size(330, 17);
            this.SearchWhileTypingCheckBox.TabIndex = 1;
            this.SearchWhileTypingCheckBox.Text = "Search as soon as I stop typing (otherwise, press Enter to search)";
            this.SearchWhileTypingCheckBox.UseVisualStyleBackColor = true;
            this.SearchWhileTypingCheckBox.CheckedChanged += new System.EventHandler(this.SearchWhileTypingCheckBox_CheckedChanged);
            //
            // DelayLabel
            //
            this.DelayLabel.AutoSize = true;
            this.DelayLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DelayLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DelayLabel.Location = new System.Drawing.Point(4, 62);
            this.DelayLabel.Name = "DelayLabel";
            this.DelayLabel.Size = new System.Drawing.Size(98, 19);
            this.DelayLabel.TabIndex = 2;
            this.DelayLabel.Text = "Typing delay";
            //
            // DelayNumericUpDown
            //
            this.DelayNumericUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.DelayNumericUpDown.Location = new System.Drawing.Point(9, 88);
            this.DelayNumericUpDown.Name = "DelayNumericUpDown";
            this.DelayNumericUpDown.Size = new System.Drawing.Size(80, 20);
            this.DelayNumericUpDown.TabIndex = 3;
            this.DelayNumericUpDown.ValueChanged += new System.EventHandler(this.DelayNumericUpDown_ValueChanged);
            //
            // DelayUnitLabel
            //
            this.DelayUnitLabel.AutoSize = true;
            this.DelayUnitLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DelayUnitLabel.Location = new System.Drawing.Point(95, 90);
            this.DelayUnitLabel.Name = "DelayUnitLabel";
            this.DelayUnitLabel.Size = new System.Drawing.Size(240, 13);
            this.DelayUnitLabel.TabIndex = 4;
            this.DelayUnitLabel.Text = "milliseconds after the last key press";
            //
            // ResultsLabel
            //
            this.ResultsLabel.AutoSize = true;
            this.ResultsLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultsLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ResultsLabel.Location = new System.Drawing.Point(4, 122);
            this.ResultsLabel.Name = "ResultsLabel";
            this.ResultsLabel.Size = new System.Drawing.Size(140, 19);
            this.ResultsLabel.TabIndex = 5;
            this.ResultsLabel.Text = "Number of results";
            //
            // ResultsNumericUpDown
            //
            this.ResultsNumericUpDown.Location = new System.Drawing.Point(9, 148);
            this.ResultsNumericUpDown.Name = "ResultsNumericUpDown";
            this.ResultsNumericUpDown.Size = new System.Drawing.Size(80, 20);
            this.ResultsNumericUpDown.TabIndex = 6;
            this.ResultsNumericUpDown.ValueChanged += new System.EventHandler(this.ResultsNumericUpDown_ValueChanged);
            //
            // ResultsUnitLabel
            //
            this.ResultsUnitLabel.AutoSize = true;
            this.ResultsUnitLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ResultsUnitLabel.Location = new System.Drawing.Point(95, 150);
            this.ResultsUnitLabel.Name = "ResultsUnitLabel";
            this.ResultsUnitLabel.Size = new System.Drawing.Size(240, 13);
            this.ResultsUnitLabel.TabIndex = 7;
            this.ResultsUnitLabel.Text = "results per search";
            //
            // DefaultModeLabel
            //
            this.DefaultModeLabel.AutoSize = true;
            this.DefaultModeLabel.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultModeLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DefaultModeLabel.Location = new System.Drawing.Point(4, 182);
            this.DefaultModeLabel.Name = "DefaultModeLabel";
            this.DefaultModeLabel.Size = new System.Drawing.Size(120, 19);
            this.DefaultModeLabel.TabIndex = 8;
            this.DefaultModeLabel.Text = "Search first by";
            //
            // TitleModeRadioButton
            //
            this.TitleModeRadioButton.AutoSize = true;
            this.TitleModeRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TitleModeRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TitleModeRadioButton.Location = new System.Drawing.Point(9, 208);
            this.TitleModeRadioButton.Name = "TitleModeRadioButton";
            this.TitleModeRadioButton.Size = new System.Drawing.Size(45, 17);
            this.TitleModeRadioButton.TabIndex = 9;
            this.TitleModeRadioButton.TabStop = true;
            this.TitleModeRadioButton.Text = "Title";
            this.TitleModeRadioButton.UseVisualStyleBackColor = true;
            this.TitleModeRadioButton.CheckedChanged += new System.EventHandler(this.ModeRadioButton_CheckedChanged);
            //
            // LinkModeRadioButton
            //
            this.LinkModeRadioButton.AutoSize = true;
            this.LinkModeRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LinkModeRadioButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.LinkModeRadioButton.Location = new System.Drawing.Point(80, 208);
            this.LinkModeRadioButton.Name = "LinkModeRadioButton";
            this.LinkModeRadioButton.Size = new System.Drawing.Size(117, 17);
            this.LinkModeRadioButton.TabIndex = 10;
            this.LinkModeRadioButton.TabStop = true;
            this.LinkModeRadioButton.Text = "MyAnimeList link";
            this.LinkModeRadioButton.UseVisualStyleBackColor = true;
            this.LinkModeRadioButton.CheckedChanged += new System.EventHandler(this.ModeRadioButton_CheckedChanged);
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
            this.titleLabel.Size = new System.Drawing.Size(157, 25);
            this.titleLabel.TabIndex = 24;
            this.titleLabel.Text = "Search settings";
            //
            // SearchSettingsResetButton
            //
            this.SearchSettingsResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchSettingsResetButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.SearchSettingsResetButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchSettingsResetButton.FlatAppearance.BorderSize = 0;
            this.SearchSettingsResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchSettingsResetButton.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchSettingsResetButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SearchSettingsResetButton.Location = new System.Drawing.Point(432, 12);
            this.SearchSettingsResetButton.Name = "SearchSettingsResetButton";
            this.SearchSettingsResetButton.Size = new System.Drawing.Size(92, 33);
            this.SearchSettingsResetButton.TabIndex = 27;
            this.SearchSettingsResetButton.Text = "Reset settings";
            this.SearchSettingsResetButton.UseVisualStyleBackColor = false;
            this.SearchSettingsResetButton.Click += new System.EventHandler(this.SearchSettingsResetButton_Click);
            //
            // SettingsSearchForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(536, 473);
            this.Controls.Add(this.SearchSettingsResetButton);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.linePanel);
            this.Controls.Add(this.titleLabel);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsSearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Info";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DelayNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel linePanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button SearchSettingsResetButton;
        private System.Windows.Forms.Label SearchWhileTypingLabel;
        private System.Windows.Forms.CheckBox SearchWhileTypingCheckBox;
        private System.Windows.Forms.Label DelayLabel;
        private System.Windows.Forms.NumericUpDown DelayNumericUpDown;
        private System.Windows.Forms.Label DelayUnitLabel;
        private System.Windows.Forms.Label ResultsLabel;
        private System.Windows.Forms.NumericUpDown ResultsNumericUpDown;
        private System.Windows.Forms.Label ResultsUnitLabel;
        private System.Windows.Forms.Label DefaultModeLabel;
        private System.Windows.Forms.RadioButton TitleModeRadioButton;
        private System.Windows.Forms.RadioButton LinkModeRadioButton;
    }
}
