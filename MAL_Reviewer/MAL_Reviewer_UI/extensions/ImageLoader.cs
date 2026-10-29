using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAL_Reviewer_UI.extensions
{
    /// <summary>
    /// Downloads remote images without blocking the interface, keeping each download in memory
    /// so the same cover is fetched once per session.
    /// </summary>
    public static class ImageLoader
    {
        #region Fields

        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

        private static readonly ConcurrentDictionary<string, Task<byte[]>> Cache = new ConcurrentDictionary<string, Task<byte[]>>();

        /// <summary>
        /// The images created by the loader, which it may dispose when replacing them.
        /// Shared resource images are never in here.
        /// </summary>
        private static readonly ConditionalWeakTable<Image, object> Owned = new ConditionalWeakTable<Image, object>();

        #endregion

        #region Public methods

        /// <summary>
        /// Shows the image at the passed URL in a picture box, or the fallback image when there is none or it fails.
        /// </summary>
        /// <param name="pictureBox"> The picture box to fill. </param>
        /// <param name="url"> The image's URL. </param>
        /// <param name="fallback"> The image shown when the URL is empty or the download fails. </param>
        public static async Task LoadImageAsync(this PictureBox pictureBox, string url, Image fallback = null)
        {
            // Remembering what this call is about, so a slower earlier call can't overwrite a newer one.
            pictureBox.ImageLocation = null;
            pictureBox.Tag = url;

            Image image = null;

            if (!string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                try
                {
                    byte[] bytes = await Download(url);

                    using (MemoryStream stream = new MemoryStream(bytes))
                    using (Image downloaded = Image.FromStream(stream))
                    {
                        // Copying, since an image made from a stream needs the stream to stay open.
                        image = new Bitmap(downloaded);
                    }

                    Owned.AddOrUpdate(image, null);
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is ArgumentException || ex is ExternalException)
                {
                    // Forgetting the failed download so it can be retried later.
                    Cache.TryRemove(url, out _);
                    image = null;
                }
            }

            if (pictureBox.IsDisposed || !Equals(pictureBox.Tag, url))
            {
                image?.Dispose();
                return;
            }

            SetImage(pictureBox, image ?? fallback);
        }

        /// <summary>
        /// Clears a picture box, disposing the image if the loader created it.
        /// </summary>
        /// <param name="pictureBox"> The picture box to clear. </param>
        public static void ClearImage(this PictureBox pictureBox)
        {
            pictureBox.Tag = null;
            SetImage(pictureBox, null);
        }

        #endregion

        #region Private methods

        private static Task<byte[]> Download(string url)
        {
            return Cache.GetOrAdd(url, key => Client.GetByteArrayAsync(key));
        }

        private static void SetImage(PictureBox pictureBox, Image image)
        {
            Image previous = pictureBox.Image;
            pictureBox.Image = image;

            if (previous != null && previous != image && Owned.TryGetValue(previous, out _))
            {
                Owned.Remove(previous);
                previous.Dispose();
            }
        }

        #endregion
    }
}
