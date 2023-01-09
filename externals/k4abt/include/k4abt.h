/** \file k4abt.h
 * Kinect For Azure Body Tracking SDK.
 */

#ifndef K4ABT_H
#define K4ABT_H

#include <k4abtversion.h>
#include <k4abttypes.h>

#ifdef K4ABT_STATIC_DEFINE
#  define K4ABT_EXPORT
#else
#ifdef _WIN32
#  ifndef K4ABT_EXPORT
#    define K4ABT_EXPORT __declspec(dllexport)
#  endif
#else
#  ifndef K4ABT_EXPORT
#    define K4ABT_EXPORT __attribute__((visibility("default")))
#  endif
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

/**
 * \defgroup btfunctions Functions
 * \ingroup btcsdk
 *
 * Public functions of the SDK
 *
 * @{
 */

/** Create a body tracker handle.
 *
 * \param sensor_calibration
 * The sensor calibration that will be used for capture processing.
 *
 * \param config
 * The configuration we want to run the tracker in. This can be initialized with ::K4ABT_TRACKER_CONFIG_DEFAULT.
 *
 * \param tracker_handle
 * Output parameter which on success will return a handle to the body tracker.
 *
 * \relates k4abt_tracker_t
 *
 * \return ::K4A_RESULT_SUCCEEDED if the body tracker handle was created successfully.
 *
 * \remarks
 * If successful, k4abt_tracker_create() will return a body tracker handle in the tracker parameter. This handle grants
 * access to the body tracker and may be used in the other k4abt API calls.
 *
 * \remarks
 * When done with body tracking, close the handle with k4abt_tracker_destroy().
 *
 * \remarks
 * Only one tracker is allowed to exist at the same time in each process. If you call this API without destroying the
 * previous tracker you created, the API call will fail.
 */
 K4ABT_EXPORT k4a_result_t k4abt_tracker_create(const k4a_calibration_t* sensor_calibration,
                                                k4abt_tracker_configuration_t config,
                                                k4abt_tracker_t* tracker_handle);

/** Releases a body tracker handle.
 *
 * \param tracker_handle
 * Handle obtained by k4abt_tracker_create().
 *
 * \relates k4abt_tracker_t
 *
 * \remarks
 * Once released, the tracker_handle is no longer valid.
 */
 K4ABT_EXPORT void k4abt_tracker_destroy(k4abt_tracker_t tracker_handle);

 /** Control the temporal smoothing across frames.
 *
 * \param tracker_handle
 * Handle obtained by k4abt_tracker_create().
 *
 * \param smoothing_factor
 * Set between 0 for no smoothing and 1 for full smoothing. Less smoothing will increase the responsiveness of the
 * detected skeletons but will cause more positional and orientational jitters.
 *
 * \relates k4abt_tracker_t
 *
 * \remarks
 * The default smoothness value is defined as K4ABT_DEFAULT_TRACKER_SMOOTHING_FACTOR.
 */
 K4ABT_EXPORT void k4abt_tracker_set_temporal_smoothing(k4abt_tracker_t tracker_handle, float smoothing_factor);

/** Add a k4a sensor capture to the tracker input queue to generate its body tracking result asynchronously.
 *
 * \param tracker_handle
 * Handle obtained by k4abt_tracker_create().
 *
 * \param sensor_capture_handle
 * Handle to a sensor capture returned by k4a_device_get_capture() from k4a SDK. It should contain the depth data for
 * this function to work. Otherwise the function will return failure.
 *
 * \param timeout_in_ms
 * Specifies the time in milliseconds the function should block waiting to add the sensor capture to the tracker
 * process queue. 0 is a check of the status without blocking. Passing a value of #K4A_WAIT_INFINITE will block
 * indefinitely until the capture is added to the process queue.
 *
 * \returns
 * ::K4A_WAIT_RESULT_SUCCEEDED if a sensor capture is successfully added to the processing queue. If the queue is still
 * full before the timeout elapses, the function will return ::K4A_WAIT_RESULT_TIMEOUT. All other failures will return
 * ::K4A_WAIT_RESULT_FAILED.
 *
 * \relates k4abt_tracker_t
 *
 * \remarks
 * Add a k4a capture to the tracker input queue so that it can be processed asynchronously to generate the body tracking
 * result. The processed results will be added to an output queue maintained by k4abt_tracker_t instance. Call
 * k4abt_tracker_pop_result to get the result and pop it from the output queue.
 * If the input queue or output queue is full, this function will block up until the timeout is reached.
 * Once body_frame data is read, the user must call k4abt_frame_release() to return the allocated memory to the SDK
 *
 * \remarks
 * Upon successfully insert a sensor capture to the input queue this function will return success.
 *
 * \remarks
 * This function returns ::K4A_WAIT_RESULT_FAILED when either the tracker is shut down by k4abt_tracker_shutdown() API,
 * or an internal problem is encountered before adding to the input queue: such as low memory condition,
 * sensor_capture_handle not containing the depth data, or other unexpected issues.
 *
 */
 K4ABT_EXPORT k4a_wait_result_t k4abt_tracker_enqueue_capture(k4abt_tracker_t tracker_handle,
                                                              k4a_capture_t sensor_capture_handle,
                                                              int32_t timeout_in_ms);

