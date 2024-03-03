using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    /// <summary>
    /// Implementation of <see cref="ICustomMemoryAllocator"/> interface
    /// via <see cref="Marshal.AllocHGlobal(int)"/> and <see cref="Marshal.FreeHGlobal(IntPtr)"/> methods.
    /// </summary>
    public sealed class HGlobalMemoryAllocator : ICustomMemoryAllocator
    {
        /// <summary>Singleton.</summary>
        public static readonly ICustomMemoryAllocator Instance = new HGlobalMemoryAllocator();

        /// <summary>For internal usage.</summary>
        internal static readonly Sensor.NativeApi.MemoryDestroyCallback MemoryDestroyCallback = new(ReleaseUnmanagedBuffer);

        private HGlobalMemoryAllocator()
        { }

        /// <summary>Allocates a buffer of size at least <paramref name="size"/> bytes.</summary>
        /// <param name="size">Minimum size in bytes needed for the buffer. Cannot be negative.</param>
        /// <param name="context">Not used. Always <see cref="IntPtr.Zero"/>.</param>
        /// <returns>A pointer to the newly allocated memory. This memory must be released using the <see cref="Free(IntPtr, IntPtr)"/> method.</returns>
        public IntPtr Allocate(int size, out IntPtr context)
        {
            context = IntPtr.Zero;
            return Marshal.AllocHGlobal(size);
        }

        /// <summary>Frees memory previously allocated by <see cref="Allocate(int, out IntPtr)"/> method.</summary>
        /// <param name="buffer">The handle returned by the original matching call to <see cref="Allocate(int, out IntPtr)"/> method.</param>
        /// <param name="context">Not used.</param>
        public void Free(IntPtr buffer, IntPtr context)
            => ReleaseUnmanagedBuffer(buffer, context);

        private static void ReleaseUnmanagedBuffer(IntPtr buffer, IntPtr context)
            => Marshal.FreeHGlobal(buffer);
    }
}
