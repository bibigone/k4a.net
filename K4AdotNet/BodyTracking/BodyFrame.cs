using System;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Azure Kinect body tracking frame.</summary>
    public sealed class BodyFrame
        : IDisposablePlus, IReferenceDuplicatable<BodyFrame>, IEquatable<BodyFrame>
    {
        private readonly ChildrenDisposer children = new ChildrenDisposer();                    // to track returned Image objects
        private readonly NativeHandles.HandleWrapper<NativeHandles.BodyFrameHandle> handle;     // this class is an wrapper around this handle

        private BodyFrame(NativeHandles.BodyFrameHandle handle)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to free unmanaged resources associated with current instance.
        /// </summary>
        /// <remarks><para>
        /// Under the hood, reference counter is decremented on this call. When the references reach zero the unmanaged resources are destroyed.
        /// (Multiple objects of <see cref="BodyFrame"/> can reference one and the same body frame. For details see <see cref="DuplicateReference"/>.)
        /// </para><para>
        /// Can be called multiple times but event <see cref="Disposed"/> will be raised only once.
        /// </para></remarks>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        /// <seealso cref="DuplicateReference"/>
        public void Dispose()
        {
            children.Dispose();
            handle.Dispose();
        }

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Creates new reference to the same unmanaged body frame object.</summary>
        /// <returns>New object that references exactly to the same underlying unmanaged object as original one. Not <see langword="null"/>.</returns>
        /// <remarks>It helps to manage underlying object lifetime and to access capture from different threads and different components of application.</remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        /// <seealso cref="Dispose"/>
        public BodyFrame DuplicateReference()
            => new BodyFrame(handle.ValueNotDisposed.DuplicateReference());

        /// <summary>Gets the body frame's device timestamp.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Microseconds64 DeviceTimestamp => NativeApi.FrameGetDeviceTimestamp(handle.ValueNotDisposed);

        /// <summary>Deprecated in version 0.9.2 of Body Tracking SDK. Please use <see cref="DeviceTimestamp"/> property instead of this one.</summary>
        [Obsolete("Deprecated in version 0.9.2 of Body Tracking SDK. Please use DeviceTimestamp property instead of this one.")]
        public Microseconds64 Timestamp => DeviceTimestamp;

        /// <summary>Gets the number of detected bodies.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int BodyCount => (int)NativeApi.FrameGetNumBodies(handle.ValueNotDisposed);

        /// <summary>Get the original capture that was used to calculate this body frame.</summary>
        /// <remarks><para>
        /// Called when the user has received a body frame handle and wants to access the data contained in it.
        /// </para><para>
        /// It is highly recommended to call <see cref="Sensor.Capture.Dispose"/> for returned capture explicitly:
        /// <code>
        /// using (var capture = bodyFrame.Capture)
        /// {
        ///     // working with capture
        /// }
        /// </code>
        /// But <see cref="BodyFrame"/> object automatically tracks all <see cref="Sensor.Capture"/> objects it returned. And will call <see cref="Sensor.Capture.Dispose"/>
        /// for all of them if client code didn't it.
        /// </para><para>
        /// For this reason, if you want to keep returned <see cref="Sensor.Capture"/> for longer life time than life time of <see cref="BodyFrame"/> object,
        /// use <see cref="Sensor.Capture.DuplicateReference"/> method.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Sensor.Capture Capture => children.Register(Sensor.Capture.Create(NativeApi.FrameGetCapture(handle.ValueNotDisposed)))!;

        /// <summary>Non-a-body value on index body map.</summary>
        /// <seealso cref="BodyIndexMap"/>
        public const byte NotABodyIndexMapPixelValue = byte.MaxValue;

        /// <summary>Gets the body index map: image in <see cref="Sensor.ImageFormat.Custom8"/> format, one byte per pixel, pixel value: zero-based index of a detected body or <see cref="NotABodyIndexMapPixelValue"/> for background pixels.</summary>
        /// <remarks><para>
        /// Body Index map is the body instance segmentation map. Each pixel maps to the corresponding pixel in the
        /// depth image or the IR image. The value for each pixel is byte and represents which body the pixel belongs to. It can be either
        /// background (value <see cref="NotABodyIndexMapPixelValue"/>) or the zero-based index of a detected body.
        /// </para><para>
        /// It is highly recommended to call <see cref="Sensor.Image.Dispose"/> for returned image explicitly:
        /// <code>
        /// using (var bodyIndexMap = bodyFrame.BodyIndexMap)
        /// {
        ///     // working with body index map
        /// }
        /// </code>
        /// But <see cref="BodyFrame"/> object automatically tracks all <see cref="Sensor.Image"/> objects it returned. And will call <see cref="Sensor.Image.Dispose"/>
        /// for all of them if client code didn't it.
        /// </para><para>
        /// For this reason, if you want to keep returned <see cref="Sensor.Image"/> for longer life time than life time of <see cref="BodyFrame"/> object,
        /// use <see cref="Sensor.Image.DuplicateReference"/> method.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Sensor.Image BodyIndexMap => children.Register(Sensor.Image.Create(NativeApi.FrameGetBodyIndexMap(handle.ValueNotDisposed)))!;

        /// <summary>Gets the joint information for a particular person index.</summary>
        /// <param name="bodyIndex">Zero-based index of a tracked body. Must me positive number. Must be less than <see cref="BodyCount"/>.</param>
        /// <param name="skeleton">Output: this contains the body skeleton information.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bodyIndex"/> is less than zero or greater than or equal to <see cref="BodyCount"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        /// <seealso cref="Skeleton"/>
        /// <seealso cref="JointType"/>
        public void GetBodySkeleton(int bodyIndex, out Skeleton skeleton)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            if (NativeApi.FrameGetBodySkeleton(handle.ValueNotDisposed, (uint)bodyIndex, out skeleton) != NativeCallResults.Result.Succeeded)
                throw new BodyTrackingException($"Cannot extract skeletal data for body with index {bodyIndex}");
        }

        /// <summary>Gets the body id for a particular person index.</summary>
        /// <param name="bodyIndex">Zero-based index of a tracked body. Must me positive number. Must be less than <see cref="BodyCount"/>.</param>
        /// <returns>Returns the body id. In case of failures will return <see cref="BodyId.Invalid"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bodyIndex"/> is less than zero or greater than or equal to <see cref="BodyCount"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public BodyId GetBodyId(int bodyIndex)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            return NativeApi.FrameGetBodyId(handle.ValueNotDisposed, (uint)bodyIndex);
        }

        internal static BodyFrame? Create(NativeHandles.BodyFrameHandle? bodyFrameHandle)
            => bodyFrameHandle != null && !bodyFrameHandle.IsInvalid ? new BodyFrame(bodyFrameHandle) : null;

        #region Equatable

        /// <summary>Two body frames are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="bodyFrame">Another body frame to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both body frames reference to one and the same unmanaged object.</returns>
        public bool Equals(BodyFrame? bodyFrame)
            => !(bodyFrame is null) && bodyFrame.handle.Equals(handle);

        /// <summary>Two body frames are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Some object to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is also <see cref="BodyFrame"/> and they both reference to one and the same unmanaged object.</returns>
        public override bool Equals(object? obj)
            => obj is BodyFrame frame && Equals(frame);

        /// <summary>Uses underlying handle as hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        /// <seealso cref="Equals(BodyFrame)"/>
        public override int GetHashCode()
            => handle.GetHashCode();

        /// <summary>To be consistent with <see cref="Equals(BodyFrame)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(BodyFrame)"/>
        public static bool operator ==(BodyFrame? left, BodyFrame? right)
            => (left is null && right is null) || (!(left is null) && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(BodyFrame)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(BodyFrame)"/>
        public static bool operator !=(BodyFrame? left, BodyFrame? right)
            => !(left == right);

        /// <summary>Convenient (for debugging needs, first of all) string representation of object as an address of unmanaged object in memory.</summary>
        /// <returns><c>{HandleTypeName}#{Address}</c></returns>
        public override string ToString()
            => handle.ToString();

        #endregion
    }
}