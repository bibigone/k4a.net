using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace K4AdotNet.Samples.Wpf.Common
{
    public static class ImageConverters
    {
        public static unsafe void DecodeMjpegToBgra(Sensor.Image mjpegImage, byte[] dstBuffer)
        {
            using var stream = new UnmanagedMemoryStream((byte*)mjpegImage.Buffer.ToPointer(), mjpegImage.SizeBytes);

            var decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);

            BitmapSource frame = decoder.Frames[0];
            if (frame.Format != PixelFormats.Bgra32)
                frame = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0.0);

            frame.CopyPixels(dstBuffer, frame.PixelWidth * 4, 0);
        }
    }
}
