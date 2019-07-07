using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_t
    // {
    //     k4a_calibration_camera_t depth_camera_calibration;
    //     k4a_calibration_camera_t color_camera_calibration;
    //     k4a_calibration_extrinsics_t extrinsics[K4A_CALIBRATION_TYPE_NUM][K4A_CALIBRATION_TYPE_NUM];
    //     k4a_depth_mode_t depth_mode;
    //     k4a_color_resolution_t color_resolution;
    // } k4a_calibration_t;
    /// <summary>Information about device calibration in particular depth mode and color resolution.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Calibration
    {
        /// <summary>Depth camera calibration.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CameraCalibration DepthCameraCalibration;

        /// <summary>Color camera calibration.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CameraCalibration ColorCameraCalibration;

        /// <summary>Extrinsic transformation parameters.</summary>
        /// <remarks>
        /// The extrinsic parameters allow 3D coordinate conversions between depth camera, color camera, the IMU's gyroscope
        /// and accelerometer.To transform from a source to a target 3D coordinate system, use the parameters stored
        /// under <c>Extrinsics[source * (int)CalibrationSensor.Count + target]</c>.
        /// </remarks>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count)]
        public CalibrationExtrinsics[] Extrinsics;

        /// <summary>Depth camera mode for which calibration was obtained.</summary>
        public DepthMode DepthMode;

        /// <summary>Color camera resolution for which calibration was obtained.</summary>
        public ColorResolution ColorResolution;

        public CalibrationExtrinsics GetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor)
            => Extrinsics[(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor];

        public void SetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor, CalibrationExtrinsics extrinsics)
            => Extrinsics[(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor] = extrinsics;
    }
}
