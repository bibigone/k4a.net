using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_capture_t);
    //
    /// <summary>Handle to an Azure Kinect capture.</summary>
    /// <remarks>
    /// Empty captures are created with <c>k4a_capture_create()</c>.
    /// Captures can be obtained from a device using <c>k4a_device_get_capture()</c>.
    /// </remarks>
    internal abstract class CaptureHandle : HandleBase, IReferenceDuplicatable<CaptureHandle>
    {
        /// <inheritdoc cref="IReferenceDuplicatable{CaptureHandle}.DuplicateReference"/>
        public abstract CaptureHandle DuplicateReference();

        public abstract bool IsOrbbec { get; }

        public class Azure : CaptureHandle
        {
            private Azure() { }

            /// <inheritdoc cref="IReferenceDuplicatable{CaptureHandle}.DuplicateReference"/>
            public override CaptureHandle DuplicateReference()
            {
                if (IsInvalid)
                    throw new InvalidOperationException("Invalid handle");
                NativeApi.Azure.CaptureReference(this);
                return new Azure() { handle = handle };
            }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.CaptureRelease(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : CaptureHandle
        {
            private Orbbec() { }

            /// <inheritdoc cref="IReferenceDuplicatable{CaptureHandle}.DuplicateReference"/>
            public override CaptureHandle DuplicateReference()
            {
                if (IsInvalid)
                    throw new InvalidOperationException("Invalid handle");
                NativeApi.Orbbec.CaptureReference(this);
                return new Orbbec() { handle = handle };
            }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    ReleaseOrbbecHandle(NativeApi.Orbbec.CaptureRelease);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
