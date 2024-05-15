using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    partial class NativeApi
    {
        public class Orbbec : NativeApi
        {
            public static readonly Orbbec Instance = new();

            private Orbbec() { }

            public override bool IsOrbbec => true;

            public override uint DeviceGetInstalledCount()
                => k4a_device_get_installed_count();

            // K4A_EXPORT uint32_t k4a_device_get_installed_count(void);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint k4a_device_get_installed_count();

            public override NativeCallResults.Result DeviceOpen(uint index, out NativeHandles.DeviceHandle? deviceHandle)
            {
                var res = k4a_device_open(index, out var handle);
                deviceHandle = handle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_device_open(uint32_t index, k4a_device_t *device_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_open(uint index, out NativeHandles.DeviceHandle.Orbbec? deviceHandle);

            public override NativeCallResults.WaitResult DeviceGetCapture(
                NativeHandles.DeviceHandle deviceHandle,
                out NativeHandles.CaptureHandle? captureHandle,
                Timeout timeout)
            {
                var res = k4a_device_get_capture((NativeHandles.DeviceHandle.Orbbec)deviceHandle, out var orbbecCaptureHandle, timeout);
                captureHandle = orbbecCaptureHandle;
                return res;
            }

            // K4A_EXPORT k4a_wait_result_t k4a_device_get_capture(k4a_device_t device_handle,
            //                                                     k4a_capture_t *capture_handle,
            //                                                     int32_t timeout_in_ms);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.WaitResult k4a_device_get_capture(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                out NativeHandles.CaptureHandle.Orbbec? captureHandle,
                Timeout timeout);

            public override NativeCallResults.WaitResult DeviceGetImuSample(
                NativeHandles.DeviceHandle deviceHandle,
                out ImuSample imuSample,
                Timeout timeout)
                => k4a_device_get_imu_sample((NativeHandles.DeviceHandle.Orbbec)deviceHandle, out imuSample, timeout);

            // K4A_EXPORT k4a_wait_result_t k4a_device_get_imu_sample(k4a_device_t device_handle,
            //                                                        k4a_imu_sample_t *imu_sample,
            //                                                        int32_t timeout_in_ms);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.WaitResult k4a_device_get_imu_sample(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                out ImuSample imuSample,
                Timeout timeout);

            public override NativeCallResults.Result CaptureCreate(out NativeHandles.CaptureHandle? captureHandle)
            {
                var res = k4a_capture_create(out var handle);
                captureHandle = handle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_capture_create(k4a_capture_t *capture_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_capture_create(out NativeHandles.CaptureHandle.Orbbec? captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetColorImage(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_color_image((NativeHandles.CaptureHandle.Orbbec)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_color_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Orbbec? k4a_capture_get_color_image(NativeHandles.CaptureHandle.Orbbec captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetDepthImage(NativeHandles.CaptureHandle captureHandle)
                =>  k4a_capture_get_depth_image((NativeHandles.CaptureHandle.Orbbec)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_depth_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Orbbec? k4a_capture_get_depth_image(NativeHandles.CaptureHandle.Orbbec captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetIRImage(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_ir_image((NativeHandles.CaptureHandle.Orbbec)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_ir_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Orbbec? k4a_capture_get_ir_image(NativeHandles.CaptureHandle.Orbbec captureHandle);

            public override void CaptureSetColorImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_color_image((NativeHandles.CaptureHandle.Orbbec)captureHandle, (NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT void k4a_capture_set_color_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_color_image(NativeHandles.CaptureHandle.Orbbec captureHandle, NativeHandles.ImageHandle.Orbbec imageHandle);

            public override void CaptureSetDepthImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_depth_image((NativeHandles.CaptureHandle.Orbbec)captureHandle, (NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT void k4a_capture_set_depth_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_depth_image(NativeHandles.CaptureHandle.Orbbec captureHandle, NativeHandles.ImageHandle.Orbbec imageHandle);

            public override void CaptureSetIRImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_ir_image((NativeHandles.CaptureHandle.Orbbec)captureHandle, (NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT void k4a_capture_set_ir_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_ir_image(NativeHandles.CaptureHandle.Orbbec captureHandle, NativeHandles.ImageHandle.Orbbec imageHandle);

            public override NativeCallResults.Result SetAllocator(
                MemoryAllocateCallback? allocate,
                MemoryDestroyCallback? free)
                => throw new NotSupportedException("OrbbecSDK K4A Wrapper does not support custom memory allocators");

            public override NativeCallResults.Result ImageCreate(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                out NativeHandles.ImageHandle? imageHandle)
            {
                var res = k4a_image_create(format, widthPixels, heightPixels, strideBytes, out var orbbecHandle);
                imageHandle = orbbecHandle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_image_create(k4a_image_format_t format,
            //                                          int width_pixels,
            //                                          int height_pixels,
            //                                          int stride_bytes,
            //                                          k4a_image_t *image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_image_create(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                out NativeHandles.ImageHandle.Orbbec? imageHandle);

            public override NativeCallResults.Result ImageCreateFromBuffer(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                IntPtr buffer,
                UIntPtr bufferSize,
                MemoryDestroyCallback? bufferReleaseCallback,
                IntPtr bufferReleaseCallbackContext,
                out NativeHandles.ImageHandle? imageHandle)
            {
                var res = k4a_image_create_from_buffer(format, widthPixels, heightPixels, strideBytes,
                    buffer, bufferSize, bufferReleaseCallback, bufferReleaseCallbackContext, out var orbbecHandle);
                imageHandle = orbbecHandle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_image_create_from_buffer(k4a_image_format_t format,
            //                                                      int width_pixels,
            //                                                      int height_pixels,
            //                                                      int stride_bytes,
            //                                                      uint8_t* buffer,
            //                                                      size_t buffer_size,
            //                                                      k4a_memory_destroy_cb_t* buffer_release_cb,
            //                                                      void* buffer_release_cb_context,
            //                                                      k4a_image_t* image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_image_create_from_buffer(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                IntPtr buffer,
                UIntPtr bufferSize,
                MemoryDestroyCallback? bufferReleaseCallback,
                IntPtr bufferReleaseCallbackContext,
                out NativeHandles.ImageHandle.Orbbec? imageHandle);

            public override IntPtr ImageGetBuffer(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_buffer((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT uint8_t *k4a_image_get_buffer(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr k4a_image_get_buffer(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override UIntPtr ImageGetSize(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_size((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT size_t k4a_image_get_size(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern UIntPtr k4a_image_get_size(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override ImageFormat ImageGetFormat(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_format((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT k4a_image_format_t k4a_image_get_format(k4a_image_t image_handle);
            /// <summary>Get the format of the image.</summary>
            /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
            /// <returns>
            /// This function is not expected to fail, all images are created with a known format.
            /// If the <paramref name="imageHandle"/> is invalid, the function will return <see cref="ImageFormat.Custom"/>.
            /// </returns>
            /// <remarks>Use this function to determine the format of the image buffer.</remarks>
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern ImageFormat k4a_image_get_format(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override int ImageGetWidthPixels(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_width_pixels((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT int k4a_image_get_width_pixels(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_width_pixels(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override int ImageGetHeightPixels(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_height_pixels((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT int k4a_image_get_height_pixels(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_height_pixels(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override int ImageGetStrideBytes(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_stride_bytes((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT int k4a_image_get_stride_bytes(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_stride_bytes(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override Microseconds64 ImageGetDeviceTimestamp(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_device_timestamp_usec((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT uint64_t k4a_image_get_device_timestamp_usec(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Microseconds64 k4a_image_get_device_timestamp_usec(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override Nanoseconds64 ImageGetSystemTimestamp(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_system_timestamp_nsec((NativeHandles.ImageHandle.Orbbec)imageHandle);

            // K4A_EXPORT uint64_t k4a_image_get_system_timestamp_nsec(k4a_image_t image_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Nanoseconds64 k4a_image_get_system_timestamp_nsec(NativeHandles.ImageHandle.Orbbec imageHandle);

            public override void ImageSetDeviceTimestamp(NativeHandles.ImageHandle imageHandle, Microseconds64 timestamp)
                => k4a_image_set_device_timestamp_usec((NativeHandles.ImageHandle.Orbbec)imageHandle, timestamp);

            // K4A_EXPORT void k4a_image_set_device_timestamp_usec(k4a_image_t image_handle, uint64_t timestamp_usec);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_device_timestamp_usec(NativeHandles.ImageHandle.Orbbec imageHandle, Microseconds64 timestamp);

            public override void ImageSetSystemTimestamp(NativeHandles.ImageHandle imageHandle, Nanoseconds64 timestamp)
                => k4a_image_set_system_timestamp_nsec((NativeHandles.ImageHandle.Orbbec)imageHandle, timestamp);

            // K4A_EXPORT void k4a_image_set_system_timestamp_nsec(k4a_image_t image_handle, uint64_t timestamp_nsec);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_system_timestamp_nsec(NativeHandles.ImageHandle.Orbbec imageHandle, Nanoseconds64 timestamp);

            public override NativeCallResults.Result DeviceStartCameras(NativeHandles.DeviceHandle deviceHandle, in DeviceConfiguration config)
                => k4a_device_start_cameras((NativeHandles.DeviceHandle.Orbbec)deviceHandle, in config);

            // K4A_EXPORT k4a_result_t k4a_device_start_cameras(k4a_device_t device_handle, const k4a_device_configuration_t *config);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_start_cameras(NativeHandles.DeviceHandle.Orbbec deviceHandle, in DeviceConfiguration config);

            public override void DeviceStopCameras(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_stop_cameras((NativeHandles.DeviceHandle.Orbbec)deviceHandle);

            // K4A_EXPORT void k4a_device_stop_cameras(k4a_device_t device_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_device_stop_cameras(NativeHandles.DeviceHandle.Orbbec deviceHandle);

            public override NativeCallResults.Result DeviceStartImu(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_start_imu((NativeHandles.DeviceHandle.Orbbec)deviceHandle);

            // K4A_EXPORT k4a_result_t k4a_device_start_imu(k4a_device_t device_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_start_imu(NativeHandles.DeviceHandle.Orbbec deviceHandle);

            public override void DeviceStopImu(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_stop_imu((NativeHandles.DeviceHandle.Orbbec)deviceHandle);

            // K4A_EXPORT void k4a_device_stop_imu(k4a_device_t device_handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_device_stop_imu(NativeHandles.DeviceHandle.Orbbec deviceHandle);

            public override NativeCallResults.BufferResult DeviceGetSerialnum(
                NativeHandles.DeviceHandle deviceHandle,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_device_get_serialnum((NativeHandles.DeviceHandle.Orbbec)deviceHandle, buffer, ref size);

            // K4A_EXPORT k4a_buffer_result_t k4a_device_get_serialnum(k4a_device_t device_handle,
            //                                                         char *serial_number,
            //                                                         size_t *serial_number_size);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_device_get_serialnum(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result DeviceGetVersion(
                NativeHandles.DeviceHandle deviceHandle,
                out HardwareVersion version)
                => k4a_device_get_version((NativeHandles.DeviceHandle.Orbbec)deviceHandle, out version);

            // K4A_EXPORT k4a_result_t k4a_device_get_version(k4a_device_t device_handle, k4a_hardware_version_t *version);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_version(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                out HardwareVersion version);

            public override NativeCallResults.Result DeviceGetColorControlCapabilities(
                NativeHandles.DeviceHandle deviceHandle,
                ColorControlCommand command,
                out byte supportsAuto,
                out int minValue,
                out int maxValue,
                out int stepValue,
                out int defaultValue,
                out ColorControlMode defaultMode)
                => k4a_device_get_color_control_capabilities((NativeHandles.DeviceHandle.Orbbec)deviceHandle, command,
                    out supportsAuto, out minValue, out maxValue, out stepValue, out defaultValue, out defaultMode);

            // K4A_EXPORT k4a_result_t k4a_device_get_color_control_capabilities(k4a_device_t device_handle,
            //                                                                   k4a_color_control_command_t command,
            //                                                                   bool *supports_auto,
            //                                                                   int32_t *min_value,
            //                                                                   int32_t *max_value,
            //                                                                   int32_t *step_value,
            //                                                                   int32_t *default_value,
            //                                                                   k4a_color_control_mode_t *default_mode);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_color_control_capabilities(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                ColorControlCommand command,
                out byte supportsAuto,
                out int minValue,
                out int maxValue,
                out int stepValue,
                out int defaultValue,
                out ColorControlMode defaultMode);

            public override NativeCallResults.Result DeviceGetColorControl(
                NativeHandles.DeviceHandle deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value)
                => k4a_device_get_color_control((NativeHandles.DeviceHandle.Orbbec)deviceHandle, command, out mode, out value);

            // K4A_EXPORT k4a_result_t k4a_device_get_color_control(k4a_device_t device_handle,
            //                                                      k4a_color_control_command_t command,
            //                                                      k4a_color_control_mode_t *mode,
            //                                                      int32_t *value);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_color_control(
                NativeHandles.DeviceHandle.Orbbec deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value);

            public override NativeCallResults.Result DeviceSetColorControl(
                NativeHandles.DeviceHandle deviceHandle,
                ColorControlCommand command,
                ColorControlMode mode,
                int value)
                => k4a_device_set_color_control((NativeHandles.DeviceHandle.Orbbec)deviceHandle, command, mode, value);

            // K4A_EXPORT k4a_result_t k4a_device_set_color_control(k4a_device_t device_handle,
            //                                                      k4a_color_control_command_t command,
            //                                                      k4a_color_control_mode_t mode,
            //                                                      int32_t value);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_set_color_control(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                ColorControlCommand command,
                ColorControlMode mode,
                int value);

            public override NativeCallResults.BufferResult DeviceGetRawCalibration(
                NativeHandles.DeviceHandle deviceHandle,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_device_get_raw_calibration((NativeHandles.DeviceHandle.Orbbec)deviceHandle, buffer, ref size);

            // K4A_EXPORT k4a_buffer_result_t k4a_device_get_raw_calibration(k4a_device_t device_handle,
            //                                                               uint8_t *data,
            //                                                               size_t *data_size);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_device_get_raw_calibration(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result DeviceGetCalibration(
                NativeHandles.DeviceHandle deviceHandle,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration)
                => k4a_device_get_calibration((NativeHandles.DeviceHandle.Orbbec)deviceHandle, depthMode, colorResolution, out calibration);

            // K4A_EXPORT k4a_result_t k4a_device_get_calibration(k4a_device_t device_handle,
            //                                                    const k4a_depth_mode_t depth_mode,
            //                                                    const k4a_color_resolution_t color_resolution,
            //                                                    k4a_calibration_t *calibration);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_calibration(
                NativeHandles.DeviceHandle.Orbbec deviceHandle,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration);

            public override NativeCallResults.Result CalibrationGetFromRaw(
                byte[] rawCalibration,
                UIntPtr rawCalibrationSize,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration)
                => k4a_calibration_get_from_raw(rawCalibration, rawCalibrationSize, depthMode, colorResolution, out calibration);

            // K4A_EXPORT k4a_result_t k4a_calibration_get_from_raw(char *raw_calibration,
            //                                                      size_t raw_calibration_size,
            //                                                      const k4a_depth_mode_t depth_mode,
            //                                                      const k4a_color_resolution_t color_resolution,
            //                                                      k4a_calibration_t *calibration);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_get_from_raw(
                byte[] rawCalibration,
                UIntPtr rawCalibrationSize,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration);

            public override NativeCallResults.Result Calibration3DTo3D(
                in CalibrationData calibration,
                in Float3 sourcePoint3DMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float3 targetPoint3DMm)
                => k4a_calibration_3d_to_3d(in calibration, in sourcePoint3DMm, sourceCamera, targetCamera, out targetPoint3DMm);

            // K4A_EXPORT k4a_result_t k4a_calibration_3d_to_3d(const k4a_calibration_t *calibration,
            //                                                  const k4a_float3_t *source_point3d_mm,
            //                                                  const k4a_calibration_type_t source_camera,
            //                                                  const k4a_calibration_type_t target_camera,
            //                                                  k4a_float3_t *target_point3d_mm);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_3d_to_3d(
                in CalibrationData calibration,
                in Float3 sourcePoint3DMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float3 targetPoint3DMm);

            public override NativeCallResults.Result Calibration2DTo3D(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                float sourceDepthMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float3 targetPoint3DMm,
                out int valid)
                => k4a_calibration_2d_to_3d(in calibration, in sourcePoint2D, sourceDepthMm, sourceCamera, targetCamera, out targetPoint3DMm, out valid);

            // K4A_EXPORT k4a_result_t k4a_calibration_2d_to_3d(const k4a_calibration_t *calibration,
            //                                                  const k4a_float2_t *source_point2d,
            //                                                  const float source_depth_mm,
            //                                                  const k4a_calibration_type_t source_camera,
            //                                                  const k4a_calibration_type_t target_camera,
            //                                                  k4a_float3_t *target_point3d_mm,
            //                                                  int *valid);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_2d_to_3d(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                float sourceDepthMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float3 targetPoint3DMm,
                out int valid);

            public override NativeCallResults.Result Calibration3DTo2D(
                in CalibrationData calibration,
                in Float3 sourcePoint3DMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float2 targetPoint2D,
                out int valid)
                => k4a_calibration_3d_to_2d(in calibration, in sourcePoint3DMm, sourceCamera, targetCamera, out targetPoint2D, out valid);

            // K4A_EXPORT k4a_result_t k4a_calibration_3d_to_2d(const k4a_calibration_t *calibration,
            //                                                  const k4a_float3_t *source_point3d_mm,
            //                                                  const k4a_calibration_type_t source_camera,
            //                                                  const k4a_calibration_type_t target_camera,
            //                                                  k4a_float2_t *target_point2d,
            //                                                  int *valid);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_3d_to_2d(
                in CalibrationData calibration,
                in Float3 sourcePoint3DMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float2 targetPoint2D,
                out int valid);

            public override NativeCallResults.Result Calibration2DTo2D(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                float sourceDepthMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float2 targetPoint2D,
                out int valid)
                => k4a_calibration_2d_to_2d(in calibration, in sourcePoint2D, sourceDepthMm, sourceCamera, targetCamera, out targetPoint2D, out valid);

            // K4A_EXPORT k4a_result_t k4a_calibration_2d_to_2d(const k4a_calibration_t *calibration,
            //                                                  const k4a_float2_t *source_point2d,
            //                                                  const float source_depth_mm,
            //                                                  const k4a_calibration_type_t source_camera,
            //                                                  const k4a_calibration_type_t target_camera,
            //                                                  k4a_float2_t *target_point2d,
            //                                                  int *valid);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_2d_to_2d(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                float sourceDepthMm,
                CalibrationGeometry sourceCamera,
                CalibrationGeometry targetCamera,
                out Float2 targetPoint2D,
                out int valid);

            public override NativeCallResults.Result CalibrationColor2DToDepth2D(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                NativeHandles.ImageHandle depthImage,
                out Float2 targetPoint2D,
                out int valid)
                => k4a_calibration_color_2d_to_depth_2d(in calibration, in sourcePoint2D, depthImage, out targetPoint2D, out valid);

            // K4A_EXPORT k4a_result_t k4a_calibration_color_2d_to_depth_2d(const k4a_calibration_t* calibration,
            //                                                     const k4a_float2_t* source_point2d,
            //                                                     const k4a_image_t depth_image,
            //                                                     k4a_float2_t *target_point2d,
            //                                                     int* valid);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_color_2d_to_depth_2d(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                NativeHandles.ImageHandle depthImage,
                out Float2 targetPoint2D,
                out int valid);


            public override NativeHandles.TransformationHandle? TransformationCreate(in CalibrationData calibration)
                => k4a_transformation_create(in calibration);

            // K4A_EXPORT k4a_transformation_t k4a_transformation_create(const k4a_calibration_t *calibration);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.TransformationHandle.Orbbec? k4a_transformation_create(in CalibrationData calibration);

            public override NativeCallResults.Result TransformationDepthImageToColorCamera(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle transformedDepthImage)
                => k4a_transformation_depth_image_to_color_camera(
                    (NativeHandles.TransformationHandle.Orbbec)transformationHandle,
                    (NativeHandles.ImageHandle.Orbbec)depthImage,
                    (NativeHandles.ImageHandle.Orbbec)transformedDepthImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_color_camera(k4a_transformation_t transformation_handle,
            //                                                                        const k4a_image_t depth_image,
            //                                                                        k4a_image_t transformed_depth_image);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_color_camera(
                NativeHandles.TransformationHandle.Orbbec transformationHandle,
                NativeHandles.ImageHandle.Orbbec depthImage,
                NativeHandles.ImageHandle.Orbbec transformedDepthImage);

            public override NativeCallResults.Result TransformationDepthImageToColorCameraCustom(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle customImage,
                NativeHandles.ImageHandle transformedDepthImage,
                NativeHandles.ImageHandle transformedCustomImage,
                TransformationInterpolation interpolation,
                int invalidCustomValue)
                => k4a_transformation_depth_image_to_color_camera_custom(
                    (NativeHandles.TransformationHandle.Orbbec)transformationHandle,
                    (NativeHandles.ImageHandle.Orbbec)depthImage,
                    (NativeHandles.ImageHandle.Orbbec)customImage,
                    (NativeHandles.ImageHandle.Orbbec)transformedDepthImage,
                    (NativeHandles.ImageHandle.Orbbec)transformedCustomImage,
                    interpolation,
                    invalidCustomValue);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_color_camera_custom(k4a_transformation_t transformation_handle,
            //                                                                               const k4a_image_t depth_image,
            //                                                                               const k4a_image_t custom_image,
            //                                                                               k4a_image_t transformed_depth_image,
            //                                                                               k4a_image_t transformed_custom_image,
            //                                                                               k4a_transformation_interpolation_type_t interpolation_type,
            //                                                                               uint32_t invalid_custom_value);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_color_camera_custom(
                NativeHandles.TransformationHandle.Orbbec transformationHandle,
                NativeHandles.ImageHandle.Orbbec depthImage,
                NativeHandles.ImageHandle.Orbbec customImage,
                NativeHandles.ImageHandle.Orbbec transformedDepthImage,
                NativeHandles.ImageHandle.Orbbec transformedCustomImage,
                TransformationInterpolation interpolation,
                int invalidCustomValue);

            public override NativeCallResults.Result TransformationColorImageToDepthCamera(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle colorImage,
                NativeHandles.ImageHandle transformedColorImage)
                => k4a_transformation_color_image_to_depth_camera(
                    (NativeHandles.TransformationHandle.Orbbec)transformationHandle,
                    (NativeHandles.ImageHandle.Orbbec)depthImage,
                    (NativeHandles.ImageHandle.Orbbec)colorImage,
                    (NativeHandles.ImageHandle.Orbbec)transformedColorImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_color_image_to_depth_camera(k4a_transformation_t transformation_handle,
            //                                                                        const k4a_image_t depth_image,
            //                                                                        const k4a_image_t color_image,
            //                                                                        k4a_image_t transformed_color_image);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_color_image_to_depth_camera(
                NativeHandles.TransformationHandle.Orbbec transformationHandle,
                NativeHandles.ImageHandle.Orbbec depthImage,
                NativeHandles.ImageHandle.Orbbec colorImage,
                NativeHandles.ImageHandle.Orbbec transformedColorImage);

            public override NativeCallResults.Result TransformationDepthImageToPointCloud(
                    NativeHandles.TransformationHandle transformationHandle,
                    NativeHandles.ImageHandle depthImage,
                    CalibrationGeometry camera,
                    NativeHandles.ImageHandle xyzImage)
                => k4a_transformation_depth_image_to_point_cloud(
                    (NativeHandles.TransformationHandle.Orbbec)transformationHandle,
                    (NativeHandles.ImageHandle.Orbbec)depthImage,
                    camera,
                    (NativeHandles.ImageHandle.Orbbec)xyzImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_point_cloud(k4a_transformation_t transformation_handle,
            //                                                                       const k4a_image_t depth_image,
            //                                                                       const k4a_calibration_type_t camera,
            //                                                                       k4a_image_t xyz_image);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_point_cloud(
                    NativeHandles.TransformationHandle.Orbbec transformationHandle,
                    NativeHandles.ImageHandle.Orbbec depthImage,
                    CalibrationGeometry camera,
                    NativeHandles.ImageHandle.Orbbec xyzImage);

            /// <summary>Create depthengine helper (OrbbecSDK-K4A-Wrapper only).</summary>
            /// <param name="depthEngineHandle"></param>
            /// <returns></returns>
            /// <remarks>This API is currently mainly used to initialize depthengine, This function only needs to be called when on the Linux platform</remarks>
            public NativeCallResults.Result DepthEngineHelperCreate(out NativeHandles.DepthEngineHandle depthEngineHandle)
                => k4a_depth_engine_helper_create(out depthEngineHandle);

            // K4A_EXPORT k4a_result_t k4a_depth_engine_helper_create(k4a_depthengine_t* handle);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_depth_engine_helper_create(out NativeHandles.DepthEngineHandle depthEngineHandle);

            /// <summary>get device sync mode (OrbbecSDK-K4A-Wrapper only)</summary>
            /// <param name="device">Device handle</param>
            /// <returns>Current sync mode</returns>
            /// <remarks>
            /// This API is currently mainly used to get device sync mode.
            /// The device synchronization mode will change according to the mode configured in the start_cameras function.
            /// </remarks>
            public WiredSyncMode DeviceGetWiredSyncMode(NativeHandles.DeviceHandle device)
                => k4a_device_get_wired_sync_mode((NativeHandles.DeviceHandle.Orbbec)device);

            // K4A_EXPORT k4a_wired_sync_mode_t k4a_device_get_wired_sync_mode(k4a_device_t device);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern WiredSyncMode k4a_device_get_wired_sync_mode(NativeHandles.DeviceHandle.Orbbec device);

            /// <summary>enable/disable soft filter for depth camera (OrbbecSDK-K4A-Wrapper only)</summary>
            /// <param name="deviceHandle">Device handle</param>
            /// <param name="enable">Device software filtering switch</param>
            /// <returns></returns>
            /// <remarks>This API is used to set filtering.</remarks>
            public NativeCallResults.Result DeviceEnableSoftFilter(NativeHandles.DeviceHandle deviceHandle, byte enable)
                => k4a_device_enable_soft_filter((NativeHandles.DeviceHandle.Orbbec)deviceHandle, enable);

            // K4A_EXPORT k4a_result_t k4a_device_enable_soft_filter(k4a_device_t device_handle, bool enable);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_enable_soft_filter(NativeHandles.DeviceHandle.Orbbec deviceHandle, byte enable);

            /// <summary>switch device clock sync mode (OrbbecSDK-K4A-Wrapper only)</summary>
            /// <param name="deviceHandle">Device handle</param>
            /// <param name="timestampMode">Device clock synchronization mode</param>
            /// <param name="param">
            /// If <paramref name="timestampMode"/> is <see cref="DeviceClockSyncMode.Reset"/>: The delay time of executing the timestamp reset function after receiving the command or signal in microseconds.
            /// If <paramref name="timestampMode"/> is <see cref="DeviceClockSyncMode.Sync"/>: The interval for auto-repeated synchronization, in microseconds. If the value is 0, synchronization is performed only once.
            /// </param>
            /// <remarks><para>
            /// This API is used for device clock synchronization mode switching.
            /// </para><para>
            /// It is necessary to ensure that the mode switching of all devices is completed before any device start_cameras.
            /// </para><para>
            /// It is necessary to ensure that the master and slave devices are configured in the same mode.
            /// </para></remarks>
            public NativeCallResults.Result DeviceSwitchDeviceClockSyncMode(NativeHandles.DeviceHandle deviceHandle, DeviceClockSyncMode timestampMode, uint param)
                => k4a_device_switch_device_clock_sync_mode((NativeHandles.DeviceHandle.Orbbec)deviceHandle, timestampMode, param);

            // K4A_EXPORT k4a_result_t k4a_device_switch_device_clock_sync_mode(k4a_device_t device_handle, k4a_device_clock_sync_mode_t timestamp_mode, uint32_t param);
            [DllImport(Sdk.Orbbec.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_switch_device_clock_sync_mode(NativeHandles.DeviceHandle.Orbbec deviceHandle, DeviceClockSyncMode timestampMode, uint param);
        }
    }
}
