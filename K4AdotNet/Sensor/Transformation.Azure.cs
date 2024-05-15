using System;

namespace K4AdotNet.Sensor
{
    partial class Transformation
    {
        public sealed class Azure : Transformation
        {
            /// <summary>
            /// Creates transformation object for a give calibration data.
            /// </summary>
            /// <param name="calibration">Camera calibration data.</param>
            /// <remarks><para>
            /// Each transformation instance requires some pre-computed resources to be allocated,
            /// which are retained until the call of <see cref="Dispose"/> method.
            /// </para></remarks>
            /// <exception cref="InvalidOperationException">Something wrong with calibration data.</exception>
            /// <seealso cref="Dispose"/>
            public Azure(in CalibrationData calibration)
                : base(CreateTransformation(NativeApi.Azure.Instance, in calibration), in calibration)
            { }
        }
    }
}
