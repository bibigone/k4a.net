using System;

namespace K4AdotNet.Sensor
{
    // Defined in k4atypes.h:
    // typedef struct _k4a_calibration_t
    // {
    //     k4a_calibration_camera_t depth_camera_calibration;
    //     k4a_calibration_camera_t color_camera_calibration;
    //     k4a_calibration_extrinsics_t extrinsics[K4A_CALIBRATION_TYPE_NUM][K4A_CALIBRATION_TYPE_NUM];
    //     k4a_depth_mode_t depth_mode;
    //     k4a_color_resolution_t color_resolution;
    // } k4a_calibration_t;
    //
    /// <summary>Information about device calibration in particular depth mode and color resolution.</summary>
    /// <seealso cref="Transformation"/>
    public abstract partial class Calibration : SdkObject
    {
        private readonly NativeApi api;

        public readonly CalibrationData Data;

        private Calibration(NativeApi api, in CalibrationData data)
            : base(api.IsOrbbec)
        {
            this.api = api;
            Data = data;
        }

        /// <summary>Depth camera calibration.</summary>
        public CameraCalibration DepthCameraCalibration => Data.DepthCameraCalibration;

        /// <summary>Color camera calibration.</summary>
        public CameraCalibration ColorCameraCalibration => Data.ColorCameraCalibration;

        /// <summary>Extrinsic transformation parameters.</summary>
        /// <remarks>
        /// The extrinsic parameters allow 3D coordinate conversions between depth camera, color camera, the IMU's gyroscope
        /// and accelerometer. To transform from a source to a target 3D coordinate system, use the parameters stored
        /// under <c>Extrinsics[source * (int)CalibrationSensor.Count + target]</c>.
        /// </remarks>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        /// <seealso cref="SetExtrinsics(CalibrationGeometry, CalibrationGeometry, CalibrationExtrinsics)"/>
        public CalibrationExtrinsics[]? Extrinsics => Data.Extrinsics;

        /// <summary>Depth camera mode for which calibration was obtained.</summary>
        public DepthMode DepthMode => Data.DepthMode;

        /// <summary>Color camera resolution for which calibration was obtained.</summary>
        public ColorResolution ColorResolution => Data.ColorResolution;

        #region Helper methods

        /// <summary>Does this calibration data look as valid?</summary>
        /// <remarks>
        /// WARNING! This property performs only some basic and simple checks.
        /// If it returns <see langword="true"/>, calibration data can still be meaningless/incorrect.
        /// </remarks>
        public bool IsValid => Data.IsValid;

        /// <summary>Helper method to get mutual extrinsics parameters for a given couple of sensors in Azure Kinect device.</summary>
        /// <param name="sourceSensor">Source coordinate system for transformation.</param>
        /// <param name="targetSensor">Destination coordinate system for transformation.</param>
        /// <returns>Extracted parameters of transformation from <paramref name="sourceSensor"/> to <paramref name="targetSensor"/>.</returns>
        /// <seealso cref="SetExtrinsics(CalibrationGeometry, CalibrationGeometry, CalibrationExtrinsics)"/>
        /// <seealso cref="Extrinsics"/>
        public CalibrationExtrinsics GetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor)
            => Data.Extrinsics![(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor];

        /// <summary>Helper method to set mutual extrinsics parameters for a given couple of sensors in Azure Kinect device.</summary>
        /// <param name="sourceSensor">Source coordinate system for transformation.</param>
        /// <param name="targetSensor">Destination coordinate system for transformation.</param>
        /// <param name="extrinsics">Parameters of source-to-destination transformation to be set.</param>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        /// <seealso cref="Extrinsics"/>
        public void SetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor, CalibrationExtrinsics extrinsics)
            => Data.Extrinsics![(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor] = extrinsics;

        #endregion

        #region Dummy calibration for test and stub needs

        /// <summary>
        /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned) but valid calibration data.
        /// This can be useful for testing and subbing needs.
        /// </summary>
        /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
        /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
        /// <returns>Created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        public static Calibration CreateDummy(DepthMode depthMode, ColorResolution colorResolution)
            => Sdk.ComboMode switch
            {
                ComboMode.Azure => Azure.CreateDummy(depthMode, colorResolution),
                ComboMode.Orbbec => Orbbec.CreateDummy(depthMode, colorResolution),
                ComboMode.Both => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned, there is specified distance between depth and color cameras) but valid calibration data.
        /// This can be useful for testing and subbing needs.
        /// </summary>
        /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
        /// <param name="distanceBetweenDepthAndColorMm">Distance (horizontal) between depth and color cameras.</param>
        /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
        /// <returns>Result: created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        public static Calibration CreateDummy(DepthMode depthMode, ColorResolution colorResolution, float distanceBetweenDepthAndColorMm)
            => Sdk.ComboMode switch
            {
                ComboMode.Azure => Azure.CreateDummy(depthMode, colorResolution, distanceBetweenDepthAndColorMm),
                ComboMode.Orbbec => Orbbec.CreateDummy(depthMode, colorResolution, distanceBetweenDepthAndColorMm),
                ComboMode.Both => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };

