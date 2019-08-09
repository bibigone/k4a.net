using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_intrinsics_t
    // {
    //     k4a_calibration_model_type_t type;
    //     unsigned int parameter_count;
    //     k4a_calibration_intrinsic_parameters_t parameters;
    // } k4a_calibration_intrinsics_t;
    //
    /// <summary>Camera sensor intrinsic calibration data.</summary>
    /// <remarks><para>
    /// Intrinsic calibration represents the internal optical properties of the camera.
    /// </para><para>
    /// Azure Kinect devices are calibrated with Brown Conrady which is compatible with OpenCV.
    /// </para></remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationIntrinsics
    {
        /// <summary>Type of calibration model used.</summary>
        public CalibrationModel Model;

        /// <summary>Number of valid entries in <see cref="Parameters"/>.</summary>
        public int ParameterCount;

        /// <summary>Calibration parameters.</summary>
        public CalibrationIntrinsicParameters Parameters;
    }
}
