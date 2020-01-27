namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef enum
    // {
    //     K4ABT_TRACKER_PROCESSING_MODE_GPU = 0,
    //     K4ABT_TRACKER_PROCESSING_MODE_CPU,
    // } k4abt_tracker_processing_mode_t;
    //
    /// <summary>Tracker processing mode types.</summary>
    /// <remarks>
    /// The CPU only mode doesn't require the machine to have a GPU to run this SDK.
    /// But it will be much slower than the GPU mode.
    /// </remarks>
    /// <seealso cref="TrackerConfiguration"/>
    /// <seealso cref="Tracker.Tracker"/>
    public enum TrackerProcessingMode
    {
        /// <summary>SDK will use GPU mode to run the tracker.</summary>
        Gpu = 0,

        /// <summary>SDK will use CPU only mode to run the tracker.</summary>
        Cpu,
    }
}
