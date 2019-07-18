using System;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class FailedEventArgs : EventArgs
    {
        public FailedEventArgs(Exception exception)
            => Exception = exception;

        public Exception Exception { get; }
    }
}