        #endregion

        #region Wrappers around native API (inspired by struct calibration from k4a.hpp)

        /// <summary>Gets the camera calibration for a device from a raw calibration blob.</summary>
        /// <param name="rawCalibration">Raw calibration blob obtained from a device or recording. The raw calibration must be <c>0</c>-terminated. Cannot be <see langword="null"/>.</param>
        /// <param name="depthMode">Mode in which depth camera is operated.</param>
        /// <param name="colorResolution">Resolution in which color camera is operated.</param>
        /// <returns>Calibration data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rawCalibration"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="rawCalibration"/> must be 0-terminated.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        public static Calibration CreateFromRaw(byte[] rawCalibration, DepthMode depthMode, ColorResolution colorResolution)
            => Sdk.ComboMode switch
            {
                ComboMode.Azure => Azure.CreateFromRaw(rawCalibration, depthMode, colorResolution),
                ComboMode.Orbbec => Orbbec.CreateFromRaw(rawCalibration, depthMode, colorResolution),
                ComboMode.Both => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };

        /// <summary>Transform a 2D pixel coordinate with an associated depth value of the source camera into a 2D pixel coordinate of the target camera.</summary>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">The depth of <paramref name="sourcePoint2D"/> in millimeters.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <returns>
        /// The 2D pixel in <paramref name="targetCamera"/> coordinates
        /// or <see langword="null"/> if <paramref name="sourcePoint2D"/> does not map to a valid 2D coordinate in the <paramref name="targetCamera"/> coordinate system.
        /// </returns>
        /// <remarks><para>
        /// This function maps a pixel between the coordinate systems of the depth and color cameras. It is equivalent to calling
        /// <see cref="Convert2DTo3D(Float2, float, CalibrationGeometry, CalibrationGeometry)"/> to compute the 3D point corresponding to <paramref name="sourcePoint2D"/> and then using
        /// <see cref="Convert3DTo2D(Float3, CalibrationGeometry, CalibrationGeometry)"/> to map the 3D point into the coordinate system of the <paramref name="targetCamera"/>.
        /// </para><para>
        /// If <paramref name="sourceCamera"/> and <paramref name="targetCamera"/> are identical, the function immediately returns value of
        /// <paramref name="sourcePoint2D"/> parameter and doesn't compute any transformations.
        /// </para></remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceCamera"/> is not a camera or <paramref name="targetCamera"/> is not a camera.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Cannot perform transformation. Most likely, calibration data is invalid.
        /// </exception>
        public Float2? Convert2DTo2D(Float2 sourcePoint2D, float sourceDepthMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCamera)
        {
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = api.Calibration2DTo2D(in Data, in sourcePoint2D, sourceDepthMm, sourceCamera, targetCamera, out var targetPoint2D, out var validFlag);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot transform 2D point to 2D point: invalid calibration data.");
            if (validFlag == 0)
                return null;
            return targetPoint2D;
        }

