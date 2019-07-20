using K4AdotNet.Record;
using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.Threading;

namespace K4AdotNet.Samples.Wpf
{
    public abstract class BackgroundReadingLoop : IDisposable
    {
        public static BackgroundReadingLoop CreateForPlayback(string filePath, bool disableColor, bool disableDepth, bool doNotPlayFasterThanOriginalFps)
            => new PlaybackReadingLoop(filePath, disableColor, disableDepth, doNotPlayFasterThanOriginalFps);

        public static BackgroundReadingLoop CreateForDevice(Device device, DepthMode depthMode, ColorResolution colorResolution, FrameRate frameRate)
            => new DeviceReadingLoop(device, depthMode, colorResolution, frameRate);

        protected BackgroundReadingLoop()
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

        private sealed class PlaybackReadingLoop : BackgroundReadingLoop
        {
            private readonly bool doNotPlayFasterThanOriginalFps;
            private readonly Playback playback;
            private readonly int frameRateHz;

            public PlaybackReadingLoop(string filePath, bool disableColor, bool disableDepth, bool doNotPlayFasterThanOriginalFps)
            {
                this.doNotPlayFasterThanOriginalFps = doNotPlayFasterThanOriginalFps;

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
                        while (doNotPlayFasterThanOriginalFps && !playback.IsDisposed)
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

        #region Device

        private sealed class DeviceReadingLoop : BackgroundReadingLoop
        {
            private readonly Device device;

            public DeviceReadingLoop(Device device, DepthMode depthMode, ColorResolution colorResolution, FrameRate frameRate)
            {
                this.device = device;
                DepthMode = depthMode;
                ColorResolution = colorResolution;
                FrameRate = frameRate;
            }

            public override void Dispose()
                => device.Dispose();

            public override ColorResolution ColorResolution { get; }

            public override DepthMode DepthMode { get; }

            public FrameRate FrameRate { get; }

            public override void GetCalibration(out Calibration calibration)
                => device.GetCalibration(DepthMode, ColorResolution, out calibration);

            public override string ToString()
                => device.ToString();

            protected override void BackgroundLoop()
            {
                try
                {
                    device.StartCameras(new DeviceConfiguration
                    {
                        CameraFps = FrameRate,
                        ColorFormat = ImageFormat.ColorBgra32,
                        ColorResolution = ColorResolution,
                        DepthMode = DepthMode,
                        WiredSyncMode = WiredSyncMode.Standalone,
                    });

                    while (!device.IsDisposed)
                    {
                        var res = device.TryGetCapture(out var capture);
                        if (res)
                        {
                            using (capture)
                            {
                                CaptureReady?.Invoke(this, new CaptureReadyEventArgs(capture));
                            }
                        }
                        else
                        {
                            Thread.Sleep(1);
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
