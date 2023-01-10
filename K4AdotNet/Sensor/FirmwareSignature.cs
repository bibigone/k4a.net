namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_FIRMWARE_SIGNATURE_MSFT,
    //     K4A_FIRMWARE_SIGNATURE_TEST,
    //     K4A_FIRMWARE_SIGNATURE_UNSIGNED
    // } k4a_firmware_signature_t;
    //
    /// <summary>Firmware signature type.</summary>
    public enum FirmwareSignature : int
    {
        /// <summary>Microsoft signed firmware.</summary>
        MicrosoftSignedFirmware = 0,

        /// <summary>Test signed firmware.</summary>
        TestSignedFirmware,

        /// <summary>Unsigned firmware.</summary>
        UnsignedFirmware,
    }
}
