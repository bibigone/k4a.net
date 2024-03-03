using Microsoft.VisualStudio.TestTools.UnitTesting;
using K4AdotNet.Sensor;
using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Unit.Sensor
{
    [TestClass]
    public class ImageTests
    {
        private static readonly int testWidth = 32;
        private static readonly int testHeight = 16;
        private static readonly Microseconds64 testDeviceTimestamp = Microseconds64.FromSeconds(1.5);
        private static readonly Nanoseconds64 testSystemTimestamp = Nanoseconds64.FromSeconds(1.5001);

#if !ORBBECSDK_K4A_WRAPPER
        private static readonly int testWhiteBalance = 300;
        private static readonly int testIsoSpeed = 100;
#endif

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
#if ORBBECSDK_K4A_WRAPPER
        [Ignore("OrbbecSDK-K4A-Wrapper does not support stride and size calculation for NV12 format")]
#endif
        public void TestColorNV12ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorNV12);

        [TestMethod]
        public void TestColorYUY2ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.ColorYUY2);

        [TestMethod]
        public void TestCustom8ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.Custom8);

        [TestMethod]
        public void TestCustom16ImageCreation()
            => TestImageCreationWithNoSizeSpecified(ImageFormat.Custom16);

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
            if (format == ImageFormat.ColorNV12)
                expectedSize = expectedSize * 3 / 2;

            var image = strideOrNull.HasValue
                ? new Image(format, testWidth, testHeight, strideOrNull.Value)
                : new Image(format, testWidth, testHeight);

            Assert.AreNotEqual(IntPtr.Zero, image.Buffer);

            // Check common (readonly) properties
            Assert.AreEqual(format, image.Format);
            Assert.AreEqual(testWidth, image.WidthPixels);
            Assert.AreEqual(testHeight, image.HeightPixels);
#if !ORBBECSDK_K4A_WRAPPER
            Assert.AreEqual(stride, image.StrideBytes);
#endif
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
#if ORBBECSDK_K4A_WRAPPER
            var format = ImageFormat.Custom16;
#else
            var format = ImageFormat.Depth16;
