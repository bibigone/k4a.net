using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    [Serializable]
    public class DeviceConnectionLostException : Exception
    {
        public DeviceConnectionLostException() : base("USB connection lost")
        { }

        public DeviceConnectionLostException(string message) : base(message)
        { }

        public DeviceConnectionLostException(string message, Exception innerException) : base(message, innerException)
        { }

        protected DeviceConnectionLostException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}