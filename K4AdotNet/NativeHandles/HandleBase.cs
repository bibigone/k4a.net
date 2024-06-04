using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.NativeHandles
{
    // Defined in k4atypes.h:
    // #define K4A_DECLARE_HANDLE(_handle_name_)
    //
    /// <summary>Base class for all native handles declared it Sensor SDK.</summary>
    /// <remarks>
    /// Handles represent object instances in Sensor SDK.
    /// Handles are opaque pointers returned by the SDK which represent an object.
    /// Invalid handles are set to 0 (<c>IntPtr.Zero</c>).
    /// </remarks>
    internal abstract class HandleBase : SafeHandle, IEquatable<HandleBase>
    {
        /// <summary>Instances always own handles they store.</summary>
        protected HandleBase() : base(invalidHandleValue: IntPtr.Zero, ownsHandle: true)
        { }

        /// <summary>Invalid handle is <see cref="IntPtr.Zero"/>.</summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>Convenient (for debugging needs, first of all) string representation of object.</summary>
        /// <returns><c>{TypeName}#{Address}</c></returns>
        public override string ToString()
            => GetType().Name + "#" + handle.ToString("X");

        #region Equatable

        /// <summary>Uses underlying value of handle as hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        /// <seealso cref="Equals(HandleBase)"/>
        public override int GetHashCode()
            => handle.ToInt32();

        /// <summary>Two objects are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="other">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both handles reference to one and the same object.</returns>
        public bool Equals(HandleBase? other)
            => other is not null && other.handle == handle;

        /// <summary>Two objects are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is <see cref="HandleBase"/> and they both reference to one and the same object.</returns>
        public override bool Equals(object? obj)
            => obj is HandleBase handle && Equals(handle);

        /// <summary>To be consistent with <see cref="Equals(HandleBase)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleBase)"/>
        public static bool operator ==(HandleBase? left, HandleBase? right)
            => (left is null && right is null) || (left is not null && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(HandleBase)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleBase)"/>
        public static bool operator !=(HandleBase? left, HandleBase? right)
            => !(left == right);

        #endregion

        /// <summary>
        /// For some unknown reason, disposing of Orbbec handles from the main UI thread results in magic troubles with working of some COM objects...
        /// As a dirty fix, we're calling handle releasing from random background thread from the thread pool.
        /// </summary>
        /// <param name="releaser">Native method that releases native handle.</param>
        protected void ReleaseOrbbecHandle(Action<IntPtr> releaser)
        {
            var thread = System.Threading.Thread.CurrentThread;
            if (thread.IsBackground || thread.IsThreadPoolThread)
            {
                releaser(handle);
                return;
            }

            var t = System.Threading.Tasks.Task.Run(() => releaser(handle));
            t.Wait();
            t.Dispose();
        }
    }
}
