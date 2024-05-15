using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    partial class NativeApi
    {
        public sealed class Orbbec : NativeApi
        {
            public static readonly Orbbec Instance = new();

            public override bool IsOrbbec => true;

            public override NativeHandles.ImageHandle? FrameGetBodyIndexMap(NativeHandles.BodyFrameHandle bodyFrameHandle)
                => k4abt_frame_get_body_index_map(bodyFrameHandle);

            // K4ABT_EXPORT k4a_image_t k4abt_frame_get_body_index_map(k4abt_frame_t body_frame_handle);
            [DllImport(Sdk.BODY_TRACKING_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.ImageHandle.Orbbec? k4abt_frame_get_body_index_map(NativeHandles.BodyFrameHandle bodyFrameHandle);

            public override NativeHandles.CaptureHandle? FrameGetCapture(NativeHandles.BodyFrameHandle bodyFrameHandle)
                => k4abt_frame_get_capture(bodyFrameHandle);

            // K4ABT_EXPORT k4a_capture_t k4abt_frame_get_capture(k4abt_frame_t body_frame_handle);
            [DllImport(Sdk.BODY_TRACKING_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeHandles.CaptureHandle.Orbbec? k4abt_frame_get_capture(NativeHandles.BodyFrameHandle bodyFrameHandle);
        }
    }
}
