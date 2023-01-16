using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Record
{
    /// <summary>Class for all playback-related exceptions.</summary>
    /// <seealso cref="Playback"/>
    [Serializable]
    public class PlaybackException : Exception
    {
        /// <summary>File system path to recording.</summary>
        /// <seealso cref="Playback.FilePath"/>
        public string FilePath { get; }

        /// <summary>Creates exception with default message.</summary>
        /// <param name="filePath">File system path to recording.</param>
        public PlaybackException(string filePath) : base($"Error during reading from file \"{filePath}\".")
            => FilePath = filePath;

        /// <summary>Creates exception with specified message.</summary>
        /// <param name="message">Message for exception. Can be reached then via <see cref="Exception.Message"/> property.</param>
        /// <param name="filePath">File system path to recording.</param>
        public PlaybackException(string message, string filePath) : base(message)
            => FilePath = filePath;

        /// <summary>Constructor for deserialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected PlaybackException(SerializationInfo info, StreamingContext context) : base(info, context)
            => FilePath = info.GetString(nameof(FilePath)) ?? string.Empty;

        /// <summary>For serialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(FilePath), FilePath);
        }
    }
}