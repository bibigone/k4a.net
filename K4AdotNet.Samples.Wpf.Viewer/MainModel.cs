using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class MainModel : ViewModelBase
    {
        //  For designer
        public MainModel() : base()
        { }

        public MainModel(IApp app) : base(app)
        { }

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

        public bool DoNotPlayFasterThanOriginalFps
        {
            get => doNotPlayFasterThanOriginalFps;
            set => SetPropertyValue(ref doNotPlayFasterThanOriginalFps, value, nameof(DoNotPlayFasterThanOriginalFps));
        }
        private bool doNotPlayFasterThanOriginalFps = true;

        public void Playback()
        {
            var filePath = app!.BrowseFileToOpen("MKV recordings|*.mkv");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    using (app.IndicateWaiting())
                    {
                        var readingLoop = BackgroundReadingLoop.CreateForPlayback(filePath,
                            disableColor: DisableColor,
                            disableDepth: DisableDepth,
                            doNotPlayFasterThanOriginalFps: DoNotPlayFasterThanOriginalFps);
                        var viewModel = new ViewerModel(app, readingLoop);
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

        public int DeviceIndex
        {
            get => deviceIndex;
            set => SetPropertyValue(ref deviceIndex, value, nameof(DeviceIndex), nameof(IsOpenDeviceEnabled));
        }
        private int deviceIndex = Device.DefaultDeviceIndex;

        public IReadOnlyList<int> DeviceIndicies
        {
            get => deviceIndicies;
            set => SetPropertyValue(ref deviceIndicies, value, nameof(DeviceIndicies), nameof(IsOpenDeviceEnabled));
        }
        private IReadOnlyList<int> deviceIndicies = Array.Empty<int>();

        public void RefreshDevices()
        {
            try
            {
                var count = Device.InstalledCount;
                if (count > 0)
                {
                    DeviceIndicies = Enumerable.Range(0, count).ToList();
                    if (DeviceIndex >= count || DeviceIndex < 0)
                        DeviceIndex = Device.DefaultDeviceIndex;
                }
                else
                {
                    DeviceIndicies = Array.Empty<int>();
                    DeviceIndex = Device.DefaultDeviceIndex;
                }
            }
            catch (Exception exc)
            {
                app!.ShowErrorMessage(exc.Message);
            }
        }

        public IReadOnlyList<KeyValuePair<DepthMode, string>> DepthModes { get; } = Helpers.AllDepthModes;

        public DepthMode DepthMode
        {
            get => depthMode;
            set => SetPropertyValue(ref depthMode, value, nameof(DepthMode), nameof(IsOpenDeviceEnabled));
        }
        private DepthMode depthMode = DepthMode.NarrowViewUnbinned;

        public IReadOnlyList<KeyValuePair<ColorResolution, string>> ColorResolutions { get; } = Helpers.AllColorResolutions;

        public ColorResolution ColorResolution
        {
            get => colorResolution;
            set => SetPropertyValue(ref colorResolution, value, nameof(ColorResolution), nameof(IsOpenDeviceEnabled));
        }
        private ColorResolution colorResolution = ColorResolution.R720p;

        public IReadOnlyList<KeyValuePair<FrameRate, string>> FrameRates { get; } = Helpers.AllFrameRates;

        public FrameRate FrameRate
        {
            get => frameRate;
            set => SetPropertyValue(ref frameRate, value, nameof(FrameRate), nameof(IsOpenDeviceEnabled));
        }
        private FrameRate frameRate = FrameRate.Thirty;

        public bool IsOpenDeviceEnabled
        {
            get
            {
                var isOrbbec = false;
                if (Sdk.ComboMode == ComboMode.Orbbec)
                    isOrbbec = true;
                else if (Sdk.ComboMode == ComboMode.Both && DeviceIndex >= Device.Azure.InstalledCount)
                    isOrbbec = true;

                return (ColorResolution != ColorResolution.Off || DepthMode != DepthMode.Off)
                    && ColorResolution.IsCompatibleWith(FrameRate, isOrbbec)
                    && DepthMode.IsCompatibleWith(FrameRate)
                    && DeviceIndex >= 0 && DeviceIndex < DeviceIndicies.Count;
            }
        }

        public void OpenDevice()
        {
            Device? device = null;

            try
            {
                device = Device.Open(DeviceIndex);
                using (app!.IndicateWaiting())
                {
                    var readingLoop = BackgroundReadingLoop.CreateForDevice(device, DepthMode, ColorResolution, FrameRate);
                    var viewModel = new ViewerModel(app, readingLoop);
                    app.ShowWindowForModel(viewModel);
                }
            }
            catch (Exception exc)
            {
                device?.Dispose();
                app!.ShowErrorMessage(exc.Message, "Device Failed");
            }
        }

        #endregion
    }
}
