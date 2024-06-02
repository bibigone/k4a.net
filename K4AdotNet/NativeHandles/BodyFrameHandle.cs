using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_frame_t);
    //
    /// <summary>Handle to an Azure Kinect body tracking frame.</summary>
    internal class BodyFrameHandle : HandleBase, IReferenceDuplicatable<BodyFrameHandle>
    {
        private BodyFrameHandle()
        { }

        /// <inheritdoc cref="IReferenceDuplicatable{BodyFrameHandle}.DuplicateReference"/>
        public BodyFrameHandle DuplicateReference()
        {
            if (IsInvalid)
                throw new InvalidOperationException("Invalid handle");
            NativeApi.FrameReference(this);
            return new() { handle = handle };
        }

        /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                if (Sdk.ComboMode == ComboMode.Azure)
                    NativeApi.FrameRelease(handle);
                else
                    ReleaseOrbbecHandle(NativeApi.FrameRelease);
            }
            return true;
        }
    }
}
