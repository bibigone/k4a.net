using System;
using System.Runtime.InteropServices;
using System.Text;

namespace K4AdotNet.Sensor
{
    /// <summary>DLL imports for most of native functions from <c>k4a.h</c> header file.</summary>
    internal static class NativeApi
    {
        /// <summary>Default device index.</summary>
        /// <remarks>Passed as an argument to <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/> to open the default sensor.</remarks>
        public const uint DEFAULT_DEVICE_INDEX = 0;

        // K4A_EXPORT uint32_t k4a_device_get_installed_count(void);
        /// <summary>Gets the number of connected devices.</summary>
        /// <returns>Number of sensors connected to the PC.</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_installed_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DeviceGetInstalledCount();

        // K4A_EXPORT k4a_result_t k4a_device_open(uint32_t index, k4a_device_t *device_handle);
        /// <summary>Open an Azure Kinect device.</summary>
        /// <param name="index">The index of the device to open, starting with 0. Use <see cref="DEFAULT_DEVICE_INDEX"/> constant as value for this parameter to open default device.</param>
        /// <param name="deviceHandle">Output parameter which on success will return a handle to the device.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the device was opened successfully.</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceOpen(uint index, out NativeHandles.DeviceHandle deviceHandle);

        // K4A_EXPORT k4a_wait_result_t k4a_device_get_capture(k4a_device_t device_handle,
        //                                                     k4a_capture_t *capture_handle,
        //                                                     int32_t timeout_in_ms);
        /// <summary>Reads a sensor capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="captureHandle">If successful this contains a handle to a capture object.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the capture.
        /// If set to <see cref="Timeout.NoWait"/>, the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.WaitResult.Succeeded"/> if a capture is returned.
        /// If a capture is not available before the timeout elapses, the function will return <see cref="NativeApiCallResults.WaitResult.Timeout"/>.
        /// All other failures will return <see cref="NativeApiCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="DeviceStartCameras"/> is called and before <see cref="DeviceStopCameras"/> is called.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.WaitResult DeviceGetCapture(
            NativeHandles.DeviceHandle deviceHandle,
            out NativeHandles.CaptureHandle captureHandle,
            Timeout timeout);

