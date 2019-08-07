using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    /// <summary>DLL imports of some native functions from <c>k4a.h</c>, <c>record.h</c>, <c>playback.h</c> and <c>k4abt.h</c> header files.</summary>
    /// <remarks>These functions are required for implementation of <c>XxxHandle</c> classes.</remarks>
    internal static class NativeApi
    {
        #region k4a.h

        // K4A_EXPORT void k4a_capture_reference(k4a_capture_t capture_handle);
        /// <summary>Add a reference to a capture.</summary>
        /// <param name="captureHandle">Capture to add a reference to.</param>
        /// <remarks>Call this function to add an additional reference to a capture.
        ///     This reference must be removed with <see cref="CaptureReference(IntPtr)"/>.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_reference", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureReference(IntPtr captureHandle);

        // K4A_EXPORT void k4a_capture_release(k4a_capture_t capture_handle);
        /// <summary>Release a capture.</summary>
        /// <param name="captureHandle">Capture to release.</param>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureRelease(IntPtr captureHandle);

        // K4A_EXPORT void k4a_image_reference(k4a_image_t image_handle);
        /// <summary>Add a reference to the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <remarks>
        /// References manage the lifetime of the object. When the references reach zero the object is destroyed. A caller must
        /// not access the object after its reference is released.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_reference", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageReference(IntPtr imageHandle);

        // K4A_EXPORT void k4a_image_release(k4a_image_t image_handle);
        /// <summary>Remove a reference from the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <remarks>
        /// References manage the lifetime of the object. When the references reach zero the object is destroyed. A caller must
        /// not access the object after its reference is released.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageRelease(IntPtr imageHandle);

        // K4A_EXPORT void k4a_transformation_destroy(k4a_transformation_t transformation_handle);
        /// <summary>Destroy transformation handle.</summary>
        /// <param name="transformationHandle">Transformation handle to destroy.</param>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_transformation_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransformationDestroy(IntPtr transformationHandle);

        // K4A_EXPORT void k4a_device_close(k4a_device_t device_handle);
        /// <summary>Closes an Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle of device for which the get operation is performed on.</param>
        /// <remarks>Once closed, the handle is no longer valid.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeviceClose(IntPtr deviceHandle);

        #endregion

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
        public static extern void FrameReference(IntPtr bodyFrameHandle);

        #endregion

        #region record.h and playback.h

        // K4ARECORD_EXPORT void k4a_record_close(k4a_record_t recording_handle);
        /// <summary>Closes a recording handle.</summary>
        /// <param name="recordingHandle">Recording handle to be closed.</param>
        /// <remarks>If there is any unwritten data it will be flushed to disk before closing the recording.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RecordClose(IntPtr recordingHandle);

        // K4ARECORD_EXPORT void k4a_playback_close(k4a_playback_t playback_handle);
        /// <summary>Closes a recording playback handle.</summary>
        /// <param name="recordingHandle">Recording playback handle to be closed.</param>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PlaybackClose(IntPtr recordingHandle);

        #endregion
    }
}
