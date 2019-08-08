namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_CALIBRATION_TYPE_UNKNOWN = -1,
    //     K4A_CALIBRATION_TYPE_DEPTH,
    //     K4A_CALIBRATION_TYPE_COLOR,
    //     K4A_CALIBRATION_TYPE_GYRO,
    //     K4A_CALIBRATION_TYPE_ACCEL,
    //     K4A_CALIBRATION_TYPE_NUM,
    // } k4a_calibration_type_t;
    //
    /// <summary>Kinect for Azure device consists of different sensors each of them has their own coordinate system and calibration extrinsics.</summary>
    /// <seealso cref="CalibrationGeometries"/>
    /// <seealso cref="CalibrationExtrinsics"/>
    /// <seealso cref="Calibration.Extrinsics"/>
    public enum CalibrationGeometry
    {
        /// <summary>Calibration type is unknown.</summary>
        Unknown = -1,

        /// <summary>Depth sensor.</summary>
        Depth,

        /// <summary>Color sensor.</summary>
        Color,

        /// <summary>Gyroscope sensor.</summary>
        Gyro,

        /// <summary>Accelerometer sensor.</summary>
        Accel,

        /// <summary>Number of types excluding unknown type.</summary>
        Count,
    }
}
