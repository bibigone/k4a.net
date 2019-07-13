using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Record
{
    [Serializable]
    internal class PlaybackException : Exception
    {
        public PlaybackException()
        {
        }

        public PlaybackException(string message) : base(message)
        {
        }

        public PlaybackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlaybackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}