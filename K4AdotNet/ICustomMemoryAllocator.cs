using System;

namespace K4AdotNet
{
    /// <summary>
    /// Base interface for custom memory manager that can be used to allocate and free memory used by images.
    /// </summary>
    /// <seealso cref="Sdk.CustomMemoryAllocator"/>
    public interface ICustomMemoryAllocator
    {
        /// <summary>Function for a memory allocation. Will be called by internals of Azure Kinect SDK.</summary>
        /// <param name="size">Minimum size in bytes needed for the buffer.</param>
        /// <param name="context">Output parameter for a context that will be provided in the subsequent call to the <see cref="Free(IntPtr, IntPtr)"/> callback.</param>
        /// <returns>A pointer to the newly allocated memory.</returns>
        IntPtr Allocate(int size, out IntPtr context);

        /// <summary>Function for a memory object being destroyed. Will be called by internals of Azure Kinect SDK.</summary>
        /// <param name="buffer">The buffer pointer that was supplied by the <see cref="Allocate(int, out IntPtr)"/> method and that should be free.</param>
        /// <param name="context">The context for    the memory object that needs to be destroyed that was supplied by the <see cref="Allocate(int, out IntPtr)"/> method.</param>
        void Free(IntPtr buffer, IntPtr context);
    }
}
