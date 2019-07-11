using System.Runtime.InteropServices;

namespace K4AdotNet.Playback
{
    // typedef struct _k4a_record_configuration_t
    // {
    //      k4a_image_format_t color_format;
    //      k4a_color_resolution_t color_resolution;
    //      k4a_depth_mode_t depth_mode;
    //      k4a_fps_t camera_fps;
    //      bool color_track_enabled;
    //      bool depth_track_enabled;
    //      bool ir_track_enabled;
    //      bool imu_track_enabled;
    //      int32_t depth_delay_off_color_usec;
    //      k4a_wired_sync_mode_t wired_sync_mode;
    //      uint32_t subordinate_delay_off_master_usec;
    // } k4a_record_configuration_t;
    /// <summary>Structure containing the device configuration used to record.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RecordConfiguration
    {
        /// <summary>Image format used to record the color camera.</summary>
        public Sensor.ImageFormat ColorFormat;

        /// <summary>Image resolution used to record the color camera.</summary>
        public Sensor.ColorResolution ColorResolution;

        /// <summary>Mode used to record the depth camera.</summary>
        public Sensor.DepthMode DepthMode;

        /// <summary>Frame rate used to record the color and depth camera.</summary>
        public Sensor.FrameRate CameraFps;

        /// <summary><see langword="true"/> if the recording contains Color camera frames.</summary>
        public bool ColorTrackEnabled;

        /// <summary><see langword="true"/> if the recording contains Depth camera frames.</summary>
        public bool DepthTrackEnabled;

        /// <summary><see langword="true"/> if the recording contains IR camera frames.</summary>
        public bool IRTrackEnabled;

        /// <summary><see langword="true"/> if the recording contains IMU sample data.</summary>
        public bool ImuTrackEnabled;

        /// <summary>
        /// The delay between color and depth images in the recording.
        /// A negative delay means depth images are first, and a positive delay means color images are first.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Delay DepthDelayOffColor;

        /// <summary>External synchronization mode.</summary>
        public Sensor.WiredSyncMode WiredSyncMode;

        /// <summary>
        /// The timestamp offset of the start of the recording. All recorded time stamps are offset by this value such that
        /// the recording starts at timestamp <see cref="Delay.Zero"/>. This value can be used to synchronize time stamps between two recording files.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Delay SubordinateDelayOffMaster;
    }
}
