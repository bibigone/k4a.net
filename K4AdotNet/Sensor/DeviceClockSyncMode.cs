namespace K4AdotNet.Sensor
{
    // typedef enum {
    //      K4A_DEVICE_CLOCK_SYNC_MODE_RESET = 0,
    //      K4A_DEVICE_CLOCK_SYNC_MODE_SYNC
    // } k4a_device_clock_sync_mode_t;
    /// <summary>Device clock synchronization mode type. Only for Orbbec K4A Wrapper.</summary>
    public enum DeviceClockSyncMode : int
    {
        /// <summary>device clock sync mode is reset</summary>
        Reset = 0,

        /// <summary>device clock sync mode is asynchronous timing</summary>
        Sync,
    }
}
