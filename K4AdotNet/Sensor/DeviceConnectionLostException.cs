using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    [Serializable]
    public class DeviceConnectionLostException : DeviceException
    {
        public DeviceConnectionLostException()
            : this(Device.DefaultDeviceIndex)
        { }

        public DeviceConnectionLostException(int deviceIndex)
            : base("Connection lost with Kinect device" + FormatDeviceIndex(deviceIndex) + ". Check USB connection with device and power supply.")
        {
            DeviceIndex = deviceIndex;
        }

        protected DeviceConnectionLostException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}