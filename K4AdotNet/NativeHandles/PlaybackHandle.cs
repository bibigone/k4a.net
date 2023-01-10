namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_record_t);
    //
    /// <summary>Handle to a Kinect for Azure recording opened for playback.</summary>
    internal sealed class PlaybackHandle : HandleBase
    {
        private PlaybackHandle()
        { }

        protected override bool ReleaseHandle()
        {
            NativeApi.PlaybackClose(handle);
            return true;
        }

        public static readonly PlaybackHandle Zero = new();
    }
}
