using System;
using System.Runtime.Serialization;

namespace K4AdotNet.Sensor
{
    /// <summary>Base class for all device-related exceptions.</summary>
    /// <seealso cref="Device"/>
    [Serializable]
    public class DeviceException : Exception
    {
        /// <summary>Zero-based index of device.</summary>
        /// <seealso cref="Device.DeviceIndex"/>
        public int DeviceIndex { get; }

        /// <summary>Creates exception with specified message.</summary>
        /// <param name="message">Message for exception. Can be reached then via <see cref="Exception.Message"/> property..</param>
        /// <param name="deviceIndex">Zero-based index of device.</param>
        public DeviceException(string message, int deviceIndex) : base(message)
            => DeviceIndex = deviceIndex;

        /// <summary>Constructor for deserialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected DeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
            => DeviceIndex = info.GetInt32(nameof(DeviceIndex));

        /// <summary>For serialization needs.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(DeviceIndex), DeviceIndex);
        }

        /// <summary>Helper method for inheritors. Formats device index to optional device number.</summary>
        /// <param name="deviceIndex">Zero-based index of device.</param>
        /// <returns>
        /// Empty string if <paramref name="deviceIndex"/> is equal to <see cref="Device.DefaultDeviceIndex"/>,
        /// <c> #{deviceIndex+1}</c> otherwise.</returns>
        protected static string FormatDeviceIndex(int deviceIndex)
            => deviceIndex == Device.DefaultDeviceIndex ? string.Empty : $" #{deviceIndex + 1}";
    }
}