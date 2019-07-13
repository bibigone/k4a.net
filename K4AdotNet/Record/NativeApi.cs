using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    /// <summary>DLL imports for most of native functions from <c>record.h</c> and <c>playback.h</c> header files.</summary>
    internal static class NativeApi
    {
        #region record.h

        // K4ARECORD_EXPORT k4a_result_t k4a_record_create(const char* path,
        //                                                 k4a_device_t device,
        //                                                 const k4a_device_configuration_t device_config,
        //                                                 k4a_record_t *recording_handle);
        /// <summary>
        /// Opens a new recording file for writing.
        /// The file will be created if it doesn't exist, or overwritten if an existing file is specified.
        /// </summary>
        /// <param name="path">File system path for the new recording.</param>
        /// <param name="device">The Azure Kinect device that is being recorded. The device handle is used to store device calibration and serial
        /// number information. May be <see cref="NativeHandles.DeviceHandle.Zero"/> if recording user-generated data.</param>
        /// <param name="deviceConfiguration">The configuration the Azure Kinect device was started with.</param>
        /// <param name="recordingHandle">If successful, this contains a pointer to the new recording handle.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Streaming does not need to be started on the device at the time this function is called, but when it is started
        /// it should be started with the same configuration provided in <paramref name="deviceConfiguration"/>.
        /// 
        /// Subsequent calls to <see cref="RecordWriteCapture(NativeHandles.RecordHandle, NativeHandles.CaptureHandle)"/> will need to have images in the resolution and format defined
        /// in <paramref name="deviceConfiguration"/>.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordCreate(
            [In] byte[] path,
            NativeHandles.DeviceHandle device,
            Sensor.DeviceConfiguration deviceConfiguration,
            out NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_tag(k4a_record_t recording_handle, const char *name, const char *value);
        /// <summary>Adds a tag to the recording. All tags need to be added before the recording header is written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="name">The name of the tag to write.</param>
        /// <param name="value">The string value to store in the tag.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_tag", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddTag(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] name,
            [In] byte[] value);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_imu_track(k4a_record_t recording_handle);
        /// <summary>Adds the track header for recording IMU. The track needs to be added before the recording header is written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_imu_track", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddImuTrack(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_header(k4a_record_t recording_handle);
        /// <summary>Writes the recording header and metadata to file. This must be called before captures can be written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_header", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteHeader(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_capture(k4a_record_t recording_handle, k4a_capture_t capture_handle);
        /// <summary>
        /// Writes a camera capture to file.
        /// Captures must be written in increasing order of timestamp, and the file's header must already be written.
        /// </summary>
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="captureHandle">The handle of a capture to write to file.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// This method will write all images in the capture to the corresponding tracks in the recording file.
        /// If any of the images fail to write, other images will still be written before a failure is returned.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteCapture(NativeHandles.RecordHandle recordingHandle, NativeHandles.CaptureHandle captureHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_imu_sample(k4a_record_t recording_handle, k4a_imu_sample_t imu_sample);
        /// <summary>Writes an IMU sample to file.</summary>
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="imuSample">A structure containing the IMU sample data and time stamps.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Samples must be written in increasing order of timestamp, and the file's header must already be written.
        /// When writing IMU samples at the same time as captures, the samples should be within 1 second of the most recently
        /// written capture.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteImuSample(NativeHandles.RecordHandle recordingHandle, Sensor.ImuSample imuSample);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_flush(k4a_record_t recording_handle);
        /// <summary>Flushes all pending recording data to disk.</summary>
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// This method ensures that all data passed to the recording API prior to calling flush is written to disk.
        /// If continuing to write recording data, care must be taken to ensure no new time stamps are added from before the flush.
        /// 
        /// If an error occurs, best effort is made to flush as much data to disk as possible, but the integrity of the file is
        /// not guaranteed.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_flush", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordFlush(NativeHandles.RecordHandle recordingHandle);

        #endregion

        #region playback.h

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_open(const char *path, k4a_playback_t *playback_handle);
        /// <summary>Opens an existing recording file for reading.</summary>
        /// <param name="path">File system path of the existing recording.</param>
        /// <param name="playbackHandle">If successful, this contains a pointer to the recording handle.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackOpen(
            [In] byte[] path,
            out NativeHandles.PlaybackHandle playbackHandle);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_raw_calibration(k4a_playback_t playback_handle,
        //                                                                       uint8_t* data,
        //                                                                       size_t *data_size);
        /// <summary>Get the raw calibration blob for the Azure Kinect device used during recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_raw_calibration", CallingConvention = CallingConvention.Cdecl)]
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
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="calibration">Output: calibration data.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>The calibration may not exist if the device was not specified during recording.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackGetCalibration(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.Calibration calibration);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_get_record_configuration(k4a_playback_t playback_handle,
        //                                                                     k4a_record_configuration_t* config);
        /// <summary>Get the device configuration used during recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="config">Output: recording configuration.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="config"/> was successfully written. <see cref="NativeCallResults.Result.Failed"/> otherwise.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_record_configuration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackGetRecordConfiguration(
            NativeHandles.PlaybackHandle playbackHandle,
            out RecordConfiguration config);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_tag(k4a_playback_t playback_handle,
        //                                                           const char *name,
        //                                                           char *value,
        //                                                           size_t *value_size);
        /// <summary>Read the value of a tag from a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_tag", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackGetTag(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] name,
            [Out] byte[] value,
            ref UIntPtr valueSize);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_set_color_conversion(k4a_playback_t playback_handle,
        //                                                                 k4a_image_format_t target_format);
        /// <summary>
        /// Set the image format that color captures will be converted to. By default the conversion format will be the same as
        /// the image format stored in the recording file, and no conversion will occur.
        /// </summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_set_color_conversion", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackSetColorConversion(
            NativeHandles.PlaybackHandle playbackHandle,
            Sensor.ImageFormat targetFormat);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_capture(k4a_playback_t playback_handle,
        //                                                                    k4a_capture_t* capture_handle);
        /// <summary>Read the next capture in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_next_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetNextCapture(
            NativeHandles.PlaybackHandle playbackHandle,
            out NativeHandles.CaptureHandle captureHandle);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_capture(k4a_playback_t playback_handle,
        //                                                                        k4a_capture_t* capture_handle);
        /// <summary>Read the previous capture in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_previous_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetPreviousCapture(
            NativeHandles.PlaybackHandle playbackHandle,
            out NativeHandles.CaptureHandle captureHandle);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_imu_sample(k4a_playback_t playback_handle,
        //                                                                       k4a_imu_sample_t* imu_sample);
        /// <summary>Read the next IMU sample in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a sample is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the end of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the next IMU sample in sequence after the most recently returned sample.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Microseconds64, PlaybackSeekOrigin)"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp greater than or equal to the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the beginning of the stream and this method will return the first IMU sample in the recording.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_next_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetNextImuSample(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.ImuSample imuSample);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_imu_sample(k4a_playback_t playback_handle,
        //                                                                           k4a_imu_sample_t* imu_sample);
        /// <summary>Read the previous IMU sample in the recording sequence.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a sample is returned, or <see cref="NativeCallResults.StreamResult.Eof"/>
        /// if the start of the recording is reached. All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the previous IMU sample in sequence before the most recently returned sample.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Microseconds64, PlaybackSeekOrigin)"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp less than the seek time.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousImuSample(NativeHandles.PlaybackHandle, out Sensor.ImuSample)"/> that returned <see cref="NativeCallResults.StreamResult.Eof"/>, the playback
        /// position is at the beginning of the stream and this method will return the first IMU sample in the recording.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_previous_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetPreviousImuSample(
            NativeHandles.PlaybackHandle playbackHandle,
            out Sensor.ImuSample imuSample);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_seek_timestamp(k4a_playback_t playback_handle,
        //                                                           int64_t offset_usec,
        //                                                           k4a_playback_seek_origin_t origin);
        /// <summary>Seek to a specific timestamp within a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_seek_timestamp", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackSeekTimestamp(
            NativeHandles.PlaybackHandle playbackHandle,
            Microseconds64 offset,
            PlaybackSeekOrigin origin);

        // K4ARECORD_EXPORT uint64_t k4a_playback_get_last_timestamp_usec(k4a_playback_t playback_handle);
        /// <summary>Gets the last timestamp in a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <returns>The timestamp of the last capture image or IMU sample.</returns>
        /// <remarks>Recordings start at timestamp <see cref="Microseconds64.Zero"/>, and end at the timestamp returned by this method.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_last_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern Microseconds64 PlaybackGetLastTimestamp(NativeHandles.PlaybackHandle playbackHandle);

        #endregion
    }
}
