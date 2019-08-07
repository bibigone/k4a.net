using System;
using System.Threading;

namespace K4AdotNet.NativeHandles
{
    /// <summary>
    /// Helper wrapper around <see cref="HandleBase"/> objects.
    /// Implements <see cref="IDisposablePlus"/> interface, which is really helpful for public classes.
    /// </summary>
    /// <typeparam name="T">Type of handle.</typeparam>
    internal sealed class HandleWrapper<T> : IDisposablePlus, IEquatable<HandleWrapper<T>>
        where T : HandleBase
    {
        private volatile int disposeCounter;        // to raise Disposed event only on the first call of Dispose()

        /// <summary>Creates <see cref="IDisposablePlus"/>-wrapper around specified handle.</summary>
        /// <param name="handle">Handle to be wrapped. Not <see langword="null"/>. And must be valid and not closed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="handle"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="handle"/> is invalid or closed.</exception>
        public HandleWrapper(T handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (handle.IsInvalid || handle.IsClosed)
                throw new ArgumentException("Handle must be valid", nameof(handle));

            Value = handle;
        }

        /// <summary>Direct access to the underlying handle object.</summary>
        public T Value { get; }

        /// <summary>Like <see cref="Value"/> but checks that object is not disposed in addition.</summary>
        /// <exception cref="ObjectDisposedException">If object is disposed.</exception>
        /// <seealso cref="IsDisposed"/>
        public T ValueNotDisposed
        {
            get
            {
                CheckNotDisposed();
                return Value;
            }
        }

        /// <summary>
        /// Disposes underlying handle
        /// plus raises <see cref="Disposed"/> event if it is first call of this method for object.
        /// </summary>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
        {
            var incrementedValue = Interlocked.Increment(ref disposeCounter);
            if (incrementedValue == 1)
            {
                Value.Dispose();
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => disposeCounter > 0;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler Disposed;

        /// <summary>Checks that object is not disposed.</summary>
        /// <exception cref="ObjectDisposedException">If object is disposed.</exception>
        public void CheckNotDisposed()
        {
            if (disposeCounter > 0)
                throw new ObjectDisposedException(Value.ToString());
        }

        /// <summary>Implicit conversion from handle to wrapper for usability.</summary>
        /// <param name="handle">Handle to be wrapped.</param>
        public static implicit operator HandleWrapper<T> (T handle)
            => new HandleWrapper<T>(handle);

        /// <summary>String representation exactly as handle object has.</summary>
        /// <returns><see cref="HandleBase.ToString"/></returns>
        public override string ToString()
            => Value.ToString();

        #region Equatable

        /// <summary>Delegates hash code calculation to <see cref="HandleBase.GetHashCode"/> implementation.</summary>
        /// <returns>Hash code consistent with <see cref="Equals(HandleWrapper{T})"/>.</returns>
        public override int GetHashCode()
            => Value.GetHashCode();

        /// <summary>Delegates comparison to <see cref="HandleBase.Equals(HandleBase)"/>.</summary>
        /// <param name="other">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both handles reference to one and the same object.</returns>
        public bool Equals(HandleWrapper<T> other)
            => !(other is null) && other.Value.Equals(Value);

        /// <summary>Two objects are equal when they reference to one and the same unmanaged object.</summary>
        /// <param name="obj">Another handle to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is <see cref="HandleWrapper{T}"/> and they both reference to one and the same object.</returns>
        public override bool Equals(object obj)
            => obj is HandleWrapper<T> && Equals((HandleWrapper<T>)obj);

        /// <summary>To be consistent with <see cref="Equals(HandleWrapper{T})"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleWrapper{T})"/>
        public static bool operator ==(HandleWrapper<T> left, HandleWrapper<T> right)
            => (left is null && right is null) || (!(left is null) && left.Equals(right));

        /// <summary>To be consistent with <see cref="Equals(HandleWrapper{T})"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(HandleWrapper{T})"/>
        public static bool operator !=(HandleWrapper<T> left, HandleWrapper<T> right)
            => !(left == right);

        #endregion
    }
}
