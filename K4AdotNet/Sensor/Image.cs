using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>image</c> class from <c>k4a.hpp</c>
    public sealed class Image : IDisposablePlus, IReferenceDuplicatable<Image>
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.ImageHandle> handle;

        private Image(NativeHandles.ImageHandle handle)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        internal static Image Create(NativeHandles.ImageHandle handle)
            => handle != null && !handle.IsInvalid ? new Image(handle) : null;

        public Image(ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
        {
            var res = NativeApi.ImageCreate(format, widthPixels, heightPixels, strideBytes, out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new ArgumentException($"Cannot create image with format {format}, size {widthPixels}x{heightPixels} pixels and stride {strideBytes} bytes.");
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
            => handle.Dispose();

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public Image DuplicateReference()
            => new Image(handle.ValueNotDisposed.DuplicateReference());

        public IntPtr Buffer => NativeApi.ImageGetBuffer(handle.ValueNotDisposed);

        public int Size => Helpers.UIntPtrToInt32(NativeApi.ImageGetSize(handle.ValueNotDisposed));

        public ImageFormat Format => NativeApi.ImageGetFormat(handle.ValueNotDisposed);

        public int WidthPixels => NativeApi.ImageGetWidthPixels(handle.ValueNotDisposed);

        public int HeightPixels => NativeApi.ImageGetHeightPixels(handle.ValueNotDisposed);

        public int StrideBytes => NativeApi.ImageGetStrideBytes(handle.ValueNotDisposed);

        public Microseconds64 Timestamp
        {
            get => NativeApi.ImageGetTimestamp(handle.ValueNotDisposed);
            set => NativeApi.ImageSetTimestamp(handle.ValueNotDisposed, value);
        }

        public Microseconds64 Exposure
        {
            get => NativeApi.ImageGetExposureUsec(handle.ValueNotDisposed);
            set => NativeApi.ImageSetExposureTimeUsec(handle.ValueNotDisposed, value);
        }

        public int WhiteBalance
        {
            get => checked((int)NativeApi.ImageGetWhiteBalance(handle.ValueNotDisposed));
            set => NativeApi.ImageSetWhiteBalance(handle.ValueNotDisposed, checked((uint)value));
        }

        public int IsoSpeed
        {
            get => checked((int)NativeApi.ImageGetIsoSpeed(handle.ValueNotDisposed));
            set => NativeApi.ImageSetIsoSpeed(handle.ValueNotDisposed, checked((uint)value));
        }

        internal static NativeHandles.ImageHandle ToHandle(Image image)
            => image?.handle?.ValueNotDisposed ?? NativeHandles.ImageHandle.Zero;
    }
}
