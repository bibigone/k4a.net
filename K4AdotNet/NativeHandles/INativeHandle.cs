using System;

namespace K4AdotNet.NativeHandles
{
    /// <summary>
    /// Base interface for all structures that wrap native handle value.
    /// Such structures are used instead of <see cref="IntPtr"/> for better type safety in native API.
    /// </summary>
    internal interface INativeHandle
    {
        /// <summary>Raw value of native handle. Use accurately.</summary>
        IntPtr UnsafeValue { get; }

        /// <summary>Is handle valid.</summary>
        /// <remarks>Zero handle (that is <see cref="IntPtr.Zero"/>) is treated as invalid one.</remarks>
        bool IsValid { get; }

        /// <summary>Release of handle.</summary>
        /// <remarks>Can be called for invalid handle. In this case this method does nothing.</remarks>
        void Release();
    }
}
