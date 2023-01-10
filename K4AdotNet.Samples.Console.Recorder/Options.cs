using CommandLine;
using K4AdotNet.Sensor;
using System;
using System.IO;

namespace K4AdotNet.Samples.Console.Recorder
{
    /// <summary>Command line options.</summary>
    internal sealed class Options
    {
        [Option("device", Required = false, Default = 0, HelpText =
            "Specify the device index to use.")]
        public int DeviceIndex { get; set; }

        [Option('l', "record-length", Required = false, Default = 0, HelpText =
            "Limit the recording to N seconds (0 means 'till key press').")]
        public int RecordLengthSeconds { get; set; }

        [Option('c', "color-mode", Required = false, Default = "1080p", HelpText =
            "Set the color sensor mode.\n" +
            "Available options: 3072p, 2160p, 1536p, 1440p, 1080p, 720p, 720p_NV12, 720p_YUY2, OFF")]
        public string ColorMode { get; set; } = "1080p";

        [Option('d', "depth-mode", Required = false, Default = "NFOV_UNBINNED", HelpText =
            "Set the depth sensor mode.\n" +
            "Available options: NFOV_2X2BINNED, NFOV_UNBINNED, WFOV_2X2BINNED, WFOV_UNBINNED, PASSIVE_IR, OFF")]
        public string DepthMode { get; set; } = "NFOV_UNBINNED";

        [Option("depth-delay", Required = false, Default = 0, HelpText =
            "Set the time offset between color and depth frames in microseconds.\n" +
            "A negative value means depth frames will arrive before color frames.\n" +
            "The delay must be less than 1 frame period.")]
        public int DepthDelayMicroseconds { get; set; }

        [Option('r', "rate", Required = false, Default = 30, HelpText =
            "Set the camera frame rate in Frames per Second.\n" +
            "Available options: 30, 15, 5")]
        public int FrameRate { get; set; }

        [Value(0, Default = null, Required = false, MetaValue = "output.mkv", HelpText =
            "Output file name. Default: 'yyyy-MM-dd H_mm_ss.mkv' file in 'My Videos' folder.")]
        public string? Output { get; set; }

        public int GetDeviceIndex()
        {
            if (DeviceIndex < 0)
                throw new ApplicationException($"Invalid value {DeviceIndex} of parameter --device");
            return DeviceIndex;
        }

        public string GetOutputFilePath()
        {
            var res = Output;
            if (string.IsNullOrEmpty(res))
            {
                var dstPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                res = Path.Combine(dstPath, string.Format("{0:yyyy-MM-dd H_mm_ss}.mkv", DateTime.Now));
            }
            return res;
        }

        public DeviceConfiguration GetConfiguration()
        {
            var config = DeviceConfiguration.DisableAll;
            config.CameraFps = GetFrameRate();
            config.DepthMode = GetDepthMode();
            config.DepthDelayOffColor = new Microseconds32(DepthDelayMicroseconds);
            (config.ColorResolution, config.ColorFormat) = GetColorResolutionAndFormat();
            return config;
        }

        private (ColorResolution resolution, ImageFormat format) GetColorResolutionAndFormat()
            => (ColorMode.ToUpperInvariant()) switch
                {
                    "3072P" => (ColorResolution.R3072p, ImageFormat.ColorMjpg),
                    "2160P" => (ColorResolution.R2160p, ImageFormat.ColorMjpg),
                    "1536P" => (ColorResolution.R1536p, ImageFormat.ColorMjpg),
                    "1440P" => (ColorResolution.R1440p, ImageFormat.ColorMjpg),
                    "1080P" => (ColorResolution.R1080p, ImageFormat.ColorMjpg),
                    "720P" => (ColorResolution.R720p, ImageFormat.ColorMjpg),
                    "720P_NV12" => (ColorResolution.R720p, ImageFormat.ColorNV12),
                    "720P_YUY2" => (ColorResolution.R720p, ImageFormat.ColorYUY2),
                    "OFF" => (ColorResolution.Off, ImageFormat.ColorMjpg),
                    _ => throw new ApplicationException($"Invalid value {ColorMode} of parameter --color-mode (-c)"),
                };

        private DepthMode GetDepthMode()
            => (DepthMode.ToUpperInvariant()) switch
                {
                    "NFOV_2X2BINNED" => Sensor.DepthMode.NarrowView2x2Binned,
                    "NFOV_UNBINNED" => Sensor.DepthMode.NarrowViewUnbinned,
                    "WFOV_2X2BINNED" => Sensor.DepthMode.WideView2x2Binned,
                    "WFOV_UNBINNED" => Sensor.DepthMode.WideViewUnbinned,
                    "PASSIVE_IR" => Sensor.DepthMode.PassiveIR,
                    "OFF" => Sensor.DepthMode.Off,
                    _ => throw new ApplicationException($"Invalid value {DepthMode} of parameter --depth-mode (-d)"),
                };

        private FrameRate GetFrameRate()
            => FrameRate switch
                {
                    5 => Sensor.FrameRate.Five,
                    15 => Sensor.FrameRate.Fifteen,
                    30 => Sensor.FrameRate.Thirty,
                    _ => throw new ApplicationException($"Invalid value {FrameRate} of parameter --rate (-r)"),
                };
    }
}
