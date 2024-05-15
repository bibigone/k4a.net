using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    /// <summary>Exception: device is occupied or device has insufficient power supply.</summary>
    /// <seealso cref="Device.Open(int)"/>
    [Serializable]
    public class DeviceOccupiedException : DeviceException
    {
        /// <summary>Creates exception for device with specified index.</summary>
        /// <param name="deviceIndex">Zero-based device index. Can be reached then via <see cref="DeviceException.DeviceIndex"/> property.</param>
        /// <param name="isOrbbec"><see langword="true"/> for ORBBEC devices, <see langword="false"/> for AZURE Kinect devices.</param>
        /// <seealso cref="Device.DeviceIndex"/>
        public DeviceOccupiedException(int deviceIndex, bool? isOrbbec = null)
            : base("Cannot open " + Helpers.GetDeviceModelName(isOrbbec) + " device" + FormatDeviceIndex(deviceIndex) + ". Possibly, the device is occupied by another software or has insufficient power supply.",
                  deviceIndex)
        { }

        /// <summary>Constructor for deserialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected DeviceOccupiedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}