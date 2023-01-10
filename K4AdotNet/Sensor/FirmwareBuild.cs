namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_FIRMWARE_BUILD_RELEASE,
    //     K4A_FIRMWARE_BUILD_DEBUG
    // } k4a_firmware_build_t;
    //
    /// <summary>Firmware build type.</summary>
    public enum FirmwareBuild : int
    {
        /// <summary>Production firmware.</summary>
        Release = 0,

        /// <summary>Pre-production firmware.</summary>
        Debug,
    }
}
