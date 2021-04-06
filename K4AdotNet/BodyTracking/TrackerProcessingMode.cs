namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef enum
    // {
    //     K4ABT_TRACKER_PROCESSING_MODE_GPU = 0,
    //     K4ABT_TRACKER_PROCESSING_MODE_CPU,
    //     K4ABT_TRACKER_PROCESSING_MODE_GPU_CUDA,
    //     K4ABT_TRACKER_PROCESSING_MODE_GPU_TENSORRT,
    //     K4ABT_TRACKER_PROCESSING_MODE_GPU_DIRECTML
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
        /// <summary>
        /// SDK will use the most appropriate GPU mode for the operating system to run the tracker.
        /// Currently this is ONNX DirectML EP for Windows and ONNX Cuda EP for Linux. ONNX TensorRT EP is experimental
        /// </summary>
        Gpu = 0,

        /// <summary>SDK will use CPU only mode to run the tracker.</summary>
        Cpu,

        /// <summary>SDK will use ONNX Cuda EP to run the tracker.</summary>
        GpuCuda,

        /// <summary>SDK will use ONNX TensorRT EP to run the tracker.</summary>
        GpuTensorRT,

        /// <summary>SDK will use ONNX DirectML EP to run the tracker (Windows only).</summary>
        GpuDirectML,
    }
}
