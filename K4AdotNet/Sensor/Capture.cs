using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>capture</c> class from <c>k4a.hpp</c>
    //
    /// <summary>A capture represents a set of images that were captured by a Azure Kinect device at approximately the same time.</summary>
    /// <remarks><para>
    /// A capture may have a color, IR, and depth image.A capture may have up to one image of each type.
    /// A capture may have no image for a given type as well.
    /// </para><para>
    /// Captures also store a temperature value which represents the temperature of the device at the time of the capture.
    /// </para><para>
    /// While all the images associated with the capture were collected at approximately the same time, each image has an
    /// individual timestamp which may differ from each other. If the device was configured to capture depth and color images
    /// separated by a delay, <see cref="Device.GetCapture"/> and <see cref="Device.TryGetCapture(out Capture, Timeout)"/> will return
    /// a capture containing both image types separated by the configured delay.
    /// </para></remarks>
    /// <seealso cref="Device.GetCapture"/>
    /// <seealso cref="Device.TryGetCapture(out Capture, Timeout)"/>
    public sealed class Capture
        : IDisposablePlus, IReferenceDuplicatable<Capture>, IEquatable<Capture>
    {
        private readonly ChildrenDisposer children = new ChildrenDisposer();                // to track returned Image objects
        private readonly NativeHandles.HandleWrapper<NativeHandles.CaptureHandle> handle;   // this class is an wrapper around this handle

        /// <summary>Creates an empty capture object.</summary>
        /// <exception cref="InvalidOperationException">
        /// Sensor DK fails to create empty capture object for some reason. For details see logs.
        /// </exception>
        public Capture()
        {
            var res = NativeApi.CaptureCreate(out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new InvalidOperationException("Failed to create blank capture instance");
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        private Capture(NativeHandles.CaptureHandle handle)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        internal static Capture Create(NativeHandles.CaptureHandle handle)
            => handle != null && !handle.IsInvalid ? new Capture(handle) : null;

        /// <summary>
        /// Call this method to free unmanaged resources associated with current instance.
        /// </summary>
        /// <remarks><para>
        /// Under the hood, reference counter is decremented on this call. When the references reach zero the unmanaged resources are destroyed.
        /// (Multiple objects of <see cref="Capture"/> can reference one and the same capture. For details see <see cref="DuplicateReference"/>.)
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
        public event EventHandler Disposed;

        /// <summary>Creates new reference to the same unmanaged capture object.</summary>
        /// <returns>New object that references exactly to the same underlying unmanaged object as original one. Not <see langword="null"/>.</returns>
        /// <remarks>It helps to manage underlying object lifetime and to access capture from different threads and different components of application.</remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        /// <seealso cref="Dispose"/>
        public Capture DuplicateReference()
            => new Capture(handle.ValueNotDisposed.DuplicateReference());

        /// <summary>Get and set the color image associated with the given capture. Can be <see langword="null"/> if the capture doesn't have color data.</summary>
        /// <remarks><para>
        /// It is highly recommended to call <see cref="Image.Dispose"/> for returned image explicitly:
        /// <code>
        /// using (var colorImage = capture.ColorImage)
        /// {
        ///     if (colorImage != null)
        ///     {
        ///         // working with color image
        ///     }
        /// }
        /// </code>
        /// But <see cref="Capture"/> object automatically tracks all <see cref="Image"/> objects it returned. And will call <see cref="Image.Dispose"/>
        /// for all of them if client code didn't it.
        /// </para><para>
        /// For this reason, if you want to keep returned <see cref="Image"/> for longer life time than life time of <see cref="Capture"/> object,
        /// use <see cref="Image.DuplicateReference"/> method.
        /// </para><para>
        /// The capture will add a reference on any <see cref="Image"/> that is added to it with this setter.
        /// If an existing image is being replaced, the previous image will have the reference released.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Image ColorImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetColorImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetColorImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        /// <summary>Get and set the depth map associated with the given capture. Can be <see langword="null"/> if the capture doesn't have depth data.</summary>
        /// <remarks><para>
        /// It is highly recommended to call <see cref="Image.Dispose"/> for returned image explicitly:
        /// <code>
        /// using (var depthImage = capture.DepthImage)
        /// {
        ///     if (depthImage != null)
        ///     {
        ///         // working with depth image
        ///     }
        /// }
        /// </code>
        /// But <see cref="Capture"/> object automatically tracks all <see cref="Image"/> objects it returned. And will call <see cref="Image.Dispose"/>
        /// for all of them if client code didn't it.
        /// </para><para>
        /// For this reason, if you want to keep returned <see cref="Image"/> for longer life time than life time of <see cref="Capture"/> object,
        /// use <see cref="Image.DuplicateReference"/> method.
        /// </para><para>
        /// The capture will add a reference on any <see cref="Image"/> that is added to it with this setter.
        /// If an existing image is being replaced, the previous image will have the reference released.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Image DepthImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetDepthImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetDepthImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        /// <summary>Get and set the IR (infrared) image associated with the given capture. Can be <see langword="null"/> if the capture doesn't have IR data.</summary>
        /// <remarks><para>
        /// It is highly recommended to call <see cref="Image.Dispose"/> for returned image explicitly:
        /// <code>
        /// using (var irImage = capture.IRImage)
        /// {
        ///     if (irImage != null)
        ///     {
        ///         // working with IR image
        ///     }
        /// }
        /// </code>
        /// But <see cref="Capture"/> object automatically tracks all <see cref="Image"/> objects it returned. And will call <see cref="Image.Dispose"/>
        /// for all of them if client code didn't it.
        /// </para><para>
        /// For this reason, if you want to keep returned <see cref="Image"/> for longer life time than life time of <see cref="Capture"/> object,
        /// use <see cref="Image.DuplicateReference"/> method.
        /// </para><para>
        /// The capture will add a reference on any <see cref="Image"/> that is added to it with this setter.
        /// If an existing image is being replaced, the previous image will have the reference released.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Image IRImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetIRImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetIRImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        /// <summary>Get and set the temperature associated with the capture, in Celsius.</summary>
        /// <remarks>
        /// This function returns the temperature of the device at the time of the capture in Celsius. If
        /// the temperature is unavailable, the function will return <see cref="float.NaN"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public float TemperatureC
        {
            get => NativeApi.CaptureGetTemperatureC(handle.ValueNotDisposed);
            set => NativeApi.CaptureSetTemperatureC(handle.ValueNotDisposed, value);
        }

        /// <summary>Extracts handle from <paramref name="capture"/>.</summary>
        /// <param name="capture">Managed object. Can be <see langword="null"/>.</param>
        /// <returns>Appropriate unmanaged handle. Can be <see cref="NativeHandles.CaptureHandle.Zero"/>.</returns>
        internal static NativeHandles.CaptureHandle ToHandle(Capture capture)
            => capture?.handle?.ValueNotDisposed ?? NativeHandles.CaptureHandle.Zero;

        #region Equatable

        /// <summary>Two captures are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="capture">Another captures to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both captures reference to one and the same unmanaged object.</returns>
        public bool Equals(Capture capture)
            => !(capture is null) && capture.handle.Equals(handle);

        /// <summary>Two captures are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Some object to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is also <see cref="Capture"/> and they both reference to one and the same unmanaged object.</returns>
        public override bool Equals(object obj)
            => obj is Capture && Equals((Capture)obj);

        /// <summary>Uses underlying handle as hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        /// <seealso cref="Equals(Capture)"/>
        public override int GetHashCode()
            => handle.GetHashCode();

        /// <summary>To be consistent with <see cref="Equals(Capture)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Capture)"/>
        public static bool operator ==(Capture left, Capture right)
            => (left is null && right is null) || (!(left is null) && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(Capture)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Capture)"/>
        public static bool operator !=(Capture left, Capture right)
            => !(left == right);

        /// <summary>Convenient (for debugging needs, first of all) string representation of object as an address of unmanaged object in memory.</summary>
        /// <returns><c>{HandleTypeName}#{Address}</c></returns>
        public override string ToString()
            => handle.ToString();

        #endregion
    }
}
