using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    partial class Image
    {
        public sealed class Orbbec : Image
        {
            internal Orbbec(NativeHandles.ImageHandle handle)
                : base(handle) { }

            /// <summary>Creates new image with specified format and size in pixels.</summary>
            /// <param name="format">Format of image. Must be format with known stride: <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <remarks>
            /// This version of constructor can be used only for <paramref name="format"/> with known dependency between image width in pixels and stride in bytes
            /// and cannot be used for other formats. For details see <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.
            /// For other formats use <see cref="Orbbec(ImageFormat, int, int, int)"/> or <see cref="Orbbec(ImageFormat, int, int, int, int)"/>.
            /// </remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Image stride in bytes cannot be automatically calculated from <paramref name="widthPixels"/> for specified <paramref name="format"/>.
            /// </exception>
            public Orbbec(ImageFormat format, int widthPixels, int heightPixels)
                : this(format, widthPixels, heightPixels, format.StrideBytes(widthPixels))
            { }

            /// <summary>Creates new image with specified format, size in pixels and stride in bytes.</summary>
            /// <param name="format">Format of image. Cannot be <see cref="ImageFormat.ColorMjpg"/>.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be positive.</param>
            /// <remarks>
            /// This version of image construction should be preferred in case of OrbbecSDK-K4A-Wrapper usage.
            /// </remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// <paramref name="strideBytes"/> is equal to zero. In this case size of image in bytes must be specified to create image.
            /// </exception>
            public Orbbec(ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
                : base(CreateImage(NativeApi.Orbbec.Instance, format, widthPixels, heightPixels, strideBytes))
            { }

            /// <summary>Creates new image with specified format, size in pixels and stride in bytes.</summary>
            /// <param name="format">Format of image.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be non-negative. Zero value can be used for <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>.</param>
            /// <param name="sizeBytes">Size of image buffer in size. Non negative. Cannot be less than size calculated from image parameters.</param>
            /// <remarks>
            /// This version of image construction allocates memory buffer via <see cref="Sdk.CustomMemoryAllocator"/>
            /// or <see cref="HGlobalMemoryAllocator"/> if not set.
            /// </remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>
            /// or <paramref name="sizeBytes"/> is less than zero or <paramref name="sizeBytes"/> is less than size calculated from <paramref name="heightPixels"/> and <paramref name="strideBytes"/>.
            /// </exception>
            public Orbbec(ImageFormat format, int widthPixels, int heightPixels, int strideBytes, int sizeBytes)
                : base(CreateImage(format, widthPixels, heightPixels, strideBytes, sizeBytes))
            { }

            private static NativeHandles.ImageHandle CreateImage(ImageFormat format, int widthPixels, int heightPixels, int strideBytes, int sizeBytes)
            {
                // OrbbecSDK K4A Wrapper has limited support of image creation from provided memory buffer.
                // For this reason, trying to use "standard" image creation, if possible.
                if (strideBytes == 0 && sizeBytes % heightPixels == 0)
                    strideBytes = sizeBytes / heightPixels;
                if (strideBytes > 0 && strideBytes * heightPixels == sizeBytes)
                    return CreateImage(NativeApi.Orbbec.Instance, format, widthPixels, heightPixels, strideBytes);

                return CreateImage(NativeApi.Orbbec.Instance, format, widthPixels, heightPixels, strideBytes, sizeBytes);
            }

            public override Image DuplicateReference()
                => new Orbbec((NativeHandles.ImageHandle.Orbbec)handle.ValueNotDisposed.DuplicateReference());

            /// <summary>Creates new image for specified underlying buffer with specified format and size in pixels.</summary>
            /// <typeparam name="T">Type of array elements in underlying buffer. Must be value type.</typeparam>
            /// <param name="buffer">Underlying buffer for image. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
            /// <param name="format">Format of image. Must be format with known stride: <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <returns>Created image. Not <see langword="null"/>.</returns>
            /// <remarks><para>
            /// This version of method can be used only for <paramref name="format"/> with known dependency between image width in pixels and stride in bytes
            /// and cannot be used for other formats. For details see <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.
            /// For other formats use <see cref="CreateFromArray{T}(T[], ImageFormat, int, int, int)"/>.
            /// </para><para>
            /// <see cref="Buffer"/> points to pinned array <paramref name="buffer"/> for such images.
            /// </para><para>
            /// OrbbecSDK-K4A-Wrapper has limited support of this functionality.
            /// </para></remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or <paramref name="buffer"/> array is too small for specified image parameters.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Image stride in bytes cannot be automatically calculated from <paramref name="widthPixels"/> for specified <paramref name="format"/>.
            /// </exception>
            /// <seealso cref="ImageFormats.StrideBytes(ImageFormat, int)"/>
            public static new Orbbec CreateFromArray<T>(T[] buffer, ImageFormat format, int widthPixels, int heightPixels)
                where T : struct
                => CreateFromArray(buffer, format, widthPixels, heightPixels, format.StrideBytes(widthPixels));

            /// <summary>Creates new image for specified underlying buffer with specified format and size in pixels.</summary>
            /// <typeparam name="T">Type of array elements in underlying buffer. Must be value type.</typeparam>
            /// <param name="buffer">Underlying buffer for image. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
            /// <param name="format">Format of image.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be non-negative. Zero value can be used for <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>.</param>        /// <returns>Created image. Not <see langword="null"/>.</returns>
            /// <remarks><para>
            /// <see cref="Buffer"/> points to pinned array <paramref name="buffer"/> for such images.
            /// </para><para>
            /// OrbbecSDK-K4A-Wrapper has limited support of this functionality.
            /// </para></remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>
            /// or <paramref name="buffer"/> array is too small for specified image parameters.
            /// </exception>
            public static new Orbbec CreateFromArray<T>(T[] buffer, ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
                where T : struct
            {
                if (buffer is null)
                    throw new ArgumentNullException(nameof(buffer));
                if (widthPixels <= 0)
                    throw new ArgumentOutOfRangeException(nameof(widthPixels));
                if (heightPixels <= 0)
                    throw new ArgumentOutOfRangeException(nameof(heightPixels));
                if (strideBytes < 0)
                    throw new ArgumentOutOfRangeException(nameof(strideBytes));
                if (format.HasKnownBytesPerPixel() && strideBytes < widthPixels * format.BytesPerPixel())
                    throw new ArgumentOutOfRangeException(nameof(strideBytes));
                var sizeBytes = buffer.Length * Marshal.SizeOf<T>();
                if (strideBytes > 0 && sizeBytes < format.ImageSizeBytes(strideBytes, heightPixels))
                    throw new ArgumentOutOfRangeException(nameof(buffer) + "." + nameof(buffer.Length));

                var bufferPin = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                var bufferPtr = bufferPin.AddrOfPinnedObject();

                var res = NativeApi.Orbbec.Instance.ImageCreateFromBuffer(format, widthPixels, heightPixels, strideBytes,
                    bufferPtr, Helpers.Int32ToUIntPtr(sizeBytes),
                    pinnedArrayReleaseCallback, (IntPtr)bufferPin,
                    out var handle);
                if (res != NativeCallResults.Result.Succeeded || handle is null || handle.IsInvalid)
                {
                    bufferPin.Free();
                    throw new NotSupportedException("OrbbecSDK-K4A-Wrapper has limited support of this functionality. Please, prefer image creation via constructors.");
                }

                return (Orbbec)Create(handle)!;
            }

#if !(NETSTANDARD2_0 || NET461)
            /// <summary>Creates new image for specified underlying memory owner with specified format and size in pixels.</summary>
            /// <typeparam name="T">Type of elements in underlying memory buffer. Must be value type.</typeparam>
            /// <param name="memoryOwner">Memory owner of underlying buffer. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
            /// <param name="format">Format of image. Must be format with known stride: <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <returns>Created image. Not <see langword="null"/>.</returns>
            /// <remarks><para>
            /// This version of method can be used only for <paramref name="format"/> with known dependency between image width in pixels and stride in bytes
            /// and cannot be used for other formats. For details see <see cref="ImageFormats.StrideBytes(ImageFormat, int)"/>.
            /// For other formats use <see cref="CreateFromArray{T}(T[], ImageFormat, int, int, int)"/>.
            /// </para><para>
            /// <see cref="Buffer"/> points to pinned memory of <paramref name="memoryOwner"/>.
            /// </para></remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or memory of <paramref name="memoryOwner"/> is too small for specified image parameters.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Image stride in bytes cannot be automatically calculated from <paramref name="widthPixels"/> for specified <paramref name="format"/>.
            /// </exception>
            /// <seealso cref="ImageFormats.StrideBytes(ImageFormat, int)"/>
            public static new Orbbec CreateFromMemory<T>(System.Buffers.IMemoryOwner<T> memoryOwner, ImageFormat format, int widthPixels, int heightPixels)
                where T : unmanaged
                => CreateFromMemory(memoryOwner, format, widthPixels, heightPixels, format.StrideBytes(widthPixels));


            /// <summary>Creates new image for specified underlying memory owner with specified format and size in pixels.</summary>
            /// <typeparam name="T">Type of elements in underlying memory buffer. Must be value type.</typeparam>
            /// <param name="memoryOwner">Memory owner of underlying buffer. Cannot be <see langword="null"/>. Object will pin and keep reference to this array during all lifetime.</param>
            /// <param name="format">Format of image.</param>
            /// <param name="widthPixels">Width of image in pixels. Must be positive.</param>
            /// <param name="heightPixels">Height of image in pixels. Must be positive.</param>
            /// <param name="strideBytes">Image stride in bytes (the number of bytes per horizontal line of the image). Must be non-negative. Zero value can be used for <see cref="ImageFormat.ColorMjpg"/> and <see cref="ImageFormat.Custom"/>.</param>        /// <returns>Created image. Not <see langword="null"/>.</returns>
            /// <remarks><para>
            /// <see cref="Buffer"/> points to pinned memory of <paramref name="memoryOwner"/>.
            /// </para></remarks>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="widthPixels"/> or <paramref name="heightPixels"/> is equal to or less than zero
            /// or <paramref name="strideBytes"/> is less than zero or <paramref name="strideBytes"/> is too small for specified <paramref name="format"/>
            /// or memory of <paramref name="memoryOwner"/> is too small for specified image parameters.
            /// </exception>
            public static new unsafe Orbbec CreateFromMemory<T>(System.Buffers.IMemoryOwner<T> memoryOwner, ImageFormat format, int widthPixels, int heightPixels, int strideBytes)
                where T : unmanaged
            {
                if (memoryOwner is null)
                    throw new ArgumentNullException(nameof(memoryOwner));
                if (widthPixels <= 0)
                    throw new ArgumentOutOfRangeException(nameof(widthPixels));
                if (heightPixels <= 0)
                    throw new ArgumentOutOfRangeException(nameof(heightPixels));
                if (strideBytes < 0)
                    throw new ArgumentOutOfRangeException(nameof(strideBytes));
                if (format.HasKnownBytesPerPixel() && strideBytes < widthPixels * format.BytesPerPixel())
                    throw new ArgumentOutOfRangeException(nameof(strideBytes));

                var memory = memoryOwner.Memory;
                var sizeBytes = memory.Length * Marshal.SizeOf<T>();
                if (strideBytes > 0 && sizeBytes < format.ImageSizeBytes(strideBytes, heightPixels))
                    throw new ArgumentOutOfRangeException(nameof(memoryOwner) + "." + nameof(memoryOwner.Memory) + nameof(memory.Length));

                var memoryPin = memory.Pin();
                var memoryPtr = new IntPtr(memoryPin.Pointer);

                var res = NativeApi.Orbbec.Instance.ImageCreateFromBuffer(format, widthPixels, heightPixels, strideBytes,
                    memoryPtr, Helpers.Int32ToUIntPtr(sizeBytes),
                    pinnedMemoryReleaseCallback, PinnedMemoryContext.Create(memoryOwner, memoryPin),
                    out var handle);
                if (res != NativeCallResults.Result.Succeeded || handle is null || handle.IsInvalid)
                {
                    memoryPin.Dispose();
                    throw new NotSupportedException("OrbbecSDK-K4A-Wrapper has limited support of this functionality. Please, prefer image creation via constructors.");
                }

                return (Orbbec)Create(handle)!;
            }
#endif
        }
    }
}
