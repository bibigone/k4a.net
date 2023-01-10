namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // K4A_DECLARE_HANDLE(k4a_device_t);
    //
    /// <summary>Handle to an Azure Kinect device.</summary>
    internal sealed class DeviceHandle : HandleBase
    {
        private DeviceHandle()
        { }

        protected override bool ReleaseHandle()
        {
            NativeApi.DeviceClose(handle);
            return true;
        }

        public static readonly DeviceHandle Zero = new();
    }
}
