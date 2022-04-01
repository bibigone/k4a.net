using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Unity
{
    //キャプチャークラスを受け取るのみ
    public class CaptureEventArgs : EventArgs
    {
        public CaptureEventArgs(Capture capture)
        {
            Capture = capture;
        }

        public Capture Capture { get; }
    }
}