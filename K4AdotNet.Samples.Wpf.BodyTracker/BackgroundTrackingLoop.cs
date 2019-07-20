using K4AdotNet.BodyTracking;
using K4AdotNet.Sensor;
using System;
using System.Threading;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BackgroundTrackingLoop : IDisposable
    {
        private readonly Tracker tracker;

        public BackgroundTrackingLoop(ref Calibration calibration)
        {
            tracker = new Tracker(ref calibration);
            new Thread(BackgroundLoop) { IsBackground = true }.Start();
        }

        public void Dispose()
            => tracker.Dispose();

        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        public event EventHandler<FailedEventArgs> Failed;

        public void Enqueue(Capture capture)
            => tracker.EnqueueCapture(capture);

        private void BackgroundLoop()
        {
            try
            {
                while (!tracker.IsDisposed)
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
            catch (ObjectDisposedException)
            { }
            catch (Exception exc)
            {
                Failed?.Invoke(this, new FailedEventArgs(exc));
            }
        }
    }
}
