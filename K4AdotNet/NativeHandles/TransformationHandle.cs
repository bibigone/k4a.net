using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_transformation_t);
    //
    /// <summary>Handle to an Azure Kinect transformation context.</summary>
    /// <remarks>Handles are created with <c>k4a_transformation_create()</c>.</remarks>
    internal abstract class TransformationHandle : HandleBase
    {
        public abstract bool IsOrbbec { get; }

        public class Azure : TransformationHandle
        {
            private Azure() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.TransformationDestroy(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : TransformationHandle
        {
            private Orbbec() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    ReleaseOrbbecHandle(NativeApi.Orbbec.TransformationDestroy);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
