namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_playback_data_block_t);
    //
    /// <summary>Handle to a block of data read from a <see cref="PlaybackHandle"/> custom track.</summary>
    internal sealed class PlaybackDataBlockHandle : HandleBase
    {
        private PlaybackDataBlockHandle()
        { }

        protected override bool ReleaseHandle()
        {
            NativeApi.PlaybackDataBlockRelease(handle);
            return true;
        }

        public static readonly PlaybackDataBlockHandle Zero = new();
    }
}
