using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_frame_t);
    //
    /// <summary>Handle to an Azure Kinect body tracking frame.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct BodyFrameHandle : INativeHandle
    {
        private readonly IntPtr value;

        /// <inheritdoc cref="INativeHandle.UnsafeValue"/>
        IntPtr INativeHandle.UnsafeValue => value;

        /// <inheritdoc cref="INativeHandle.IsValid"/>
        public bool IsValid => value != IntPtr.Zero;

        /// <summary>Call this method if you want to have one more reference to the same body frame.</summary>
        /// <remarks>Returns the same handle.</remarks>
        public BodyFrameHandle DuplicateReference()
        {
            if (!IsValid)
                throw new InvalidOperationException("Invalid handle");
            NativeApi.FrameReference(this);
            return this;
        }

        /// <inheritdoc cref="INativeHandle.Release"/>
        public void Release()
        {
            if (IsValid)
                NativeApi.FrameRelease(this);
        }
    }
}
