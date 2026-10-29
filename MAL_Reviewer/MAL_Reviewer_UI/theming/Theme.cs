using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ThemeModels;
using MAL_Reviewer_UI.interfaces;

namespace MAL_Reviewer_UI.theming
{
    /// <summary>
    /// Applies the theme to the windows.
    /// The forms keep the light colors they were designed with; each designed color is replaced by the theme's color of the same role,
    /// (see <see cref="ThemeMap"/>), and put back when the light theme is chosen again.
    /// </summary>
    public static class Theme
    {
        #region Fields

        /// <summary>
        /// A color the theme manages: the designed one, and the one last applied in its place.
        /// </summary>
        private class ThemedColor
        {
            public Color Designed;
            public Color Applied;
        }

        /// <summary>
        /// The colors managed for each object, (controls, button appearances, grid cell styles), by property name.
        /// </summary>
        private static readonly ConditionalWeakTable<object, Dictionary<string, ThemedColor>> colors = new ConditionalWeakTable<object, Dictionary<string, ThemedColor>>();

        /// <summary>
        /// The controls whose new children are themed as they are added.
        /// </summary>
        private static readonly ConditionalWeakTable<Control, object> watched = new ConditionalWeakTable<Control, object>();

        /// <summary>
        /// The themed windows, which follow the theme's changes.
        /// </summary>
        private static readonly List<WeakReference<Form>> forms = new List<WeakReference<Form>>();

        /// <summary>
        /// Whether a grid used the system's look for its headers before being themed.
        /// </summary>
        private static readonly ConditionalWeakTable<DataGridView, object> systemHeaders = new ConditionalWeakTable<DataGridView, object>();

        #endregion

        #region Properties

        /// <summary>
        /// The colors of the theme, or null for the light theme, (the designed colors).
        /// </summary>
        public static ThemePalette Palette { get; private set; }

        /// <summary>
        /// Whether the theme is dark.
        /// </summary>
        public static bool IsDark => Palette?.IsDark == true;

        #endregion

        #region Public methods

        /// <summary>
        /// Switches to a theme, and applies it to every themed window.
        /// </summary>
        /// <param name="palette"> The theme's colors, or null for the light theme. </param>
        public static void Use(ThemePalette palette)
        {
            Palette = palette == null ? null : new ThemePalette(palette);

            forms.RemoveAll(reference => !reference.TryGetTarget(out Form form) || form.IsDisposed);
            forms.ForEach(reference =>
            {
                if (reference.TryGetTarget(out Form form))
                {
                    Apply(form);
                }
            });
        }

        /// <summary>
        /// Themes a window, along with the controls added to it later, and keeps it following the theme.
        /// </summary>
        /// <param name="form"> The window. </param>
        public static void Attach(Form form)
        {
            forms.Add(new WeakReference<Form>(form));
            Watch(form);
            Apply(form);
        }

        /// <summary>
        /// The theme's version of a designed background color.
        /// </summary>
        /// <param name="designed"> The designed color. </param>
        public static Color Back(Color designed) => ThemeMap.Back(designed, Palette);

        /// <summary>
        /// The theme's version of a designed text color.
        /// </summary>
        /// <param name="designed"> The designed color. </param>
        public static Color Fore(Color designed) => ThemeMap.Fore(designed, Palette);

        /// <summary>
        /// Sets a designed background color, (on hover for instance), which shows in the theme's version.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="designed"> The designed color. </param>
        public static void SetThemedBackColor(this Control control, Color designed)
        {
            Set(control, nameof(Control.BackColor), designed, Back, color => control.BackColor = color, designed: true);
        }

