using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>image</c> class from <c>k4a.hpp</c>
    public sealed class Image : IDisposable, IReferenceDuplicatable<Image>
    {
        private readonly NativeHandles.ImageHandle handle;

        private Image(NativeHandles.ImageHandle handle)
            => this.handle = handle;

        internal static Image Create(NativeHandles.ImageHandle handle)
            => handle != null && !handle.IsInvalid ? new Image(handle) : null;

        public Image(ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
        {
            var res = NativeApi.ImageCreate(format, widthPixels, heightPixels, strideBytes, out handle);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ArgumentException($"Cannot create image with format {format}, size {widthPixels}x{heightPixels} pixels and stride {strideBytes} bytes.");
        }

        public void Dispose()
            => handle.Dispose();

        public Image DuplicateReference()
            => new Image(handle.DuplicateReference());

        public IntPtr Buffer => NativeApi.ImageGetBuffer(handle);

        public int Size => checked((int)NativeApi.ImageGetSize(handle).ToUInt32());

        public ImageFormat Format => NativeApi.ImageGetFormat(handle);

        public int WidthPixels => NativeApi.ImageGetWidthPixels(handle);

        public int HeightPixels => NativeApi.ImageGetHeightPixels(handle);

        public int StrideBytes => NativeApi.ImageGetStrideBytes(handle);

        public Microseconds64 Timestamp
        {
            get => NativeApi.ImageGetTimestamp(handle);
            set => NativeApi.ImageSetTimestamp(handle, value);
        }

        public Microseconds64 Exposure
        {
            get => NativeApi.ImageGetExposureUsec(handle);
            set => NativeApi.ImageSetExposureTimeUsec(handle, value);
        }

        public int WhiteBalance
        {
            get => checked((int)NativeApi.ImageGetWhiteBalance(handle));
            set => NativeApi.ImageSetWhiteBalance(handle, checked((uint)value));
        }

        public int IsoSpeed
        {
            get => checked((int)NativeApi.ImageGetIsoSpeed(handle));
            set => NativeApi.ImageSetIsoSpeed(handle, checked((uint)value));
        }

        internal static NativeHandles.ImageHandle ToHandle(Image image)
            => image?.handle ?? NativeHandles.ImageHandle.Zero;
    }
}
