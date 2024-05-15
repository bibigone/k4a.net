using System;

namespace K4AdotNet.Sensor
{
    partial class Calibration
    {
        public sealed class Azure : Calibration
        {
            public Azure(in CalibrationData data)
                : base(NativeApi.Azure.Instance, data) { }

            /// <summary>
            /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned) but valid calibration data.
            /// This can be useful for testing and subbing needs.
            /// </summary>
            /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
            /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
            /// <returns>Created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
            public static new Azure CreateDummy(DepthMode depthMode, ColorResolution colorResolution)
            {
                CalibrationData.CreateDummy(depthMode, colorResolution, false, out var data);
                return new(in data);
            }

            /// <summary>
            /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned, there is specified distance between depth and color cameras) but valid calibration data.
            /// This can be useful for testing and subbing needs.
            /// </summary>
            /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
            /// <param name="distanceBetweenDepthAndColorMm">Distance (horizontal) between depth and color cameras.</param>
            /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
            /// <returns>Result: created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
            public static new Azure CreateDummy(DepthMode depthMode, ColorResolution colorResolution, float distanceBetweenDepthAndColorMm)
            {
                CalibrationData.CreateDummy(depthMode, colorResolution, false, distanceBetweenDepthAndColorMm, out var data);
                return new(in data);
            }

            /// <summary>Gets the camera calibration for a device from a raw calibration blob.</summary>
            /// <param name="rawCalibration">Raw calibration blob obtained from a device or recording. The raw calibration must be <c>0</c>-terminated. Cannot be <see langword="null"/>.</param>
            /// <param name="depthMode">Mode in which depth camera is operated.</param>
            /// <param name="colorResolution">Resolution in which color camera is operated.</param>
            /// <returns>Calibration data.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="rawCalibration"/> cannot be <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="rawCalibration"/> must be 0-terminated.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
            public static new Azure CreateFromRaw(byte[] rawCalibration, DepthMode depthMode, ColorResolution colorResolution)
            {
                if (rawCalibration == null)
                    throw new ArgumentNullException(nameof(rawCalibration));
                if (rawCalibration.IndexOf(0) < 0)
                    throw new ArgumentException($"{nameof(rawCalibration)} must be 0-terminated.", nameof(rawCalibration));
                if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                    throw new ArgumentOutOfRangeException(nameof(depthMode) + " and " + nameof(colorResolution), $"{nameof(depthMode)} and {nameof(colorResolution)} cannot be equal to Off simultaneously.");
                var res = NativeApi.Azure.Instance.CalibrationGetFromRaw(rawCalibration, Helpers.Int32ToUIntPtr(rawCalibration.Length), depthMode, colorResolution, out var calibration);
                if (res == NativeCallResults.Result.Failed)
                    throw new InvalidOperationException("Cannot create calibration from parameters specified.");
                return new(calibration);
            }

            /// <summary>Helper method to create <see cref="Transformation"/> object from this calibration data. For details see <see cref="Transformation.Azure(in CalibrationData)"/>.</summary>
            /// <returns>Created transformation object. Not <see langword="null"/>.</returns>
            /// <seealso cref="Transformation.Azure.Azure(in CalibrationData)"/>.
            public override Transformation CreateTransformation()
                => new Transformation.Azure(in Data);
        }
    }
}
