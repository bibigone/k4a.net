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

        public BackgroundTrackingLoop(ref Calibration calibration, bool cpuOnlyMode, SensorOrientation sensorOrientation, float smoothingFactor)
        {
            var config = new TrackerConfiguration
            {
                SensorOrientation = sensorOrientation,
                ProcessingMode = cpuOnlyMode
                    ? TrackerProcessingMode.Cpu
                    : TrackerProcessingMode.Gpu
            };
            tracker = new Tracker(ref calibration, config) { TemporalSmoothingFactor = smoothingFactor };
            isRunning = true;
            backgroundThread = new Thread(BackgroundLoop) { IsBackground = true };
            backgroundThread.Start();
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

        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        public event EventHandler<FailedEventArgs> Failed;

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
