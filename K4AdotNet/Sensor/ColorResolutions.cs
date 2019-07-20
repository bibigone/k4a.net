using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension methods for <see cref="ColorResolution"/> enumeration.</summary>
    public static class ColorResolutions
    {
        private static readonly int[] heights = new[] { 0, 720, 1080, 1440, 1536, 2160, 3072 };
        private const float NOMINAL_HFOV_DEGREES = 90f;
        private const float NOMINAL_VFOV_4_3_DEGREES = 74.3f;
        private const float NOMINAL_VFOV_16_9_DEGREES = 59f;

        public static readonly IReadOnlyList<ColorResolution> All = new[]
        {
            ColorResolution.R720p,
            ColorResolution.R1080p,
            ColorResolution.R1440p,
            ColorResolution.R1536p,
            ColorResolution.R2160p,
            ColorResolution.R3072p,
        };

        // https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        public static bool IsCompatibleWith(this ColorResolution colorResolution, FrameRate frameRate)
            => !(colorResolution == ColorResolution.R3072p && frameRate == FrameRate.Thirty);

        public static bool IsCompatibleWith(this ColorResolution colorResolution, ImageFormat imageFormat)
            => imageFormat == ImageFormat.ColorBgra32 || imageFormat == ImageFormat.ColorMjpg
            || (colorResolution == ColorResolution.R720p && (imageFormat == ImageFormat.ColorNV12 || imageFormat == ImageFormat.ColorYUY2));

        /// <summary>Returns image width in pixels for a given resolution.</summary>
        public static int WidthPixels(this ColorResolution resolution)
            => resolution.IsAspectRatio4to3()
                ? resolution.HeightPixels() * 4 / 3
                : resolution.HeightPixels() * 16 / 9;

        /// <summary>Returns image height in pixels for a given resolution.</summary>
        public static int HeightPixels(this ColorResolution resolution)
            => heights[(int)resolution];

        public static void GetNominalFov(this ColorResolution resolution, out float horizontalDegrees, out float verticalDegrees)
        {
            if (resolution.IsAspectRatio16to9())
            {
                horizontalDegrees = NOMINAL_HFOV_DEGREES;
                verticalDegrees = NOMINAL_VFOV_16_9_DEGREES;
            }
            else if (resolution.IsAspectRatio4to3())
            {
                horizontalDegrees = NOMINAL_HFOV_DEGREES;
                verticalDegrees = NOMINAL_VFOV_4_3_DEGREES;
            }
            else
            {
                horizontalDegrees = 0;
                verticalDegrees = 0;
            }
        }

        public static bool IsAspectRatio4to3(this ColorResolution resolution)
            => resolution == ColorResolution.R1536p || resolution == ColorResolution.R3072p;

        public static bool IsAspectRatio16to9(this ColorResolution resolution)
            => resolution == ColorResolution.R720p || resolution == ColorResolution.R1080p
            || resolution == ColorResolution.R1440p || resolution == ColorResolution.R2160p;
    }
}
