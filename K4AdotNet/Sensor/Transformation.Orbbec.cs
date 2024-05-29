using System;

namespace K4AdotNet.Sensor
{
    partial class Transformation
    {
        /// <summary>
        /// Implementation of base <see cref="Device"/> class for Orbbec Femto devices.
        /// This class works via `Orbbec SDK K4A Wrapper` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Orbbec"/> and <see cref="ComboMode.Both"/>.</remarks>
        public sealed class Orbbec : Transformation
        {
            /// <summary>
            /// Creates transformation object for a give calibration data (for Orbbec Femto devices).
            /// </summary>
            /// <param name="calibration">Camera calibration data.</param>
            /// <remarks><para>
            /// Each transformation instance requires some pre-computed resources to be allocated,
            /// which are retained until the call of <see cref="Dispose"/> method.
            /// </para></remarks>
            /// <exception cref="InvalidOperationException">Something wrong with calibration data.</exception>
            /// <seealso cref="Dispose"/>
            public Orbbec(in CalibrationData calibration)
                : base(CreateTransformation(NativeApi.Orbbec.Instance, in calibration), in calibration)
            { }
        }
    }
}
