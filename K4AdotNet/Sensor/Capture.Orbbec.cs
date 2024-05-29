using System;
using System.Diagnostics;

namespace K4AdotNet.Sensor
{
    partial class Capture
    {
        /// <summary>
        /// Implementation of base <see cref="Capture"/> class for Orbbec Femto devices.
        /// This class works via `Orbbec SDK K4A Wrapper` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Orbbec"/> and <see cref="ComboMode.Both"/>.</remarks>
        public class Orbbec : Capture
        {
            /// <summary>Creates an empty capture object for Orbbec implementation.</summary>
            /// <exception cref="InvalidOperationException">
            /// Sensor SDK fails to create empty capture object for some reason.
            /// Or the library is not initialized.
            /// Or the library is initialized in incompatible mode.
            /// </exception>
            public Orbbec()
                : base(CreateCaptureHandle(NativeApi.Orbbec.Instance))
            { }

            internal Orbbec(NativeHandles.CaptureHandle handle)
                : base(handle)
            {
                Debug.Assert(handle is NativeHandles.CaptureHandle.Orbbec);
            }

            /// <inheritdoc cref="Capture.DuplicateReference"/>
            public override Capture DuplicateReference()
                => new Orbbec(handle.ValueNotDisposed.DuplicateReference());
        }
    }
}
