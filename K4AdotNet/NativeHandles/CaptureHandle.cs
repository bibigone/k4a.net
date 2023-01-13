using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_capture_t);
    //
    /// <summary>Handle to an Azure Kinect capture.</summary>
    /// <remarks>
    /// Empty captures are created with <c>k4a_capture_create()</c>.
    /// Captures can be obtained from a device using <c>k4a_device_get_capture()</c>.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CaptureHandle : INativeHandle
    {
        private readonly IntPtr value;

        /// <inheritdoc cref="INativeHandle.UnsafeValue"/>
        IntPtr INativeHandle.UnsafeValue => value;

        /// <inheritdoc cref="INativeHandle.IsValid"/>
        public bool IsValid => value != IntPtr.Zero;

        /// <summary>
        /// Call this method if you want to have one more reference to the same capture.
        /// </summary>
        /// <remarks>Returns the same handle.</remarks>
        public CaptureHandle DuplicateReference()
        {
            if (!IsValid)
                throw new InvalidOperationException("Invalid handle");
            NativeApi.CaptureReference(this);
            return this;
        }

        /// <inheritdoc cref="INativeHandle.Release"/>
        public void Release()
        {
            if (IsValid)
                NativeApi.CaptureRelease(this);
        }
    }
}
