using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_device_configuration_t
    // {
    //      k4a_image_format_t color_format;
    //      k4a_color_resolution_t color_resolution;
    //      k4a_depth_mode_t depth_mode;
    //      k4a_fps_t camera_fps;
    //      bool synchronized_images_only;
    //      int32_t depth_delay_off_color_usec;
    //      k4a_wired_sync_mode_t wired_sync_mode;
    //      uint32_t subordinate_delay_off_master_usec;
    //      bool disable_streaming_indicator;
    // } k4a_device_configuration_t;
    //
    /// <summary>Configuration parameters for an Azure Kinect device.</summary>
    /// <seealso cref="Device.StartCameras(DeviceConfiguration)"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceConfiguration
    {
        /// <summary>Image format to capture with the color camera.</summary>
        /// <remarks><para>
        /// The color camera does not natively produce <c>BGRA32</c> images.
        /// Setting <see cref="ImageFormat.ColorBgra32"/> value will result in higher CPU utilization.
        /// </para><para>
        /// Use only <see cref="ImageFormat"/>s that is applicable for color images.
        /// To check it you can call <see cref="ImageFormats.IsColor(ImageFormat)"/>.
        /// </para></remarks>
        /// <seealso cref="ImageFormats.IsColor(ImageFormat)"/>
        public ImageFormat ColorFormat;

        /// <summary>Image resolution to capture with the color camera.</summary>
        /// <remarks>
        /// Not all combinations of <see cref="ColorFormat"/>, <see cref="ColorResolution"/>
        /// and <see cref="DepthMode"/> are supported. To check compatibility use
        /// <see cref="ColorResolutions.IsCompatibleWith(ColorResolution, FrameRate)"/>
        /// and <see cref="ColorResolutions.IsCompatibleWith(ColorResolution, ImageFormat)"/>.
        /// </remarks>
        /// <seealso cref="ColorResolutions.IsCompatibleWith(ColorResolution, FrameRate)"/>
        /// <seealso cref="ColorResolutions.IsCompatibleWith(ColorResolution, ImageFormat)"/>
        public ColorResolution ColorResolution;

        /// <summary>Capture mode for the depth camera.</summary>
        /// <remarks>
        /// To check compatibility of <see cref="DepthMode"/> with <see cref="CameraFps"/>
        /// use <see cref="DepthModes.IsCompatibleWith(DepthMode, FrameRate)"/>.
        /// </remarks>
        /// <seealso cref="DepthModes.IsCompatibleWith(DepthMode, FrameRate)"/>
        public DepthMode DepthMode;

        /// <summary>Desired frame rate for the color and depth camera.</summary>
        /// <remarks>
        /// <see cref="FrameRate.Thirty"/> cannot be used with <see cref="ColorResolution.R3072p"/>
        /// and <see cref="DepthMode.WideViewUnbinned"/>. Use <see cref="FrameRates.IsCompatibleWith(FrameRate, ColorResolution)"/>
        /// and <see cref="FrameRates.IsCompatibleWith(FrameRate, DepthMode)"/> to check compatibility.
        /// </remarks>
        /// <seealso cref="FrameRates.IsCompatibleWith(FrameRate, ColorResolution)"/>
        /// <seealso cref="FrameRates.IsCompatibleWith(FrameRate, DepthMode)"/>
        public FrameRate CameraFps;

        /// <summary>Only produce capture objects if they contain synchronized color and depth images.</summary>
        /// <remarks><para>
        /// This setting controls the behavior in which images are dropped when images are produced faster than they can be
        /// read, or if there are errors in reading images from the device.
        /// </para><para>
        /// If set to <see langword="true"/>, capture objects will only be produced with both color and depth images.
        /// If set to <see langword="false"/>, capture objects may be produced only a single image when the corresponding image is dropped.
        /// </para><para>
        /// Setting this to <see langword="false"/> ensures that the caller receives all of the images received from the camera, regardless of
        /// whether the corresponding images expected in the capture are available.
        /// </para><para>
        /// If either the color or depth camera are disabled, this setting has no effect.
        /// </para></remarks>
        [MarshalAs(UnmanagedType.I1)]
        public bool SynchronizedImagesOnly;

        /// <summary>Desired delay between the capture of the color image and the capture of the depth image.</summary>
        /// <remarks>
        /// A negative value indicates that the depth image should be captured before the color image.
        /// Any value between negative and positive one capture period is valid.
        /// </remarks>
        public Microseconds32 DepthDelayOffColor;

        /// <summary>The external synchronization mode.</summary>
        public WiredSyncMode WiredSyncMode;

        /// <summary>The external synchronization timing.</summary>
        /// <remarks><para>
        /// If this camera is a subordinate, this sets the capture delay between the color camera capture and the external
        /// input pulse. A setting of zero indicates that the master and subordinate color images should be aligned.
        /// </para><para>
        /// This setting does not effect the 'Sync out' connection.
        /// </para><para>
        /// This value must be positive and range from zero to one capture period.
        /// </para><para>
        /// If this is not a subordinate, then this value is ignored.
        /// </para></remarks>
        public Microseconds32 SubordinateDelayOffMaster;

        /// <summary>Streaming indicator automatically turns on when the color or depth camera's are in use.</summary>
        /// <remarks>This setting disables that behavior and keeps the LED in an off state.</remarks>
        [MarshalAs(UnmanagedType.I1)]
        public bool DisableStreamingIndicator;

        /// <summary>
        /// Checks that this configuration is OK (all parameters have correct and compatible values).
        /// </summary>
        /// <param name="message">
        /// What's wrong with configuration or <see langword="null"/> if configuration is valid.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if configuration is valid and can be used in <see cref="Device.StartCameras(DeviceConfiguration)"/>.
        /// <see langword="false"/> if configuration has some invalid or incompatible parameters.
        /// </returns>
        public bool IsValid(out string message)
        {
            if (!ColorFormat.IsColor())
            {
                message = $"{nameof(ColorFormat)} = {ColorFormat} cannot be used for color camera";
                return false;
            }

            if (!CameraFps.IsCompatibleWith(DepthMode))
            {
                message = $"{nameof(CameraFps)} = {CameraFps} is not compatible with {nameof(DepthMode)} = {DepthMode}";
                return false;
            }

            if (!CameraFps.IsCompatibleWith(ColorResolution))
            {
                message = $"{nameof(CameraFps)} = {CameraFps} is not compatible with {nameof(ColorResolution)} = {ColorResolution}";
                return false;
            }

            if (!ColorResolution.IsCompatibleWith(ColorFormat))
            {
                message = $"{nameof(ColorFormat)} = {ColorFormat} is not compatible with {nameof(ColorResolution)} = {ColorResolution}";
                return false;
            }

            if (SubordinateDelayOffMaster < 0)
            {
                message = $"{nameof(SubordinateDelayOffMaster)} = {SubordinateDelayOffMaster} cannot be negative.";
                return false;
            }

            message = null;
            return true;
        }

        // Defined in k4atypes.h:
        // static const k4a_device_configuration_t K4A_DEVICE_CONFIG_INIT_DISABLE_ALL = { K4A_IMAGE_FORMAT_COLOR_MJPG,
        //                                                                                K4A_COLOR_RESOLUTION_OFF,
        //                                                                                K4A_DEPTH_MODE_OFF,
        //                                                                                K4A_FRAMES_PER_SECOND_30,
        //                                                                                false,
        //                                                                                0,
        //                                                                                K4A_WIRED_SYNC_MODE_STANDALONE,
        //                                                                                0,
        //                                                                                false };
        /// <summary>Initial configuration setting for disabling all sensors.</summary>
        /// <remarks>Use this setting to initialize a <see cref="DeviceConfiguration"/> to a disabled state.</remarks>
        public static readonly DeviceConfiguration DisableAll = new DeviceConfiguration
        {
             ColorFormat = ImageFormat.ColorMjpg,
             ColorResolution = ColorResolution.Off,
             DepthMode = DepthMode.Off,
             CameraFps = FrameRate.Thirty,
             SynchronizedImagesOnly = false,
             DepthDelayOffColor = Microseconds32.Zero,
             WiredSyncMode = WiredSyncMode.Standalone,
             SubordinateDelayOffMaster = Microseconds32.Zero,
             DisableStreamingIndicator = false,
        };
    }
}
