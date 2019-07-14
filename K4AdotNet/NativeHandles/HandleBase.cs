using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // #define K4A_DECLARE_HANDLE(_handle_name_)
    // ...
    /// <summary>Base class for all native handles declared it Sensor SDK.</summary>
    /// <remarks>
    /// Handles represent object instances in Sensor SDK.
    /// Handles are opaque pointers returned by the SDK which represent an object.
    /// Invalid handles are set to 0 (<c>IntPtr.Zero</c>).
    /// </remarks>
    internal abstract class HandleBase : SafeHandle
    {
        /// <summary>Instances always own handles they store.</summary>
        protected HandleBase() : base(invalidHandleValue: IntPtr.Zero, ownsHandle: true)
        { }

        /// <summary>Invalid handle is <c>IntPtr.Zero</c>.</summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        public override string ToString()
            => GetType().Name + "#" + handle.ToString("X");
    }
}
