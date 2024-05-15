using System;
using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Extensions to <see cref="FrameRate"/> enumeration. Adds some metadata and capabilities to <see cref="FrameRate"/> enumeration.</summary>
    /// <seealso cref="FrameRate"/>
    public static class FrameRates
    {
        /// <summary>
        /// All possible <see cref="FrameRate"/>s.
        /// May be helpful for UI, tests, etc.
        /// </summary>
        public static readonly IReadOnlyList<FrameRate> All = new[]
        {
            FrameRate.Five,
            FrameRate.Fifteen,
            FrameRate.Thirty,
        };

        /// <summary>Converts enumeration value to appropriate number of frames per second (Hz).</summary>
        /// <param name="frameRate">Enumeration element to be converted to Hz number.</param>
        /// <returns>Appropriate nominal number of frames per second (Hz).</returns>
        /// <remarks>
        /// The actual frame rate of camera may vary slightly due to dropped data, synchronization variation between devices, clock accuracy.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Unknown value of <paramref name="frameRate"/>.</exception>
        public static int ToNumberHz(this FrameRate frameRate)
        {
            switch (frameRate)
            {
                case FrameRate.Five: return 5;
                case FrameRate.Fifteen: return 15;
                case FrameRate.Thirty: return 30;
                default: throw new ArgumentOutOfRangeException(nameof(frameRate));
            }
        }

        /// <summary>Creates enumeration value from appropriate number of frames per second (Hz).</summary>
        /// <param name="frameRateHz">Frame rate as number in Hz.</param>
        /// <returns>Appropriate enumeration element.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value of <paramref name="frameRateHz"/> is not supported.</exception>
        public static FrameRate FromNumberHz(int frameRateHz)
        {
            switch (frameRateHz)
            {
                case 5: return FrameRate.Five;
                case 15: return FrameRate.Fifteen;
                case 30: return FrameRate.Thirty;
                default: throw new ArgumentOutOfRangeException(nameof(frameRateHz));
            }
        }

        /// <summary>Checks compatibility of a given frame rate with particular depth mode.</summary>
        /// <param name="frameRate">Frame rate under test.</param>
        /// <param name="depthMode">Depth mode to be checked for compatibility with <paramref name="frameRate"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="depthMode"/> can be used in combination with <paramref name="frameRate"/>,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#depth-camera-supported-operating-modes
        /// </remarks>
        /// <seealso cref="DepthModes.IsCompatibleWith(DepthMode, FrameRate)"/>
        public static bool IsCompatibleWith(this FrameRate frameRate, DepthMode depthMode)
            => depthMode != DepthMode.WideViewUnbinned
                || depthMode == DepthMode.WideViewUnbinned && (frameRate == FrameRate.Five || frameRate == FrameRate.Fifteen);

        /// <summary>Checks compatibility of a given frame rate with particular resolution of color camera.</summary>
        /// <param name="frameRate">Frame rate under test.</param>
        /// <param name="colorResolution">Color camera resolution to be checked for compatibility with <paramref name="frameRate"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="colorResolution"/> can be used in combination with <paramref name="frameRate"/>,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// For details see:
        /// https://docs.microsoft.com/en-us/azure/Kinect-dk/hardware-specification#color-camera-supported-operating-modes
        /// </remarks>
        /// <seealso cref="ColorResolutions.IsCompatibleWith(ColorResolution, FrameRate, bool)"/>
        public static bool IsCompatibleWith(this FrameRate frameRate, ColorResolution colorResolution, bool isOrbbec)
        {
            if (isOrbbec)
                return colorResolution != ColorResolution.R3072p && colorResolution != ColorResolution.R1536p;
            else
                return !(frameRate == FrameRate.Thirty && colorResolution == ColorResolution.R3072p);
        }
    }
}
