using System;

namespace K4AdotNet.BodyTracking
{
    public sealed class BodyFrame : IDisposable, IReferenceDuplicatable<BodyFrame>
    {
        private readonly NativeHandles.BodyFrameHandle handle;

        private BodyFrame(NativeHandles.BodyFrameHandle handle)
            => this.handle = handle;

        public void Dispose()
            => handle.Dispose();

        public BodyFrame DuplicateReference()
            => new BodyFrame(handle.DuplicateReference());

        public Microseconds64 Timestamp => NativeApi.FrameGetTimestamp(handle);

        public int BodyCount => checked((int)NativeApi.FrameGetNumBodies(handle).ToUInt32());

        public Sensor.Capture Capture
            => Sensor.Capture.Create(NativeApi.FrameGetCapture(handle));

        public Sensor.Image BodyIndexMap
            => Sensor.Image.Create(NativeApi.FrameGetBodyIndexMap(handle));

        public void GetBodySkeleton(int bodyIndex, out Skeleton skeleton)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            if (NativeApi.FrameGetBodySkeleton(handle, new UIntPtr(checked((uint)bodyIndex)), out skeleton) != NativeCallResults.Result.Succeeded)
                throw new BodyTrackingException($"Cannot extract skeletal data for body with index {bodyIndex}");
        }

        public BodyId GetBodyId(int bodyIndex)
        {
            if (bodyIndex < 0 || bodyIndex >= BodyCount)
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            return NativeApi.FrameGetBodyId(handle, new UIntPtr(checked((uint)bodyIndex)));
        }

        internal static BodyFrame Create(NativeHandles.BodyFrameHandle bodyFrameHandle)
            => bodyFrameHandle != null && !bodyFrameHandle.IsInvalid ? new BodyFrame(bodyFrameHandle) : null;
    }
}