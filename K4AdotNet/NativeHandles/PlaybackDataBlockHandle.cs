using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_playback_data_block_t);
    //
    /// <summary>Handle to a block of data read from a <see cref="PlaybackHandle"/> custom track.</summary>
    internal abstract class PlaybackDataBlockHandle : HandleBase
    {
        public abstract bool IsOrbbec { get; }

        public class Azure : PlaybackDataBlockHandle
        {
            private Azure() { }

            public override bool IsOrbbec => false;

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Azure.PlaybackDataBlockRelease(handle);
                return true;
            }
        }

        public class Orbbec : PlaybackDataBlockHandle
        {
            private Orbbec() { }

            public override bool IsOrbbec => true;

            /// <inheritdoc cref="SafeHandle.ReleaseHandle"/>
            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                    NativeApi.Orbbec.PlaybackDataBlockRelease(handle);
                return true;
            }
        }
    }
}
