namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef enum
    // {
    //     K4ABT_SENSOR_ORIENTATION_DEFAULT = 0,
    //     K4ABT_SENSOR_ORIENTATION_CLOCKWISE90,
    //     K4ABT_SENSOR_ORIENTATION_COUNTERCLOCKWISE90,
    //     K4ABT_SENSOR_ORIENTATION_FLIP180,
    // } k4abt_sensor_orientation_t;
    //
    /// <summary>Sensor mounting orientation types.</summary>
    /// <remarks>
    /// This enumeration specifies the sensor mounting orientation. Passing the correct orientation in <see cref="Tracker.Tracker"/>
    /// can help the body tracker to achieve more accurate body tracking.
    /// </remarks>
    /// <seealso cref="TrackerConfiguration"/>
    /// <seealso cref="Tracker.Tracker"/>
    public enum SensorOrientation
    {
        /// <summary>Mount the sensor at its default orientation.</summary>
        Default = 0,

        /// <summary>Clock-wisely rotate the sensor 90°.</summary>
        Clockwise90,

        /// <summary>Counter clock-wisely rotate the sensor 90°.</summary>
        Counterclockwise90,

        /// <summary>Mount the sensor upside-down.</summary>
        Flip180,
    }
}
