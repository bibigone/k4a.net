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

            var image = strideOrNull.HasValue
                ? new Image(format, testWidth, testHeight, strideOrNull.Value)
                : new Image(format, testWidth, testHeight);

            Assert.AreNotEqual(IntPtr.Zero, image.Buffer);

            // Check common (readonly) properties
            Assert.AreEqual(format, image.Format);
            Assert.AreEqual(testWidth, image.WidthPixels);
            Assert.AreEqual(testHeight, image.HeightPixels);
            Assert.AreEqual(stride, image.StrideBytes);
            Assert.AreEqual(expectedSize, image.SizeBytes);
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

        #region Testing of IsDisposed property, Dispose() method and Disposed event

        [TestMethod]
        public void TestDisposing()
        {
            var image = new Image(ImageFormat.Depth16, testWidth, testHeight);

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

        private Image CreateImageFromArray(out WeakReference<byte[]> weakReferenceToArray)
        {
            var format = ImageFormat.ColorBgra32;
            var strideBytes = format.StrideBytes(1);
            var array = new byte[strideBytes];
            weakReferenceToArray = new WeakReference<byte[]>(array);
            return Image.CreateFromArray(array, format, 1, 1, strideBytes);
        }

        [TestMethod]
        public void TestArrayUnpinningOnDispose()
        {
            var image = CreateImageFromArray(out var weakReferenceToArray);
            image.Dispose();

            // Force collecting of array
            GC.Collect();

            Assert.IsFalse(weakReferenceToArray.TryGetTarget(out var notUsed));

            // Nothing bad if we call dispose second time
            image.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestObjectDisposedException()
        {
            var image = new Image(ImageFormat.Depth16, testWidth, testHeight);
            image.Dispose();
            var buffer = image.Buffer;      // <- ObjectDisposedException
        }

        #endregion

        #region Test setting of properties

        [TestMethod]
        public void TestMutableProperties()
        {
            using (var image = new Image(ImageFormat.Depth16, testWidth, testHeight))
            {
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
            }
        }

        #endregion

        #region Test duplicate reference

        [TestMethod]
        public void TestDuplicateReference()
        {
            var image = CreateImageFromArray(out var weakReferenceToArray);
            var refImage = image.DuplicateReference();

            Assert.AreEqual(image.Buffer, refImage.Buffer);
            Assert.AreEqual(image.SizeBytes, refImage.SizeBytes);
            Assert.AreEqual(image.WidthPixels, refImage.WidthPixels);
            Assert.AreEqual(image.HeightPixels, refImage.HeightPixels);
            Assert.AreEqual(image.StrideBytes, refImage.StrideBytes);
            Assert.AreEqual(image.Format, refImage.Format);

            // Check that when we change property of image,
            // then property of refImage is also synchronously changed
            image.Timestamp = testTimestamp;
            Assert.AreEqual(testTimestamp, refImage.Timestamp);

            // And vice versa
            refImage.WhiteBalance = testWhiteBalance;
            Assert.AreEqual(testWhiteBalance, image.WhiteBalance);

            // And for one more property
            image.IsoSpeed = testIsoSpeed;
            Assert.AreEqual(testIsoSpeed, refImage.IsoSpeed);

            // Dispose source image
            image.Dispose();

            // But refImage must be still alive
            Assert.IsFalse(refImage.IsDisposed);
            Assert.AreNotEqual(IntPtr.Zero, refImage.Buffer);

            // Force GC
            GC.Collect();

            // But array is still alive because refImage keeps it
            Assert.IsTrue(weakReferenceToArray.TryGetTarget(out var notUsed));

            refImage.Dispose();
        }

        #endregion
    }
}