/** Gets the next available body frame.
 *
 * \param tracker_handle
 * Handle obtained by k4abt_tracker_create().
 *
 * \param body_frame_handle
 * If successful this contains a handle to a body frame object. Caller must call k4abt_release_frame() when its done
 * using this frame.
 *
 * \param timeout_in_ms
 * Specifies the time in milliseconds the function should block waiting for the body frame. 0 is a check of the queue
 * without blocking. Passing a value of #K4A_WAIT_INFINITE will blocking indefinitely.
 *
 * \returns
 * ::K4A_WAIT_RESULT_SUCCEEDED if a body frame is returned. If a body frame is not available before the timeout elapses,
 * the function will return ::K4A_WAIT_RESULT_TIMEOUT. All other failures will return ::K4A_WAIT_RESULT_FAILED.
 *
 * \relates k4abt_tracker_t
 *
 * \remarks
 * Retrieves the next available body frame result and pop it from the output queue in the k4abt_tracker_t. If a new body
 * frame is not currently available, this function will block up until the timeout is reached. The SDK will buffer at
 * least three body frames worth of data before stopping new capture being queued by k4abt_tracker_enqueue_capture.
 * Once body_frame data is read, the user must call k4abt_frame_release() to return the allocated memory to the SDK.
 *
 * \remarks
 * Upon successfully reads a body frame this function will return success.
 *
 * \remarks
 * This function returns ::K4A_WAIT_RESULT_FAILED when either the tracker is shut down by k4abt_tracker_shutdown() API
 * and the remaining tracker queue is empty, or an internal problem is encountered: such as low memory condition, or
 * other unexpected issues.
 */
 K4ABT_EXPORT k4a_wait_result_t k4abt_tracker_pop_result(k4abt_tracker_t tracker_handle,
                                                         k4abt_frame_t* body_frame_handle,
                                                         int32_t timeout_in_ms);

/** Shutdown the tracker so that no further capture can be added to the input queue.
 *
 * \param tracker_handle
 * Handle obtained by k4abt_tracker_create().
 *
 * \relates k4abt_tracker_t
 *
 * \remarks
 * Once the tracker is shutdown, k4abt_tracker_enqueue_capture() API will always immediately return failure.
 *
 * \remarks
 * If there are remaining catpures in the tracker queue after the tracker is shutdown, k4abt_tracker_pop_result() can
 * still return successfully. Once the tracker queue is empty, the k4abt_tracker_pop_result() call will always immediately
 * return failure.
 *
 * \remarks
 * This function may be called while another thread is blocking in k4abt_tracker_enqueue_capture() or k4abt_tracker_pop_result().
 * Calling this function while another thread is in that function will result in that function returning a failure.
 *
 */
 K4ABT_EXPORT void k4abt_tracker_shutdown(k4abt_tracker_t tracker_handle);

/** Release a body frame back to the SDK
 *
 * \param body_frame_handle
 * Handle to a body frame object to return to SDK.
 *
 * \relates k4abt_frame_t
 *
 * \remarks
 * Called when the user is finished using the body frame.
 */
 K4ABT_EXPORT void k4abt_frame_release(k4abt_frame_t body_frame_handle);

/** Add a reference to a body frame.
 *
 * \param body_frame_handle
 * Body frame to add a reference to.
 *
 * \relates k4abt_frame_t
 *
 * \remarks
 * Call this function to add an additional reference to a body frame. This reference must be removed with
 * k4abt_frame_release().
 *
 * \remarks
 * This function is not thread-safe.
 */
 K4ABT_EXPORT void k4abt_frame_reference(k4abt_frame_t body_frame_handle);

/** Get the number of people from the k4abt_frame_t
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \returns Returns the number of detected bodies. 0 if the function fails.
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 */
 K4ABT_EXPORT uint32_t k4abt_frame_get_num_bodies(k4abt_frame_t body_frame_handle);

/** Get the joint information for a particular person index from the k4abt_frame_t
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \param index
 * The index of the body of which the joint information is queried.
 *
 * \param skeleton
 * If successful this contains the body skeleton information.
 *
 * \returns ::K4A_RESULT_SUCCEEDED if a valid body skeleton is returned. All failures will return ::K4A_RESULT_FAILED.
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 */
 K4ABT_EXPORT k4a_result_t k4abt_frame_get_body_skeleton(k4abt_frame_t body_frame_handle, uint32_t index, k4abt_skeleton_t* skeleton);

