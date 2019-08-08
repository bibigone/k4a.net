using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    /// <summary>Extensions to <see cref="CalibrationGeometry"/> enumeration. Adds some metadata to <see cref="CalibrationGeometry"/> enumeration.</summary>
    /// <seealso cref="CalibrationGeometry"/>
    public static class CalibrationGeometries
    {
        /// <summary>
        /// All usable <see cref="CalibrationGeometry"/>s
        /// (<see cref="CalibrationGeometry.Unknown"/> and <see cref="CalibrationGeometry.Count"/> are not in this list).
        /// May be helpful for UI, tests, etc.
        /// </summary>
        public static readonly IReadOnlyList<CalibrationGeometry> All = new[]
        {
            CalibrationGeometry.Depth,
            CalibrationGeometry.Color,
            CalibrationGeometry.Gyro,
            CalibrationGeometry.Accel,
        };

        /// <summary>Is it camera?</summary>
        /// <param name="geometry">Element to be tested.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="geometry"/> is an color or depth camera,
        /// <see langword="false"/> - otherwise.
        /// </returns>
        public static bool IsCamera(this CalibrationGeometry geometry)
            => geometry == CalibrationGeometry.Depth || geometry == CalibrationGeometry.Color;

        /// <summary>Is it part of IMU sensor (gyro or accelerometer)?</summary>
        /// <param name="geometry">Element to be tested.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="geometry"/> is an part of IMU sensor (gyro or accel),
        /// <see langword="false"/> - otherwise.
        /// </returns>
        public static bool IsImuPart(this CalibrationGeometry geometry)
            => geometry == CalibrationGeometry.Gyro || geometry == CalibrationGeometry.Accel;
    }
}
