using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class BodyIndexMapTransformation : IDisposable
    {
        private readonly Transformation transformation;
        private readonly Image maskedDepthMap;
        private readonly Image transformedMaskedDepthMap;
        private readonly Image transformedBodyIndexMap;

        public BodyIndexMapTransformation(ref Calibration calibration)
        {
            transformation = new Transformation(ref calibration);
            maskedDepthMap = new Image(ImageFormat.Depth16, calibration.DepthMode.WidthPixels(), calibration.DepthMode.HeightPixels());
            var colorWidth = calibration.ColorResolution.WidthPixels();
            var colorHeight = calibration.ColorResolution.HeightPixels();
            transformedMaskedDepthMap = new Image(ImageFormat.Depth16, colorWidth, colorHeight);
            transformedBodyIndexMap = new Image(ImageFormat.Custom, colorWidth, colorHeight, strideBytes: colorWidth, sizeBytes: colorWidth * colorHeight);
        }

        public void Dispose()
        {
            transformation.Dispose();
            maskedDepthMap.Dispose();
            transformedMaskedDepthMap.Dispose();
            transformedBodyIndexMap.Dispose();
        }

        public Image ToColor(Image depthMap, Image bodyIndexMap, int bodyCount)
        {
            if (bodyIndexMap == null || depthMap == null)
                return null;

            // Object can be disposed from different thread, thus it is worth to keep references to images while we're working with their buffers
            using (var maskedDepthMapRef = maskedDepthMap.DuplicateReference())
            using (var transformedMaskedDepthMapRef = transformedMaskedDepthMap.DuplicateReference())
            using (var transformedBodyIndexMapRef = transformedBodyIndexMap.DuplicateReference())
            {
                ClearTransformedBodyIndex(transformedBodyIndexMapRef);

                for (var bodyIndex = 0; bodyIndex < bodyCount; bodyIndex++)
                {
                    ApplyBodyMask(depthMap, bodyIndexMap, bodyIndex, maskedDepthMapRef);
                    transformation.DepthImageToColorCamera(maskedDepthMapRef, transformedMaskedDepthMapRef);
                    FillBodyIndexMapFromMaskedDepth(transformedMaskedDepthMapRef, bodyIndex, transformedBodyIndexMapRef);
                }

                return transformedBodyIndexMapRef.DuplicateReference();
            }
        }

        private static unsafe void FillBodyIndexMapFromMaskedDepth(Image maskedDepthMap, int bodyIndex, Image bodyIndexMap)
        {
            var height = maskedDepthMap.HeightPixels;
            var width = maskedDepthMap.WidthPixels;
            for (var y = 0; y < height; y++)
            {
                ushort* srcDepthPtr = (ushort*)(maskedDepthMap.Buffer + maskedDepthMap.StrideBytes * y).ToPointer();
                byte* bodyIndexPtr = (byte*)(bodyIndexMap.Buffer + bodyIndexMap.StrideBytes * y).ToPointer();
                for (var x = 0; x < width; x++)
                {
                    var srcDepth = *(srcDepthPtr++);
                    if (srcDepth != 0)
                        *bodyIndexPtr = (byte)bodyIndex;
                    bodyIndexPtr++;
                }
            }
        }

        private static unsafe void ApplyBodyMask(Image depthMap, Image bodyIndexMap, int bodyIndex, Image maskedDepthMap)
        {
            var height = depthMap.HeightPixels;
            var width = depthMap.WidthPixels;
            for (var y = 0; y < height; y++)
            {
                ushort* srcDepthPtr = (ushort*)(depthMap.Buffer + depthMap.StrideBytes * y).ToPointer();
                byte* bodyIndexPtr = (byte*)(bodyIndexMap.Buffer + bodyIndexMap.StrideBytes * y).ToPointer();
                ushort* dstDepthPtr = (ushort*)(maskedDepthMap.Buffer + maskedDepthMap.StrideBytes * y).ToPointer();
                for (var x = 0; x < width; x++)
                {
                    var srcDepth = *(srcDepthPtr++);
                    var indx = *(bodyIndexPtr++);
                    *(dstDepthPtr++) = indx == bodyIndex ? srcDepth : ushort.MinValue;
                }
            }
        }

        private static unsafe void ClearTransformedBodyIndex(Image transformedBodyIndexMap)
        {
            byte* ptr = (byte*)transformedBodyIndexMap.Buffer.ToPointer();
            var size = transformedBodyIndexMap.SizeBytes;
            for (var i = 0; i < size; i++)
                *(ptr++) = BodyTracking.BodyFrame.NotABodyIndexMapPixelValue;
        }
    }
}
