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
        private readonly SkeletonVisualizer skeletonVisualizer;

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

            // Image visualizers for depth
            var depthMode = readingLoop.DepthMode;
            depthImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());

            // Image visualizers for color
            var colorRes = readingLoop.ColorResolution;
            if (colorRes != ColorResolution.Off)
                colorImageVisualizer = ImageVisualizer.CreateForColorBgra(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());

            // Skeleton visualization
            skeletonVisualizer = new SkeletonVisualizer(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels(), ProjectJointToDepthMap);

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

        private void BackgroundLoop_Failed(object sender, FailedEventArgs e)
            => app.ShowErrorMessage(e.Exception.Message);

        private void ReadingLoop_CaptureReady(object sender, CaptureReadyEventArgs e)
        {
            trackingLoop.Enqueue(e.Capture);
        }

        private void TrackingLoop_BodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            skeletonVisualizer.Update(e.BodyFrame);

            using (var capture = e.BodyFrame.Capture)
            {
                using (var colorImage = capture.ColorImage)
                {
                    colorImageVisualizer?.Update(colorImage);
                }

                using (var depthImage = capture.DepthImage)
                {
                    depthImageVisualizer?.Update(depthImage);
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
        }

        public void Run()
            => readingLoop?.Run();

        public string Title { get; }

        public BitmapSource ColorImageSource => colorImageVisualizer?.ImageSource;
        public BitmapSource DepthImageSource => depthImageVisualizer?.ImageSource;

        public ImageSource SkeletonImageSource => skeletonVisualizer?.ImageSource;

        public GridLength DepthColumnWidth { get; }
        public GridLength ColorColumnWidth { get; }
    }
}
