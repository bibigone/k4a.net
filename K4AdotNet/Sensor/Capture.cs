using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>capture</c> class from <c>k4a.hpp</c>
    public sealed class Capture : IDisposable, IReferenceDuplicatable<Capture>
    {
        private readonly NativeHandles.CaptureHandle handle;

        public Capture()
        {
            var res = NativeApi.CaptureCreate(out handle);
            if (res != NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Failed to create blank capture instance");
        }

        internal Capture(NativeHandles.CaptureHandle handle)
            => this.handle = handle;

        internal static Capture Create(NativeHandles.CaptureHandle handle)
            => handle != null && !handle.IsInvalid ? new Capture(handle) : null;

        public void Dispose()
            => handle.Dispose();

        public Capture DuplicateReference()
            => new Capture(handle.DuplicateReference());

        public Image ColorImage
        {
            get => Image.Create(NativeApi.CaptureGetColorImage(handle));
            set => NativeApi.CaptureSetColorImage(handle, Image.ToHandle(value));
        }

        public Image DepthImage
        {
            get => Image.Create(NativeApi.CaptureGetDepthImage(handle));
            set => NativeApi.CaptureSetDepthImage(handle, Image.ToHandle(value));
        }

        public Image IRImage
        {
            get => Image.Create(NativeApi.CaptureGetIRImage(handle));
            set => NativeApi.CaptureSetIRImage(handle, Image.ToHandle(value));
        }

        public float TemperatureC
        {
            get => NativeApi.CaptureGetTemperatureC(handle);
            set => NativeApi.CaptureSetTemperatureC(handle, value);
        }

        internal static NativeHandles.CaptureHandle ToHandle(Capture capture)
            => capture?.handle ?? NativeHandles.CaptureHandle.Zero;
    }
}
