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
    //      K4A_COLOR_CONTROL_POWERLINE_FREQUENCY
    // } k4a_color_control_command_t;
    /// <summary>Color sensor control commands.</summary>
    /// <remarks>
    /// Control values set on a device are reset only when the device is power cycled. The device will retain the settings
    /// even if the device is closed or the application is restarted.
    /// </remarks>
    public enum ColorControlCommand
    {
        /// <summary>Exposure time setting.</summary>
        /// <remarks>
        /// May be set to <see cref="ColorControlMode.Auto"/> or <see cref="ColorControlMode.Manual"/>.
        /// Exposure time is measured in microseconds.
        /// </remarks>
        ExposureTimeAbsolute = 0,

        /// <summary>Exposure or Framerate priority setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// Value of <c>0</c> means framerate priority. Value of <c>1</c> means exposure priority.
        /// Using exposure priority may impact the framerate of both the color and depth cameras.
        /// Deprecated starting in 1.1.0. Please discontinue usage, firmware does not support this.
        /// </remarks>
        [Obsolete]
        AutoExposurePriority,

        /// <summary>Brightness setting.</summary>
        /// <remarks>
        /// May only be set to <see cref="ColorControlMode.Manual"/>.
        /// The valid range is 0 to 255. The default value is 128.
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
    }
}
