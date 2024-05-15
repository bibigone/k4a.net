using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    /// <summary>DLL imports of some native functions from <c>k4a.h</c>, <c>record.h</c>, <c>playback.h</c> and <c>k4abt.h</c> header files.</summary>
    /// <remarks>These functions are required for implementation of <c>XxxHandle</c> classes.</remarks>
    internal static partial class NativeApi
    {
        #region k4abt.h

        // K4ABT_EXPORT void k4abt_tracker_destroy(k4abt_tracker_t tracker_handle);
        /// <summary>Releases a body tracker handle. </summary>
        /// <param name="trackerHandle">Tracker to be destroyed.</param>
        /// <remarks>Once destroyed, the handle is no longer valid.</remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TrackerDestroy(IntPtr trackerHandle);

        // K4ABT_EXPORT void k4abt_frame_release(k4abt_frame_t body_frame_handle);
        /// <summary>Release a body frame back to the SDK.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object to return to SDK.</param>
        /// <remarks>Once released, the handle is no longer valid.</remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FrameRelease(IntPtr bodyFrameHandle);

        // K4ABT_EXPORT void k4abt_frame_reference(k4abt_frame_t body_frame_handle);
        /// <summary>Add a reference to a body frame.</summary>
        /// <param name="bodyFrameHandle">Body frame to add a reference to.</param>
        /// <remarks>Call this function to add an additional reference to a body frame.</remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_reference", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FrameReference(BodyFrameHandle bodyFrameHandle);

        #endregion
    }
}
