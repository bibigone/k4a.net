using System;

namespace K4AdotNet.Sensor
{
    /// <summary>Helper extension and static methods for <see cref="FrameRate"/> enumeration.</summary>
    public static class FrameRateExtensions
    {
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
    }
}
