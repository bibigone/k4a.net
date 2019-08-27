using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    internal class StartViewModel : ViewModelBase
    {
        public StartViewModel(IApp app) 
            : base(app)
        {
            playbackCommand = new DelegateCommand(Playback);
            openDeviceCommand = new DelegateCommand(OpenDevice, CanOpenDevice);
        }

        #region Playback

        public ICommand PlaybackCommand => playbackCommand;
        private readonly DelegateCommand playbackCommand;

        private void Playback()
        {
            var filePath = app.BrowseFileToOpen("MKV recordings|*.mkv");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    using (app.IndicateWaiting())
                    {
                        var readingLoop = BackgroundReadingLoop.CreateForPlayback(filePath,
                            disableColor: false, disableDepth: false, doNotPlayFasterThanOriginalFps: true);

                        if (readingLoop.ColorResolution == ColorResolution.Off)
                            throw new ApplicationException("There is no color data in the video");

                        if (readingLoop.DepthMode == DepthMode.Off || readingLoop.DepthMode == DepthMode.PassiveIR)
                            throw new ApplicationException("There is no depth data in the video");

                        var viewModel = new ProcessingViewModel(readingLoop);
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

        #region Device

        public IReadOnlyList<KeyValuePair<DepthMode, string>> DepthModes { get; } 
            = Helpers.AllDepthModes.Where(i => i.Key != DepthMode.Off && i.Key != DepthMode.PassiveIR).ToArray();

        public DepthMode DepthMode
        {
            get => depthMode;
            set
            {
                var changed = SetPropertyValue(ref depthMode, value, nameof(DepthMode));
                if (changed)
                    openDeviceCommand.InvalidateCanExecute();
            }
        }
        private DepthMode depthMode = DepthMode.WideView2x2Binned;

        public IReadOnlyList<KeyValuePair<ColorResolution, string>> ColorResolutions { get; } 
            = Helpers.AllColorResolutions.Where(i => i.Key != ColorResolution.Off).ToArray();

        public ColorResolution ColorResolution
        {
            get => colorResolution;
            set
            {
                var changed = SetPropertyValue(ref colorResolution, value, nameof(ColorResolution));
                if (changed)
                    openDeviceCommand.InvalidateCanExecute();
            }
        }
        private ColorResolution colorResolution = ColorResolution.R720p;

        public IReadOnlyList<KeyValuePair<FrameRate, string>> FrameRates { get; } = Helpers.AllFrameRates;

        public FrameRate FrameRate
        {
            get => frameRate;
            set
            {
                var changed = SetPropertyValue(ref frameRate, value, nameof(FrameRate));
                if (changed)
                    openDeviceCommand.InvalidateCanExecute();
            }
        }
        private FrameRate frameRate = FrameRate.Fifteen;

        public ICommand OpenDeviceCommand => openDeviceCommand;
        private readonly DelegateCommand openDeviceCommand;

        private void OpenDevice()
        {
            try
            {
                using (app.IndicateWaiting())
                {
                    var device = Device.Open();
                    var readingLoop = BackgroundReadingLoop.CreateForDevice(device, DepthMode, ColorResolution, FrameRate);
                    var processingModel = new ProcessingViewModel(readingLoop);
                    app.ShowWindowForModel(processingModel);
                }
            }
            catch (Exception ex)
            {
                app.ShowErrorMessage(ex.Message, "Cannot open Azure Kinect device");
            }
        }

        private bool CanOpenDevice()
            => DepthMode.IsCompatibleWith(FrameRate) && ColorResolution.IsCompatibleWith(FrameRate);

        #endregion
    }
}
