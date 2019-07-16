using Microsoft.VisualStudio.TestTools.UnitTesting;
using K4AdotNet.Sensor;
using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.SensorTypesUnitTests
{
    [TestClass]
    public class ImageTests
    {
        private static readonly int testWidth = 32;
        private static readonly int testHeight = 16;
        private static readonly Microseconds64 testTimestamp = Microseconds64.FromSeconds(1.5);
        private static readonly int testWhiteBalance = 300;
        private static readonly int testIsoSpeed = 100;

        #region Testing image creation without size specifying

        [TestMethod]
        public void TestDepthMapCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.Depth16);

        [TestMethod]
        public void TestIRImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.IR16);

        [TestMethod]
        public void TestColorBgraImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorBgra32);

        [TestMethod]
        public void TestColorNV12ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorNV12);

        [TestMethod]
        public void TestColorYUY2ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorYUY2);

        [TestMethod]
        public void TestCustomImageWithKnownStrideCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.Custom, testWidth * 6);

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreationWithZeroStrideAndNoSize()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorMjpg, 0);

        private void TestImageCreationWithNoSizeSpecified(ImageFormat format, int? strideOrNull = null)
        {
            var stride = strideOrNull ?? format.StrideBytes(testWidth);
            var expectedSize = stride * testHeight;

            var image = new Image(format, testWidth, testHeight, stride);

            Assert.AreNotEqual(IntPtr.Zero, image.Buffer);

            // Check common properties
            Assert.AreEqual(format, image.Format);
            Assert.AreEqual(testWidth, image.WidthPixels);
            Assert.AreEqual(testHeight, image.HeightPixels);
            Assert.AreEqual(stride, image.StrideBytes);
            Assert.AreEqual(expectedSize, image.SizeBytes);

            // Check Timestamp property
            Assert.AreEqual(Microseconds64.Zero, image.Timestamp);
            image.Timestamp = testTimestamp;
            Assert.AreEqual(testTimestamp, image.Timestamp);

            // Check WhiteBalance property
            Assert.AreEqual(0, image.WhiteBalance);
            image.WhiteBalance = testWhiteBalance;
            Assert.AreEqual(testWhiteBalance, image.WhiteBalance);

            // Check IsoSpeed property
            Assert.AreEqual(0, image.IsoSpeed);
            image.IsoSpeed = testIsoSpeed;
            Assert.AreEqual(testIsoSpeed, image.IsoSpeed);

            // Check disposing
            Assert.IsFalse(image.IsDisposed);
            var disposedEventCounter = 0;
            image.Disposed += (_, __) => disposedEventCounter++;
            image.Dispose();
            Assert.IsTrue(image.IsDisposed);
            Assert.AreEqual(1, disposedEventCounter);

            // We can call Dispose() multiple times
            image.Dispose();
            Assert.IsTrue(image.IsDisposed);
            // But Disposed event must be invoked only once
            Assert.AreEqual(1, disposedEventCounter);
        }

        #endregion

        #region Testing of image creation with size specified and from array

        [TestMethod]
        public void TestCreationWithSizeSpecified()
        {
            var format = ImageFormat.ColorMjpg;
            var strideBytes = 0;
            var sizeBytes = testWidth * testHeight * 2 + 1976;

            using (var image = new Image(format, testWidth, testHeight, strideBytes, sizeBytes))
            {
                Assert.AreNotEqual(IntPtr.Zero, image.Buffer);
                Assert.AreEqual(format, image.Format);
                Assert.AreEqual(testWidth, image.WidthPixels);
                Assert.AreEqual(testHeight, image.HeightPixels);
                Assert.AreEqual(strideBytes, image.StrideBytes);
                Assert.AreEqual(sizeBytes, image.SizeBytes);
            }
        }

        [TestMethod]
        public void TestCreationFromArray()
        {
            var format = ImageFormat.Depth16;
            var strideBytes = format.StrideBytes(testWidth);
            var lengthElements = testWidth * testHeight;
            var array = new short[lengthElements];

            using (var image = Image.CreateFromArray(array, format, testWidth, testHeight, strideBytes))
            {
                Assert.AreNotEqual(IntPtr.Zero, image.Buffer);
                Assert.AreEqual(format, image.Format);
                Assert.AreEqual(testWidth, image.WidthPixels);
                Assert.AreEqual(testHeight, image.HeightPixels);
                Assert.AreEqual(strideBytes, image.StrideBytes);
                Assert.AreEqual(array.Length * sizeof(short), image.SizeBytes);

                // Check that Buffer points to array
                for (var i = 0; i < array.Length; i++)
                    array[i] = unchecked((short)i);

                var buffer = image.Buffer;
                for (var i = 0; i < array.Length; i++)
                    Assert.AreEqual(array[i], Marshal.ReadInt16(buffer, i * sizeof(short)));

                Marshal.WriteInt16(buffer, ofs: 123 * sizeof(short), val: 2019);
                Assert.AreEqual(2019, array[123]);
            }
        }

        #endregion
    }
}
