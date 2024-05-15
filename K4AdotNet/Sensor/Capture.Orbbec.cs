using System;
using System.Diagnostics;

namespace K4AdotNet.Sensor
{
    partial class Capture
    {
        public class Orbbec : Capture
        {
            /// <summary>Creates an empty capture object.</summary>
            /// <exception cref="InvalidOperationException">
            /// Sensor SDK fails to create empty capture object for some reason. For details see logs.
            /// </exception>
            public Orbbec()
                : base(CreateCaptureHandle(NativeApi.Orbbec.Instance))
            { }

            internal Orbbec(NativeHandles.CaptureHandle handle)
                : base(handle)
            {
                Debug.Assert(handle is NativeHandles.CaptureHandle.Orbbec);
            }

            public override Capture DuplicateReference()
                => new Orbbec(handle.ValueNotDisposed.DuplicateReference());
        }
    }
}
