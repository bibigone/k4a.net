using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class MainModel : ViewModelBase
    {
        //  For designer
        public MainModel() : base()
        { }

        public MainModel(IApp app) : base(app)
        { }

        #region Playback

        public bool DisableColor
        {
            get => disableColor;
            set => SetPropertyValue(ref disableColor, value, nameof(DisableColor));
        }
        private bool disableColor = true;

        public bool DoNotPlayFasterThanOriginalFps
        {
            get => doNotPlayFasterThanOriginalFps;
            set => SetPropertyValue(ref doNotPlayFasterThanOriginalFps, value, nameof(DoNotPlayFasterThanOriginalFps));
        }
        private bool doNotPlayFasterThanOriginalFps;

        public void Playback()
        {
            var filePath = app.BrowseFileToOpen("MKV recordings|*.mkv");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    using (app.IndicateWaiting())
                    {
                        var readingLoop = BackgroundReadingLoop.CreateForPlayback(filePath,
                            disableColor: DisableColor,
                            disableDepth: false,
                            doNotPlayFasterThanOriginalFps: DoNotPlayFasterThanOriginalFps);
                        var viewModel = new TrackerModel(app, readingLoop, ProcessingMode, DnnModel, SensorOrientation, SmoothingFactor);
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
            = Helpers.AllDepthModes.Where(pair => pair.Key.HasDepth()).ToList();

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
        private ColorResolution colorResolution = ColorResolution.Off;

        public IReadOnlyList<KeyValuePair<FrameRate, string>> FrameRates { get; } = Helpers.AllFrameRates;

        public FrameRate FrameRate
        {
            get => frameRate;
            set => SetPropertyValue(ref frameRate, value, nameof(FrameRate), nameof(IsOpenDeviceEnabled));
        }
        private FrameRate frameRate = FrameRate.Thirty;

        public bool IsOpenDeviceEnabled
            => ColorResolution.IsCompatibleWith(FrameRate)
            && DepthMode.IsCompatibleWith(FrameRate);

        public void OpenDevice()
        {
            Device device = null;

            try
            {
                device = Device.Open();

                using (app.IndicateWaiting())
                {
                    var readingLoop = BackgroundReadingLoop.CreateForDevice(device, DepthMode, ColorResolution, FrameRate);
                    var viewModel = new TrackerModel(app, readingLoop, ProcessingMode, DnnModel, SensorOrientation, SmoothingFactor);
                    app.ShowWindowForModel(viewModel);
                }
            }
            catch (Exception exc)
            {
                device?.Dispose();
                app.ShowErrorMessage(exc.Message, "Device Failed");
            }
        }

        #endregion

        #region Settings

        public BodyTracking.TrackerProcessingMode ProcessingMode
        {
            get => processingMode;
            set => SetPropertyValue(ref processingMode, value, nameof(ProcessingMode));
        }
        private BodyTracking.TrackerProcessingMode processingMode = BodyTracking.TrackerProcessingMode.GpuCuda;

        public IReadOnlyList<KeyValuePair<BodyTracking.TrackerProcessingMode, string>> ProcessingModes { get; }
            = new Dictionary<BodyTracking.TrackerProcessingMode, string>
            {
                { BodyTracking.TrackerProcessingMode.Cpu, "CPU" },
                { BodyTracking.TrackerProcessingMode.Gpu, "GPU auto" },
                { BodyTracking.TrackerProcessingMode.GpuCuda, "GPU CUDA" },
                { BodyTracking.TrackerProcessingMode.GpuTensorRT, "GPU TensorRT" },
                { BodyTracking.TrackerProcessingMode.GpuDirectML, "GPU DirectML" },
            }.ToList();

        public DnnModel DnnModel
        {
            get => dnnModel;
            set => SetPropertyValue(ref dnnModel, value, nameof(DnnModel));
        }
        private DnnModel dnnModel = DnnModel.Default;

        public IReadOnlyList<KeyValuePair<DnnModel, string>> DnnModels { get; }
            = new Dictionary<DnnModel, string>
            {
                { DnnModel.Default, "default (slower)" },
                { DnnModel.Lite, "lite (faster)" },
            }.ToList();

        public float SmoothingFactor
        {
            get => smoothingFactor;
            set => SetPropertyValue(ref smoothingFactor, value, nameof(SmoothingFactor));
        }
        private float smoothingFactor = BodyTracking.Tracker.DefaultSmoothingFactor;

        public BodyTracking.SensorOrientation SensorOrientation
        {
            get => sensorOrientation;
            set => SetPropertyValue(ref sensorOrientation, value, nameof(SensorOrientation));
        }
        private BodyTracking.SensorOrientation sensorOrientation = BodyTracking.SensorOrientation.Default;

        public IReadOnlyList<KeyValuePair<BodyTracking.SensorOrientation, string>> SensorOrientations { get; }
            = new Dictionary<BodyTracking.SensorOrientation, string>
            {
                { BodyTracking.SensorOrientation.Default, "Default" },
                { BodyTracking.SensorOrientation.Clockwise90, "90° clockwise" },
                { BodyTracking.SensorOrientation.Counterclockwise90, "90° counter-clockwise" },
                { BodyTracking.SensorOrientation.Flip180, "Upside-down" },
            }.ToList();

        #endregion
    }
}
