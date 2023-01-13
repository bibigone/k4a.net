using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_record_t);
    //
    /// <summary>Handle to a Kinect for Azure recording opened for writing.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RecordHandle : INativeHandle
    {
        private readonly IntPtr value;

        /// <inheritdoc cref="INativeHandle.UnsafeValue"/>
        IntPtr INativeHandle.UnsafeValue => value;

        /// <inheritdoc cref="INativeHandle.IsValid"/>
        public bool IsValid => value != IntPtr.Zero;

        /// <inheritdoc cref="INativeHandle.Release"/>
        public void Release()
        {
            if (IsValid)
                NativeApi.RecordClose(this);
        }
    }
}
