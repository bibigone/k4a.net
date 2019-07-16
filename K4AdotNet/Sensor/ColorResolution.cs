namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_COLOR_RESOLUTION_OFF = 0,
    //     K4A_COLOR_RESOLUTION_720P,
    //     K4A_COLOR_RESOLUTION_1080P,
    //     K4A_COLOR_RESOLUTION_1440P,
    //     K4A_COLOR_RESOLUTION_1536P,
    //     K4A_COLOR_RESOLUTION_2160P,
    //     K4A_COLOR_RESOLUTION_3072P,
    // } k4a_color_resolution_t;
    /// <summary>Color sensor resolutions.</summary>
    /// <seealso cref="ColorResolutions"/>
    public enum ColorResolution
    {
        /// <summary>Color camera will be turned off with this setting</summary>
        Off = 0,

        /// <summary>1280x720  16:9</summary>
        R720p,

        /// <summary>1920x1080 16:9</summary>
        R1080p,

        /// <summary>2560x1440 16:9</summary>
        R1440p,

        /// <summary>2048x1536 4:3</summary>
        R1536p,

        /// <summary>3840x2160 16:9</summary>
        R2160p,

        /// <summary>4096x3072 4:3</summary>
        R3072p,
    }
}
