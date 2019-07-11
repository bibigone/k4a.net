using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    /// <summary>DLL imports for most of native functions from <c>record.h</c> header file.</summary>
    internal static class NativeApi
    {
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
        /// number information. May be <see langword="null"/> if recording user-generated data.</param>
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
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_create", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern NativeCallResults.Result RecordCreate(
            [MarshalAs(UnmanagedType.LPStr)] string path,
            NativeHandles.DeviceHandle device,
            Sensor.DeviceConfiguration deviceConfiguration,
            out NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_tag(k4a_record_t recording_handle, const char *name, const char *value);
        /// <summary>Adds a tag to the recording. All tags need to be added before the recording header is written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <param name="name">The name of the tag to write.</param>
        /// <param name="value">The string value to store in the tag.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </remarks>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_tag", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern NativeCallResults.Result RecordAddTag(
            NativeHandles.RecordHandle recordingHandle,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_add_imu_track(k4a_record_t recording_handle);
        /// <summary>Adds the track header for recording IMU. The track needs to be added before the recording header is written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_add_imu_track", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordAddImuTrack(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_header(k4a_record_t recording_handle);
        /// <summary>Writes the recording header and metadata to file. This must be called before captures can be written.</summary>
        /// <param name="recordingHandle">The handle of a new recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        [DllImport(Sdk.RECORD_DLL_NAME, EntryPoint = "k4a_record_write_header", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result RecordWriteHeader(NativeHandles.RecordHandle recordingHandle);

        // K4ARECORD_EXPORT k4a_result_t k4a_record_write_capture(k4a_record_t recording_handle, k4a_capture_t capture_handle);
        /// <summary>
        /// Writes a camera capture to file.
        /// Captures must be written in increasing order of timestamp, and the file's header must already be written.
        /// </summary>
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
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
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
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
        /// <param name="recordingHandle">The handle of recording, obtained by <see cref="RecordCreate(string, NativeHandles.DeviceHandle, Sensor.DeviceConfiguration, out NativeHandles.RecordHandle)"/>.</param>
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
    }
}
