using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf
{
    public sealed class CaptureReadyEventArgs : EventArgs
    {
        public CaptureReadyEventArgs(Capture capture)
            => Capture = capture;

        public Capture Capture { get; }
    }
}
