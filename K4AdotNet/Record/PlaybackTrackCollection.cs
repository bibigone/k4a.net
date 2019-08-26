using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace K4AdotNet.Record
{
    /// <summary>Collection of information about all tracks in <see cref="Playback"/>, including built-in tracks.</summary>
    /// <seealso cref="Playback.Tracks"/>
    /// <seealso cref="PlaybackTrack"/>
    public sealed class PlaybackTrackCollection : IReadOnlyList<PlaybackTrack>
    {
        private readonly Playback playback;
        private readonly Lazy<PlaybackTrack>[] tracks;

        internal PlaybackTrackCollection(Playback playback)
        {
            this.playback = playback;

            var count = Helpers.UIntPtrToInt32(NativeApi.PlaybackGetTrackCount(PlaybackHandle));
            tracks = new Lazy<PlaybackTrack>[count];
            for (var i = 0; i < tracks.Length; i++)
            {
                var trackIndex = i;     // copy value of index to new variable to keep it value unchanged in the below closure
                tracks[i] = new Lazy<PlaybackTrack>(() => new PlaybackTrack(playback, trackIndex), isThreadSafe: true);
            }
        }

        /// <summary>Gets information about track in <see cref="Playback"/> by its zero-bases index.</summary>
        /// <param name="index">Zero-based index of track in <see cref="Playback"/>.</param>
        /// <returns>Information about track in <see cref="Playback"/> with a given index. Not <see langword="null"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is less than zero or is greater than or equal to <see cref="Count"/>.</exception>
        /// <exception cref="ObjectDisposedException">Appropriate <see cref="Playback"/> object was disposed.</exception>
        public PlaybackTrack this[int index] => tracks[index].Value;

        /// <summary>Gets information about track in <see cref="Playback"/> by its name.</summary>
        /// <param name="name">Name of track.</param>
        /// <returns>Information about track with a given name or <see langword="null"/> if there is no track with such name in <see cref="Playback"/>.</returns>
        /// <exception cref="ObjectDisposedException">Appropriate <see cref="Playback"/> object was disposed.</exception>
        /// <seealso cref="Exists(string)"/>
        public PlaybackTrack this[string name]
        {
            get
            {
                for (var i = 0; i < tracks.Length; i++)
                {
                    var track = tracks[i].Value;
                    if (track.Name == name)
                        return track;
                }
                return null;
            }
        }

        /// <summary>Get the number of tracks in a playback file.</summary>
        public int Count => tracks.Length;

        /// <summary>Implementation of <see cref="IEnumerable{PlaybackTrack}"/>.</summary>
        /// <returns>All tracks in this collection.</returns>
        /// <exception cref="ObjectDisposedException">Appropriate <see cref="Playback"/> object was disposed.</exception>
        public IEnumerator<PlaybackTrack> GetEnumerator()
        {
            for (var i = 0; i < tracks.Length; i++)
                yield return tracks[i].Value;
        }

        /// <summary>Implementation of <see cref="IEnumerable"/>.</summary>
        /// <returns>All tracks in this collection.</returns>
        /// <exception cref="ObjectDisposedException">Appropriate <see cref="Playback"/> object was disposed.</exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < tracks.Length; i++)
                yield return tracks[i].Value;
        }

        /// <summary>Checks whether a track with the given track name exists in the playback file.</summary>
        /// <param name="trackName">The track name to be checked to see whether it exists or not. Not <see langword="null"/> and not empty.</param>
        /// <returns><see langword="true"/> if the track exists.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="trackName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ObjectDisposedException">Appropriate <see cref="Playback"/> object was disposed.</exception>
        public bool Exists(string trackName)
        {
            if (string.IsNullOrEmpty(trackName))
                throw new ArgumentNullException(nameof(trackName));
            var trackNameAsBytes = Helpers.StringToBytes(trackName, Encoding.UTF8);
            return NativeApi.PlaybackCheckTrackExists(PlaybackHandle, trackNameAsBytes);
        }

        private NativeHandles.PlaybackHandle PlaybackHandle => Playback.ToHandle(playback);
    }
}
