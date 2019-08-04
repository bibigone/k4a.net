namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //      K4A_IMAGE_FORMAT_COLOR_MJPG = 0,
    //      K4A_IMAGE_FORMAT_COLOR_NV12,
    //      K4A_IMAGE_FORMAT_COLOR_YUY2,
    //      K4A_IMAGE_FORMAT_COLOR_BGRA32,
    //      K4A_IMAGE_FORMAT_DEPTH16,
    //      K4A_IMAGE_FORMAT_IR16,
    //      K4A_IMAGE_FORMAT_CUSTOM,
    // } k4a_image_format_t;
    /// <summary>Image format type.</summary>
    /// <remarks>Helpful extension methods for this enumeration type can be found in <see cref="ImageFormats"/>.</remarks>
    /// <seealso cref="ImageFormats"/>
    public enum ImageFormat
    {
        /// <summary>
        /// The buffer for each image is encoded as a JPEG and can be decoded by a JPEG decoder.
        /// </summary>
        /// <remarks>
        /// Because the image is compressed, the stride parameter is not applicable.
        /// Each MJPG encoded image in a stream may be of differing size depending on the compression efficiency.
        /// </remarks>
        ColorMjpg = 0,

        /// <summary>
        /// NV12 images separate the luminance and chroma data such that all the luminance is at the
        /// beginning of the buffer, and the chroma lines follow immediately after.
        /// </summary>
        /// <remarks>
        /// Stride indicates the length of each line in bytes and should be used to determine the start location of each line
        /// of the image in memory. Chroma has half as many lines of height and half the width in pixels of the luminance.
        /// Each chroma line has the same width in bytes as a luminance line.
        /// </remarks>
        ColorNV12,

        /// <summary>
        /// YUY2 stores chroma and luminance data in interleaved pixels.
        /// </summary>
        /// <remarks>
        /// Stride indicates the length of each line in bytes and should be used to determine the start location of each
        /// line of the image in memory.
        /// </remarks>
        ColorYUY2,

        /// <summary>
        /// Each pixel of BGRA32 data is four bytes. The first three bytes represent Blue, Green,
        /// and Red data. The fourth byte is the alpha channel and is unused in the Azure Kinect APIs.
        /// </summary>
        /// <remarks>
        /// Stride indicates the length of each line in bytes and should be used to determine the start location of each
        /// line of the image in memory.
        ///
        /// The Azure Kinect device does not natively capture in this format. Requesting images of this format
        /// requires additional computation in the API.
        /// </remarks>
        ColorBgra32,

        /// <summary>
        /// Each pixel of DEPTH16 data is two bytes of little endian unsigned depth data. The unit of the data is in
        /// millimeters from the origin of the camera.
        /// </summary>
        /// <remarks>
        /// Stride indicates the length of each line in bytes and should be used to determine the start location of each
        /// line of the image in memory.
        /// </remarks>
        Depth16,

        /// <summary>
        /// This format represents infrared light and is captured by the depth camera.
        /// Each pixel of IR16 data is two bytes of little endian unsigned depth data. The value of the data represents
        /// brightness.
        /// </summary>
        /// <remarks>
        /// Stride indicates the length of each line in bytes and should be used to determine the start location of each
        /// line of the image in memory.
        /// </remarks>
        IR16,

        /// <summary>
        /// Custom image format.
        /// Used in conjunction with user created images or images packing non-standard data.
        /// </summary>
        /// <remarks>
        /// See the originator of the custom formatted image for information on how to interpret the data.
        /// </remarks>
        Custom,
    }
}
