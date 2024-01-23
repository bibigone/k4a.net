using K4AdotNet.Sensor;
using System.Collections.Generic;
using System.Linq;

namespace K4AdotNet.Samples.Wpf
{
    public static class Helpers
    {
        public static readonly IReadOnlyList<KeyValuePair<DepthMode, string>> AllDepthModes
            = new Dictionary<DepthMode, string>
            {
                { DepthMode.Off, "OFF" },
                { DepthMode.NarrowView2x2Binned, "Narrow, 2x2 binned (320x288)" },
                { DepthMode.NarrowViewUnbinned, "Narrow, unbinned (640x576)" },
                { DepthMode.WideView2x2Binned, "Wide, 2x2 binned (512x512)" },
                { DepthMode.WideViewUnbinned, "Wide, unbinned (1024x1024)" },
                { DepthMode.PassiveIR, "Passive IR, no depth (1024x1024)" },
            }.ToList();

        public static readonly IReadOnlyList<KeyValuePair<ColorResolution, string>> AllColorResolutions
            = new Dictionary<ColorResolution, string>
            {
                { ColorResolution.Off, "OFF" },
                { ColorResolution.R720p, "1280x720 (16:9)" },
                { ColorResolution.R1080p, "1920x1080 (16:9)" },
                { ColorResolution.R1440p, "2560x1440 (16:9)" },
#if !ORBBECSDK_K4A_WRAPPER
                { ColorResolution.R1536p, "2048x1536 (4:3)" },
#endif
                { ColorResolution.R2160p, "3840x2160 (16:9)" },
#if !ORBBECSDK_K4A_WRAPPER
                { ColorResolution.R3072p, "4096x3072 (4:3)" },
#endif
            }.ToList();

        public static readonly IReadOnlyList<KeyValuePair<FrameRate, string>> AllFrameRates
            = new Dictionary<FrameRate, string>
            {
                { FrameRate.Five, "5 FPS" },
                { FrameRate.Fifteen, "15 FPS" },
#if ORBBECSDK_K4A_WRAPPER
                { FrameRate.TwentyFive, "25 FPS" },
#endif
                { FrameRate.Thirty, "30 FPS" },
            }.ToList();

    }
}
