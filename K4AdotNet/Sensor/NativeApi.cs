using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    /// <summary>DLL imports for most of native functions from <c>k4a.h</c> header file.</summary>
    internal abstract partial class NativeApi
    {
        /// <summary>Default device index.</summary>
        /// <remarks>Passed as an argument to <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/> to open the default sensor.</remarks>
        public const uint DEFAULT_DEVICE_INDEX = 0;

        public static NativeApi GetInstance(bool isOrbbec)
            => isOrbbec ? Orbbec.Instance : Azure.Instance;

        public abstract bool IsOrbbec { get; }

        /// <summary>Gets the number of connected devices.</summary>
        /// <returns>Number of sensors connected to the PC.</returns>
        public abstract uint DeviceGetInstalledCount();

        /// <summary>Open an Azure Kinect device.</summary>
        /// <param name="index">The index of the device to open, starting with 0. Use <see cref="DEFAULT_DEVICE_INDEX"/> constant as value for this parameter to open default device.</param>
        /// <param name="deviceHandle">Output parameter which on success will return a handle to the device.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the device was opened successfully.</returns>
        public abstract NativeCallResults.Result DeviceOpen(uint index, out NativeHandles.DeviceHandle? deviceHandle);

        /// <summary>Reads a sensor capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="captureHandle">If successful this contains a handle to a capture object.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the capture.
        /// If set to <see cref="Timeout.NoWait"/>, the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.WaitResult.Succeeded"/> if a capture is returned.
        /// If a capture is not available before the timeout elapses, the function will return <see cref="NativeCallResults.WaitResult.Timeout"/>.
        /// All other failures will return <see cref="NativeCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="DeviceStartCameras"/> is called and before <see cref="DeviceStopCameras"/> is called.
        /// </remarks>
        public abstract NativeCallResults.WaitResult DeviceGetCapture(
            NativeHandles.DeviceHandle deviceHandle,
            out NativeHandles.CaptureHandle? captureHandle,
            Timeout timeout);

        /// <summary>Reads an IMU sample.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="imuSample">Information about IMU sample.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the sample.
        /// If set to <see cref="Timeout.NoWait"/>, the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.WaitResult.Succeeded"/> if a sample is returned.
        /// If a sample is not available before the timeout elapses, the function will return <see cref="NativeCallResults.WaitResult.Timeout"/>.
        /// All other failures will return <see cref="NativeCallResults.WaitResult.Failed"/>.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="DeviceStartCameras"/> is called and before <see cref="DeviceStopCameras"/> is called.
        /// </remarks>
        public abstract NativeCallResults.WaitResult DeviceGetImuSample(
            NativeHandles.DeviceHandle deviceHandle,
            out ImuSample imuSample,
            Timeout timeout);

        /// <summary>Create an empty capture object.</summary>
        /// <param name="captureHandle">Output parameter which on success will return a handle to the capture.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the device was opened successfully.</returns>
        public abstract NativeCallResults.Result CaptureCreate(out NativeHandles.CaptureHandle? captureHandle);

        /// <summary>Get the color image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the color image part of this capture.</remarks>
        public abstract NativeHandles.ImageHandle? CaptureGetColorImage(NativeHandles.CaptureHandle captureHandle);

        /// <summary>Get the depth image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the depth image part of this capture.</remarks>
        public abstract NativeHandles.ImageHandle? CaptureGetDepthImage(NativeHandles.CaptureHandle captureHandle);

        /// <summary>Get the IR image associated with the given capture.</summary>
        /// <param name="captureHandle">Capture handle containing the image.</param>
        /// <returns>Image handle.</returns>
        /// <remarks>Call this function to access the IR image part of this capture.</remarks>
        public abstract NativeHandles.ImageHandle? CaptureGetIRImage(NativeHandles.CaptureHandle captureHandle);

        /// <summary>Set or add a color image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see cref="IntPtr.Zero"/> to remove color image from a given capture if any.</param>
        /// <remarks>If there is already a color image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        public abstract void CaptureSetColorImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        /// <summary>Set or add a depth image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see cref="IntPtr.Zero"/> to remove depth image from a given capture if any.</param>
        /// <remarks>If there is already a depth image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        public abstract void CaptureSetDepthImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        /// <summary>Set or add a IR image to the associated capture.</summary>
        /// <param name="captureHandle">Capture handle to hold the image.</param>
        /// <param name="imageHandle">Image handle containing the image or <see cref="IntPtr.Zero"/> to remove IR image from a given capture if any.</param>
        /// <remarks>If there is already a IR image contained in the capture, the existing image will be dereferenced and replaced with the new image.</remarks>
        public abstract void CaptureSetIRImage(NativeHandles.CaptureHandle captureHandle, NativeHandles.ImageHandle imageHandle);

        // typedef uint8_t *(k4a_memory_allocate_cb_t)(int size, void **context);
        /// <summary>Callback function for a memory allocation.</summary>
        /// <param name="size">Minimum size in bytes needed for the buffer.</param>
        /// <param name="context">Output parameter for a context that will be provided in the subsequent call to the <see cref="MemoryDestroyCallback"/> callback.</param>
        /// <returns>A pointer to the newly allocated memory.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr MemoryAllocateCallback(int size, out IntPtr context);

        // typedef void(k4a_memory_destroy_cb_t)(void *buffer, void *context);
        /// <summary>Callback function for a memory object being destroyed.</summary>
        /// <param name="buffer">The buffer pointer that was supplied by the caller.</param>
        /// <param name="context">The context for the memory object that needs to be destroyed that was supplied by the caller.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MemoryDestroyCallback(IntPtr buffer, IntPtr context);

        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="format">The format of the image that will be stored in this image container.</param>
        /// <param name="widthPixels">Width in pixels.</param>
        /// <param name="heightPixels">Height in pixels.</param>
        /// <param name="strideBytes">
        /// The number of bytes per horizontal line of the image.
        /// If set to 0, the stride will be set to the minimum size given the <paramref name="format"/> and <paramref name="widthPixels"/>.
        /// </param>
        /// <param name="imageHandle">Handle of created image in case of success.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> on success.</returns>
        /// <remarks>This function is used to create images of formats that have consistent stride.
        /// The function is not suitable for compressed formats that may not be represented by the same number of bytes per line.
        /// 
        /// For most image formats, the function will allocate an image buffer of size <paramref name="heightPixels"/> * <paramref name="strideBytes"/>.
        /// Buffers <see cref="ImageFormat.ColorNV12"/> format will allocate an additional <paramref name="heightPixels"/> / 2 set of lines (each of
        /// <paramref name="strideBytes"/>).
        /// 
        /// This function cannot be used to allocate <see cref="ImageFormat.ColorMjpg"/> buffers.
        /// 
        /// To create an image object without the API allocating memory, or to represent an image that has a non-deterministic
        /// stride, use <see cref="ImageCreateFromBuffer"/>.
        /// </remarks>
        public abstract NativeCallResults.Result ImageCreate(
            ImageFormat format,
            int widthPixels,
            int heightPixels,
            int strideBytes,
            out NativeHandles.ImageHandle? imageHandle);

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
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> on success.</returns>
        /// <remarks>
        /// This function creates an <see cref="NativeHandles.ImageHandle"/> from a pre-allocated buffer. When all references to this object reach zero
        /// the provided <paramref name="bufferReleaseCallback"/> callback function is called so that the memory can be released.
        /// </remarks>
        public abstract NativeCallResults.Result ImageCreateFromBuffer(
            ImageFormat format,
            int widthPixels,
            int heightPixels,
            int strideBytes,
            IntPtr buffer,
            UIntPtr bufferSize,
            MemoryDestroyCallback? bufferReleaseCallback,
            IntPtr bufferReleaseCallbackContext,
            out NativeHandles.ImageHandle? imageHandle);

        /// <summary>Get the image buffer.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// The function will return <see cref="IntPtr.Zero"/> if there is an error, and will normally return a pointer to the image buffer.
        /// </returns>
        /// <remarks>Use this buffer to access the raw image data.</remarks>
        public abstract IntPtr ImageGetBuffer(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image buffer size.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>The function will return <see cref="UIntPtr.Zero"/> if there is an error, and will normally return the image size.</returns>
        /// <remarks>Use this function to know what the size of the image buffer is returned by <see cref="ImageGetBuffer"/>.</remarks>
        public abstract UIntPtr ImageGetSize(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the format of the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known format.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <see cref="ImageFormat.Custom"/>.
        /// </returns>
        /// <remarks>Use this function to determine the format of the image buffer.</remarks>
        public abstract ImageFormat ImageGetFormat(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image width in pixels.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known width.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <c>0</c>.
        /// </returns>
        public abstract int ImageGetWidthPixels(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image height in pixels.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known height.
        /// If the <paramref name="imageHandle"/> is invalid, the function will return <c>0</c>.
        /// </returns>
        public abstract int ImageGetHeightPixels(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image stride in bytes.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// This function is not expected to fail, all images are created with a known stride.
        /// If the <paramref name="imageHandle"/> is invalid or the image's format does not have a stride, the function will return <c>0</c>.
        /// </returns>
        public abstract int ImageGetStrideBytes(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image's device timestamp in microseconds.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// If the <paramref name="imageHandle"/> is invalid or if no timestamp was set for the image, this function will return <see cref="Microseconds64.Zero"/>.
        /// It is also possible for <see cref="Microseconds64.Zero"/> to be a valid timestamp originating from the beginning of a recording or the start of streaming.
        /// </returns>
        /// <remarks>
        /// Returns the device timestamp of the image, as captured by the hardware.Timestamps are recorded by the device and
        /// represent the mid-point of exposure.They may be used for relative comparison, but their absolute value has no
        /// defined meaning.
        /// </remarks>
        public abstract Microseconds64 ImageGetDeviceTimestamp(NativeHandles.ImageHandle imageHandle);

        /// <summary>Get the image's system timestamp in nanoseconds.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <returns>
        /// If the <paramref name="imageHandle"/> is invalid or if no timestamp was set for the image, this function will return <see cref="Nanoseconds64.Zero"/>.
        /// It is also possible for <see cref="Nanoseconds64.Zero"/> to be a valid timestamp originating from the beginning of a recording or the start of streaming.
        /// </returns>
        /// <remarks>
        /// Returns the system timestamp of the image. Timestamps are recorded by the host. They may be used for relative
        /// comparison, as they are relative to the corresponding system clock.The absolute value is a monotonic count from
        /// an arbitrary point in the past.
        /// 
        /// The system timestamp is captured at the moment host PC finishes receiving the image.
        /// 
        /// On Linux the system timestamp is read from <c>clock_gettime(CLOCK_MONOTONIC)</c>, which measures realtime and is not
        /// impacted by adjustments to the system clock.It starts from an arbitrary point in the past. On Windows the system
        /// timestamp is read from <c>QueryPerformanceCounter()</c>, it also measures realtime and is not impacted by adjustments to the
        /// system clock. It also starts from an arbitrary point in the past.
        /// </remarks>
        public abstract Nanoseconds64 ImageGetSystemTimestamp(NativeHandles.ImageHandle imageHandle);

        /// <summary>Set the device time stamp, in microseconds, of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the timestamp on.</param>
        /// <param name="timestamp">Time stamp of the image.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/> to construct an image.
        /// </remarks>
        public abstract void ImageSetDeviceTimestamp(NativeHandles.ImageHandle imageHandle, Microseconds64 timestamp);

        /// <summary>Set the system time stamp, in nanoseconds, of the image.</summary>
        /// <param name="imageHandle">Handle of the image to set the timestamp on.</param>
        /// <param name="timestamp">Time stamp of the image.</param>
        /// <remarks>
        /// Use this function in conjunction with <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/> to construct an image.
        /// 
        /// The system timestamp is a high performance and increasing clock (from boot). The timestamp represents the time
        /// immediately after the image buffer was read by the host PC.
        /// </remarks>
        public abstract void ImageSetSystemTimestamp(NativeHandles.ImageHandle imageHandle, Nanoseconds64 timestamp);

        /// <summary>Starts color and depth camera capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="config">The configuration we want to run the device in. This can be initialized with <see cref="DeviceConfiguration.DisableAll"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Individual sensors configured to run will now start to stream captured data.
        /// 
        /// It is not valid to call this method a second time on the same device until <see cref="DeviceStopCameras(NativeHandles.DeviceHandle)"/> has been called.
        /// </remarks>
        public abstract NativeCallResults.Result DeviceStartCameras(NativeHandles.DeviceHandle deviceHandle, in DeviceConfiguration config);

        /// <summary>Stops the color and depth camera capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <remarks>
        /// The streaming of individual sensors stops as a result of this call. Once called, <see cref="DeviceStartCameras(NativeHandles.DeviceHandle, in DeviceConfiguration)"/>
        /// may be called again to resume sensor streaming.
        /// 
        /// This function may be called while another thread is blocking in <see cref="DeviceGetCapture(NativeHandles.DeviceHandle, out NativeHandles.CaptureHandle?, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function returning a failure.
        /// </remarks>
        public abstract void DeviceStopCameras(NativeHandles.DeviceHandle deviceHandle);

        /// <summary>Starts the IMU sample stream.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> is returned on success.</returns>
        /// <remarks>
        /// Call this API to start streaming IMU data. It is not valid to call this function a second time on the same
        /// device until <see cref="DeviceStopImu(NativeHandles.DeviceHandle)"/> has been called.
        /// 
        /// This function is dependent on the state of the cameras. The color or depth camera must be started before the IMU.
        /// <see cref="NativeCallResults.Result.Failed"/> will be returned if one of the cameras is not running.
        /// </remarks>
        public abstract NativeCallResults.Result DeviceStartImu(NativeHandles.DeviceHandle deviceHandle);

        /// <summary>Stops the IMU capture.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <remarks>
        /// The streaming of the IMU stops as a result of this call. Once called, <see cref="DeviceStartImu(NativeHandles.DeviceHandle)"/> may
        /// be called again to resume sensor streaming, so long as the cameras are running.
        /// 
        /// This function may be called while another thread is blocking in <see cref="DeviceGetImuSample(NativeHandles.DeviceHandle, out ImuSample, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function returning a failure.
        /// </remarks>
        public abstract void DeviceStopImu(NativeHandles.DeviceHandle deviceHandle);

        /// <summary>Get the Azure Kinect device serial number.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="buffer">
        /// Location to write the serial number to. If the function returns <see cref="NativeCallResults.BufferResult.Succeeded"/>,
        /// this will be a NULL-terminated string of ASCII characters.
        /// If this input is <see langword="null"/>, <paramref name="size"/> will still be updated to return
        /// the size of the buffer needed to store the string.
        /// </param>
        /// <param name="size">
        /// On input, the size of the <paramref name="buffer"/> buffer if that pointer is not <see langword="null"/>.
        /// On output, this value is set to the actual number of bytes in the serial number (including the null terminator).
        /// </param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.BufferResult.Succeeded"/> means that the <paramref name="buffer"/> has been filled in.
        /// If the buffer is too small the function returns <see cref="NativeCallResults.BufferResult.TooSmall"/> and the size of the serial number is
        /// returned in the <paramref name="size"/> parameter.
        /// All other failures return <see cref="NativeCallResults.BufferResult.Failed"/>.
        /// </returns>
        public abstract NativeCallResults.BufferResult DeviceGetSerialnum(
            NativeHandles.DeviceHandle deviceHandle,
            IntPtr buffer,
            ref UIntPtr size);

        /// <summary>Get the version numbers of the device's subsystems.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="version">Output parameter which on success will return version info.</param>
        /// <returns>
        /// A return of <see cref="NativeCallResults.Result.Succeeded"/> means that the version structure has been filled in.
        /// All other failures return <see cref="NativeCallResults.Result.Failed"/>.
        /// </returns>
        public abstract NativeCallResults.Result DeviceGetVersion(
            NativeHandles.DeviceHandle deviceHandle,
            out HardwareVersion version);

        /// <summary>Get the Azure Kinect color sensor control capabilities.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="supportsAuto">Output: whether the color sensor's control support auto mode or not. <see langword="true"/> if it supports auto mode, otherwise <see langword="false"/>.</param>
        /// <param name="minValue">Output: the color sensor's control minimum value of <paramref name="command"/>.</param>
        /// <param name="maxValue">Output: the color sensor's control maximum value of <paramref name="command"/>.</param>
        /// <param name="stepValue">Output: the color sensor's control step value of <paramref name="command"/>.</param>
        /// <param name="defaultValue">Output: the color sensor's control default value of <paramref name="command"/>.</param>
        /// <param name="defaultMode">Output: the color sensor's control default mode of <paramref name="command"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the value was successfully returned, <see cref="NativeCallResults.Result.Failed"/> if an error occurred</returns>
        public abstract NativeCallResults.Result DeviceGetColorControlCapabilities(
            NativeHandles.DeviceHandle deviceHandle,
            ColorControlCommand command,
            out byte supportsAuto,
            out int minValue,
            out int maxValue,
            out int stepValue,
            out int defaultValue,
            out ColorControlMode defaultMode);

        /// <summary>Get the Azure Kinect color sensor control value.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">This value is always written, but is only valid when the <paramref name="mode"/> returned is <see cref="ColorControlMode.Manual"/> for the current <paramref name="command"/>.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the value was successfully returned, <see cref="NativeCallResults.Result.Failed"/> if an error occurred.</returns>
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
        public abstract NativeCallResults.Result DeviceGetColorControl(
            NativeHandles.DeviceHandle deviceHandle, ColorControlCommand command, out ColorControlMode mode, out int value);

        /// <summary>Set the Azure Kinect color sensor control value.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">Color sensor control mode to set. This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">
        /// Value to set the color sensor's control to. The value is only valid if <paramref name="mode"/>
        /// is set to <see cref="ColorControlMode.Manual"/>, and is otherwise ignored.
        /// </param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if the value was successfully set, <see cref="NativeCallResults.Result.Failed"/> if an error occurred</returns>
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
        public abstract NativeCallResults.Result DeviceSetColorControl(
            NativeHandles.DeviceHandle deviceHandle,
            ColorControlCommand command,
            ColorControlMode mode,
            int value);

        /// <summary>Get the raw calibration blob for the entire Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="buffer">
        /// Location to write the calibration data to. This field may optionally be set to <see langword="null"/> for the caller to query for
        /// the needed data size.
        /// </param>
        /// <param name="size">
        /// On passing <paramref name="size"/> into the function this variable represents the available size of the <paramref name="buffer"/>
        /// buffer. On return this variable is updated with the amount of data actually written to the buffer, or the size
        /// required to store the calibration buffer if <paramref name="buffer"/> is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.BufferResult.Succeeded"/> if <paramref name="buffer"/> was successfully written.
        /// If <paramref name="size"/> points to a buffer size that is
        /// too small to hold the output or <paramref name="buffer"/> data is <see langword="null"/>, <see cref="NativeCallResults.BufferResult.TooSmall"/> is returned
        /// and <paramref name="size"/> is updated to contain the minimum buffer size needed to capture the calibration data.
        /// </returns>
        public abstract NativeCallResults.BufferResult DeviceGetRawCalibration(
            NativeHandles.DeviceHandle deviceHandle,
            IntPtr buffer,
            ref UIntPtr size);

        /// <summary>Get the camera calibration for the entire Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle obtained by <see cref="DeviceOpen(uint, out NativeHandles.DeviceHandle?)"/>.</param>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <param name="calibration">Output: calibration data.</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="calibration"/> was successfully written. <see cref="NativeCallResults.Result.Failed"/> otherwise.</returns>
        /// <remarks>
        /// The <paramref name="calibration"/> represents the data needed to transform between the camera views and may be
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// 
        /// The <paramref name="calibration"/> output is used as input to all calibration and transformation functions.
        /// </remarks>
        public abstract NativeCallResults.Result DeviceGetCalibration(
            NativeHandles.DeviceHandle deviceHandle,
            DepthMode depthMode,
            ColorResolution colorResolution,
            out CalibrationData calibration);

        /// <summary>Get the camera calibration for a device from a raw calibration blob.</summary>
        /// <param name="rawCalibration">Raw calibration blob obtained from a device or recording. The raw calibration must be NULL terminated.</param>
        /// <param name="rawCalibrationSize">The size, in bytes, of <paramref name="rawCalibration"/> including the NULL termination.</param>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <param name="calibration">Result: calibration data</param>
        /// <returns><see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="calibration"/> was successfully written. <see cref="NativeCallResults.Result.Failed"/> otherwise.</returns>
        /// <remarks>
        /// The <paramref name="calibration"/> represents the data needed to transform between the camera views and is
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// 
        /// The <paramref name="calibration"/> output is used as input to all transformation functions.
        /// </remarks>
        public abstract NativeCallResults.Result CalibrationGetFromRaw(
            byte[] rawCalibration,
            UIntPtr rawCalibrationSize,
            DepthMode depthMode,
            ColorResolution colorResolution,
            out CalibrationData calibration);

        /// <summary>Transform a 3D point of a source coordinate system into a 3D point of the target coordinate system.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint3DMm">The 3D coordinates in millimeters representing a point in <paramref name="sourceCamera"/>.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint3DMm">Output: the new 3D coordinates of the input point in the coordinate space <paramref name="targetCamera"/> in millimeters.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="targetPoint3DMm"/> was successfully written.
        /// <see cref="NativeCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// </returns>
        /// <remarks>
        /// This function is used to transform 3D points between depth and color camera coordinate systems. The function uses the
        /// extrinsic camera calibration. It computes the output via multiplication with a precomputed matrix encoding a 3D
        /// rotation and a 3D translation. If <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> are the same, then <paramref name="targetPoint3DMm"/> will
        /// be identical to <paramref name="sourcePoint3DMm"/>.
        /// </remarks>
        public abstract NativeCallResults.Result Calibration3DTo3D(
            in CalibrationData calibration,
            in Float3 sourcePoint3DMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float3 targetPoint3DMm);

        /// <summary>
        /// Transform a 2D pixel coordinate with an associated depth value of the source camera
        /// into a 3D point of the target coordinate system.
        /// </summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">
        /// The depth of <paramref name="sourcePoint2D"/> in millimeters.
        /// One way to derive the depth value in the color camera geometry is to
        /// use the function <see cref="TransformationDepthImageToColorCamera"/>.
        /// </param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint3DMm">Output: the 3D coordinates of the input pixel in the coordinate system of <paramref name="targetCamera"/> in millimeters.</param>
        /// <param name="valid">
        /// The output parameter returns a value of <see langword="true"/> if the <paramref name="sourcePoint2D"/> is a valid coordinate,
        /// and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="targetPoint3DMm"/> was successfully written.
        /// <see cref="NativeCallResults.Result.Failed"/> if <paramref name="calibration"/>
        /// contained invalid transformation parameters.
        /// If the function returns <see cref="NativeCallResults.Result.Succeeded"/>, but <paramref name="valid"/> valid is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint3DMm"/> are outside of the range of valid
        /// calibration and should be ignored.
        /// </returns>
        /// <remarks>
        /// This function applies the intrinsic calibration of <paramref name="sourceCamera"/> to compute the 3D ray from the focal point of the
        /// camera through pixel <paramref name="sourcePoint2D"/>.The 3D point on this ray is then found using <paramref name="sourceDepthMm"/>. If
        /// <paramref name="targetCamera"/> is different from <paramref name="sourceCamera"/>, the 3D point is transformed to <paramref name="targetCamera"/> using
        /// <see cref="Calibration3DTo3D(in CalibrationData, in Float3, CalibrationGeometry, CalibrationGeometry, out Float3)"/>.
        /// In practice, <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> will often be identical. In this
        /// case, no 3D to 3D transformation is applied.
        /// 
        /// If <paramref name="sourcePoint2D"/> is not considered as valid pixel coordinate
        /// according to the intrinsic camera model, <paramref name="valid"/> is set to <see langword="false"/>.
        /// If it is valid, <paramref name="valid"/> valid will be set to <see langword="true"/>. The user
        /// should not use the value of <paramref name="targetPoint3DMm"/> if <paramref name="valid"/> was set to <see langword="false"/>.
        /// </remarks>
        public abstract NativeCallResults.Result Calibration2DTo3D(
            in CalibrationData calibration,
            in Float2 sourcePoint2D,
            float sourceDepthMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float3 targetPoint3DMm,
            out int valid);

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
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="targetPoint2D"/> was successfully written.
        /// <see cref="NativeCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// If the function returns <see cref="NativeCallResults.Result.Succeeded"/>, but <paramref name="valid"/> is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint2D"/> are outside of the range of valid calibration
        /// and should be ignored.
        /// </returns>
        /// <remarks>
        /// If <paramref name="targetCamera"/> is different from <paramref name="sourceCamera"/>, <paramref name="sourcePoint3DMm"/> is transformed
        /// to <paramref name="targetCamera"/> using <see cref="Calibration3DTo3D(in CalibrationData, in Float3, CalibrationGeometry, CalibrationGeometry, out Float3)"/>.
        /// In practice, <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> will often be identical.
        /// In this case, no 3D to 3D transformation is applied. The 3D point in the coordinate system of <paramref name="targetCamera"/> is then
        /// projected onto the image plane using the intrinsic calibration of <paramref name="targetCamera"/>.
        /// 
        /// If <paramref name="sourcePoint3DMm"/> does not map to a valid 2D coordinate in the <paramref name="targetCamera"/> coordinate system,
        /// <paramref name="valid"/> is set to <see langword="false"/>. If it is valid, <paramref name="valid"/> will be set to <see langword="true"/>.
        /// The user should not use the value of <paramref name="targetPoint2D"/> if <paramref name="valid"/> was set to <see langword="false"/>.
        /// </remarks>
        public abstract NativeCallResults.Result Calibration3DTo2D(
            in CalibrationData calibration,
            in Float3 sourcePoint3DMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float2 targetPoint2D,
            out int valid);

        /// <summary>
        /// Transform a 2D pixel coordinate with an associated depth value of the source camera into a 2D pixel coordinate of the target camera.
        /// </summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">
        /// The depth of <paramref name="sourcePoint2D"/> in millimeters.
        /// One way to derive the depth value in the color camera geometry is to
        /// use the function <see cref="TransformationDepthImageToColorCamera"/>.
        /// </param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <param name="targetPoint2D">Output: the 2D pixel in <paramref name="targetCamera"/> coordinates.</param>
        /// <param name="valid">
        /// The output parameter returns <see langword="true"/> if the <paramref name="sourcePoint2D"/> is a valid coordinate in the <paramref name="targetCamera"/>
        /// coordinate system, and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="targetPoint2D"/> was successfully written.
        /// <see cref="NativeCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// If the function returns <see cref="NativeCallResults.Result.Succeeded"/>, but <paramref name="valid"/> valid is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint2D"/> are outside of the range of valid calibration
        /// and should be ignored.
        /// </returns>
        /// <remarks>
        /// This function maps a pixel between the coordinate systems of the depth and color cameras. It is equivalent to calling
        /// <see cref="Calibration2DTo3D(in CalibrationData, in Float2, float, CalibrationGeometry, CalibrationGeometry, out Float3, out int)"/> to compute the 3D point corresponding to <paramref name="sourcePoint2D"/> and then using
        /// <see cref="Calibration3DTo2D(in CalibrationData, in Float3, CalibrationGeometry, CalibrationGeometry, out Float2, out int)"/> to map the 3D point into the coordinate system of the <paramref name="targetCamera"/>.
        /// 
        /// If <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> are identical, the function immediately sets <paramref name="targetPoint2D"/> to
        /// <paramref name="sourcePoint2D"/> and returns without computing any transformations.
        /// 
        /// If <paramref name="sourcePoint2D"/> does not map to a valid 2D coordinate in the <paramref name="targetCamera"/> coordinate system,
        /// <paramref name="valid"/> is set to <see langword="false"/>. If it is valid, <paramref name="valid"/> will be set to <see langword="true"/>.
        /// The user should not use the value of <paramref name="targetPoint2D"/> if <paramref name="valid"/> was set to 0.
        /// </remarks>
        public abstract NativeCallResults.Result Calibration2DTo2D(
            in CalibrationData calibration,
            in Float2 sourcePoint2D,
            float sourceDepthMm,
            CalibrationGeometry sourceCamera,
            CalibrationGeometry targetCamera,
            out Float2 targetPoint2D,
            out int valid);

        /// <summary>Transform a 2D pixel coordinate from color camera into a 2D pixel coordinate of the depth camera.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <param name="sourcePoint2D">The 2D pixel in color camera coordinates.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="targetPoint2D">The 2D pixel in depth camera coordinates.</param>
        /// <param name="valid">
        /// The output parameter returns a value of <see langword="true"/> if the <paramref name="sourcePoint2D"/> is a valid coordinate in the depth camera
        /// coordinate system, and will return <see langword="false"/> if the coordinate is not valid in the calibration model.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="targetPoint2D"/> was successfully written.
        /// <see cref="NativeCallResults.Result.Failed"/> if <paramref name="calibration"/> contained invalid transformation parameters.
        /// If the function returns <see cref="NativeCallResults.Result.Succeeded"/>, but <paramref name="valid"/> valid is <see langword="false"/>,
        /// the transformation was computed, but the results in <paramref name="targetPoint2D"/> are outside of the range of valid calibration
        /// and should be ignored.
        /// </returns>
        /// <remarks>
        /// This function represents an alternative to <see cref="Calibration2DTo2D"/>
        /// if the number of pixels that need to be transformed is small. This function searches along an epipolar line in the depth image to find the corresponding
        /// depth pixel. If a larger number of pixels need to be transformed, it might be computationally cheaper to call
        /// <see cref="TransformationDepthImageToColorCamera"/>
        /// to get correspondence depth values for these color pixels, then call the function <see cref="Calibration2DTo2D"/>.
        ///
        /// If <paramref name="sourcePoint2D"/> does not map to a valid 2D coordinate in the depth camera coordinate system, <paramref name="valid"/> is set
        /// to <see langword="false"/>. If it is valid, <paramref name="valid"/> will be set to <see langword="true"/>.
        /// The user should not use the value of <paramref name="targetPoint2D"/> if <paramref name="valid"/> was set to <see langword="false"/>.
        /// </remarks>
        public abstract NativeCallResults.Result CalibrationColor2DToDepth2D(
            in CalibrationData calibration,
            in Float2 sourcePoint2D,
            NativeHandles.ImageHandle depthImage,
            out Float2 targetPoint2D,
            out int valid);

        /// <summary>Get handle to transformation.</summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <returns>A transformation handle. An invalid handle is returned if creation fails.</returns>
        /// <remarks>
        /// The transformation handle is used to transform images from the coordinate system of one camera into the other. Each
        /// transformation handle requires some pre-computed resources to be allocated, which are retained until the handle is
        /// destroyed.
        /// </remarks>
        public abstract NativeHandles.TransformationHandle? TransformationCreate(in CalibrationData calibration);

        /// <summary>Transforms the depth map into the geometry of the color camera.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="transformedDepthImage">Handle to output transformed depth image.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="transformedDepthImage"/> was successfully written
        /// and <see cref="NativeCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// This produces a depth image for which each pixel matches the corresponding pixel coordinates of the color camera.
        /// 
        /// <paramref name="depthImage"/> and <paramref name="transformedDepthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// 
        /// <paramref name="transformedDepthImage"/> must have a width and height matching the width and height of the color camera in the mode
        /// specified by the <see cref="CalibrationData"/> used to create the <paramref name="transformationHandle"/> with <see cref="TransformationCreate(in CalibrationData)"/>.
        /// 
        /// The contents <paramref name="transformedDepthImage"/> will be filled with the depth values derived from <paramref name="depthImage"/> in the color
        /// camera's coordinate space.
        /// 
        /// <paramref name="transformedDepthImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/>.
        /// </remarks>
        public abstract NativeCallResults.Result TransformationDepthImageToColorCamera(
            NativeHandles.TransformationHandle transformationHandle,
            NativeHandles.ImageHandle depthImage,
            NativeHandles.ImageHandle transformedDepthImage);

        /// <summary>Transforms depth map and a custom image into the geometry of the color camera.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="customImage">Handle to input custom image.</param>
        /// <param name="transformedDepthImage">Handle to output transformed depth image.</param>
        /// <param name="transformedCustomImage">Handle to output transformed custom image.</param>
        /// <param name="interpolation">
        /// Parameter that controls how pixels in <paramref name="customImage"/> should be interpolated when transformed to color camera space.
        /// <see cref="TransformationInterpolation.Linear"/> if linear interpolation should be used.
        /// <see cref="TransformationInterpolation.Nearest"/> if nearest neighbor interpolation should be used.
        /// </param>
        /// <param name="invalidCustomValue">
        /// Defines the custom image pixel value that should be written to <paramref name="transformedCustomImage"/> in case the corresponding
        /// depth pixel can not be transformed into the color camera space.
        /// </param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="transformedDepthImage"/> and <paramref name="transformedCustomImage"/> were successfully written and
        /// <see cref="NativeCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// This produces a depth image and a corresponding custom image for which each pixel matches the corresponding
        /// pixel coordinates of the color camera.
        /// 
        /// <paramref name="depthImage"/> and <paramref name="transformedDepthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// 
        /// <paramref name="customImage"/> and <paramref name="transformedCustomImage"/> must be of format <see cref="ImageFormat.Custom8"/> or
        /// <see cref="ImageFormat.Custom16"/>.
        /// 
        /// <paramref name="transformedDepthImage"/> and <paramref name="transformedCustomImage"/> must have a width and height matching the width and
        /// height of the color camera in the mode specified by the <see cref="CalibrationData"/> used to create the
        /// <paramref name="transformationHandle"/> with <see cref="TransformationCreate(in CalibrationData)"/>.
        /// 
        /// <paramref name="customImage"/> must have a width and height matching the width and height of <paramref name="depthImage"/>.
        /// 
        /// The contents <paramref name="transformedDepthImage"/> will be filled with the depth values derived from <paramref name="depthImage"/> in the color
        /// camera's coordinate space.
        /// 
        /// The contents <paramref name="transformedCustomImage"/> will be filled with the values derived from <paramref name="customImage"/> in the color
        /// camera's coordinate space.
        /// 
        /// Using linear interpolation could create new values to <paramref name="transformedCustomImage"/> which do no exist in <paramref name="customImage"/>.
        /// Setting <paramref name="interpolation"/> to <see cref="TransformationInterpolation.Nearest"/> will prevent this from happening but will result in less
        /// smooth image.
        /// </remarks>
        public abstract NativeCallResults.Result TransformationDepthImageToColorCameraCustom(
            NativeHandles.TransformationHandle transformationHandle,
            NativeHandles.ImageHandle depthImage,
            NativeHandles.ImageHandle customImage,
            NativeHandles.ImageHandle transformedDepthImage,
            NativeHandles.ImageHandle transformedCustomImage,
            TransformationInterpolation interpolation,
            int invalidCustomValue);

        /// <summary>Transforms a color image into the geometry of the depth camera.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="colorImage">Handle to input color image.</param>
        /// <param name="transformedColorImage">Handle to output transformed color image.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="transformedColorImage"/> was successfully written
        /// and <see cref="NativeCallResults.Result.Failed"/> otherwise.
        /// </returns>
        /// <remarks>
        /// This produces a color image for which each pixel matches the corresponding pixel coordinates of the depth camera.
        /// 
        /// <paramref name="depthImage"/> and <paramref name="colorImage"/> need to represent the same moment in time. The depth data will be applied to the
        /// color image to properly warp the color data to the perspective of the depth camera.
        /// 
        /// <paramref name="depthImage"/> must be of type <see cref="ImageFormat.Depth16"/>. <paramref name="colorImage"/> must be of format
        /// <see cref="ImageFormat.ColorBgra32"/>.
        /// 
        /// <paramref name="transformedColorImage"/> image must be of format <see cref="ImageFormat.ColorBgra32"/>. <paramref name="transformedColorImage"/> must
        /// have the width and height of the depth camera in the mode specified by the <see cref="CalibrationData"/> used to create
        /// the <paramref name="transformationHandle"/> with <see cref="TransformationCreate(in CalibrationData)"/>.
        /// 
        /// <paramref name="transformedColorImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/>.
        /// </remarks>
        public abstract NativeCallResults.Result TransformationColorImageToDepthCamera(
            NativeHandles.TransformationHandle transformationHandle,
            NativeHandles.ImageHandle depthImage,
            NativeHandles.ImageHandle colorImage,
            NativeHandles.ImageHandle transformedColorImage);

        /// <summary>Transforms the depth image into 3 planar images representing X, Y and Z-coordinates of corresponding 3D points.</summary>
        /// <param name="transformationHandle">Transformation handle.</param>
        /// <param name="depthImage">Handle to input depth image.</param>
        /// <param name="camera">Geometry in which depth map was computed.</param>
        /// <param name="xyzImage">Handle to output xyz image.</param>
        /// <returns>
        /// <see cref="NativeCallResults.Result.Succeeded"/> if <paramref name="xyzImage"/> was successfully written
        /// and <see cref="NativeCallResults.Result.Failed"/> otherwise.
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
        /// <paramref name="xyzImage"/> should be created by the caller using <see cref="ImageCreate(ImageFormat, int, int, int, out NativeHandles.ImageHandle?)"/>
        /// or <see cref="ImageCreateFromBuffer(ImageFormat, int, int, int, IntPtr, UIntPtr, MemoryDestroyCallback, IntPtr, out NativeHandles.ImageHandle?)"/>.
        /// </remarks>
        public abstract NativeCallResults.Result TransformationDepthImageToPointCloud(
                NativeHandles.TransformationHandle transformationHandle,
                NativeHandles.ImageHandle depthImage,
                CalibrationGeometry camera,
                NativeHandles.ImageHandle xyzImage);
    }
}
