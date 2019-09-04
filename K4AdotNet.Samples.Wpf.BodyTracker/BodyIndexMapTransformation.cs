using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BodyIndexMapTransformation : IDisposable
    {
        private readonly Transformation transformation;
        private readonly Image bodyIndexMapCustom8;
        private readonly Image transformedDepthMap;
        private readonly Image transformedBodyIndexMap;

        public BodyIndexMapTransformation(ref Calibration calibration)
        {
            transformation = new Transformation(ref calibration);
            var depthWidth = calibration.DepthMode.WidthPixels();
            var depthHeight = calibration.DepthMode.HeightPixels();
            bodyIndexMapCustom8 = new Image(ImageFormat.Custom8, depthWidth, depthHeight);
            var colorWidth = calibration.ColorResolution.WidthPixels();
            var colorHeight = calibration.ColorResolution.HeightPixels();
            transformedDepthMap = new Image(ImageFormat.Depth16, colorWidth, colorHeight);
            transformedBodyIndexMap = new Image(ImageFormat.Custom8, colorWidth, colorHeight, strideBytes: colorWidth, sizeBytes: colorWidth * colorHeight);
        }

        public void Dispose()
        {
            transformation.Dispose();
            bodyIndexMapCustom8.Dispose();
            transformedDepthMap.Dispose();
            transformedBodyIndexMap.Dispose();
        }

        public unsafe Image ToColor(Image depthMap, Image bodyIndexMap)
        {
            if (bodyIndexMap == null || depthMap == null)
                return null;

            // Object can be disposed from different thread,
            // thus it is worth to keep references to images while we're working with their buffers
            using (var bodyIndexMapCustom8Ref = bodyIndexMapCustom8.DuplicateReference())
            {
                // Custom format -> Custom8 format
                // See issue https://github.com/microsoft/Azure-Kinect-Sensor-SDK/issues/704
                Buffer.MemoryCopy(
                    bodyIndexMap.Buffer.ToPointer(), bodyIndexMapCustom8Ref.Buffer.ToPointer(),
                    bodyIndexMapCustom8Ref.SizeBytes, bodyIndexMap.SizeBytes);

                using (var transformedDepthMapRef = transformedDepthMap.DuplicateReference())
                using (var transformedBodyIndexMapRef = transformedBodyIndexMap.DuplicateReference())
                {
                    transformation.DepthImageToColorCameraCustom(
                        depthMap, bodyIndexMapCustom8Ref,
                        transformedDepthMapRef, transformedBodyIndexMapRef,
                        TransformationInterpolation.Nearest, BodyTracking.BodyFrame.NotABodyIndexMapPixelValue);

                    return transformedBodyIndexMapRef.DuplicateReference();
                }
            }
        }
    }
}
