namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_COLOR_CONTROL_MODE_AUTO = 0,
    //     K4A_COLOR_CONTROL_MODE_MANUAL,
    // } k4a_color_control_mode_t;
    //
    /// <summary>Color sensor control mode.</summary>
    public enum ColorControlMode : int
    {
        /// <summary>set the associated <see cref="ColorControlCommand"/> to auto mode</summary>
        Auto = 0,

        /// <summary>set the associated <see cref="ColorControlCommand"/> to manual mode</summary>
        Manual,
    }
}
