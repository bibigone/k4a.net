using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_record_t);
    //
    /// <summary>Handle to a Kinect for Azure recording opened for playback.</summary>
    internal abstract class PlaybackHandle : HandleBase
    {
        public abstract bool IsOrbbec { get; }

        public class Azure : PlaybackHandle
        {
            private Azure() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.PlaybackClose(handle);
                return true;
            }

            public override bool IsOrbbec => false;
        }

        public class Orbbec : PlaybackHandle
        {
            private Orbbec() { }

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    ReleaseOrbbecHandle(NativeApi.Orbbec.PlaybackClose);
                return true;
            }

            public override bool IsOrbbec => true;
        }
    }
}
