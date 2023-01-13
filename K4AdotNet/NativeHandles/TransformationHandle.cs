using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_transformation_t);
    //
    /// <summary>Handle to an Azure Kinect transformation context.</summary>
    /// <remarks>Handles are created with <c>k4a_transformation_create()</c>.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct TransformationHandle : INativeHandle
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
                NativeApi.TransformationDestroy(this);
        }
    }
}
