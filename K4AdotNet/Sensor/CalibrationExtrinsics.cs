using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_extrinsics_t
    // {
    //    float rotation[9];
    //    float translation[3];
    // } k4a_calibration_extrinsics_t;
    //
    /// <summary>Extrinsic calibration defines the physical relationship between two separate sensors inside Kinect for Azure device.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationExtrinsics
    {
        /// <summary>3x3 rotation matrix.</summary>
        public Float3x3 Rotation;

        /// <summary>Translation vector (in millimeters).</summary>
        public Float3 Translation;
    }
}
