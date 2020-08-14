using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef union
    // {
    //     struct _param
    //     {
    //         float cx;
    //         float cy;
    //         float fx;
    //         float fy;
    //         float k1;
    //         float k2;
    //         float k3;
    //         float k4;
    //         float k5;
    //         float k6;
    //         float codx;
    //         float cody;
    //         float p2;
    //         float p1;
    //         float metric_radius;
    //     } param;
    //     float v[15];
    // } k4a_calibration_intrinsic_parameters_t;
    //
    /// <summary>Intrinsic calibration represents the internal optical properties of the camera.</summary>
    /// <remarks>Azure Kinect devices are calibrated with Brown Conrady which is compatible with OpenCV.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationIntrinsicParameters
    {
        /// <summary>Principal point in image, x. Corresponding index in array: 0.</summary>
        public float Cx;

        /// <summary>Principal point in image, y. Corresponding index in array: 1.</summary>
        public float Cy;

        /// <summary>Focal length x. Corresponding index in array: 2.</summary>
        public float Fx;

        /// <summary>Focal length y. Corresponding index in array: 3.</summary>
        public float Fy;

        /// <summary>k1 radial distortion coefficient. Corresponding index in array: 4.</summary>
        public float K1;

        /// <summary>kw radial distortion coefficient. Corresponding index in array: 5.</summary>
        public float K2;

        /// <summary>k3 radial distortion coefficient. Corresponding index in array: 6.</summary>
        public float K3;

        /// <summary>k4 radial distortion coefficient. Corresponding index in array: 7.</summary>
        public float K4;

        /// <summary>k5 radial distortion coefficient. Corresponding index in array: 8.</summary>
        public float K5;

        /// <summary>k6 radial distortion coefficient. Corresponding index in array: 9.</summary>
        public float K6;

        /// <summary>Center of distortion in Z=1 plane, x (only used for Rational6KT). Corresponding index in array: 10.</summary>
        public float Codx;

        /// <summary>Center of distortion in Z=1 plane, y (only used for Rational6KT). Corresponding index in array: 11.</summary>
        public float Cody;

        /// <summary>Tangential distortion coefficient 2. Corresponding index in array: 12.</summary>
        public float P2;

        /// <summary>Tangential distortion coefficient 1. Corresponding index in array: 13.</summary>
        public float P1;

        /// <summary>Not used in current version. Corresponding index in array: 14.</summary>
        public float NotUsed;

        /// <summary>Creates intrinsics parameters from an array representation.</summary>
        /// <param name="values">Array of length <see cref="ParameterCount"/>. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wrong length of <paramref name="values"/> array.</exception>
        public CalibrationIntrinsicParameters(float[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != ParameterCount)
                throw new ArgumentOutOfRangeException(nameof(values) + "." + nameof(values.Length));

            Cx = values[0];
            Cy = values[1];
            Fx = values[2];
            Fy = values[3];
            K1 = values[4];
            K2 = values[5];
            K3 = values[6];
            K4 = values[7];
            K5 = values[8];
            K6 = values[9];
            Codx = values[10];
            Cody = values[11];
            P2 = values[12];
            P1 = values[13];
            NotUsed = values[14];
        }

        /// <summary>Array representation of intrinsic model parameters.</summary>
        /// <returns>Array representation. Not <see langword="null"/>.</returns>
        public float[] ToArray()
            => new[] { Cx, Cy, Fx, Fy, K1, K2, K3, K4, K5, K6, Codx, Cody, P2, P1, NotUsed };

        /// <summary>Accessing to intrinsics parameter by index.</summary>
        /// <param name="index">Index of parameter. From <c>0</c> to <see cref="ParameterCount"/> excluding.</param>
        /// <returns>Value of appropriate parameter.</returns>
        /// <exception cref="IndexOutOfRangeException">Index is less than <c>0</c> or is greater or equal to <see cref="ParameterCount"/>.</exception>
        public float this[int index]
        {
            get => index switch
            {
                0 => Cx,
                1 => Cy,
                2 => Fx,
                3 => Fy,
                4 => K1,
                5 => K2,
                6 => K3,
                7 => K4,
                8 => K5,
                9 => K6,
                10 => Codx,
                11 => Cody,
                12 => P2,
                13 => P1,
                14 => NotUsed,
                _ => throw new IndexOutOfRangeException(nameof(index)),
            };

            set
            {
                switch (index)
                {
                    case 0: Cx = value; break;
                    case 1: Cy = value; break;
                    case 2: Fx = value; break;
                    case 3: Fy = value; break;
                    case 4: K1 = value; break;
                    case 5: K2 = value; break;
                    case 6: K3 = value; break;
                    case 7: K4 = value; break;
                    case 8: K5 = value; break;
                    case 9: K6 = value; break;
                    case 10: Codx = value; break;
                    case 11: Cody = value; break;
                    case 12: P2 = value; break;
                    case 13: P1 = value; break;
                    case 14: NotUsed = value; break;
                    default: throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>Count of all parameters. Only part of parameters can be used in different models.</summary>
        public const int ParameterCount = 15;
    }
}
