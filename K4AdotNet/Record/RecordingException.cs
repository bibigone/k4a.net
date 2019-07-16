using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Record
{
    [Serializable]
    public class RecordingException : Exception
    {
        public RecordingException()
        { }

        public RecordingException(string message) : base(message)
        { }

        public RecordingException(string message, Exception innerException) : base(message, innerException)
        { }

        protected RecordingException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}