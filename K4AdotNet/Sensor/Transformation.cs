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
        public Transformation(ref Calibration calibration)
        {
            var handle = NativeApi.TransformationCreate(ref calibration);
            if (handle == null || handle.IsInvalid)
            {
                throw new InvalidOperationException(
                    $"Cannot create transformation object from specified calibration data or {Sdk.DEPTHENGINE_DLL_NAME} library cannot be found).");
            }

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            DepthMode = calibration.DepthMode;
            ColorResolution = calibration.ColorResolution;
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

        /// <summary>Gets a value indicating whether the image has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler Disposed;

        /// <summary>Depth mode for which this transformation was created.</summary>
        public DepthMode DepthMode { get; }

        /// <summary>Resolution of color camera for which this transformation was created.</summary>
        public ColorResolution ColorResolution { get; }

        /// <summary>Transforms the depth map into the geometry of the color camera.</summary>
        /// <param name="depthImage">Input depth map to be transformed. Not <see langword="null"/>. Must have resolution of depth camera in <see cref="DepthMode"/> mode.</param>
        /// <param name="transformedDepthImage">Output depth image. Not <see langword="null"/>. Must have resolution of color camera in <see cref="ColorResolution"/> resolution.</param>
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
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
            CheckImageParameter(nameof(transformedDepthImage), transformedDepthImage, ImageFormat.Depth16, ColorResolution);

            var res = NativeApi.TransformationDepthImageToColorCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(transformedDepthImage));
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Failed to transform specified depth image to color camera.");
        }

        /// <summary>Transforms a color image into the geometry of the depth camera.</summary>
        /// <param name="depthImage">Input depth map. Not <see langword="null"/>. Must have resolution of depth camera in <see cref="DepthMode"/> mode.</param>
        /// <param name="colorImage">Input color image to be transformed. Not <see langword="null"/>. Must have resolution of color camera in <see cref="ColorResolution"/> resolution.</param>
        /// <param name="transformedColorImage">Output color image. Not <see langword="null"/>. Must have resolution of depth camera in <see cref="DepthMode"/> mode.</param>
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
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
            CheckImageParameter(nameof(colorImage), colorImage, ImageFormat.ColorBgra32, ColorResolution);
            CheckImageParameter(nameof(transformedColorImage), transformedColorImage, ImageFormat.ColorBgra32, DepthMode);

            var res = NativeApi.TransformationColorImageToDepthCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(colorImage), Image.ToHandle(transformedColorImage));
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Failed to transform specified color image to depth camera.");
        }

        /// <summary>Transforms the depth image into 3 planar images representing X, Y and Z-coordinates of corresponding 3D points.</summary>
        /// <param name="depthImage">Input depth image to be transformed to point cloud. Not <see langword="null"/>. Must have resolution of <paramref name="camera"/> camera.</param>
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
                CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
                CheckImageParameter(nameof(xyzImage), xyzImage, ImageFormat.Custom, DepthMode);
            }
            else if (camera == CalibrationGeometry.Color)
            {
                CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, ColorResolution);
                CheckImageParameter(nameof(xyzImage), xyzImage, ImageFormat.Custom, ColorResolution);
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

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat, int expectedWidth, int expectedHeight)
        {
            if (paramValue == null)
                throw new ArgumentNullException(paramName);
            if (paramValue.Format != expectedFormat)
                throw new ArgumentException($"{paramName} must have {expectedFormat} format but has {paramValue.Format}.", paramName);
            if (paramValue.WidthPixels != expectedWidth)
                throw new ArgumentException($"{paramName} must have {expectedWidth} width in pixels but has {paramValue.WidthPixels}.", paramName);
            if (paramValue.HeightPixels != expectedHeight)
                throw new ArgumentException($"{paramName} must have {expectedHeight} height pixels but has {paramValue.HeightPixels}.", paramName);
        }

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat, DepthMode depthMode)
            => CheckImageParameter(paramName, paramValue, expectedFormat, depthMode.WidthPixels(), depthMode.HeightPixels());

        private static void CheckImageParameter(string paramName, Image paramValue, ImageFormat expectedFormat, ColorResolution colorResolution)
            => CheckImageParameter(paramName, paramValue, expectedFormat, colorResolution.WidthPixels(), colorResolution.HeightPixels());
    }
}