        /// <summary>
        /// Sets a designed text color, (on hover for instance), which shows in the theme's version.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="designed"> The designed color. </param>
        public static void SetThemedForeColor(this Control control, Color designed)
        {
            Set(control, nameof(Control.ForeColor), designed, Fore, color => control.ForeColor = color, designed: true);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Themes the controls added to a control and its children from now on.
        /// </summary>
        private static void Watch(Control control)
        {
            if (watched.TryGetValue(control, out _))
            {
                return;
            }

            watched.Add(control, null);

            control.ControlAdded += (sender, e) =>
            {
                Watch(e.Control);
                Apply(e.Control);
            };

            // The parts drawn by Windows can only change once the control has a window.
            if (control is Form form)
            {
                form.HandleCreated += (sender, e) => NativeTheme.SetTitleBar(form, IsDark);
            }

            if (NativeTheme.HasScrollBars(control))
            {
                control.HandleCreated += (sender, e) => NativeTheme.SetScrollBars(control, IsDark);
            }

            foreach (Control child in control.Controls)
            {
                Watch(child);
            }
        }

        /// <summary>
        /// Applies the theme to a control and its children.
        /// </summary>
        private static void Apply(Control control)
        {
            ApplyColors(control);

            foreach (Control child in control.Controls)
            {
                Apply(child);
            }

            if (control is IThemedControl themed)
            {
                themed.ApplyTheme();
            }
        }

        private static void ApplyColors(Control control)
        {
            // Colors inherited from the parent follow it by themselves.
            if (control.Parent == null || control.BackColor != control.Parent.BackColor || IsManaged(control, nameof(Control.BackColor)))
            {
                Set(control, nameof(Control.BackColor), control.BackColor, Back, color => control.BackColor = color);
            }

            if (control.Parent == null || control.ForeColor != control.Parent.ForeColor || IsManaged(control, nameof(Control.ForeColor)))
            {
                Set(control, nameof(Control.ForeColor), control.ForeColor, Fore, color => control.ForeColor = color);
            }

            switch (control)
            {
                case ButtonBase button:
                    FlatButtonAppearance look = button.FlatAppearance;
                    Set(look, nameof(look.MouseOverBackColor), look.MouseOverBackColor, Back, color => look.MouseOverBackColor = color);
                    Set(look, nameof(look.MouseDownBackColor), look.MouseDownBackColor, Back, color => look.MouseDownBackColor = color);
                    Set(look, nameof(look.BorderColor), look.BorderColor, Fore, color => look.BorderColor = color);
                    break;

                case LinkLabel link:
                    Set(link, nameof(link.LinkColor), link.LinkColor, Fore, color => link.LinkColor = color);
                    Set(link, nameof(link.ActiveLinkColor), link.ActiveLinkColor, Fore, color => link.ActiveLinkColor = color);
                    Set(link, nameof(link.VisitedLinkColor), link.VisitedLinkColor, Fore, color => link.VisitedLinkColor = color);
                    break;

                case DataGridView grid:
                    ApplyGridColors(grid);
                    break;
            }

            if (control is Form form)
            {
                NativeTheme.SetTitleBar(form, IsDark);
            }

            if (NativeTheme.HasScrollBars(control))
            {
                NativeTheme.SetScrollBars(control, IsDark);
            }
        }

        private static void ApplyGridColors(DataGridView grid)
        {
            Set(grid, nameof(grid.BackgroundColor), grid.BackgroundColor, Back, color => grid.BackgroundColor = color);
            Set(grid, nameof(grid.GridColor), grid.GridColor, Back, color => grid.GridColor = color);

            foreach (DataGridViewCellStyle style in new[] { grid.DefaultCellStyle, grid.ColumnHeadersDefaultCellStyle, grid.RowsDefaultCellStyle, grid.AlternatingRowsDefaultCellStyle })
            {
                Set(style, nameof(style.BackColor), style.BackColor, Back, color => style.BackColor = color);
                Set(style, nameof(style.SelectionBackColor), style.SelectionBackColor, Back, color => style.SelectionBackColor = color);
                Set(style, nameof(style.ForeColor), style.ForeColor, Fore, color => style.ForeColor = color);
                Set(style, nameof(style.SelectionForeColor), style.SelectionForeColor, Fore, color => style.SelectionForeColor = color);
            }

            // The system's look of the headers ignores their colors.
            if (grid.EnableHeadersVisualStyles)
            {
                systemHeaders.AddOrUpdate(grid, null);
            }

            grid.EnableHeadersVisualStyles = Palette == null && systemHeaders.TryGetValue(grid, out _);
        }

        /// <summary>
        /// Whether the theme already manages a color of an object.
        /// </summary>
        private static bool IsManaged(object owner, string property)
        {
            return colors.TryGetValue(owner, out Dictionary<string, ThemedColor> owned) && owned.ContainsKey(property);
        }

        /// <summary>
        /// Shows the theme's version of a designed color.
        /// A color changed since it was last themed is taken as the new designed color.
        /// </summary>
        /// <param name="owner"> The object with the color. </param>
        /// <param name="property"> The name of the color's property. </param>
        /// <param name="current"> The color shown now, or the newly designed one. </param>
        /// <param name="translate"> Gives the theme's version of a designed color. </param>
        /// <param name="apply"> Shows a color. </param>
        /// <param name="designed"> Whether the passed color is a newly designed one. </param>
        private static void Set(object owner, string property, Color current, Func<Color, Color> translate, Action<Color> apply, bool designed = false)
        {
            Dictionary<string, ThemedColor> owned = colors.GetOrCreateValue(owner);

            if (designed || !owned.TryGetValue(property, out ThemedColor color) || color.Applied != current)
            {
                color = new ThemedColor { Designed = current };
                owned[property] = color;
            }

            Color shown = color.Applied = translate(color.Designed);

            if (designed || shown != current)
            {
                apply(shown);
            }
        }

        #endregion
    }
}
