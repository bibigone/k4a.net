using System;
using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Extensions to <see cref="ColorResolution"/> enumeration. Adds some metadata to <see cref="ColorResolution"/> enumeration.</summary>
    /// <seealso cref="ColorResolution"/>
    public static class ColorResolutions
    {
        private static readonly int[] heights = new[] { 0, 720, 1080, 1440, 1536, 2160, 3072 };
#if ORBBECSDK_K4A_WRAPPER
        private const float NOMINAL_HFOV_4_3_DEGREES = 65f;
        private const float NOMINAL_HFOV_16_9_DEGREES = 80f;
        private const float NOMINAL_VFOV_4_3_DEGREES = 51f;
        private const float NOMINAL_VFOV_16_9_DEGREES = 51f;
#else
        private const float NOMINAL_HFOV_4_3_DEGREES = 90f;
        private const float NOMINAL_HFOV_16_9_DEGREES = 90f;
        private const float NOMINAL_VFOV_4_3_DEGREES = 74.3f;
        private const float NOMINAL_VFOV_16_9_DEGREES = 59f;
#endif

        /// <summary>
        /// All possible <see cref="ColorResolution"/>s including <see cref="ColorResolution.Off"/>.
        /// May be helpful for UI, tests, etc.
        /// </summary>
        public static readonly IReadOnlyList<ColorResolution> All = new[]
        {
            ColorResolution.Off,
            ColorResolution.R720p,
            ColorResolution.R1080p,
            ColorResolution.R1440p,
#if !ORBBECSDK_K4A_WRAPPER
            ColorResolution.R1536p,
#endif
            ColorResolution.R2160p,
#if !ORBBECSDK_K4A_WRAPPER
            ColorResolution.R3072p,
#endif
        };

        /// <summary>Checks that resolution is compatible with a given frame rate.</summary>
        /// <param name="colorResolution">Color resolution to be tested on compatibility with <paramref name="frameRate"/>.</param>
        /// <param name="frameRate">Frame rate to be tested on compatibility with <paramref name="colorResolution"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="colorResolution"/> can be used in combination with <paramref name="frameRate"/>,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        /// <seealso cref="FrameRates.IsCompatibleWith(FrameRate, ColorResolution)"/>
        public static bool IsCompatibleWith(this ColorResolution colorResolution, FrameRate frameRate)
#if ORBBECSDK_K4A_WRAPPER
#pragma warning disable CS0618 // Type or member is obsolete
            => colorResolution != ColorResolution.R1536p && colorResolution != ColorResolution.R3072p;
#pragma warning restore CS0618 // Type or member is obsolete
#else
            => !(colorResolution == ColorResolution.R3072p && frameRate == FrameRate.Thirty);
#endif

        /// <summary>Checks that resolution is compatible with a given image format.</summary>
        /// <param name="colorResolution">Color resolution to be tested on compatibility with <paramref name="imageFormat"/>.</param>
        /// <param name="imageFormat">Image format to be tested on compatibility with <paramref name="colorResolution"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="colorResolution"/> can be used for images of <paramref name="imageFormat"/> format,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        /// <seealso cref="ImageFormat"/>
        public static bool IsCompatibleWith(this ColorResolution colorResolution, ImageFormat imageFormat)
            => imageFormat == ImageFormat.ColorBgra32 || imageFormat == ImageFormat.ColorMjpg
            || (colorResolution == ColorResolution.R720p && (imageFormat == ImageFormat.ColorNV12 || imageFormat == ImageFormat.ColorYUY2));

        /// <summary>Returns image width in pixels for a given resolution.</summary>
        /// <param name="resolution">Color resolution (element of enumeration).</param>
        /// <returns>Width in pixels.</returns>
        public static int WidthPixels(this ColorResolution resolution)
            => resolution.IsAspectRatio4to3()
                ? resolution.HeightPixels() * 4 / 3
                : resolution.HeightPixels() * 16 / 9;

        /// <summary>Returns image height in pixels for a given resolution.</summary>
        /// <param name="resolution">Color resolution (element of enumeration)</param>
        /// <returns>Height in pixels.</returns>
        public static int HeightPixels(this ColorResolution resolution)
            => heights[(int)resolution];

        /// <summary>Gets nominal (without taking into account distortions) field-of-view (FOV) for a given color resolution.</summary>
        /// <param name="resolution">Color resolution.</param>
        /// <param name="horizontalDegrees">Output: nominal horizontal field-of-view (FOV) in degrees.</param>
        /// <param name="verticalDegrees">Output: nominal vertical field-of-view (FOV) in degrees.</param>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        public static void GetNominalFov(this ColorResolution resolution, out float horizontalDegrees, out float verticalDegrees)
        {
            if (resolution.IsAspectRatio16to9())
            {
                horizontalDegrees = NOMINAL_HFOV_16_9_DEGREES;
                verticalDegrees = NOMINAL_VFOV_16_9_DEGREES;
            }
            else if (resolution.IsAspectRatio4to3())
            {
                horizontalDegrees = NOMINAL_HFOV_4_3_DEGREES;
                verticalDegrees = NOMINAL_VFOV_4_3_DEGREES;
            }
            else
            {
                horizontalDegrees = 0;
                verticalDegrees = 0;
            }
        }

        /// <summary>Aspect ratio of a given resolution: is it 4:3?</summary>
        /// <param name="resolution">Element of enumeration.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="resolution"/> has 4:3 aspect ratio,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        public static bool IsAspectRatio4to3(this ColorResolution resolution)
#pragma warning disable CS0618 // Type or member is obsolete
            => resolution == ColorResolution.R1536p || resolution == ColorResolution.R3072p;
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>Aspect ratio of a given resolution: is it 16:9?</summary>
        /// <param name="resolution">Element of enumeration.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="resolution"/> has 16:9 aspect ratio,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        public static bool IsAspectRatio16to9(this ColorResolution resolution)
            => resolution == ColorResolution.R720p || resolution == ColorResolution.R1080p
            || resolution == ColorResolution.R1440p || resolution == ColorResolution.R2160p;
    }
}
