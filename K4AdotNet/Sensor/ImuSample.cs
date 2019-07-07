using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_imu_sample_t
    // {
    //     float temperature;
    //     k4a_float3_t acc_sample;
    //     uint64_t acc_timestamp_usec;
    //     k4a_float3_t gyro_sample;
    //     uint64_t gyro_timestamp_usec;
    // } k4a_imu_sample_t;
    /// <summary>IMU sample.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ImuSample
    {
        /// <summary>Temperature reading of this sample (Celsius).</summary>
        public float Temperature;

        /// <summary>Accelerometer sample in meters per second squared.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Float3 AccelerometerSample;

        /// <summary>Time stamp of the accelerometer.</summary>
        public TimeStamp AccelerometerTimeStamp;

        /// <summary>Gyro sample in radians per second.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Float3 GyroSample;

        /// <summary>Time stamp of the gyroscope in microseconds.</summary>
        public long GyroTimeStamp;
    }
}
