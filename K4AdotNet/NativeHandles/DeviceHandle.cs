using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_device_t);
    //
    /// <summary>Handle to an Azure Kinect device.</summary>
    internal abstract class DeviceHandle : HandleBase
    {
        public abstract bool IsOrbbec { get; }

        public class Azure : DeviceHandle
        {
            public static readonly Azure Zero = new() { handle = System.IntPtr.Zero };

            private Azure() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.DeviceClose(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : DeviceHandle
        {
            public static readonly Orbbec Zero = new() { handle = System.IntPtr.Zero };

            private Orbbec() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    ReleaseOrbbecHandle(NativeApi.Orbbec.DeviceClose);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
