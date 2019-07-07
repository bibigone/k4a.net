namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension methods for <see cref="ColorResolution"/> enumeration.</summary>
    public static class ColorResolutionExtenstions
    {
        private static readonly int[] widths = new[] { 0, 1280, 1920, 2560, 2048, 3840, 4096 };
        private static readonly int[] heights = new[] { 0, 720, 1080, 1440, 1536, 2160, 3072 };

        /// <summary>Returns image width in pixels for a given resolution.</summary>
        public static int Width(this ColorResolution resolution)
            => widths[(int)resolution];

        /// <summary>Returns image height in pixels for a given resolution.</summary>
        public static int Height(this ColorResolution resolution)
            => heights[(int)resolution];
    }
}