#endif
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

        [TestMethod]
        public void TestCreationFromMemory()
        {
#if ORBBECSDK_K4A_WRAPPER
            var format = ImageFormat.Custom16;
#else
            var format = ImageFormat.Depth16;
#endif
            var strideBytes = format.StrideBytes(testWidth);
            var lengthElements = testWidth * testHeight;
            var array = new short[lengthElements];
            var owner = new TestMemoryOwner(array);

            using (var image = Image.CreateFromMemory(owner, format, testWidth, testHeight, strideBytes))
            {
                Assert.AreNotEqual(IntPtr.Zero, image.Buffer);
                Assert.AreEqual(format, image.Format);
                Assert.AreEqual(testWidth, image.WidthPixels);
                Assert.AreEqual(testHeight, image.HeightPixels);
                Assert.AreEqual(strideBytes, image.StrideBytes);
                Assert.AreEqual(owner.Memory.Length * sizeof(short), image.SizeBytes);

                // Check that Buffer points to array
                for (var i = 0; i < array.Length; i++)
                    array[i] = unchecked((short)i);

                var buffer = image.Buffer;
                for (var i = 0; i < array.Length; i++)
                    Assert.AreEqual(array[i], Marshal.ReadInt16(buffer, i * sizeof(short)));

                Marshal.WriteInt16(buffer, ofs: 123 * sizeof(short), val: 2019);
                Assert.AreEqual(2019, array[123]);

                Assert.IsFalse(owner.IsDisposed);
            }

            Assert.IsTrue(owner.IsDisposed);
        }

        private sealed class TestMemoryOwner : System.Buffers.IMemoryOwner<short>
        {
            private readonly short[] buffer;
            private volatile bool isDisposed;
            public TestMemoryOwner(short[] buffer) => this.buffer = buffer;
            public Memory<short> Memory => buffer;
            public bool IsDisposed => isDisposed;
            public void Dispose() => isDisposed = true;
            
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

            Assert.IsFalse(weakReferenceToArray.TryGetTarget(out _));

            // Nothing bad if we call dispose second time
            image.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestObjectDisposedException()
        {
            var image = new Image(ImageFormat.Depth16, testWidth, testHeight);
            image.Dispose();
            _ = image.Buffer;      // <- ObjectDisposedException
        }

        #endregion

        #region Test setting of properties

        [TestMethod]
        public void TestMutableProperties()
        {
            using (var image = new Image(ImageFormat.Depth16, testWidth, testHeight))
            {
                // Check DeviceTimestamp property
                Assert.AreEqual(Microseconds64.Zero, image.DeviceTimestamp);
                image.DeviceTimestamp = testDeviceTimestamp;
                Assert.AreEqual(testDeviceTimestamp, image.DeviceTimestamp);

                // Check SystemTimestamp property
                Assert.AreEqual(Nanoseconds64.Zero, image.SystemTimestamp);
                image.SystemTimestamp = testSystemTimestamp;
#if !ORBBECSDK_K4A_WRAPPER
                Assert.AreEqual(testSystemTimestamp, image.SystemTimestamp);
#else
                Assert.AreEqual(testSystemTimestamp.ValueNsec / 1_000_000L, image.SystemTimestamp.ValueNsec / 1_000_000L);
#endif

#if !ORBBECSDK_K4A_WRAPPER
                // Check WhiteBalance property
                Assert.AreEqual(0, image.WhiteBalance);
                image.WhiteBalance = testWhiteBalance;
                Assert.AreEqual(testWhiteBalance, image.WhiteBalance);

                // Check IsoSpeed property
                Assert.AreEqual(0, image.IsoSpeed);
                image.IsoSpeed = testIsoSpeed;
                Assert.AreEqual(testIsoSpeed, image.IsoSpeed);
#endif
            }
        }

#endregion

        #region Test duplicate reference

        [TestMethod]
        public void TestDuplicateReference()
        {
            var image = CreateImageFromArray(out var weakReferenceToArray);
            var refImage = image.DuplicateReference();

            Assert.AreEqual(image, refImage);
            Assert.IsTrue(image == refImage);
            Assert.IsFalse(image != refImage);

            Assert.AreEqual(image.Buffer, refImage.Buffer);
            Assert.AreEqual(image.SizeBytes, refImage.SizeBytes);
            Assert.AreEqual(image.WidthPixels, refImage.WidthPixels);
            Assert.AreEqual(image.HeightPixels, refImage.HeightPixels);
            Assert.AreEqual(image.StrideBytes, refImage.StrideBytes);
            Assert.AreEqual(image.Format, refImage.Format);

            // Check that when we change property of image,
            // then property of refImage is also synchronously changed
            image.DeviceTimestamp = testDeviceTimestamp;
            Assert.AreEqual(testDeviceTimestamp, refImage.DeviceTimestamp);

#if !ORBBECSDK_K4A_WRAPPER
            // And vice versa
            refImage.WhiteBalance = testWhiteBalance;
            Assert.AreEqual(testWhiteBalance, image.WhiteBalance);

            // And for one more property
            image.IsoSpeed = testIsoSpeed;
            Assert.AreEqual(testIsoSpeed, refImage.IsoSpeed);
#endif

            // Dispose source image
            image.Dispose();

            // But refImage must be still alive
            Assert.IsFalse(refImage.IsDisposed);
            Assert.AreNotEqual(IntPtr.Zero, refImage.Buffer);

            // Force GC
            GC.Collect();

            // But array is still alive because refImage keeps it
            Assert.IsTrue(weakReferenceToArray.TryGetTarget(out _));

            refImage.Dispose();
        }

#endregion

        #region Testing of CopyTo and FillFrom

        [TestMethod]
        public void TestCopyToAndFillFromForByteArray()
        {
            using (var image = new Image(ImageFormat.ColorBgra32, 1, 1))
            {
                var src = new byte[] { 127, 63, 255, 0 };
                image.FillFrom(src);
                var dst = new byte[4];
                image.CopyTo(dst);
                for (var i = 0; i < dst.Length; i++)
                    Assert.AreEqual(src[i], dst[i]);
            }
        }

        [TestMethod]
        public void TestCopyToAndFillFromForShortArray()
        {
            using (var image = new Image(ImageFormat.Depth16, 2, 2))
            {
                var src = new short[] { -1234, 0, short.MinValue, short.MaxValue };
                image.FillFrom(src);
                var dst = new short[4];
                image.CopyTo(dst);
                for (var i = 0; i < dst.Length; i++)
                    Assert.AreEqual(src[i], dst[i]);
            }
        }

        [TestMethod]
        public void TestCopyToAndFillFromForIntArray()
        {
            using (var image = new Image(ImageFormat.Custom, 2, 2, 2 * sizeof(int)))
            {
                var src = new int[] { -1234, 0, int.MinValue, int.MaxValue };
                image.FillFrom(src);
                var dst = new int[4];
                image.CopyTo(dst);
                for (var i = 0; i < dst.Length; i++)
                    Assert.AreEqual(src[i], dst[i]);
            }
        }

        [TestMethod]
        public void TestCopyToAndFillFromForFloatArray()
        {
            using (var image = new Image(ImageFormat.Custom, 2, 2, 2 * sizeof(float)))
            {
                var src = new float[] { -1234.56f, 0f, float.MinValue, float.MaxValue };
                image.FillFrom(src);
                var dst = new float[4];
                image.CopyTo(dst);
                for (var i = 0; i < dst.Length; i++)
                    Assert.AreEqual(src[i], dst[i]);
            }
        }

        #endregion

        #region Test image size calculations

        [TestMethod]
#if ORBBECSDK_K4A_WRAPPER
        [Ignore("OrbbecSDK-K4A-Wrapper does not support stride and size calculation for NV12 format")]
#endif
        public void TestImageSizeCalculationNV12()
        {
            using (var image = new Image(ImageFormat.ColorNV12, 1280, 720))
            {
                Assert.AreEqual(image.StrideBytes, ImageFormat.ColorNV12.StrideBytes(1280));
                Assert.AreEqual(image.SizeBytes, ImageFormat.ColorNV12.ImageSizeBytes(image.StrideBytes, 720));
            }
        }

        [TestMethod]
        public void TestImageSizeCalculationYUY2()
        {
            using (var image = new Image(ImageFormat.ColorYUY2, 1280, 720))
            {
                Assert.AreEqual(image.StrideBytes, ImageFormat.ColorYUY2.StrideBytes(1280));
                Assert.AreEqual(image.SizeBytes, ImageFormat.ColorYUY2.ImageSizeBytes(image.StrideBytes, 720));
            }
        }

        [TestMethod]
        public void TestImageSizeCalculationDepth()
        {
            using (var image = new Image(ImageFormat.Depth16, 1280, 720))
            {
                Assert.AreEqual(image.StrideBytes, ImageFormat.Depth16.StrideBytes(1280));
                Assert.AreEqual(image.SizeBytes, ImageFormat.Depth16.ImageSizeBytes(image.StrideBytes, 720));
            }
        }

        [TestMethod]
        public void TestImageSizeCalculationIR16()
        {
            using (var image = new Image(ImageFormat.IR16, 1280, 720))
            {
                Assert.AreEqual(image.StrideBytes, ImageFormat.IR16.StrideBytes(1280));
                Assert.AreEqual(image.SizeBytes, ImageFormat.IR16.ImageSizeBytes(image.StrideBytes, 720));
            }
        }

#if !ORBBECSDK_K4A_WRAPPER
        [TestMethod]
        public void TestImageSizeCalculationCustom()
        {
            var bytesPerPixel = 4;
            using (var image = new Image(ImageFormat.Custom, 1280, 720, 1280 * bytesPerPixel))
            {
                Assert.AreEqual(image.SizeBytes, ImageFormat.Custom.ImageSizeBytes(image.StrideBytes, 720));
            }
        }
#endif

#endregion

#if !ORBBECSDK_K4A_WRAPPER

        #region Test custom memory management

        private sealed class TestMemoryAllocator : ICustomMemoryAllocator
        {
            public int AllocateCount { get; private set; }
            public nint AllocContextValue { get; set; }
            public int LastAllocSizeValue { get; private set; }
            public nint LastAllocReturnValue { get; private set; }

            public int FreeCount { get; private set; }
            public nint LastFreeContextValue { get; private set; }

            nint ICustomMemoryAllocator.Allocate(int size, out nint context)
            {
                AllocateCount++;
                context = AllocContextValue;
                LastAllocSizeValue = size;
                return LastAllocReturnValue = Marshal.AllocHGlobal(size);
            }

            void ICustomMemoryAllocator.Free(nint buffer, nint context)
            {
                FreeCount++;
                LastFreeContextValue = context;
                Marshal.FreeHGlobal(buffer);
            }
        }

        [TestMethod]
        public void TestCustomMemoryManagement()
        {
            // Set our test custom allocator
            var testAllocator = new TestMemoryAllocator();
            Sdk.CustomMemoryAllocator = testAllocator;
            // Check initial state
            Assert.AreEqual(0, testAllocator.AllocateCount);
            Assert.AreEqual(0, testAllocator.FreeCount);

            // The first test image - should result in one memory allocation
            var testContextA = testAllocator.AllocContextValue = 12345;
            var testImageA = new Image(ImageFormat.Depth16, 1, 1);
            // One allocation but no calls of Free
            Assert.AreEqual(1, testAllocator.AllocateCount);    // 0 -> 1 !
            Assert.AreEqual(0, testAllocator.FreeCount);        // unchanged
            // SDK can request bigger buffer and place actual image buffer somewhere inside allocated one
            Assert.IsTrue(testAllocator.LastAllocSizeValue >= 2);
            Assert.IsTrue(testAllocator.LastAllocReturnValue <= testImageA.Buffer);
            Assert.IsTrue(testAllocator.LastAllocReturnValue + testAllocator.LastAllocSizeValue >= testImageA.Buffer + 2);

            // The second test image - should result in one more memory allocation
            var testContextB = testAllocator.AllocContextValue = 98765;
            var testImageB = new Image(ImageFormat.ColorBgra32, 1, 1);
            // Two allocations but still no calls of Free
            Assert.AreEqual(2, testAllocator.AllocateCount);    // 1 -> 2 !
            Assert.AreEqual(0, testAllocator.FreeCount);        // unchanged
            // SDK can request bigger buffer and place actual image buffer somewhere inside allocated one
            Assert.IsTrue(testAllocator.LastAllocSizeValue >= 4);
            Assert.IsTrue(testAllocator.LastAllocReturnValue <= testImageB.Buffer);
            Assert.IsTrue(testAllocator.LastAllocReturnValue + testAllocator.LastAllocSizeValue >= testImageB.Buffer + 4);

            // Disposing of the first test image - should result in appropriate call of Free method
            testImageA.Dispose();
            // Now, one call to Free
            Assert.AreEqual(2, testAllocator.AllocateCount);    // unchanged
            Assert.AreEqual(1, testAllocator.FreeCount);        // 0 -> 1 !
            Assert.AreEqual(testContextA, testAllocator.LastFreeContextValue);

            // Clear custom allocator
            Sdk.CustomMemoryAllocator = null;

            // Now creation of test image does not result in calls to our testAllocator instance
            var testImageC = new Image(ImageFormat.ColorYUY2, testWidth, testHeight);
            Assert.AreEqual(2, testAllocator.AllocateCount);
            Assert.AreEqual(1, testAllocator.FreeCount);

            // The second test image was created with the aid of out test allocator,
            // this why it should be releasing using the same allocator in spite of the fact that this allocator is not active anymore
            testImageB.Dispose();
            Assert.AreEqual(2, testAllocator.AllocateCount);    // unchanged
            Assert.AreEqual(2, testAllocator.FreeCount);        // 1 -> 2 !
            Assert.AreEqual(testContextB, testAllocator.LastFreeContextValue);

            // But for the testImageC our testAllocator shouldn't be called on releasing either
            testImageC.Dispose();
            Assert.AreEqual(2, testAllocator.AllocateCount);    // unchanged
            Assert.AreEqual(2, testAllocator.FreeCount);        // unchanged value!
        }

        #endregion

#endif
    }
}
