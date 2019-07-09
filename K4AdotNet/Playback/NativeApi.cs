using System;
using System.Runtime.InteropServices;
using System.Text;

namespace K4AdotNet.Playback
{
    /// <summary>DLL imports for most of native functions from <c>playback.h</c> header file.</summary>
    internal static class NativeApi
    {
        // K4ARECORD_EXPORT k4a_result_t k4a_playback_open(const char *path, k4a_playback_t *playback_handle);
        /// <summary>Opens an existing recording file for reading.</summary>
        /// <param name="path">File system path of the existing recording.</param>
        /// <param name="playbackHandle">If successful, this contains a pointer to the recording handle.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_open", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern NativeCallResults.Result PlaybackOpen(
            [MarshalAs(UnmanagedType.LPStr)] string path,
            out NativeHandles.PlaybackHandle playbackHandle);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_raw_calibration(k4a_playback_t playback_handle,
        //                                                                       uint8_t* data,
        //                                                                       size_t *data_size);
        /// <summary>Get the raw calibration blob for the Azure Kinect device used during recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="data">
        /// Location to write the calibration data to. This field may optionally be set to <see langword="null"/>
        /// if the caller wants to query for the needed data size.
        /// </param>
        /// <param name="dataSize">
        /// On passing <paramref name="dataSize"/> into the function this variable represents the available size to write the raw data to. On
        /// return this variable is updated with the amount of data actually written to the buffer.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.BufferResult.Succeeded"/> if <paramref name="data"/> was successfully written.
        /// If <paramref name="dataSize"/> points to a buffer size that is too small to hold the output,
        /// <see cref="NativeCallResults.BufferResult.TooSmall"/> is returned and <paramref name="dataSize"/> is updated to contain the
        /// minimum buffer size needed to capture the calibration data.
        /// </returns>
        /// <remarks>The raw calibration may not exist if the device was not specified during recording.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_raw_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackGetRawCalibration(
            NativeHandles.PlaybackHandle playbackHandle,
            [Out] byte[] data,
            ref UIntPtr dataSize);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_get_calibration(k4a_playback_t playback_handle,
        //                                                            k4a_calibration_t* calibration);
        /// <summary>
        /// Get the camera calibration for Azure Kinect device used during recording.
        /// The output struct is used as input to all transformation functions.
        /// </summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="calibration">Output: calibration data.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>The calibration may not exist if the device was not specified during recording.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackGetCalibration(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.Calibration calibration);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_get_record_configuration(k4a_playback_t playback_handle,
        //                                                                     k4a_record_configuration_t* config);
        /// <summary>Get the device configuration used during recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="config">Output: recording configuration.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="config"/> was successfully written. <see cref="NativeCallResults.Result.Failed"/> otherwise.</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_record_configuration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackGetRecordConfiguration(
            NativeHandles.PlaybackHandle playbackHandle,
            out RecordConfiguration config);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_tag(k4a_playback_t playback_handle,
        //                                                           const char *name,
        //                                                           char *value,
        //                                                           size_t *value_size);
        /// <summary>Read the value of a tag from a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="name">The name of the tag to read.</param>
        /// <param name="value">
        /// Location to write the tag value. If a <see langword="null"/> buffer is specified,
        /// <paramref name="valueSize"/> will be set to the size of buffer needed to store the string.
        /// </param>
        /// <param name="valueSize">
        /// On input, the size of the <paramref name="value"/> buffer. On output, this is set to the length of the tag value (including the null
        /// terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="value"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/> and the needed size of the <paramref name="value"/>
        /// buffer is returned in the <paramref name="valueSize"/> parameter.
        /// <see cref="NativeCallResults.BufferResult.Failed"/> is returned if the tag does not exist.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_tag", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern NativeCallResults.BufferResult PlaybackGetTag(
            NativeHandles.PlaybackHandle playbackHandle,
            string name,
            StringBuilder value,
            ref UIntPtr valueSize);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_set_color_conversion(k4a_playback_t playback_handle,
        //                                                                 k4a_image_format_t target_format);
        /// <summary>
        /// Set the image format that color captures will be converted to. By default the conversion format will be the same as
        /// the image format stored in the recording file, and no conversion will occur.
        /// </summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="targetFormat">The target image format to be returned in captures.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the format conversion is supported. <see cref="NativeCallResults.Result.Failed"/> otherwise.</returns>
        /// <remarks>
        /// After the color conversion format is set, all <see cref="NativeHandles.CaptureHandle"/> objects returned from the playback handle will have
        /// their color images converted to the <paramref name="targetFormat"/>.
        /// 
        /// Color format conversion occurs in the user-thread, so setting <paramref name="targetFormat"/> to anything other than the format
        /// stored in the file may significantly increase the latency of <see cref="PlaybackGetNextCapture"/> and
        /// <see cref="PlaybackGetPreviousCapture"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_set_color_conversion", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackSetColorConversion(
            NativeHandles.PlaybackHandle playbackHandle,
            Sensor.ImageFormat targetFormat);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_capture(k4a_playback_t playback_handle,
        //                                                                    k4a_capture_t* capture_handle);
        /// <summary>Read the next capture in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="captureHandle">If successful this contains a handle to a capture object.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a capture is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the end of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the next capture in sequence after the most recently returned capture.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp"/> will return the capture
        /// in the recording closest to the seek time with an image timestamp greater than or equal to the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousCapture(NativeHandles.PlaybackHandle, out NativeHandles.CaptureHandle)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the beginning of the stream and this method will return the first capture in the recording.
        /// 
        /// Capture objects returned by the playback API will always contain at least one image, but may have images missing if
        /// frames were dropped in the original recording. When calling <see cref="Sensor.NativeApi.CaptureGetColorImage(NativeHandles.CaptureHandle)"/>,
        /// <see cref="Sensor.NativeApi.CaptureGetDepthImage(NativeHandles.CaptureHandle)"/>, or <see cref="Sensor.NativeApi.CaptureGetIRImage(NativeHandles.CaptureHandle)"/>,
        /// the image should be checked for <see langword="null"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_next_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetNextCapture(
            NativeHandles.PlaybackHandle playbackHandle,
            out NativeHandles.CaptureHandle captureHandle);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_capture(k4a_playback_t playback_handle,
        //                                                                        k4a_capture_t* capture_handle);
        /// <summary>Read the previous capture in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="captureHandle">If successful this contains a handle to a capture object.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a capture is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the start of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the previous capture in sequence before the most recently returned capture.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp"/> will return the capture
        /// in the recording closest to the seek time with all image time stamps less than the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetNextCapture(NativeHandles.PlaybackHandle, out NativeHandles.CaptureHandle)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the end of the stream and this method will return the last capture in the recording.
        /// 
        /// Capture objects returned by the playback API will always contain at least one image, but may have images missing if
        /// frames were dropped in the original recording. When calling <see cref="Sensor.NativeApi.CaptureGetColorImage(NativeHandles.CaptureHandle)"/>,
        /// <see cref="Sensor.NativeApi.CaptureGetDepthImage(NativeHandles.CaptureHandle)"/>, or <see cref="Sensor.NativeApi.CaptureGetIRImage(NativeHandles.CaptureHandle)"/>,
        /// the image should be checked for <see langword="null"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_next_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetPreviousCapture(
            NativeHandles.PlaybackHandle playbackHandle,
            out NativeHandles.CaptureHandle captureHandle);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_imu_sample(k4a_playback_t playback_handle,
        //                                                                       k4a_imu_sample_t* imu_sample);
        /// <summary>Read the next IMU sample in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a sample is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the end of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the next IMU sample in sequence after the most recently returned sample.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Timestamp, PlaybackSeekOrigin)"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp greater than or equal to the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the beginning of the stream and this method will return the first IMU sample in the recording.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_next_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetNextImuSample(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.ImuSample imuSample);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_imu_sample(k4a_playback_t playback_handle,
        //                                                                           k4a_imu_sample_t* imu_sample);
        /// <summary>Read the previous IMU sample in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a sample is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the start of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the previous IMU sample in sequence before the most recently returned sample.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Timestamp, PlaybackSeekOrigin)"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp less than the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the beginning of the stream and this method will return the first IMU sample in the recording.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_previous_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetPreviousImuSample(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.ImuSample imuSample);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_seek_timestamp(k4a_playback_t playback_handle,
        //                                                           int64_t offset_usec,
        //                                                           k4a_playback_seek_origin_t origin);
        /// <summary>Seek to a specific timestamp within a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="offset">The timestamp offset to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">Specifies if the seek operation should be done relative to the beginning or end of the recording.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if the seek operation was successful, or <see cref="NativeCallResults.Result.Failed"/>
        /// if an error occurred. The current seek position is left unchanged if a failure is returned.
        /// </returns>
        /// <remarks>
        /// The first call to <see cref="PlaybackGetNextCapture(NativeHandles.PlaybackHandle, out NativeHandles.CaptureHandle)"/> after this method
        /// will return the first capture containing an image timestamp greater than or equal to the seek time.
        /// 
        /// The first call to <see cref="PlaybackGetPreviousCapture(NativeHandles.PlaybackHandle, out NativeHandles.CaptureHandle)"/> after this method
        /// will return the firs capture with all image timestamps less than the seek time.
        /// 
        /// The first call to <see cref="PlaybackGetNextImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp greater than or equal to the seek time.
        /// 
        /// The first call to <see cref="PlaybackGetPreviousImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp less than the seek time.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_seek_timestamp", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackSeekTimestamp(
            NativeHandles.PlaybackHandle playbackHandle,
            Timestamp offset,
            PlaybackSeekOrigin origin);

        // K4ARECORD_EXPORT uint64_t k4a_playback_get_last_timestamp_usec(k4a_playback_t playback_handle);
        /// <summary>Gets the last timestamp in a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(string, out NativeHandles.PlaybackHandle)"/>.</param>
        /// <returns>The timestamp of the last capture image or IMU sample.</returns>
        /// <remarks>Recordings start at timestamp <see cref="Timestamp.Zero"/>, and end at the timestamp returned by this method.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_playback_get_last_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern Timestamp PlaybackGetLastTimestamp(NativeHandles.PlaybackHandle playbackHandle);
    }
}
