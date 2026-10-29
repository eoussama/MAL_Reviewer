using System;
using System.Drawing;

namespace MAL_Reviewer_UI.extensions
{
    /// <summary>
    /// The application's icons, each loaded once.
    /// Every access to Properties.Resources creates a new bitmap, which leaks when done on every hover.
    /// </summary>
    public static class Icons
    {
        private static readonly Lazy<Image> anime = new Lazy<Image>(() => Properties.Resources.icon_anime);
        private static readonly Lazy<Image> manga = new Lazy<Image>(() => Properties.Resources.icon_manga);
        private static readonly Lazy<Image> close = new Lazy<Image>(() => Properties.Resources.icon_close);
        private static readonly Lazy<Image> user = new Lazy<Image>(() => Properties.Resources.icon_user);
        private static readonly Lazy<Image> loadingWhite = new Lazy<Image>(() => Properties.Resources.loading_gif_white);
        private static readonly Lazy<Image> loadingControlLight = new Lazy<Image>(() => Properties.Resources.loading_gif_control_light);
        private static readonly Lazy<Image> loadingControl = new Lazy<Image>(() => Properties.Resources.loading_gif_control);

        public static Image Anime => anime.Value;

        public static Image Manga => manga.Value;

        public static Image Close => close.Value;

        public static Image User => user.Value;

        public static Image LoadingWhite => loadingWhite.Value;

        public static Image LoadingControlLight => loadingControlLight.Value;

        public static Image LoadingControl => loadingControl.Value;
    }
}
