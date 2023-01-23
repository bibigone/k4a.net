using System;
using System.Threading;

namespace K4AdotNet.NativeHandles
{
    /// <summary>
    /// Helper wrapper around native handle structures that implement <see cref="INativeHandle"/> interface.
    /// Implements <see cref="IDisposablePlus"/> interface, which is really helpful in implementation of public classes.
    /// Plus this class has a finalyzer that calls <see cref="INativeHandle.Release"/> for objects that were not disposed in an explicit manner.
    /// </summary>
    /// <typeparam name="T">Type of native handle.</typeparam>
    internal sealed class HandleWrapper<T> : IDisposablePlus, IEquatable<HandleWrapper<T>>
        where T : struct, INativeHandle
    {
        private readonly T handle;                  // underlying native handle
        private volatile int releaseCounter;        // to release handle only once

        /// <summary>Creates <see cref="IDisposablePlus"/>-wrapper around specified handle.</summary>
        /// <param name="handle">Handle to be wrapped. Must be valid.</param>
        /// <exception cref="ArgumentException">If <paramref name="handle"/> is invalid.</exception>
        public HandleWrapper(T handle)
        {
            if (!handle.IsValid)
                throw new ArgumentException("Handle must be valid", nameof(handle));
            this.handle = handle;
        }

        /// <summary>Direct access to the underlying handle object.</summary>
        public T Value => handle;

        /// <summary>Like <see cref="Value"/> but checks that object is not disposed in addition.</summary>
        /// <exception cref="ObjectDisposedException">If object is disposed.</exception>
        /// <seealso cref="IsDisposed"/>
        public T ValueNotDisposed
        {
            get
            {
                CheckNotDisposed();
                return handle;
            }
        }

        /// <summary>Calls <see cref="INativeHandle.Release"/> for objects that were not disposed in an explicit manner.</summary>
        ~HandleWrapper()
            => ReleaseHandle();

        /// <summary>
        /// Disposes underlying handle
        /// plus raises <see cref="Disposed"/> event if it is the first call of this method for the object.
        /// </summary>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
        {
            if (ReleaseHandle())
            {
                GC.SuppressFinalize(this);
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Releases handle only once.</summary>
        /// <returns><see langword="true"/> - handle was release.</returns>
        private bool ReleaseHandle()
        {
            // Release only once
            var incrementedValue = Interlocked.Increment(ref releaseCounter);
            if (incrementedValue == 1)
            {
                handle.Release();
                return true;
            }
            return false;
        }

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => releaseCounter > 0;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>Checks that object is not disposed.</summary>
        /// <exception cref="ObjectDisposedException">If object is disposed.</exception>
        public void CheckNotDisposed()
        {
            if (releaseCounter > 0)
                throw new ObjectDisposedException(ToString());
        }

        /// <summary>Implicit conversion from handle to wrapper for usability.</summary>
        /// <param name="handle">Handle to be wrapped.</param>
        public static implicit operator HandleWrapper<T> (T handle)
            => new(handle);

        /// <summary>String representation of underlying native handle.</summary>
        /// <returns><c>{HandleTypeName}#{Address}</c></returns>
        public override string ToString()
            => handle.GetType().Name + "#" + handle.UnsafeValue.ToString("X");

        #region Equatable

        /// <summary>Delegates hash code calculation to handle implementation.</summary>
        /// <returns>Hash code consistent with <see cref="Equals(HandleWrapper{T})"/>.</returns>
        public override int GetHashCode()
            => unchecked((int)handle.UnsafeValue.ToInt64());

        /// <summary>Delegates comparison to handle.</summary>
        /// <param name="other">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both handles reference to one and the same object.</returns>
        public bool Equals(HandleWrapper<T>? other)
            => other is not null && other.handle.UnsafeValue == handle.UnsafeValue;

        /// <summary>Two objects are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is <see cref="HandleWrapper{T}"/> and they both reference to one and the same object.</returns>
        public override bool Equals(object? obj)
            => obj is HandleWrapper<T> wrapper && Equals(wrapper);

        /// <summary>To be consistent with <see cref="Equals(HandleWrapper{T})"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleWrapper{T})"/>
        public static bool operator ==(HandleWrapper<T>? left, HandleWrapper<T>? right)
            => (left is null && right is null) || (left is not null && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(HandleWrapper{T})"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleWrapper{T})"/>
        public static bool operator !=(HandleWrapper<T>? left, HandleWrapper<T>? right)
            => !(left == right);

        #endregion
    }
}
