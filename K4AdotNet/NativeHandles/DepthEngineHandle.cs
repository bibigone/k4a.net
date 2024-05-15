using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_depthengine_t);
    //
    /// <summary>Handle to a depthengine instance (OrbbecSDK-K4A-Wrapper only).</summary>
    internal class DepthEngineHandle : HandleBase
    {
        private DepthEngineHandle() { }

        /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
                NativeApi.Orbbec.DepthEngineHelperRelease(handle);
            return true;
        }
    }
}
