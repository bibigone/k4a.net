using System.Collections.Generic;

namespace K4AdotNet.Sensor
{
    public static class CalibrationGeometries
    {
        public static readonly IReadOnlyList<CalibrationGeometry> All = new[]
        {
            CalibrationGeometry.Depth,
            CalibrationGeometry.Color,
            CalibrationGeometry.Gyro,
            CalibrationGeometry.Accel,
        };

        public static bool IsCamera(this CalibrationGeometry geometry)
            => geometry == CalibrationGeometry.Depth || geometry == CalibrationGeometry.Color;

        public static bool IsImuPart(this CalibrationGeometry geometry)
            => geometry == CalibrationGeometry.Gyro || geometry == CalibrationGeometry.Accel;
    }
}
