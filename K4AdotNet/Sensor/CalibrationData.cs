using System;
using System.Runtime.InteropServices;

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
    /// <remarks>
    /// This structure represents calibration data itself.
    /// All implementation-specific methods can be found in the <see cref="Calibration"/> wrapper class.
    /// </remarks>
    /// <seealso cref="Calibration"/>
    /// <seealso cref="Transformation"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationData
    {
        /// <summary>Depth camera calibration.</summary>
        public CameraCalibration DepthCameraCalibration;

        /// <summary>Color camera calibration.</summary>
        public CameraCalibration ColorCameraCalibration;

        /// <summary>Extrinsic transformation parameters.</summary>
        /// <remarks>
        /// The extrinsic parameters allow 3D coordinate conversions between depth camera, color camera, the IMU's gyroscope
        /// and accelerometer. To transform from a source to a target 3D coordinate system, use the parameters stored
        /// under <c>Extrinsics[source * (int)CalibrationSensor.Count + target]</c>.
        /// </remarks>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        /// <seealso cref="SetExtrinsics(CalibrationGeometry, CalibrationGeometry, CalibrationExtrinsics)"/>
#pragma warning disable CS0618 // Type or member is obsolete: UnmanagedType.Struct is obsolete in .NET Standard 2.1
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count)]
#pragma warning restore CS0618 // Type or member is obsolete
        public CalibrationExtrinsics[]? Extrinsics;

        /// <summary>Depth camera mode for which calibration was obtained.</summary>
        public DepthMode DepthMode;

        /// <summary>Color camera resolution for which calibration was obtained.</summary>
        public ColorResolution ColorResolution;

        #region Helper methods

        /// <summary>Does this calibration data look as valid?</summary>
        /// <remarks>
        /// WARNING! This property performs only some basic and simple checks.
        /// If it returns <see langword="true"/>, calibration data can still be meaningless/incorrect.
        /// </remarks>
        public bool IsValid
            => Extrinsics != null
            && Extrinsics.Length == (int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count
            && (DepthMode != DepthMode.Off || ColorResolution != ColorResolution.Off)
            && ColorCameraCalibration.ResolutionWidth == ColorResolution.WidthPixels()
            && ColorCameraCalibration.ResolutionHeight == ColorResolution.HeightPixels()
            && DepthCameraCalibration.ResolutionWidth == DepthMode.WidthPixels()
            && DepthCameraCalibration.ResolutionHeight == DepthMode.HeightPixels()
            && ColorCameraCalibration.Intrinsics.ParameterCount >= 0
            && ColorCameraCalibration.Intrinsics.ParameterCount <= CalibrationIntrinsicParameters.ParameterCount
            && DepthCameraCalibration.Intrinsics.ParameterCount >= 0
            && DepthCameraCalibration.Intrinsics.ParameterCount <= CalibrationIntrinsicParameters.ParameterCount;

        /// <summary>Helper method to get mutual extrinsics parameters for a given couple of sensors in Azure Kinect device.</summary>
        /// <param name="sourceSensor">Source coordinate system for transformation.</param>
        /// <param name="targetSensor">Destination coordinate system for transformation.</param>
        /// <returns>Extracted parameters of transformation from <paramref name="sourceSensor"/> to <paramref name="targetSensor"/>.</returns>
        /// <seealso cref="SetExtrinsics(CalibrationGeometry, CalibrationGeometry, CalibrationExtrinsics)"/>
        /// <seealso cref="Extrinsics"/>
        public CalibrationExtrinsics GetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor)
            => Extrinsics![(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor];

        /// <summary>Helper method to set mutual extrinsics parameters for a given couple of sensors in Azure Kinect device.</summary>
        /// <param name="sourceSensor">Source coordinate system for transformation.</param>
        /// <param name="targetSensor">Destination coordinate system for transformation.</param>
        /// <param name="extrinsics">Parameters of source-to-destination transformation to be set.</param>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        /// <seealso cref="Extrinsics"/>
        public void SetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor, CalibrationExtrinsics extrinsics)
            => Extrinsics![(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor] = extrinsics;

        #endregion

        #region Dummy calibration for test and stub needs

        /// <summary>
        /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned) but valid calibration data.
        /// This can be useful for testing and subbing needs.
        /// </summary>
        /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
        /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
        /// <param name="isOrbbec"><see langword="true"/> for `Orbbec SDK K4A Wrapper` implementation, <see langword="false"/> for `original K4A` implementation.</param>
        /// <param name="calibration">Result: created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        public static void CreateDummy(DepthMode depthMode, ColorResolution colorResolution, bool isOrbbec, out CalibrationData calibration)
        {
            if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                throw new ArgumentOutOfRangeException(nameof(depthMode) + " and " + nameof(colorResolution), $"{nameof(depthMode)} and {nameof(colorResolution)} cannot be equal to Off simultaneously.");

            calibration = default;

            // depth camera
            calibration.DepthMode = depthMode;
            depthMode.GetNominalFov(out var hFovDegrees, out var vFovDegrees);
            InitDummyCameraCalibration(ref calibration.DepthCameraCalibration,
                depthMode.WidthPixels(), depthMode.HeightPixels(),
                hFovDegrees, vFovDegrees);

            // color camera
            calibration.ColorResolution = colorResolution;
            colorResolution.GetNominalFov(isOrbbec, out hFovDegrees, out vFovDegrees);
            InitDummyCameraCalibration(ref calibration.ColorCameraCalibration,
                colorResolution.WidthPixels(), colorResolution.HeightPixels(),
                hFovDegrees, vFovDegrees);

            // extrinsics
            calibration.Extrinsics = new CalibrationExtrinsics[(int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count];
            for (var i = 0; i < calibration.Extrinsics.Length; i++)
                InitDummyExtrinsics(ref calibration.Extrinsics[i]);
        }

        /// <summary>
        /// Creates dummy (no distortions, ideal pin-hole geometry, all sensors are aligned, there is specified distance between depth and color cameras) but valid calibration data.
        /// This can be useful for testing and subbing needs.
        /// </summary>
        /// <param name="depthMode">Depth mode for which dummy calibration should be created. Can be <see cref="DepthMode.Off"/>.</param>
        /// <param name="distanceBetweenDepthAndColorMm">Distance (horizontal) between depth and color cameras.</param>
        /// <param name="colorResolution">Color resolution for which dummy calibration should be created. Can be <see cref="ColorResolution.Off"/>.</param>
        /// <param name="isOrbbec"><see langword="true"/> for `Orbbec SDK K4A Wrapper` implementation, <see langword="false"/> for `original K4A` implementation.</param>
        /// <param name="calibration">Result: created dummy calibration data for <paramref name="depthMode"/> and <paramref name="colorResolution"/> specified.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="depthMode"/> and <paramref name="colorResolution"/> cannot be equal to <c>Off</c> simultaneously.</exception>
        public static void CreateDummy(DepthMode depthMode, ColorResolution colorResolution, bool isOrbbec, float distanceBetweenDepthAndColorMm,
            out CalibrationData calibration)
        {
            CreateDummy(depthMode, colorResolution, isOrbbec, out calibration);

            var extr = calibration.GetExtrinsics(CalibrationGeometry.Color, CalibrationGeometry.Depth);
            extr.Translation = new Float3(distanceBetweenDepthAndColorMm, 0, 0);
            calibration.SetExtrinsics(CalibrationGeometry.Color, CalibrationGeometry.Depth, extr);

            extr = calibration.GetExtrinsics(CalibrationGeometry.Depth, CalibrationGeometry.Color);
            extr.Translation = new Float3(-distanceBetweenDepthAndColorMm, 0, 0);
            calibration.SetExtrinsics(CalibrationGeometry.Depth, CalibrationGeometry.Color, extr);
        }

        // Ideal pin-hole camera. No distortions.
        private static void InitDummyCameraCalibration(ref CameraCalibration cameraCalibration, int widthPixels, int heightPixels,
            float hFovDegrees, float vFovDegrees)
        {
            if (widthPixels <= 0 || heightPixels <= 0)
                return;

            cameraCalibration.ResolutionWidth = widthPixels;
            cameraCalibration.ResolutionHeight = heightPixels;
            cameraCalibration.MetricRadius = 1.7f;

            cameraCalibration.Intrinsics.Model = CalibrationModel.BrownConrady;     // This model is in use in Azure Kinect Sensor SDK
            cameraCalibration.Intrinsics.ParameterCount = 14;                       // Corresponds to BrownConrady model
            cameraCalibration.Intrinsics.Parameters.Cx = (widthPixels - 1f) / 2f;
            cameraCalibration.Intrinsics.Parameters.Cy = (heightPixels - 1f) / 2f;
            cameraCalibration.Intrinsics.Parameters.Fx = SizeAndFovToFocus(widthPixels, hFovDegrees);
            cameraCalibration.Intrinsics.Parameters.Fy = SizeAndFovToFocus(heightPixels, vFovDegrees);

            InitDummyExtrinsics(ref cameraCalibration.Extrinsics);
        }

        // Ideal pin-hole camera. No distortions.
        private static float SizeAndFovToFocus(int sizePixels, float fovDegrees)
            => sizePixels / (float)(2 * Math.Tan(fovDegrees * Math.PI / 360));

        // Identity transformation. All sensors are ideally aligned in dummy calibration data.
        private static void InitDummyExtrinsics(ref CalibrationExtrinsics extrinsics)
            => extrinsics.Rotation.M11 = extrinsics.Rotation.M22 = extrinsics.Rotation.M33 = 1f;

        #endregion
    }
}
