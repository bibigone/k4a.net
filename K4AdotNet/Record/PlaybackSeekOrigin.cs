namespace K4AdotNet.Record
{
    // Defined in types.h:
    // typedef enum
    // {
    //     K4A_PLAYBACK_SEEK_BEGIN,
    //     K4A_PLAYBACK_SEEK_END,
    //     K4A_PLAYBACK_SEEK_DEVICE_TIME,
    // } k4a_playback_seek_origin_t;
    //
    /// <summary>Playback seeking positions.</summary>
    public enum PlaybackSeekOrigin : int
    {
        /// <summary>Seek relative to the beginning of a recording.</summary>
        Begin,

        /// <summary>Seek relative to the end of a recording.</summary>
        End,

        /// <summary>Seek to an absolute device timestamp.</summary>
        DeviceTime,
    }
}