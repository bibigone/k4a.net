using K4AdotNet.BodyTracking;
using K4AdotNet.Sensor;
using System;
using System.Threading;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BackgroundTrackingLoop : IDisposable
    {
        private readonly Tracker tracker;
        private readonly Thread backgroundThread;
        private volatile bool isRunning;

        public BackgroundTrackingLoop(Calibration calibration, TrackerProcessingMode processingMode, DnnModel dnnModel, SensorOrientation sensorOrientation, float smoothingFactor)
        {
            var config = new TrackerConfiguration
            {
                SensorOrientation = sensorOrientation,
                ProcessingMode = processingMode,
                ModelPath = GetModelPath(dnnModel),
            };
            tracker = new(in calibration.Data, config) { TemporalSmoothingFactor = smoothingFactor };
            isRunning = true;
            backgroundThread = new(BackgroundLoop) { IsBackground = true };
            backgroundThread.Start();
        }

        private static string GetModelPath(DnnModel dnnModel)
        {
            switch (dnnModel)
            {
                case DnnModel.Default: return Sdk.BODY_TRACKING_DNN_MODEL_FILE_NAME;
                case DnnModel.Lite: return Sdk.BODY_TRACKING_DNN_MODEL_LITE_FILE_NAME;
                default: throw new NotSupportedException();
            }
    }

        public void Dispose()
        {
            if (isRunning)
            {
                isRunning = false;
                if (backgroundThread.ThreadState != ThreadState.Unstarted)
                    backgroundThread.Join();
            }

            tracker.Dispose();
        }

        public event EventHandler<BodyFrameReadyEventArgs>? BodyFrameReady;

        public event EventHandler<FailedEventArgs>? Failed;

        public void Enqueue(Capture capture)
            => tracker.EnqueueCapture(capture);

        private void BackgroundLoop()
        {
            try
            {
                while (isRunning)
                {
                    if (tracker.TryPopResult(out var bodyFrame))
                    {
                        using (bodyFrame)
                        {
                            BodyFrameReady?.Invoke(this, new BodyFrameReadyEventArgs(bodyFrame));
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception exc)
            {
                Failed?.Invoke(this, new FailedEventArgs(exc));
            }
        }
    }
}
