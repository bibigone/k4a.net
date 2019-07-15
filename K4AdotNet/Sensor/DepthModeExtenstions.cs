namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension methods for <see cref="DepthMode"/> enumeration.</summary>
    public static class DepthModeExtenstions
    {
        private static readonly int[] widths = new[] { 0, 320, 640, 512, 1024, 1024 };
        private static readonly int[] heights = new[] { 0, 288, 576, 512, 1024, 1024 };

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
    }
}
