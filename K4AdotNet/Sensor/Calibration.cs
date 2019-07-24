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
    /// <summary>Information about device calibration in particular depth mode and color resolution.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Calibration
    {
        /// <summary>Depth camera calibration.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CameraCalibration DepthCameraCalibration;

        /// <summary>Color camera calibration.</summary>
        [MarshalAs(UnmanagedType.Struct)]
        public CameraCalibration ColorCameraCalibration;

        /// <summary>Extrinsic transformation parameters.</summary>
        /// <remarks>
        /// The extrinsic parameters allow 3D coordinate conversions between depth camera, color camera, the IMU's gyroscope
        /// and accelerometer.To transform from a source to a target 3D coordinate system, use the parameters stored
        /// under <c>Extrinsics[source * (int)CalibrationSensor.Count + target]</c>.
        /// </remarks>
        /// <seealso cref="GetExtrinsics(CalibrationGeometry, CalibrationGeometry)"/>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count)]
        public CalibrationExtrinsics[] Extrinsics;

        /// <summary>Depth camera mode for which calibration was obtained.</summary>
        public DepthMode DepthMode;

        /// <summary>Color camera resolution for which calibration was obtained.</summary>
        public ColorResolution ColorResolution;

        public CalibrationExtrinsics GetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor)
            => Extrinsics[(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor];

        public void SetExtrinsics(CalibrationGeometry sourceSensor, CalibrationGeometry targetSensor, CalibrationExtrinsics extrinsics)
            => Extrinsics[(int)sourceSensor * (int)CalibrationGeometry.Count + (int)targetSensor] = extrinsics;

        #region Dummy calibration for test and stub needs

        public static void CreateDummy(DepthMode depthMode, ColorResolution colorResolution, out Calibration calibration)
        {
            calibration = default(Calibration);

            // depth camera
            calibration.DepthMode = depthMode;
            depthMode.GetNominalFov(out var hFovDegrees, out var vFovDegrees);
            InitDummyCameraCalibration(ref calibration.DepthCameraCalibration,
                depthMode.WidthPixels(), depthMode.HeightPixels(),
                hFovDegrees, vFovDegrees);

            // color camera
            calibration.ColorResolution = colorResolution;
            colorResolution.GetNominalFov(out hFovDegrees, out vFovDegrees);
            InitDummyCameraCalibration(ref calibration.ColorCameraCalibration,
                colorResolution.WidthPixels(), colorResolution.HeightPixels(),
                hFovDegrees, vFovDegrees);

            // extrinsics
            calibration.Extrinsics = new CalibrationExtrinsics[(int)CalibrationGeometry.Count * (int)CalibrationGeometry.Count];
            for (var i = 0; i < calibration.Extrinsics.Length; i++)
                InitDummyExtrinsics(ref calibration.Extrinsics[i]);
        }

        private static void InitDummyCameraCalibration(ref CameraCalibration cameraCalibration, int widthPixels, int heightPixels,
            float hFovDegrees, float vFovDegrees)
        {
            if (widthPixels <= 0 || heightPixels <= 0)
                return;

            cameraCalibration.ResolutionWidth = widthPixels;
            cameraCalibration.ResolutionHeight = heightPixels;
            cameraCalibration.MetricRadius = 1.7f;

            cameraCalibration.Intrinsics.Model = CalibrationModel.BrownConrady;
            cameraCalibration.Intrinsics.ParameterCount = 14;
            cameraCalibration.Intrinsics.Parameters.Cx = widthPixels / 2f;
            cameraCalibration.Intrinsics.Parameters.Cy = heightPixels / 2f;
            cameraCalibration.Intrinsics.Parameters.Fx = SizeAndFovToFocus(widthPixels, hFovDegrees);
            cameraCalibration.Intrinsics.Parameters.Fy = SizeAndFovToFocus(heightPixels, vFovDegrees);

            InitDummyExtrinsics(ref cameraCalibration.Extrinsics);
        }

        private static float SizeAndFovToFocus(int sizePixels, float fovDegrees)
            => sizePixels / (float)(2 * Math.Tan(fovDegrees * Math.PI / 360));

        private static void InitDummyExtrinsics(ref CalibrationExtrinsics extrinsics)
            => extrinsics.Rotation.M11 = extrinsics.Rotation.M22 = extrinsics.Rotation.M33 = 1f;

        #endregion

        #region Wrappers around native API (inspired by struct calibration from k4a.hpp)

        public static void CreateFromRaw(byte[] rawCalibration, DepthMode depthMode, ColorResolution colorResolution, out Calibration calibration)
        {
            if (rawCalibration == null)
                throw new ArgumentNullException(nameof(rawCalibration));
            var res = NativeApi.CalibrationGetFromRaw(rawCalibration, Helpers.Int32ToUIntPtr(rawCalibration.Length), depthMode, colorResolution, out calibration);
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Cannot create calibration from parameters specified.");
        }

        public Float2? Convert2DTo2D(Float2 sourcePoint2D, float sourceDepthMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCamera)
        {
            if (sourceDepthMm <= float.Epsilon)
                throw new ArgumentOutOfRangeException(nameof(sourceDepthMm));
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = NativeApi.Calibration2DTo2D(ref this, ref sourcePoint2D, sourceDepthMm, sourceCamera, targetCamera, out var targetPoint2D, out var valid);
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Cannot transform 2D point to 2D point: invalid calibration data.");
            if (!valid)
                return null;
            return targetPoint2D;
        }

        public Float3? Convert2DTo3D(Float2 sourcePoint2D, float sourceDepthMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCamera)
        {
            if (sourceDepthMm <= float.Epsilon)
                throw new ArgumentOutOfRangeException(nameof(sourceDepthMm));
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = NativeApi.Calibration2DTo3D(ref this, ref sourcePoint2D, sourceDepthMm, sourceCamera, targetCamera, out var targetPoint3DMm, out var valid);
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Cannot transform 2D point to 3D point: invalid calibration data.");
            if (!valid)
                return null;
            return targetPoint3DMm;
        }

        public Float2? Convert3DTo2D(Float3 sourcePoint3DMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCamera)
        {
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = NativeApi.Calibration3DTo2D(ref this, ref sourcePoint3DMm, sourceCamera, targetCamera, out var targetPoint2D, out var valid);
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Cannot transform 3D point to 2D point: invalid calibration data.");
            if (!valid)
                return null;
            return targetPoint2D;
        }

        public Float3 Convert3DTo3D(Float3 sourcePoint3DMm, CalibrationGeometry sourceCamera, CalibrationGeometry targetCamera)
        {
            if (!sourceCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(sourceCamera));
            if (!targetCamera.IsCamera())
                throw new ArgumentOutOfRangeException(nameof(targetCamera));
            var res = NativeApi.Calibration3DTo3D(ref this, ref sourcePoint3DMm, sourceCamera, targetCamera, out var targetPoint3DMm);
            if (res == NativeCallResults.Result.Failed)
                throw new InvalidOperationException("Cannot transform 3D point to 3D point: invalid calibration data.");
            return targetPoint3DMm;
        }

        public Transformation CreateTransformation()
            => new Transformation(ref this);

        #endregion
    }
}
