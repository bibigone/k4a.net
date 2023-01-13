using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_tracker_t);
    //
    /// <summary>Handle to Azure Kinect body tracking component.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct TrackerHandle : INativeHandle
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
                NativeApi.TrackerDestroy(this);
        }
    }
}
