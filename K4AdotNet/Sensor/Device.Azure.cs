using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace K4AdotNet.Sensor
{
    partial class Device
    {
        /// <summary>
        /// Implementation of base <see cref="Device"/> class for Azure Kinect devices.
        /// This class works via `original K4A` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Azure"/> and <see cref="ComboMode.Both"/>.</remarks>
        public class Azure : Device
        {
            internal Azure(NativeHandles.DeviceHandle handle, int deviceIndex, string serialNumber, HardwareVersion version)
                : base(handle, deviceIndex, serialNumber, version)
            {
                Debug.Assert(handle is NativeHandles.DeviceHandle.Azure);
            }

            /// <summary>Gets the number of connected Azure Kinect devices.</summary>
            /// <remarks>Some devices can be occupied by other processes by they are counted here as connected.</remarks>
            public static new int InstalledCount => checked((int)NativeApi.Azure.Instance.DeviceGetInstalledCount());

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
            public static bool TryOpen([NotNullWhen(returnValue: true)] out Azure? device, int index = DefaultDeviceIndex)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var res = NativeApi.Azure.Instance.DeviceOpen(checked((uint)index), out var deviceHandle);
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

                device = new Azure(deviceHandle, index, serialNumber, version);
                return true;
            }

            private static bool TryGetSerialNumber(NativeHandles.DeviceHandle deviceHandle, [NotNullWhen(returnValue: true)] out string? serialNumber)
            {
                if (!Helpers.TryGetValueInByteBuffer(NativeApi.Azure.Instance.DeviceGetSerialnum, deviceHandle, out var result))
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
                => NativeApi.Azure.Instance.DeviceGetVersion(deviceHandle, out version) == NativeCallResults.Result.Succeeded;

            /// <summary>Opens an Azure Kinect device. Like <see cref="TryOpen(out Azure, int)"/> but raises exception in case of failure.</summary>
            /// <param name="index">Zero-based index of device to be opened. By default - <see cref="DefaultDeviceIndex"/>.</param>
            /// <returns>Opened device. Not <see langword="null"/>. Don't forget to call <see cref="Dispose"/> method for this object after usage.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
            /// <exception cref="DeviceNotFoundException">Device with <paramref name="index"/> index is not found. Most likely it is not connected or hasn't power supply.</exception>
            /// <exception cref="DeviceOccupiedException">This Kinect device is occupied by another process, or it is already opened in current process, or it has not enough power supply.</exception>
            /// <seealso cref="TryOpen(out Azure, int)"/>
            /// <seealso cref="InstalledCount"/>
            public static new Azure Open(int index = DefaultDeviceIndex)
            {
                if (!TryOpen(out var device, index))
                {
                    if (index >= InstalledCount)
                    {
                        throw new DeviceNotFoundException(index, isOrbbec: false);
                    }

                    throw new DeviceOccupiedException(index, isOrbbec: false);
                }

                return device;
            }

            /// <summary>Convenient string representation of object.</summary>
            /// <returns><c>Azure Kinect #{SerialNumber}</c></returns>
            public override string ToString()
                => "Azure Kinect #" + SerialNumber;

            /// <inheritdoc cref="Device.IsConnected"/>
            public override bool IsConnected
                => !handle.IsDisposed
                    && NativeApi.Azure.Instance.DeviceGetSyncJack(handle.Value, out _, out _) == NativeCallResults.Result.Succeeded;

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
                    CheckResult(NativeApi.Azure.Instance.DeviceGetSyncJack(handle.ValueNotDisposed, out var syncInConnectedFlag, out _));
                    return syncInConnectedFlag != 0;
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
                    CheckResult(NativeApi.Azure.Instance.DeviceGetSyncJack(handle.ValueNotDisposed, out _, out var syncOutConnectedFlag));
                    return syncOutConnectedFlag != 0;
                }
            }

            /// <inheritdoc cref="Device.GetCalibration(DepthMode, ColorResolution)"/>
            public override Calibration GetCalibration(DepthMode depthMode, ColorResolution colorResolution)
            {
                if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                    throw new ArgumentOutOfRangeException(nameof(depthMode) + " and " + nameof(colorResolution), $"{nameof(depthMode)} and {nameof(colorResolution)} cannot be equal to Off simultaneously.");

                CheckResult(api.DeviceGetCalibration(handle.ValueNotDisposed, depthMode, colorResolution, out CalibrationData calibrationData));
                return new Calibration.Azure(calibrationData);
            }
        }
    }
}
