using K4AdotNet.Sensor;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Body tracker. The main class in Body Tracking component.</summary>
    /// <remarks><para>
    /// Processing is organized as pipeline with queues.
    /// Use <see cref="TryEnqueueCapture(Capture, Timeout)"/> to add new capture to processing pipeline.
    /// Use <see cref="TryPopResult(out BodyFrame, Timeout)"/> to extract processed capture and body data from pipeline.
    /// </para></remarks>
    /// <seealso cref="BodyFrame"/>
    public sealed class Tracker : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.TrackerHandle> handle;   // this class is an wrapper around this handle
        private volatile int queueSize;                                                     // captures in queue
        private volatile bool isDisposed;
        private float temporalSmoothingFactor = DefaultSmoothingFactor;
        private readonly object temporalSmoothingFactorSync = new object();

        /// <summary>Creates a body tracker.</summary>
        /// <param name="calibration">The sensor calibration that will be used for capture processing.</param>
        /// <param name="config">The configuration we want to run the tracker in. This can be initialized with <see cref="TrackerConfiguration.Default"/>.</param>
        /// <remarks><para>
        /// Under the hood Body Tracking runtime will be initialized during the first call of this constructor.
        /// It is rather time consuming operation: initialization of ONNX runtime, loading and parsing of neural network model, etc.
        /// For this reason, it is recommended to initialize Body Tracking runtime in advance: <see cref="Sdk.TryInitializeBodyTrackingRuntime(TrackerProcessingMode, out string)"/>.
        /// </para><para>
        /// Also, Body Tracking runtime must be available in one of the following locations:
        /// directory with executable file,
        /// directory with <c>K4AdotNet</c> assembly,
        /// installation directory of Body Tracking SDK under <c>Program Files</c>.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Invalid value of <paramref name="calibration"/>: <see cref="Calibration.DepthMode"/> cannot be <see cref="DepthMode.Off"/> and <see cref="DepthMode.PassiveIR"/>.
        /// Because depth data is required for body tracking.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Invalid/unsupported characters in <see cref="TrackerConfiguration.ModelPath"/> of <paramref name="config"/>.
        /// </exception>
        /// <exception cref="BodyTrackingException">
        /// Unable to find/initialize Body Tracking runtime
        /// or wrong path to DNN model specified in <paramref name="config"/>.
        /// </exception>
        /// <seealso cref="Sdk.IsBodyTrackingRuntimeAvailable(out string)"/>
        /// <seealso cref="Sdk.TryInitializeBodyTrackingRuntime(TrackerProcessingMode, out string)"/>
        public Tracker(in Calibration calibration, TrackerConfiguration config)
        {
            if (!calibration.DepthMode.HasDepth())
                throw new ArgumentOutOfRangeException(nameof(calibration) + "." + nameof(calibration.DepthMode));
            if (!string.IsNullOrWhiteSpace(config.ModelPath) && (config.ModelPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || !Helpers.IsAsciiCompatible(config.ModelPath)))
                throw new ArgumentException($"Path \"{config.ModelPath}\" contains invalid/unsupported characters.", nameof(config) + "." + nameof(config.ModelPath));

            DepthMode = calibration.DepthMode;

            if (!Sdk.TryCreateTrackerHandle(in calibration, config, out var handle, out var message))
                throw new BodyTrackingException(message);

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object? sender, EventArgs e)
        {
            queueSize = 0;
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to free unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
        {
            isDisposed = true;
            if (!handle.IsDisposed)
                Shutdown();
            handle.Dispose();
        }

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => isDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Shutdown the tracker so that no further capture can be added to the input queue.</summary>
        /// <remarks><para>
        /// Once the tracker is shutdown, <see cref="TryEnqueueCapture(Capture, Timeout)"/> method will always immediately return failure.
        /// </para><para>
        /// If there are remaining captures in the tracker queue after the tracker is shutdown, <see cref="TryPopResult(out BodyFrame, Timeout)"/> can
        /// still return successfully. Once the tracker queue is empty, the <see cref="TryPopResult(out BodyFrame, Timeout)"/> call will always immediately
        /// return failure.
        /// </para><para>
        /// This function may be called while another thread is blocking in <see cref="TryEnqueueCapture(Capture, Timeout)"/> or <see cref="TryPopResult(out BodyFrame, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function raising an exception.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">Object was disposed.</exception>
        public void Shutdown()
            => NativeApi.TrackerShutdown(handle.ValueNotDisposed);

        /// <summary>Depth mode for which this tracker was created.</summary>
        public DepthMode DepthMode { get; }

        /// <summary>How many captures are there in the processing pipeline?</summary>
        /// <seealso cref="MaxQueueSize"/>
        /// <seealso cref="IsQueueFull"/>
        public int QueueSize => queueSize;

        /// <summary>Is processing pipeline full?</summary>
        /// <seealso cref="QueueSize"/>
        /// <seealso cref="MaxQueueSize"/>
        /// <seealso cref="TryEnqueueCapture(Capture, Timeout)"/>
        public bool IsQueueFull => queueSize >= MaxQueueSize;

        /// <summary>Raised on increasing of <see cref="QueueSize"/>.</summary>
        public event EventHandler? QueueSizeIncreased;

        /// <summary>Raised on decreasing of <see cref="QueueSize"/>.</summary>
        public event EventHandler? QueueSizeDecreased;

        /// <summary>Temporal smoothing across frames (0 - 1). Default value is <see cref="DefaultSmoothingFactor"/>.</summary>
        /// <remarks>
        /// Set between 0 for no smoothing and 1 for full smoothing.
        /// Less smoothing will increase the responsiveness of the
        /// detected skeletons but will cause more positional and orientation jitters.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than zero or is greater than one.</exception>
        /// <exception cref="ObjectDisposedException">Object was disposed.</exception>
        public float TemporalSmoothingFactor
        {
            get
            {
                lock (temporalSmoothingFactorSync)
                {
                    return temporalSmoothingFactor;
                }
            }

            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(TemporalSmoothingFactor));

                lock (temporalSmoothingFactorSync)
                {
                    NativeApi.TrackerSetTemporalSmoothing(handle.ValueNotDisposed, value);
                    temporalSmoothingFactor = value;
                }
            }
        }

        /// <summary>Adds a Azure Kinect sensor capture to the tracker input queue to generate its body tracking result asynchronously.</summary>
        /// <param name="capture">It should contain the depth and IR data compatible with <see cref="DepthMode"/> for this function to work. Not <see langword="null"/>.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting to add the sensor capture to the tracker process queue.
        /// Default value is <see cref="Timeout.NoWait"/>, which means checking of the status without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until the capture is added to the process queue.
        /// </param>
        /// <returns>
        /// <see langword="true"/> - if a sensor capture is successfully added to the processing queue.
        /// <see langword="false"/> - if the queue is still full (see <see cref="IsQueueFull"/> property) before the <paramref name="timeout"/> elapses.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="capture"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="capture"/> doesn't contain depth and/or IR data compatible with <see cref="DepthMode"/>.</exception>
        /// <exception cref="ObjectDisposedException">Object was disposed before this call or has been disposed during this call.</exception>
        /// <exception cref="BodyTrackingException">Cannot add capture to the tracker for some unknown reason. See logs for details.</exception>
        public bool TryEnqueueCapture(Capture capture, Timeout timeout = default)
        {
            if (capture is null)
                throw new ArgumentNullException(nameof(capture));

            if (isDisposed)
                throw new ObjectDisposedException(nameof(Tracker));

            var res = NativeApi.TrackerEnqueueCapture(handle.ValueNotDisposed, Capture.ToHandle(capture), timeout);
            if (res == NativeCallResults.WaitResult.Timeout)
                return false;
            if (res == NativeCallResults.WaitResult.Failed)
            {
                // to throw ObjectDisposedException() if failure is a result of disposing
                if (isDisposed)
                    throw new ObjectDisposedException(nameof(Tracker));
                handle.CheckNotDisposed();

                using (var depthImage = capture.DepthImage)
                {
                    if (depthImage == null)
                    {
                        throw new ArgumentException(
                            "Capture should contain the depth data.",
                            nameof(capture));
                    }
                    if (depthImage.Format != ImageFormat.Depth16)
                    {
                        throw new ArgumentException(
                            $"Invalid format of depth data in capture: expected {ImageFormat.Depth16} but was {depthImage.Format}.",
                            nameof(capture));
                    }
                    if (depthImage.WidthPixels != DepthMode.WidthPixels() || depthImage.HeightPixels != DepthMode.HeightPixels())
                    {
                        throw new ArgumentException(
                            $"Invalid resolution of depth data in capture: expected {DepthMode.WidthPixels()}x{DepthMode.HeightPixels()} pixels but was {depthImage.WidthPixels}x{depthImage.HeightPixels} pixels.",
                            nameof(capture));
                    }
                }

                using (var irImage = capture.IRImage)
                {
                    if (irImage == null)
                    {
                        throw new ArgumentException(
                            "Capture should contain the IR data.",
                            nameof(capture));
                    }
                    if (irImage.Format != ImageFormat.IR16)
                    {
                        throw new ArgumentException(
                            $"Invalid format of IR data in capture: expected {ImageFormat.IR16} but was {irImage.Format}.",
                            nameof(capture));
                    }
                    if (irImage.WidthPixels != DepthMode.WidthPixels() || irImage.HeightPixels != DepthMode.HeightPixels())
                    {
                        throw new ArgumentException(
                            $"Invalid resolution of IR data in capture: expected {DepthMode.WidthPixels()}x{DepthMode.HeightPixels()} pixels but was {irImage.WidthPixels}x{irImage.HeightPixels} pixels.",
                            nameof(capture));
                    }
                }

                throw new BodyTrackingException("Cannot add new capture to body tracking pipeline. See logs for details.");
            }

            Interlocked.Increment(ref queueSize);
            QueueSizeIncreased?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>Equivalent to call of <see cref="TryEnqueueCapture(Capture, Timeout)"/> with infinite timeout: <see cref="Timeout.Infinite"/>.</summary>
        /// <param name="capture">It should contain the depth data compatible with <see cref="DepthMode"/> for this function to work. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="capture"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="capture"/> doesn't contain depth and/or IR data compatible with <see cref="DepthMode"/>.</exception>
        /// <exception cref="ObjectDisposedException">Object was disposed before this call or has been disposed during this call.</exception>
        /// <exception cref="BodyTrackingException">Cannot add capture to the tracker for some unknown reason. See logs for details.</exception>
        public void EnqueueCapture(Capture capture)
        {
            var res = TryEnqueueCapture(capture, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
        }

        /// <summary>Gets the next available body frame.</summary>
        /// <param name="bodyFrame">
        /// If successful this contains object with body data (don't forget to free this object by calling <see cref="BodyFrame.Dispose"/>),
        /// otherwise - <see langword="null"/>.
        /// </param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the body frame.
        /// Default value is <see cref="Timeout.NoWait"/>, which means checking of the status without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until the body frame becomes available.
        /// </param>
        /// <returns>
        /// <see langword="true"/> - if a body frame is returned,
        /// <see langword="false"/> - if a body frame is not available before the timeout elapses.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Object was disposed before this call or has been disposed during this call.</exception>
        /// <exception cref="BodyTrackingException">Cannot get body frame for some unknown reason. See logs for details.</exception>
        /// <seealso cref="PopResult"/>
        public bool TryPopResult([NotNullWhen(returnValue: true)] out BodyFrame? bodyFrame, Timeout timeout = default)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(Tracker));

            var res = NativeApi.TrackerPopResult(handle.ValueNotDisposed, out var bodyFrameHandle, timeout);
            if (res == NativeCallResults.WaitResult.Timeout)
            {
                bodyFrame = null;
                return false;
            }
            if (res == NativeCallResults.WaitResult.Failed)
            {
                // to throw ObjectDisposedException() if failure is a result of disposing
                if (isDisposed)
                    throw new ObjectDisposedException(nameof(Tracker));
                handle.CheckNotDisposed(); 

                throw new BodyTrackingException("Cannot extract tracking result from body tracking pipeline. See logs for details.");
            }

            Interlocked.Decrement(ref queueSize);
            QueueSizeDecreased?.Invoke(this, EventArgs.Empty);

            bodyFrame = BodyFrame.Create(bodyFrameHandle);
            return bodyFrame != null;
        }

        /// <summary>Equivalent to call of <see cref="TryPopResult(out BodyFrame, Timeout)"/> with infinite timeout: <see cref="Timeout.Infinite"/>.</summary>
        /// <returns>Enqueued body frame. Not <see langword="null"/>. Don't forget to call <see cref="BodyFrame.Dispose"/> for returned object after usage.</returns>
        /// <exception cref="ObjectDisposedException">Object was disposed before this call or has been disposed during this call.</exception>
        /// <exception cref="BodyTrackingException">Cannot get body frame for some unknown reason. See logs for details.</exception>
        /// <seealso cref="TryPopResult(out BodyFrame, Timeout)"/>
        public BodyFrame PopResult()
        {
            var res = TryPopResult(out var bodyFrame, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
            return bodyFrame!;
        }

        /// <summary>Max amount of captures that can be simultaneously in processing pipeline.</summary>
        /// <seealso cref="IsQueueFull"/>
        public static readonly int MaxQueueSize = NativeApi.MAX_TRACKING_QUEUE_SIZE;

        /// <summary>The default tracker temporal smoothing factor.</summary>
        public static readonly float DefaultSmoothingFactor = NativeApi.DEFAULT_TRACKER_SMOOTHING_FACTOR;
    }
}
