using K4AdotNet.Sensor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Unit.Sensor
{
    [TestClass]
    public class MarshalingTests
    {
        [TestMethod]
        public void TestSizesOfStructures()
        {
            // sizeof(k4a_calibration_t) == 1032
            Assert.AreEqual(1032, Marshal.SizeOf<CalibrationData>());

            // sizeof(k4a_calibration_camera_t) == 128
            Assert.AreEqual(128, Marshal.SizeOf<CameraCalibration>());

            // sizeof(k4a_calibration_extrinsics_t) == 48
            Assert.AreEqual(48, Marshal.SizeOf<CalibrationExtrinsics>());

            // sizeof(k4a_calibration_intrinsics_t) == 68
            Assert.AreEqual(68, Marshal.SizeOf<CalibrationIntrinsics>());

            // sizeof(k4a_calibration_intrinsic_parameters_t) == 60
            Assert.AreEqual(60, Marshal.SizeOf<CalibrationIntrinsicParameters>());

            // sizeof(k4a_device_configuration_t) == 36
            Assert.AreEqual(36, Marshal.SizeOf<DeviceConfiguration>());

            // sizeof(k4a_version_t) == 12
            Assert.AreEqual(12, Marshal.SizeOf<FirmwareVersion>());

            // sizeof(k4a_hardware_version_t) == 56
            Assert.AreEqual(56, Marshal.SizeOf<HardwareVersion>());

            // sizeof(k4a_imu_sample_t) == 48
            Assert.AreEqual(48, Marshal.SizeOf<ImuSample>());
        }
    }
}
