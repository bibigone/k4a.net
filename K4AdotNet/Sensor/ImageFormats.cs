using System;
using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Extensions to <see cref="ImageFormat"/> enumeration. Adds some metadata to <see cref="ImageFormat"/> enumeration.</summary>
    /// <seealso cref="ImageFormat"/>
    public static class ImageFormats
    {
        /// <summary>All usable <see cref="ImageFormat"/>s. May be helpful for UI, tests, etc.</summary>
        public static readonly IReadOnlyList<ImageFormat> All = new[] {
            ImageFormat.ColorMjpg,
            ImageFormat.ColorNV12,
            ImageFormat.ColorYUY2,
            ImageFormat.ColorBgra32,
            ImageFormat.Depth16,
            ImageFormat.IR16,
            ImageFormat.Custom,
        };

        /// <summary>Is this format usable for color images?</summary>
        /// <param name="imageFormat">Image format to be tested.</param>
        /// <returns><see langword="true"/> if image format is suitable for color images, <see langword="false"/> - otherwise.</returns>
        public static bool IsColor(this ImageFormat imageFormat)
            => imageFormat <= ImageFormat.ColorBgra32;

        /// <summary>Is this format a depth map?</summary>
        /// <param name="imageFormat">Image format to be tested.</param>
        /// <returns><see langword="true"/> if image format is a depth map, <see langword="false"/> - otherwise.</returns>
        public static bool IsDepth(this ImageFormat imageFormat)
            => imageFormat == ImageFormat.Depth16;

        /// <summary>Does this image format have known (predefined) bytes per pixel?</summary>
        /// <param name="imageFormat">Image format to be tested.</param>
        /// <returns><see langword="true"/> if bytes-per-pixel is known for specified <paramref name="imageFormat"/>, <see langword="false"/> - otherwise.</returns>
        public static bool HasKnownBytesPerPixel(this ImageFormat imageFormat)
            => imageFormat == ImageFormat.Depth16 || imageFormat == ImageFormat.IR16
            || imageFormat == ImageFormat.ColorBgra32 || imageFormat == ImageFormat.ColorYUY2;

        /// <summary>How many bytes are used for one pixel?</summary>
        /// <param name="imageFormat">Image format.</param>
        /// <returns>How many bytes are used for one pixel.</returns>
        /// <exception cref="ArgumentException">Bytes-per-pixel is unknown/undefined for specified <paramref name="imageFormat"/>.</exception>
        public static int BytesPerPixel(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Depth16:
                case ImageFormat.IR16:
                case ImageFormat.ColorYUY2:
                    return 2;
                case ImageFormat.ColorBgra32:
                    return 4;
                default:
                    throw new ArgumentException($"Cannot determine bytes per pixel for {imageFormat} format.", nameof(imageFormat));
            }
        }

        /// <summary>Calculates default image stride from image <paramref name="widthPixels"/> for specified <paramref name="imageFormat"/>.</summary>
        /// <param name="imageFormat">Format of image. Not all formats have predefined formula for stride based on image width. Moreover, some formats do not have stride at all.</param>
        /// <param name="widthPixels">Width of image in pixels. Non-negative.</param>
        /// <returns>Default value for image stride (the number of bytes per horizontal line of the image).</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="widthPixels"/> cannot be less than zero.</exception>
        /// <exception cref="ArgumentException">Cannot determine image stride in bytes from <paramref name="widthPixels"/> for specified <paramref name="imageFormat"/>.</exception>
        public static int StrideBytes(this ImageFormat imageFormat, int widthPixels)
        {
            if (widthPixels < 0)
                throw new ArgumentOutOfRangeException(nameof(widthPixels));
            switch (imageFormat)
            {
                case ImageFormat.Depth16:
                case ImageFormat.IR16:
                case ImageFormat.ColorYUY2:
                    return 2 * widthPixels;
                case ImageFormat.ColorBgra32:
                    return 4 * widthPixels;
                case ImageFormat.ColorNV12:
                    return widthPixels;
                default:
                    throw new ArgumentException($"Cannot determine image stride in bytes from width in pixels for {imageFormat} format.", nameof(imageFormat));
            }
        }

        /// <summary>Calculate image data size in bytes.</summary>
        /// <param name="imageFormat">Image format. Any format can be used if <paramref name="strideBytes"/> is not null.</param>
        /// <param name="strideBytes">Image stride in bytes. Must be positive number. Cannot be zero.</param>
        /// <param name="heightPixels">Image height in pixel. Non-negative.</param>
        /// <returns>Calculated image data size in bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="strideBytes"/> or <paramref name="strideBytes"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If image has unknown stride that is <paramref name="strideBytes"/> equals zero.
        /// </exception>
        public static int ImageSizeBytes(this ImageFormat imageFormat, int strideBytes, int heightPixels)
        {
            if (strideBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            if (strideBytes == 0)
                throw new ArgumentException($"Cannot calculate image size in bytes when stride is unknown.", nameof(strideBytes));
            if (heightPixels < 0)
                throw new ArgumentOutOfRangeException(nameof(heightPixels));
            var res = strideBytes * heightPixels;
            // special case for NV12
            if (imageFormat == ImageFormat.ColorNV12)
                res = 3 * res / 2;
            return res;
        }
    }
}
