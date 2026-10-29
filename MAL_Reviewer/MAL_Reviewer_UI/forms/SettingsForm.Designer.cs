namespace MAL_Reviewer_UI.forms
{
    partial class SettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.SidePanel = new System.Windows.Forms.Panel();
            this.GeneralButton = new System.Windows.Forms.Button();
            this.sizeControlImageList = new System.Windows.Forms.ImageList(this.components);
            this.InfoButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.TemplateButton = new System.Windows.Forms.Button();
            this.ThemeButton = new System.Windows.Forms.Button();
            this.UserButton = new System.Windows.Forms.Button();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.extraInfo = new System.Windows.Forms.ToolTip(this.components);
            this.SidePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SidePanel
            // 
            this.SidePanel.BackColor = System.Drawing.SystemColors.Control;
            this.SidePanel.Controls.Add(this.GeneralButton);
            this.SidePanel.Controls.Add(this.InfoButton);
            this.SidePanel.Controls.Add(this.SearchButton);
            this.SidePanel.Controls.Add(this.TemplateButton);
            this.SidePanel.Controls.Add(this.ThemeButton);
            this.SidePanel.Controls.Add(this.UserButton);
            this.SidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SidePanel.Location = new System.Drawing.Point(0, 0);
            this.SidePanel.Name = "SidePanel";
            this.SidePanel.Size = new System.Drawing.Size(75, 473);
            this.SidePanel.TabIndex = 0;
            // 
            // GeneralButton
            // 
            this.GeneralButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralButton.BackColor = System.Drawing.Color.Transparent;
            this.GeneralButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GeneralButton.FlatAppearance.BorderSize = 0;
            this.GeneralButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.GeneralButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.GeneralButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GeneralButton.ImageIndex = 5;
            this.GeneralButton.ImageList = this.sizeControlImageList;
            this.GeneralButton.Location = new System.Drawing.Point(3, 4);
            this.GeneralButton.Name = "GeneralButton";
            this.GeneralButton.Size = new System.Drawing.Size(69, 67);
            this.GeneralButton.TabIndex = 9;
            this.extraInfo.SetToolTip(this.GeneralButton, "General settings");
            this.GeneralButton.UseVisualStyleBackColor = false;
            this.GeneralButton.Click += new System.EventHandler(this.GeneralButton_Click);
            // 
            // sizeControlImageList
            // 
            this.sizeControlImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sizeControlImageList.ImageStream")));
            this.sizeControlImageList.TransparentColor = System.Drawing.SystemColors.ControlLight;
            this.sizeControlImageList.Images.SetKeyName(0, "icon-user.png");
            this.sizeControlImageList.Images.SetKeyName(1, "icon_search.png");
            this.sizeControlImageList.Images.SetKeyName(2, "icon_info.png");
            this.sizeControlImageList.Images.SetKeyName(3, "icon_template.png");
            this.sizeControlImageList.Images.SetKeyName(4, "icon_theme.png");
            this.sizeControlImageList.Images.SetKeyName(5, "icon_settings.png");
            // 
            // InfoButton
            // 
            this.InfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoButton.BackColor = System.Drawing.Color.Transparent;
            this.InfoButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InfoButton.FlatAppearance.BorderSize = 0;
            this.InfoButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.InfoButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.InfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InfoButton.ImageIndex = 2;
            this.InfoButton.ImageList = this.sizeControlImageList;
            this.InfoButton.Location = new System.Drawing.Point(3, 354);
            this.InfoButton.Name = "InfoButton";
            this.InfoButton.Size = new System.Drawing.Size(69, 67);
            this.InfoButton.TabIndex = 8;
            this.extraInfo.SetToolTip(this.InfoButton, "About");
            this.InfoButton.UseVisualStyleBackColor = false;
            this.InfoButton.Click += new System.EventHandler(this.InfoButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchButton.BackColor = System.Drawing.Color.Transparent;
            this.SearchButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchButton.FlatAppearance.BorderSize = 0;
            this.SearchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.SearchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.SearchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchButton.ImageIndex = 1;
            this.SearchButton.ImageList = this.sizeControlImageList;
            this.SearchButton.Location = new System.Drawing.Point(3, 284);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(69, 67);
            this.SearchButton.TabIndex = 7;
            this.extraInfo.SetToolTip(this.SearchButton, "Search settings");
            this.SearchButton.UseVisualStyleBackColor = false;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // TemplateButton
            // 
            this.TemplateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TemplateButton.BackColor = System.Drawing.Color.Transparent;
            this.TemplateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TemplateButton.FlatAppearance.BorderSize = 0;
            this.TemplateButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.TemplateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.TemplateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TemplateButton.ImageIndex = 3;
            this.TemplateButton.ImageList = this.sizeControlImageList;
            this.TemplateButton.Location = new System.Drawing.Point(3, 214);
            this.TemplateButton.Name = "TemplateButton";
            this.TemplateButton.Size = new System.Drawing.Size(69, 67);
            this.TemplateButton.TabIndex = 6;
            this.extraInfo.SetToolTip(this.TemplateButton, "Review templates settings");
            this.TemplateButton.UseVisualStyleBackColor = false;
            this.TemplateButton.Click += new System.EventHandler(this.TemplateButton_Click);
            // 
            // ThemeButton
            // 
            this.ThemeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ThemeButton.BackColor = System.Drawing.Color.Transparent;
            this.ThemeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ThemeButton.FlatAppearance.BorderSize = 0;
            this.ThemeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.ThemeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.ThemeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ThemeButton.ImageIndex = 4;
            this.ThemeButton.ImageList = this.sizeControlImageList;
            this.ThemeButton.Location = new System.Drawing.Point(3, 144);
            this.ThemeButton.Name = "ThemeButton";
            this.ThemeButton.Size = new System.Drawing.Size(69, 67);
            this.ThemeButton.TabIndex = 5;
            this.extraInfo.SetToolTip(this.ThemeButton, "Theme settings");
            this.ThemeButton.UseVisualStyleBackColor = false;
            this.ThemeButton.Click += new System.EventHandler(this.ThemeButton_Click);
            // 
            // UserButton
            // 
            this.UserButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UserButton.BackColor = System.Drawing.Color.Transparent;
            this.UserButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UserButton.FlatAppearance.BorderSize = 0;
            this.UserButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.UserButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.UserButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UserButton.ImageIndex = 0;
            this.UserButton.ImageList = this.sizeControlImageList;
            this.UserButton.Location = new System.Drawing.Point(3, 74);
            this.UserButton.Name = "UserButton";
            this.UserButton.Size = new System.Drawing.Size(69, 67);
            this.UserButton.TabIndex = 4;
            this.extraInfo.SetToolTip(this.UserButton, "User settings");
            this.UserButton.UseVisualStyleBackColor = false;
            this.UserButton.Click += new System.EventHandler(this.UserButton_Click);
            // 
            // ContentPanel
            // 
            this.ContentPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(75, 0);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Padding = new System.Windows.Forms.Padding(30, 0, 30, 0);
            this.ContentPanel.Size = new System.Drawing.Size(600, 473);
            this.ContentPanel.TabIndex = 1;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 473);
            this.Controls.Add(this.ContentPanel);
            this.Controls.Add(this.SidePanel);
            this.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.SidePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SidePanel;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button TemplateButton;
        private System.Windows.Forms.Button ThemeButton;
        private System.Windows.Forms.Button UserButton;
        private System.Windows.Forms.ImageList sizeControlImageList;
        private System.Windows.Forms.Button InfoButton;
        private System.Windows.Forms.Panel ContentPanel;
        private System.Windows.Forms.ToolTip extraInfo;
        private System.Windows.Forms.Button GeneralButton;
    }
}