using System;
using System.Threading;

namespace K4AdotNet.BodyTracking
{
    public sealed class Tracker : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.TrackerHandle> handle;
        private volatile int queueSize;

        public Tracker(ref Sensor.Calibration calibration)
        {
            if (NativeApi.TrackerCreate(ref calibration, out var handle) != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new BodyTrackingException("Cannot create body tracking infrastructure");
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            queueSize = 0;
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
            => handle.Dispose();

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public void Shutdown()
        {
            NativeApi.TrackerShutdown(handle.Value);
            queueSize = 0;
        }

        public int QueueSize => queueSize;

        public event EventHandler QueueSizeIncreased;

        public event EventHandler QueueSizeDecreased;

        public bool TryEnqueueCapture(Sensor.Capture capture, Timeout timeout = default(Timeout))
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));

            var res = NativeApi.TrackerEnqueueCapture(handle.ValueNotDisposed, Sensor.Capture.ToHandle(capture), timeout);
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
            var res = NativeApi.TrackerPopResult(handle.ValueNotDisposed, out var bodyFrameHandle, timeout);
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
