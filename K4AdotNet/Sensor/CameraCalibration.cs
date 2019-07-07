using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_camera_t
    // {
    //     k4a_calibration_extrinsics_t extrinsics;
    //     k4a_calibration_intrinsics_t intrinsics;
    //     int resolution_width;
    //     int resolution_height;
    //     float metric_radius;
    // } k4a_calibration_camera_t;
    /// <summary>Camera calibration contains intrinsic and extrinsic calibration information for depth/color camera.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraCalibration
{
        /// <summary>Extrinsic calibration data.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CalibrationExtrinsics Extrinsics;

        /// <summary>Intrinsic calibration data.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CalibrationIntrinsics Intrinsics;

        /// <summary>Resolution width of the camera.</summary>
        public int ResolutionWidth;

        /// <summary>Resolution height of the camera.</summary>
        public int ResolutionHeight;

        /// <summary>Max FOV of the camera.</summary>
        public float MetricRadius;
    }
}
