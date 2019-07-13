using System;
using System.Threading;

namespace K4AdotNet.BodyTracking
{
    public sealed class Tracker : IDisposable
    {
        private readonly NativeHandles.TrackerHandle handle;
        private volatile int queueSize;

        public Tracker(ref Sensor.Calibration calibration)
        {
            if (NativeApi.TrackerCreate(ref calibration, out handle) != NativeCallResults.Result.Succeeded)
                throw new BodyTrackingException("Cannot create body tracking infrastructure");
        }

        public void Dispose()
        {
            handle.Dispose();
            queueSize = 0;
        }

        public void Shutdown()
        {
            NativeApi.TrackerShutdown(handle);
            queueSize = 0;
        }

        public int QueueSize => queueSize;

        public event EventHandler QueueSizeIncreased;

        public event EventHandler QueueSizeDecreased;

        public bool TryEnqueueCapture(Sensor.Capture capture, Timeout timeout = default(Timeout))
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));

            var res = NativeApi.TrackerEnqueueCapture(handle, Sensor.Capture.ToHandle(capture), timeout);
            if (res == NativeCallResults.WaitResult.Timeout)
                return false;
            if (res == NativeCallResults.WaitResult.Failed)
                throw new BodyTrackingException("Cannot add new capture to body tracking pipeline");

            Interlocked.Increment(ref queueSize);
            QueueSizeIncreased?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public BodyFrame TryPopResult(Timeout timeout = default(Timeout))
        {
            var res = NativeApi.TrackerPopResult(handle, out var bodyFrameHandle, timeout);
            if (res == NativeCallResults.WaitResult.Timeout)
                return null;
            if (res == NativeCallResults.WaitResult.Failed)
                throw new BodyTrackingException("Cannot extract tracking result from body tracking pipeline");

            Interlocked.Decrement(ref queueSize);
            QueueSizeDecreased?.Invoke(this, EventArgs.Empty);

            return BodyFrame.Create(bodyFrameHandle);
        }

        public static readonly int MaxQueueSize = NativeApi.MAX_TRACKING_QUEUE_SIZE;
    }
}
