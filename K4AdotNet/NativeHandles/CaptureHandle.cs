namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_capture_t);
    /// <summary>Handle to an Azure Kinect capture.</summary>
    /// <remarks>
    /// Empty captures are created with <c>k4a_capture_create()</c>.
    /// Captures can be obtained from a device using <c>k4a_device_get_capture()</c>.
    /// </remarks>
    internal sealed class CaptureHandle : HandleBase, IReferenceDuplicatable<CaptureHandle>
    {
        private CaptureHandle()
        { }

        /// <summary>
        /// Call this method if you want to have one more reference to the same capture.
        /// </summary>
        /// <returns>Additional reference to the same capture. Don't forget to call <see cref="System.IDisposable.Dispose"/> method for object returned.</returns>
        public CaptureHandle DuplicateReference()
        {
            NativeApi.CaptureReference(handle);
            return new CaptureHandle { handle = handle };
        }

        protected override bool ReleaseHandle()
        {
            NativeApi.CaptureRelease(handle);
            return true;
        }
    }
}
