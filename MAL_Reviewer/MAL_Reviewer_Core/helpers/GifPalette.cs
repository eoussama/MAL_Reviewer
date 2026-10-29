using System;
using System.Drawing;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Recolors GIF images by changing their color tables, which keeps every frame of an animation.
    /// </summary>
    public static class GifPalette
    {
        #region Fields

        private const byte ExtensionIntroducer = 0x21;
        private const byte ImageSeparator = 0x2C;
        private const byte Trailer = 0x3B;

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a copy of a GIF image with every color of its color tables replaced.
        /// </summary>
        /// <param name="gif"> The bytes of the GIF image. </param>
        /// <param name="recolor"> Gives the new version of a color. </param>
        /// <exception cref="FormatException"> The bytes aren't a GIF image. </exception>
        public static byte[] Recolor(byte[] gif, Func<Color, Color> recolor)
        {
            ArgumentNullException.ThrowIfNull(gif);
            ArgumentNullException.ThrowIfNull(recolor);

            if (gif.Length < 13 || gif[0] != 'G' || gif[1] != 'I' || gif[2] != 'F')
            {
                throw new FormatException("Not a GIF image.");
            }

            byte[] result = (byte[])gif.Clone();

            // The logical screen descriptor, which may be followed by the global color table.
            int position = RecolorTable(result, 13, result[10], recolor);

            while (true)
            {
                switch (ByteAt(result, position++))
                {
                    case ExtensionIntroducer:
                        position = SkipSubBlocks(result, position + 1);
                        break;

                    case ImageSeparator:
                        // The image descriptor, which may be followed by a local color table, then the LZW code size.
                        position = RecolorTable(result, position + 9, ByteAt(result, position + 8), recolor);
                        position = SkipSubBlocks(result, position + 1);
                        break;

                    case Trailer:
                        return result;

                    default:
                        throw new FormatException("Unexpected block in the GIF image.");
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recolors the color table at a position when the flags say there is one, and returns the position after it.
        /// </summary>
        private static int RecolorTable(byte[] gif, int position, byte flags, Func<Color, Color> recolor)
        {
            if ((flags & 0x80) == 0)
            {
                return position;
            }

            int size = 3 * (1 << ((flags & 0x07) + 1));

            if (position + size > gif.Length)
            {
                throw new FormatException("The GIF image is cut short.");
            }

            for (int i = position; i < position + size; i += 3)
            {
                Color color = recolor(Color.FromArgb(gif[i], gif[i + 1], gif[i + 2]));

                gif[i] = color.R;
                gif[i + 1] = color.G;
                gif[i + 2] = color.B;
            }

            return position + size;
        }

        /// <summary>
        /// Skips data sub-blocks, (each starts with its length, a length of 0 ends them).
        /// </summary>
        private static int SkipSubBlocks(byte[] gif, int position)
        {
            int length;

            while ((length = ByteAt(gif, position)) != 0)
            {
                position += length + 1;
            }

            return position + 1;
        }

        private static byte ByteAt(byte[] gif, int position)
        {
            return position < gif.Length ? gif[position] : throw new FormatException("The GIF image is cut short.");
        }

        #endregion
    }
}
