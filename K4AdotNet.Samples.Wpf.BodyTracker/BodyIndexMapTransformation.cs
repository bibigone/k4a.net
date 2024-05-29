using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BodyIndexMapTransformation : IDisposable
    {
        private readonly Transformation transformation;
        private readonly Image transformedDepthMap;
        private readonly Image transformedBodyIndexMap;

        public BodyIndexMapTransformation(in Calibration calibration)
        {
            transformation = calibration.CreateTransformation();
            var colorWidth = calibration.ColorResolution.WidthPixels();
            var colorHeight = calibration.ColorResolution.HeightPixels();
            transformedDepthMap = calibration.IsOrbbec
                ? new Image.Orbbec(ImageFormat.Depth16, colorWidth, colorHeight)
                : new Image.Azure(ImageFormat.Depth16, colorWidth, colorHeight);
            transformedBodyIndexMap = calibration.IsOrbbec
                ? new Image.Orbbec(ImageFormat.Custom8, colorWidth, colorHeight, strideBytes: colorWidth, sizeBytes: colorWidth * colorHeight)
                : new Image.Azure(ImageFormat.Custom8, colorWidth, colorHeight, strideBytes: colorWidth, sizeBytes: colorWidth * colorHeight);
        }

        public void Dispose()
        {
            transformation.Dispose();
            transformedDepthMap.Dispose();
            transformedBodyIndexMap.Dispose();
        }

        public unsafe Image? ToColor(Image? depthMap, Image? bodyIndexMap)
        {
            if (bodyIndexMap == null || depthMap == null)
                return null;

            // Object can be disposed from different thread,
            // thus it is worth to keep references to images while we're working with their buffers
            using (var transformedDepthMapRef = transformedDepthMap.DuplicateReference())
            using (var transformedBodyIndexMapRef = transformedBodyIndexMap.DuplicateReference())
            {
                using var dm = depthMap.ConvertTo(transformation.IsOrbbec);
                using var bim = bodyIndexMap.ConvertTo(transformation.IsOrbbec);

                transformation.DepthImageToColorCameraCustom(
                    dm, bim,
                    transformedDepthMapRef, transformedBodyIndexMapRef,
                    TransformationInterpolation.Nearest, BodyTracking.BodyFrame.NotABodyIndexMapPixelValue);

                return transformedBodyIndexMapRef.DuplicateReference();
            }
        }
    }
}