        // K4A_EXPORT k4a_wait_result_t k4a_device_get_imu_sample(k4a_device_t device_handle,
        //                                                        k4a_imu_sample_t *imu_sample,
        //                                                        int32_t timeout_in_ms);
        /// <summary>Reads an IMU sample.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="imuSample">Information about IMU sample.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the sample.
        /// If set to <see cref="Timeout.NoWait"/>, the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.WaitResult.Succeeded"/> if a sample is returned.
        /// If a sample is not available before the timeout elapses, the function will return <see cref="NativeApiCallResults.WaitResult.Timeout"/>.
        /// All other failures will return <see cref="NativeApiCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="DeviceStartCameras"/> is called and before <see cref="DeviceStopCameras"/> is called.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_imu_sample", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.WaitResult DeviceGetImuSample(
            NativeHandles.DeviceHandle deviceHandle,
            out ImuSample imuSample,
            Timeout timeout);

        // K4A_EXPORT k4a_result_t k4a_capture_create(k4a_capture_t *capture_handle);
        /// <summary>Create an empty capture object.</summary>
        /// <param name="captureHandle">Output parameter which on success will return a handle to the capture.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the device was opened successfully.</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result CaptureCreate(out NativeHandles.CaptureHandle captureHandle);

        // K4A_EXPORT k4a_image_t k4a_capture_get_color_image(k4a_capture_t capture_handle);
        /// <summary>Get the color image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the color image part of this capture.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_get_color_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.ImageHandle CaptureGetColorImage(NativeHandles.CaptureHandle captureHandle);

        // K4A_EXPORT k4a_image_t k4a_capture_get_depth_image(k4a_capture_t capture_handle);
        /// <summary>Get the depth image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the depth image part of this capture.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_get_depth_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.ImageHandle CaptureGetDepthImage(NativeHandles.CaptureHandle captureHandle);

        // K4A_EXPORT k4a_image_t k4a_capture_get_ir_image(k4a_capture_t capture_handle);
        /// <summary>Get the IR image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the IR image part of this capture.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_get_ir_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.ImageHandle CaptureGetIRImage(NativeHandles.CaptureHandle captureHandle);

        // K4A_EXPORT void k4a_capture_set_color_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
        /// <summary>Set or add a color image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see langword="null"/> to remove color image from a given capture if any.</param>
        /// <remarks>If there is already a color image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_set_color_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureSetColorImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT void k4a_capture_set_depth_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
        /// <summary>Set or add a depth image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see langword="null"/> to remove depth image from a given capture if any.</param>
        /// <remarks>If there is already a depth image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_set_depth_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureSetDepthImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT void k4a_capture_set_ir_image(k4a_capture_t capture_handle, k4a_image_t image_handle);
        /// <summary>Set or add a IR image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see langword="null"/> to remove IR image from a given capture if any.</param>
        /// <remarks>If there is already a IR image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_set_ir_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureSetIRImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT void k4a_capture_set_temperature_c(k4a_capture_t capture_handle, float temperature_c);
        /// <summary>Set the temperature associated with the capture.</summary>
        /// <param name="captureHandle">Capture handle to set the temperature on.</param>
        /// <param name="temperatureC">Temperature in Celsius to store.</param>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_set_temperature_c", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureSetTemperatureC(NativeHandles.CaptureHandle captureHandle, float temperatureC);

        // K4A_EXPORT float k4a_capture_get_temperature_c(k4a_capture_t capture_handle);
        /// <summary>Get the temperature associated with the capture.</summary>
        /// <param name="captureHandle">Capture handle to retrieve the temperature from.</param>
        /// <returns>
        /// This function returns the temperature of the device at the time of the capture in Celsius.
        /// If the temperature is unavailable, the function will return <see cref="float.NaN"/>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_capture_get_temperature_c", CallingConvention = CallingConvention.Cdecl)]
        public static extern float CaptureGetTemperatureC(NativeHandles.CaptureHandle captureHandle);

        // K4A_EXPORT k4a_result_t k4a_image_create(k4a_image_format_t format,
        //                                          int width_pixels,
        //                                          int height_pixels,
        //                                          int stride_bytes,
        //                                          k4a_image_t *image_handle);
        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="format">The format of the image that will be stored in this image container.</param>
        /// <param name="widthPixels">Width in pixels.</param>
        /// <param name="heightPixels">Height in pixels.</param>
        /// <param name="strideBytes">The number of bytes per horizontal line of the image.</param>
        /// <param name="imageHandle">Handle of created image in case of success.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> on success.</returns>
        /// <remarks>This function is used to create images of formats that have consistent stride.
        /// The function is not suitable for compressed formats that may not be represented by the same number of bytes per line.
        /// The function will allocate an image buffer of size <paramref name="heightPixels"/> * <paramref name="strideBytes"/> bytes.
        /// To create an image object without the API allocating memory, or to represent an image that has a non-deterministic
        /// stride, use <see cref="ImageCreateFromBuffer"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result ImageCreate(
            ImageFormat format,
            int widthPixels,
            int heightPixels,
            int strideBytes,
            out NativeHandles.ImageHandle imageHandle);

        // typedef void(k4a_memory_destroy_cb_t)(void *buffer, void *context);
        /// <summary>Callback function for a memory object being destroyed.</summary>
        /// <param name="buffer">The buffer pointer that was supplied by the caller.</param>
        /// <param name="context">The context for the memory object that needs to be destroyed that was supplied by the caller.</param>
        public delegate void MemoryDestroyCallback(IntPtr buffer, IntPtr context);

        // K4A_EXPORT k4a_result_t k4a_image_create_from_buffer(k4a_image_format_t format,
        //                                                      int width_pixels,
        //                                                      int height_pixels,
        //                                                      int stride_bytes,
        //                                                      uint8_t* buffer,
        //                                                      size_t buffer_size,
        //                                                      k4a_memory_destroy_cb_t* buffer_release_cb,
        //                                                      void* buffer_release_cb_context,
        //                                                      k4a_image_t* image_handle);
        /// <summary>Create an image from a pre-allocated buffer.</summary>
        /// <param name="format">The format of the image that will be stored in this image container.</param>
        /// <param name="widthPixels">Width in pixels.</param>
        /// <param name="heightPixels">Height in pixels.</param>
        /// <param name="strideBytes">The number of bytes per horizontal line of the image.</param>
        /// <param name="buffer">Pointer to a pre-allocated image buffer.</param>
        /// <param name="bufferSize">Size in bytes of the pre-allocated image buffer.</param>
        /// <param name="bufferReleaseCallback">
        /// Callback to the buffer free function, called when all references to the buffer have been released.
        /// This parameter is optional (can be <see langword="null"/>).</param>
        /// <param name="bufferReleaseCallbackContext">
        /// Context for the buffer free function. This value will be called as 2nd parameter to <paramref name="bufferReleaseCallback"/>
        /// when the callback is invoked.
        /// </param>
        /// <param name="imageHandle">Handle of created image in case of success.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> on success.</returns>
        /// <remarks>
        /// This function creates an <see cref="NativeHandles.ImageHandle"/> from a pre-allocated buffer. When all references to this object reach zero
        /// the provided <paramref name="bufferReleaseCallback"/> callback function is called so that the memory can be released.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_create_from_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result ImageCreateFromBuffer(
            ImageFormat format,
            int widthPixels,
            int heightPixels,
            int strideBytes,
            IntPtr buffer,
            UIntPtr bufferSize,
            MemoryDestroyCallback bufferReleaseCallback,
            IntPtr bufferReleaseCallbackContext,
            out NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT uint8_t *k4a_image_get_buffer(k4a_image_t image_handle);
        /// <summary>Get the image buffer.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// The function will return <see cref="IntPtr.Zero"/> if there is an error, and will normally return a pointer to the image buffer.
        /// </returns>
        /// <remarks>Use this buffer to access the raw image data.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ImageGetBuffer(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT size_t k4a_image_get_size(k4a_image_t image_handle);
        /// <summary>Get the image buffer size.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>The function will return <see cref="UIntPtr.Zero"/> if there is an error, and will normally return the image size.</returns>
        /// <remarks>Use this function to know what the size of the image buffer is returned by <see cref="ImageGetBuffer"/>.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ImageGetSize(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT k4a_image_format_t k4a_image_get_format(k4a_image_t image_handle);
        /// <summary>Get the format of the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known format.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <see cref="ImageFormat.Custom"/>.
        /// </returns>
        /// <remarks>Use this function to determine the format of the image buffer.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_format", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImageFormat ImageGetFormat(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT int k4a_image_get_width_pixels(k4a_image_t image_handle);
        /// <summary>Get the image width in pixels.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known width.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <c>0</c>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_width_pixels", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImageGetWidthPixels(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT int k4a_image_get_height_pixels(k4a_image_t image_handle);
        /// <summary>Get the image height in pixels.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known height.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <c>0</c>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_height_pixels", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImageGetHeightPixels(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT int k4a_image_get_stride_bytes(k4a_image_t image_handle);
        /// <summary>Get the image stride in bytes.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known stride.
        /// If the <paramref name="imageHandle"/> is invalid or the image's format does not have a stride, the function will return <c>0</c>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_stride_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImageGetStrideBytes(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT uint64_t k4a_image_get_timestamp_usec(k4a_image_t image_handle);
        /// <summary>Get the image time stamp.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// If the <paramref name="imageHandle"/> is invalid or if no time stamp was set for the image,
        /// this function will return <see cref="TimeStamp.Zero"/>.
        /// It is also possible for <see cref="TimeStamp.Zero"/> to be a valid time stamp originating from the beginning
        /// of a recording or the start of streaming.
        /// </returns>
        /// <remarks>
        /// Returns the time stamp of the image. Time stamps are recorded by the device and represent the mid-point of exposure.
        /// They may be used for relative comparison, but their absolute value has no defined meaning.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern TimeStamp ImageGetTimeStamp(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT uint64_t k4a_image_get_exposure_usec(k4a_image_t image_handle);
        /// <summary>Get the image exposure in microseconds.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// If the <paramref name="imageHandle"/> is invalid or if no exposure was set for the image,
        /// this function will return <c>0</c>. Otherwise,
        /// it will return the image exposure time in microseconds.
        /// </returns>
        /// <remarks>Returns an exposure time in microseconds. This is only supported on color image formats.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_exposure_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ImageGetExposureUsec(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT uint32_t k4a_image_get_white_balance(k4a_image_t image_handle);
        /// <summary>Get the image white balance.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// Returns the image white balance in Kelvin. If <paramref name="imageHandle"/> is invalid, or the white balance was not set or
        /// not applicable to the image, the function will return <c>0</c>.
        /// </returns>
        /// <remarks>Returns the image's white balance. This function is only valid for color captures, and not for depth or IR captures.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_white_balance", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImageGetWhiteBalance(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT uint32_t k4a_image_get_iso_speed(k4a_image_t image_handle);
        /// <summary>Get the image ISO speed.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// Returns the ISO speed of the image. <c>0</c> indicates the ISO speed was not available or an error occurred.
        /// </returns>
        /// <remarks>This function is only valid for color captures, and not for depth  or IR captures.</remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_get_iso_speed", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImageGetIsoSpeed(NativeHandles.ImageHandle imageHandle);

        // K4A_EXPORT void k4a_image_set_timestamp_usec(k4a_image_t image_handle, uint64_t timestamp_usec);
        /// <summary>Set the time stamp, in microseconds, of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the timestamp on.</param>
        /// <param name="timestamp">Time stamp of the image.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/> to construct an image.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_set_timestamp_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageSetTimeStamp(NativeHandles.ImageHandle imageHandle, TimeStamp timestamp);

        // K4A_EXPORT void k4a_image_set_exposure_time_usec(k4a_image_t image_handle, uint64_t exposure_usec);
        /// <summary>Set the exposure time, in microseconds, of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the exposure time on.</param>
        /// <param name="exposureUsec">Exposure time of the image in microseconds.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/> to construct an image.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_set_exposure_time_usec", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageSetExposureTimeUsec(NativeHandles.ImageHandle imageHandle, ulong exposureUsec);

        // K4A_EXPORT void k4a_image_set_white_balance(k4a_image_t image_handle, uint32_t white_balance);
        /// <summary>Set the white balance of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the white balance on.</param>
        /// <param name="whiteBalance">White balance of the image in degrees Kelvin.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/> to construct an image.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_set_white_balance", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageSetWhiteBalance(NativeHandles.ImageHandle imageHandle, uint whiteBalance);

        // K4A_EXPORT void k4a_image_set_iso_speed(k4a_image_t image_handle, uint32_t iso_speed);
        /// <summary>Set the ISO speed of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the ISO speed on.</param>
        /// <param name="isoSpeed">ISO speed of the image.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/> to construct an image.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_image_set_iso_speed", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageSetIsoSpeed(NativeHandles.ImageHandle imageHandle, uint isoSpeed);

        // K4A_EXPORT k4a_result_t k4a_device_start_cameras(k4a_device_t device_handle, k4a_device_configuration_t *config);
        /// <summary>Starts color and depth camera capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="config">The configuration we want to run the device in. This can be initialized with <see cref="DeviceConfiguration.DisableAll"/>.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Individual sensors configured to run will now start to stream captured data.
        /// 
        /// It is not valid to call this method a second time on the same device until <see cref="DeviceStopCameras(NativeHandles.DeviceHandle)"/> has been called.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_start_cameras", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceStartCameras(NativeHandles.DeviceHandle deviceHandle, [In] ref DeviceConfiguration config);

        // K4A_EXPORT void k4a_device_stop_cameras(k4a_device_t device_handle);
        /// <summary>Stops the color and depth camera capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <remarks>
        /// The streaming of individual sensors stops as a result of this call. Once called, <see cref="DeviceStartCameras(NativeHandles.DeviceHandle, ref DeviceConfiguration)"/>
        /// may be called again to resume sensor streaming.
        /// 
        /// This function may be called while another thread is blocking in <see cref="DeviceGetCapture(NativeHandles.DeviceHandle, out NativeHandles.CaptureHandle, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function returning a failure.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_stop_cameras", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeviceStopCameras(NativeHandles.DeviceHandle deviceHandle);

        // K4A_EXPORT k4a_result_t k4a_device_start_imu(k4a_device_t device_handle);
        /// <summary>Starts the IMU sample stream.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Call this API to start streaming IMU data. It is not valid to call this function a second time on the same
        /// device until <see cref="DeviceStopImu(NativeHandles.DeviceHandle)"/> has been called.
        /// 
        /// This function is dependent on the state of the cameras. The color or depth camera must be started before the IMU.
        /// <see cref="NativeApiCallResults.Result.Failed"/> will be returned if one of the cameras is not running.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_start_imu", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceStartImu(NativeHandles.DeviceHandle deviceHandle);


        // K4A_EXPORT void k4a_device_stop_imu(k4a_device_t device_handle);
        /// <summary>Stops the IMU capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <remarks>
        /// The streaming of the IMU stops as a result of this call. Once called, <see cref="DeviceStartImu(NativeHandles.DeviceHandle)"/> may
        /// be called again to resume sensor streaming, so long as the cameras are running.
        /// 
        /// This function may be called while another thread is blocking in <see cref="DeviceGetImuSample(NativeHandles.DeviceHandle, out ImuSample, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function returning a failure.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_stop_imu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeviceStopImu(NativeHandles.DeviceHandle deviceHandle);

        // K4A_EXPORT k4a_buffer_result_t k4a_device_get_serialnum(k4a_device_t device_handle,
        //                                                         char *serial_number,
        //                                                         size_t *serial_number_size);
        /// <summary>Get the Azure Kinect device serial number.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="serialNumber">
        /// Location to write the serial number to. If the function returns <see cref="NativeApiCallResults.BufferResult.Succeeded"/>,
        /// this will be a NULL-terminated string of ASCII characters.
        /// If this input is <see langword="null"/>, <paramref name="serialNumberSize"/> will still be updated to return
        /// the size of the buffer needed to store the string.
        /// </param>
        /// <param name="serialNumberSize">
        /// On input, the size of the <paramref name="serialNumber"/> buffer if that pointer is not <see langword="null"/>.
        /// On output, this value is set to the actual number of bytes in the serial number (including the null terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeApiCallResults.BufferResult.Succeeded"/> means that the <paramref name="serialNumber"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeApiCallResults.BufferResult.TooSmall"/> and the size of the serial number is
        /// returned in the <paramref name="serialNumberSize"/> parameter.
        /// All other failures return <see cref="NativeApiCallResults.BufferResult.Failed"/>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_serialnum", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern NativeApiCallResults.BufferResult DeviceGetSerialnum(
            NativeHandles.DeviceHandle deviceHandle,
            StringBuilder serialNumber,
            ref UIntPtr serialNumberSize);

        // K4A_EXPORT k4a_result_t k4a_device_get_version(k4a_device_t device_handle, k4a_hardware_version_t *version);
        /// <summary>Get the version numbers of the device's subsystems.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="version">Output parameter which on success will return version info.</param>
        /// <returns>
        /// A return of <see cref="NativeApiCallResults.Result.Succeeded"/> means that the version structure has been filled in.
        /// All other failures return <see cref="NativeApiCallResults.Result.Failed"/>.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceGetVersion(
            NativeHandles.DeviceHandle deviceHandle,
            out HardwareVersion version);

        // K4A_EXPORT k4a_result_t k4a_device_get_color_control_capabilities(k4a_device_t device_handle,
        //                                                                   k4a_color_control_command_t command,
        //                                                                   bool *supports_auto,
        //                                                                   int32_t *min_value,
        //                                                                   int32_t *max_value,
        //                                                                   int32_t *step_value,
        //                                                                   int32_t *default_value,
        //                                                                   k4a_color_control_mode_t *default_mode);
        /// <summary>Get the Azure Kinect color sensor control capabilities.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="supportsAuto">Output: whether the color sensor's control support auto mode or not. <see langword="true"/> if it supports auto mode, otherwise <see langword="false"/>.</param>
        /// <param name="minValue">Output: the color sensor's control minimum value of <paramref name="command"/>.</param>
        /// <param name="maxValue">Output: the color sensor's control maximum value of <paramref name="command"/>.</param>
        /// <param name="stepValue">Output: the color sensor's control step value of <paramref name="command"/>.</param>
        /// <param name="defaultValue">Output: the color sensor's control default value of <paramref name="command"/>.</param>
        /// <param name="defaultMode">Output: the color sensor's control default mode of <paramref name="command"/>.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the value was successfully returned, <see cref="NativeApiCallResults.Result.Failed"/> if an error occurred</returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_color_control_capabilities", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceGetColorControlCapabilities(
            NativeHandles.DeviceHandle deviceHandle,
            ColorControlCommand command,
            out bool supportsAuto,
            out int minValue,
            out int maxValue,
            out int stepValue,
            out int defaultValue,
            out ColorControlMode defaultMode);

        // K4A_EXPORT k4a_result_t k4a_device_get_color_control(k4a_device_t device_handle,
        //                                                      k4a_color_control_command_t command,
        //                                                      k4a_color_control_mode_t *mode,
        //                                                      int32_t *value);
        /// <summary>Get the Azure Kinect color sensor control value.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">This value is always written, but is only valid when the <paramref name="mode"/> returned is <see cref="ColorControlMode.Manual"/> for the current <paramref name="command"/>.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the value was successfully returned, <see cref="NativeApiCallResults.Result.Failed"/> if an error occurred.</returns>
        /// <remarks>
        /// Each control command may be set to manual or automatic. See the definition of <see cref="ColorControlCommand"/> on
        /// how to interpret the <paramref name="value"/> for each command.
        /// 
        /// Some control commands are only supported in manual mode. When a command is in automatic mode, the <paramref name="value"/> for
        /// that command is not valid.
        /// 
        /// Control values set on a device are reset only when the device is power cycled. The device will retain the
        /// settings even if the <paramref name="deviceHandle"/> is closed or the application is restarted.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_color_control", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceGetColorControl(
            NativeHandles.DeviceHandle deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value);

        // K4A_EXPORT k4a_result_t k4a_device_set_color_control(k4a_device_t device_handle,
        //                                                      k4a_color_control_command_t command,
        //                                                      k4a_color_control_mode_t mode,
        //                                                      int32_t value);
        /// <summary>Set the Azure Kinect color sensor control value.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">Color sensor control mode to set. This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">
        /// Value to set the color sensor's control to. The value is only valid if <paramref name="mode"/>
        /// is set to <see cref="ColorControlMode.Manual"/>, and is otherwise ignored.
        /// </param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the value was successfully set, <see cref="NativeApiCallResults.Result.Failed"/> if an error occurred</returns>
        /// <remarks>
        /// Each control command may be set to manual or automatic. See the definition of <see cref="ColorControlCommand"/> on how
        /// to interpret the <paramref name="value"/> for each command.
        /// 
        /// Some control commands are only supported in manual mode. When a command is in automatic mode, the <paramref name="value"/> for that
        /// command is not valid.
        /// 
        /// Control values set on a device are reset only when the device is power cycled. The device will retain the settings
        /// even if the device is closed or the application is restarted.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_set_color_control", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceSetColorControl(
            NativeHandles.DeviceHandle deviceHandle,
            ColorControlCommand command,
            ColorControlMode mode,
            int value);

        // K4A_EXPORT k4a_buffer_result_t k4a_device_get_raw_calibration(k4a_device_t device_handle,
        //                                                               uint8_t *data,
        //                                                               size_t *data_size);
        /// <summary>Get the raw calibration blob for the entire Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="data">
        /// Location to write the calibration data to. This field may optionally be set to <see langword="null"/> for the caller to query for
        /// the needed data size.
        /// </param>
        /// <param name="dataSize">
        /// On passing <paramref name="dataSize"/> into the function this variable represents the available size of the <paramref name="data"/>
        /// buffer. On return this variable is updated with the amount of data actually written to the buffer, or the size
        /// required to store the calibration buffer if <paramref name="data"/> is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.BufferResult.Succeeded"/> if <paramref name="data"/> was successfully written.
        /// If <paramref name="dataSize"/> points to a buffer size that is
        /// too small to hold the output or <paramref name="data"/> data is <see langword="null"/>, <see cref="NativeApiCallResults.BufferResult.TooSmall"/> is returned
        /// and <paramref name="dataSize"/> is updated to contain the minimum buffer size needed to capture the calibration data.
        /// </returns>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_raw_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.BufferResult DeviceGetRawCalibration(
            NativeHandles.DeviceHandle deviceHandle,
            [Out] byte[] data,
            ref UIntPtr dataSize);

        // K4A_EXPORT k4a_result_t k4a_device_get_calibration(k4a_device_t device_handle,
        //                                                    const k4a_depth_mode_t depth_mode,
        //                                                    const k4a_color_resolution_t color_resolution,
        //                                                    k4a_calibration_t *calibration);
        /// <summary>Get the camera calibration for the entire Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <param name="calibration">Output: calibration data.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="calibration"/> was successfully written. <see cref="NativeApiCallResults.Result.Failed"/> otherwise.</returns>
        /// <remarks>
        /// The <paramref name="calibration"/> represents the data needed to transform between the camera views and may be
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// 
        /// The <paramref name="calibration"/> output is used as input to all calibration and transformation functions.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_calibration", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceGetCalibration(
            NativeHandles.DeviceHandle deviceHandle,
            DepthMode depthMode,
            ColorResolution colorResolution,
            out Calibration calibration);

        // K4A_EXPORT k4a_result_t k4a_device_get_sync_jack(k4a_device_t device_handle,
        //                                                  bool *sync_in_jack_connected,
        //                                                  bool *sync_out_jack_connected);
        /// <summary>Get the device jack status for the synchronization in and synchronization out connectors.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle)"/>.</param>
        /// <param name="syncInJackConnected">Upon successful return this value will be set to true if a cable is connected to this sync in jack.</param>
        /// <param name="syncOutJackConnected">Upon successful return this value will be set to true if a cable is connected to this sync out jack.</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if the connector status was successfully read.</returns>
        /// <remarks>
        /// If <paramref name="syncOutJackConnected"/> is <see langword="true"/> then <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to
        /// <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Master"/>. If <paramref name="syncInJackConnected"/> is <see langword="true"/> then
        /// <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Subordinate"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_device_get_sync_jack", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result DeviceGetSyncJack(
            NativeHandles.DeviceHandle deviceHandle,
            out bool syncInJackConnected,
            out bool syncOutJackConnected);

        // K4A_EXPORT k4a_result_t k4a_calibration_get_from_raw(char *raw_calibration,
        //                                                      size_t raw_calibration_size,
        //                                                      const k4a_depth_mode_t depth_mode,
        //                                                      const k4a_color_resolution_t color_resolution,
        //                                                      k4a_calibration_t *calibration);
        /// <summary>Get the camera calibration for a device from a raw calibration blob.</summary>
        /// <param name="rawCalibration">Raw calibration blob obtained from a device or recording. The raw calibration must be NULL terminated.</param>
        /// <param name="rawCalibrationSize">The size, in bytes, of <paramref name="rawCalibration"/> including the NULL termination.</param>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <param name="calibration">Result: calibration data</param>
        /// <returns><see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="calibration"/> was successfully written. <see cref="NativeApiCallResults.Result.Failed"/> otherwise.</returns>
        /// <remarks>
        /// The <paramref name="calibration"/> represents the data needed to transform between the camera views and is
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// 
        /// The <paramref name="calibration"/> output is used as input to all transformation functions.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_calibration_get_from_raw", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result CalibrationGetFromRaw(
            byte[] rawCalibration,
            UIntPtr rawCalibrationSize,
            DepthMode depthMode,
            ColorResolution colorResolution,
            out Calibration calibration);

        // K4A_EXPORT k4a_result_t k4a_calibration_3d_to_3d(const k4a_calibration_t *calibration,
        //                                                  const k4a_float3_t *source_point3d_mm,
        //                                                  const k4a_calibration_type_t source_camera,
        //                                                  const k4a_calibration_type_t target_camera,
        //                                                  k4a_float3_t *target_point3d_mm);
        /// <summary>Transform a 3D point of a source coordinate system into a 3D point of the target coordinate system.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint3DMm">The 3D coordinates in millimeters representing a point in <paramref name="sourceCamera"/>.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint3DMm">Output: the new 3D coordinates of the input point in the coordinate space <paramref name="targetCamera"/> in millimeters.</param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="targetPoint3DMm"/> was successfully written.
        /// <see cref="NativeApiCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// </returns>
        /// <remarks>
        /// This function is used to transform 3D points between depth and color camera coordinate systems. The function uses the
        /// extrinsic camera calibration. It computes the output via multiplication with a precomputed matrix encoding a 3D
        /// rotation and a 3D translation. If <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> are the same, then <paramref name="targetPoint3DMm"/> will
        /// be identical to <paramref name="sourcePoint3DMm"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_calibration_3d_to_3d", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result Calibration3DTo3D(
            [In] ref Calibration calibration,
            [In] ref Float3 sourcePoint3DMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float3 targetPoint3DMm);

        // K4A_EXPORT k4a_result_t k4a_calibration_2d_to_3d(const k4a_calibration_t *calibration,
        //                                                  const k4a_float2_t *source_point2d,
        //                                                  const float source_depth_mm,
        //                                                  const k4a_calibration_type_t source_camera,
        //                                                  const k4a_calibration_type_t target_camera,
        //                                                  k4a_float3_t *target_point3d_mm,
        //                                                  int *valid);
        /// <summary>
        /// Transform a 2D pixel coordinate with an associated depth value of the source camera
        /// into a 3D point of the target coordinate system.
        /// </summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">The depth of <paramref name="sourcePoint2D"/> in millimeters.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint3DMm">Output: the 3D coordinates of the input pixel in the coordinate system of <paramref name="targetCamera"/> in millimeters.</param>
        /// <param name="valid">
        /// The output parameter returns a value of <see langword="true"/> if the <paramref name="sourcePoint2D"/> is a valid coordinate,
        /// and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="targetPoint3DMm"/> was successfully written.
        /// <see cref="NativeApiCallResults.Result.Failed"/> if <paramref name="calibration"/>
        /// contained invalid transformation parameters.
        /// If the function returns <see cref="NativeApiCallResults.Result.Succeeded"/>, but <paramref name="valid"/> valid is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint3DMm"/> are outside of the range of valid
        /// calibration and should be ignored.
        /// </returns>
        /// <remarks>
        /// This function applies the intrinsic calibration of <paramref name="sourceCamera"/> to compute the 3D ray from the focal point of the
        /// camera through pixel <paramref name="sourcePoint2D"/>.The 3D point on this ray is then found using <paramref name="sourceDepthMm"/>. If
        /// <paramref name="targetCamera"/> is different from <paramref name="sourceCamera"/>, the 3D point is transformed to <paramref name="targetCamera"/> using
        /// <see cref="Calibration3DTo3D(ref Calibration, ref Float3, CalibrationGeometry, CalibrationGeometry, out Float3)"/>.
        /// In practice, <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> will often be identical. In this
        /// case, no 3D to 3D transformation is applied.
        /// 
        /// If <paramref name="sourcePoint2D"/> is not considered as valid pixel coordinate
        /// according to the intrinsic camera model, <paramref name="valid"/> is set to <see langword="false"/>.
        /// If it is valid, <paramref name="valid"/> valid will be set to <see langword="true"/>. The user
        /// should not use the value of <paramref name="targetPoint3DMm"/> if <paramref name="valid"/> was set to <see langword="false"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_calibration_2d_to_3d", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result Calibration2DTo3D(
            [In] ref Calibration calibration,
            [In] ref Float2 sourcePoint2D,
            float sourceDepthMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float3 targetPoint3DMm,
            out bool valid);

        // K4A_EXPORT k4a_result_t k4a_calibration_3d_to_2d(const k4a_calibration_t *calibration,
        //                                                  const k4a_float3_t *source_point3d_mm,
        //                                                  const k4a_calibration_type_t source_camera,
        //                                                  const k4a_calibration_type_t target_camera,
        //                                                  k4a_float2_t *target_point2d,
        //                                                  int *valid);
        /// <summary>Transform a 3D point of a source coordinate system into a 2D pixel coordinate of the target camera.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint3DMm">The 3D coordinates in millimeters representing a point in <paramref name="sourceCamera"/>.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint2D">Output: the 2D pixel in <paramref name="targetCamera"/> coordinates.</param>
        /// <param name="valid">
        /// The output parameter returns <see langword="true"/> if the <paramref name="sourcePoint3DMm"/> is a valid coordinate in the <paramref name="targetCamera"/>
        /// coordinate system, and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="targetPoint2D"/> was successfully written.
        /// <see cref="NativeApiCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// If the function returns <see cref="NativeApiCallResults.Result.Succeeded"/>, but <paramref name="valid"/> is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint2D"/> are outside of the range of valid calibration
        /// and should be ignored.
        /// </returns>
        /// <remarks>
        /// If <paramref name="targetCamera"/> is different from <paramref name="sourceCamera"/>, <paramref name="sourcePoint3DMm"/> is transformed
        /// to <paramref name="targetCamera"/> using <see cref="Calibration3DTo3D(ref Calibration, ref Float3, CalibrationGeometry, CalibrationGeometry, out Float3)"/>.
        /// In practice, <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> will often be identical.
        /// In this case, no 3D to 3D transformation is applied. The 3D point in the coordinate system of <paramref name="targetCamera"/> is then
        /// projected onto the image plane using the intrinsic calibration of <paramref name="targetCamera"/>.
        /// 
        /// If <paramref name="sourcePoint3DMm"/> does not map to a valid 2D coordinate in the <paramref name="targetCamera"/> coordinate system,
        /// <paramref name="valid"/> is set to <see langword="false"/>. If it is valid, <paramref name="valid"/> will be set to <see langword="true"/>.
        /// The user should not use the value of <paramref name="targetPoint2D"/> if <paramref name="valid"/> was set to <see langword="false"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_calibration_3d_to_2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result Calibration3DTo2D(
            [In] ref Calibration calibration,
            [In] ref Float3 sourcePoint3DMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float2 targetPoint2D,
            out bool valid);

        // K4A_EXPORT k4a_result_t k4a_calibration_2d_to_2d(const k4a_calibration_t *calibration,
        //                                                  const k4a_float2_t *source_point2d,
        //                                                  const float source_depth_mm,
        //                                                  const k4a_calibration_type_t source_camera,
        //                                                  const k4a_calibration_type_t target_camera,
        //                                                  k4a_float2_t *target_point2d,
        //                                                  int *valid);
        /// <summary>
        /// Transform a 2D pixel coordinate with an associated depth value of the source camera into a 2D pixel coordinate of the target camera.
        /// </summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">The depth of <paramref name="sourcePoint2D"/> in millimeters.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint2D">Output: the 2D pixel in <paramref name="targetCamera"/> coordinates.</param>
        /// <param name="valid">
        /// The output parameter returns <see langword="true"/> if the <paramref name="sourcePoint2D"/> is a valid coordinate in the <paramref name="targetCamera"/>
        /// coordinate system, and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="targetPoint2D"/> was successfully written.
        /// <see cref="NativeApiCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// If the function returns <see cref="NativeApiCallResults.Result.Succeeded"/>, but <paramref name="valid"/> valid is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint2D"/> are outside of the range of valid calibration
        /// and should be ignored.
        /// </returns>
        /// <remarks>
        /// This function maps a pixel between the coordinate systems of the depth and color cameras. It is equivalent to calling
        /// <see cref="Calibration2DTo3D(ref Calibration, ref Float2, float, CalibrationGeometry, CalibrationGeometry, out Float3, out bool)"/> to compute the 3D point corresponding to <paramref name="sourcePoint2D"/> and then using
        /// <see cref="Calibration3DTo2D(ref Calibration, ref Float3, CalibrationGeometry, CalibrationGeometry, out Float2, out bool)"/> to map the 3D point into the coordinate system of the <paramref name="targetCamera"/>.
        /// 
        /// If <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> are identical, the function immediately sets <paramref name="targetPoint2D"/> to
        /// <paramref name="sourcePoint2D"/> and returns without computing any transformations.
        /// 
        /// If <paramref name="sourcePoint2D"/> does not map to a valid 2D coordinate in the <paramref name="targetCamera"/> coordinate system,
        /// <paramref name="valid"/> is set to <see langword="false"/>. If it is valid, <paramref name="valid"/> will be set to <see langword="true"/>.
        /// The user should not use the value of <paramref name="targetPoint2D"/> if <paramref name="valid"/> was set to 0.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_calibration_2d_to_2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result Calibration2DTo2D(
            [In] ref Calibration calibration,
            [In] ref Float2 sourcePoint2D,
            float sourceDepthMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float2 targetPoint2D,
            out bool valid);

        // K4A_EXPORT k4a_transformation_t k4a_transformation_create(const k4a_calibration_t *calibration);
        /// <summary>Get handle to transformation.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <returns>A transformation handle. An invalid handle is returned if creation fails.</returns>
        /// <remarks>
        /// The transformation handle is used to transform images from the coordinate system of one camera into the other. Each
        /// transformation handle requires some pre-computed resources to be allocated, which are retained until the handle is
        /// destroyed.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_transformation_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandles.TransformationHandle TransformationCreate([In] ref Calibration calibration);

        // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_color_camera(k4a_transformation_t transformation_handle,
        //                                                                        const k4a_image_t depth_image,
        //                                                                        k4a_image_t transformed_depth_image);
        /// <summary>Transforms the depth map into the geometry of the color camera.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="transformedDepthImage">Handle to output transformed depth image.</param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="transformedDepthImage"/> was successfully written
        /// and <see cref="NativeApiCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// This produces a depth image for which each pixel matches the corresponding pixel coordinates of the color camera.
        /// 
        /// <paramref name="depthImage"/> and <paramref name="transformedDepthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// 
        /// <paramref name="transformedDepthImage"/> must have a width and height matching the width and height of the color camera in the mode
        /// specified by the <see cref="Calibration"/> used to create the <paramref name="transformationHandle"/> with <see cref="TransformationCreate(ref Calibration)"/>.
        /// 
        /// The contents <paramref name="transformedDepthImage"/> will be filled with the depth values derived from <paramref name="depthImage"/> in the color
        /// camera's coordinate space.
        /// 
        /// <paramref name="transformedDepthImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_transformation_depth_image_to_color_camera", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result TransformationDepthImageToColorCamera(
            NativeHandles.TransformationHandle transformationHandle,
            NativeHandles.ImageHandle depthImage,
            NativeHandles.ImageHandle transformedDepthImage);

        // K4A_EXPORT k4a_result_t k4a_transformation_color_image_to_depth_camera(k4a_transformation_t transformation_handle,
        //                                                                        const k4a_image_t depth_image,
        //                                                                        const k4a_image_t color_image,
        //                                                                        k4a_image_t transformed_color_image);
        /// <summary>Transforms a color image into the geometry of the depth camera.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="colorImage">Handle to input color image.</param>
        /// <param name="transformedColorImage">Handle to output transformed color image.</param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="transformedColorImage"/> was successfully written
        /// and <see cref="NativeApiCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// This produces a color image for which each pixel matches the corresponding pixel coordinates of the depth camera.
        /// 
        /// <paramref name="depthImage"/> and <paramref name="colorImage"/> need to represent the same moment in time. The depth data will be applied to the
        /// color image to properly warp the color data to the perspective of the depth camera.
        /// 
        /// <paramref name="depthImage"/> must be of type <see cref="ImageFormat.Depth16"/>. <paramref name="colorImage"/> must be of format
        /// <see cref="ImageFormat.ColorBGRA32"/>.
        /// 
        /// <paramref name="transformedColorImage"/> image must be of format <see cref="ImageFormat.ColorBGRA32"/>. <paramref name="transformedColorImage"/> must
        /// have the width and height of the depth camera in the mode specified by the <see cref="Calibration"/> used to create
        /// the <paramref name="transformationHandle"/> with <see cref="TransformationCreate(ref Calibration)"/>.
        /// 
        /// <paramref name="transformedColorImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_transformation_color_image_to_depth_camera", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result TransformationColorImageToDepthCamera(
            NativeHandles.TransformationHandle transformationHandle,
            NativeHandles.ImageHandle depthImage,
            NativeHandles.ImageHandle colorImage,
            NativeHandles.ImageHandle transformedColorImage);

        // K4A_EXPORT k4a_result_t k4a_transformation_depth_image_to_point_cloud(k4a_transformation_t transformation_handle,
        //                                                                       const k4a_image_t depth_image,
        //                                                                       const k4a_calibration_type_t camera,
        //                                                                       k4a_image_t xyz_image);
        /// <summary>Transforms the depth image into 3 planar images representing X, Y and Z-coordinates of corresponding 3D points.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="camera">Geometry in which depth map was computed.</param>
        /// <param name="xyzImage">Handle to output xyz image.</param>
        /// <returns>
        /// <see cref="NativeApiCallResults.Result.Succeeded"/> if <paramref name="xyzImage"/> was successfully written
        /// and <see cref="NativeApiCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// <paramref name="depthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// 
        /// The <paramref name="camera"/> parameter tells the function what the perspective of the <paramref name="depthImage"/> is.
        /// If the <paramref name="depthImage"/> was captured directly from the depth camera, the value should be <see cref="CalibrationGeometry.Depth"/>.
        /// If the <paramref name="depthImage"/> is the result of a transformation into the color camera's coordinate space using
        /// <see cref="TransformationDepthImageToColorCamera(NativeHandles.TransformationHandle, NativeHandles.ImageHandle, NativeHandles.ImageHandle)"/>,
        /// the value should be <see cref="CalibrationGeometry.Color"/>.
        /// 
        /// The format of <paramref name="xyzImage"/> must be <see cref="ImageFormat.Custom"/>. The width and height of <paramref name="xyzImage"/> must match the
        /// width and height of <paramref name="depthImage"/>. <paramref name="xyzImage"/> must have a stride in bytes of at least 6 times its width in pixels.
        /// 
        /// Each pixel of the <paramref name="xyzImage"/> consists of three <see cref="short"/> values, totaling 6 bytes. The three <see cref="short"/> values are the
        /// X, Y, and Z values of the point.
        /// 
        /// <paramref name="xyzImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle)"/>.
        /// </remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_transformation_depth_image_to_point_cloud", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeApiCallResults.Result TransformationDepthImageToPointCloud(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                CalibrationGeometry camera,
                NativeHandles.ImageHandle xyzImage);
    }
}
