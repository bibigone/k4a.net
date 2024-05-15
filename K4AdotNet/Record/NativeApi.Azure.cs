using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    partial class NativeApi
    {
        public sealed class Azure : NativeApi
        {
            public static readonly Azure Instance = new();

            private Azure() { }

            public override bool IsOrbbec => false;

            public override NativeCallResults.Result RecordCreate(
                byte[] path,
                NativeHandles.DeviceHandle device,
                Sensor.DeviceConfiguration deviceConfiguration,
                out NativeHandles.RecordHandle? recordingHandle)
            {
                var res = k4a_record_create(path, (NativeHandles.DeviceHandle.Azure)device, deviceConfiguration, out var h);
                recordingHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_result_t k4a_record_create(const char* path,
            //                                                 k4a_device_t device,
            //                                                 const k4a_device_configuration_t device_config,
            //                                                 k4a_record_t *recording_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_create(
                [In] byte[] path,
                NativeHandles.DeviceHandle.Azure device,
                Sensor.DeviceConfiguration deviceConfiguration,
                out NativeHandles.RecordHandle.Azure? recordingHandle);

            public override NativeCallResults.Result RecordAddTag(
                NativeHandles.RecordHandle recordingHandle,
                byte[] name,
                byte[] value)
                => k4a_record_add_tag((NativeHandles.RecordHandle.Azure)recordingHandle, name, value);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_add_tag(k4a_record_t recording_handle, const char *name, const char *value);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_add_tag(
                NativeHandles.RecordHandle.Azure recordingHandle,
                [In] byte[] name,
                [In] byte[] value);

            public override NativeCallResults.Result RecordAddImuTrack(NativeHandles.RecordHandle recordingHandle)
                => k4a_record_add_imu_track((NativeHandles.RecordHandle.Azure)recordingHandle);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_add_imu_track(k4a_record_t recording_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_add_imu_track(NativeHandles.RecordHandle.Azure recordingHandle);

            public override NativeCallResults.Result RecordAddAttachment(
                NativeHandles.RecordHandle recordingHandle,
                byte[] attachmentName,
                byte[] buffer,
                UIntPtr bufferSize)
                => k4a_record_add_attachment((NativeHandles.RecordHandle.Azure)recordingHandle, attachmentName, buffer, bufferSize);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_add_attachment(const k4a_record_t recording_handle,
            //                                                         const char *attachment_name,
            //                                                         const uint8_t *buffer,
            //                                                         size_t buffer_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_add_attachment(
                NativeHandles.RecordHandle.Azure recordingHandle,
                [In] byte[] attachmentName,
                [In] byte[] buffer,
                UIntPtr bufferSize);

            public override NativeCallResults.Result RecordAddCustomVideoTrack(
                NativeHandles.RecordHandle recordingHandle,
                byte[] trackName,
                byte[] codecId,
                byte[] codecContext,
                UIntPtr codecContextSize,
                in RecordVideoSettings trackSettings)
                => k4a_record_add_custom_video_track(
                    (NativeHandles.RecordHandle.Azure)recordingHandle,
                    trackName, codecId, codecContext, codecContextSize, in trackSettings);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_add_custom_video_track(const k4a_record_t recording_handle,
            //                                                                 const char *track_name,
            //                                                                 const char *codec_id,
            //                                                                 const uint8_t *codec_context,
            //                                                                 size_t codec_context_size,
            //                                                                 const k4a_record_video_settings_t *track_settings);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_add_custom_video_track(
                NativeHandles.RecordHandle.Azure recordingHandle,
                [In] byte[] trackName,
                [In] byte[] codecId,
                [In] byte[] codecContext,
                UIntPtr codecContextSize,
                in RecordVideoSettings trackSettings);

            public override NativeCallResults.Result RecordAddCustomSubtitleTrack(
                NativeHandles.RecordHandle recordingHandle,
                byte[] trackName,
                byte[] codecId,
                byte[] codecContext,
                UIntPtr codecContextSize,
                in RecordSubtitleSettings trackSettings)
                => k4a_record_add_custom_subtitle_track((NativeHandles.RecordHandle.Azure)recordingHandle,
                    trackName, codecId, codecContext, codecContextSize, in trackSettings);

            // K4ARECORD_EXPORT k4a_result_t
            // k4a_record_add_custom_subtitle_track(const k4a_record_t recording_handle,
            //                                      const char *track_name,
            //                                      const char *codec_id,
            //                                      const uint8_t *codec_context,
            //                                      size_t codec_context_size,
            //                                      const k4a_record_subtitle_settings_t *track_settings);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_add_custom_subtitle_track(
                NativeHandles.RecordHandle.Azure recordingHandle,
                [In] byte[] trackName,
                [In] byte[] codecId,
                [In] byte[] codecContext,
                UIntPtr codecContextSize,
                in RecordSubtitleSettings trackSettings);

            public override NativeCallResults.Result RecordWriteHeader(NativeHandles.RecordHandle recordingHandle)
                => k4a_record_write_header((NativeHandles.RecordHandle.Azure)recordingHandle);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_write_header(k4a_record_t recording_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_write_header(NativeHandles.RecordHandle.Azure recordingHandle);

            public override NativeCallResults.Result RecordWriteCapture(NativeHandles.RecordHandle recordingHandle, NativeHandles.CaptureHandle captureHandle)
                => k4a_record_write_capture((NativeHandles.RecordHandle.Azure)recordingHandle, (NativeHandles.CaptureHandle.Azure)captureHandle);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_write_capture(k4a_record_t recording_handle, k4a_capture_t capture_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_write_capture(NativeHandles.RecordHandle.Azure recordingHandle, NativeHandles.CaptureHandle.Azure captureHandle);

            public override NativeCallResults.Result RecordWriteImuSample(NativeHandles.RecordHandle recordingHandle, Sensor.ImuSample imuSample)
                => k4a_record_write_imu_sample((NativeHandles.RecordHandle.Azure)recordingHandle, imuSample);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_write_imu_sample(k4a_record_t recording_handle, k4a_imu_sample_t imu_sample);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_write_imu_sample(NativeHandles.RecordHandle.Azure recordingHandle, Sensor.ImuSample imuSample);

            public override NativeCallResults.Result RecordWriteCustomTrackData(
                NativeHandles.RecordHandle recordingHandle,
                byte[] trackName,
                Microseconds64 deviceTimestamp,
                byte[] customData,
                UIntPtr customDataSize)
                => k4a_record_write_custom_track_data((NativeHandles.RecordHandle.Azure)recordingHandle,
                    trackName, deviceTimestamp, customData, customDataSize);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_write_custom_track_data(const k4a_record_t recording_handle,
            //                                                                  const char *track_name,
            //                                                                  uint64_t device_timestamp_usec,
            //                                                                  uint8_t *custom_data,
            //                                                                  size_t custom_data_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_write_custom_track_data(
                NativeHandles.RecordHandle.Azure recordingHandle,
                [In] byte[] trackName,
                Microseconds64 deviceTimestamp,
                [In] byte[] customData,
                UIntPtr customDataSize);

            public override NativeCallResults.Result RecordFlush(NativeHandles.RecordHandle recordingHandle)
                => k4a_record_flush((NativeHandles.RecordHandle.Azure)recordingHandle);

            // K4ARECORD_EXPORT k4a_result_t k4a_record_flush(k4a_record_t recording_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_record_flush(NativeHandles.RecordHandle.Azure recordingHandle);

            public override NativeCallResults.Result PlaybackGetRecordConfiguration(
                NativeHandles.PlaybackHandle playbackHandle,
                out RecordConfiguration config)
            {
                var res = k4a_playback_get_record_configuration((NativeHandles.PlaybackHandle.Azure)playbackHandle, out var cfg);
                config = new()
                {
                    CameraFps = cfg.CameraFps,
                    ColorFormat = cfg.ColorFormat,
                    ColorResolution = cfg.ColorResolution,
                    ColorTrackEnabled = cfg.ColorTrackEnabled,
                    DepthDelayOffColor = cfg.DepthDelayOffColor,
                    DepthMode = cfg.DepthMode,
                    DepthTrackEnabled = cfg.DepthTrackEnabled,
                    ImuTrackEnabled = cfg.ImuTrackEnabled,
                    IRTrackEnabled = cfg. IRTrackEnabled,
                    StartTimeOffset = cfg.StartTimeOffset.ValueUsec,
                    SubordinateDelayOffMaster = cfg.SubordinateDelayOffMaster,
                    WiredSyncMode = cfg.WiredSyncMode,
                };
                return res;
            }

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_get_record_configuration(k4a_playback_t playback_handle,
            //                                                                     k4a_record_configuration_t* config);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_get_record_configuration(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out RecordConfigurationAzure config);

            public override NativeCallResults.Result PlaybackOpen(
                byte[] path,
                out NativeHandles.PlaybackHandle? playbackHandle)
            {
                var res = k4a_playback_open(path, out var h);
                playbackHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_open(const char *path, k4a_playback_t *playback_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_open(
                [In] byte[] path,
                out NativeHandles.PlaybackHandle.Azure? playbackHandle);

            public override NativeCallResults.BufferResult PlaybackGetRawCalibration(
                NativeHandles.PlaybackHandle playbackHandle,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_get_raw_calibration((NativeHandles.PlaybackHandle.Azure)playbackHandle, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_raw_calibration(k4a_playback_t playback_handle,
            //                                                                       uint8_t* data,
            //                                                                       size_t *data_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_get_raw_calibration(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result PlaybackGetCalibration(
                NativeHandles.PlaybackHandle playbackHandle,
                out Sensor.CalibrationData calibration)
                => k4a_playback_get_calibration((NativeHandles.PlaybackHandle.Azure)playbackHandle, out calibration);

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_get_calibration(k4a_playback_t playback_handle,
            //                                                            k4a_calibration_t* calibration);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_get_calibration(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out Sensor.CalibrationData calibration);

            public override byte PlaybackCheckTrackExists(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName)
                => k4a_playback_check_track_exists((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName);

            // K4ARECORD_EXPORT bool k4a_playback_check_track_exists(k4a_playback_t playback_handle, const char* track_name);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern byte k4a_playback_check_track_exists(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                byte[] trackName);

            public override UIntPtr PlaybackGetTrackCount(NativeHandles.PlaybackHandle playbackHandle)
                => k4a_playback_get_track_count((NativeHandles.PlaybackHandle.Azure)playbackHandle);

            // K4ARECORD_EXPORT size_t k4a_playback_get_track_count(k4a_playback_t playback_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern UIntPtr k4a_playback_get_track_count(NativeHandles.PlaybackHandle.Azure playbackHandle);

            public override NativeCallResults.BufferResult PlaybackGetTrackName(
                NativeHandles.PlaybackHandle playbackHandle,
                UIntPtr trackIndex,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_get_track_name((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackIndex, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_track_name(k4a_playback_t playback_handle,
            //                                                                  size_t track_index,
            //                                                                  char* track_name,
            //                                                                  size_t* track_name_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_get_track_name(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                UIntPtr trackIndex,
                IntPtr buffer,
                ref UIntPtr size);

            public override byte PlaybackTrackIsBuiltIn(NativeHandles.PlaybackHandle playbackHandle, byte[] trackName)
                => k4a_playback_track_is_builtin((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName);

            // K4ARECORD_EXPORT bool k4a_playback_track_is_builtin(k4a_playback_t playback_handle, const char* track_name);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern byte k4a_playback_track_is_builtin(NativeHandles.PlaybackHandle.Azure playbackHandle, [In] byte[] trackName);

            public override NativeCallResults.Result PlaybackTrackGetVideoSetting(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName,
                out RecordVideoSettings videoSettings)
                => k4a_playback_track_get_video_settings((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName, out videoSettings);

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_track_get_video_settings(k4a_playback_t playback_handle,
            //                                                                     const char* track_name,
            //                                                                     k4a_record_video_settings_t *video_settings);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_track_get_video_settings(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] trackName,
                out RecordVideoSettings videoSettings);

            public override NativeCallResults.BufferResult PlaybackTrackGetCodecId(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_track_get_codec_id((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_track_get_codec_id(k4a_playback_t playback_handle,
            //                                                                      const char* track_name,
            //                                                                      char* codec_id,
            //                                                                      size_t *codec_id_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_track_get_codec_id(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] trackName,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.BufferResult PlaybackTrackGetCodecContext(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_track_get_codec_context((NativeHandles.PlaybackHandle.Azure)playbackHandle,
                    trackName, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_track_get_codec_context(k4a_playback_t playback_handle,
            //                                                                           const char* track_name,
            //                                                                           uint8_t *codec_context,
            //                                                                           size_t* codec_context_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_track_get_codec_context(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] trackName,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.BufferResult PlaybackGetTag(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] name,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_get_tag((NativeHandles.PlaybackHandle.Azure)playbackHandle,
                    name, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_tag(k4a_playback_t playback_handle,
            //                                                           const char *name,
            //                                                           char *value,
            //                                                           size_t *value_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_get_tag(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] name,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result PlaybackSetColorConversion(
                NativeHandles.PlaybackHandle playbackHandle,
                Sensor.ImageFormat targetFormat)
                => k4a_playback_set_color_conversion((NativeHandles.PlaybackHandle.Azure)playbackHandle, targetFormat);

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_set_color_conversion(k4a_playback_t playback_handle,
            //                                                                 k4a_image_format_t target_format);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_set_color_conversion(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                Sensor.ImageFormat targetFormat);

            public override NativeCallResults.BufferResult PlaybackGetAttachment(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] fileName,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_playback_get_attachment((NativeHandles.PlaybackHandle.Azure)playbackHandle, fileName, buffer, ref size);

            // K4ARECORD_EXPORT k4a_buffer_result_t k4a_playback_get_attachment(k4a_playback_t playback_handle,
            //                                                                  const char* file_name,
            //                                                                  uint8_t *data,
            //                                                                  size_t* data_size);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_playback_get_attachment(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] fileName,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.StreamResult PlaybackGetNextCapture(
                NativeHandles.PlaybackHandle playbackHandle,
                out NativeHandles.CaptureHandle? captureHandle)
            {
                var res = k4a_playback_get_next_capture((NativeHandles.PlaybackHandle.Azure)playbackHandle, out var h);
                captureHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_capture(k4a_playback_t playback_handle,
            //                                                                    k4a_capture_t* capture_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_next_capture(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out NativeHandles.CaptureHandle.Azure? captureHandle);

            public override NativeCallResults.StreamResult PlaybackGetPreviousCapture(
                NativeHandles.PlaybackHandle playbackHandle,
                out NativeHandles.CaptureHandle? captureHandle)
            {
                var res = k4a_playback_get_previous_capture((NativeHandles.PlaybackHandle.Azure)playbackHandle, out var h);
                captureHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_capture(k4a_playback_t playback_handle,
            //                                                                        k4a_capture_t* capture_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_previous_capture(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out NativeHandles.CaptureHandle.Azure? captureHandle);

            public override NativeCallResults.StreamResult PlaybackGetNextImuSample(
                NativeHandles.PlaybackHandle playbackHandle,
                out Sensor.ImuSample imuSample)
                => k4a_playback_get_next_imu_sample((NativeHandles.PlaybackHandle.Azure)playbackHandle, out imuSample);

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_imu_sample(k4a_playback_t playback_handle,
            //                                                                       k4a_imu_sample_t* imu_sample);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_next_imu_sample(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out Sensor.ImuSample imuSample);

            public override NativeCallResults.StreamResult PlaybackGetPreviousImuSample(
                NativeHandles.PlaybackHandle playbackHandle,
                out Sensor.ImuSample imuSample)
                => k4a_playback_get_previous_imu_sample((NativeHandles.PlaybackHandle.Azure)playbackHandle, out imuSample);

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_imu_sample(k4a_playback_t playback_handle,
            //                                                                           k4a_imu_sample_t* imu_sample);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_previous_imu_sample(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                out Sensor.ImuSample imuSample);

            public override NativeCallResults.StreamResult PlaybackGetNextDataBlock(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName,
                out NativeHandles.PlaybackDataBlockHandle? dataHandle)
            {
                var res = k4a_playback_get_next_data_block((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName, out var h);
                dataHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_next_data_block(k4a_playback_t playback_handle,
            //                                                                       const char* track_name,
            //                                                                       k4a_playback_data_block_t *data_block_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_next_data_block(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] trackName,
                out NativeHandles.PlaybackDataBlockHandle.Azure? dataHandle);

            public override NativeCallResults.StreamResult PlaybackGetPreviousDataBlock(
                NativeHandles.PlaybackHandle playbackHandle,
                byte[] trackName,
                out NativeHandles.PlaybackDataBlockHandle? dataHandle)
            {
                var res = k4a_playback_get_previous_data_block((NativeHandles.PlaybackHandle.Azure)playbackHandle, trackName, out var h);
                dataHandle = h;
                return res;
            }

            // K4ARECORD_EXPORT k4a_stream_result_t k4a_playback_get_previous_data_block(k4a_playback_t playback_handle,
            //                                                                           const char* track_name,
            //                                                                           k4a_playback_data_block_t *data_block_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.StreamResult k4a_playback_get_previous_data_block(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                [In] byte[] trackName,
                out NativeHandles.PlaybackDataBlockHandle.Azure? dataHandle);

            public override Microseconds64 PlaybackDataBlockGetDeviceTimestamp(NativeHandles.PlaybackDataBlockHandle dataBlockHandle)
                => k4a_playback_data_block_get_device_timestamp_usec((NativeHandles.PlaybackDataBlockHandle.Azure)dataBlockHandle);

            // K4ARECORD_EXPORT uint64_t k4a_playback_data_block_get_device_timestamp_usec(k4a_playback_data_block_t data_block_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Microseconds64 k4a_playback_data_block_get_device_timestamp_usec(
                NativeHandles.PlaybackDataBlockHandle.Azure dataBlockHandle);

            public override UIntPtr PlaybackDataBlockGetBufferSize(NativeHandles.PlaybackDataBlockHandle dataBlockHandle)
                => k4a_playback_data_block_get_buffer_size((NativeHandles.PlaybackDataBlockHandle.Azure)dataBlockHandle);

            // K4ARECORD_EXPORT size_t k4a_playback_data_block_get_buffer_size(k4a_playback_data_block_t data_block_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern UIntPtr k4a_playback_data_block_get_buffer_size(NativeHandles.PlaybackDataBlockHandle.Azure dataBlockHandle);

            public override IntPtr PlaybackDataBlockGetBuffer(NativeHandles.PlaybackDataBlockHandle dataBlockHandle)
                => k4a_playback_data_block_get_buffer((NativeHandles.PlaybackDataBlockHandle.Azure)dataBlockHandle);

            // K4ARECORD_EXPORT uint8_t *k4a_playback_data_block_get_buffer(k4a_playback_data_block_t data_block_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr k4a_playback_data_block_get_buffer(NativeHandles.PlaybackDataBlockHandle.Azure dataBlockHandle);

            public override NativeCallResults.Result PlaybackSeekTimestamp(
                NativeHandles.PlaybackHandle playbackHandle,
                Microseconds64 offset,
                PlaybackSeekOrigin origin)
                => k4a_playback_seek_timestamp((NativeHandles.PlaybackHandle.Azure)playbackHandle, offset, origin);

            // K4ARECORD_EXPORT k4a_result_t k4a_playback_seek_timestamp(k4a_playback_t playback_handle,
            //                                                           int64_t offset_usec,
            //                                                           k4a_playback_seek_origin_t origin);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_playback_seek_timestamp(
                NativeHandles.PlaybackHandle.Azure playbackHandle,
                Microseconds64 offset,
                PlaybackSeekOrigin origin);

            public override Microseconds64 PlaybackGetRecordingLength(NativeHandles.PlaybackHandle playbackHandle)
                => k4a_playback_get_recording_length_usec((NativeHandles.PlaybackHandle.Azure)playbackHandle);

            // K4ARECORD_EXPORT uint64_t k4a_playback_get_recording_length_usec(k4a_playback_t playback_handle);
            [DllImport(Sdk.Azure.RECORD_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Microseconds64 k4a_playback_get_recording_length_usec(NativeHandles.PlaybackHandle.Azure playbackHandle);
        }
    }
}
