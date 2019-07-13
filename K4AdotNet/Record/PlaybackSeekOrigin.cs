namespace K4AdotNet.Record
{
    // Defined in types.h:
    // typedef enum
    // {
    //     K4A_PLAYBACK_SEEK_BEGIN, /**< Seek relative to the beginning of a recording. */
    //     K4A_PLAYBACK_SEEK_END    /**< Seek relative to the end of a recording. */
    // } k4a_playback_seek_origin_t;
    /// <summary>Playback seeking positions.</summary>
    public enum PlaybackSeekOrigin
    {
        /// <summary>Seek relative to the beginning of a recording.</summary>
        Begin,

        /// <summary>Seek relative to the end of a recording.</summary>
        End,
    }
}