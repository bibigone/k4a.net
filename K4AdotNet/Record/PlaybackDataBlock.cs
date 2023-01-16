using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Record
{
    /// <summary>Data block read from custom track in recording.</summary>
    /// <seealso cref="PlaybackTrack"/>
    public sealed class PlaybackDataBlock : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.PlaybackDataBlockHandle> handle;     // this class is an wrapper around this handle

        private PlaybackDataBlock(NativeHandles.PlaybackDataBlockHandle handle)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object? sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to release data block and free all unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the data block has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Gets the device timestamp of a data block in microseconds.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        public Microseconds64 DeviceTimestamp
            => NativeApi.PlaybackDataBlockGetDeviceTimestamp(handle.ValueNotDisposed);

        /// <summary>Gets the buffer size of a data block in bytes.</summary>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        public int SizeBytes
            => Helpers.UIntPtrToInt32(NativeApi.PlaybackDataBlockGetBufferSize(handle.ValueNotDisposed));

        /// <summary>Gets the buffer of a data block.</summary>
        /// <remarks>Use this buffer to access the data written to a custom recording track.</remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        public IntPtr Buffer
            => NativeApi.PlaybackDataBlockGetBuffer(handle.ValueNotDisposed);

        /// <summary>Copies block data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for block data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(byte[] dst)
        {
            if (dst is null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            if (dst.Length < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            Marshal.Copy(Buffer, dst, 0, size);
            return size;
        }

        /// <summary>Copies block data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for block data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(short[] dst)
        {
            if (dst is null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(short);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of block data {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        /// <summary>Copies block data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for block data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(float[] dst)
        {
            if (dst is null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(float);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of block data {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        /// <summary>Copies block data from <see cref="Buffer"/> to <paramref name="dst"/> array.</summary>
        /// <param name="dst">Destination array for block data. Cannot be <see langword="null"/>. Must be long enough (see <see cref="SizeBytes"/>).</param>
        /// <returns>Number of copied array elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dst"/> cannot be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length of <paramref name="dst"/> array is too small.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        public int CopyTo(int[] dst)
        {
            if (dst is null)
                throw new ArgumentNullException(nameof(dst));
            var size = SizeBytes;
            var elementSize = sizeof(int);
            if (dst.Length * elementSize < size)
                throw new ArgumentOutOfRangeException(nameof(dst) + "." + nameof(dst.Length));
            if (size % elementSize != 0)
                throw new InvalidOperationException($"Size of block data {size} is not divisible by element size {elementSize}.");
            var dstCount = SizeBytes / elementSize;
            Marshal.Copy(Buffer, dst, 0, dstCount);
            return dstCount;
        }

        internal static PlaybackDataBlock? Create(NativeHandles.PlaybackDataBlockHandle handle)
            => handle.IsValid ? new(handle) : null;
    }
}
