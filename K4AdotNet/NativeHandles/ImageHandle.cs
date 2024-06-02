using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_image_t);
    //
    /// <summary>Handle to an Azure Kinect image.</summary>
    /// <remarks>Images from a device are retrieved through a <c>k4a_capture_t</c> object returned by <c>k4a_device_get_capture()</c>.</remarks>
    internal abstract class ImageHandle : HandleBase, IReferenceDuplicatable<ImageHandle>
    {
        private ImageHandle() { }

        /// <inheritdoc cref="IReferenceDuplicatable{ImageHandle}.DuplicateReference"/>
        public abstract ImageHandle DuplicateReference();

        public abstract bool IsOrbbec { get; }

        public class Azure : ImageHandle
        {
            public static readonly Azure Zero = new() { handle = IntPtr.Zero };

            private Azure() { }

            /// <inheritdoc cref="IReferenceDuplicatable{ImageHandle}.DuplicateReference"/>
            public override ImageHandle DuplicateReference()
            {
                if (IsInvalid)
                    throw new InvalidOperationException("Invalid handle");
                NativeApi.Azure.ImageReference(this);
                return new Azure() { handle = handle };
            }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.ImageRelease(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : ImageHandle
        {
            public static readonly Orbbec Zero = new() { handle = IntPtr.Zero };

            private Orbbec() { }

            /// <inheritdoc cref="IReferenceDuplicatable{ImageHandle}.DuplicateReference"/>
            public override ImageHandle DuplicateReference()
            {
                if (IsInvalid)
                    throw new InvalidOperationException("Invalid handle");
                NativeApi.Orbbec.ImageReference(this);
                return new Orbbec() { handle = handle };
            }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    ReleaseOrbbecHandle(NativeApi.Orbbec.ImageRelease, wait: true);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
