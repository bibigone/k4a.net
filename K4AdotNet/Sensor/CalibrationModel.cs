using System;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_CALIBRATION_LENS_DISTORTION_MODEL_UNKNOWN = 0,
    //     K4A_CALIBRATION_LENS_DISTORTION_MODEL_THETA,
    //     K4A_CALIBRATION_LENS_DISTORTION_MODEL_POLYNOMIAL_3K,
    //     K4A_CALIBRATION_LENS_DISTORTION_MODEL_RATIONAL_6KT,
    //     K4A_CALIBRATION_LENS_DISTORTION_MODEL_BROWN_CONRADY,
    // } k4a_calibration_model_type_t;
    //
    /// <summary>The model used to interpret the calibration parameters.</summary>
    public enum CalibrationModel
    {
        /// <summary>Calibration model is unknown.</summary>
        Unknown = 0,

        /// <summary>Deprecated (not supported). Calibration model is Theta (arctan).</summary>
        [Obsolete]
        Theta,

        /// <summary>Deprecated (not supported). Calibration model Polynomial 3K.</summary>
        [Obsolete]
        Polynomial3K,

        /// <summary>Deprecated (only supported early internal devices). Calibration model Rational 6KT.</summary>
        [Obsolete]
        Rational6KT,

        /// <summary>Calibration model Brown Conrady (compatible with OpenCV).</summary>
        BrownConrady,
    }
}
