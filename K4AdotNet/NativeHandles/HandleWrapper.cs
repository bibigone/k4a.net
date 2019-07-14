using System;
using System.Threading;

namespace K4AdotNet.NativeHandles
{
    internal sealed class HandleWrapper<T> : IDisposablePlus
        where T : HandleBase
    {
        private volatile int disposeCounter;

        public HandleWrapper(T handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (handle.IsInvalid || handle.IsClosed)
                throw new ArgumentException("Handle must be valid", nameof(handle));

            Value = handle;
        }

        public T Value { get; }

        public T ValueNotDisposed
        {
            get
            {
                CheckNotDisposed();
                return Value;
            }
        }

        public void Dispose()
        {
            var incrementedValue = Interlocked.Increment(ref disposeCounter);
            if (incrementedValue == 1)
            {
                Value.Dispose();
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsDisposed => disposeCounter > 0;

        public event EventHandler Disposed;

        public void CheckNotDisposed()
        {
            if (disposeCounter > 0)
                throw new ObjectDisposedException(Value.ToString());
        }

        public static implicit operator HandleWrapper<T> (T handle)
            => new HandleWrapper<T>(handle);
    }
}
