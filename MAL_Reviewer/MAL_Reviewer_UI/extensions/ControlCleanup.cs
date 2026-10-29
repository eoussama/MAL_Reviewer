using System.Linq;
using System.Windows.Forms;

namespace MAL_Reviewer_UI.extensions
{
    /// <summary>
    /// Removing controls the way WinForms needs it.
    /// </summary>
    public static class ControlCleanup
    {
        /// <summary>
        /// Removes and disposes all the controls of a collection.
        /// Controls.Clear() alone keeps their window handles and images alive.
        /// </summary>
        /// <param name="controls"> The collection to empty. </param>
        public static void ClearAndDispose(this Control.ControlCollection controls)
        {
            Control[] removed = controls.Cast<Control>().ToArray();

            controls.Clear();

            foreach (Control control in removed)
            {
                control.Dispose();
            }
        }
    }
}
