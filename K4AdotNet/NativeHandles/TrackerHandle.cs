using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_tracker_t);
    //
    /// <summary>Handle to Azure Kinect body tracking component.</summary>
    internal class TrackerHandle : HandleBase
    {
        private TrackerHandle() { }

        /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                if (Sdk.DetermineDefaultImplIsOrbbec())
                    ReleaseOrbbecHandle(NativeApi.TrackerDestroy);
                else
                    NativeApi.TrackerDestroy(handle);
            }
            return true;
        }
    }
}
