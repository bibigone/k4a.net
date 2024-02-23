using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
#if ORBBECSDK_K4A_WRAPPER
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_depthengine_t);
    //
    /// <summary>Handle to a depthengine instance (OrbbecSDK-K4A-Wrapper only).</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DepthEngineHandle : INativeHandle
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
                NativeApi.DepthEngineHelperRelease(this);
        }
    }
#endif
}
