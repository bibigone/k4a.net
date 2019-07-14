using System;

namespace K4AdotNet
{
    internal interface IDisposablePlus : IDisposable
    {
        bool IsDisposed { get; }
        event EventHandler Disposed;
    }
}
