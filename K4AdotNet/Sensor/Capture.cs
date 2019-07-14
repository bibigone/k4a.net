using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>capture</c> class from <c>k4a.hpp</c>
    public sealed class Capture : IDisposablePlus, IReferenceDuplicatable<Capture>
    {
        private readonly ChildrenDisposer children = new ChildrenDisposer();
        private readonly NativeHandles.HandleWrapper<NativeHandles.CaptureHandle> handle;

        public Capture()
        {
            var res = NativeApi.CaptureCreate(out var handle);
            if (res != NativeCallResults.Result.Failed || handle == null || handle.IsInvalid)
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

        public void Dispose()
        {
            children.Dispose();
            handle.Dispose();
        }

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public Capture DuplicateReference()
            => new Capture(handle.ValueNotDisposed.DuplicateReference());

        public Image ColorImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetColorImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetColorImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        public Image DepthImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetDepthImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetDepthImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        public Image IRImage
        {
            get => children.Register(Image.Create(NativeApi.CaptureGetIRImage(handle.ValueNotDisposed)));
            set => NativeApi.CaptureSetIRImage(handle.ValueNotDisposed, Image.ToHandle(value));
        }

        public float TemperatureC
        {
            get => NativeApi.CaptureGetTemperatureC(handle.ValueNotDisposed);
            set => NativeApi.CaptureSetTemperatureC(handle.ValueNotDisposed, value);
        }

        internal static NativeHandles.CaptureHandle ToHandle(Capture capture)
            => capture?.handle?.ValueNotDisposed ?? NativeHandles.CaptureHandle.Zero;
    }
}
