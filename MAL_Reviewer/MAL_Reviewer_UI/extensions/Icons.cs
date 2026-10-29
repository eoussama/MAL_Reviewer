using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MAL_Reviewer_Core.helpers;

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
        private static readonly Lazy<Image> link = new Lazy<Image>(() => Properties.Resources.icon_link);
        private static readonly Lazy<Image> loadingWhite = new Lazy<Image>(() => Properties.Resources.loading_gif_white);
        private static readonly Lazy<Image> loadingControlLight = new Lazy<Image>(() => Properties.Resources.loading_gif_control_light);
        private static readonly Lazy<Image> loadingControl = new Lazy<Image>(() => Properties.Resources.loading_gif_control);

        /// <summary>
        /// The loading animation drawn on <see cref="SystemColors.Control"/>, which is recolored for other backgrounds.
        /// </summary>
        private static readonly Lazy<byte[]> loadingGif = new Lazy<byte[]>(() =>
        {
            using (Stream stream = typeof(Icons).Assembly.GetManifestResourceStream("loading.gif"))
            using (MemoryStream copy = new MemoryStream())
            {
                stream.CopyTo(copy);
                return copy.ToArray();
            }
        });

        private static readonly Color loadingGifBackground = Color.FromArgb(0xF0, 0xF0, 0xF0);

        /// <summary>
        /// The recolored loading animations, by background color.
        /// </summary>
        private static readonly Dictionary<int, Image> recoloredLoading = new Dictionary<int, Image>();

        public static Image Anime => anime.Value;

        public static Image Manga => manga.Value;

        public static Image Close => close.Value;

        public static Image User => user.Value;

        public static Image Link => link.Value;

        public static Image LoadingWhite => loadingWhite.Value;

        public static Image LoadingControlLight => loadingControlLight.Value;

        public static Image LoadingControl => loadingControl.Value;

        /// <summary>
        /// The loading animation that fits a background color.
        /// </summary>
        /// <param name="background"> The color behind the animation. </param>
        public static Image Loading(Color background)
        {
            // The animations drawn for the light colors.
            switch (background.Name)
            {
                case "White":
                    return LoadingWhite;
                case "ControlLight":
                    return LoadingControlLight;
                case "Control":
                    return LoadingControl;
            }

            if (background.A < 255)
            {
                return LoadingControl;
            }

            if (!recoloredLoading.TryGetValue(background.ToArgb(), out Image image))
            {
                byte[] gif = GifPalette.Recolor(loadingGif.Value, pixel => ThemeMap.MoveToBackground(pixel, loadingGifBackground, background));

                // An animated image reads its frames from the stream as it plays, so the stream stays open.
                image = Image.FromStream(new MemoryStream(gif));
                recoloredLoading[background.ToArgb()] = image;
            }

            return image;
        }
    }
}
