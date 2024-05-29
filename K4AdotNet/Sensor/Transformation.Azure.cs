using System;

namespace K4AdotNet.Sensor
{
    partial class Transformation
    {
        /// <summary>
        /// Implementation of base <see cref="Transformation"/> class for Azure Kinect devices.
        /// This class works via `original K4A` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Azure"/> and <see cref="ComboMode.Both"/>.</remarks>
        public sealed class Azure : Transformation
        {
            /// <summary>
            /// Creates transformation object for a give calibration data (implementation for Azure Kinect devices).
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
