using System;

namespace K4AdotNet
{
    /// <summary>
    /// <see cref="IDisposable"/> interface with additions: property <see cref="IsDisposed"/> and event <see cref="Disposed"/>.
    /// </summary>
    internal interface IDisposablePlus : IDisposable
    {
        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        bool IsDisposed { get; }

        /// <summary>Raised on object disposing (only once).</summary>
        event EventHandler Disposed;
    }
}
