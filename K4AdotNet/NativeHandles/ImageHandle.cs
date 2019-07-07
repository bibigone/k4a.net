namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_image_t);
    /// <summary>Handle to an Azure Kinect image.</summary>
    /// <remarks>Images from a device are retrieved through a <c>k4a_capture_t</c> object returned by <c>k4a_device_get_capture()</c>.</remarks>
    internal sealed class ImageHandle : HandleBase
    {
        private ImageHandle()
        { }

        /// <summary>Call this method if you want to have one more reference to the same image.</summary>
        /// <returns>Additional reference to the same image. Don't forget to call <see cref="System.IDisposable.Dispose"/> method for object returned.</returns>
        public ImageHandle DuplicateReference()
        {
            NativeApi.ImageReference(handle);
            return new ImageHandle { handle = handle };
        }

        protected override bool ReleaseHandle()
        {
            NativeApi.ImageRelease(handle);
            return true;
        }
    }
}
