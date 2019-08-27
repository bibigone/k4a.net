using K4AdotNet.Sensor;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    internal class ProcessingViewModel : ViewModelBase, IDisposable
    {
        private volatile bool isDisposed;
        private readonly BackgroundReadingLoop readingLoop;
        private readonly Transformation transformation;
        private readonly Processor processor;
        private readonly Image depthImage;
        private readonly ManualResetEventSlim needUpdateBitmap = new ManualResetEventSlim(false);

        public ProcessingViewModel(BackgroundReadingLoop readingLoop, IApp app)
            : base(app)
        {
            this.readingLoop = readingLoop ?? throw new ArgumentNullException(nameof(readingLoop));

            readingLoop.GetCalibration(out var calibration);
            transformation = calibration.CreateTransformation();

            var frameWidth = readingLoop.ColorResolution.WidthPixels();
            var frameHeight = readingLoop.ColorResolution.HeightPixels();
            depthImage = new Image(ImageFormat.Depth16, frameWidth, frameHeight);
            bitmap = app.Dispatcher.Invoke(new Func<WriteableBitmap>(
                () => new WriteableBitmap(frameWidth, frameHeight, 96, 96, PixelFormats.Bgra32, null)));

            processor = new Processor(frameWidth, frameHeight)
            {
                DepthLimitMillimeters = 1000,
                UnknownDepthIsBackground = true,
                BackgroundOpacity = 127,
            };

            readingLoop.CaptureReady += ReadingLoop_CaptureReady;
            readingLoop.Failed += ReadingLoop_Failed;
            processor.ImageUpdated += Processor_ImageUpdated;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                readingLoop.CaptureReady -= ReadingLoop_CaptureReady;
                readingLoop.Failed -= ReadingLoop_Failed;
                readingLoop.Dispose();

                processor.ImageUpdated -= Processor_ImageUpdated;
                processor.Stop();
                processor.Dispose();

                depthImage.Dispose();
                needUpdateBitmap.Dispose();
            }
        }


        public ImageSource Image => bitmap;
        private readonly WriteableBitmap bitmap;

        public void Run()
        {
            try
            {
                processor.Start();
            }
            catch (InvalidOperationException) { }

            readingLoop.Run();
        }

        private void ReadingLoop_CaptureReady(object sender, CaptureReadyEventArgs e)
        {
            using (e.Capture)
            {
                if (e.Capture.ColorImage != null && e.Capture.DepthImage != null)
                {
                    transformation.DepthImageToColorCamera(e.Capture.DepthImage, depthImage);
                    var frameData = new FrameData(e.Capture.ColorImage.DuplicateReference(), depthImage.DuplicateReference());
                    processor.Enqueue(frameData);
                }
            }
        }

        private void ReadingLoop_Failed(object sender, FailedEventArgs e)
        {
            app.Dispatcher.BeginInvoke(new Action(() => app.ShowErrorMessage(e.Exception.Message, "Capture failed")));
        }

        private void Processor_ImageUpdated(object sender, EventArgs e)
        {
            needUpdateBitmap.Set();
            bitmap.Dispatcher.BeginInvoke(new Action(UpdateBitmap), DispatcherPriority.Render);
        }

        private unsafe void UpdateBitmap()
        {
            if (!isDisposed && needUpdateBitmap.IsSet)
            {
                needUpdateBitmap.Reset();

                bitmap.Lock();
                processor.ReadImage(image =>
                    Buffer.MemoryCopy(image.Buffer.ToPointer(), bitmap.BackBuffer.ToPointer(),
                                      bitmap.BackBufferStride * bitmap.PixelHeight, image.SizeBytes));
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                bitmap.Unlock();
            }
        }
    }
}
