using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MAL_Reviewer_UI.theming
{
    /// <summary>
    /// The parts Windows draws itself: title bars and scroll bars.
    /// They are only touched once a dark theme was used, so the light theme keeps the system's look.
    /// </summary>
    internal static class NativeTheme
    {
        #region Fields

        private const int DarkTitleBarAttribute = 20;
        private const uint FrameChanged = 0x0020, NoSize = 0x0001, NoMove = 0x0002, NoZOrder = 0x0004, NoActivate = 0x0010;

        /// <summary>
        /// The windows made dark, which have to be made light again.
        /// </summary>
        private static readonly ConditionalWeakTable<Control, object> darkened = new ConditionalWeakTable<Control, object>();

        #endregion

        #region Public methods

        /// <summary>
        /// Whether Windows draws scroll bars for the control.
        /// </summary>
        /// <param name="control"> The control. </param>
        public static bool HasScrollBars(Control control)
        {
            return control is ScrollableControl || control is ListBox || control is RichTextBox || control is ScrollBar
                || (control is TextBox textBox && textBox.Multiline);
        }

        /// <summary>
        /// Makes the title bar of a window dark or light.
        /// </summary>
        /// <param name="form"> The window. </param>
        /// <param name="dark"> Whether to make it dark. </param>
        public static void SetTitleBar(Form form, bool dark)
        {
            if (!form.TopLevel || !NeedsChange(form, dark))
            {
                return;
            }

            int value = dark ? 1 : 0;
            DwmSetWindowAttribute(form.Handle, DarkTitleBarAttribute, ref value, sizeof(int));

            // Redrawing the title bar of a window already on screen.
            SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0, FrameChanged | NoSize | NoMove | NoZOrder | NoActivate);
        }

        /// <summary>
        /// Makes the scroll bars of a control dark or light.
        /// </summary>
        /// <param name="control"> The control. </param>
        /// <param name="dark"> Whether to make them dark. </param>
        public static void SetScrollBars(Control control, bool dark)
        {
            if (NeedsChange(control, dark))
            {
                SetWindowTheme(control.Handle, dark ? "DarkMode_Explorer" : null, null);
            }
        }

        #endregion

        #region Private methods

        private static bool NeedsChange(Control control, bool dark)
        {
            if (!control.IsHandleCreated || (!dark && !darkened.TryGetValue(control, out _)))
            {
                return false;
            }

            if (dark)
            {
                darkened.AddOrUpdate(control, null);
            }
            else
            {
                darkened.Remove(control);
            }

            return true;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string subAppName, string subIdList);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

        #endregion
    }
}