/** Get the body id for a particular person index from the k4abt_frame_t
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \param index
 * The index of the body of which the body id information is queried.
 *
 * \returns Returns the body id. All failures will return K4ABT_INVALID_BODY_ID.
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the id of the body given a
 * particular index.
 *
 */
 K4ABT_EXPORT uint32_t k4abt_frame_get_body_id(k4abt_frame_t body_frame_handle, uint32_t index);

/** Get the body frame's device timestamp in microseconds
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \returns
 * Returns the device timestamp of the body frame. If the \p body_frame_handle is invalid this function will return 0. It is
 * also possible for 0 to be a valid timestamp originating from the beginning of a recording or the start of streaming.
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 */
 K4ABT_EXPORT uint64_t k4abt_frame_get_device_timestamp_usec(k4abt_frame_t body_frame_handle);

/** Get the body frame's system timestamp in nanoseconds
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \returns
 * Returns the system timestamp of the body frame. If the \p body_frame_handle is invalid this function will return 0. It is
 * also possible for 0 to be a valid timestamp originating from the beginning of a recording or the start of streaming.
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 */
 K4ABT_EXPORT uint64_t k4abt_frame_get_system_timestamp_nsec(k4abt_frame_t body_frame_handle);

/** Get the body index map from k4abt_frame_t
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \returns
 * Call this function to access the body index map image. Release the image with k4a_image_release().
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 * \remarks Body Index map is the body instance segmentation map. Each pixel maps to the corresponding pixel in the
 * depth image or the ir image. The value for each pixel represents which body the pixel belongs to. It can be either
 * background (value K4ABT_BODY_INDEX_MAP_BACKGROUND) or the index of a detected k4abt_body_t.
 */
 K4ABT_EXPORT k4a_image_t k4abt_frame_get_body_index_map(k4abt_frame_t body_frame_handle);

/** Get the original capture that is used to calculate the k4abt_frame_t
 *
 * \param body_frame_handle
 * Handle to a body frame object returned by k4abt_tracker_pop_result function.
 *
 * \returns
 * Call this function to access the original k4a_capture_t. Release this capture with k4a_capture_release().
 *
 * \relates k4abt_frame_t
 *
 * \remarks Called when the user has received a body frame handle and wants to access the data contained in it.
 *
 */
 K4ABT_EXPORT k4a_capture_t k4abt_frame_get_capture(k4abt_frame_t body_frame_handle);

/** Sample synchronized usage (for simplicity, we don't add any error check here)

    int main()
    {
        ...

        // Read sensor calibration data from the sensor or a recording file
        k4a_calibration_t sensor_calibration = ...

        k4abt_tracker_t tracker;
        k4abt_tracker_configuration_t tracker_config = K4ABT_TRACKER_CONFIG_DEFAULT;
        k4abt_tracker_create(&sensor_calibration, tracker_config, &tracker);

        while (!stop)
        {
            // Read sensor_capture that contains depth information from the sensor or a recording file
            k4a_capture_t sensor_capture = ...

            // Add capture to the body tracker processing queue
            k4abt_tracker_enqueue_capture(tracker, sensor_capture, K4A_WAIT_INFINITE);

            // Release the original capture once it is no longer in use
            k4abt_frame_release(sensor_capture);

            ...

            //
            k4abt_frame_t body_frame;
            k4abt_tracker_pop_result(tracker, &body_frame, K4A_WAIT_INFINITE);

            k4a_image_t body_index_map = k4abt_frame_get_body_index_map(body_frame);

            int num_bodies = k4abt_frame_get_num_bodies(body_frame);
            for (int i = 0; i < count; ++i)
            {
                k4abt_body_t body;
                k4abt_frame_get_skeleton(body_frame_handle, i, &body.skeleton);
                body.id = k4abt_frame_get_body_id(body_frame_handle, i);
                ...
            }

            // Retrieve the original sensor capture that generates the body frame result. It will the be same as
            // sensor_capture for this synchronized example. The function is called here to only demonstrate the usage.
            k4a_capture_t original_sensor_capture = k4abt_frame_get_capture(body_frame)
            ...

            // It is the user's responsibility to release the capture queried by k4abt_frame_get_capture API.
            k4a_capture_release(original_sensor_capture);

            k4a_image_release(body_index_map);
            k4abt_frame_release(body_frame);
        }

        ...

        // Destroy tracker once you are done with it
        k4abt_tracker_destroy(tracker)
    }
*/

/**
 * @}
 */

#ifdef __cplusplus
}
#endif

#endif /* K4ABT_H */


