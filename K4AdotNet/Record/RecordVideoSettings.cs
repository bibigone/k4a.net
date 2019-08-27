using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    // Defined in types.h:
    // typedef struct _k4a_record_video_settings_t
    // {
    //     uint64_t width;
    //     uint64_t height;
    //     uint64_t frame_rate;
    // } k4a_record_video_settings_t;
    //
    /// <summary>Structure containing additional metadata specific to custom video tracks.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RecordVideoSettings
    {
        /// <summary>Frame width of the video.</summary>
        public long Width;

        /// <summary>Frame height of the video.</summary>
        public long Height;

        /// <summary>Frame rate (frames-per-second) of the video.</summary>
        public long FrameRate;
    }
}
