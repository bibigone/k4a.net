using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    partial class NativeApi
    {
        public class Azure : NativeApi
        {
            public static readonly Azure Instance = new();

            private Azure() { }

            public override bool IsOrbbec => false;

            public override uint DeviceGetInstalledCount()
                => k4a_device_get_installed_count();

            // K4A_EXPORT uint32_t k4a_device_get_installed_count(void);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern uint k4a_device_get_installed_count();

            public override NativeCallResults.Result DeviceOpen(uint index, out NativeHandles.DeviceHandle? deviceHandle)
            {
                var res = k4a_device_open(index, out var handle);
                deviceHandle = handle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_device_open(uint32_t index, k4a_device_t *device_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_open(uint index, out NativeHandles.DeviceHandle.Azure? deviceHandle);

            public override NativeCallResults.WaitResult DeviceGetCapture(
                NativeHandles.DeviceHandle deviceHandle,
                out NativeHandles.CaptureHandle? captureHandle,
                Timeout timeout)
            {
                var res = k4a_device_get_capture((NativeHandles.DeviceHandle.Azure)deviceHandle, out var azureCaptureHandle, timeout);
                captureHandle = azureCaptureHandle;
                return res;
            }

            // K4A_EXPORT k4a_wait_result_t k4a_device_get_capture(k4a_device_t device_handle,
            //                                                     k4a_capture_t *capture_handle,
            //                                                     int32_t timeout_in_ms);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.WaitResult k4a_device_get_capture(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                out NativeHandles.CaptureHandle.Azure? captureHandle,
                Timeout timeout);

            public override NativeCallResults.WaitResult DeviceGetImuSample(
                NativeHandles.DeviceHandle deviceHandle,
                out ImuSample imuSample,
                Timeout timeout)
                => k4a_device_get_imu_sample((NativeHandles.DeviceHandle.Azure)deviceHandle, out imuSample, timeout);

            // K4A_EXPORT k4a_wait_result_t k4a_device_get_imu_sample(k4a_device_t device_handle,
            //                                                        k4a_imu_sample_t *imu_sample,
            //                                                        int32_t timeout_in_ms);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.WaitResult k4a_device_get_imu_sample(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                out ImuSample imuSample,
                Timeout timeout);

            public override NativeCallResults.Result CaptureCreate(out NativeHandles.CaptureHandle? captureHandle)
            {
                var res = k4a_capture_create(out var handle);
                captureHandle = handle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_capture_create(k4a_capture_t *capture_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_capture_create(out NativeHandles.CaptureHandle.Azure? captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetColorImage(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_color_image((NativeHandles.CaptureHandle.Azure)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_color_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Azure? k4a_capture_get_color_image(NativeHandles.CaptureHandle.Azure captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetDepthImage(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_depth_image((NativeHandles.CaptureHandle.Azure)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_depth_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Azure? k4a_capture_get_depth_image(NativeHandles.CaptureHandle.Azure captureHandle);

            public override NativeHandles.ImageHandle? CaptureGetIRImage(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_ir_image((NativeHandles.CaptureHandle.Azure)captureHandle);

            // K4A_EXPORT k4a_image_t k4a_capture_get_ir_image(k4a_capture_t capture_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Azure? k4a_capture_get_ir_image(NativeHandles.CaptureHandle.Azure captureHandle);

            public override void CaptureSetColorImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_color_image((NativeHandles.CaptureHandle.Azure)captureHandle, (NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT void k4a_capture_set_color_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_color_image(NativeHandles.CaptureHandle.Azure captureHandle, NativeHandles.ImageHandle.Azure imageHandle);

            public override void CaptureSetDepthImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_depth_image((NativeHandles.CaptureHandle.Azure)captureHandle, (NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT void k4a_capture_set_depth_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_depth_image(NativeHandles.CaptureHandle.Azure captureHandle, NativeHandles.ImageHandle.Azure imageHandle);

            public override void CaptureSetIRImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle)
                => k4a_capture_set_ir_image((NativeHandles.CaptureHandle.Azure)captureHandle, (NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT void k4a_capture_set_ir_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_ir_image(NativeHandles.CaptureHandle.Azure captureHandle, NativeHandles.ImageHandle.Azure imageHandle);

            /// <summary>Set the temperature associated with the capture.</summary>
            /// <param name="captureHandle">Capture handle to set the temperature on.</param>
            /// <param name="temperatureC">Temperature in Celsius to store.</param>
            public void CaptureSetTemperatureC(NativeHandles.CaptureHandle captureHandle, float temperatureC)
                => k4a_capture_set_temperature_c((NativeHandles.CaptureHandle.Azure)captureHandle, temperatureC);

            // K4A_EXPORT void k4a_capture_set_temperature_c(k4a_capture_t capture_handle, float temperature_c);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_capture_set_temperature_c(NativeHandles.CaptureHandle.Azure captureHandle, float temperatureC);

            /// <summary>Get the temperature associated with the capture.</summary>
            /// <param name="captureHandle">Capture handle to retrieve the temperature from.</param>
            /// <returns>
            /// This function returns the temperature of the device at the time of the capture in Celsius.
            /// If the temperature is unavailable, the function will return <see cref="float.NaN"/>.
            /// </returns>
            public float CaptureGetTemperatureC(NativeHandles.CaptureHandle captureHandle)
                => k4a_capture_get_temperature_c((NativeHandles.CaptureHandle.Azure)captureHandle);

            // K4A_EXPORT float k4a_capture_get_temperature_c(k4a_capture_t capture_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_get_temperature_c", CallingConvention = CallingConvention.Cdecl)]
            private static extern float k4a_capture_get_temperature_c(NativeHandles.CaptureHandle.Azure captureHandle);

            public override NativeCallResults.Result SetAllocator(
                MemoryAllocateCallback? allocate,
                MemoryDestroyCallback? free)
                => k4a_set_allocator(allocate, free);

            // K4A_EXPORT k4a_result_t k4a_set_allocator(k4a_memory_allocate_cb_t allocate, k4a_memory_destroy_cb_t free);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_set_allocator(
                MemoryAllocateCallback? allocate,
                MemoryDestroyCallback? free);

            public override NativeCallResults.Result ImageCreate(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                out NativeHandles.ImageHandle? imageHandle)
            {
                var res = k4a_image_create(format, widthPixels, heightPixels, strideBytes, out var azureHandle);
                imageHandle = azureHandle;
                return res;
            }

            // K4A_EXPORT k4a_result_t k4a_image_create(k4a_image_format_t format,
            //                                          int width_pixels,
            //                                          int height_pixels,
            //                                          int stride_bytes,
            //                                          k4a_image_t *image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_image_create(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                out NativeHandles.ImageHandle.Azure? imageHandle);

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
                    buffer, bufferSize, bufferReleaseCallback, bufferReleaseCallbackContext, out var azureHandle);
                imageHandle = azureHandle;
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_image_create_from_buffer(
                ImageFormat format,
                int widthPixels,
                int heightPixels,
                int strideBytes,
                IntPtr buffer,
                UIntPtr bufferSize,
                MemoryDestroyCallback? bufferReleaseCallback,
                IntPtr bufferReleaseCallbackContext,
                out NativeHandles.ImageHandle.Azure? imageHandle);

            public override IntPtr ImageGetBuffer(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_buffer((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint8_t *k4a_image_get_buffer(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr k4a_image_get_buffer(NativeHandles.ImageHandle.Azure imageHandle);

            public override UIntPtr ImageGetSize(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_size((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT size_t k4a_image_get_size(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern UIntPtr k4a_image_get_size(NativeHandles.ImageHandle.Azure imageHandle);

            public override ImageFormat ImageGetFormat(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_format((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT k4a_image_format_t k4a_image_get_format(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern ImageFormat k4a_image_get_format(NativeHandles.ImageHandle.Azure imageHandle);

            public override int ImageGetWidthPixels(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_width_pixels((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT int k4a_image_get_width_pixels(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_width_pixels(NativeHandles.ImageHandle.Azure imageHandle);

            public override int ImageGetHeightPixels(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_height_pixels((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT int k4a_image_get_height_pixels(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_height_pixels(NativeHandles.ImageHandle.Azure imageHandle);

            public override int ImageGetStrideBytes(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_stride_bytes((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT int k4a_image_get_stride_bytes(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern int k4a_image_get_stride_bytes(NativeHandles.ImageHandle.Azure imageHandle);

            public override Microseconds64 ImageGetDeviceTimestamp(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_device_timestamp_usec((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint64_t k4a_image_get_device_timestamp_usec(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Microseconds64 k4a_image_get_device_timestamp_usec(NativeHandles.ImageHandle.Azure imageHandle);

            public override Nanoseconds64 ImageGetSystemTimestamp(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_system_timestamp_nsec((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint64_t k4a_image_get_system_timestamp_nsec(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Nanoseconds64 k4a_image_get_system_timestamp_nsec(NativeHandles.ImageHandle.Azure imageHandle);

            public override void ImageSetDeviceTimestamp(NativeHandles.ImageHandle imageHandle, Microseconds64 timestamp)
                => k4a_image_set_device_timestamp_usec((NativeHandles.ImageHandle.Azure)imageHandle, timestamp);

            // K4A_EXPORT void k4a_image_set_device_timestamp_usec(k4a_image_t image_handle, uint64_t timestamp_usec);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_device_timestamp_usec(NativeHandles.ImageHandle.Azure imageHandle, Microseconds64 timestamp);

            public override void ImageSetSystemTimestamp(NativeHandles.ImageHandle imageHandle, Nanoseconds64 timestamp)
                => k4a_image_set_system_timestamp_nsec((NativeHandles.ImageHandle.Azure)imageHandle, timestamp);

            // K4A_EXPORT void k4a_image_set_system_timestamp_nsec(k4a_image_t image_handle, uint64_t timestamp_nsec);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_system_timestamp_nsec(NativeHandles.ImageHandle.Azure imageHandle, Nanoseconds64 timestamp);

            /// <summary>Get the image exposure in microseconds.</summary>
            /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
            /// <returns>
            /// If the <paramref name="imageHandle"/> is invalid or if no exposure was set for the image,
            /// this function will return <see cref="Microseconds64.Zero"/>. Otherwise,
            /// it will return the image exposure time in microseconds.
            /// </returns>
            /// <remarks>Returns an exposure time in microseconds. This is only supported on color image formats.</remarks>
            public Microseconds64 ImageGetExposure(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_exposure_usec((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint64_t k4a_image_get_exposure_usec(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern Microseconds64 k4a_image_get_exposure_usec(NativeHandles.ImageHandle.Azure imageHandle);

            /// <summary>Get the image white balance.</summary>
            /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
            /// <returns>
            /// Returns the image white balance in Kelvin. If <paramref name="imageHandle"/> is invalid, or the white balance was not set or
            /// not applicable to the image, the function will return <c>0</c>.
            /// </returns>
            /// <remarks>Returns the image's white balance. This function is only valid for color captures, and not for depth or IR captures.</remarks>
            public uint ImageGetWhiteBalance(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_white_balance((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint32_t k4a_image_get_white_balance(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern uint k4a_image_get_white_balance(NativeHandles.ImageHandle.Azure imageHandle);

            /// <summary>Get the image ISO speed.</summary>
            /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
            /// <returns>
            /// Returns the ISO speed of the image. <c>0</c> indicates the ISO speed was not available or an error occurred.
            /// </returns>
            /// <remarks>This function is only valid for color captures, and not for depth or IR captures.</remarks>
            public uint ImageGetIsoSpeed(NativeHandles.ImageHandle imageHandle)
                => k4a_image_get_iso_speed((NativeHandles.ImageHandle.Azure)imageHandle);

            // K4A_EXPORT uint32_t k4a_image_get_iso_speed(k4a_image_t image_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern uint k4a_image_get_iso_speed(NativeHandles.ImageHandle.Azure imageHandle);

            /// <summary>Set the exposure time, in microseconds, of the image.</summary>
            /// <param name="imageHandle">Handle of the image to set the exposure time on.</param>
            /// <param name="exposure">Exposure time of the image in microseconds.</param>
            /// <remarks>
            /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
            /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/> to construct an image.
            /// </remarks>
            public void ImageSetExposure(NativeHandles.ImageHandle imageHandle, Microseconds64 exposure)
                => k4a_image_set_exposure_usec((NativeHandles.ImageHandle.Azure)imageHandle, exposure);

            // K4A_EXPORT void k4a_image_set_exposure_usec(k4a_image_t image_handle, uint64_t exposure_usec);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_exposure_usec(NativeHandles.ImageHandle.Azure imageHandle, Microseconds64 exposure);

            /// <summary>Set the white balance of the image.</summary>
            /// <param name="imageHandle">Handle of the image to set the white balance on.</param>
            /// <param name="whiteBalance">White balance of the image in degrees Kelvin.</param>
            /// <remarks>
            /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
            /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/> to construct an image.
            /// </remarks>
            public void ImageSetWhiteBalance(NativeHandles.ImageHandle imageHandle, uint whiteBalance)
                => k4a_image_set_white_balance((NativeHandles.ImageHandle.Azure)imageHandle, whiteBalance);

            // K4A_EXPORT void k4a_image_set_white_balance(k4a_image_t image_handle, uint32_t white_balance);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_white_balance(NativeHandles.ImageHandle.Azure imageHandle, uint whiteBalance);

            /// <summary>Set the ISO speed of the image.</summary>
            /// <param name="imageHandle">Handle of the image to set the ISO speed on.</param>
            /// <param name="isoSpeed">ISO speed of the image.</param>
            /// <remarks>
            /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
            /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/> to construct an image.
            /// </remarks>
            public void ImageSetIsoSpeed(NativeHandles.ImageHandle imageHandle, uint isoSpeed)
                => k4a_image_set_iso_speed((NativeHandles.ImageHandle.Azure)imageHandle, isoSpeed);

            // K4A_EXPORT void k4a_image_set_iso_speed(k4a_image_t image_handle, uint32_t iso_speed);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_image_set_iso_speed(NativeHandles.ImageHandle.Azure imageHandle, uint isoSpeed);

            public override NativeCallResults.Result DeviceStartCameras(NativeHandles.DeviceHandle deviceHandle, in DeviceConfiguration config)
                => k4a_device_start_cameras((NativeHandles.DeviceHandle.Azure)deviceHandle, in config);

            // K4A_EXPORT k4a_result_t k4a_device_start_cameras(k4a_device_t device_handle, const k4a_device_configuration_t *config);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_start_cameras(NativeHandles.DeviceHandle.Azure deviceHandle, in DeviceConfiguration config);

            public override void DeviceStopCameras(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_stop_cameras((NativeHandles.DeviceHandle.Azure)deviceHandle);

            // K4A_EXPORT void k4a_device_stop_cameras(k4a_device_t device_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_device_stop_cameras(NativeHandles.DeviceHandle.Azure deviceHandle);

            public override NativeCallResults.Result DeviceStartImu(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_start_imu((NativeHandles.DeviceHandle.Azure)deviceHandle);

            // K4A_EXPORT k4a_result_t k4a_device_start_imu(k4a_device_t device_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_start_imu(NativeHandles.DeviceHandle.Azure deviceHandle);

            public override void DeviceStopImu(NativeHandles.DeviceHandle deviceHandle)
                => k4a_device_stop_imu((NativeHandles.DeviceHandle.Azure)deviceHandle);

            // K4A_EXPORT void k4a_device_stop_imu(k4a_device_t device_handle);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void k4a_device_stop_imu(NativeHandles.DeviceHandle.Azure deviceHandle);

            public override NativeCallResults.BufferResult DeviceGetSerialnum(
                NativeHandles.DeviceHandle deviceHandle,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_device_get_serialnum((NativeHandles.DeviceHandle.Azure)deviceHandle, buffer, ref size);

            // K4A_EXPORT k4a_buffer_result_t k4a_device_get_serialnum(k4a_device_t device_handle,
            //                                                         char *serial_number,
            //                                                         size_t *serial_number_size);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_device_get_serialnum(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result DeviceGetVersion(
                NativeHandles.DeviceHandle deviceHandle,
                out HardwareVersion version)
                => k4a_device_get_version((NativeHandles.DeviceHandle.Azure)deviceHandle, out version);

            // K4A_EXPORT k4a_result_t k4a_device_get_version(k4a_device_t device_handle, k4a_hardware_version_t *version);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_version(
                NativeHandles.DeviceHandle.Azure deviceHandle,
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
            {
                CheckColorControlCommand(command);
                return k4a_device_get_color_control_capabilities((NativeHandles.DeviceHandle.Azure)deviceHandle, command,
                    out supportsAuto, out minValue, out maxValue, out stepValue, out defaultValue, out defaultMode);
            }

            private static void CheckColorControlCommand(ColorControlCommand command)
            {
                if (command == ColorControlCommand.Hdr)
                    throw new NotSupportedException($"{nameof(ColorControlCommand.Hdr)} is not supported by Azure Kinect devices.");
            }

            // K4A_EXPORT k4a_result_t k4a_device_get_color_control_capabilities(k4a_device_t device_handle,
            //                                                                   k4a_color_control_command_t command,
            //                                                                   bool *supports_auto,
            //                                                                   int32_t *min_value,
            //                                                                   int32_t *max_value,
            //                                                                   int32_t *step_value,
            //                                                                   int32_t *default_value,
            //                                                                   k4a_color_control_mode_t *default_mode);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_color_control_capabilities(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                ColorControlCommand command,
                out byte supportsAuto,
                out int minValue,
                out int maxValue,
                out int stepValue,
                out int defaultValue,
                out ColorControlMode defaultMode);

            public override NativeCallResults.Result DeviceGetColorControl(
                NativeHandles.DeviceHandle deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value)
            {
                CheckColorControlCommand(command);
                return k4a_device_get_color_control((NativeHandles.DeviceHandle.Azure)deviceHandle, command, out mode, out value);
            }

            // K4A_EXPORT k4a_result_t k4a_device_get_color_control(k4a_device_t device_handle,
            //                                                      k4a_color_control_command_t command,
            //                                                      k4a_color_control_mode_t *mode,
            //                                                      int32_t *value);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_color_control(
                NativeHandles.DeviceHandle.Azure deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value);

            public override NativeCallResults.Result DeviceSetColorControl(
                NativeHandles.DeviceHandle deviceHandle,
                ColorControlCommand command,
                ColorControlMode mode,
                int value)
            {
                CheckColorControlCommand(command);
                return k4a_device_set_color_control((NativeHandles.DeviceHandle.Azure)deviceHandle, command, mode, value);
            }

            // K4A_EXPORT k4a_result_t k4a_device_set_color_control(k4a_device_t device_handle,
            //                                                      k4a_color_control_command_t command,
            //                                                      k4a_color_control_mode_t mode,
            //                                                      int32_t value);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_set_color_control(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                ColorControlCommand command,
                ColorControlMode mode,
                int value);

            public override NativeCallResults.BufferResult DeviceGetRawCalibration(
                NativeHandles.DeviceHandle deviceHandle,
                IntPtr buffer,
                ref UIntPtr size)
                => k4a_device_get_raw_calibration((NativeHandles.DeviceHandle.Azure)deviceHandle, buffer, ref size);

            // K4A_EXPORT k4a_buffer_result_t k4a_device_get_raw_calibration(k4a_device_t device_handle,
            //                                                               uint8_t *data,
            //                                                               size_t *data_size);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.BufferResult k4a_device_get_raw_calibration(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                IntPtr buffer,
                ref UIntPtr size);

            public override NativeCallResults.Result DeviceGetCalibration(
                NativeHandles.DeviceHandle deviceHandle,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration)
                => k4a_device_get_calibration((NativeHandles.DeviceHandle.Azure)deviceHandle, depthMode, colorResolution, out calibration);

            // K4A_EXPORT k4a_result_t k4a_device_get_calibration(k4a_device_t device_handle,
            //                                                    const k4a_depth_mode_t depth_mode,
            //                                                    const k4a_color_resolution_t color_resolution,
            //                                                    k4a_calibration_t *calibration);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_calibration(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                DepthMode depthMode,
                ColorResolution colorResolution,
                out CalibrationData calibration);

            /// <summary>Get the device jack status for the synchronization in and synchronization out connectors.</summary>
            /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
            /// <param name="syncInJackConnected">Upon successful return this value will be set to true if a cable is connected to this sync in jack.</param>
            /// <param name="syncOutJackConnected">Upon successful return this value will be set to true if a cable is connected to this sync out jack.</param>
            /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the connector status was successfully read.</returns>
            /// <remarks>
            /// If <paramref name="syncOutJackConnected"/> is <see langword="true"/> then <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to
            /// <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Master"/>. If <paramref name="syncInJackConnected"/> is <see langword="true"/> then
            /// <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Subordinate"/>.
            /// </remarks>
            public NativeCallResults.Result DeviceGetSyncJack(
                NativeHandles.DeviceHandle deviceHandle,
                out byte syncInJackConnected,
                out byte syncOutJackConnected)
                => k4a_device_get_sync_jack((NativeHandles.DeviceHandle.Azure)deviceHandle, out syncInJackConnected, out syncOutJackConnected);

            // K4A_EXPORT k4a_result_t k4a_device_get_sync_jack(k4a_device_t device_handle,
            //                                                  bool *sync_in_jack_connected,
            //                                                  bool *sync_out_jack_connected);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_device_get_sync_jack(
                NativeHandles.DeviceHandle.Azure deviceHandle,
                out byte syncInJackConnected,
                out byte syncOutJackConnected);

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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
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
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_calibration_color_2d_to_depth_2d(
                in CalibrationData calibration,
                in Float2 sourcePoint2D,
                NativeHandles.ImageHandle depthImage,
                out Float2 targetPoint2D,
                out int valid);

            public override NativeHandles.TransformationHandle? TransformationCreate(in CalibrationData calibration)
                => k4a_transformation_create(in calibration);

            // K4A_EXPORT k4a_transformation_t k4a_transformation_create(const k4a_calibration_t *calibration);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.TransformationHandle.Azure? k4a_transformation_create(in CalibrationData calibration);

            public override NativeCallResults.Result TransformationDepthImageToColorCamera(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle transformedDepthImage)
                => k4a_transformation_depth_image_to_color_camera(
                    (NativeHandles.TransformationHandle.Azure)transformationHandle,
                    (NativeHandles.ImageHandle.Azure)depthImage,
                    (NativeHandles.ImageHandle.Azure)transformedDepthImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_color_camera(k4a_transformation_t transformation_handle,
            //                                                                        const k4a_image_t depth_image,
            //                                                                        k4a_image_t transformed_depth_image);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_color_camera(
                NativeHandles.TransformationHandle.Azure transformationHandle,
                NativeHandles.ImageHandle.Azure depthImage,
                NativeHandles.ImageHandle.Azure transformedDepthImage);

            public override NativeCallResults.Result TransformationDepthImageToColorCameraCustom(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle customImage,
                NativeHandles.ImageHandle transformedDepthImage,
                NativeHandles.ImageHandle transformedCustomImage,
                TransformationInterpolation interpolation,
                int invalidCustomValue)
                => k4a_transformation_depth_image_to_color_camera_custom(
                    (NativeHandles.TransformationHandle.Azure)transformationHandle,
                    (NativeHandles.ImageHandle.Azure)depthImage,
                    (NativeHandles.ImageHandle.Azure)customImage,
                    (NativeHandles.ImageHandle.Azure)transformedDepthImage,
                    (NativeHandles.ImageHandle.Azure)transformedCustomImage,
                    interpolation,
                    invalidCustomValue);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_color_camera_custom(k4a_transformation_t transformation_handle,
            //                                                                               const k4a_image_t depth_image,
            //                                                                               const k4a_image_t custom_image,
            //                                                                               k4a_image_t transformed_depth_image,
            //                                                                               k4a_image_t transformed_custom_image,
            //                                                                               k4a_transformation_interpolation_type_t interpolation_type,
            //                                                                               uint32_t invalid_custom_value);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_color_camera_custom(
                NativeHandles.TransformationHandle.Azure transformationHandle,
                NativeHandles.ImageHandle.Azure depthImage,
                NativeHandles.ImageHandle.Azure customImage,
                NativeHandles.ImageHandle.Azure transformedDepthImage,
                NativeHandles.ImageHandle.Azure transformedCustomImage,
                TransformationInterpolation interpolation,
                int invalidCustomValue);

            public override NativeCallResults.Result TransformationColorImageToDepthCamera(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                NativeHandles.ImageHandle colorImage,
                NativeHandles.ImageHandle transformedColorImage)
                => k4a_transformation_color_image_to_depth_camera(
                    (NativeHandles.TransformationHandle.Azure)transformationHandle,
                    (NativeHandles.ImageHandle.Azure)depthImage,
                    (NativeHandles.ImageHandle.Azure)colorImage,
                    (NativeHandles.ImageHandle.Azure)transformedColorImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_color_image_to_depth_camera(k4a_transformation_t transformation_handle,
            //                                                                        const k4a_image_t depth_image,
            //                                                                        const k4a_image_t color_image,
            //                                                                        k4a_image_t transformed_color_image);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_color_image_to_depth_camera(
                NativeHandles.TransformationHandle.Azure transformationHandle,
                NativeHandles.ImageHandle.Azure depthImage,
                NativeHandles.ImageHandle.Azure colorImage,
                NativeHandles.ImageHandle.Azure transformedColorImage);

            public override NativeCallResults.Result TransformationDepthImageToPointCloud(
                    NativeHandles.TransformationHandle transformationHandle,
                    NativeHandles.ImageHandle depthImage,
                    CalibrationGeometry camera,
                    NativeHandles.ImageHandle xyzImage)
                => k4a_transformation_depth_image_to_point_cloud(
                    (NativeHandles.TransformationHandle.Azure)transformationHandle,
                    (NativeHandles.ImageHandle.Azure)depthImage,
                    camera,
                    (NativeHandles.ImageHandle.Azure)xyzImage);

            // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_point_cloud(k4a_transformation_t transformation_handle,
            //                                                                       const k4a_image_t depth_image,
            //                                                                       const k4a_calibration_type_t camera,
            //                                                                       k4a_image_t xyz_image);
            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_transformation_depth_image_to_point_cloud(
                    NativeHandles.TransformationHandle.Azure transformationHandle,
                    NativeHandles.ImageHandle.Azure depthImage,
                    CalibrationGeometry camera,
                    NativeHandles.ImageHandle.Azure xyzImage);
        }
    }
}
