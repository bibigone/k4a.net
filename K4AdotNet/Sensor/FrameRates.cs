using System;
using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension and static methods for <see cref="FrameRate"/> enumeration.</summary>
    public static class FrameRates
    {
        public static readonly IReadOnlyList<FrameRate> All = new[]
        {
            FrameRate.Five,
            FrameRate.Fifteen,
            FrameRate.Thirty,
        };

        /// <summary>Convert enumeration value to appropriate number of frames per second (Hz).</summary>
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

        /// <summary>Constructs enumeration value from appropriate number of frames per second (Hz).</summary>
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

        public static bool IsCompatibleWith(this FrameRate frameRate, DepthMode depthMode)
            => !(frameRate == FrameRate.Thirty && depthMode == DepthMode.WideViewUnbinned);

        public static bool IsCompatibleWith(this FrameRate frameRate, ColorResolution colorResolution)
            => !(frameRate == FrameRate.Thirty && colorResolution == ColorResolution.R3072p);
    }
}
