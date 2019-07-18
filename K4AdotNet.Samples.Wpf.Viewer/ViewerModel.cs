using K4AdotNet.Sensor;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class ViewerModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher dispatcher;
        private readonly BackgroundReadLoop readLoop;

        private readonly Transformation transformation;
        private readonly Image depthOverColorImage;

        private readonly ImageVisualizer colorImageVisualizer;
        private readonly ImageVisualizer depthImageVisualizer;
        private readonly ImageVisualizer depthOverColorImageVisualizer;
        private readonly ImageVisualizer irImageVisualizer;

        private readonly ActualFpsCalculator colorFps = new ActualFpsCalculator();
        private readonly ActualFpsCalculator depthFps = new ActualFpsCalculator();
        private readonly ActualFpsCalculator irFps = new ActualFpsCalculator();

        public ViewerModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            Title = "Kinect for Azure #123456";
            DepthColumnWidth = IRColumnWidth = ColorColumnWidth = new GridLength(1, GridUnitType.Star);
        }

        public ViewerModel(Dispatcher dispatcher, BackgroundReadLoop readLoop)
        {
            this.dispatcher = dispatcher;
            this.readLoop = readLoop;
            readLoop.CaptureReady += ReadLoop_CaptureReady;
            readLoop.Failed += ReadLoop_Failed;

            Title = readLoop.ToString();

            var colorRes = readLoop.ColorResolution;
            var depthMode = readLoop.DepthMode;
            readLoop.GetCalibration(out var calibration);

            if (colorRes != ColorResolution.Off)
            {
                colorImageVisualizer = ImageVisualizer.CreateForColorBgra(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());

                if (depthMode.HasDepth())
                {
                    transformation = new Transformation(ref calibration);
                    depthOverColorImage = new Image(ImageFormat.Depth16, colorRes.WidthPixels(), colorRes.HeightPixels());
                    depthOverColorImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, colorRes.WidthPixels(), colorRes.HeightPixels());
                }

            }
            
            if (depthMode.HasDepth())
                depthImageVisualizer = ImageVisualizer.CreateForDepth(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());
            if (depthMode.HasPassiveIR())
                irImageVisualizer = ImageVisualizer.CreateForIR(dispatcher, depthMode.WidthPixels(), depthMode.HeightPixels());

            if (colorRes != ColorResolution.Off && depthMode.HasPassiveIR())
            {
                IRColumnWidth = new GridLength(irImageVisualizer.WidthPixels, GridUnitType.Star);
                DepthColumnWidth = depthMode.HasDepth() ? IRColumnWidth : new GridLength(0, GridUnitType.Pixel);
                ColorColumnWidth = new GridLength(
                    irImageVisualizer.HeightPixels * colorImageVisualizer.WidthPixels / colorImageVisualizer.HeightPixels,
                    GridUnitType.Star);
            }
            else if (colorRes != ColorResolution.Off)
            {
                DepthColumnWidth = IRColumnWidth = new GridLength(0, GridUnitType.Pixel);
                ColorColumnWidth = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                DepthColumnWidth = IRColumnWidth = new GridLength(1, GridUnitType.Star);
                ColorColumnWidth = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void ReadLoop_Failed(object sender, FailedEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void ReadLoop_CaptureReady(object sender, CaptureReadyEventArgs e)
        {
            using (var colorImage = e.Capture.ColorImage)
            {
                var was = colorImageVisualizer?.Update(colorImage);
                UpdateFpsIfNeeded(was, colorFps, nameof(ColorFps));
            }

            using (var depthImage = e.Capture.DepthImage)
            {
                var was = depthImageVisualizer?.Update(depthImage);
                UpdateFpsIfNeeded(was, depthFps, nameof(DepthFps));

                if (depthImage != null && transformation != null && depthOverColorImage != null && depthOverColorImageVisualizer != null)
                {
                    transformation.DepthImageToColorCamera(depthImage, depthOverColorImage);
                    depthOverColorImageVisualizer?.Update(depthOverColorImage);
                }
            }

            using (var irImage = e.Capture.IRImage)
            {
                var was = irImageVisualizer?.Update(irImage);
                UpdateFpsIfNeeded(was, irFps, nameof(IRFps));
            }
        }

        public void Dispose()
        {
            if (readLoop != null)
            {
                readLoop.Failed -= ReadLoop_Failed;
                readLoop.CaptureReady -= ReadLoop_CaptureReady;
                readLoop.Dispose();
            }

            transformation?.Dispose();
            depthOverColorImage?.Dispose();
        }

        public void Run()
            => readLoop?.Run();

        public string Title { get; }

        public BitmapSource ColorImageSource => colorImageVisualizer?.ImageSource;
        public BitmapSource DepthImageSource => depthImageVisualizer?.ImageSource;
        public BitmapSource DepthOverColorImageSource => depthOverColorImageVisualizer?.ImageSource;
        public BitmapSource IRImageSource => irImageVisualizer?.ImageSource;

        public string ColorResolutionInfo => FormatResolution(colorImageVisualizer);
        public string DepthResolutionInfo => FormatResolution(depthImageVisualizer);
        public string IRResolutionInfo => FormatResolution(irImageVisualizer);

        private static string FormatResolution(ImageVisualizer imageVisualizer)
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

        #region INotifyPropertyChanged

        /// <summary>
        /// For UI binding.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (Thread.CurrentThread != dispatcher.Thread)
            {
                // If this method is called from non-UI thread, then redirect it to UI thread
                dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => RaisePropertyChanged(propertyName)));
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
