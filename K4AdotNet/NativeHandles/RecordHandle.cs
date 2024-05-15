using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_record_t);
    //
    /// <summary>Handle to a Kinect for Azure recording opened for writing.</summary>
    internal abstract class RecordHandle : HandleBase
    {
        public abstract bool IsOrbbec { get; }

        public class Azure : RecordHandle
        {
            private Azure() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.RecordClose(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : RecordHandle
        {
            private Orbbec() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Orbbec.RecordClose(handle);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
