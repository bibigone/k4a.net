using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace K4AdotNet.Record
{
    /// <summary>Information about track in <see cref="Playback"/>.</summary>
    /// <seealso cref="Playback.Tracks"/>
    /// <seealso cref="PlaybackTrackCollection"/>
    public sealed class PlaybackTrack
    {
        private readonly Playback playback;
        private readonly byte[] nameAsBytes;

        /// <summary>Creates track object with specified <paramref name="index"/> for specified <paramref name="playback"/>.</summary>
        /// <param name="playback">Owner. Not <see langword="null"/>.</param>
        /// <param name="index">Zero-based index of track.</param>
        /// <exception cref="PlaybackException">Cannot get name of track with specified index.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed <paramref name="playback"/> object.</exception>
        internal PlaybackTrack(Playback playback, int index)
        {
            this.playback = playback;
            Index = index;
            Name = GetTrackName(out nameAsBytes);
            IsBuiltIn = NativeApi.PlaybackTrackIsBuiltIn(PlaybackHandle, nameAsBytes) != 0;
        }

        private string GetTrackName(out byte[] nameAsBytes)
        {
            if (!Helpers.TryGetValueInByteBuffer(GetTrackName, Index, out var bytes))
                throw new PlaybackException("Cannot get name of track #" + Index, playback.FilePath);
            nameAsBytes = bytes;
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1);
        }

        private NativeCallResults.BufferResult GetTrackName(int trackIndex, IntPtr buffer, ref UIntPtr size)
            => NativeApi.PlaybackGetTrackName(PlaybackHandle, Helpers.Int32ToUIntPtr(trackIndex), buffer, ref size);

        /// <summary>Zero-based index of track.</summary>
        public int Index { get; }

        /// <summary>Name of track. Not <see langword="null"/> and empty.</summary>
        public string Name { get; }

        /// <summary>Checks whether a track is one of the built-in tracks: "COLOR", "DEPTH", etc...</summary>
        /// <exception cref="ObjectDisposedException">Appropriated <see cref="Playback"/> object was disposed object.</exception>
        public bool IsBuiltIn { get; }

        /// <summary>Gets the video-specific track information for this track.</summary>
        /// <exception cref="InvalidOperationException">This is not a video track.</exception>
        /// <exception cref="ObjectDisposedException">Appropriated <see cref="Playback"/> object was disposed object.</exception>
        public RecordVideoSettings VideoSettings
        {
            get
            {
                var res = NativeApi.PlaybackTrackGetVideoSetting(PlaybackHandle, nameAsBytes, out var videoSettings);
                if (res != NativeCallResults.Result.Succeeded)
                    throw new InvalidOperationException("This is not a video track.");
                return videoSettings;
            }
        }

        /// <summary>Gets the codec id string for this track.</summary>
        /// <remarks>
        /// The codec ID is a string that corresponds to the codec of the track's data. Some of the existing formats are listed
        /// here: https://www.matroska.org/technical/specs/codecid/index.html. It can also be custom defined by the user.
        /// </remarks>
        /// <exception cref="PlaybackException">Cannot get coded ID for this track for some reason. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">Appropriated <see cref="Playback"/> object was disposed object.</exception>
        public string CodecId
        {
            get
            {
                if (!Helpers.TryGetValueInByteBuffer(GetCodecId, nameAsBytes, out var codecIdAsBytes))
                    throw new PlaybackException("Cannot get codec ID for track #" + Index, playback.FilePath);
                return Encoding.UTF8.GetString(codecIdAsBytes, 0, codecIdAsBytes.Length - 1);
            }
        }

        private NativeCallResults.BufferResult GetCodecId(byte[] trackNameAsBytes, IntPtr buffer, ref UIntPtr size)
            => NativeApi.PlaybackTrackGetCodecId(PlaybackHandle, trackNameAsBytes, buffer, ref size);

        /// <summary>Gets the codec context for this track.</summary>
        /// <remarks>
        /// The codec context is a codec-specific buffer that contains any required codec metadata that is only known to the
        /// codec. It is mapped to the matroska <c>CodecPrivate</c> element. Not <see langword="null"/>.
        /// </remarks>
        /// <exception cref="PlaybackException">Cannot get coded context for this track for some reason. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">Appropriated <see cref="Playback"/> object was disposed object.</exception>
        public byte[] CodecContext
        {
            get
            {
                if (!Helpers.TryGetValueInByteBuffer(GetCodecContext, nameAsBytes, out var codecContext))
                    throw new PlaybackException("Cannot get codec context for track #" + Index, playback.FilePath);
                return codecContext;
            }
        }

        private NativeCallResults.BufferResult GetCodecContext(byte[] trackNameAsBytes, IntPtr buffer, ref UIntPtr size)
            => NativeApi.PlaybackTrackGetCodecContext(PlaybackHandle, trackNameAsBytes, buffer, ref size);

        /// <summary>Reads the next data block for this track.</summary>
        /// <param name="dataBlock">Output: data block object on success or <see langword="null"/> on EOF. Don't forget to dispose this object after usage.</param>
        /// <returns>
        /// <see langword="true"/> - if a data block is returned,
        /// <see langword="false"/> - if the end of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the data block after the most recently returned data block for a particular track.
        /// </para><para>
        /// If a call was made to <see cref="TryGetPreviousDataBlock"/> which returned <see langword="false"/>, then the
        /// playback position is at the beginning of the recording and calling this method will return the first data block in the track.
        /// </para><para>
        /// The first call to this method after <see cref="Playback.SeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/>
        /// will return the data block in the recording closest to the seek time with a timestamp greater than or equal to the seek time.
        /// </para><para>
        /// This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...
        /// <see cref="IsBuiltIn"/> property can be used to determine if a track is a built-in track.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed playback object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        /// <exception cref="InvalidOperationException">This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...</exception>
        public bool TryGetNextDataBlock([NotNullWhen(returnValue: true)] out PlaybackDataBlock? dataBlock)
        {
            if (IsBuiltIn)
                throw new InvalidOperationException("This method cannot be called for built-in tracks like COLOR, DEPTH, etc.");

            var res = NativeApi.PlaybackGetNextDataBlock(PlaybackHandle, nameAsBytes, out var dataHandle);

            if (res == NativeCallResults.StreamResult.Eof)
            {
                dataBlock = null;
                return false;
            }

            if (res == NativeCallResults.StreamResult.Succeeded && dataHandle != null && !dataHandle.IsInvalid)
            {
                dataBlock = PlaybackDataBlock.Create(dataHandle)!;
                return true;
            }

            throw new PlaybackException(playback.FilePath);
        }

        /// <summary>Reads the previous data block for this track.</summary>
        /// <param name="dataBlock">Output: data block object on success or <see langword="null"/> on EOF. Don't forget to dispose this object after usage.</param>
        /// <returns>
        /// <see langword="true"/> - if a data block is returned,
        /// <see langword="false"/> - if the start of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the data block before the most recently returned data block for a particular track.
        /// </para><para>
        /// If a call was made to <see cref="TryGetNextDataBlock"/> which returned <see langword="false"/>, then the
        /// playback position is at the end of the recording and calling this method will return the last data block in the track.
        /// </para><para>
        /// The first call to this method after <see cref="Playback.SeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/>
        /// will return the data block in the recording closest to the seek time with a timestamp less than the seek time.
        /// </para><para>
        /// This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...
        /// <see cref="IsBuiltIn"/> property can be used to determine if a track is a built-in track.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed playback object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        /// <exception cref="InvalidOperationException">This method cannot be used with the built-in tracks: "COLOR", "DEPTH", etc...</exception>
        public bool TryGetPreviousDataBlock([NotNullWhen(returnValue: true)] out PlaybackDataBlock? dataBlock)
        {
            if (IsBuiltIn)
                throw new InvalidOperationException("This method cannot be called for built-in tracks like COLOR, DEPTH, etc.");

            var res = NativeApi.PlaybackGetPreviousDataBlock(PlaybackHandle, nameAsBytes, out var dataHandle);

            if (res == NativeCallResults.StreamResult.Eof)
            {
                dataBlock = null;
                return false;
            }

            if (res == NativeCallResults.StreamResult.Succeeded && dataHandle != null && !dataHandle.IsInvalid)
            {
                dataBlock = PlaybackDataBlock.Create(dataHandle)!;
                return true;
            }

            throw new PlaybackException(playback.FilePath);
        }

        private NativeHandles.PlaybackHandle PlaybackHandle => Playback.ToHandle(playback);
    }
}
