namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_tracker_t);
    /// <summary>Handle to Azure Kinect body tracking component.</summary>
    internal sealed class TrackerHandle : HandleBase
    {
        private TrackerHandle()
        { }

        protected override bool ReleaseHandle()
        {
            NativeApi.TrackerDestroy(handle);
            return true;
        }

        public static readonly TrackerHandle Zero = new TrackerHandle();
    }
}
