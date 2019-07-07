namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_WIRED_SYNC_MODE_STANDALONE,
    //     K4A_WIRED_SYNC_MODE_MASTER,
    //     K4A_WIRED_SYNC_MODE_SUBORDINATE
    // } k4a_wired_sync_mode_t;
    /// <summary>Synchronization mode when connecting two or more devices together.</summary>
    public enum WiredSyncMode
    {
        /// <summary>Neither 'Sync In' or 'Sync Out' connections are used.</summary>
        Standalone = 0,

        /// <summary>The 'Sync Out' jack is enabled and synchronization data it driven out the connected wire.</summary>
        Master,

        /// <summary>
        /// The 'Sync In' jack is used for synchronization and 'Sync Out' is driven for the
        /// next device in the chain. 'Sync Out' is a mirror of 'Sync In' for this mode.
        /// </summary>
        Subordinate,
    }
}
