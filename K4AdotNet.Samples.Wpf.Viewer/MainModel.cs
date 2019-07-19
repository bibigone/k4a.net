using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class MainModel : INotifyPropertyChanged
    {
        private readonly IApp app;

        public MainModel()
        { }

        public MainModel(IApp app)
            => this.app = app;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetPropertyValue<T>(ref T field, T value, string propertyName, string additionalPropertyName = null)
            where T : struct
        {
            if (!field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                if (!string.IsNullOrEmpty(additionalPropertyName))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(additionalPropertyName));
                return true;
            }
            return false;
        }

        #region Playback

        public bool IsPlaybackEnabled => !DisableColor || !DisableDepth;

        public bool DisableColor
        {
            get => disableColor;
            set => SetPropertyValue(ref disableColor, value, nameof(DisableColor), nameof(IsPlaybackEnabled));
        }
        private bool disableColor;

        public bool DisableDepth
        {
            get => disableDepth;
            set => SetPropertyValue(ref disableDepth, value, nameof(DisableDepth), nameof(IsPlaybackEnabled));
        }
        private bool disableDepth;

        public bool PlayAsFastAsPossible
        {
            get => playAsFastAsPossible;
            set => SetPropertyValue(ref playAsFastAsPossible, value, nameof(PlayAsFastAsPossible));
        }
        private bool playAsFastAsPossible;

        public void Playback()
        {
            var filePath = app.BrowseFileToOpen("MKV recordings|*.mkv");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    using (app.IndicateWaiting())
                    {
                        var readLoop = BackgroundReadLoop.CreateForPlayback(filePath,
                            disableColor: DisableColor,
                            disableDepth: DisableDepth,
                            playAsFastAsPossible: PlayAsFastAsPossible);
                        var viewModel = new ViewerModel(app, readLoop);
                        app.ShowWindowForModel(viewModel);
                    }
                }
                catch (Exception exc)
                {
                    app.ShowErrorMessage(exc.Message, "Cannot open file for playback");
                }
            }
        }

        #endregion

        #region Kinect device

        public IReadOnlyList<KeyValuePair<DepthMode, string>> DepthModes { get; }
            = new Dictionary<DepthMode, string>
            {
                { DepthMode.Off, "OFF" },
                { DepthMode.NarrowView2x2Binned, "Narrow, 2x2 binned (320x288)" },
                { DepthMode.NarrowViewUnbinned, "Narrow, unbinned (640x576)" },
                { DepthMode.WideView2x2Binned, "Wide, 2x2 binned (512x512)" },
                { DepthMode.WideViewUnbinned, "Wide, unbinned (1024x1024)" },
                { DepthMode.PassiveIR, "Passive IR, no depth (1024x1024)" },
            }.ToList();

        public DepthMode DepthMode
        {
            get => depthMode;
            set => SetPropertyValue(ref depthMode, value, nameof(DepthMode), nameof(IsOpenDeviceEnabled));
        }
        private DepthMode depthMode = DepthMode.WideView2x2Binned;

        public IReadOnlyList<KeyValuePair<ColorResolution, string>> ColorResolutions { get; }
            = new Dictionary<ColorResolution, string>
            {
                { ColorResolution.Off, "OFF" },
                { ColorResolution.R720p, "1280x720 (16:9)" },
                { ColorResolution.R1080p, "1920x1080 (16:9)" },
                { ColorResolution.R1440p, "2560x1440 (16:9)" },
                { ColorResolution.R1536p, "2048x1536 (4:3)" },
                { ColorResolution.R2160p, "3840x2160 (16:9)" },
                { ColorResolution.R3072p, "4096x3072 (4:3)" },
            }.ToList();

        public ColorResolution ColorResolution
        {
            get => colorResolution;
            set => SetPropertyValue(ref colorResolution, value, nameof(ColorResolution), nameof(IsOpenDeviceEnabled));
        }
        private ColorResolution colorResolution = ColorResolution.R720p;

        public IReadOnlyList<KeyValuePair<FrameRate, string>> FrameRates { get; }
            = new Dictionary<FrameRate, string>
            {
                { FrameRate.Five, "5 FPS" },
                { FrameRate.Fifteen, "15 FPS" },
                { FrameRate.Thirty, "30 FPS" },
            }.ToList();

        public FrameRate FrameRate
        {
            get => frameRate;
            set => SetPropertyValue(ref frameRate, value, nameof(FrameRate), nameof(IsOpenDeviceEnabled));
        }
        private FrameRate frameRate = FrameRate.Fifteen;

        public bool IsOpenDeviceEnabled
            => (ColorResolution != ColorResolution.Off || DepthMode != DepthMode.Off)
            && ColorResolution.IsCompatibleWith(FrameRate)
            && DepthMode.IsCompatibleWith(FrameRate);

        public void OpenDevice()
        {
            if (!Device.TryOpen(out var device))
            {
                app.ShowErrorMessage("Kinect device not found. Make sure that Kinect device is connected and has power supply.", "Not Connected");
                return;
            }

            try
            {
                using (app.IndicateWaiting())
                {
                    var readLoop = BackgroundReadLoop.CreateForDevice(device, DepthMode, ColorResolution, FrameRate);
                    var viewModel = new ViewerModel(app, readLoop);
                    app.ShowWindowForModel(viewModel);
                }
            }
            catch (Exception exc)
            {
                device.Dispose();
                app.ShowErrorMessage(exc.Message, "Device Failed");
            }
        }

        #endregion
    }
}
