using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    /// <summary>Interops to some native functions from <c>k4a.h</c> header file.</summary>
    internal static class DllImports
    {
        // K4A_EXPORT void k4a_capture_reference(k4a_capture_t capture_handle);
        /// <summary>Add a reference to a capture.</summary>
        /// <param name="captureHandle">Capture to add a reference to.</param>
        /// <remarks>Call this function to add an additional reference to a capture.
        ///     This reference must be removed with <see cref="CaptureReference(IntPtr)"/>.</remarks>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_capture_reference", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureReference(IntPtr captureHandle);

        // K4A_EXPORT void k4a_capture_release(k4a_capture_t capture_handle);
        /// <summary>Release a capture.</summary>
        /// <param name="captureHandle">Capture to release.</param>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_capture_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CaptureRelease(IntPtr captureHandle);

        // K4A_EXPORT void k4a_image_reference(k4a_image_t image_handle);
        /// <summary>Add a reference to the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <remarks>
        /// References manage the lifetime of the object. When the references reach zero the object is destroyed. A caller must
        /// not access the object after its reference is released.
        /// </remarks>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_image_reference", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageReference(IntPtr imageHandle);

        // K4A_EXPORT void k4a_image_release(k4a_image_t image_handle);
        /// <summary>Remove a reference from the image.</summary>
        /// <param name="imageHandle">Handle of the image for which the get operation is performed on.</param>
        /// <remarks>
        /// References manage the lifetime of the object. When the references reach zero the object is destroyed. A caller must
        /// not access the object after its reference is released.
        /// </remarks>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_image_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImageRelease(IntPtr imageHandle);

        // K4A_EXPORT void k4a_transformation_destroy(k4a_transformation_t transformation_handle);
        /// <summary>Destroy transformation handle.</summary>
        /// <param name="transformationHandle">Transformation handle to destroy.</param>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_transformation_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TransformationDestroy(IntPtr transformationHandle);

        // K4A_EXPORT void k4a_device_close(k4a_device_t device_handle);
        /// <summary>Closes an Azure Kinect device.</summary>
        /// <param name="deviceHandle">Handle of device for which the get operation is performed on.</param>
        /// <remarks>Once closed, the handle is no longer valid.</remarks>
        [DllImport(Sdk.K4A_DLL_NAME, EntryPoint = "k4a_device_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeviceClose(IntPtr deviceHandle);
    }
}
