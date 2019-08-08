using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Extensions to <see cref="DepthMode"/> enumeration. Adds some metadata to <see cref="DepthMode"/> enumeration.</summary>
    /// <seealso cref="DepthMode"/>
    public static class DepthModes
    {
        private static readonly int[] widths = new[] { 0, 320, 640, 512, 1024, 1024 };
        private static readonly int[] heights = new[] { 0, 288, 576, 512, 1024, 1024 };
        private static readonly int[] operatingMinsMm = new[] { 0, 500, 500, 250, 250, 0 };
        private static readonly int[] operatingMaxsMm = new[] { 0, 5460, 3860, 2880, 2210, 0 };
        private const float NOMINAL_HFOV_NARROW_DEGREES = 75f;
        private const float NOMINAL_VFOV_NARROW_DEGREES = 65f;
        private const float NOMINAL_HFOV_WIDE_DEGREES = 120f;
        private const float NOMINAL_VFOV_WIDE_DEGREES = 120f;

        /// <summary>
        /// All possible <see cref="DepthModes"/>s including <see cref="DepthMode.Off"/>.
        /// May be helpful for UI, tests, etc.
        /// </summary>
        public static readonly IReadOnlyList<DepthMode> All = new[]
        {
            DepthMode.Off,
            DepthMode.NarrowView2x2Binned,
            DepthMode.NarrowViewUnbinned,
            DepthMode.WideView2x2Binned,
            DepthMode.WideViewUnbinned,
            DepthMode.PassiveIR,
        };

        /// <summary>Is there depth stream in a given depth mode?</summary>
        /// <param name="depthMode">Depth mode under test.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="depthMode"/> actually generates depth data,
        /// <see langword="false"/> if <paramref name="depthMode"/> is <see cref="DepthMode.Off"/> or <see cref="DepthMode.PassiveIR"/>.
        /// </returns>
        public static bool HasDepth(this DepthMode depthMode)
            => depthMode > DepthMode.Off && depthMode < DepthMode.PassiveIR;

        /// <summary>Is there IR stream in a given depth mode?</summary>
        /// <param name="depthMode">Depth mode under test.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="depthMode"/> actually generates IR data,
        /// <see langword="false"/> if <paramref name="depthMode"/> is <see cref="DepthMode.Off"/>.
        /// </returns>
        public static bool HasPassiveIR(this DepthMode depthMode)
            => depthMode > DepthMode.Off && depthMode <= DepthMode.PassiveIR;

        /// <summary>Checks that depth mode is compatible with a given framerate.</summary>
        /// <param name="depthMode">Depth mode to be tested on compatibility with <paramref name="frameRate"/>.</param>
        /// <param name="frameRate">Frame rate to be tested on compatibility with <paramref name="depthMode"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="depthMode"/> can be used in combination with <paramref name="frameRate"/>,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        /// </remarks>
        /// <seealso cref="FrameRates.IsCompatibleWith(FrameRate, DepthMode)"/>
        public static bool IsCompatibleWith(this DepthMode depthMode, FrameRate frameRate)
            => !(depthMode == DepthMode.WideViewUnbinned && frameRate == FrameRate.Thirty);

        /// <summary>Returns depth and IR images width in pixels for a given depth mode.</summary>
        /// <param name="depthMode">Element of enumeration.</param>
        /// <returns>Width in pixels.</returns>
        public static int WidthPixels(this DepthMode depthMode)
            => widths[(int)depthMode];

        /// <summary>Returns depth and IR images height in pixels for a given depth mode.</summary>
        /// <param name="depthMode">Element of enumeration.</param>
        /// <returns>Height in pixels.</returns>
        public static int HeightPixels(this DepthMode depthMode)
            => heights[(int)depthMode];

        /// <summary>Is depth mode has wide field of view?</summary>
        /// <param name="depthMode">Depth mode (element of enumeration).</param>
        /// <returns>
        /// <see langword="true"/> - wide field-of-view,
        /// <see langword="false"/> - narrow field-of-view.
        /// </returns>
        public static bool IsWideView(this DepthMode depthMode)
            => depthMode >= DepthMode.WideView2x2Binned;

        /// <summary>Does depth mode use binning for smoothing/filtering?</summary>
        /// <param name="depthMode">Depth mode (element of enumeration).</param>
        /// <returns>
        /// <see langword="true"/> - pixels are binned,
        /// <see langword="false"/> - no binning is used.
        /// </returns>
        /// <remarks>
        /// Binned modes reduce the captured camera resolution by combining adjacent sensor pixels into a bin.
        /// </remarks>
        public static bool IsBinned(this DepthMode depthMode)
            => depthMode == DepthMode.NarrowView2x2Binned || depthMode == DepthMode.WideView2x2Binned;

        /// <summary>
        /// Gets operation range (minimal and maximum distance visible on depth map) for a given depth mode.
        /// </summary>
        /// <param name="depthMode">Depth mode.</param>
        /// <param name="minDistanceMm">Minimum visible on depth map distance in millimeters.</param>
        /// <param name="maxDistanceMm">Maximum visible on depth map distance in millimeters.</param>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        /// </remarks>
        public static void GetOperatingRange(this DepthMode depthMode, out int minDistanceMm, out int maxDistanceMm)
        {
            minDistanceMm = operatingMinsMm[(int)depthMode];
            maxDistanceMm = operatingMaxsMm[(int)depthMode];
        }

        /// <summary>Gets nominal (without taking into account distortions) field-of-view (FOV) for a given depth mode.</summary>
        /// <param name="depthMode">Depth mode.</param>
        /// <param name="horizontalDegrees">Output: nominal horizontal field-of-view (FOV) in degrees.</param>
        /// <param name="verticalDegrees">Output: nominal vertical field-of-view (FOV) in degrees.</param>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        /// </remarks>
        public static void GetNominalFov(this DepthMode depthMode, out float horizontalDegrees, out float verticalDegrees)
        {
            if (depthMode == DepthMode.Off || depthMode == DepthMode.PassiveIR)
            {
                horizontalDegrees = 0;
                verticalDegrees = 0;
            }
            else if (depthMode.IsWideView())
            {
                horizontalDegrees = NOMINAL_HFOV_WIDE_DEGREES;
                verticalDegrees = NOMINAL_VFOV_WIDE_DEGREES;
            }
            else
            {
                horizontalDegrees = NOMINAL_HFOV_NARROW_DEGREES;
                verticalDegrees = NOMINAL_VFOV_NARROW_DEGREES;
            }
        }
    }
}
