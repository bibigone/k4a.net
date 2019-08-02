using Microsoft.VisualStudio.TestTools.UnitTesting;
using K4AdotNet.Sensor;

namespace K4AdotNet.Tests.SensorTypesUnitTests
{
    [TestClass]
    public class DeviceConfigurationTests
    {
        [TestMethod]
        public void TestThatDisableAllConfigIsValid()
        {
            var config = DeviceConfiguration.DisableAll;

            Assert.IsTrue(config.IsValid(out var message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void TestValidConfig()
        {
            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Thirty,
                ColorFormat = ImageFormat.ColorMjpg,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NarrowViewUnbinned,
                WiredSyncMode = WiredSyncMode.Standalone,
            };

            Assert.IsTrue(config.IsValid(out var message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void TestInvalidConfigs()
        {
            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Thirty,
                ColorFormat = ImageFormat.ColorMjpg,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.WideViewUnbinned,     // this mode is not compatible with 30 FPS
                WiredSyncMode = WiredSyncMode.Standalone,
            };

            Assert.IsFalse(config.IsValid(out var message));
            Assert.IsNotNull(message);

            config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Thirty,
                ColorFormat = ImageFormat.ColorMjpg,
                ColorResolution = ColorResolution.R3072p,   // this resolution is not compatible with 30 FPS
                DepthMode = DepthMode.Off,
                WiredSyncMode = WiredSyncMode.Standalone,
            };

            Assert.IsFalse(config.IsValid(out message));
            Assert.IsNotNull(message);

            config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Fifteen,
                ColorFormat = ImageFormat.ColorNV12,
                ColorResolution = ColorResolution.R1080p,   // this resolution is not compatible with NV12
                DepthMode = DepthMode.Off,
                WiredSyncMode = WiredSyncMode.Standalone,
            };

            Assert.IsFalse(config.IsValid(out message));
            Assert.IsNotNull(message);

            config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Fifteen,
                ColorFormat = ImageFormat.IR16,             // <- oops!
                ColorResolution = ColorResolution.Off,
                DepthMode = DepthMode.Off,
                WiredSyncMode = WiredSyncMode.Standalone,
            };

            Assert.IsFalse(config.IsValid(out message));
            Assert.IsNotNull(message);
        }
    }
}
