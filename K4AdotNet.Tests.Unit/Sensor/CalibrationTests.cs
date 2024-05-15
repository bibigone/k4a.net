﻿using K4AdotNet.Sensor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Unit.Sensor
{
    [TestClass]
    public class CalibrationTests
    {
        #region Creation from raw calibration data

        [TestMethod]
        public void TestCreationFromRaw()
        {
            var rawCalibration = ReadRawCalibrationFromResources();

            foreach (var depthMode in DepthModes.All)
            {
                foreach (var colorResolution in ColorResolutions.AllSupportedByAzure)
                {
                    if (depthMode == DepthMode.Off && colorResolution == ColorResolution.Off)
                        continue;

                    var calibration = Calibration.CreateFromRaw(rawCalibration, depthMode, colorResolution);

                    Assert.IsTrue(calibration.IsValid);

                    Assert.AreEqual(depthMode, calibration.DepthMode);
                    Assert.AreEqual(colorResolution, calibration.ColorResolution);
                }
            }
        }

        private byte[] ReadRawCalibrationFromResources()
        {
            var assembly = GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream("K4AdotNet.Tests.Unit.raw_calibration.bin"))
            {
                var rawCalibration = new byte[stream.Length];
                var len = stream.Read(rawCalibration, 0, rawCalibration.Length);
                System.Diagnostics.Debug.Assert(len == rawCalibration.Length);
                return rawCalibration;
            }
        }

        #endregion

        #region Dummy calibration

        [TestMethod]
        public void TestDummyCalibration()
        {
            TestDummyCalibration(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestDummyCalibration(DepthMode.NarrowViewUnbinned, ColorResolution.Off);
#if !ORBBECSDK_K4A_WRAPPER
            TestDummyCalibration(DepthMode.Off, ColorResolution.R1536p);
            TestDummyCalibration(DepthMode.PassiveIR, ColorResolution.R3072p);
#endif
        }

        private void TestDummyCalibration(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution);

            Assert.IsTrue(calibration.IsValid);

            Assert.AreEqual(depthMode, calibration.DepthMode);
            Assert.AreEqual(colorResolution, calibration.ColorResolution);
        }

#endregion

        #region Convert2DTo2D

        [TestMethod]
        public void TestConvert2DTo2D()
        {
            TestConvert2DTo2D(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestConvert2DTo2D(DepthMode.NarrowViewUnbinned, ColorResolution.R1080p);
#if !ORBBECSDK_K4A_WRAPPER
            TestConvert2DTo2D(DepthMode.WideView2x2Binned, ColorResolution.R1536p);
            TestConvert2DTo2D(DepthMode.WideViewUnbinned, ColorResolution.R3072p);
#endif
        }

        private void TestConvert2DTo2D(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution);

            var point2d = calibration.Convert2DTo2D(new Float2(100f, 10f), 2000f, CalibrationGeometry.Depth, CalibrationGeometry.Depth);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(100f, point2d.Value.X);
            Assert.AreEqual(10f, point2d.Value.Y);

            point2d = calibration.Convert2DTo2D(new Float2(10f, 100f), 3000f, CalibrationGeometry.Color, CalibrationGeometry.Color);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(10f, point2d.Value.X);
            Assert.AreEqual(100f, point2d.Value.Y);

            var depthCenter = new Float2(calibration.DepthCameraCalibration.Intrinsics.Parameters.Cx, calibration.DepthCameraCalibration.Intrinsics.Parameters.Cy);
            var colorCenter = new Float2(calibration.ColorCameraCalibration.Intrinsics.Parameters.Cx, calibration.ColorCameraCalibration.Intrinsics.Parameters.Cy);

            point2d = calibration.Convert2DTo2D(depthCenter, 1000f, CalibrationGeometry.Depth, CalibrationGeometry.Color);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(colorCenter, point2d);

            point2d = calibration.Convert2DTo2D(colorCenter, 2000f, CalibrationGeometry.Color, CalibrationGeometry.Depth);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(depthCenter, point2d);

            point2d = calibration.Convert2DTo2D(depthCenter, 0f, CalibrationGeometry.Depth, CalibrationGeometry.Color);
            Assert.IsNull(point2d);

            point2d = calibration.Convert2DTo2D(colorCenter, -10f, CalibrationGeometry.Color, CalibrationGeometry.Depth);
            Assert.IsNull(point2d);
        }

#endregion

        #region Convert2DTo3D

        [TestMethod]
        public void TestConvert2DTo3D()
        {
            TestConvert2DTo3D(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestConvert2DTo3D(DepthMode.NarrowViewUnbinned, ColorResolution.R1080p);
#if !ORBBECSDK_K4A_WRAPPER
            TestConvert2DTo3D(DepthMode.WideView2x2Binned, ColorResolution.R1536p);
            TestConvert2DTo3D(DepthMode.WideViewUnbinned, ColorResolution.R3072p);
#endif
        }

        private void TestConvert2DTo3D(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution);

            var depthCenter = new Float2(calibration.DepthCameraCalibration.Intrinsics.Parameters.Cx, calibration.DepthCameraCalibration.Intrinsics.Parameters.Cy);
            var colorCenter = new Float2(calibration.ColorCameraCalibration.Intrinsics.Parameters.Cx, calibration.ColorCameraCalibration.Intrinsics.Parameters.Cy);

            var point3d = calibration.Convert2DTo3D(depthCenter, 1000f, CalibrationGeometry.Depth, CalibrationGeometry.Depth);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(0f, point3d.Value.X);
            Assert.AreEqual(0f, point3d.Value.Y);
            Assert.AreEqual(1000f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(colorCenter, 2000f, CalibrationGeometry.Color, CalibrationGeometry.Color);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(0f, point3d.Value.X);
            Assert.AreEqual(0f, point3d.Value.Y);
            Assert.AreEqual(2000f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(colorCenter, 3000f, CalibrationGeometry.Color, CalibrationGeometry.Depth);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(0f, point3d.Value.X);
            Assert.AreEqual(0f, point3d.Value.Y);
            Assert.AreEqual(3000f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(depthCenter, 4000f, CalibrationGeometry.Depth, CalibrationGeometry.Color);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(0f, point3d.Value.X);
            Assert.AreEqual(0f, point3d.Value.Y);
            Assert.AreEqual(4000f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(depthCenter, 500f, CalibrationGeometry.Depth, CalibrationGeometry.Accel);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(0f, point3d.Value.X);
            Assert.AreEqual(0f, point3d.Value.Y);
            Assert.AreEqual(500f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(depthCenter, -500f, CalibrationGeometry.Depth, CalibrationGeometry.Color);
            Assert.IsNotNull(point3d);
            Assert.AreEqual(-500f, point3d.Value.Z);

            point3d = calibration.Convert2DTo3D(new Float2(50f, 100f), 0f, CalibrationGeometry.Color, CalibrationGeometry.Depth);
            Assert.IsNull(point3d);
        }

#endregion

        #region Convert3DTo2D

        [TestMethod]
        public void TestConvert3DTo2D()
        {
            TestConvert3DTo2D(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestConvert3DTo2D(DepthMode.NarrowViewUnbinned, ColorResolution.R1080p);
#if !ORBBECSDK_K4A_WRAPPER
            TestConvert3DTo2D(DepthMode.WideView2x2Binned, ColorResolution.R1536p);
            TestConvert3DTo2D(DepthMode.WideViewUnbinned, ColorResolution.R3072p);
#endif
        }

        private void TestConvert3DTo2D(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution);

            var depthCenter = new Float2(calibration.DepthCameraCalibration.Intrinsics.Parameters.Cx, calibration.DepthCameraCalibration.Intrinsics.Parameters.Cy);

            var point2d = calibration.Convert3DTo2D(new Float3(0f, 0f, 1000f), CalibrationGeometry.Depth, CalibrationGeometry.Depth);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(depthCenter, point2d.Value);

            point2d = calibration.Convert3DTo2D(new Float3(0f, 0f, 2000f), CalibrationGeometry.Gyro, CalibrationGeometry.Depth);
            Assert.IsNotNull(point2d);
            Assert.AreEqual(depthCenter, point2d.Value);
        }

#endregion

        #region Convert3DTo3D

        [TestMethod]
        public void TestConvert3DTo3D()
        {
            TestConvert3DTo3D(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestConvert3DTo3D(DepthMode.NarrowViewUnbinned, ColorResolution.R1080p);
#if !ORBBECSDK_K4A_WRAPPER
            TestConvert3DTo3D(DepthMode.WideView2x2Binned, ColorResolution.R1536p);
            TestConvert3DTo3D(DepthMode.WideViewUnbinned, ColorResolution.R3072p);
#endif
        }

        private void TestConvert3DTo3D(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution);

            var testPoint = new Float3(10f, 10f, 1000f);

            var point3d = calibration.Convert3DTo3D(testPoint, CalibrationGeometry.Gyro, CalibrationGeometry.Accel);
            Assert.AreEqual(testPoint, point3d);
        }

#endregion

        #region ConvertColor2DToDepth2D

        [TestMethod]
        public void TestConvertColor2DToDepth2D()
        {
            TestConvertColor2DToDepth2D(DepthMode.NarrowView2x2Binned, ColorResolution.R720p);
            TestConvertColor2DToDepth2D(DepthMode.NarrowViewUnbinned, ColorResolution.R1080p);
#if !ORBBECSDK_K4A_WRAPPER
            TestConvertColor2DToDepth2D(DepthMode.WideView2x2Binned, ColorResolution.R1536p);
            TestConvertColor2DToDepth2D(DepthMode.WideViewUnbinned, ColorResolution.R3072p);
#endif
        }

        private void TestConvertColor2DToDepth2D(DepthMode depthMode, ColorResolution colorResolution)
        {
            var calibration = Calibration.CreateDummy(depthMode, colorResolution, 30);

            var depth2d = new Float2(calibration.DepthCameraCalibration.Intrinsics.Parameters.Cx, calibration.DepthCameraCalibration.Intrinsics.Parameters.Cy);
            var depthMm = (short)1800;
            var color2d = calibration.Convert2DTo2D(depth2d, depthMm, CalibrationGeometry.Depth, CalibrationGeometry.Color).Value;

            var depthImageBuffer = new short[depthMode.WidthPixels() * depthMode.HeightPixels()];
            for (var i = 0; i < depthImageBuffer.Length; i++)
                depthImageBuffer[i] = depthMm;
            var depthImage = new Image.Azure(ImageFormat.Depth16, depthMode.WidthPixels(), depthMode.HeightPixels());
            depthImage.FillFrom(depthImageBuffer);

            var point2d = calibration.ConvertColor2DToDepth2D(color2d, depthImage);
            Assert.IsNotNull(point2d);
            Assert.IsTrue(Math.Abs(depth2d.X - point2d.Value.X) < 1f);
            Assert.IsTrue(Math.Abs(depth2d.Y - point2d.Value.Y) < 1f);

            depthImage.Dispose();
        }

        #endregion

        #region Transform tests

        [TestMethod]
        public void TestTransformToPointCloud()
        {
            var depthMode = DepthMode.NarrowViewUnbinned;
            var colorResolution = ColorResolution.R720p;

            var calibration = Calibration.CreateDummy(depthMode, colorResolution, 30);

            var depthMm = (short)1_000;

            var depthImageBuffer = new short[depthMode.WidthPixels() * depthMode.HeightPixels()];
            for (var i = 0; i < depthImageBuffer.Length; i++)
                depthImageBuffer[i] = depthMm;
            var depthImage = new Image.Azure(ImageFormat.Depth16, depthMode.WidthPixels(), depthMode.HeightPixels());
            depthImage.FillFrom(depthImageBuffer);

            var xyzImage = new Image.Azure(ImageFormat.Custom, depthMode.WidthPixels(), depthMode.HeightPixels(), depthMode.WidthPixels() * 6);

            using (var transform = calibration.CreateTransformation())
            {
                transform.DepthImageToPointCloud(depthImage, CalibrationGeometry.Depth, xyzImage);
                var xyzData = new short[depthMode.WidthPixels() * depthMode.HeightPixels() * 3];
                Marshal.Copy(xyzImage.Buffer, xyzData, 0, xyzData.Length);

                var indx = 0;
                for (var y = 0; y < depthMode.HeightPixels(); y++)
                {
                    for (var x = 0; x < depthMode.WidthPixels(); x++)
                    {
                        var p3d = calibration.Convert2DTo3D(new Float2(x, y), depthMm, CalibrationGeometry.Depth, CalibrationGeometry.Depth);
                        Assert.IsNotNull(p3d);

                        Assert.IsTrue(Math.Abs(p3d.Value.X - xyzData[indx++]) < 1);
                        Assert.IsTrue(Math.Abs(p3d.Value.Y - xyzData[indx++]) < 1);
                        Assert.IsTrue(Math.Abs(p3d.Value.Z - depthMm) < 1);
                        Assert.IsTrue(Math.Abs(depthMm - xyzData[indx++]) < 1);
                    }
                }
            }

            xyzImage.Dispose();
            depthImage.Dispose();
        }

        #endregion
    }
}
