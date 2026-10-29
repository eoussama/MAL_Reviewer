using System.Drawing;
using System.Windows.Forms;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.models.ThemeModels;
using MAL_Reviewer_UI.interfaces;

namespace MAL_Reviewer_UI.theming
{
    /// <summary>
    /// A tab control that draws itself in the theme's colors, since Windows draws tabs in its own light colors.
    /// With the light theme, Windows draws it as usual.
    /// </summary>
    public class ThemedTabControl : TabControl, IThemedControl
    {
        #region Fields

        /// <summary>
        /// The styles that let the control draw itself.
        /// </summary>
        private static readonly ControlStyles[] DrawingStyles = { ControlStyles.UserPaint, ControlStyles.AllPaintingInWmPaint, ControlStyles.OptimizedDoubleBuffer, ControlStyles.ResizeRedraw };

        /// <summary>
        /// Those styles as Windows Forms set them, which the light theme puts back.
        /// </summary>
        private ControlStyles systemStyles;

        private bool systemStylesSaved;

        #endregion

        #region Public methods

        public void ApplyTheme()
        {
            if (!this.systemStylesSaved)
            {
                foreach (ControlStyles style in DrawingStyles)
                {
                    this.systemStyles |= GetStyle(style) ? style : 0;
                }

                this.systemStylesSaved = true;
            }

            foreach (ControlStyles style in DrawingStyles)
            {
                SetStyle(style, Theme.Palette != null || this.systemStyles.HasFlag(style));
            }

            foreach (TabPage page in this.TabPages)
            {
                ApplyTheme(page);
            }

            Invalidate();
        }

        #endregion

        #region Protected methods

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is TabPage page)
            {
                ApplyTheme(page);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ThemePalette palette = Theme.Palette;

            if (palette == null)
            {
                base.OnPaint(e);
                return;
            }

            e.Graphics.Clear(this.Parent?.BackColor ?? palette[ThemeRole.Background]);

            // The frame around the pages.
            Rectangle pages = this.DisplayRectangle;
            pages.Inflate(2, 2);

            using (Brush background = new SolidBrush(palette[ThemeRole.Background]))
            using (Pen line = new Pen(palette[ThemeRole.Line]))
            {
                e.Graphics.FillRectangle(background, pages);
                e.Graphics.DrawRectangle(line, pages.X, pages.Y, pages.Width - 1, pages.Height - 1);

                for (int i = 0; i < this.TabCount; i++)
                {
                    Rectangle tab = GetTabRect(i);
                    bool selected = i == this.SelectedIndex;

                    // The selected tab opens onto its page.
                    if (selected)
                    {
                        tab.Height += 2;
                    }

                    using (Brush tabBackground = new SolidBrush(palette[selected ? ThemeRole.Background : ThemeRole.Header]))
                    {
                        e.Graphics.FillRectangle(tabBackground, tab);
                    }

                    e.Graphics.DrawLines(line, new[] { new Point(tab.Left, tab.Bottom - 1), new Point(tab.Left, tab.Top), new Point(tab.Right - 1, tab.Top), new Point(tab.Right - 1, tab.Bottom - 1) });

                    TextRenderer.DrawText(e.Graphics, this.TabPages[i].Text, this.Font, tab, palette[selected ? ThemeRole.Text : ThemeRole.SecondaryText],
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gives a page the theme's background, or the system's one with the light theme.
        /// </summary>
        private static void ApplyTheme(TabPage page)
        {
            ThemePalette palette = Theme.Palette;

            // Setting the color turns the system's background off, so it's turned back on after.
            if (palette == null)
            {
                page.ResetBackColor();
                page.UseVisualStyleBackColor = true;
            }
            else
            {
                page.BackColor = palette[ThemeRole.Background];
            }
        }

        #endregion
    }
}
