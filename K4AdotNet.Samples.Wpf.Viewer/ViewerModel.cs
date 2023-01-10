using K4AdotNet.Sensor;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class ViewerModel : ViewModelBase, IDisposable
    {
        private readonly BackgroundReadingLoop? readingLoop;

        // To transform depth map to color camera plane
        private readonly Transformation? transformation;
        private readonly Image? depthOverColorImage;

        // To visualize images received from Capture
        private readonly ImageVisualizer? colorImageVisualizer;
        private readonly ImageVisualizer? depthImageVisualizer;
        private readonly ImageVisualizer? depthOverColorImageVisualizer;
        private readonly ImageVisualizer? irImageVisualizer;

        // To calculate actual fps
        private readonly ActualFpsCalculator colorFps = new();
        private readonly ActualFpsCalculator depthFps = new();
        private readonly ActualFpsCalculator irFps = new();

        // For designer
        public ViewerModel()
            : base()
        {
            Title = "Kinect for Azure #123456";
            DepthColumnWidth = IRColumnWidth = ColorColumnWidth = new GridLength(1, GridUnitType.Star);
        }

        public ViewerModel(IApp app, BackgroundReadingLoop readingLoop)
            : base(app)
        {
            this.readingLoop = readingLoop;
            readingLoop.CaptureReady += ReadingLoop_CaptureReady;
            readingLoop.Failed += ReadingLoop_Failed;

            Title = readingLoop.ToString();

            var colorRes = readingLoop.ColorResolution;
            var depthMode = readingLoop.DepthMode;

            // Image visualizers for color
            if (colorRes != ColorResolution.Off)
            {
                colorImageVisualizer = ImageVisualizer.CreateForColorBgra(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());

                if (depthMode.HasDepth())
                {
                    readingLoop.GetCalibration(out var calibration);
                    transformation = calibration.CreateTransformation();
                    depthOverColorImage = new(ImageFormat.Depth16, colorRes.WidthPixels(), colorRes.HeightPixels());
                    depthOverColorImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());
                }

            }

            // Image visualizers for depth and IR (if any)
            if (depthMode.HasDepth())
                depthImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());
            if (depthMode.HasPassiveIR())
                irImageVisualizer = ImageVisualizer.CreateForIR(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());

            // Proportions between columns
            if (colorRes != ColorResolution.Off && depthMode.HasPassiveIR())
            {
                IRColumnWidth = new(irImageVisualizer!.WidthPixels, GridUnitType.Star);
                DepthColumnWidth = depthMode.HasDepth() ? IRColumnWidth : new GridLength(0, GridUnitType.Pixel);
                ColorColumnWidth = new(
                    irImageVisualizer.HeightPixels * colorImageVisualizer!.WidthPixels / colorImageVisualizer.HeightPixels,
                    GridUnitType.Star);
            }
            else if (colorRes != ColorResolution.Off)
            {
                DepthColumnWidth = IRColumnWidth = new(0, GridUnitType.Pixel);
                ColorColumnWidth = new(1, GridUnitType.Star);
            }
            else
            {
                IRColumnWidth = new(1, GridUnitType.Star);
                ColorColumnWidth = new(0, GridUnitType.Pixel);
                DepthColumnWidth = depthMode.HasDepth() ? IRColumnWidth : ColorColumnWidth;
            }
        }

        private void ReadingLoop_Failed(object? sender, FailedEventArgs e)
            => dispatcher.BeginInvoke(new Action(() => app!.ShowErrorMessage(e.Exception.Message)));

        private void ReadingLoop_CaptureReady(object? sender, CaptureReadyEventArgs e)
        {
            using (var colorImage = e.Capture?.ColorImage)
            {
                var was = colorImageVisualizer?.Update(colorImage);
                UpdateFpsIfNeeded(was, colorFps, nameof(ColorFps));
            }

            using (var depthImage = e.Capture?.DepthImage)
            {
                var was = depthImageVisualizer?.Update(depthImage);
                UpdateFpsIfNeeded(was, depthFps, nameof(DepthFps));

                if (depthImage != null && transformation != null && depthOverColorImage != null && depthOverColorImageVisualizer != null)
                {
                    // Object can be disposed from different thread
                    // As a result depthOverColorImage may be disposed while we're working with it
                    // To protect from such scenario, keep reference to it
                    using (var depthOverColorImageRef = depthOverColorImage.DuplicateReference())
                    {
                        transformation.DepthImageToColorCamera(depthImage, depthOverColorImageRef);
                        depthOverColorImageVisualizer?.Update(depthOverColorImageRef);
                    }
                }
            }

            using (var irImage = e.Capture?.IRImage)
            {
                var was = irImageVisualizer?.Update(irImage);
                UpdateFpsIfNeeded(was, irFps, nameof(IRFps));
            }
        }

        public void Dispose()
        {
            if (readingLoop != null)
            {
                readingLoop.Failed -= ReadingLoop_Failed;
                readingLoop.CaptureReady -= ReadingLoop_CaptureReady;
                readingLoop.Dispose();
            }

            transformation?.Dispose();
            depthOverColorImage?.Dispose();
        }

        public void Run()
            => readingLoop?.Run();

        public string? Title { get; }

        public BitmapSource? ColorImageSource => colorImageVisualizer?.ImageSource;
        public BitmapSource? DepthImageSource => depthImageVisualizer?.ImageSource;
        public BitmapSource? DepthOverColorImageSource => depthOverColorImageVisualizer?.ImageSource;
        public BitmapSource? IRImageSource => irImageVisualizer?.ImageSource;

        public string ColorResolutionInfo => FormatResolution(colorImageVisualizer);
        public string DepthResolutionInfo => FormatResolution(depthImageVisualizer);
        public string IRResolutionInfo => FormatResolution(irImageVisualizer);

        private static string FormatResolution(ImageVisualizer? imageVisualizer)
            => imageVisualizer != null ? $"{imageVisualizer.WidthPixels}x{imageVisualizer.HeightPixels}" : "0x0";

        public string ColorFps => FormatFps(colorFps);
        public string DepthFps => FormatFps(depthFps);
        public string IRFps => FormatFps(irFps);

        private static string FormatFps(ActualFpsCalculator fps)
        {
            var value = fps.FramesPerSecond;
            return value > float.Epsilon ? value.ToString("0.0") : string.Empty;
        }

        private void UpdateFpsIfNeeded(bool? wasFrame, ActualFpsCalculator fps, string nameOfProperty)
        {
            if (wasFrame == true)
            {
                if (fps.RegisterFrame())
                {
                    RaisePropertyChanged(nameOfProperty);
                }
            }
        }

        public double DepthOverColorOpacity
        {
            get => depthOverColorOpacity;

            set
            {
                if (depthOverColorOpacity != value)
                {
                    depthOverColorOpacity = value;
                    RaisePropertyChanged(nameof(DepthOverColorOpacity));
                }
            }
        }
        private double depthOverColorOpacity;

        public double DepthMaxVisibleDistance
        {
            get => (depthImageVisualizer?.VisualizationParameter ?? 0) / 1000.0;
            set
            {
                if (DepthMaxVisibleDistance != value && depthImageVisualizer != null)
                {
                    depthImageVisualizer.VisualizationParameter = (int)(value * 1000);
                    RaisePropertyChanged(nameof(DepthMaxVisibleDistance));
                }
            }
        }

        public double IRBrightnessCorrection
        {
            get => irImageVisualizer?.VisualizationParameter ?? 4;
            set
            {
                if (IRBrightnessCorrection != value && irImageVisualizer != null)
                {
                    irImageVisualizer.VisualizationParameter = (int)value;
                    RaisePropertyChanged(nameof(IRBrightnessCorrection));
                }
            }
        }

        public GridLength DepthColumnWidth { get; }
        public GridLength IRColumnWidth { get; }
        public GridLength ColorColumnWidth { get; }
    }
}
