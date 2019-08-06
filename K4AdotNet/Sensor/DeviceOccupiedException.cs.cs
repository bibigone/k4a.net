using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    [Serializable]
    public class DeviceOccupiedException : DeviceException
    {
        public DeviceOccupiedException()
            : this(Device.DefaultDeviceIndex)
        { }

        public DeviceOccupiedException(int deviceIndex)
            : base("Cannot connect to Kinect device" + FormatDeviceIndex(deviceIndex) +". Possibly, it is occupied by another software.")
        {
            DeviceIndex = deviceIndex;
        }

        protected DeviceOccupiedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}