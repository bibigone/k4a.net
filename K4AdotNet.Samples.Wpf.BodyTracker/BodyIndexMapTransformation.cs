using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BodyIndexMapTransformation : IDisposable
    {
        private readonly Transformation transformation;
        private readonly Image transformedDepthMap;
        private readonly Image transformedBodyIndexMap;

        public BodyIndexMapTransformation(ref Calibration calibration)
        {
            transformation = new Transformation(ref calibration);
            var colorWidth = calibration.ColorResolution.WidthPixels();
            var colorHeight = calibration.ColorResolution.HeightPixels();
            transformedDepthMap = new Image(ImageFormat.Depth16, colorWidth, colorHeight);
            transformedBodyIndexMap = new Image(ImageFormat.Custom8, colorWidth, colorHeight, strideBytes: colorWidth, sizeBytes: colorWidth * colorHeight);
        }

        public void Dispose()
        {
            transformation.Dispose();
            transformedDepthMap.Dispose();
            transformedBodyIndexMap.Dispose();
        }

        public Image ToColor(Image depthMap, Image bodyIndexMap)
        {
            if (bodyIndexMap == null || depthMap == null)
                return null;

            // Object can be disposed from different thread,
            // thus it is worth to keep references to images while we're working with their buffers
            using (var transformedDepthMapRef = transformedDepthMap.DuplicateReference())
            using (var transformedBodyIndexMapRef = transformedBodyIndexMap.DuplicateReference())
            {
                transformation.DepthImageToColorCameraCustom(
                    depthMap, bodyIndexMap,
                    transformedDepthMap, transformedBodyIndexMap,
                    TransformationInterpolation.Nearest, BodyTracking.BodyFrame.NotABodyIndexMapPixelValue);
                return transformedBodyIndexMapRef.DuplicateReference();
            }
        }
    }
}
