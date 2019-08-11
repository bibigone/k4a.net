using System;
using System.Runtime.Serialization;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Class for all body tracking related exceptions.</summary>
    /// <seealso cref="Tracker"/>
    [Serializable]
    public class BodyTrackingException : Exception
    {
        /// <summary>Creates exception with specified message.</summary>
        /// <param name="message">Message for exception. Can be reached then via <see cref="Exception.Message"/> property.</param>
        public BodyTrackingException(string message) : base(message)
        { }

        /// <summary>Constructor for deserialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected BodyTrackingException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}