using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>image</c> class from <c>k4a.hpp</c>
    //
    /// <summary>An Azure Kinect image referencing its buffer and metadata.</summary>
    /// <remarks><para>
    /// Is used for color images, IR images, depth maps, body index maps, 3D point clouds.
    /// </para></remarks>
    /// <seealso cref="ImageFormat"/>
    public sealed class Image
        : IDisposablePlus, IReferenceDuplicatable<Image>, IEquatable<Image>
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.ImageHandle> handle;     // This class is an wrapper around this handle

        private Image(NativeHandles.ImageHandle handle)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        internal static Image Create(NativeHandles.ImageHandle handle)
            => handle != null && !handle.IsInvalid ? new Image(handle) : null;

        /// <summary>Creates new image with specified format and size in pixels.</summary>
        /// <param name="format">Format of image. Must be format with known stride: <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.</param>
        /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
        /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
        /// <remarks>
        /// This version of constructor can be used only for <paramref name="format"/> with known dependency between image width in pixels and stride in bytes
        /// and cannot be used for other formats. For details see <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.
        /// For other formats use <see cref="Image(ImageFormat, int, int, int)"/> or <see cref="Image(ImageFormat, int, int, int, int)"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Image stride in bytes cannot be automatically calculated from <paramref name="widthPixels"/> for specified <paramref name="format"/>.
        /// </exception>
        public Image(ImageFormat format, int widthPixels, int heightPixels)
            : this(format, widthPixels, heightPixels, format.StrideBytes(widthPixels))
        { }

        /// <summary>Creates new image with specified format, size in pixels and stride in bytes.</summary>
        /// <param name="format">Format of image. Cannot be <see cref="ImageFormat.ColorMjpg"/>.</param>
        /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
        /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
        /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be positive.</param>
        /// <remarks>
        /// Don't use this method to create image with unknown/unspecified stride (like <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>).
        /// For such formats, size of image in bytes must be specified to create image: <see cref="Image(ImageFormat, int, int, int, int)"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
        /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="strideBytes"/> is equal to zero. In this case size of image in bytes must be specified to create image.
        /// </exception>
        public Image(ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
        {
            if (widthPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(widthPixels));
            if (heightPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(heightPixels));
            if (strideBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            if (strideBytes == 0)
                throw new InvalidOperationException("Zero stride is used for formats with complex structure like MJPEG. In this case size of image in bytes must be specified to create image.");
            if (format.HasKnownBytesPerPixel() && strideBytes < widthPixels * format.BytesPerPixel())
                throw new ArgumentOutOfRangeException(nameof(strideBytes));

            var res = NativeApi.ImageCreate(format, widthPixels, heightPixels, strideBytes, out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new ArgumentException($"Cannot create image with format {format}, size {widthPixels}x{heightPixels} pixels and stride {strideBytes} bytes.");

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        /// <summary>Creates new image with specified format, size in pixels and stride in bytes.</summary>
        /// <param name="format">Format of image.</param>
        /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
        /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
        /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be non-negative. Zero value can be used for <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>.</param>
        /// <param name="sizeBytes">Size of image buffer in size. Non negative. Cannot be less than size calculated from image parameters.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
        /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>
        /// or <paramref name="sizeBytes"/> is less than zero or <paramref name="sizeBytes"/> is less than size calculated from <paramref name="heightPixels"/> and <paramref name="strideBytes"/>.
        /// </exception>
        public Image(ImageFormat format, int widthPixels, int heightPixels, int strideBytes, int sizeBytes)
        {
            if (widthPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(widthPixels));
            if (heightPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(heightPixels));
            if (strideBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            if (format.HasKnownBytesPerPixel() && strideBytes < widthPixels * format.BytesPerPixel())
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            if (sizeBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(sizeBytes));
            if (strideBytes > 0 && sizeBytes < format.ImageSizeBytes(strideBytes, heightPixels))
                throw new ArgumentOutOfRangeException(nameof(sizeBytes));

            var buffer = Marshal.AllocHGlobal(sizeBytes);
            if (buffer == IntPtr.Zero)
                throw new OutOfMemoryException($"Cannot allocate buffer of {sizeBytes} bytes.");

            var res = NativeApi.ImageCreateFromBuffer(format, widthPixels, heightPixels, strideBytes,
                buffer, Helpers.Int32ToUIntPtr(sizeBytes), unmanagedBufferReleaseCallback, IntPtr.Zero,
                out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new ArgumentException($"Cannot create image with format {format}, size {widthPixels}x{heightPixels} pixels, stride {strideBytes} bytes from buffer of size {sizeBytes} bytes.");

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        /// <summary>Creates new image for specified underlying buffer with specified format and size in pixels.</summary>
        /// <typeparam name="T">Type of array elements in underlying buffer. Must be value type.</typeparam>
        /// <param name="buffer">Underlying buffer for image. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
        /// <param name="format">Format of image. Must be format with known stride: <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.</param>
        /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
        /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
        /// <returns>Created image. Not <see langword="null"/>.</returns>
        /// <remarks><para>
        /// This version of method can be used only for <paramref name="format"/> with known dependency between image width in pixels and stride in bytes
        /// and cannot be used for other formats. For details see <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.
        /// For other formats use <see cref="CreateFromArray{T}(T[], ImageFormat, int, int, int)"/>.
        /// </para><para>
        /// <see cref="Buffer"/> points to pinned array <paramref name="buffer"/> for such images.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
        /// or <paramref name="buffer"/> array is too small for specified image parameters.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Image stride in bytes cannot be automatically calculated from <paramref name="widthPixels"/> for specified <paramref name="format"/>.
        /// </exception>
        /// <seealso cref="ImageFormats.StrideBytes(ImageFormat, int)"/>
        public static Image CreateFromArray<T>(T[] buffer, ImageFormat format, int widthPixels, int heightPixels)
            where T : struct
            => CreateFromArray(buffer, format, widthPixels, heightPixels, format.StrideBytes(widthPixels));

        /// <summary>Creates new image for specified underlying buffer with specified format and size in pixels.</summary>
        /// <typeparam name="T">Type of array elements in underlying buffer. Must be value type.</typeparam>
        /// <param name="buffer">Underlying buffer for image. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
        /// <param name="format">Format of image.</param>
        /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
        /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
        /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be non-negative. Zero value can be used for <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>.</param>        /// <returns>Created image. Not <see langword="null"/>.</returns>
        /// <remarks><para>
        /// <see cref="Buffer"/> points to pinned array <paramref name="buffer"/> for such images.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
        /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>
        /// or <paramref name="buffer"/> array is too small for specified image parameters.
        /// </exception>
        public static Image CreateFromArray<T>(T[] buffer, ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
            where T: struct
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (widthPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(widthPixels));
            if (heightPixels <= 0)
                throw new ArgumentOutOfRangeException(nameof(heightPixels));
            if (strideBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            if (format.HasKnownBytesPerPixel() && strideBytes < widthPixels * format.BytesPerPixel())
                throw new ArgumentOutOfRangeException(nameof(strideBytes));
            var sizeBytes = buffer.Length * Marshal.SizeOf<T>();
            if (strideBytes > 0 && sizeBytes < format.ImageSizeBytes(strideBytes, heightPixels))
                throw new ArgumentOutOfRangeException(nameof(buffer) + "." + nameof(buffer.Length));

            var bufferPin = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var bufferPtr = bufferPin.AddrOfPinnedObject();

            var res = NativeApi.ImageCreateFromBuffer(format, widthPixels, heightPixels, strideBytes,
                bufferPtr, Helpers.Int32ToUIntPtr(sizeBytes), pinnedArrayReleaseCallback, (IntPtr)bufferPin,
                out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new ArgumentException($"Cannot create image with format {format}, size {widthPixels}x{heightPixels} pixels, stride {strideBytes} bytes from buffer of size {sizeBytes} bytes.");

            return Create(handle);
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
        /// (Multiple objects of <see cref="Image"/> can reference one and the same image. For details see <see cref="DuplicateReference"/>.)
        /// </para><para>
        /// Can be called multiple times but event <see cref="Disposed"/> will be raised only once.
        /// </para></remarks>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        /// <seealso cref="DuplicateReference"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the image has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler Disposed;

        /// <summary>Creates new reference to the same unmanaged image object.</summary>
        /// <returns>New object that references exactly to the same underlying unmanaged object as original one. Not <see langword="null"/>.</returns>
        /// <remarks>It helps to manage underlying object lifetime and to access image data from different threads and different components of application.</remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        /// <seealso cref="Dispose"/>
        public Image DuplicateReference()
            => new Image(handle.ValueNotDisposed.DuplicateReference());

        /// <summary>Get the image buffer.</summary>
        /// <remarks>Use this buffer to access the raw image data.</remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public IntPtr Buffer => NativeApi.ImageGetBuffer(handle.ValueNotDisposed);

        /// <summary>Get the image buffer size in bytes.</summary>
        /// <remarks>Use this function to know what the size of the image buffer is returned by <see cref="Buffer"/>.</remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int SizeBytes => Helpers.UIntPtrToInt32(NativeApi.ImageGetSize(handle.ValueNotDisposed));

        /// <summary>Get the format of the image.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public ImageFormat Format => NativeApi.ImageGetFormat(handle.ValueNotDisposed);

        /// <summary>Get the image width in pixels.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int WidthPixels => NativeApi.ImageGetWidthPixels(handle.ValueNotDisposed);

        /// <summary>Get the image height in pixels.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int HeightPixels => NativeApi.ImageGetHeightPixels(handle.ValueNotDisposed);

        /// <summary>Get the image stride in bytes (the number of bytes per horizontal line of the image).</summary>
        /// <remarks>Can be zero for compressed formats with unknown stride like MJPEG.</remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int StrideBytes => NativeApi.ImageGetStrideBytes(handle.ValueNotDisposed);

        /// <summary>Deprecated in version 1.2 of Sensor SDK. Please use <see cref="DeviceTimestamp"/> property instead of this one.</summary>
        [Obsolete("Deprecated in version 1.2 of Sensor SDK. Please use DeviceTimestamp property instead of this one.")]
        public Microseconds64 Timestamp
        {
            get => DeviceTimestamp;
            set => DeviceTimestamp = value;
        }

        /// <summary>Get and set the image's device timestamp.</summary>
        /// <remarks><para>
        /// Returns the device timestamp of the image, as captured by the hardware. Timestamps are recorded by the device and
        /// represent the mid-point of exposure. They may be used for relative comparison, but their absolute value has no
        /// defined meaning.
        /// </para><para>
        /// <see cref="Microseconds64"/> supports implicit conversion to/from <see cref="TimeSpan"/>.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        /// <seealso cref="SystemTimestamp"/>
        public Microseconds64 DeviceTimestamp
        {
            get => NativeApi.ImageGetDeviceTimestamp(handle.ValueNotDisposed);
            set => NativeApi.ImageSetDeviceTimestamp(handle.ValueNotDisposed, value);
        }

        /// <summary>Get and set the image's system timestamp.</summary>
        /// <remarks><para>
        /// Returns the system timestamp of the image. Timestamps are recorded by the host. They may be used for relative
        /// comparison, as they are relative to the corresponding system clock.The absolute value is a monotonic count from
        /// an arbitrary point in the past.
        /// </para><para>
        /// The system timestamp is captured at the moment host PC finishes receiving the image.
        /// </para><para>
        /// On Linux the system timestamp is read from <c>clock_gettime(CLOCK_MONOTONIC)</c>, which measures realtime and is not
        /// impacted by adjustments to the system clock.It starts from an arbitrary point in the past. On Windows the system
        /// timestamp is read from <c>QueryPerformanceCounter()</c>, it also measures realtime and is not impacted by adjustments to the
        /// system clock. It also starts from an arbitrary point in the past.
        /// </para><para>
        /// <see cref="Nanoseconds64"/> supports implicit conversion to/from <see cref="TimeSpan"/>.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        /// <seealso cref="DeviceTimestamp"/>
        public Nanoseconds64 SystemTimestamp
        {
            get => NativeApi.ImageGetSystemTimestamp(handle.ValueNotDisposed);
            set => NativeApi.ImageSetSystemTimestamp(handle.ValueNotDisposed, value);
        }

        /// <summary>Get and set the image exposure time. This is only supported on color image formats.</summary>
        /// <remarks>
        /// <see cref="Microseconds64"/> supports implicit conversion to/from <see cref="TimeSpan"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public Microseconds64 Exposure
        {
            get => NativeApi.ImageGetExposure(handle.ValueNotDisposed);
            set => NativeApi.ImageSetExposure(handle.ValueNotDisposed, value);
        }

        /// <summary>Get and set the image white balance in degrees Kelvin. This is only supported on color image formats.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int WhiteBalance
        {
            get => checked((int)NativeApi.ImageGetWhiteBalance(handle.ValueNotDisposed));
            set => NativeApi.ImageSetWhiteBalance(handle.ValueNotDisposed, checked((uint)value));
        }

        /// <summary>Get and set the image ISO speed. This is only supported on color image formats.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
        public int IsoSpeed
        {
            get => checked((int)NativeApi.ImageGetIsoSpeed(handle.ValueNotDisposed));
            set => NativeApi.ImageSetIsoSpeed(handle.ValueNotDisposed, checked((uint)value));
        }

        /// <summary>Copies image data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for image data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(byte[] dst)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            if (dst.Length < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            Marshal.Copy(Buffer, dst, 0, size);
            return size;
        }

        /// <summary>Copies image data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for image data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(short[] dst)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(short);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        /// <summary>Copies image data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for image data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(float[] dst)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(float);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        /// <summary>Copies image data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for image data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(int[] dst)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(int);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        /// <summary>Fills data in image buffer from specified managed array.</summary>
        /// <param name="src">Array with raw image data. Cannot be <see langword="null"/>. Must have appropriate length (see <see cref="SizeBytes"/>).</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="src"/> array does not match size of image buffer.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public void FillFrom(byte[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (src.Length != SizeBytes)
                throw new ArgumentOutOfRangeException(nameof(src) + "." + nameof(src.Length));
            Marshal.Copy(src, 0, Buffer, src.Length);
        }

        /// <summary>Fills data in image buffer from specified managed array.</summary>
        /// <param name="src">Array with raw image data. Cannot be <see langword="null"/>. Must have appropriate length (see <see cref="SizeBytes"/>).</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="src"/> array does not match size of image buffer.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public void FillFrom(short[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            var size = SizeBytes;
            var elementSize = sizeof(short);
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            if (src.Length * elementSize != size)
                throw new ArgumentOutOfRangeException(nameof(src) + "." + nameof(src.Length));
            Marshal.Copy(src, 0, Buffer, src.Length);
        }

        /// <summary>Fills data in image buffer from specified managed array.</summary>
        /// <param name="src">Array with raw image data. Cannot be <see langword="null"/>. Must have appropriate length (see <see cref="SizeBytes"/>).</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="src"/> array does not match size of image buffer.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public void FillFrom(float[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            var size = SizeBytes;
            var elementSize = sizeof(float);
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            if (src.Length * elementSize != size)
                throw new ArgumentOutOfRangeException(nameof(src) + "." + nameof(src.Length));
            Marshal.Copy(src, 0, Buffer, src.Length);
        }

        /// <summary>Fills data in image buffer from specified managed array.</summary>
        /// <param name="src">Array with raw image data. Cannot be <see langword="null"/>. Must have appropriate length (see <see cref="SizeBytes"/>).</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="src"/> array does not match size of image buffer.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public void FillFrom(int[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            var size = SizeBytes;
            var elementSize = sizeof(int);
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of image {size} is not divisible by element size {elementSize}.");
            if (src.Length * elementSize != size)
                throw new ArgumentOutOfRangeException(nameof(src) + "." + nameof(src.Length));
            Marshal.Copy(src, 0, Buffer, src.Length);
        }

        /// <summary>Extracts handle from <paramref name="image"/>.</summary>
        /// <param name="image">Managed object. Can be <see langword="null"/>.</param>
        /// <returns>Appropriate unmanaged handle. Can be <see cref="NativeHandles.ImageHandle.Zero"/>.</returns>
        internal static NativeHandles.ImageHandle ToHandle(Image image)
            => image?.handle?.ValueNotDisposed ?? NativeHandles.ImageHandle.Zero;

        #region Equatable

        /// <summary>Two images are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="image">Another image to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both images reference to one and the same unmanaged object.</returns>
        public bool Equals(Image image)
            => !(image is null) && image.handle.Equals(handle);

        /// <summary>Two images are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Some object to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is also <see cref="Image"/> and they both reference to one and the same unmanaged object.</returns>
        public override bool Equals(object obj)
            => obj is Image && Equals((Image)obj);

        /// <summary>Uses underlying handle as hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        /// <seealso cref="Equals(Image)"/>
        public override int GetHashCode()
            => handle.GetHashCode();

        /// <summary>To be consistent with <see cref="Equals(Image)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Image)"/>
        public static bool operator ==(Image left, Image right)
            => (left is null && right is null) || (!(left is null) && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(Image)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Image)"/>
        public static bool operator !=(Image left, Image right)
            => !(left == right);

        /// <summary>Convenient (for debugging needs, first of all) string representation of object as an address of unmanaged object in memory.</summary>
        /// <returns><c>{HandleTypeName}#{Address}</c></returns>
        public override string ToString()
            => handle.ToString();

        #endregion

        #region Memory management

        // This field is required to keep callback delegate in memory
        private static readonly NativeApi.MemoryDestroyCallback unmanagedBufferReleaseCallback
            = new NativeApi.MemoryDestroyCallback(ReleaseUnmanagedBuffer);

        private static void ReleaseUnmanagedBuffer(IntPtr buffer, IntPtr context)
            => Marshal.FreeHGlobal(buffer);

        // This field is required to keep callback delegate in memory
        private static readonly NativeApi.MemoryDestroyCallback pinnedArrayReleaseCallback
            = new NativeApi.MemoryDestroyCallback(ReleasePinnedArray);

        private static void ReleasePinnedArray(IntPtr buffer, IntPtr context)
            => ((GCHandle)context).Free();

        #endregion
    }
}
