using K4AdotNet.BodyTracking;
using K4AdotNet.Sensor;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class TrackerModel : ViewModelBase, IDisposable
    {
        private readonly Calibration calibration;
        private readonly BackgroundReadingLoop readingLoop;
        private readonly BackgroundTrackingLoop trackingLoop;

        // To visualize images received from Capture
        private readonly ImageVisualizer colorImageVisualizer;
        private readonly ImageVisualizer depthImageVisualizer;
        // To visualize skeletons
        private readonly SkeletonVisualizer depthSkeletonVisualizer;
        private readonly SkeletonVisualizer colorSkeletonVisualizer;
        // To transform body index map from depth camera to color camera
        private readonly BodyIndexMapTransformation bodyIndexMapTransformation;

        private readonly ActualFpsCalculator actualFps = new ActualFpsCalculator();

        // For designer
        public TrackerModel()
            : base()
        {
            Title = "Kinect for Azure #123456";
            DepthColumnWidth = ColorColumnWidth = new GridLength(1, GridUnitType.Star);
        }

        public TrackerModel(IApp app, BackgroundReadingLoop readingLoop)
            : base(app)
        {
            // try to create tracking loop first
            readingLoop.GetCalibration(out calibration);
            trackingLoop = new BackgroundTrackingLoop(ref calibration);
            trackingLoop.BodyFrameReady += TrackingLoop_BodyFrameReady;
            trackingLoop.Failed += BackgroundLoop_Failed;

            this.readingLoop = readingLoop;
            readingLoop.CaptureReady += ReadingLoop_CaptureReady;
            readingLoop.Failed += BackgroundLoop_Failed;

            Title = readingLoop.ToString();

            // Image and skeleton visualizers for depth
            var depthMode = readingLoop.DepthMode;
            depthImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());
            depthSkeletonVisualizer = new SkeletonVisualizer(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels(), ProjectJointToDepthMap);

            // Image and skeleton visualizers for color
            var colorRes = readingLoop.ColorResolution;
            if (colorRes != ColorResolution.Off)
            {
                colorImageVisualizer = ImageVisualizer.CreateForColorBgra(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());
                colorSkeletonVisualizer = new SkeletonVisualizer(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels(), ProjectJointToColorImage);
                bodyIndexMapTransformation = new BodyIndexMapTransformation(ref calibration);
            }

            // Proportions between columns
            if (colorRes != ColorResolution.Off)
            {
                DepthColumnWidth = new GridLength(depthImageVisualizer.WidthPixels, GridUnitType.Star);
                ColorColumnWidth = new GridLength(
                    depthImageVisualizer.HeightPixels * colorImageVisualizer.WidthPixels / colorImageVisualizer.HeightPixels,
                    GridUnitType.Star);
            }
            else
            {
                DepthColumnWidth = new GridLength(1, GridUnitType.Star);
                ColorColumnWidth = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private Float2? ProjectJointToDepthMap(Joint joint)
            => calibration.Convert3DTo2D(joint.PositionMm, CalibrationGeometry.Depth, CalibrationGeometry.Depth);

        private Float2? ProjectJointToColorImage(Joint joint)
            => calibration.Convert3DTo2D(joint.PositionMm, CalibrationGeometry.Depth, CalibrationGeometry.Color);

        private void BackgroundLoop_Failed(object sender, FailedEventArgs e)
            => dispatcher.BeginInvoke(new Action(() => app.ShowErrorMessage(e.Exception.Message)));

        private void ReadingLoop_CaptureReady(object sender, CaptureReadyEventArgs e)
            => trackingLoop.Enqueue(e.Capture);

        private void TrackingLoop_BodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            if (actualFps.RegisterFrame())
                RaisePropertyChanged(nameof(ActualFps));

            depthSkeletonVisualizer?.Update(e.BodyFrame);
            colorSkeletonVisualizer?.Update(e.BodyFrame);

            using (var capture = e.BodyFrame.Capture)
            {
                using (var depthImage = capture.DepthImage)
                {
                    using (var indexMap = e.BodyFrame.BodyIndexMap)
                    {
                        depthImageVisualizer?.Update(depthImage, indexMap);

                        if (colorImageVisualizer != null)
                        {
                            using (var colorImage = capture.ColorImage)
                            {
                                using (var transformedBodyIndexMap = bodyIndexMapTransformation.ToColor(depthImage, indexMap))
                                {
                                    colorImageVisualizer.Update(colorImage, transformedBodyIndexMap);
                                }
                            }
                        }

                    }
                }
            }
        }

        public void Dispose()
        {
            if (readingLoop != null)
            {
                readingLoop.Failed -= BackgroundLoop_Failed;
                readingLoop.CaptureReady -= ReadingLoop_CaptureReady;
                readingLoop.Dispose();
            }

            if (trackingLoop != null)
            {
                trackingLoop.Failed -= BackgroundLoop_Failed;
                trackingLoop.BodyFrameReady -= TrackingLoop_BodyFrameReady;
                trackingLoop.Dispose();
            }

            bodyIndexMapTransformation?.Dispose();
        }

        public void Run()
            => readingLoop?.Run();

        public string Title { get; }

        public BitmapSource ColorImageSource => colorImageVisualizer?.ImageSource;
        public BitmapSource DepthImageSource => depthImageVisualizer?.ImageSource;

        public ImageSource DepthSkeletonImageSource => depthSkeletonVisualizer?.ImageSource;
        public ImageSource ColorSkeletonImageSource => colorSkeletonVisualizer?.ImageSource;

        public byte DepthNonBodyPixelsAlpha
        {
            get => depthImageVisualizer?.NonBodyAlphaValue ?? byte.MaxValue;
            set
            {
                if (value != DepthNonBodyPixelsAlpha && depthImageVisualizer != null)
                {
                    depthImageVisualizer.NonBodyAlphaValue = value;
                    RaisePropertyChanged(nameof(DepthNonBodyPixelsAlpha));
                }
            }
        }

        public byte ColorNonBodyPixelsAlpha
        {
            get => colorImageVisualizer?.NonBodyAlphaValue ?? byte.MaxValue;
            set
            {
                if (value != ColorNonBodyPixelsAlpha && colorImageVisualizer != null)
                {
                    colorImageVisualizer.NonBodyAlphaValue = value;
                    RaisePropertyChanged(nameof(ColorNonBodyPixelsAlpha));
                }
            }
        }

        public string ActualFps => FormatFps(actualFps);

        private static string FormatFps(ActualFpsCalculator fps)
        {
            var value = fps.FramesPerSecond;
            return value > float.Epsilon ? value.ToString("0.0") : string.Empty;
        }

        public GridLength DepthColumnWidth { get; }
        public GridLength ColorColumnWidth { get; }
    }
}
