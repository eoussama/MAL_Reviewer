using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.models.ThemeModels;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms.sub_forms
{
    public partial class SettingsThemeForm : Form, ISubForm
    {
        #region Fields

        /// <summary>
        /// What each color of a theme is used for, as shown next to it.
        /// </summary>
        private static readonly Dictionary<ThemeRole, string> RoleNames = new Dictionary<ThemeRole, string>
        {
            [ThemeRole.Background] = "Window background",
            [ThemeRole.Panel] = "Panels and cards",
            [ThemeRole.Highlight] = "Selected and hovered items",
            [ThemeRole.Header] = "Headers",
            [ThemeRole.Line] = "Lines and buttons",
            [ThemeRole.Text] = "Text",
            [ThemeRole.SecondaryText] = "Labels",
            [ThemeRole.FaintText] = "Hints and dates",
            [ThemeRole.Link] = "Links"
        };

        /// <summary>
        /// The button showing each custom color.
        /// </summary>
        private readonly Dictionary<ThemeRole, Button> swatches = new Dictionary<ThemeRole, Button>();

        /// <summary>
        /// Set while the controls are filled from the settings, so filling them isn't taken as a change.
        /// </summary>
        private bool displaying = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public SettingsThemeForm()
        {
            InitializeComponent();
            CreateSwatches();

            this.VisibleChanged += SettingsThemeForm_VisibleChanged;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the sub form's visibility change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsThemeForm_VisibleChanged(object sender, System.EventArgs e)
        {
            if (this.Visible)
            {
                InitDisplay();
            }
        }

        /// <summary>
        /// Creates a color button and its name for each custom color, in two columns.
        /// </summary>
        private void CreateSwatches()
        {
            const int rows = 5, columnWidth = 245, rowHeight = 32;

            for (int i = 0; i < ThemePalette.Roles.Count; i++)
            {
                ThemeRole role = ThemePalette.Roles[i];
                Point location = new Point(i / rows * columnWidth, i % rows * rowHeight);

                Button swatch = new Button
                {
                    Cursor = Cursors.Hand,
                    FlatStyle = FlatStyle.Flat,
                    Location = location,
                    Name = $"{ role }SwatchButton",
                    Size = new Size(36, 22),
                    Tag = role,
                    UseVisualStyleBackColor = false
                };

                swatch.FlatAppearance.BorderColor = SystemColors.ControlDark;
                swatch.Click += Swatch_Click;

                Label name = new Label
                {
                    AutoSize = true,
                    ForeColor = SystemColors.ActiveCaptionText,
                    Location = new Point(location.X + 44, location.Y + 4),
                    Name = $"{ role }SwatchLabel",
                    Text = RoleNames[role]
                };

                this.swatches[role] = swatch;
                this.ColorsPanel.Controls.Add(swatch);
                this.ColorsPanel.Controls.Add(name);
            }
        }

        private void ThemeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.displaying && ((RadioButton)sender).Checked)
            {
                Core.Settings.ThemeSettings.Theme = DarkRadioButton.Checked ? ThemeKind.Dark : CustomRadioButton.Checked ? ThemeKind.Custom : ThemeKind.Light;
                ShowTheme();
            }
        }

        private void Swatch_Click(object sender, EventArgs e)
        {
            Button swatch = (Button)sender;
            ThemeRole role = (ThemeRole)swatch.Tag;

            using (ColorDialog dialog = new ColorDialog { Color = swatch.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(this.TopLevelControl) == DialogResult.OK)
                {
                    ThemeController settings = Core.Settings.ThemeSettings;

                    settings.CustomColors[role] = dialog.Color;
                    settings.Theme = ThemeKind.Custom;
                    ShowTheme();
                    InitDisplay();
                }
            }
        }

        private void StartFromLightLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => StartFrom(ThemePalette.Light);

        private void StartFromDarkLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => StartFrom(ThemePalette.Dark);

        /// <summary>
        /// Replaces the custom colors with those of a theme, and switches to the custom theme.
        /// </summary>
        /// <param name="palette"> The colors to start from. </param>
        private void StartFrom(ThemePalette palette)
        {
            Core.Settings.ThemeSettings.CustomColors = palette;
            Core.Settings.ThemeSettings.Theme = ThemeKind.Custom;
            ShowTheme();
            InitDisplay();
        }

        private void ThemeSettingsResetButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to reset the theme settings?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                Core.Settings.ThemeSettings.ResetSettings();
                Core.Save();

                ShowTheme();
                InitDisplay();

                MessageBox.Show("Theme settings were successfully reset.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Shows every window in the chosen theme.
        /// </summary>
        private static void ShowTheme()
        {
            Theme.Use(Core.Settings.ThemeSettings.ActivePalette);
        }

        #endregion

        #region Public methods

        // Displays initial information on the sub form.
        public void InitDisplay()
        {
            ThemeController settings = Core.Settings.ThemeSettings;

            this.displaying = true;

            LightRadioButton.Checked = settings.Theme == ThemeKind.Light;
            DarkRadioButton.Checked = settings.Theme == ThemeKind.Dark;
            CustomRadioButton.Checked = settings.Theme == ThemeKind.Custom;

            foreach (KeyValuePair<ThemeRole, Button> swatch in this.swatches)
            {
                Color color = settings.CustomColors[swatch.Key];

                swatch.Value.BackColor = color;
                swatch.Value.FlatAppearance.MouseOverBackColor = color;
                swatch.Value.FlatAppearance.MouseDownBackColor = color;
            }

            this.displaying = false;
        }

        #endregion
    }
}
