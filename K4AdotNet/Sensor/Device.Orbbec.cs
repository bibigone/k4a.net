using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace K4AdotNet.Sensor
{
    partial class Device
    {
        /// <summary>
        /// Implementation of base <see cref="Device"/> class for Orbbec Femto devices.
        /// This class works via `Orbbec SDK K4A Wrapper` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Orbbec"/> and <see cref="ComboMode.Both"/>.</remarks>
        public class Orbbec : Device
        {
            internal Orbbec(NativeHandles.DeviceHandle handle, int deviceIndex, string serialNumber, HardwareVersion version)
                : base(handle, deviceIndex, serialNumber, version)
            {
                Debug.Assert(handle is NativeHandles.DeviceHandle.Orbbec);
            }

            /// <summary>Gets the number of connected Orbbec Femto devices.</summary>
            /// <remarks>Some devices can be occupied by other processes by they are counted here as connected.</remarks>
            public static new int InstalledCount => checked((int)NativeApi.Orbbec.Instance.DeviceGetInstalledCount());

            /// <summary>Tries to open an Orbbec Femto device.</summary>
            /// <param name="device">Opened device on success, or <see langword="null"/> in case of failure.</param>
            /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
            /// <returns>
            /// <see langword="true"/> if device has been successfully opened,
            /// <see langword="false"/> - otherwise.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
            /// <seealso cref="Open(int)"/>
            /// <seealso cref="InstalledCount"/>
            public static bool TryOpen([NotNullWhen(returnValue: true)] out Orbbec? device, int index = DefaultDeviceIndex)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var res = NativeApi.Orbbec.Instance.DeviceOpen(checked((uint)index), out var deviceHandle);
                if (res != NativeCallResults.Result.Succeeded || deviceHandle is null || deviceHandle.IsInvalid)
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

                device = new Orbbec(deviceHandle, index, serialNumber, version);
                return true;
            }

            private static bool TryGetSerialNumber(NativeHandles.DeviceHandle deviceHandle, [NotNullWhen(returnValue: true)] out string? serialNumber)
            {
                if (!Helpers.TryGetValueInByteBuffer(NativeApi.Orbbec.Instance.DeviceGetSerialnum, deviceHandle, out var result))
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
                => NativeApi.Orbbec.Instance.DeviceGetVersion(deviceHandle, out version) == NativeCallResults.Result.Succeeded;

            /// <summary>Opens an Orbbec Femto device. Like <see cref="TryOpen(out Orbbec, int)"/> but raises exception in case of failure.</summary>
            /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
            /// <returns>Opened device. Not <see langword="null"/>. Don't forget to call <see cref="Dispose"/> method for this object after usage.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
            /// <exception cref="DeviceNotFoundException">Device with <paramref name="index"/> index is not found. Most likely it is not connected or hasn't power supply.</exception>
            /// <exception cref="DeviceOccupiedException">This Kinect device is occupied by another process, or it is already opened in current process, or it has not enough power supply.</exception>
            /// <seealso cref="TryOpen(out Orbbec, int)"/>
            /// <seealso cref="InstalledCount"/>
            public static new Orbbec Open(int index = DefaultDeviceIndex)
            {
                if (!TryOpen(out var device, index))
                {
                    if (index >= InstalledCount)
                    {
                        throw new DeviceNotFoundException(index, isOrbbec: true);
                    }

                    throw new DeviceOccupiedException(index, isOrbbec: true);
                }

                return device;
            }

            /// <summary>Convenient string representation of object.</summary>
            /// <returns><c>"Orbbec Femto #{SerialNumber}</c></returns>
            public override string ToString()
                => "Orbbec Femto #" + SerialNumber;

            /// <inheritdoc cref="Device.IsConnected"/>
            public override bool IsConnected
                => !handle.IsDisposed
                    && api.DeviceGetVersion(handle.Value, out _) == NativeCallResults.Result.Succeeded;

            /// <summary>Can Orbbec camera be started in Subordinate mode?</summary>
            /// <remarks>
            /// Orbbec camera must preset the synchronization mode in advance,
            /// which can be achieved through `k4aviewer` advance preset,
            /// this synchronization mode is implemented using the mapping Orbbec SDK synchronization mode.
            /// </remarks>
            /// <exception cref="ObjectDisposedException">This property cannot be called for disposed object.</exception>
            /// <seealso cref="WiredSyncMode"/>
            public override bool IsSyncInConnected
            {
                get
                {
                    // Inspired by the latest changes in k4a.hpp (`device` class)
                    return WiredSyncMode == WiredSyncMode.Subordinate;
                }
            }

            /// <summary>Can Orbbec camera be started in Master mode?</summary>
            /// <remarks>
            /// Orbbec camera must preset the synchronization mode in advance,
            /// which can be achieved through `k4aviewer` advance preset,
            /// this synchronization mode is implemented using the mapping Orbbec SDK synchronization mode.
            /// </remarks>
            /// <exception cref="ObjectDisposedException">This property cannot be called for disposed object.</exception>
            /// <seealso cref="WiredSyncMode"/>
            public override bool IsSyncOutConnected
            {
                get
                {
                    // Inspired by the latest changes in k4a.hpp (`device` class)
                    return WiredSyncMode == WiredSyncMode.Master;
                }
            }

            /// <summary>Switches device clock sync mode (OrbbecSDK-K4A-Wrapper only).</summary>
            /// <param name="timestampMode">Device clock synchronization mode</param>
            /// <param name="interval">
            /// If <paramref name="timestampMode"/> is <see cref="DeviceClockSyncMode.Reset"/>: The delay time of executing the timestamp reset function after receiving the command or signal in microseconds.
            /// If <paramref name="timestampMode"/> is <see cref="DeviceClockSyncMode.Sync"/>: The interval for auto-repeated synchronization, in microseconds. If the value is <see cref="Microseconds32.Zero"/>, synchronization is performed only once.
            /// </param>
            /// <remarks><para>
            /// This API is used for device clock synchronization mode switching.
            /// </para><para>
            /// It is necessary to ensure that the mode switching of all devices is completed before any device start_cameras.
            /// </para><para>
            /// It is necessary to ensure that the master and slave devices are configured in the same mode.
            /// </para></remarks>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> cannot be negative.</exception>
            /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
            /// <exception cref="DeviceConnectionLostException">Connection with the device has been lost.</exception>
            public void SwitchDeviceClockSyncMode(DeviceClockSyncMode timestampMode, Microseconds32 interval)
            {
                if (interval.ValueUsec < 0)
                    throw new ArgumentOutOfRangeException(nameof(interval));
                CheckResult(NativeApi.Orbbec.Instance.DeviceSwitchDeviceClockSyncMode(handle.ValueNotDisposed, timestampMode, (uint)interval.ValueUsec));
            }

            /// <summary>Enables/disables soft filter for depth camera (OrbbecSDK-K4A-Wrapper only).</summary>
            /// <param name="filterSwitch">Device software filtering switch: <see langword="true"/> - enable software filtering; <see langword="false"/> - disable software filtering.</param>
            /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
            /// <exception cref="DeviceConnectionLostException">Connection with the device has been lost.</exception>
            public void SetSoftFilter(bool filterSwitch)
                => CheckResult(NativeApi.Orbbec.Instance.DeviceEnableSoftFilter(handle.ValueNotDisposed, filterSwitch ? (byte)1 : (byte)0));

            /// <summary>Gets device sync mode (OrbbecSDK-K4A-Wrapper only).</summary>
            /// <remarks>The device synchronization mode will change according to the mode configured in the <see cref="StartCameras(DeviceConfiguration)"/> method.</remarks>
            /// <exception cref="ObjectDisposedException">This property cannot be called for disposed object.</exception>
            /// <seealso cref="DeviceConfiguration.WiredSyncMode"/>
            /// <seealso cref="StartCameras(DeviceConfiguration)"/>
            public WiredSyncMode WiredSyncMode
                => NativeApi.Orbbec.Instance.DeviceGetWiredSyncMode(handle.ValueNotDisposed);

            /// <inheritdoc cref="Device.GetCalibration(DepthMode, ColorResolution)"/>
            public override Calibration GetCalibration(DepthMode depthMode, ColorResolution colorResolution)
            {
                if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                    throw new ArgumentOutOfRangeException(nameof(depthMode) + " and " + nameof(colorResolution), $"{nameof(depthMode)} and {nameof(colorResolution)} cannot be equal to Off simultaneously.");
                CheckColorResolution(colorResolution);
                CheckResult(api.DeviceGetCalibration(handle.ValueNotDisposed, depthMode, colorResolution, out CalibrationData calibrationData));
                return new Calibration.Orbbec(calibrationData);
            }

            private protected override void CheckDeviceConfiguration(DeviceConfiguration config)
                => CheckColorResolution(config.ColorResolution);

            private static void CheckColorResolution(ColorResolution colorResolution)
            {
                if (colorResolution == ColorResolution.R1536p || colorResolution == ColorResolution.R3072p)
                    throw new NotSupportedException($"Resolution {colorResolution} is not supported by ORBBEC devices");
            }
        }
    }
}
