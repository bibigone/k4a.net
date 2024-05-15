using K4AdotNet.Samples.Wpf.Common;
using K4AdotNet.Sensor;
using System;
using System.Buffers;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    /// <summary>
    /// Frame data for processing.
    /// </summary>
    /// <remarks>
    /// Contains synchronized color and depth frames.
    /// </remarks>
    internal class FrameData : IDisposable
    {
        public FrameData(Image colorFrame, Image depthFrame)
        {
            ColorFrame = DecodeColorImageIfNeeded(colorFrame) ?? throw new ArgumentNullException(nameof(colorFrame));
            DepthFrame = depthFrame?.DuplicateReference() ?? throw new ArgumentNullException(nameof(depthFrame));
        }

        public void Dispose()
        {
            ColorFrame.Dispose();
            DepthFrame.Dispose();
        }

        public Image ColorFrame { get; }
        public Image DepthFrame { get; }

        private static Image? DecodeColorImageIfNeeded(Image? colorFrame)
        {
            if (colorFrame == null)
                return null;

            if (colorFrame.Format == ImageFormat.ColorMjpg)
            {
                var bgraImageSize = ImageFormat.ColorBgra32.StrideBytes(colorFrame.WidthPixels) * colorFrame.HeightPixels;
                var bgraBuffer = ArrayPool<byte>.Shared.Rent(bgraImageSize);
                try
                {
                    ImageConverters.DecodeMjpegToBgra(colorFrame, bgraBuffer);
                    var res = colorFrame is Image.Azure
                        ? (Image)new Image.Azure(ImageFormat.ColorBgra32, colorFrame.WidthPixels, colorFrame.HeightPixels)
                        : new Image.Orbbec(ImageFormat.ColorBgra32, colorFrame.WidthPixels, colorFrame.HeightPixels);
                    res.FillFrom(bgraBuffer);
                    return res;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(bgraBuffer);
                }
            }

            return colorFrame.DuplicateReference();
        }
    }
}
