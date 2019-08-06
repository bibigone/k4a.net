using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    [Serializable]
    public class DeviceException : Exception
    {
        public int DeviceIndex { get; set; }

        public DeviceException() : base("Kinect device error.")
        { }

        public DeviceException(string message) : base(message)
        { }

        public DeviceException(string message, Exception innerException) : base(message, innerException)
        { }

        protected DeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            DeviceIndex = info.GetInt32(nameof(DeviceIndex));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(DeviceIndex), DeviceIndex);
        }

        protected static string FormatDeviceIndex(int deviceIndex)
            => deviceIndex == Device.DefaultDeviceIndex ? string.Empty : $" #{deviceIndex + 1}";
    }
}