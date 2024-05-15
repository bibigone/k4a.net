using System;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //      K4A_COLOR_CONTROL_EXPOSURE_TIME_ABSOLUTE = 0,
    //      K4A_COLOR_CONTROL_AUTO_EXPOSURE_PRIORITY,
    //      K4A_COLOR_CONTROL_BRIGHTNESS,
    //      K4A_COLOR_CONTROL_CONTRAST,
    //      K4A_COLOR_CONTROL_SATURATION,
    //      K4A_COLOR_CONTROL_SHARPNESS,
    //      K4A_COLOR_CONTROL_WHITEBALANCE,
    //      K4A_COLOR_CONTROL_BACKLIGHT_COMPENSATION,
    //      K4A_COLOR_CONTROL_GAIN,
    //      K4A_COLOR_CONTROL_POWERLINE_FREQUENCY,
    //      K4A_COLOR_CONTROL_HDR  // OrbbecSDK-K4A-Wrapper only
    // } k4a_color_control_command_t;
    //
    /// <summary>Color sensor control commands.</summary>
    /// <remarks>
    /// Control values set on a device are reset only when the device is power cycled. The device will retain the settings
    /// even if the device is closed or the application is restarted.
    /// </remarks>
    public enum ColorControlCommand : int
    {
        /// <summary>Exposure time setting.</summary>
        /// <remarks><para>
        /// May be set to <see cref="ColorControlMode.Auto"/> or <see cref="ColorControlMode.Manual"/>.
        /// </para><para>
        /// The Azure Kinect supports a limited number of fixed exposure settings. When setting this, expect the exposure to
        /// be rounded up to the nearest setting. Exceptions are 1) The last value in the table is the upper limit, so a
        /// value larger than this will be overridden to the largest entry in the table. 2) The exposure time cannot be
        /// larger than the equivalent FPS. So expect 100ms exposure time to be reduced to 30ms or 33.33ms when the camera is
        /// started. The most recent copy of the table 'device_exposure_mapping' is in
        /// https://github.com/microsoft/Azure-Kinect-Sensor-SDK/blob/develop/src/color/color_priv.h
        /// </para></remarks>
        ExposureTimeAbsolute = 0,

        /// <summary>Exposure or Frame rate priority setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// Value of <c>0</c> means frame rate priority. Value of <c>1</c> means exposure priority.
        /// Using exposure priority may impact the frame rate of both the color and depth cameras.
        /// Deprecated starting from version 1.1.0 of Sensor SDK. Please discontinue usage, firmware does not support this.
        /// </remarks>
        [Obsolete("Deprecated starting from version 1.1.0 of Sensor SDK. Please discontinue usage, firmware does not support this.")]
        AutoExposurePriority,

        /// <summary>Brightness setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// The valid range is 0 to 255 for K4A and 100 for ORBBEC. The default value is 128 for K4A and 50 for ORBBEC.
        /// </remarks>
        Brightness,

        /// <summary>Contrast setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// </remarks>
        Contrast,

        /// <summary>Saturation setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// </remarks>
        Saturation,

        /// <summary>Sharpness setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// </remarks>
        Sharpness,

        /// <summary>White balance setting.</summary>
        /// <remarks>
        /// May be set to <see cref="ColorControlMode.Auto"/> or <see cref="ColorControlMode.Manual"/>.
        /// The unit is degrees Kelvin. The setting must be set to a value evenly divisible by 10 degrees.
        /// </remarks>
        Whitebalance,

        /// <summary>Backlight compensation setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// Value of <c>0</c> means backlight compensation is disabled. Value of <c>1</c> means backlight compensation is enabled.
        /// </remarks>
        BacklightCompensation,

        /// <summary>Gain setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// </remarks>
        Gain,

        /// <summary>Powerline frequency setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// Value of <c>1</c> sets the powerline compensation to 50 Hz. Value of <c>2</c> sets the powerline compensation to 60 Hz.
        /// </remarks>
        PowerlineFrequency,

        /// <summary>HDR setting (only for ORBBEC devices).</summary>
        /// <remarks><para>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// Must stop color camera before setting HDR mode.
        /// </para><para>
        /// Value of 0 means HDR is disabled. Value of 1 means HDR is enabled.
        /// </para></remarks>
        Hdr,
    }
}
