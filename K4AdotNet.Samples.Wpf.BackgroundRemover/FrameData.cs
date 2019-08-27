using K4AdotNet.Sensor;
using System;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    /// <summary>
    /// Frame data for processing.
    /// </summary>
    /// <remarks>
    /// Contains synchronized color and depth frames.
    /// </remarks>
    internal class FrameData : IDisposable
    {
        public FrameData(Image colorFrame, Image depthFrame)
        {
            ColorFrame = colorFrame ?? throw new ArgumentNullException(nameof(colorFrame));
            DepthFrame = depthFrame ?? throw new ArgumentNullException(nameof(depthFrame));
        }

        public void Dispose()
        {
            ColorFrame.Dispose();
            DepthFrame.Dispose();
        }

        public Image ColorFrame { get; }
        public Image DepthFrame { get; }
    }
}