        /// <summary>Transform a 2D pixel coordinate from color camera into a 2D pixel coordinate of the depth camera.</summary>
        /// <param name="sourcePoint2D">The 2D pixel in color camera coordinates.</param>
        /// <param name="depthImage">Input depth image. Not <see langword="null"/>.</param>
        /// <returns>
        /// The 2D pixel in depth camera coordinates or
        /// <see langword="null"/> if <paramref name="sourcePoint2D"/> does not map to a valid 2D coordinate in the depth coordinate system.
        /// </returns>
        /// <remarks>
        /// This function represents an alternative to <see cref="Convert2DTo2D"/> if the number of pixels that need to be transformed is small.
        /// This function searches along an epipolar line in the depth image to find the corresponding
        /// depth pixel. If a larger number of pixels need to be transformed, it might be computationally cheaper to call
        /// <see cref="Transformation.DepthImageToColorCamera"/>
        /// to get correspondence depth values for these color pixels, then call the function <see cref="Convert2DTo2D"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="depthImage"/> cannot be <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="depthImage"/> has invalid format and/or resolution.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Cannot perform transformation. Most likely, calibration data is invalid.
        /// </exception>
        public Float2? ConvertColor2DToDepth2D(Float2 sourcePoint2D, Image depthImage)
        {
            if (depthImage is null)
                throw new ArgumentNullException(nameof(depthImage));
            if (depthImage.IsDisposed)
                throw new ObjectDisposedException(nameof(depthImage));
            if (depthImage.Format != ImageFormat.Depth16 || depthImage.WidthPixels != DepthMode.WidthPixels() || depthImage.HeightPixels != DepthMode.HeightPixels())
                throw new ArgumentException($"Invalid format or size of {nameof(depthImage)}", nameof(depthImage));
            var res = api.CalibrationColor2DToDepth2D(in Data, in sourcePoint2D, Image.ToHandle(depthImage), out var targetPoint2D, out var validFlag);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot transform color 2D point to depth 2D point: invalid calibration data.");
            if (validFlag == 0)
                return null;
            return targetPoint2D;
        }

