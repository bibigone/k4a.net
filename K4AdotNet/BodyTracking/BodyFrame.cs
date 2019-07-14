using System;

namespace K4AdotNet.BodyTracking
{
    public sealed class BodyFrame : IDisposablePlus, IReferenceDuplicatable<BodyFrame>
    {
        private readonly ChildrenDisposer children = new ChildrenDisposer();
        private readonly NativeHandles.HandleWrapper<NativeHandles.BodyFrameHandle> handle;

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

        public void Dispose()
        {
            children.Dispose();
            handle.Dispose();
        }

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public BodyFrame DuplicateReference()
            => new BodyFrame(handle.ValueNotDisposed.DuplicateReference());

        public Microseconds64 Timestamp => NativeApi.FrameGetTimestamp(handle.ValueNotDisposed);

        public int BodyCount => Helpers.UIntPtrToInt32(NativeApi.FrameGetNumBodies(handle.ValueNotDisposed));

        public Sensor.Capture Capture => children.Register(Sensor.Capture.Create(NativeApi.FrameGetCapture(handle.ValueNotDisposed)));

        public const byte NotABodyIndexMapPixelValue = byte.MaxValue;

        public Sensor.Image BodyIndexMap => children.Register(Sensor.Image.Create(NativeApi.FrameGetBodyIndexMap(handle.ValueNotDisposed)));

        public void GetBodySkeleton(int bodyIndex, out Skeleton skeleton)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            if (NativeApi.FrameGetBodySkeleton(handle.ValueNotDisposed, Helpers.Int32ToUIntPtr(bodyIndex), out skeleton) != NativeCallResults.Result.Succeeded)
                throw new BodyTrackingException($"Cannot extract skeletal data for body with index {bodyIndex}");
        }

        public BodyId GetBodyId(int bodyIndex)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            return NativeApi.FrameGetBodyId(handle.ValueNotDisposed, Helpers.Int32ToUIntPtr(bodyIndex));
        }

        internal static BodyFrame Create(NativeHandles.BodyFrameHandle bodyFrameHandle)
            => bodyFrameHandle != null && !bodyFrameHandle.IsInvalid ? new BodyFrame(bodyFrameHandle) : null;
    }
}