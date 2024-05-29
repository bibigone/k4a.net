using System;
using System.Diagnostics;

namespace K4AdotNet.Sensor
{
    partial class Capture
    {
        /// <summary>
        /// Implementation of base <see cref="Capture"/> class for Azure Kinect devices.
        /// This class works via `original K4A` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Azure"/> and <see cref="ComboMode.Both"/>.</remarks>
        public class Azure : Capture
        {
            /// <summary>Creates an empty capture object for Azure implementation.</summary>
            /// <exception cref="InvalidOperationException">
            /// Sensor SDK fails to create empty capture object for some reason.
            /// Or the library is not initialized.
            /// Or the library is initialized in incompatible mode.
            /// </exception>
            public Azure()
                : base(CreateCaptureHandle(NativeApi.Azure.Instance))
            { }

            internal Azure(NativeHandles.CaptureHandle handle)
                : base(handle)
            {
                Debug.Assert(handle is NativeHandles.CaptureHandle.Azure);
            }

            /// <inheritdoc cref="Capture.DuplicateReference"/>
            public override Capture DuplicateReference()
                => new Azure(handle.ValueNotDisposed.DuplicateReference());

            /// <summary>Get and set the temperature associated with the capture, in Celsius.</summary>
            /// <remarks>
            /// This function returns the temperature of the device at the time of the capture in Celsius. If
            /// the temperature is unavailable, the function will return <see cref="float.NaN"/>.
            /// </remarks>
            /// <exception cref="ObjectDisposedException">This property cannot be called for disposed objects.</exception>
            public float TemperatureC
            {
                get => NativeApi.Azure.Instance.CaptureGetTemperatureC(handle.ValueNotDisposed);
                set => NativeApi.Azure.Instance.CaptureSetTemperatureC(handle.ValueNotDisposed, value);
            }
        }
    }
}
