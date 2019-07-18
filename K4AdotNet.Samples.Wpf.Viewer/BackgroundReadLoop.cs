using K4AdotNet.Record;
using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.Threading;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal abstract class BackgroundReadLoop : IDisposable
    {
        public static BackgroundReadLoop CreateForPlayback(string filePath, bool disableColor, bool disableDepth, bool playAsFastAsPossible)
            => new PlaybackReadLoop(filePath, disableColor, disableDepth, playAsFastAsPossible);

        protected BackgroundReadLoop()
        { }

        public abstract ColorResolution ColorResolution { get; }

        public abstract DepthMode DepthMode { get; }

        public abstract void Dispose();

        public abstract void GetCalibration(out Calibration calibration);

        protected abstract void BackgroundLoop();

        public void Run()
            => new Thread(BackgroundLoop) { IsBackground = true }.Start();

        public event EventHandler<CaptureReadyEventArgs> CaptureReady;

        public event EventHandler<FailedEventArgs> Failed;

        #region Playback

        private sealed class PlaybackReadLoop : BackgroundReadLoop
        {
            private readonly bool playAsFastAsPossible;
            private readonly Playback playback;
            private readonly int frameRateHz;

            public PlaybackReadLoop(string filePath, bool disableColor, bool disableDepth, bool playAsFastAsPossible)
            {
                this.playAsFastAsPossible = playAsFastAsPossible;

                playback = new Playback(filePath);
                playback.GetRecordConfiguration(out var config);
                frameRateHz = config.CameraFps.ToNumberHz();
                ColorResolution = disableColor ? ColorResolution.Off : config.ColorResolution;
                DepthMode = disableDepth
                    ? (config.DepthMode == DepthMode.PassiveIR || config.DepthMode == DepthMode.WideViewUnbinned)
                        ? DepthMode.PassiveIR : DepthMode.Off
                    : config.DepthMode;

                if (ColorResolution != ColorResolution.Off)
                    playback.SetColorConversion(ImageFormat.ColorBgra32);   // NB! This results in very-very expensive operation during data extraction!..
            }

            public override void Dispose()
                => playback.Dispose();

            public override ColorResolution ColorResolution { get; }

            public override DepthMode DepthMode { get; }

            public override void GetCalibration(out Calibration calibration)
                => playback.GetCalibration(out calibration);

            public override string ToString()
                => playback.FilePath;

            protected override void BackgroundLoop()
            {
                try
                {
                    var forward = true;
                    var sw = Stopwatch.StartNew();
                    var frameCounter = 0L;

                    while (!playback.IsDisposed)
                    {
                        while (!playAsFastAsPossible && !playback.IsDisposed)
                        {
                            var expectedTime = Microseconds64.FromSeconds((double)frameCounter / frameRateHz);
                            var elapsedTime = (Microseconds64)sw.Elapsed;
                            if (elapsedTime >= expectedTime)
                                break;
                            var sleepInterval = new Microseconds64((expectedTime.ValueUsec - elapsedTime.ValueUsec) / 2);
                            Thread.Sleep(sleepInterval);
                        }

                        var res = forward
                            ? playback.TryGetNextCapture(out var capture)
                            : playback.TryGetPreviousCapture(out capture);
                        if (!res)
                        {
                            forward = !forward;
                            continue;
                        }

                        frameCounter++;
                        using (capture)
                        {
                            CaptureReady?.Invoke(this, new CaptureReadyEventArgs(capture));
                        }
                    }
                }
                catch (ObjectDisposedException)
                { }
                catch (Exception exc)
                {
                    Failed?.Invoke(this, new FailedEventArgs(exc));
                }
            }
        }

        #endregion
    }
}
