namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //      K4A_DEPTH_MODE_OFF = 0,
    //      K4A_DEPTH_MODE_NFOV_2X2BINNED,
    //      K4A_DEPTH_MODE_NFOV_UNBINNED,
    //      K4A_DEPTH_MODE_WFOV_2X2BINNED,
    //      K4A_DEPTH_MODE_WFOV_UNBINNED,
    //      K4A_DEPTH_MODE_PASSIVE_IR,
    // } k4a_depth_mode_t;
    //
    /// <summary>Depth sensor capture modes.</summary>
    /// <remarks><para>
    /// Use extension methods from <see cref="DepthModes"/> class
    /// for additional details on the field of view, and supported frame rates for each mode.
    /// </para><para>
    /// Binned modes reduce the captured camera resolution by combining adjacent sensor pixels into a bin.
    /// </para></remarks>
    /// <seealso cref="DepthModes"/>
    public enum DepthMode : int
    {
        /// <summary>Depth sensor will be turned off with this setting.</summary>
        Off = 0,

        /// <summary>Depth captured at 320x288. Passive IR is also captured at 320x288.</summary>
        NarrowView2x2Binned,

        /// <summary>Depth captured at 640x576. Passive IR is also captured at 640x576.</summary>
        NarrowViewUnbinned,

        /// <summary>Depth captured at 512x512. Passive IR is also captured at 512x512.</summary>
        WideView2x2Binned,

        /// <summary>Depth captured at 1024x1024. Passive IR is also captured at 1024x1024.</summary>
        WideViewUnbinned,

        /// <summary>Passive IR only, captured at 1024x1024.</summary>
        PassiveIR,
    }
}
