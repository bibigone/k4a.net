namespace K4AdotNet.NativeHandles
{
    // Defined in k4abttypes.h:
    // K4A_DECLARE_HANDLE(k4abt_frame_t);
    /// <summary>Handle to an Azure Kinect body tracking frame.</summary>
    internal sealed class BodyFrameHandle : HandleBase
    {
        private BodyFrameHandle()
        { }

        /// <summary>Call this method if you want to have one more reference to the same body frame.</summary>
        /// <returns>Additional reference to the same body frame. Don't forget to call <see cref="System.IDisposable.Dispose"/> method for object returned.</returns>
        public BodyFrameHandle DuplicateReference()
        {
            NativeApi.FrameReference(handle);
            return new BodyFrameHandle { handle = handle };
        }

        protected override bool ReleaseHandle()
        {
            NativeApi.FrameRelease(handle);
            return true;
        }
    }
}
