using System;
using System.Text;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>device</c> class from <c>k4a.hpp</c>
    //
    /// <summary>Azure Kinect device. The main class in Sensor part of API.</summary>
    /// <remarks><para>
    /// To open device use <see cref="TryOpen(out Device, int)"/> or <see cref="Open(int)"/> method,
    /// to start cameras streaming call <see cref="StartCameras(DeviceConfiguration)"/>,
    /// to stop - call <see cref="StopCameras"/>.
    /// <see cref="TryGetCapture(out Capture, Timeout)"/> and <see cref="GetCapture"/> methods
    /// are used to read next capture (frame with data) from device.
    /// Don't forget to close device and release all unmanaged resources by calling <see cref="Dispose"/> method.
    /// </para><para>
    /// This class is designed to be thread-safe.
    /// </para></remarks>
    /// <seealso cref="Capture"/>
    /// <seealso cref="ImuSample"/>
    /// <threadsafety static="true" instance="true"/>
    public sealed class Device : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.DeviceHandle> handle;    // This class is an wrapper around this native handle

        private Device(NativeHandles.DeviceHandle handle, int deviceIndex, string serialNumber, HardwareVersion version)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
            DeviceIndex = deviceIndex;
            SerialNumber = serialNumber;
            Version = version;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to close device and free all unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the image has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler Disposed;

        /// <summary>Zero-based index of this device.</summary>
        /// <seealso cref="Open(int)"/>
        /// <seealso cref="TryOpen(out Device, int)"/>
        public int DeviceIndex { get; }

        /// <summary>Azure Kinect device serial number. Not <see langword="null"/>.</summary>
        public string SerialNumber { get; }

        /// <summary>Version numbers of the device's subsystems.</summary>
        public HardwareVersion Version { get; }

        /// <summary>Is this device still connected?</summary>
        /// <seealso cref="DeviceConnectionLostException"/>
        public bool IsConnected
            => !handle.IsDisposed
                && NativeApi.DeviceGetSyncJack(handle.Value, out var notUsed1, out var notUsed2) == NativeCallResults.Result.Succeeded;

        /// <summary>Gets the device jack status for the synchronization in connectors.</summary>
        /// <remarks>
        /// If <see cref="IsSyncInConnected"/> is <see langword="true"/> then
        /// <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Subordinate"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed objects.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Some unspecified error in Sensor SDK. See logs for details.</exception>
        public bool IsSyncInConnected
        {
            get
            {
                CheckResult(NativeApi.DeviceGetSyncJack(handle.ValueNotDisposed, out var syncInConnected, out var syncOutConnected));
                return syncInConnected;
            }
        }

        /// <summary>Gets the device jack status for the synchronization out connectors.</summary>
        /// <remarks>
        /// If <see cref="IsSyncOutConnected"/> is <see langword="true"/> then
        /// <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to <see cref="WiredSyncMode.Standalone"/> or <see cref="WiredSyncMode.Master"/>.
        /// If <see cref="IsSyncInConnected"/> is also <see langword="true"/> then
        /// <see cref="DeviceConfiguration.WiredSyncMode"/> mode can be set to <see cref="WiredSyncMode.Subordinate"/> (in this case 'Sync Out' is driven for the
        /// next device in the chain).
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed objects.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Some unspecified error in Sensor SDK. See logs for details.</exception>
        public bool IsSyncOutConnected
        {
            get
            {
                CheckResult(NativeApi.DeviceGetSyncJack(handle.ValueNotDisposed, out var syncInConnected, out var syncOutConnected));
                return syncOutConnected;
            }
        }

        /// <summary>Starts color and depth camera capture.</summary>
        /// <param name="config">The configuration we want to run the device in. This can be initialized with <see cref="DeviceConfiguration.DisableAll"/>.</param>
        /// <remarks><para>
        /// Individual sensors configured to run will now start to stream captured data.
        /// </para><para>
        /// It is not valid to call this method a second time on the same device until <see cref="StopCameras"/> has been called.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="ArgumentException">Configuration in <paramref name="config"/> is not valid. For details see <see cref="DeviceConfiguration.IsValid(out string)"/>.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cameras streaming is already running, or something wrong with <paramref name="config"/>, or <see cref="Sdk.DEPTHENGINE_DLL_NAME"/> library cannot be found.</exception>
        /// <seealso cref="StopCameras"/>
        public void StartCameras(DeviceConfiguration config)
        {
            if (!config.IsValid(out var message))
                throw new ArgumentException(message, nameof(config));

            CheckResult(
                NativeApi.DeviceStartCameras(handle.ValueNotDisposed, ref config),
                $"Cameras streaming is already running, or invalid configuration specified, or {Sdk.DEPTHENGINE_DLL_NAME} library cannot be found.");
        }

        /// <summary>Stops the color and depth camera capture.</summary>
        /// <remarks><para>
        /// The streaming of individual sensors stops as a result of this call. Once called, <see cref="StartCameras(DeviceConfiguration)"/>
        /// may be called again to resume sensor streaming.
        /// </para><para>
        /// This function may be called while another thread is blocking in <see cref="GetCapture"/> or <see cref="TryGetCapture(out Capture, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function failing with exception.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="StartCameras(DeviceConfiguration)"/>
        public void StopCameras()
            => NativeApi.DeviceStopCameras(handle.ValueNotDisposed);

        /// <summary>Starts the IMU sample stream.</summary>
        /// <remarks><para>
        /// Call this API to start streaming IMU data. It is not valid to call this function a second time on the same
        /// device until <see cref="StopImu"/> has been called.
        /// </para><para>
        /// This function is dependent on the state of the cameras. The color or depth camera must be started before the IMU.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with device has been lost.</exception>
        /// <exception cref="InvalidOperationException">IMU streaming is already running or cameras streaming is not running.</exception>
        /// <seealso cref="StopImu"/>
        public void StartImu()
            => CheckResult(
                NativeApi.DeviceStartImu(handle.ValueNotDisposed),
                "IMU streaming is already running or cameras streaming is not running.");

        /// <summary>Stops the IMU capture.</summary>
        /// <remarks><para>
        /// The streaming of the IMU stops as a result of this call. Once called, <see cref="StartImu"/> may
        /// be called again to resume sensor streaming, so long as the cameras are running.
        /// </para><para>
        /// This function may be called while another thread is blocking in <see cref="GetImuSample"/> or <see cref="TryGetImuSample(out ImuSample, Timeout)"/>.
        /// Calling this function while another thread is in that function will result in that function failing with exception.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="StartImu"/>
        public void StopImu()
            => NativeApi.DeviceStopImu(handle.ValueNotDisposed);

        /// <summary>Reads a sensor capture.</summary>
        /// <param name="capture">
        /// If successful this contains object with capture data read from device (don't forget to free this object by calling <see cref="Capture.Dispose"/>),
        /// otherwise - <see langword="null"/>.
        /// </param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the capture.
        /// Default value is <see cref="Timeout.NoWait"/>, which means that the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see langword="true"/> - if a capture is returned,
        /// <see langword="false"/> - if a capture is not available before the timeout elapses.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="StartCameras(DeviceConfiguration)"/> is called and before <see cref="StopCameras"/> is called.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Camera streaming is not running or has been stopped during this call.</exception>
        public bool TryGetCapture(out Capture capture, Timeout timeout = default(Timeout))
        {
            var res = NativeApi.DeviceGetCapture(handle.ValueNotDisposed, out var captureHandle, timeout);

            if (res == NativeCallResults.WaitResult.Succeeded)
            {
                capture = Capture.Create(captureHandle);
                return capture != null;
            }

            if (res == NativeCallResults.WaitResult.Timeout)
            {
                capture = null;
                return false;
            }

            ThrowException("Cameras streaming is not running or has been stopped.");
            capture = null;     // Actually, this code is unreachable
            return false;
        }

        /// <summary>Equivalent to call of <see cref="TryGetCapture(out Capture, Timeout)"/> with infinite timeout: <see cref="Timeout.Infinite"/>.</summary>
        /// <returns>Capture object read from device. Not <see langword="null"/>. Don't forget call <see cref="Capture.Dispose"/> for returned object after usage.</returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="StartCameras(DeviceConfiguration)"/> is called and before <see cref="StopCameras"/> is called.
        /// </remarks>
        /// <seealso cref="TryGetCapture(out Capture, Timeout)"/>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Camera streaming is not running or has been stopped during this call.</exception>
        public Capture GetCapture()
        {
            var res = TryGetCapture(out var capture, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
            return capture;
        }

        /// <summary>Reads an IMU sample.</summary>
        /// <param name="imuSample">Information about IMU sample if method returned <see langword="true"/>.</param>
        /// <param name="timeout">
        /// Specifies the time the function should block waiting for the capture.
        /// Default value is <see cref="Timeout.NoWait"/>, which means that the function will return without blocking.
        /// Passing <see cref="Timeout.Infinite"/> will block indefinitely until data is available, the
        /// device is disconnected, or another error occurs.
        /// </param>
        /// <returns>
        /// <see langword="true"/> - if a sample is returned,
        /// <see langword="false"/> - if a sample is not available before the timeout elapses.
        /// </returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="StartImu"/> is called and before <see cref="StopImu"/> is called.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">IMU streaming is not running or has been stopped during this call.</exception>
        public bool TryGetImuSample(out ImuSample imuSample, Timeout timeout = default(Timeout))
        {
            var res = NativeApi.DeviceGetImuSample(handle.ValueNotDisposed, out imuSample, timeout);

            if (res == NativeCallResults.WaitResult.Succeeded)
                return true;

            if (res == NativeCallResults.WaitResult.Timeout)
                return false;

            ThrowException("IMU streaming is not running or has been stopped.");
            return false;   // Actually, this code is unreachable
        }

        /// <summary>Equivalent to call of <see cref="TryGetImuSample(out ImuSample, Timeout)"/> with infinite timeout: <see cref="Timeout.Infinite"/>.</summary>
        /// <returns>Information about IMU sample.</returns>
        /// <remarks>
        /// This function needs to be called while the device is in a running state;
        /// after <see cref="StartImu"/> is called and before <see cref="StopImu"/> is called.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">IMU streaming is not running or has been stopped during this call.</exception>
        /// <seealso cref="TryGetImuSample(out ImuSample, Timeout)"/>
        public ImuSample GetImuSample()
        {
            var res = TryGetImuSample(out var imuSample, Timeout.Infinite);
            System.Diagnostics.Debug.Assert(res);
            return imuSample;
        }

        /// <summary>Set the Azure Kinect color sensor control value.</summary>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">Color sensor control mode to set. This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">
        /// Value to set the color sensor's control to. The value is only valid if <paramref name="mode"/>
        /// is set to <see cref="ColorControlMode.Manual"/>, and is otherwise ignored.
        /// </param>
        /// <returns><see langword="true"/> if the value was successfully set, <see langword="false"/> -  otherwise.</returns>
        /// <remarks><para>
        /// Each control command may be set to manual or automatic. See the definition of <see cref="ColorControlCommand"/> on how
        /// to interpret the <paramref name="value"/> for each command.
        /// </para><para>
        /// Some control commands are only supported in manual mode. When a command is in automatic mode, the <paramref name="value"/> for that
        /// command is not valid.
        /// </para><para>
        /// Control values set on a device are reset only when the device is power cycled. The device will retain the settings
        /// even if the device is disposed or the application is restarted.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public bool TrySetColorControl(ColorControlCommand command, ColorControlMode mode, int value)
            => NativeApi.DeviceSetColorControl(handle.ValueNotDisposed, command, mode, value) == NativeCallResults.Result.Succeeded;

        /// <summary>Gets the Azure Kinect color sensor control value.</summary>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="mode">This mode represents whether the command is in automatic or manual mode.</param>
        /// <param name="value">This value is always written, but is only valid when the <paramref name="mode"/> returned is <see cref="ColorControlMode.Manual"/> for the current <paramref name="command"/>.</param>
        /// <remarks><para>
        /// Each control command may be set to manual or automatic. See the definition of <see cref="ColorControlCommand"/> on
        /// how to interpret the <paramref name="value"/> for each command.
        /// </para><para>
        /// Some control commands are only supported in manual mode. When a command is in automatic mode, the <paramref name="value"/> for
        /// that command is not valid.
        /// </para><para>
        /// Control values set on a device are reset only when the device is power cycled. The device will retain the
        /// settings even if the device is disposed or the application is restarted.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Not supported <paramref name="command"/>.</exception>
        public void GetColorControl(ColorControlCommand command, out ColorControlMode mode, out int value)
            => CheckResult(
                NativeApi.DeviceGetColorControl(handle.ValueNotDisposed, command, out mode, out value),
                "Not supported command: " + command);

        /// <summary>Gets the Azure Kinect color sensor control capabilities.</summary>
        /// <param name="command">Color sensor control command.</param>
        /// <param name="supportsAuto">Output: whether the color sensor's control support auto mode or not. <see langword="true"/> if it supports auto mode, otherwise <see langword="false"/>.</param>
        /// <param name="minValue">Output: the color sensor's control minimum value of <paramref name="command"/>.</param>
        /// <param name="maxValue">Output: the color sensor's control maximum value of <paramref name="command"/>.</param>
        /// <param name="valueStep">Output: the color sensor's control step value of <paramref name="command"/>.</param>
        /// <param name="defaultValue">Output: the color sensor's control default value of <paramref name="command"/>.</param>
        /// <param name="defaultMode">Output: the color sensor's control default mode of <paramref name="command"/>.</param>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Not supported <paramref name="command"/>.</exception>
        public void GetColorControlCapabilities(ColorControlCommand command,
                                                out bool supportsAuto, out int minValue, out int maxValue, out int valueStep,
                                                out int defaultValue, out ColorControlMode defaultMode)
            => CheckResult(
                NativeApi.DeviceGetColorControlCapabilities(handle.ValueNotDisposed, command,
                                                            out supportsAuto, out minValue, out maxValue, out valueStep,
                                                            out defaultValue, out defaultMode),
                "Not supported command: " + command);

        /// <summary>Gets the raw calibration blob for the entire Azure Kinect device.</summary>
        /// <returns>Raw calibration data terminated by <c>0</c> value. Not <see langword="null"/>.</returns>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cannot read calibration data for some unexpected reason. See logs for details.</exception>
        /// <seealso cref="GetCalibration(DepthMode, ColorResolution, out Calibration)"/>
        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.DeviceGetRawCalibration, handle.ValueNotDisposed, out var result))
                ThrowException();
            return result;
        }

        /// <summary>Gets the camera calibration for the entire Azure Kinect device.</summary>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <param name="calibration">Output: calibration data.</param>
        /// <remarks><para>
        /// The <paramref name="calibration"/> represents the data needed to transform between the camera views and may be
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// </para><para>
        /// The <paramref name="calibration"/> output is used as input to all calibration and transformation functions.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cannot read calibration data for some unexpected reason. See logs for details.</exception>
        /// <seealso cref="GetCalibration(DepthMode, ColorResolution, out Calibration)"/>
        public void GetCalibration(DepthMode depthMode, ColorResolution colorResolution, out Calibration calibration)
        {
            if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                throw new ArgumentOutOfRangeException(nameof(depthMode) + " and " + nameof(colorResolution), $"{nameof(depthMode)} and {nameof(colorResolution)} cannot be equal to Off simultaneously.");

            CheckResult(NativeApi.DeviceGetCalibration(handle.ValueNotDisposed, depthMode, colorResolution, out calibration));
        }

        /// <summary>Convenient string representation of object.</summary>
        /// <returns><c>Azure Kinect #{SerialNumber}</c></returns>
        public override string ToString()
            => "Azure Kinect #" + SerialNumber;

        private void CheckResult(NativeCallResults.Result result, string invalidOperationMessage = null)
        {
            if (result != NativeCallResults.Result.Succeeded)
                ThrowException(invalidOperationMessage);
        }

        private void ThrowException(string invalidOperationMessage = null)
        {
            handle.CheckNotDisposed();
            if (!IsConnected)
                throw new DeviceConnectionLostException(DeviceIndex);
            throw new InvalidOperationException(invalidOperationMessage ?? "Unspecified error in Sensor SDK. See logs for details.");
        }

        /// <summary>Gets the number of connected devices.</summary>
        /// <remarks>Some devices can be occupied by other processes by they are counted here as connected.</remarks>
        public static int InstalledCount => checked((int)NativeApi.DeviceGetInstalledCount());

        /// <summary>Index for default device (the first connected device). Use it when you're working with single device solutions.</summary>
        public const int DefaultDeviceIndex = 0;

        /// <summary>Tries to open an Azure Kinect device.</summary>
        /// <param name="device">Opened device on success, or <see langword="null"/> in case of failure.</param>
        /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
        /// <returns>
        /// <see langword="true"/> if device has been successfully opened,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <seealso cref="Open(int)"/>
        /// <seealso cref="InstalledCount"/>
        public static bool TryOpen(out Device device, int index = DefaultDeviceIndex)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var res = NativeApi.DeviceOpen(checked((uint)index), out var deviceHandle);
            if (res != NativeCallResults.Result.Succeeded || deviceHandle == null || deviceHandle.IsInvalid)
            {
                device = null;
                return false;
            }

            if (!TryGetSerialNumber(deviceHandle, out var serialNumber)
                || !TryGetHardwareVersion(deviceHandle, out var version))
            {
                deviceHandle.Dispose();
                device = null;
                return false;
            }

            device = new Device(deviceHandle, index, serialNumber, version);
            return true;
        }

        /// <summary>Opens an Azure Kinect device. Like <see cref="TryOpen(out Device, int)"/> but raises exception in case of failure.</summary>
        /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
        /// <returns>Opened device. Not <see langword="null"/>. Don't forget to call <see cref="Dispose"/> method for this object after usage.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="DeviceNotFoundException">Device with <paramref name="index"/> index is not found. Most likely it is not connected or hasn't power supply.</exception>
        /// <exception cref="DeviceOccupiedException">This Kinect device is occupied by another process, or it is already opened in current process, or it has not enough power supply.</exception>
        /// <seealso cref="TryOpen(out Device, int)"/>
        /// <seealso cref="InstalledCount"/>
        public static Device Open(int index = DefaultDeviceIndex)
        {
            if (!TryOpen(out var device, index))
            {
                if (index >= InstalledCount)
                {
                    throw new DeviceNotFoundException(index);
                }

                throw new DeviceOccupiedException(index);
            }

            return device;
        }

        private static bool TryGetSerialNumber(NativeHandles.DeviceHandle deviceHandle, out string serialNumber)
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.DeviceGetSerialnum, deviceHandle, out var result))
            {
                serialNumber = null;
                return false;
            }

            serialNumber = result.Length > 1
                ? Encoding.ASCII.GetString(result, 0, result.Length - 1)
                : string.Empty;

            return true;
        }

        private static bool TryGetHardwareVersion(NativeHandles.DeviceHandle deviceHandle, out HardwareVersion version)
            => NativeApi.DeviceGetVersion(deviceHandle, out version) == NativeCallResults.Result.Succeeded;

        internal static NativeHandles.DeviceHandle ToHandle(Device device)
            => device?.handle.ValueNotDisposed ?? NativeHandles.DeviceHandle.Zero;
    }
}
