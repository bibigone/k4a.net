using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_intrinsics_t
    // {
    //     k4a_calibration_model_type_t type;                 /**< Type of calibration model used*/
    //     unsigned int parameter_count;                      /**< Number of valid entries in parameters*/
    //     k4a_calibration_intrinsic_parameters_t parameters; /**< Calibration parameters*/
    // } k4a_calibration_intrinsics_t;
    /// <summary>Camera sensor intrinsic calibration data.</summary>
    /// <remarks>
    /// Intrinsic calibration represents the internal optical properties of the camera.
    /// 
    /// Azure Kinect devices are calibrated with Brown Conrady which is compatible with OpenCV.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationIntrinsics
    {
        /// <summary>Type of calibration model used.</summary>
        public CalibrationModel Model;

        /// <summary>Number of valid entries in <see cref="Parameters"/>.</summary>
        public int ParameterCount;

        /// <summary>Calibration parameters.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CalibrationIntrinsicParameters Parameters;
    }
}
