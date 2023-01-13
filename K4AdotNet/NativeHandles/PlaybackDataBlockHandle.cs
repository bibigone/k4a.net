using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_playback_data_block_t);
    //
    /// <summary>Handle to a block of data read from a <see cref="PlaybackHandle"/> custom track.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct PlaybackDataBlockHandle : INativeHandle
    {
        private readonly IntPtr value;

        /// <inheritdoc cref="INativeHandle.UnsafeValue"/>
        IntPtr INativeHandle.UnsafeValue => value;

        /// <inheritdoc cref="INativeHandle.IsValid"/>
        public bool IsValid => value != IntPtr.Zero;

        /// <inheritdoc cref="INativeHandle.Release"/>
        public void Release()
        {
            if (IsValid)
                NativeApi.PlaybackDataBlockRelease(this);
        }
    }
}
