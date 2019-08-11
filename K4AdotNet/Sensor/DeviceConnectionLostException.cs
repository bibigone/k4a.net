using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    /// <summary>Exception: connection lost with Azure Kinect device.</summary>
    /// <seealso cref="Device"/>
    [Serializable]
    public class DeviceConnectionLostException : DeviceException
    {
        /// <summary>Creates exception for device with specified index.</summary>
        /// <param name="deviceIndex">Zero-based device index. Can be reached then via <see cref="DeviceException.DeviceIndex"/> property.</param>
        /// <seealso cref="Device.DeviceIndex"/>
        public DeviceConnectionLostException(int deviceIndex)
            : base("Connection lost with Azure Kinect device" + FormatDeviceIndex(deviceIndex) + ". Check USB connection with device and power supply.",
                  deviceIndex)
        { }

        /// <summary>Constructor for deserialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected DeviceConnectionLostException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}