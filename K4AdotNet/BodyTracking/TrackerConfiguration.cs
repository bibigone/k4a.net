using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef struct _k4abt_tracker_configuration_t
    // {
    //     k4abt_sensor_orientation_t sensor_orientation;
    //     k4abt_tracker_processing_mode_t processing_mode;
    //     int32_t gpu_device_id;
    //     const char* model_path;
    // } k4abt_tracker_configuration_t;
    //
    /// <summary>Configuration parameters for a k4abt body tracker.</summary>
    /// <remarks>Used by <see cref="Tracker.Tracker"/> to specify the configuration of the k4abt tracker.</remarks>
    /// <seealso cref="Tracker.Tracker"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TrackerConfiguration
    {
        /// <summary>The sensor mounting orientation type.</summary>
        /// <remarks>Setting the correct orientation can help the body tracker to achieve more accurate body tracking results.</remarks>
        public SensorOrientation SensorOrientation;

        /// <summary>Specify whether to use CPU only mode or GPU mode to run the tracker.</summary>
        /// <remarks>
        /// The CPU only mode doesn't require the machine to have a GPU to run this SDK.
        /// But it will be much slower than the GPU mode.
        /// </remarks>
        public TrackerProcessingMode ProcessingMode;

        /// <summary>Specify the GPU device ID to run the tracker.</summary>
        /// <remarks><para>
        /// The setting is not effective if the <see cref="ProcessingMode"/> setting is set to <see cref="TrackerProcessingMode.Cpu"/>.
        /// </para><para>
        /// For <see cref="TrackerProcessingMode.GpuCuda"/> and <see cref="TrackerProcessingMode.GpuTensorRT"/> modes,
        /// ID of the graphic card can be retrieved using the CUDA API.
        /// </para><para>
        /// In case when processing_mode is <see cref="TrackerProcessingMode.GpuDirectML"/>,
        /// the device ID corresponds to the enumeration order of hardware adapters as given by <c>IDXGIFactory::EnumAdapters</c>.
        /// </para><para>
        /// A device_id of 0 always corresponds to the default adapter, which is typically the primary display GPU installed on the system.
        /// </para><para>
        /// More information can be found in the ONNX Runtime Documentation.
        /// </para></remarks>
        public int GpuDeviceId;

        /// <summary>Specify the model file name and location used by the tracker.</summary>
        /// <remarks>If specified, the tracker will use this model instead of the default one.</remarks>
        public string? ModelPath;

        // static const k4abt_tracker_configuration_t K4ABT_TRACKER_CONFIG_DEFAULT = { K4ABT_SENSOR_ORIENTATION_DEFAULT,  // sensor_orientation
        //                                                                             K4ABT_TRACKER_PROCESSING_MODE_GPU, // processing_mode
        //                                                                             0 };                               // gpu_device_id
        //
        /// <summary>Default configuration setting for k4abt tracker.</summary>
        /// <remarks>Use this setting to initialize a <see cref="TrackerConfiguration"/> to a default state.</remarks>
        public static readonly TrackerConfiguration Default = new TrackerConfiguration {
                SensorOrientation = SensorOrientation.Default,
                ProcessingMode = TrackerProcessingMode.GpuCuda,
                GpuDeviceId = 0,
                ModelPath = null,
            };
    }
}
