using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    [Serializable]
    public class DeviceNotFoundException : DeviceException
    {
        public DeviceNotFoundException()
            : this(Device.DefaultDeviceIndex)
        { }

        public DeviceNotFoundException(int deviceIndex)
            : base("Kinect device" + FormatDeviceIndex(deviceIndex) + " not found. Make sure that Kinect device is connected and has power supply.")
        {
            DeviceIndex = deviceIndex;
        }

        protected DeviceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}