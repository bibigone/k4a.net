using K4AdotNet.Sensor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace K4AdotNet.Tests.Unit.Sensor
{
    [TestClass]
    public class CaptureTests
    {
        [TestMethod]
        public void TestEmptyCaptureCreation()
        {
            using (var capture = Capture.Create())
            {
                Assert.IsFalse(capture.IsDisposed);
                Assert.IsNull(capture.ColorImage);
                Assert.IsNull(capture.DepthImage);
                Assert.IsNull(capture.IRImage);
                if (capture is Capture.Azure captureAzure)
                    Assert.IsTrue(float.IsNaN(captureAzure.TemperatureC));
            }
        }

        [TestMethod]
        public void TestDisposing()
        {
            var capture = Capture.Create();

            // Check disposing
            Assert.IsFalse(capture.IsDisposed);
            var disposedEventCounter = 0;
            capture.Disposed += (_, __) => disposedEventCounter++;
            capture.Dispose();
            Assert.IsTrue(capture.IsDisposed);
            Assert.AreEqual(1, disposedEventCounter);

            // We can call Dispose() multiple times
            capture.Dispose();
            Assert.IsTrue(capture.IsDisposed);
            // But Disposed event must be invoked only once
            Assert.AreEqual(1, disposedEventCounter);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestObjectDisposedException()
        {
            var capture = Capture.Create();
            capture.Dispose();
            _ = capture.DepthImage;     // <- ObjectDisposedException
        }

        [TestMethod]
        public void TestSettingOfColorImage()
        {
            using (var capture = Capture.Create())
            {
                IntPtr imageBuffer;

                using (var image = new Image.Azure(ImageFormat.ColorBgra32, 2, 2))
                {
                    capture.ColorImage = image;

                    using (var retImage = capture.ColorImage)
                    {
                        Assert.IsNotNull(retImage);
                        Assert.AreEqual(image, retImage);
                    }

                    imageBuffer = image.Buffer;
                }

                // We can dispose original image, but capture will keep reference to it!
                using (var retImage = capture.ColorImage)
                {
                    Assert.IsNotNull(retImage);
                    Assert.AreEqual(imageBuffer, retImage.Buffer);
                }

#if !ORBBECSDK_K4A_WRAPPER
                // Clear
                capture.ColorImage = null;
                Assert.IsNull(capture.ColorImage);
#endif
            }
        }

        [TestMethod]
        public void TestSettingOfDepthImage()
        {
            using (var capture = Capture.Create())
            {
                IntPtr imageBuffer;

                using (var image = new Image.Azure(ImageFormat.Depth16, 2, 2))
                {
                    capture.DepthImage = image;

                    using (var retImage = capture.DepthImage)
                    {
                        Assert.IsNotNull(retImage);
                        Assert.AreEqual(image, retImage);
                    }

                    imageBuffer = image.Buffer;
                }

                // We can dispose original image, but capture will keep reference to it!
                using (var retImage = capture.DepthImage)
                {
                    Assert.IsNotNull(retImage);
                    Assert.AreEqual(imageBuffer, retImage.Buffer);
                }

#if !ORBBECSDK_K4A_WRAPPER
                // Clear
                capture.DepthImage = null;
                Assert.IsNull(capture.DepthImage);
#endif
            }
        }

        [TestMethod]
        public void TestSettingOfIRImage()
        {
            using (var capture = Capture.Create())
            {
                IntPtr imageBuffer;

                using (var image = new Image.Azure(ImageFormat.IR16, 2, 2))
                {
                    capture.IRImage = image;

                    using (var retImage = capture.IRImage)
                    {
                        Assert.IsNotNull(retImage);
                        Assert.AreEqual(image, retImage);
                    }

                    imageBuffer = image.Buffer;
                }

                // We can dispose original image, but capture will keep reference to it!
                using (var retImage = capture.IRImage)
                {
                    Assert.IsNotNull(retImage);
                    Assert.AreEqual(imageBuffer, retImage.Buffer);
                }

#if !ORBBECSDK_K4A_WRAPPER
                // Clear
                capture.IRImage = null;
                Assert.IsNull(capture.IRImage);
#endif
            }
        }

#if !ORBBECSDK_K4A_WRAPPER
        [TestMethod]
        public void TestSettingOfTemperature()
        {
            var testTemperatureC = 48.3f;

            using (var capture = Capture.Create())
            {
                if (capture is Capture.Azure captureAzure)
                {
                    captureAzure.TemperatureC = testTemperatureC;
                    Assert.AreEqual(testTemperatureC, captureAzure.TemperatureC);
                }
            }
        }
#endif

        [TestMethod]
        public void TestDuplicateReference()
        {
            var capture = Capture.Create();
            var refCapture = capture.DuplicateReference();

            Assert.AreEqual(capture, refCapture);
            Assert.IsTrue(capture == refCapture);
            Assert.IsFalse(capture != refCapture);

#if !ORBBECSDK_K4A_WRAPPER
            if (capture is Capture.Azure captureAzure)
            {
                var refCaptureAzure = (Capture.Azure)refCapture;
                var testTemperatureC = 48.3f;

                // Check that when we change property of source capture,
                // then property of refCapture is also synchronously changed
                captureAzure.TemperatureC = testTemperatureC;
                Assert.AreEqual(testTemperatureC, refCaptureAzure.TemperatureC);

                // And vice versa
                refCaptureAzure.TemperatureC = -testTemperatureC;
                Assert.AreEqual(-testTemperatureC, captureAzure.TemperatureC);
            }
#endif

            // And for image properties
            using (var depthMap = new Image.Azure(ImageFormat.Depth16, 2, 2))
            {
                capture.DepthImage = depthMap;
                using (var retDepthMap = refCapture.DepthImage)
                {
                    Assert.AreEqual(depthMap, retDepthMap);
                }
            }

            // Dispose source capture
            capture.Dispose();

            // But refCapture must be still alive
            Assert.IsFalse(refCapture.IsDisposed);

            refCapture.Dispose();
        }

        [TestMethod]
        public void TestAutomaticDisposingOfAllReturnedImages()
        {
            var capture = Capture.Create();
            using (var image = new Image.Azure(ImageFormat.ColorBgra32, 2, 2))
            {
                capture.ColorImage = image;
            }
            using (var image = new Image.Azure(ImageFormat.Depth16, 2, 2))
            {
                capture.DepthImage = image;
            }
            using (var image = new Image.Azure(ImageFormat.IR16, 2, 2))
            {
                capture.IRImage = image;
            }

            // Getting images
            var colorImage1 = capture.ColorImage;
            var colorImage2 = capture.ColorImage;   // multiple times
            var colorImage3 = capture.ColorImage;   // and one more
            var depthImage = capture.DepthImage;
            var irImage = capture.IRImage;

            // All returned images are alive
            Assert.IsFalse(colorImage1.IsDisposed);
            Assert.IsFalse(colorImage2.IsDisposed);
            Assert.IsFalse(colorImage3.IsDisposed);
            Assert.IsFalse(depthImage.IsDisposed);
            Assert.IsFalse(irImage.IsDisposed);

            // Explicit dispose colorImage2
            colorImage2.Dispose();

            // All returned images are alive except colorImage2
            Assert.IsFalse(colorImage1.IsDisposed);
            Assert.IsTrue(colorImage2.IsDisposed);
            Assert.IsFalse(colorImage3.IsDisposed);
            Assert.IsFalse(depthImage.IsDisposed);
            Assert.IsFalse(irImage.IsDisposed);

            // Dispose capture
            capture.Dispose();

            // All returned images - disposed
            Assert.IsTrue(colorImage1.IsDisposed);
            Assert.IsTrue(colorImage2.IsDisposed);
            Assert.IsTrue(colorImage3.IsDisposed);
            Assert.IsTrue(depthImage.IsDisposed);
            Assert.IsTrue(irImage.IsDisposed);

            // Nothing bad if we call Dispose() multiple times...
            capture.Dispose();

            Assert.IsTrue(colorImage1.IsDisposed);
            Assert.IsTrue(colorImage2.IsDisposed);
            Assert.IsTrue(colorImage3.IsDisposed);
            Assert.IsTrue(depthImage.IsDisposed);
            Assert.IsTrue(irImage.IsDisposed);
        }
    }
}