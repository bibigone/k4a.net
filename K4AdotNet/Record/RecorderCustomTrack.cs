using System;

namespace K4AdotNet.Record
{
    /// <summary>Custom track to be recorded using <see cref="Recorder"/>.</summary>
    /// <remarks>
    /// Use <see cref="WriteData(Microseconds64, byte[])"/> to write data for custom tracks.
    /// </remarks>
    /// <seealso cref="Recorder.CustomTracks"/>
    public sealed class RecorderCustomTrack
    {
        private readonly Recorder recorder;
        private readonly byte[] trackNameAsBytes;

        internal RecorderCustomTrack(Recorder recorder, string trackName, byte[] trackNameAsBytes, string codecId, byte[] codecContext)
        {
            this.recorder = recorder;
            this.trackNameAsBytes = trackNameAsBytes;

            Name = trackName;
            CodecId = codecId;
            CodecContext = codecContext;
        }

        /// <summary>The name of the custom track.</summary>
        public string Name { get; }

        /// <summary>Gets the codec id string for this track.</summary>
        /// <remarks>
        /// The codec ID is a string that corresponds to the codec of the track's data. Some of the existing formats are listed
        /// here: https://www.matroska.org/technical/specs/codecid/index.html. It can also be custom defined by the user.
        /// </remarks>
        public string CodecId { get; }

        /// <summary>Gets the codec context for this track.</summary>
        /// <remarks>
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element. Not <see langword="null"/>.
        /// </remarks>
        public byte[] CodecContext { get; }

        /// <summary>Writes data for the custom track to file.</summary>
        /// <param name="deviceTimestamp">
        /// The timestamp in microseconds for the custom track data. This timestamp should be in the same time domain as the
        /// device timestamp used for recording.
        /// </param>
        /// <param name="customData">The buffer of custom track data. Not <see langword="null"/>.</param>
        /// <remarks>
        /// Custom track data must be written in increasing order of timestamp, and the file's header must already be written.
        /// When writing custom track data at the same time as captures or IMU data, the custom data should be within 1 second of
        /// the most recently written timestamp.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="customData"/> is <see langword="null"/>.</exception>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed <see cref="Recorder"/>.</exception>
        public void WriteData(Microseconds64 deviceTimestamp, byte[] customData)
        {
            if (customData == null)
                throw new ArgumentNullException(nameof(customData));

            var customDataLength = Helpers.Int32ToUIntPtr(customData.Length);
            var res = NativeApi.RecordWriteCustomTrackData(RecorderHandle, trackNameAsBytes, deviceTimestamp, customData, customDataLength);
            if (res != NativeCallResults.Result.Succeeded)
                throw new RecordingException(recorder.FilePath);
        }

        private NativeHandles.RecordHandle RecorderHandle => Recorder.ToHandle(recorder);
    }
}