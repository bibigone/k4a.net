using System;
using System.Collections.Generic;
using System.Text;

namespace K4AdotNet.Sensor
{
    public static class ImageFormats
    {
        public static readonly IReadOnlyList<ImageFormat> All = new[] {
            ImageFormat.ColorMjpg,
            ImageFormat.ColorNV12,
            ImageFormat.ColorYUY2,
            ImageFormat.ColorBgra32,
            ImageFormat.Depth16,
            ImageFormat.IR16,
            ImageFormat.Custom,
        };

        public static bool IsColor(this ImageFormat imageFormat)
            => imageFormat <= ImageFormat.ColorBgra32;

        public static bool IsDepth(this ImageFormat imageFormat)
            => imageFormat == ImageFormat.Depth16;

        public static bool HasKnownBytesPerPixel(this ImageFormat imageFormat)
            => imageFormat == ImageFormat.Depth16 || imageFormat == ImageFormat.IR16 || imageFormat == ImageFormat.ColorBgra32;

        public static int BytesPerPixel(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Depth16:
                case ImageFormat.IR16:
                    return 2;
                case ImageFormat.ColorBgra32:
                    return 4;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
