using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_image_t);
    //
    /// <summary>Handle to an Azure Kinect image.</summary>
    /// <remarks>Images from a device are retrieved through a <c>k4a_capture_t</c> object returned by <c>k4a_device_get_capture()</c>.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ImageHandle : INativeHandle
    {
        private readonly IntPtr value;

        /// <inheritdoc cref="INativeHandle.UnsafeValue"/>
        IntPtr INativeHandle.UnsafeValue => value;

        /// <inheritdoc cref="INativeHandle.IsValid"/>
        public bool IsValid => value != IntPtr.Zero;

        /// <summary>Call this method if you want to have one more reference to the same image.</summary>
        /// <remarks>Returns the same handle.</remarks>
        public ImageHandle DuplicateReference()
        {
            if (!IsValid)
                throw new InvalidOperationException("Invalid handle");
            NativeApi.ImageReference(this);
            return this;
        }

        /// <inheritdoc cref="INativeHandle.Release"/>
        public void Release()
        {
            if (IsValid)
                NativeApi.ImageRelease(this);
        }
    }
}
