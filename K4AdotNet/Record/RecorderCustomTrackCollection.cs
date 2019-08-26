using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace K4AdotNet.Record
{
    /// <summary>Collection of <see cref="RecorderCustomTrack"/> to be written using <see cref="Recorder"/>.</summary>
    /// <remarks>
    /// Use <see cref="AddVideoTrack(string, string, byte[], RecordVideoSettings)"/> and <see cref="AddCustomSubtitleTrack(string, string, byte[], RecordSubtitleSettings)"/>
    /// to add new custom tracks to recording.
    /// </remarks>
    /// <seealso cref="Recorder.CustomTracks"/>
    /// <seealso cref="RecorderCustomTrack"/>
    public sealed class RecorderCustomTrackCollection : IReadOnlyList<RecorderCustomTrack>
    {
        private readonly Recorder recorder;
        private readonly List<RecorderCustomTrack> tracks = new List<RecorderCustomTrack>();

        internal RecorderCustomTrackCollection(Recorder recorder)
            => this.recorder = recorder;

        /// <summary>Gets custom track to be recorded by zero-based index.</summary>
        /// <param name="index">Zero-based index of custom track.</param>
        /// <returns>Custom track with a given index. Not <see langword="null"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is less than zero or is greater than or equal to <see cref="Count"/>.</exception>
        /// <remarks>
        /// Use <see cref="AddVideoTrack(string, string, byte[], RecordVideoSettings)"/> and <see cref="AddCustomSubtitleTrack(string, string, byte[], RecordSubtitleSettings)"/>
        /// to add new custom tracks to recording.
        /// </remarks>
        public RecorderCustomTrack this[int index] => tracks[index];

        /// <summary>Gets custom track to be recorded by its name.</summary>
        /// <param name="name">Name of custom track.</param>
        /// <returns>Custom track with a given name or <see langword="null"/> if there is no custom track with a given name.</returns>
        /// <remarks>
        /// Use <see cref="AddVideoTrack(string, string, byte[], RecordVideoSettings)"/> and <see cref="AddCustomSubtitleTrack(string, string, byte[], RecordSubtitleSettings)"/>
        /// to add new custom tracks to recording.
        /// </remarks>
        public RecorderCustomTrack this[string name] => tracks.Find(t => t.Name == name);

        /// <summary>Count of added custom tracks.</summary>
        public int Count => tracks.Count;

        /// <summary>Implementation of <see cref="IEnumerable{RecorderCustomTrack}"/>.</summary>
        /// <returns>All added custom tracks.</returns>
        public IEnumerator<RecorderCustomTrack> GetEnumerator() => tracks.GetEnumerator();

        /// <summary>Implementation of <see cref="IEnumerable"/>.</summary>
        /// <returns>All added custom tracks.</returns>
        IEnumerator IEnumerable.GetEnumerator() => tracks.GetEnumerator();

        /// <summary>Adds custom video track to the recording.</summary>
        /// <param name="trackName">
        /// The name of the custom video track to be added. Not <see langword="null"/>.
        /// Track names must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.
        /// </param>
        /// <param name="codecId">
        /// The codec ID of the track. Some of the existing formats are listed here:
        /// https://www.matroska.org/technical/specs/codecid/index.html. The codec ID can also be custom defined by the user.
        /// Video codec ID's should start with 'V_'.
        /// </param>
        /// <param name="codecContext">
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element. Not <see langword="null"/>.
        /// </param>
        /// <param name="trackSettings">Additional metadata for the video track such as resolution and frame rate.</param>
        /// <returns>Added custom video track. Non <see langword="null"/>. Call <see cref="RecorderCustomTrack.WritekData(Microseconds64, byte[])"/> to write data to this track.</returns>
        /// <remarks><para>
        /// Built-in video tracks like the DEPTH, IR, and COLOR tracks will be created automatically when the object of <see cref="Recorder"/>
        /// class is constructed. This API can be used to add additional video tracks to save custom data.
        /// </para><para>
        /// All tracks need to be added before the recording header is written.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="trackName"/> or <paramref name="codecId"/> or <paramref name="codecContext"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="trackName"/> is not a valid track name (must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_')
        /// or <paramref name="codecId"/> is not a valid video codec ID.
        /// </exception>
        /// <exception cref="InvalidOperationException">This method must be called before <see cref="Recorder.WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed <see cref="Recorder"/>.</exception>
        public RecorderCustomTrack AddVideoTrack(string trackName, string codecId, byte[] codecContext, RecordVideoSettings trackSettings)
        {
            Helpers.CheckTrackName(trackName);
            if (string.IsNullOrEmpty(codecId))
                throw new ArgumentNullException(nameof(codecId));
            if (!codecId.StartsWith("V_"))
                throw new ArgumentException("Video codec ID should start with 'V_'", nameof(codecId));
            if (codecContext == null)
                throw new ArgumentNullException(nameof(codecContext));
            if (trackSettings.Width < 0 || trackSettings.Height < 0 || trackSettings.FrameRate < 0)
                throw new ArgumentException($"Invalid value of {trackSettings}", nameof(trackSettings));

            var trackNameAsBytes = Helpers.StringToBytes(trackName, Encoding.ASCII);
            var codecIdAsBytes = Helpers.StringToBytes(codecId, Encoding.UTF8);
            var codecContextLength = Helpers.Int32ToUIntPtr(codecContext.Length);

            var res = NativeApi.RecordAddCustomVideoTrack(RecorderHandle, trackNameAsBytes, codecIdAsBytes, codecContext, codecContextLength, ref trackSettings);

            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddVideoTrack)}() must be called before {nameof(Recorder)}.{nameof(Recorder.WriteHeader)}().");

            var track = new RecorderCustomTrack(recorder, trackName, trackNameAsBytes, codecId, codecContext);
            tracks.Add(track);

            return track;
        }

        /// <summary>Adds custom subtitle track to the recording.</summary>
        /// <param name="trackName">
        /// The name of the custom subtitle track to be added. Not <see langword="null"/>.
        /// Track names must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.
        /// </param>
        /// <param name="codecId">
        /// The codec ID of the track. Some of the existing formats are listed here:
        /// https://www.matroska.org/technical/specs/codecid/index.html. The codec ID can also be custom defined by the user.
        /// Subtitle codec ID's should start with 'S_'.
        /// </param>
        /// <param name="codecContext">
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element. Not <see langword="null"/>.
        /// </param>
        /// <param name="trackSettings">Additional metadata for the video track such as resolution and frame rate.</param>
        /// <returns>Added custom subtitle track. Non <see langword="null"/>. Call <see cref="RecorderCustomTrack.WritekData(Microseconds64, byte[])"/> to write data to this track.</returns>
        /// <remarks><para>
        /// Built-in subtitle tracks like the IMU track will be created automatically when the <see cref="Recorder.AddImuTrack"/> API is
        /// called. This API can be used to add additional subtitle tracks to save custom data.
        /// </para><para>
        /// All tracks need to be added before the recording header is written.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="trackName"/> or <paramref name="codecId"/> or <paramref name="codecContext"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="trackName"/> is not a valid track name (must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_')
        /// or <paramref name="codecId"/> is not a valid subtitle codec ID.
        /// </exception>
        /// <exception cref="InvalidOperationException">This method must be called before <see cref="Recorder.WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed <see cref="Recorder"/>.</exception>
        public RecorderCustomTrack AddCustomSubtitleTrack(string trackName, string codecId, byte[] codecContext, RecordSubtitleSettings trackSettings)
        {
            Helpers.CheckTrackName(trackName);
            if (string.IsNullOrEmpty(codecId))
                throw new ArgumentNullException(nameof(codecId));
            if (!codecId.StartsWith("S_"))
                throw new ArgumentException("Subtitle codec ID should start with 'S_'", nameof(codecId));
            if (codecContext == null)
                throw new ArgumentNullException(nameof(codecContext));

            var trackNameAsBytes = Helpers.StringToBytes(trackName, Encoding.ASCII);
            var codecIdAsBytes = Helpers.StringToBytes(codecId, Encoding.UTF8);
            var codecContextLength = Helpers.Int32ToUIntPtr(codecContext.Length);

            var res = NativeApi.RecordAddCustomSubtitleTrack(RecorderHandle, trackNameAsBytes, codecIdAsBytes, codecContext, codecContextLength, ref trackSettings);

            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddCustomSubtitleTrack)}() must be called before {nameof(Recorder)}.{nameof(Recorder.WriteHeader)}().");

            var track = new RecorderCustomTrack(recorder, trackName, trackNameAsBytes, codecId, codecContext);
            tracks.Add(track);

            return track;
        }

        private NativeHandles.RecordHandle RecorderHandle => Recorder.ToHandle(recorder);
    }
}
