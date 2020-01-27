using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    /// <summary>DLL imports for most of native functions from <c>k4abt.h</c> header file.</summary>
    internal static class NativeApi
    {
        public const int MAX_TRACKING_QUEUE_SIZE = 3;

        // #define K4ABT_DEFAULT_TRACKER_SMOOTHING_FACTOR 0.0f
        /// <summary>The default tracker temporal smoothing factor.</summary>
        public const float DEFAULT_TRACKER_SMOOTHING_FACTOR = 0.0f;

        // K4ABT_EXPORT k4a_result_t k4abt_tracker_create(const k4a_calibration_t* sensor_calibration,
        //                                                k4abt_tracker_configuration_t config,
        //                                                k4abt_tracker_t* tracker_handle);
        /// <summary>Create a body tracker handle.</summary>
        /// <param name="sensorCalibration">The sensor calibration that will be used for capture processing.</param>
        /// <param name="config">The configuration we want to run the tracker in. This can be initialized with <see cref="DEFAULT_TRACKER_SMOOTHING_FACTOR"/>.</param>
        /// <param name="trackerHandle">Output parameter which on success will return a handle to the body tracker.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the body tracker handle was created successfully.</returns>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result TrackerCreate(
            [In] ref Sensor.Calibration sensorCalibration,
            TrackerConfiguration config,
            out NativeHandles.TrackerHandle trackerHandle);

        // K4ABT_EXPORT void k4abt_tracker_set_temporal_smoothing(k4abt_tracker_t tracker_handle, float smoothing_factor);
        /// <summary>Control the temporal smoothing across frames.</summary>
        /// <param name="trackerHandle">Handle obtained by <see cref="TrackerCreate(ref Sensor.Calibration, TrackerConfiguration, out NativeHandles.TrackerHandle)"/>.</param>
        /// <param name="smoothingFactor">
        /// Set between 0 for no smoothing and 1 for full smoothing.
        /// Less smoothing will increase the responsiveness of the
        /// detected skeletons but will cause more positional and orientation jitters.
        /// </param>
        /// <remarks>
        /// The default smoothness value is defined as <see cref="DEFAULT_TRACKER_SMOOTHING_FACTOR"/>.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_set_temporal_smoothing", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TrackerSetTemporalSmoothing(NativeHandles.TrackerHandle trackerHandle, float smoothingFactor);

        // K4ABT_EXPORT k4a_wait_result_t k4abt_tracker_enqueue_capture(k4abt_tracker_t tracker_handle,
        //                                                              k4a_capture_t sensor_capture_handle,
        //                                                              int32_t timeout_in_ms);
        /// <summary>Add a Azure Kinect sensor capture to the tracker input queue to generate its body tracking result asynchronously.</summary>
        /// <param name="trackerHandle">Handle obtained by <see cref="TrackerCreate(ref Sensor.Calibration, TrackerConfiguration, out NativeHandles.TrackerHandle)"/>.</param>
        /// <param name="sensorCaptureHandle">
        /// Handle to a sensor capture returned by <see cref="Sensor.NativeApi.CaptureCreate(out NativeHandles.CaptureHandle)"/> from Sensor SDK.
        /// It should contain the depth data for this function to work. Otherwise the function will return failure.
        /// </param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting to add the sensor capture to the tracker
        /// process queue. <see cref="Timeout.NoWait"/> is a check of the status without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until the capture is added to the process queue.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.WaitResult.Succeeded"/> if a sensor capture is successfully added to the processing queue. If the queue is still
        /// full before the timeout elapses, the function will return <see cref="NativeCallResults.WaitResult.Timeout"/>. All other failures will return
        /// <see cref="NativeCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// Add a Azure Kinect capture to the tracker input queue so that it can be processed asynchronously to generate the body tracking
        /// result. The processed results will be added to an output queue maintained by <see cref="NativeHandles.TrackerHandle"/> instance. Call
        /// <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> to get the result and pop it from the output queue.
        /// If the input queue or output queue is full, this function will block up until the timeout is reached.
        /// 
        /// Upon successfully insert a sensor capture to the input queue this function will return success.
        /// 
        /// This function returns <see cref="NativeCallResults.WaitResult.Failed"/> when either the tracker is shut down by <see cref="TrackerShutdown(NativeHandles.TrackerHandle)"/> API,
        /// or an internal problem is encountered before adding to the input queue: such as low memory condition,
        /// <paramref name="sensorCaptureHandle"/> not containing the depth data, or other unexpected issues.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_enqueue_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.WaitResult TrackerEnqueueCapture(
            NativeHandles.TrackerHandle trackerHandle,
            NativeHandles.CaptureHandle sensorCaptureHandle,
            Timeout timeout);

        // K4ABT_EXPORT k4a_wait_result_t k4abt_tracker_pop_result(k4abt_tracker_t tracker_handle,
        //                                                         k4abt_frame_t* body_frame_handle,
        //                                                         int32_t timeout_in_ms);
        /// <summary>Gets the next available body frame.</summary>
        /// <param name="trackerHandle">Handle obtained by <see cref="TrackerCreate"/>.</param>
        /// <param name="bodyFrameHandle">If successful this contains a handle to a body frame object.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the body frame. <see cref="Timeout.NoWait"/> is a check of the status without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until the body frame becomes available.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.WaitResult.Succeeded"/> if a body frame is returned. If a body frame is not available before the timeout elapses,
        /// the function will return <see cref="NativeCallResults.WaitResult.Timeout"/>. All other failures will return <see cref="NativeCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// Retrieves the next available body frame result and pop it from the output queue in the <see cref="NativeHandles.TrackerHandle"/>.
        /// If a new body frame is not currently available, this function will block up until the timeout is reached.
        /// The SDK will buffer at least three body frames worth of data before stopping new capture being queued by <see cref="TrackerEnqueueCapture(NativeHandles.TrackerHandle, NativeHandles.CaptureHandle, Timeout)"/>.
        /// 
        /// Upon successfully reads a body frame this function will return success.
        /// 
        /// This function returns <see cref="NativeCallResults.WaitResult.Failed"/> when either the tracker is shut down by <see cref="TrackerShutdown(NativeHandles.TrackerHandle)"/> API
        /// and the remaining tracker queue is empty, or an internal problem is encountered: such as low memory condition, or
        /// other unexpected issues.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_pop_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.WaitResult TrackerPopResult(
            NativeHandles.TrackerHandle trackerHandle,
            out NativeHandles.BodyFrameHandle bodyFrameHandle,
            Timeout timeout);

        //  K4ABT_EXPORT void k4abt_tracker_shutdown(k4abt_tracker_t tracker_handle);
        /// <summary>Shutdown the tracker so that no further capture can be added to the input queue.</summary>
        /// <param name="trackerHandle">Handle obtained by <see cref="TrackerCreate"/>.</param>
        /// <remarks>
        /// Once the tracker is shutdown, <see cref="TrackerEnqueueCapture(NativeHandles.TrackerHandle, NativeHandles.CaptureHandle, Timeout)"/> API will always immediately return failure.
        ///
        /// If there are remaining captures in the tracker queue after the tracker is shutdown, <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> can
        /// still return successfully. Once the tracker queue is empty, the <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> call will always immediately
        /// return failure.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_tracker_shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TrackerShutdown(NativeHandles.TrackerHandle trackerHandle);

        // K4ABT_EXPORT uint32_t k4abt_frame_get_num_bodies(k4abt_frame_t body_frame_handle);
        /// <summary>Get the number of people from the <see cref="NativeHandles.BodyFrameHandle"/>.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <returns>Returns the number of detected bodies. 0 if the function fails.</returns>
        /// <remarks>Called when the user has received a body frame handle and wants to access the data contained in it.</remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_num_bodies", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint FrameGetNumBodies(NativeHandles.BodyFrameHandle bodyFrameHandle);

        // K4ABT_EXPORT k4a_result_t k4abt_frame_get_body_skeleton(k4abt_frame_t body_frame_handle, uint32_t index, k4abt_skeleton_t* skeleton);
        /// <summary>Get the joint information for a particular person index from the <see cref="NativeHandles.BodyFrameHandle"/>.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <param name="index">The index of the body of which the joint information is queried.</param>
        /// <param name="skeleton">If successful this contains the body skeleton information.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if a valid body skeleton is returned. All failures will return <see cref="NativeCallResults.Result.Failed"/>.</returns>
        /// <remarks>Called when the user has received a body frame handle and wants to access the data contained in it.</remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_body_skeleton", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result FrameGetBodySkeleton(
            NativeHandles.BodyFrameHandle bodyFrameHandle,
            uint index,
            out Skeleton skeleton);

        // K4ABT_EXPORT uint32_t k4abt_frame_get_body_id(k4abt_frame_t body_frame_handle, uint32_t index);
        /// <summary>Get the body id for a particular person index from the <see cref="NativeHandles.BodyFrameHandle"/>.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <param name="index">The index of the body of which the body id information is queried.</param>
        /// <returns>Returns the body id. All failures will return <see cref="BodyId.Invalid"/>.</returns>
        /// <remarks>
        /// Called when the user has received a body frame handle and wants to access the id of the body given a particular index.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_body_id", CallingConvention = CallingConvention.Cdecl)]
        public static extern BodyId FrameGetBodyId(
            NativeHandles.BodyFrameHandle bodyFrameHandle,
            uint index);

        // K4ABT_EXPORT uint64_t k4abt_frame_get_device_timestamp_usec(k4abt_frame_t body_frame_handle);
        /// <summary>Get the body frame's device timestamp.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <returns>
        /// Returns the timestamp of the body frame. If the <paramref name="bodyFrameHandle"/> is invalid this function will return <see cref="Microseconds64.Zero"/>.
        /// It is also possible for <see cref="Microseconds64.Zero"/> to be a valid timestamp originating from the beginning of a recording or the start of streaming.
        /// </returns>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_device_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern Microseconds64 FrameGetDeviceTimestamp(NativeHandles.BodyFrameHandle bodyFrameHandle);

        // K4ABT_EXPORT k4a_image_t k4abt_frame_get_body_index_map(k4abt_frame_t body_frame_handle);
        /// <summary>Get the body index map from <see cref="NativeHandles.BodyFrameHandle"/>.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <returns>Call this function to access the body index map image. Don't forget to call <see cref="IDisposable.Dispose"/> for returned handle after usage.</returns>
        /// <remarks>
        /// Called when the user has received a body frame handle and wants to access the data contained in it.
        /// 
        /// Body Index map is the body instance segmentation map. Each pixel maps to the corresponding pixel in the
        /// depth image or the IR image. The value for each pixel represents which body the pixel belongs to. It can be either
        /// background (value <c>0xFF</c>) or the index of a detected body.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_body_index_map", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.ImageHandle FrameGetBodyIndexMap(NativeHandles.BodyFrameHandle bodyFrameHandle);

        // K4ABT_EXPORT k4a_capture_t k4abt_frame_get_capture(k4abt_frame_t body_frame_handle);
        /// <summary>Get the original capture that is used to calculate <see cref="NativeHandles.BodyFrameHandle"/>.</summary>
        /// <param name="bodyFrameHandle">Handle to a body frame object returned by <see cref="TrackerPopResult(NativeHandles.TrackerHandle, out NativeHandles.BodyFrameHandle, Timeout)"/> function.</param>
        /// <returns>Call this function to access the original <see cref="NativeHandles.CaptureHandle"/>. Don't forget to call <see cref="IDisposable.Dispose"/> for returned handle after usage.</returns>
        /// <remarks>
        /// Called when the user has received a body frame handle and wants to access the data contained in it.
        /// </remarks>
        [DllImport(Sdk.BODY_TRACKING_DLL_NAME, EntryPoint = "k4abt_frame_get_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.CaptureHandle FrameGetCapture(NativeHandles.BodyFrameHandle bodyFrameHandle);
    }
}
