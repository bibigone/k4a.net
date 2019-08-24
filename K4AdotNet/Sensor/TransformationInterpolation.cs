namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //      K4A_TRANSFORMATION_INTERPOLATION_TYPE_NEAREST = 0,
    //      K4A_TRANSFORMATION_INTERPOLATION_TYPE_LINEAR,
    // } k4a_transformation_interpolation_type_t;
    //
    /// <summary>Transformation interpolation type.</summary>
    /// <seealso cref="Transformation.DepthImageToColorCameraCustom"/>
    public enum TransformationInterpolation
    {
        /// <summary>Nearest neighbor interpolation.</summary>
        Nearest = 0,

        /// <summary>Linear interpolation.</summary>
        Linear,
    }
}
