namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_FRAMES_PER_SECOND_5 = 0,
    //     K4A_FRAMES_PER_SECOND_15,
    //     K4A_FRAMES_PER_SECOND_30,
    //     // add by orbbec
    //     K4A_FRAMES_PER_SECOND_25,
    // } k4a_fps_t;
    //
    /// <summary>Color and depth sensor frame rate.</summary>
    /// <remarks><para>
    /// This enumeration is used to select the desired frame rate to operate the cameras. The actual
    /// frame rate may vary slightly due to dropped data, synchronization variation between devices,
    /// clock accuracy, or if the camera exposure priority mode causes reduced frame rate.
    /// </para><para>
    /// Use extension methods from <see cref="FrameRates"/> class for additional details.
    /// </para></remarks>
    /// <seealso cref="FrameRates"/>
    public enum FrameRate : int
    {
        /// <summary>Five (5) frames per second.</summary>
        Five = 0,

        /// <summary>Fifteen (15) frames per second.</summary>
        Fifteen,

        /// <summary>Thirty (30) frames per second.</summary>
        Thirty,
    }
}
