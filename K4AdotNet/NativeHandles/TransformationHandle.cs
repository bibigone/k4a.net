namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_transformation_t);
    /// <summary>Handle to an Azure Kinect transformation context.</summary>
    /// <remarks>Handles are created with <c>k4a_transformation_create()</c>.</remarks>
    internal sealed class TransformationHandle : HandleBase
    {
        private TransformationHandle()
        { }

        protected override bool ReleaseHandle()
        {
            DllImports.TransformationDestroy(handle);
            return true;
        }
    }
}
