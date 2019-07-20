using K4AdotNet.BodyTracking;
using System;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    public sealed class BodyFrameReadyEventArgs : EventArgs
    {
        public BodyFrameReadyEventArgs(BodyFrame bodyFrame)
            => BodyFrame = bodyFrame;

        public BodyFrame BodyFrame { get; }
    }
}
