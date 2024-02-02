using K4AdotNet.Sensor;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.IO;

namespace K4AdotNet.Samples.Wpf.Common
{
    /// <summary>Helper methods to work with images.</summary>
    public static class ImageConverters
    {
        private static readonly DecoderOptions decoderOptions = new();

        /// <summary>Decodes MJPEG image to the destination BGRA image buffer.</summary>
        /// <param name="mjpegImage">Source image in MJPEG format to be decoded. Not <see langword="null"/>.</param>
        /// <param name="dstBuffer">Destination buffer for a decoded image. Should be big enough for a result BGRA image (4 bytes per pixel). Not <see langword="null"/>.</param>
        public static unsafe void DecodeMjpegToBgra(Image mjpegImage, byte[] dstBuffer)
        {
            Debug.Assert(mjpegImage.Format == ImageFormat.ColorMjpg);

            using var stream = new UnmanagedMemoryStream((byte*)mjpegImage.Buffer.ToPointer(), mjpegImage.SizeBytes);
            using var decodedImage = JpegDecoder.Instance.Decode<Bgra32>(decoderOptions, stream);
            decodedImage.CopyPixelDataTo(dstBuffer);
        }
    }
}
