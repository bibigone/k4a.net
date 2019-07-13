using System;
using System.Text;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>device</c> class from <c>k4a.hpp</c>
    public sealed class Device : IDisposable
    {
        private readonly NativeHandles.DeviceHandle handle;

        private Device(NativeHandles.DeviceHandle handle, string serialNumber, HardwareVersion version)
        {
            this.handle = handle;
            SerialNumber = serialNumber;
            Version = version;
        }

        public void Dispose()
            => handle.Dispose();

        public string SerialNumber { get; }

        public HardwareVersion Version { get; }

        public bool IsConnected
            => handle != null && !handle.IsInvalid && !handle.IsClosed
                && NativeApi.DeviceGetSyncJack(handle, out var notUsed1, out var notUsed2) == NativeCallResults.Result.Succeeded;

        public bool SyncInConnected
        {
            get
            {
                CheckResult(NativeApi.DeviceGetSyncJack(handle, out var syncInConnected, out var syncOutConnected));
                return syncInConnected;
            }
        }

        public bool SyncOutConnected
        {
            get
            {
                CheckResult(NativeApi.DeviceGetSyncJack(handle, out var syncInConnected, out var syncOutConnected));
                return syncOutConnected;
            }
        }

        public void StartCameras(ref DeviceConfiguration config)
            => CheckResult(NativeApi.DeviceStartCameras(handle, ref config), nameof(config));

        public void StopCameras()
            => NativeApi.DeviceStopCameras(handle);

        public void StartImu()
            => CheckResult(NativeApi.DeviceStartImu(handle));

        public void StopImu()
            => NativeApi.DeviceStopImu(handle);

        public Capture TryGetCapture(Timeout timeout = default(Timeout))
        {
            var res = NativeApi.DeviceGetCapture(handle, out var captureHandle, timeout);
            if (res == NativeCallResults.WaitResult.Succeeded)
                return Capture.Create(captureHandle);
            if (res == NativeCallResults.WaitResult.Timeout)
                return null;
            ThrowException();
            return null;
        }

        public bool TryGetImuSample(out ImuSample imuSample, Timeout timeout = default(Timeout))
        {
            var res = NativeApi.DeviceGetImuSample(handle, out imuSample, timeout);
            if (res == NativeCallResults.WaitResult.Succeeded)
                return true;
            if (res == NativeCallResults.WaitResult.Timeout)
                return false;
            ThrowException();
            return false;
        }

        public bool TrySetColorControl(ColorControlCommand command, ColorControlMode mode, int value)
            => NativeApi.DeviceSetColorControl(handle, command, mode, value) == NativeCallResults.Result.Succeeded;

        public void GetColorControl(ColorControlCommand command, out ColorControlMode mode, out int value)
            => CheckResult(NativeApi.DeviceGetColorControl(handle, command, out mode, out value), nameof(command));

        public void GetColorControlCapabilities(ColorControlCommand command,
                                                   out bool supportsAuto, out int minValue, out int maxValue, out int valueStep,
                                                   out int defaultValue, out ColorControlMode defaultMode)
            => CheckResult(NativeApi.DeviceGetColorControlCapabilities(handle, command,
                                                           out supportsAuto, out minValue, out maxValue, out valueStep,
                                                           out defaultValue, out defaultMode), nameof(command));

        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.DeviceGetRawCalibration, handle, out var result))
                ThrowException();
            return result;
        }

        public void GetCalibration(DepthMode depthMode, ColorResolution colorResolution, out Calibration calibration)
            => CheckResult(NativeApi.DeviceGetCalibration(handle, depthMode, colorResolution, out calibration), nameof(depthMode), nameof(colorResolution));

        public override string ToString()
        {
            return "Azure Kinect #" + SerialNumber;
        }

        private void CheckResult(NativeCallResults.Result result, params string[] argNames)
        {
            if (result != NativeCallResults.Result.Succeeded)
                ThrowException(argNames);
        }

        private void ThrowException(params string[] argNames)
        {
            if (handle.IsClosed)
                throw new ObjectDisposedException(ToString());
            if (!IsConnected)
                throw new DeviceConnectionLostException();
            if (argNames != null && argNames.Length > 0)
                throw new ArgumentException("Invalid value of one of the following arguments: " + string.Join(", ", argNames));
            throw new InvalidOperationException();
        }

        public static int InstalledCount => checked((int)NativeApi.DeviceGetInstalledCount());

        public static Device TryOpen(int index = 0)
        {
            var res = NativeApi.DeviceOpen(checked((uint)index), out var deviceHandle);
            if (res != NativeCallResults.Result.Succeeded || deviceHandle == null || deviceHandle.IsInvalid)
                return null;

            if (!TryGetSerialNumber(deviceHandle, out var serialNumber)
                || !TryGetHardwareVersion(deviceHandle, out var version))
            {
                deviceHandle.Dispose();
                return null;
            }

            return new Device(deviceHandle, serialNumber, version);
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
            => device?.handle ?? NativeHandles.DeviceHandle.Zero;
    }
}
