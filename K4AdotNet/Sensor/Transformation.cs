using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>transformation</c> class from <c>k4a.hpp</c>
    public sealed class Transformation : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.TransformationHandle> handle;

        public Transformation(ref Calibration calibration)
        {
            var handle = NativeApi.TransformationCreate(ref calibration);
            if (handle == null || handle.IsInvalid)
                throw new ArgumentException("Cannot create transformation object from specified calibration data (or depthengine_1_0.dll library cannot be found).", nameof(calibration));
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

        public void Dispose()
            => handle.Dispose();

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public DepthMode DepthMode { get; }

        public ColorResolution ColorResolution { get; }

        public void DepthImageToColorCamera(Image depthImage, Image transformedDepthImage)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
            CheckImageParameter(nameof(transformedDepthImage), transformedDepthImage, ImageFormat.Depth16, ColorResolution);

            var res = NativeApi.TransformationDepthImageToColorCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(transformedDepthImage));
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Failed to transform specified depth image to color camera.");
        }

        public void ColorImageToDepthCamera(Image depthImage, Image colorImage, Image transformedColorImage)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
            CheckImageParameter(nameof(colorImage), colorImage, ImageFormat.ColorBgra32, ColorResolution);
            CheckImageParameter(nameof(transformedColorImage), transformedColorImage, ImageFormat.ColorBgra32, DepthMode);

            var res = NativeApi.TransformationColorImageToDepthCamera(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), Image.ToHandle(colorImage), Image.ToHandle(transformedColorImage));
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Failed to transform specified color image to depth camera.");
        }

        public void DepthImageToPointCloud(Image depthImage, CalibrationGeometry camera, Image xyzImage)
        {
            CheckImageParameter(nameof(depthImage), depthImage, ImageFormat.Depth16, DepthMode);
            CheckImageParameter(nameof(xyzImage), xyzImage, ImageFormat.Custom, DepthMode);
            if (xyzImage.StrideBytes < 3 * sizeof(short) * xyzImage.WidthPixels)
                throw new ArgumentException($"{xyzImage} must have a stride in bytes of at least 6 times its width in pixels.");
            if (!camera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(camera));

            var res = NativeApi.TransformationDepthImageToPointCloud(handle.ValueNotDisposed,
                Image.ToHandle(depthImage), camera, Image.ToHandle(xyzImage));
            if (res == NativeCallResults.Result.Failed)
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
