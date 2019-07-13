using System;
using System.Runtime.Serialization;

namespace K4AdotNet.BodyTracking
{
    [Serializable]
    public class BodyTrackingException : Exception
    {
        public BodyTrackingException()
        { }

        public BodyTrackingException(string message) : base(message)
        { }

        public BodyTrackingException(string message, Exception innerException) : base(message, innerException)
        { }

        protected BodyTrackingException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}