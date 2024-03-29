﻿using System;
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
        /// </summary>
        /// <param name="path">File system path for the new recording.</param>
        /// <param name="device">The Azure Kinect device that is being recorded. The device handle is used to store device calibration and serial
        /// number information. May be <see cref="IntPtr.Zero"/> if recording user-generated data.</param>
        /// <param name="deviceConfiguration">The configuration the Azure Kinect device was started with.</param>
        /// <param name="recordingHandle">If successful, this contains a pointer to the new recording handle.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// The file will be created if it doesn't exist, or overwritten if an existing file is specified.
        ///
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
        /// <summary>Adds a tag to the recording.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="name">The name of the tag to write.</param>
        /// <param name="value">The string value to store in the tag.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// 
        /// Tag names must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.
        /// 
        /// All tags need to be added before the recording header is written.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_tag", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddTag(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] name,
            [In] byte[] value);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_imu_track(k4a_record_t recording_handle);
        /// <summary>Adds the track header for recording IMU.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>The track needs to be added before the recording header is written.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_imu_track", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddImuTrack(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_attachment(const k4a_record_t recording_handle,
        //                                                         const char *attachment_name,
        //                                                         const uint8_t *buffer,
        //                                                         size_t buffer_size);
        /// <summary>Adds an attachment to the recording.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="attachmentName">The name of the attachment to be stored in the recording file. This name should be a valid filename with an extension.</param>
        /// <param name="buffer">The attachment data buffer.</param>
        /// <param name="bufferSize">The size of the attachment data buffer.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>All attachments need to be added before the recording header is written.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_attachment", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddAttachment(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] attachmentName,
            [In] byte[] buffer,
            UIntPtr bufferSize);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_custom_video_track(const k4a_record_t recording_handle,
        //                                                                 const char *track_name,
        //                                                                 const char *codec_id,
        //                                                                 const uint8_t *codec_context,
        //                                                                 size_t codec_context_size,
        //                                                                 const k4a_record_video_settings_t *track_settings);
        /// <summary>Adds custom video tracks to the recording.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="trackName">The name of the custom video track to be added.</param>
        /// <param name="codecId">
        /// A UTF8 null terminated string containing the codec ID of the track. Some of the existing formats are listed here:
        /// https://www.matroska.org/technical/specs/codecid/index.html. The codec ID can also be custom defined by the user.
        /// Video codec ID's should start with 'V_'.
        /// </param>
        /// <param name="codecContext">
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element.
        /// </param>
        /// <param name="codecContextSize">The size of the codec context buffer.</param>
        /// <param name="trackSettings">Additional metadata for the video track such as resolution and frame rate.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Built-in video tracks like the DEPTH, IR, and COLOR tracks will be created automatically when the <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>
        /// API is called. This API can be used to add additional video tracks to save custom data.
        /// 
        /// Track names must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.
        /// 
        /// All tracks need to be added before the recording header is written.
        /// 
        /// Call <see cref="RecordWriteCustomTrackData"/> with the same <paramref name="trackName"/> to write data to this track.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_custom_video_track", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddCustomVideoTrack(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] trackName,
            [In] byte[] codecId,
            [In] byte[] codecContext,
            UIntPtr codecContextSize,
            in RecordVideoSettings trackSettings);

        // K4ARECORD_EXPORT k4a_result_t
        // k4a_record_add_custom_subtitle_track(const k4a_record_t recording_handle,
        //                                      const char *track_name,
        //                                      const char *codec_id,
        //                                      const uint8_t *codec_context,
        //                                      size_t codec_context_size,
        //                                      const k4a_record_subtitle_settings_t *track_settings);
        /// <summary>Adds custom subtitle tracks to the recording.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="trackName">The name of the custom subtitle track to be added.</param>
        /// <param name="codecId">
        /// A UTF8 null terminated string containing the codec ID of the track. Some of the existing formats are listed here:
        /// https://www.matroska.org/technical/specs/codecid/index.html. The codec ID can also be custom defined by the user.
        /// Subtitle codec ID's should start with 'S_'.
        /// </param>
        /// <param name="codecContext">
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element.
        /// </param>
        /// <param name="codecContextSize">The size of the codec context buffer.</param>
        /// <param name="trackSettings">Additional metadata for the subtitle track.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Built-in subtitle tracks like the IMU track will be created automatically when the <see cref="RecordAddImuTrack(NativeHandles.RecordHandle)"/> API is
        /// called. This API can be used to add additional subtitle tracks to save custom data.
        /// 
        /// Track names must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.
        /// 
        /// All tracks need to be added before the recording header is written.
        /// 
        /// Call <see cref="RecordWriteCustomTrackData"/> with the same <paramref name="trackName"/> to write data to this track.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_custom_subtitle_track", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddCustomSubtitleTrack(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] trackName,
            [In] byte[] codecId,
            [In] byte[] codecContext,
            UIntPtr codecContextSize,
            in RecordSubtitleSettings trackSettings);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_header(k4a_record_t recording_handle);
        /// <summary>Writes the recording header and metadata to file.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>This must be called before captures can be written.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_header", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteHeader(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_capture(k4a_record_t recording_handle, k4a_capture_t capture_handle);
        /// <summary>Writes a camera capture to file.</summary>
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="captureHandle">The handle of a capture to write to file.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// This method will write all images in the capture to the corresponding tracks in the recording file.
        /// If any of the images fail to write, other images will still be written before a failure is returned.
        /// 
        /// Captures must be written in increasing order of timestamp, and the file's header must already be written.
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

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_custom_track_data(const k4a_record_t recording_handle,
        //                                                                  const char *track_name,
        //                                                                  uint64_t device_timestamp_usec,
        //                                                                  uint8_t *custom_data,
        //                                                                  size_t custom_data_size);
        /// <summary>Writes data for a custom track to file.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(byte[], NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="trackName">The name of the custom track that the data is going to be written to.</param>
        /// <param name="deviceTimestamp">
        /// The timestamp in microseconds for the custom track data. This timestamp should be in the same time domain as the
        /// device timestamp used for recording.
        /// </param>
        /// <param name="customData">The buffer of custom track data.</param>
        /// <param name="customDataSize">The size of the custom track data buffer.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Custom track data must be written in increasing order of timestamp, and the file's header must already be written.
        /// When writing custom track data at the same time as captures or IMU data, the custom data should be within 1 second of
        /// the most recently written timestamp.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_custom_track_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteCustomTrackData(
            NativeHandles.RecordHandle recordingHandle,
            [In] byte[] trackName,
            Microseconds64 deviceTimestamp,
            [In] byte[] customData,
            UIntPtr customDataSize);

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
        /// <param name="buffer">
        /// Location to write the calibration data to. This field may optionally be set to <see langword="null"/>
        /// if the caller wants to query for the needed data size.
        /// </param>
        /// <param name="size">
        /// On passing <paramref name="buffer"/> into the function this variable represents the available size to write the raw data to. On
        /// return this variable is updated with the amount of data actually written to the buffer.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.BufferResult.Succeeded"/> if <paramref name="buffer"/> was successfully written.
        /// If <paramref name="buffer"/> points to a buffer size that is too small to hold the output,
        /// <see cref="NativeCallResults.BufferResult.TooSmall"/> is returned and <paramref name="size"/> is updated to contain the
        /// minimum buffer size needed to capture the calibration data.
        /// </returns>
        /// <remarks>The raw calibration may not exist if the device was not specified during recording.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_raw_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackGetRawCalibration(
            NativeHandles.PlaybackHandle playbackHandle,
            IntPtr buffer,
            ref UIntPtr size);

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

        // K4ARECORD_EXPORT bool k4a_playback_check_track_exists(k4a_playback_t playback_handle, const char* track_name);
        /// <summary>Checks whether a track with the given track name exists in the playback file.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The track name to be checked to see whether it exists or not.</param>
        /// <returns><see langword="true"/> if the track exists.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_check_track_exists", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte PlaybackCheckTrackExists(
            NativeHandles.PlaybackHandle playbackHandle,
            byte[] trackName);

        // K4ARECORD_EXPORT size_t k4a_playback_get_track_count(k4a_playback_t playback_handle);
        /// <summary>Get the number of tracks in a playback file.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <returns>The number of tracks in the playback file.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_track_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr PlaybackGetTrackCount(NativeHandles.PlaybackHandle playbackHandle);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_track_name(k4a_playback_t playback_handle,
        //                                                                  size_t track_index,
        //                                                                  char* track_name,
        //                                                                  size_t* track_name_size);
        /// <summary>Gets the name of a track at a specific index.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackIndex">The index of the track to read the name form.</param>
        /// <param name="buffer">
        /// Location to write the track name. This will be a UTF8 null terminated string. If a <see langword="null"/> buffer is specified,
        /// <paramref name="size"/> will be set to the size of buffer needed to store the string.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer. On output, this is set to the length of the <paramref name="buffer"/> value
        /// (including the 0 terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/>
        /// and the needed size of the <paramref name="buffer"/> buffer is returned in the <paramref name="size"/> parameter.
        /// <see cref="NativeCallResults.BufferResult.Failed"/> is returned if the track index does not exist.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_track_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackGetTrackName(
            NativeHandles.PlaybackHandle playbackHandle,
            UIntPtr trackIndex,
            IntPtr buffer,
            ref UIntPtr size);

        // K4ARECORD_EXPORT bool k4a_playback_track_is_builtin(k4a_playback_t playback_handle, const char* track_name);
        /// <summary>Checks whether a track is one of the built-in tracks: "COLOR", "DEPTH", etc...</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The track name to be checked to see whether it is a built-in track.</param>
        /// <returns><see langword="true"/> if the track is built-in. If the provided track name does not exist, <see langword="false"/> will be returned.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_track_is_builtin", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte PlaybackTrackIsBuiltIn(NativeHandles.PlaybackHandle playbackHandle, [In] byte[] trackName);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_track_get_video_settings(k4a_playback_t playback_handle,
        //                                                                     const char* track_name,
        //                                                                     k4a_record_video_settings_t *video_settings);
        /// <summary>Gets the video-specific track information for a particular video track.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The track name to read video settings from.</param>
        /// <param name="videoSettings">Location to write the track's video settings.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> is returned on success,
        /// <see cref="NativeCallResults.Result.Failed"/> is returned if the specified track does not exist or is not a video track.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_track_get_video_settings", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result PlaybackTrackGetVideoSetting(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] trackName,
            out RecordVideoSettings videoSettings);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_track_get_codec_id(k4a_playback_t playback_handle,
        //                                                                      const char* track_name,
        //                                                                      char* codec_id,
        //                                                                      size_t *codec_id_size);
        /// <summary>Gets the codec id string for a particular track.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The track name to read the codec id from.</param>
        /// <param name="buffer">
        /// Location to write the codec id. This will be a UTF8 null terminated string. If a <see langword="null"/> buffer is specified,
        /// <paramref name="size"/> will be set to the size of buffer needed to store the string.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer. On output, this is set to the length of the <paramref name="buffer"/> value
        /// (including the 0 terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/>
        /// and the needed size of the <paramref name="buffer"/> buffer is returned in the <paramref name="size"/> parameter.
        /// <see cref="NativeCallResults.BufferResult.Failed"/> is returned if the <paramref name="trackName"/> does not exist.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// The codec ID is a string that corresponds to the codec of the track's data. Some of the existing formats are listed
        /// here: https://www.matroska.org/technical/specs/codecid/index.html. It can also be custom defined by the user.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_track_get_codec_id", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackTrackGetCodecId(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] trackName,
            IntPtr buffer,
            ref UIntPtr size);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_track_get_codec_context(k4a_playback_t playback_handle,
        //                                                                           const char* track_name,
        //                                                                           uint8_t *codec_context,
        //                                                                           size_t* codec_context_size);
        /// <summary>Gets the codec context for a particular track.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The track name to read the codec id from.</param>
        /// <param name="buffer">
        /// Location to write the codec context data. If a <see langword="null"/> buffer is specified,
        /// <paramref name="size"/> will be set to the size of buffer needed to store the data.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer. On output, this is set to the length of the <paramref name="buffer"/> data.
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/>
        /// and the needed size of the <paramref name="buffer"/> buffer is returned in the <paramref name="size"/> parameter.
        /// <see cref="NativeCallResults.BufferResult.Failed"/> is returned if the <paramref name="trackName"/> does not exist.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec.It is mapped to the matroska Codec Private field.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_track_get_codec_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackTrackGetCodecContext(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] trackName,
            IntPtr buffer,
            ref UIntPtr size);

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_tag(k4a_playback_t playback_handle,
        //                                                           const char *name,
        //                                                           char *value,
        //                                                           size_t *value_size);
        /// <summary>Read the value of a tag from a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="name">The name of the tag to read.</param>
        /// <param name="buffer">
        /// Location to write the tag value. If a <see cref="IntPtr.Zero"/> buffer is specified,
        /// <paramref name="size"/> will be set to the size of buffer needed to store the string.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer. On output, this is set to the length of the tag value (including the null
        /// terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/> and the needed size of the <paramref name="buffer"/>
        /// buffer is returned in the <paramref name="size"/> parameter.
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
            IntPtr buffer,
            ref UIntPtr size);

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

        // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_attachment(k4a_playback_t playback_handle,
        //                                                                  const char* file_name,
        //                                                                  uint8_t *data,
        //                                                                  size_t* data_size);
        /// <summary>Reads an attachment file from a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="fileName">Attachment file name.</param>
        /// <param name="buffer">
        /// Location to write the attachment data. If a <see langword="null"/> buffer is specified, <paramref name="size"/> will be set to the size of
        /// buffer needed to store the data.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer. On output, this is set to the length of the attachment data.
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/>
        /// and the needed size of the <paramref name="buffer"/> buffer is returned in the <paramref name="size"/> parameter.
        /// <see cref="NativeCallResults.BufferResult.Failed"/> is returned if the attachment <paramref name="fileName"/> does not exist.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_attachment", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.BufferResult PlaybackGetAttachment(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] fileName,
            IntPtr buffer,
            ref UIntPtr size);

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

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_data_block(k4a_playback_t playback_handle,
        //                                                                       const char* track_name,
        //                                                                       k4a_playback_data_block_t *data_block_handle);
        /// <summary>Read the next data block for a particular track.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The name of the track to read the next data block from.</param>
        /// <param name="dataHandle">The location to write the data block handle.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a data block is returned,
        /// or <see cref="NativeCallResults.StreamResult.Eof"/> if the end of the recording is reached.
        /// All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the data block after the most recently returned data block for a particular track.
        /// 
        /// If a call was made to <see cref="PlaybackGetPreviousDataBlock"/> which returned <see cref="NativeCallResults.StreamResult.Eof"/>, then the
        /// playback position is at the beginning of the recording and calling this method with the same
        /// track will return the first data block in the track.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Microseconds64, PlaybackSeekOrigin)"/>
        /// will return the data block in the recording closest to the seek time with a timestamp greater than or equal to the seek time.
        /// 
        /// This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...
        /// <see cref="PlaybackTrackIsBuiltIn(NativeHandles.PlaybackHandle, byte[])"/> can be used to determine if a track is a built-in track.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_next_data_block", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetNextDataBlock(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] trackName,
            out NativeHandles.PlaybackDataBlockHandle dataHandle);

        // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_data_block(k4a_playback_t playback_handle,
        //                                                                           const char* track_name,
        //                                                                           k4a_playback_data_block_t *data_block_handle);
        /// <summary>Read the previous data block for a particular track.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="trackName">The name of the track to read the previous data block from.</param>
        /// <param name="dataHandle">The location to write the data block handle.</param>
        /// <returns>
        /// <see cref="NativeCallResults.StreamResult.Succeeded"/> if a data block is returned,
        /// or <see cref="NativeCallResults.StreamResult.Eof"/> if the start of the recording is reached.
        /// All other failures will return <see cref="NativeCallResults.StreamResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This method always returns the data block before the most recently returned data block for a particular track.
        /// 
        /// If a call was made to <see cref="PlaybackGetNextDataBlock"/> which returned <see cref="NativeCallResults.StreamResult.Eof"/>, then the
        /// playback position is at the end of the recording and calling this method with the same
        /// track will return the last data block in the track.
        /// 
        /// The first call to this method after <see cref="PlaybackSeekTimestamp(NativeHandles.PlaybackHandle, Microseconds64, PlaybackSeekOrigin)"/>
        /// will return the data block in the recording closest to the seek time with a timestamp less than the seek time.
        /// 
        /// This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...
        /// <see cref="PlaybackTrackIsBuiltIn(NativeHandles.PlaybackHandle, byte[])"/> can be used to determine if a track is a built-in track.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_previous_data_block", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.StreamResult PlaybackGetPreviousDataBlock(
            NativeHandles.PlaybackHandle playbackHandle,
            [In] byte[] trackName,
            out NativeHandles.PlaybackDataBlockHandle dataHandle);

        // K4ARECORD_EXPORT uint64_t k4a_playback_data_block_get_device_timestamp_usec(k4a_playback_data_block_t data_block_handle);
        /// <summary>Get the device timestamp of a data block in microseconds.</summary>
        /// <param name="dataBlockHandle">
        /// Handle obtained by <see cref="PlaybackGetNextDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>
        /// or <see cref="PlaybackGetPreviousDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>.
        /// </param>
        /// <returns>
        /// Returns the device timestamp of the data block. If the <paramref name="dataBlockHandle"/> is invalid this function will return <see cref="Microseconds64.Zero"/>.
        /// It is also possible for <see cref="Microseconds64.Zero"/> to be a valid timestamp originating from when a device was first powered on.
        /// </returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_data_block_get_device_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern Microseconds64 PlaybackDataBlockGetDeviceTimestamp(NativeHandles.PlaybackDataBlockHandle dataBlockHandle);

        // K4ARECORD_EXPORT size_t k4a_playback_data_block_get_buffer_size(k4a_playback_data_block_t data_block_handle);
        /// <summary>Get the buffer size of a data block.</summary>
        /// <param name="dataBlockHandle">
        /// Handle obtained by <see cref="PlaybackGetNextDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>
        /// or <see cref="PlaybackGetPreviousDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>.
        /// </param>
        /// <returns>
        /// Returns the buffer size of the data block, or 0 if the data block is invalid.
        /// </returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_data_block_get_buffer_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr PlaybackDataBlockGetBufferSize(NativeHandles.PlaybackDataBlockHandle dataBlockHandle);

        // K4ARECORD_EXPORT uint8_t *k4a_playback_data_block_get_buffer(k4a_playback_data_block_t data_block_handle);
        /// <summary>Get the buffer of a data block.</summary>
        /// <param name="dataBlockHandle">
        /// Handle obtained by <see cref="PlaybackGetNextDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>
        /// or <see cref="PlaybackGetPreviousDataBlock(NativeHandles.PlaybackHandle, byte[], out NativeHandles.PlaybackDataBlockHandle)"/>.
        /// </param>
        /// <returns>
        /// Returns a pointer to the data block buffer, or <see cref="IntPtr.Zero"/> if the data block is invalid.
        /// </returns>
        /// <remarks>Use this buffer to access the data written to a custom recording track.</remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_data_block_get_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PlaybackDataBlockGetBuffer(NativeHandles.PlaybackDataBlockHandle dataBlockHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_playback_seek_timestamp(k4a_playback_t playback_handle,
        //                                                           int64_t offset_usec,
        //                                                           k4a_playback_seek_origin_t origin);
        /// <summary>Seek to a specific timestamp within a recording.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <param name="offset">The timestamp offset to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">
        /// Specifies how the given timestamp should be interpreted. Seek can be done relative to the beginning or end of the
        /// recording, or using an absolute device timestamp.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if the seek operation was successful, or <see cref="NativeCallResults.Result.Failed"/>
        /// if an error occurred. The current seek position is left unchanged if a failure is returned.
        /// </returns>
        /// <remarks>
        /// The first device timestamp in a recording is usually non-zero. The recording file starts at the device timestamp
        /// defined by <see cref="RecordConfiguration.StartTimeOffset"/>,
        /// which is accessible via <see cref="PlaybackGetRecordConfiguration(NativeHandles.PlaybackHandle, out RecordConfiguration)"/>.
        ///
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

        // K4ARECORD_EXPORT uint64_t k4a_playback_get_recording_length_usec(k4a_playback_t playback_handle);
        /// <summary>Returns the length of the recording in microseconds.</summary>
        /// <param name="playbackHandle">Handle obtained by <see cref="PlaybackOpen(byte[], out NativeHandles.PlaybackHandle)"/>.</param>
        /// <returns>The recording length, calculated as the difference between the first and last timestamp in the file.</returns>
        /// <remarks>
        /// The recording length may be longer than an individual track if, for example, the IMU continues to run after the last
        /// color image is recorded.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_playback_get_recording_length_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern Microseconds64 PlaybackGetRecordingLength(NativeHandles.PlaybackHandle playbackHandle);

        #endregion
    }
}
