using System;

namespace K4AdotNet.Samples.Wpf
{
    public sealed class FailedEventArgs : EventArgs
    {
        public FailedEventArgs(Exception exception)
            => Exception = exception;

        public Exception Exception { get; }
    }
}
