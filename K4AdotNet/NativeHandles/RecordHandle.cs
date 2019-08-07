namespace K4AdotNet.NativeHandles
{
    // Defined in record/types.h:
    // K4A_DECLARE_HANDLE(k4a_record_t);
    //
    /// <summary>Handle to a Kinect for Azure recording opened for writing.</summary>
    internal sealed class RecordHandle : HandleBase
    {
        private RecordHandle()
        { }

        protected override bool ReleaseHandle()
        {
            NativeApi.RecordClose(handle);
            return true;
        }

        public static readonly RecordHandle Zero = new RecordHandle();
    }
}
