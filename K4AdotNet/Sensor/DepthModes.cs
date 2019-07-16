using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension methods for <see cref="DepthMode"/> enumeration.</summary>
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

        public static readonly IReadOnlyList<DepthMode> All = new[]
        {
            DepthMode.NarrowView2x2Binned,
            DepthMode.NarrowViewUnbinned,
            DepthMode.WideView2x2Binned,
            DepthMode.WideViewUnbinned,
            DepthMode.PassiveIR,
        };

        public static bool HasDepth(this DepthMode depthMode)
            => depthMode > DepthMode.Off && depthMode < DepthMode.PassiveIR;

        public static bool HasPassiveIR(this DepthMode depthMode)
            => depthMode > DepthMode.Off && depthMode <= DepthMode.PassiveIR;

        // https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        public static bool IsCompatibleWith(this DepthMode depthMode, FrameRate frameRate)
            => !(depthMode == DepthMode.WideViewUnbinned && frameRate == FrameRate.Thirty);

        /// <summary>Returns depth and IR images width in pixels for a given depth mode.</summary>
        public static int WidthPixels(this DepthMode depthMode)
            => widths[(int)depthMode];

        /// <summary>Returns depth and IR images height in pixels for a given depth mode.</summary>
        public static int HeightPixels(this DepthMode depthMode)
            => heights[(int)depthMode];

        /// <summary>Is depth mode has wide field of view?</summary>
        public static bool IsWideView(this DepthMode depthMode)
            => depthMode >= DepthMode.WideView2x2Binned;

        /// <summary>Does depth mode use binning for smoothing/filtering?</summary>
        public static bool IsBinned(this DepthMode depthMode)
            => depthMode == DepthMode.NarrowView2x2Binned || depthMode == DepthMode.WideView2x2Binned;

        // https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification
        public static void GetOperatingRange(this DepthMode depthMode, out int minDistanceMm, out int maxDistanceMm)
        {
            minDistanceMm = operatingMinsMm[(int)depthMode];
            maxDistanceMm = operatingMaxsMm[(int)depthMode];
        }

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
