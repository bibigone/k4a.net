using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_hardware_version_t
    // {
    //     k4a_version_t rgb;
    //     k4a_version_t depth;
    //     k4a_version_t audio;
    //     k4a_version_t depth_sensor;
    //     k4a_firmware_build_t firmware_build;
    //     k4a_firmware_signature_t firmware_signature;
    // } k4a_hardware_version_t;
    //
    /// <summary>Structure to define hardware version.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareVersion
    {
        /// <summary>Color camera firmware version.</summary>
        public FirmwareVersion ColorCameraFirmwareVersion;

        /// <summary>Depth camera firmware version.</summary>
        public FirmwareVersion DepthCameraFirmwareVersion;

        /// <summary>Audio device firmware version.</summary>
        public FirmwareVersion AudioDeviceFirmwareVersion;

        /// <summary>Depth sensor firmware version.</summary>
        public FirmwareVersion DepthSensorFirmwareVersion;

        /// <summary>Build type reported by the firmware.</summary>
        public FirmwareBuild FirmwareBuild;

        /// <summary>Signature type of the firmware.</summary>
        public FirmwareSignature FirmwareSignature;
    }
}