        /// <summary>
        /// Transform a 2D pixel coordinate with an associated depth value of the source camera
        /// into a 3D point of the target coordinate system.
        /// </summary>
        /// <param name="sourcePoint2D">The 2D pixel in <paramref name="sourceCamera"/> coordinates.</param>
        /// <param name="sourceDepthMm">The depth of <paramref name="sourcePoint2D"/> in millimeters.</param>
        /// <param name="sourceCamera">The current camera.</param>
        /// <param name="targetCameraOrSensor">The target camera or IMU sensor.</param>
        /// <returns>
        /// 3D coordinates of the input pixel in the coordinate system of <paramref name="targetCameraOrSensor"/> in millimeters
        /// or <see langword="null"/> if the results are outside of the range of valid calibration.
        /// </returns>
        /// <remarks>
        /// This function applies the intrinsic calibration of <paramref name="sourceCamera"/> to compute the 3D ray from the focal point of the
        /// camera through pixel <paramref name="sourcePoint2D"/>.The 3D point on this ray is then found using <paramref name="sourceDepthMm"/>. If
        /// <paramref name="targetCameraOrSensor"/> is different from <paramref name="sourceCamera"/>, the 3D point is transformed to <paramref name="targetCameraOrSensor"/> using
        /// <see cref="Convert3DTo3D(Float3, CalibrationGeometry, CalibrationGeometry)"/>.
        /// In practice, <paramref name="sourceCamera"/> and <paramref name="targetCameraOrSensor"/> will often be identical. In this
        /// case, no 3D to 3D transformation is applied.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceCamera"/> is not a camera.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Cannot perform transformation. Most likely, calibration data is invalid.
        /// </exception>
        public Float3? Convert2DTo3D(Float2 sourcePoint2D, float sourceDepthMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCameraOrSensor)
        {
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCameraOrSensor.IsCamera() && !targetCameraOrSensor.IsImuPart())
                throw new ArgumentOutOfRangeException(nameof(targetCameraOrSensor));
            var res = api.Calibration2DTo3D(in Data, in sourcePoint2D, sourceDepthMm, sourceCamera, targetCameraOrSensor, out var targetPoint3DMm, out var validFlag);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot transform 2D point to 3D point: invalid calibration data.");
            if (validFlag == 0)
                return null;
            return targetPoint3DMm;
        }

        /// <summary>Transform a 3D point of a source coordinate system into a 2D pixel coordinate of the target camera.</summary>
        /// <param name="sourcePoint3DMm">The 3D coordinates in millimeters representing a point in <paramref name="sourceCameraOrSensor"/>.</param>
        /// <param name="sourceCameraOrSensor">The current camera or IMU sensor.</param>
        /// <param name="targetCamera">The target camera.</param>
        /// <returns>
        /// The 2D pixel in <paramref name="targetCamera"/> coordinates
        /// or <see langword="null"/> if the results are outside of the range of valid calibration.
        /// </returns>
        /// <remarks>
        /// If <paramref name="targetCamera"/> is different from <paramref name="sourceCameraOrSensor"/>, <paramref name="sourcePoint3DMm"/> is transformed
        /// to <paramref name="targetCamera"/> using <see cref="Convert3DTo3D(Float3, CalibrationGeometry, CalibrationGeometry)"/>.
        /// In practice, <paramref name="sourceCameraOrSensor"/> and <paramref name="targetCamera"/> will often be identical.
        /// In this case, no 3D to 3D transformation is applied. The 3D point in the coordinate system of <paramref name="targetCamera"/> is then
        /// projected onto the image plane using the intrinsic calibration of <paramref name="targetCamera"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="targetCamera"/> is not a camera.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Cannot perform transformation. Most likely, calibration data is invalid.
        /// </exception>
        public Float2? Convert3DTo2D(Float3 sourcePoint3DMm, CalibrationGeometry sourceCameraOrSensor, CalibrationGeometry targetCamera)
        {
            if (!sourceCameraOrSensor.IsCamera() && !sourceCameraOrSensor.IsImuPart())
                throw new ArgumentOutOfRangeException(nameof(sourceCameraOrSensor));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = api.Calibration3DTo2D(in Data, in sourcePoint3DMm, sourceCameraOrSensor, targetCamera, out var targetPoint2D, out var validFlag);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot transform 3D point to 2D point: invalid calibration data.");
            if (validFlag == 0)
                return null;
            return targetPoint2D;
        }

        /// <summary>Transform a 3D point of a source coordinate system into a 3D point of the target coordinate system.</summary>
        /// <param name="sourcePoint3DMm">The 3D coordinates in millimeters representing a point in <paramref name="sourceCameraOrSensor"/>.</param>
        /// <param name="sourceCameraOrSensor">The current coordinate system of camera or IMU sensor.</param>
        /// <param name="targetCameraOrSensor">The target coordinate system of camera or IMU sensor.</param>
        /// <returns>The new 3D coordinates of the input point in the coordinate space <paramref name="targetCameraOrSensor"/> in millimeters.</returns>
        /// <remarks>
        /// This function is used to transform 3D points between depth and color camera coordinate systems. The function uses the
        /// extrinsic camera calibration. It computes the output via multiplication with a precomputed matrix encoding a 3D
        /// rotation and a 3D translation. If <paramref name="sourceCameraOrSensor"/> and <paramref name="targetCameraOrSensor"/> are the same, then result will
        /// be identical to <paramref name="sourcePoint3DMm"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Cannot perform transformation. Most likely, calibration data is invalid.
        /// </exception>
        public Float3 Convert3DTo3D(Float3 sourcePoint3DMm, CalibrationGeometry sourceCameraOrSensor, CalibrationGeometry targetCameraOrSensor)
        {
            if (!sourceCameraOrSensor.IsCamera() && !sourceCameraOrSensor.IsImuPart())
                throw new ArgumentOutOfRangeException(nameof(sourceCameraOrSensor));
            if (!targetCameraOrSensor.IsCamera() && !targetCameraOrSensor.IsImuPart())
                throw new ArgumentOutOfRangeException(nameof(targetCameraOrSensor));
            var res = api.Calibration3DTo3D(in Data, in sourcePoint3DMm, sourceCameraOrSensor, targetCameraOrSensor, out var targetPoint3DMm);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot transform 3D point to 3D point: invalid calibration data.");
            return targetPoint3DMm;
        }

        /// <summary>Helper method to create <see cref="Transformation"/> object from this calibration data. For details see <see cref="Transformation"/>.</summary>
        /// <returns>Created transformation object. Not <see langword="null"/>.</returns>
        /// <seealso cref="Transformation"/>.
        public abstract Transformation CreateTransformation();

        #endregion
    }
}
