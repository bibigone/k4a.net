using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    // Defined in types.h:
    // typedef struct _k4a_record_subtitle_settings_t
    // {
    //     bool high_freq_data;
    // } k4a_record_subtitle_settings_t;
    //
    /// <summary>Structure containing additional metadata specific to custom subtitle tracks.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RecordSubtitleSettings
    {
        /// <summary>
        /// If <see langword="true"/>, data will be grouped together in batches to reduce overhead. 
        /// If <see langword="false"/>, data will be stored as individual blocks with full timestamp information (Default).
        /// </summary>
        /// <remarks>
        /// If <see langword="true"/>, only a single timestamp will
        /// be stored per batch, and an estimated timestamp will be used by <see cref="Playback.SeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/> and
        /// <see cref="PlaybackDataBlock.Timestamp"/>. The estimated timestamp is calculated with the assumption that
        /// blocks are evenly spaced within a batch. If precise timestamps are required, the timestamp should be added to
        /// each data block itself.
        /// </remarks>
        [MarshalAs(UnmanagedType.I1)]
        public bool HighFreqData;
    }
}
