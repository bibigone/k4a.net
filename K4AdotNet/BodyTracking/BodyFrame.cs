using System;

namespace K4AdotNet.BodyTracking
{
    public sealed class BodyFrame
        : IDisposablePlus, IReferenceDuplicatable<BodyFrame>, IEquatable<BodyFrame>
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

        #region Equatable

        /// <summary>Two body frames are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="bodyFrame">Another body frame to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both body frames reference to one and the same unmanaged object.</returns>
        public bool Equals(BodyFrame bodyFrame)
            => !(bodyFrame is null) && bodyFrame.handle.Equals(handle);

        /// <summary>Two body frames are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Some object to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is also <see cref="BodyFrame"/> and they both reference to one and the same unmanaged object.</returns>
        public override bool Equals(object obj)
            => obj is BodyFrame && Equals((BodyFrame)obj);

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
        public static bool operator ==(BodyFrame left, BodyFrame right)
            => (left is null && right is null) || (!(left is null) && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(BodyFrame)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(BodyFrame)"/>
        public static bool operator !=(BodyFrame left, BodyFrame right)
            => !(left == right);

        /// <summary>Convenient (for debugging needs, first of all) string representation of object as an address of unmanaged object in memory.</summary>
        /// <returns><c>{HandleTypeName}#{Address}</c></returns>
        public override string ToString()
            => handle.ToString();

        #endregion
    }
}