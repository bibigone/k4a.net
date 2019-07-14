using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Looks like this structure is unused in current version of Body Tracking SDK.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Body
    {
        /// <summary>An ID for the body that can be used for frame-to-frame correlation.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public BodyId Id;

        /// <summary>The skeleton information for the body.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Skeleton Skeleton;
    }
}
