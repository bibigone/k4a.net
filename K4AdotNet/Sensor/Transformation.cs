using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>transformation</c> class from <c>k4a.hpp</c>
    //
    /// <summary>
    /// Azure Kinect transformation functionality.
    /// Can be used to transform images and depth maps between cameras and to reproject depth map to 3D space.
    /// It uses <see cref="Calibration"/> data for transformations.
    /// </summary>
    public sealed class Transformation : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.TransformationHandle> handle;
        private readonly Calibration calibration;

        /// <summary>
        /// Creates transformation object for a give calibration data.
        /// </summary>
        /// <param name="calibration">Camera calibration data.</param>
        /// <remarks><para>
        /// Each transformation instance requires some pre-computed resources to be allocated,
        /// which are retained until the call of <see cref="Dispose"/> method.
        /// </para><para>
        /// <see cref="Sdk.DEPTHENGINE_DLL_NAME"/> library is required for this functionality.
        /// </para></remarks>
        /// <exception cref="InvalidOperationException">Something wrong with calibration data or <see cref="Sdk.DEPTHENGINE_DLL_NAME"/> library cannot be loaded.</exception>
        /// <seealso cref="Dispose"/>
        public Transformation(in Calibration calibration)
        {
            var handle = NativeApi.TransformationCreate(in calibration);
            if (handle == null || handle.IsInvalid)
            {
                throw new InvalidOperationException(
                    $"Cannot create transformation object from specified calibration data or {Sdk.DEPTHENGINE_DLL_NAME} library cannot be found).");
            }

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            this.calibration = calibration;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to free all unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Calibration data for which this transformation was created.</summary>
        public Calibration Calibration => calibration;

        /// <summary>Depth mode for which this transformation was created.</summary>
        public DepthMode DepthMode => calibration.DepthMode;

        /// <summary>Resolution of color camera for which this transformation was created.</summary>
        public ColorResolution ColorResolution => calibration.ColorResolution;

        /// <summary>Transforms the depth map into the geometry of the color camera.</summary>
        /// <param name="depthImage">Input depth map to be transformed. Not <see langword="null"/>. Must have resolution of depth camera.</param>
        /// <param name="transformedDepthImage">Output depth image. Not <see langword="null"/>. Must have resolution of color camera.</param>
        /// <remarks>
        /// This produces a depth image for which each pixel matches the corresponding pixel coordinates of the color camera.
        /// The contents <paramref name="transformedDepthImage"/> will be filled with the depth values derived from <paramref name="depthImage"/> in the color
        /// camera's coordinate space.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="depthImage"/> is <see langword="null"/> or <paramref name="transformedDepthImage"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="depthImage"/> or <paramref name="transformedDepthImage"/> has invalid format or resolution.</exception>
        /// <exception cref="InvalidOperationException">Failed to transform specified depth image to color camera.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void DepthImageToColorCamera(Image depthImage, Image transformedDepthImage)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, calibration.DepthCameraCalibration);
            CheckImageParameter(nameof(transformedDepthImage), transformedDepthImage, ImageFormat.Depth16, calibration.ColorCameraCalibration);

            var res = NativeApi.TransformationDepthImageToColorCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(transformedDepthImage));
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Failed to transform specified depth image to color camera.");
        }

        /// <summary>Transforms the depth map into the geometry of the color camera.</summary>
        /// <param name="depthImage">Input depth map to be transformed. Not <see langword="null"/>. Must have resolution of depth camera.</param>
        /// <param name="customImage">Input custom image to be transformed. In <see cref="ImageFormat.Custom8"/> or <see cref="ImageFormat.Custom16"/> format. Not <see langword="null"/>. Must have resolution of depth camera.</param>
        /// <param name="transformedDepthImage">Output depth image. Not <see langword="null"/>. Must have resolution of color camera.</param>
        /// <param name="transformedCustomImage">Output custom image. Not <see langword="null"/>. Must have resolution of color camera and be of the same format as <paramref name="customImage"/>.</param>
        /// <param name="interpolation">
        /// Parameter that controls how pixels in <paramref name="customImage"/> should be interpolated when transformed to color camera space.
        /// <see cref="TransformationInterpolation.Linear"/> if linear interpolation should be used.
        /// <see cref="TransformationInterpolation.Nearest"/> if nearest neighbor interpolation should be used.
        /// </param>
        /// <param name="invalidCustomValue">
        /// Defines the custom image pixel value that should be written to <paramref name="transformedCustomImage"/> in case the corresponding
        /// depth pixel can not be transformed into the color camera space.
        /// </param>
        /// <remarks><para>
        /// This produces a depth image and a corresponding custom image for which each pixel matches the corresponding
        /// pixel coordinates of the color camera.
        /// </para><para>
        /// <paramref name="depthImage"/> and <paramref name="transformedDepthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// </para><para>
        /// <paramref name="customImage"/> and <paramref name="transformedCustomImage"/> must be of format <see cref="ImageFormat.Custom8"/> or
        /// <see cref="ImageFormat.Custom16"/>.
        /// </para><para>
        /// The contents <paramref name="transformedDepthImage"/> will be filled with the depth values derived from <paramref name="depthImage"/> in the color
        /// camera's coordinate space.
        /// </para><para>
        /// The contents <paramref name="transformedCustomImage"/> will be filled with the values derived from <paramref name="customImage"/> in the color
        /// camera's coordinate space.
        /// </para><para>
        /// Using linear interpolation could create new values to <paramref name="transformedCustomImage"/> which do no exist in <paramref name="customImage"/>.
        /// Setting <paramref name="interpolation"/> to <see cref="TransformationInterpolation.Nearest"/> will prevent this from happening but will result in less
        /// smooth image.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="depthImage"/> is <see langword="null"/>
        /// or <paramref name="customImage"/> is <see langword="null"/>
        /// or <paramref name="transformedDepthImage"/> is <see langword="null"/>
        /// or <paramref name="transformedCustomImage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="depthImage"/> or <paramref name="customImage"/> or <paramref name="transformedDepthImage"/> or <paramref name="transformedCustomImage"/>
        /// has invalid format or resolution.
        /// </exception>
        /// <exception cref="InvalidOperationException">Failed to transform specified depth and custom images to color camera.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void DepthImageToColorCameraCustom(
            Image depthImage, Image customImage,
            Image transformedDepthImage, Image transformedCustomImage,
            TransformationInterpolation interpolation, int invalidCustomValue)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, calibration.DepthCameraCalibration);
            CheckImageParameter(nameof(customImage), customImage, ImageFormat.Custom8, ImageFormat.Custom16, calibration.DepthCameraCalibration);
            CheckImageParameter(nameof(transformedDepthImage), transformedDepthImage, ImageFormat.Depth16, calibration.ColorCameraCalibration);
            CheckImageParameter(nameof(transformedCustomImage), transformedCustomImage, customImage.Format, calibration.ColorCameraCalibration);

            var res = NativeApi.TransformationDepthImageToColorCameraCustom(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(customImage),
                Image.ToHandle(transformedDepthImage), Image.ToHandle(transformedCustomImage),
                interpolation, invalidCustomValue);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Failed to transform specified depth and custom images to color camera.");
        }

        /// <summary>Transforms a color image into the geometry of the depth camera.</summary>
        /// <param name="depthImage">Input depth map. Not <see langword="null"/>. Must have resolution of depth camera.</param>
        /// <param name="colorImage">Input color image to be transformed. Not <see langword="null"/>. Must have resolution of color camera.</param>
        /// <param name="transformedColorImage">Output color image. Not <see langword="null"/>. Must have resolution of depth camera.</param>
        /// <remarks><para>
        /// This produces a color image for which each pixel matches the corresponding pixel coordinates of the depth camera.
        /// </para><para>
        /// <paramref name="depthImage"/> and <paramref name="colorImage"/> need to represent the same moment in time. The depth data will be applied to the
        /// color image to properly warp the color data to the perspective of the depth camera.
        /// </para><para>
        /// <paramref name="depthImage"/> must be of type <see cref="ImageFormat.Depth16"/>. <paramref name="colorImage"/> must be of format
        /// <see cref="ImageFormat.ColorBgra32"/>.
        /// </para><para>
        /// <paramref name="transformedColorImage"/> image must be of format <see cref="ImageFormat.ColorBgra32"/>.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="depthImage"/> is <see langword="null"/> or <paramref name="colorImage"/> is <see langword="null"/> or <paramref name="transformedColorImage"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="depthImage"/> or <paramref name="colorImage"/> or <paramref name="transformedColorImage"/> has invalid format or resolution.</exception>
        /// <exception cref="InvalidOperationException">Failed to transform specified color image to depth camera.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void ColorImageToDepthCamera(Image depthImage, Image colorImage, Image transformedColorImage)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, calibration.DepthCameraCalibration);
            CheckImageParameter(nameof(colorImage), colorImage, ImageFormat.ColorBgra32, calibration.ColorCameraCalibration);
            CheckImageParameter(nameof(transformedColorImage), transformedColorImage, ImageFormat.ColorBgra32, calibration.DepthCameraCalibration);

            var res = NativeApi.TransformationColorImageToDepthCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(colorImage), Image.ToHandle(transformedColorImage));
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Failed to transform specified color image to depth camera.");
        }

        /// <summary>Transforms the depth image into 3 planar images representing X, Y and Z-coordinates of corresponding 3D points.</summary>
        /// <param name="depthImage">Input depth image to be transformed to point cloud. Not <see langword="null"/>. Must have resolution.</param>
        /// <param name="camera">Geometry in which depth map was computed (<see cref="CalibrationGeometry.Depth"/> or <see cref="CalibrationGeometry.Color"/>).</param>
        /// <param name="xyzImage">Output XYZ image for point cloud data. Not <see langword="null"/>. Must have resolution of <paramref name="camera"/> camera.</param>
        /// <remarks><para>
        /// <paramref name="depthImage"/> must be of format <see cref="ImageFormat.Depth16"/>.
        /// </para><para>
        /// The <paramref name="camera"/> parameter tells the function what the perspective of the <paramref name="depthImage"/> is.
        /// If the <paramref name="depthImage"/> was captured directly from the depth camera, the value should be <see cref="CalibrationGeometry.Depth"/>.
        /// If the <paramref name="depthImage"/> is the result of a transformation into the color camera's coordinate space using
        /// <see cref="DepthImageToColorCamera(Image, Image)"/>, the value should be <see cref="CalibrationGeometry.Color"/>.
        /// </para><para>
        /// The format of <paramref name="xyzImage"/> must be <see cref="ImageFormat.Custom"/>. The width and height of <paramref name="xyzImage"/> must match the
        /// width and height of <paramref name="depthImage"/>. <paramref name="xyzImage"/> must have a stride in bytes of at least 6 times its width in pixels.
        /// </para><para>
        /// Each pixel of the <paramref name="xyzImage"/> consists of three <see cref="short"/> values, totaling 6 bytes. The three <see cref="short"/> values are the
        /// X, Y, and Z values of the point.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="camera"/> is not a camera.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="depthImage"/> is <see langword="null"/> or <paramref name="xyzImage"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="depthImage"/> or <paramref name="xyzImage"/> has invalid format or resolution.</exception>
        /// <exception cref="InvalidOperationException">Failed to transform specified depth image to point cloud.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void DepthImageToPointCloud(Image depthImage, CalibrationGeometry camera, Image xyzImage)
        {
            if (camera == CalibrationGeometry.Depth)
            {
                CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, calibration.DepthCameraCalibration);
                CheckImageParameter(nameof(xyzImage), xyzImage, ImageFormat.Custom, calibration.DepthCameraCalibration);
            }
            else if (camera == CalibrationGeometry.Color)
            {
                CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, calibration.ColorCameraCalibration);
                CheckImageParameter(nameof(xyzImage), xyzImage, ImageFormat.Custom, calibration.ColorCameraCalibration);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(camera));
            }

            if (xyzImage.StrideBytes < 3 * sizeof(short) * xyzImage.WidthPixels)
            {
                throw new ArgumentException($"{xyzImage} must have a stride in bytes of at least 6 times its width in pixels.");
            }

            var res = NativeApi.TransformationDepthImageToPointCloud(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), camera, Image.ToHandle(xyzImage));
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"Failed to transform specified depth image to point cloud in coordinates of {camera} camera.");
        }

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat1, ImageFormat expectedFormat2, int expectedWidth, int expectedHeight)
        {
            if (paramValue == null)
                throw new ArgumentNullException(paramName);
            if (paramValue.Format != expectedFormat1 && paramValue.Format != expectedFormat2)
                throw new ArgumentException($"{paramName} must have {CombineExpectedFormatsForMessage(expectedFormat1, expectedFormat2)} format but has {paramValue.Format}.", paramName);
            if (paramValue.WidthPixels != expectedWidth)
                throw new ArgumentException($"{paramName} must have {expectedWidth} width in pixels but has {paramValue.WidthPixels}.", paramName);
            if (paramValue.HeightPixels != expectedHeight)
                throw new ArgumentException($"{paramName} must have {expectedHeight} height pixels but has {paramValue.HeightPixels}.", paramName);
        }

        private static string CombineExpectedFormatsForMessage(ImageFormat format1, ImageFormat format2)
            => format1 == format2 ? format1.ToString() : (format1.ToString() + " or " + format2.ToString());

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat, CameraCalibration cameraCalibration)
            => CheckImageParameter(paramName, paramValue, expectedFormat, expectedFormat, cameraCalibration.ResolutionWidth, cameraCalibration.ResolutionHeight);

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat1, ImageFormat expectedFormat2, CameraCalibration cameraCalibration)
            => CheckImageParameter(paramName, paramValue, expectedFormat1, expectedFormat2, cameraCalibration.ResolutionWidth, cameraCalibration.ResolutionHeight);
    }
}
