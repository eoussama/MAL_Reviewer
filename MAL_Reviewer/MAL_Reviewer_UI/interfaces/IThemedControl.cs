namespace MAL_Reviewer_UI.interfaces
{
    /// <summary>
    /// A control with parts the theme can't recolor by itself, (images, custom drawing).
    /// </summary>
    interface IThemedControl
    {
        /// <summary>
        /// Updates those parts, once the theme's colors were applied to the control and its children.
        /// </summary>
        void ApplyTheme();
    }
}
