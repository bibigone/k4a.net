using System;
using System.Threading;

namespace K4AdotNet.BodyTracking
{
    public sealed class Tracker : IDisposablePlus
    {
        // Current version of Body Tracking SDK does not support creation of multiple instances.
        // Attempt to create the second one simply crashes the application.
        private static volatile int instancesCounter;

        private readonly NativeHandles.HandleWrapper<NativeHandles.TrackerHandle> handle;
        private volatile int queueSize;

        public Tracker(ref Sensor.Calibration calibration)
        {
            if (Interlocked.Increment(ref instancesCounter) != 1)
            {
                Interlocked.Decrement(ref instancesCounter);
                throw new NotSupportedException("Oops! Current version of Body Tracking SDK does not support creation of multiple body tracker. Sorry!");
            }

            if (NativeApi.TrackerCreate(ref calibration, out var handle) != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new BodyTrackingException("Cannot initialize body tracking infrastructure");

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            Interlocked.Decrement(ref instancesCounter);
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

        public bool IsQueueFull => queueSize >= MaxQueueSize;

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
            {
                handle.CheckNotDisposed();      // to throw ObjectDisposedException() if failure is a result of disposing
                throw new BodyTrackingException("Cannot add new capture to body tracking pipeline");
            }

            Interlocked.Increment(ref queueSize);
            QueueSizeIncreased?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public void EnqueueCapture(Sensor.Capture capture)
        {
            var res = TryEnqueueCapture(capture, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
        }

        public bool TryPopResult(out BodyFrame bodyFrame, Timeout timeout = default(Timeout))
        {
            var res = NativeApi.TrackerPopResult(handle.ValueNotDisposed, out var bodyFrameHandle, timeout);
            if (res == NativeCallResults.WaitResult.Timeout)
            {
                bodyFrame = null;
                return false;
            }
            if (res == NativeCallResults.WaitResult.Failed)
            {
                handle.CheckNotDisposed();      // to throw ObjectDisposedException() if failure is a result of disposing
                throw new BodyTrackingException("Cannot extract tracking result from body tracking pipeline");
            }

            Interlocked.Decrement(ref queueSize);
            QueueSizeDecreased?.Invoke(this, EventArgs.Empty);

            bodyFrame = BodyFrame.Create(bodyFrameHandle);
            return bodyFrame != null;
        }

        public BodyFrame PopResult()
        {
            var res = TryPopResult(out var bodyFrame, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
            return bodyFrame;
        }

        public static readonly int MaxQueueSize = NativeApi.MAX_TRACKING_QUEUE_SIZE;
    }
}
