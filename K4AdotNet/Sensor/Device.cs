using System;
using System.Diagnostics.CodeAnalysis;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>device</c> class from <c>k4a.hpp</c>
    //
    /// <summary>Azure Kinect or Orbbec Femto device itself. The main class in Sensor part of API.</summary>
    /// <remarks><para>
    /// To open device use <see cref="TryOpen(out Device, int)"/> or <see cref="Open(int)"/> method,
    /// to start cameras streaming call <see cref="StartCameras(DeviceConfiguration)"/>,
    /// to stop - call <see cref="StopCameras"/>.
    /// <see cref="TryGetCapture(out Capture, Timeout)"/> and <see cref="GetCapture"/> methods
    /// are used to read next capture (frame with data) from device.
    /// Don't forget to close device and release all unmanaged resources by calling <see cref="Dispose"/> method.
    /// </para></remarks>
    /// <seealso cref="Capture"/>
    /// <seealso cref="ImuSample"/>
    public abstract partial class Device : SdkObject, IDisposablePlus
    {
        private readonly NativeApi api;
        private readonly NativeHandles.HandleWrapper<NativeHandles.DeviceHandle> handle;    // This class is an wrapper around this native handle

        internal Device(NativeHandles.DeviceHandle handle, int deviceIndex, string serialNumber, HardwareVersion version)
            : base(handle.IsOrbbec)
        {
            this.api = NativeApi.GetInstance(IsOrbbec);
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
            DeviceIndex = deviceIndex;
            SerialNumber = serialNumber;
            Version = version;
        }

        private void Handle_Disposed(object? sender, EventArgs e)
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

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Zero-based index of this device.</summary>
        /// <seealso cref="Open(int)"/>
        /// <seealso cref="TryOpen(out Device, int)"/>
        public int DeviceIndex { get; }

        /// <summary>Device serial number. Not <see langword="null"/>.</summary>
        public string SerialNumber { get; }

        /// <summary>Version numbers of the device's subsystems.</summary>
        public HardwareVersion Version { get; }

        /// <summary>Is this device still connected?</summary>
        /// <seealso cref="DeviceConnectionLostException"/>
        public abstract bool IsConnected { get; }

        /// <summary>Starts color and depth camera capture.</summary>
        /// <param name="config">The configuration we want to run the device in. This can be initialized with <see cref="DeviceConfiguration.DisableAll"/>.</param>
        /// <remarks><para>
        /// Individual sensors configured to run will now start to stream captured data.
        /// </para><para>
        /// It is not valid to call this method a second time on the same device until <see cref="StopCameras"/> has been called.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="ArgumentException">Configuration in <paramref name="config"/> is not valid. For details see <see cref="DeviceConfiguration.IsValid(bool, out string)"/>.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cameras streaming is already running, or something wrong with <paramref name="config"/>.</exception>
        /// <seealso cref="StopCameras"/>
        public void StartCameras(DeviceConfiguration config)
        {
            if (!config.IsValid(IsOrbbec, out var message))
                throw new ArgumentException(message, nameof(config));
            CheckDeviceConfiguration(config);
            CheckResult(
                api.DeviceStartCameras(handle.ValueNotDisposed, in config),
                $"Cameras streaming is already running, or invalid configuration specified.");
        }

        private protected virtual void CheckDeviceConfiguration(DeviceConfiguration config)
        { }

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
            => api.DeviceStopCameras(handle.ValueNotDisposed);

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
                api.DeviceStartImu(handle.ValueNotDisposed),
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
            => api.DeviceStopImu(handle.ValueNotDisposed);

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
        public bool TryGetCapture([NotNullWhen(returnValue: true)] out Capture? capture, Timeout timeout = default)
        {
            var res = api.DeviceGetCapture(handle.ValueNotDisposed, out var captureHandle, timeout);

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
            return capture!;
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
        public bool TryGetImuSample(out ImuSample imuSample, Timeout timeout = default)
        {
            var res = api.DeviceGetImuSample(handle.ValueNotDisposed, out imuSample, timeout);

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
            => api.DeviceSetColorControl(handle.ValueNotDisposed, command, mode, value) == NativeCallResults.Result.Succeeded;

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
                api.DeviceGetColorControl(handle.ValueNotDisposed, command, out mode, out value),
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
        {
            byte supportsAutoFlag;
            CheckResult(
                api.DeviceGetColorControlCapabilities(handle.ValueNotDisposed, command,
                                                        out supportsAutoFlag, out minValue, out maxValue, out valueStep,
                                                        out defaultValue, out defaultMode),
                "Not supported command: " + command);
            supportsAuto = supportsAutoFlag != 0;
        }

        /// <summary>Gets the raw calibration blob for the entire Azure Kinect device.</summary>
        /// <returns>Raw calibration data terminated by <c>0</c> byte. Not <see langword="null"/>.</returns>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cannot read calibration data for some unexpected reason. See logs for details.</exception>
        /// <seealso cref="GetCalibration(DepthMode, ColorResolution)"/>
        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(api.DeviceGetRawCalibration, handle.ValueNotDisposed, out var result))
                ThrowException();
            return result;
        }

        /// <summary>Gets the camera calibration for the entire Azure Kinect device.</summary>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <returns>Calibration data.</returns>
        /// <remarks><para>
        /// The return value represents the data needed to transform between the camera views and may be
        /// different for each operating <paramref name="depthMode"/> and <paramref name="colorResolution"/> the device is configured to operate in.
        /// </para><para>
        /// The return value is used as input to all calibration and transformation functions.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="DeviceConnectionLostException">Connection with Azure Kinect device has been lost.</exception>
        /// <exception cref="InvalidOperationException">Cannot read calibration data for some unexpected reason. See logs for details.</exception>
        /// <seealso cref="GetRawCalibration()"/>
        public abstract Calibration GetCalibration(DepthMode depthMode, ColorResolution colorResolution);

        private void CheckResult(NativeCallResults.Result result, string? invalidOperationMessage = null)
        {
            if (result != NativeCallResults.Result.Succeeded)
                ThrowException(invalidOperationMessage);
        }

        [DoesNotReturn]
        private void ThrowException(string? invalidOperationMessage = null)
        {
            handle.CheckNotDisposed();
            if (!IsConnected)
                throw new DeviceConnectionLostException(DeviceIndex);
            throw new InvalidOperationException(invalidOperationMessage ?? "Unspecified error in Sensor SDK. See logs for details.");
        }

        /// <summary>Gets the number of connected devices.</summary>
        /// <remarks><para>
        /// Some devices can be occupied by other processes by they are counted here as connected.
        /// </para><para>
        /// In <see cref="ComboMode.Both"/> mode <see cref="InstalledCount"/> is equal to the sum of
        /// <see cref="Azure.InstalledCount"/> and <see cref="Orbbec.InstalledCount"/>.
        /// </para></remarks>
        public static int InstalledCount
        {
            get
            {
                var count = 0;
                if ((Sdk.ComboMode & ComboMode.Azure) == ComboMode.Azure)
                    count += Azure.InstalledCount;
                if ((Sdk.ComboMode & ComboMode.Orbbec) == ComboMode.Orbbec)
                    count += Orbbec.InstalledCount;
                return count;
            }
        }

        /// <summary>
        /// Index for default device (the first connected device).
        /// Use it when you're working with single device solutions.
        /// </summary>
        public const int DefaultDeviceIndex = 0;

        /// <summary>Tries to open an Azure Kinect or Orbbec Femto device.</summary>
        /// <param name="device">Opened device on success, or <see langword="null"/> in case of failure.</param>
        /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
        /// <returns>
        /// <see langword="true"/> if device has been successfully opened,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        /// <remarks>
        /// In <see cref="ComboMode.Both"/> mode:
        /// If <paramref name="index"/> is less than <see cref="Azure.InstalledCount"/>
        /// then Azure Kinect device will be opened with <paramref name="index"/> index via <see cref="Azure.TryOpen(out Azure?, int)"/>.
        /// Otherwise Orbbec Femto device will be opened with <paramref name="index"/> minus <see cref="Azure.InstalledCount"/> index
        /// via <see cref="Orbbec.TryOpen(out Orbbec?, int)"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <seealso cref="Open(int)"/>
        /// <seealso cref="InstalledCount"/>
        public static bool TryOpen([NotNullWhen(returnValue: true)] out Device? device, int index = DefaultDeviceIndex)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if ((Sdk.ComboMode & ComboMode.Azure) == ComboMode.Azure)
            {
                var azureCount = Azure.InstalledCount;
                if (index < azureCount)
                {
                    var azureRes = Azure.TryOpen(out var azureDevice, index);
                    device = azureDevice;
                    return azureRes;
                }
                index -= azureCount;
            }

            if ((Sdk.ComboMode & ComboMode.Orbbec) == ComboMode.Orbbec)
            {
                if (index >= 0 && index < Orbbec.InstalledCount)
                {
                    var orrbecRes = Orbbec.TryOpen(out var orbbecDevice, index);
                    device = orbbecDevice;
                    return orrbecRes;
                }
            }

            device = null;
            return false;
        }

        /// <summary>Opens an Azure Kinect or Orbbec Femto device. Like <see cref="TryOpen(out Device, int)"/> but raises exception in case of failure.</summary>
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

        [return:NotNullIfNotNull(nameof(device))]
        internal static NativeHandles.DeviceHandle? ToHandle(Device? device)
            => device?.handle?.ValueNotDisposed;
    }
}
