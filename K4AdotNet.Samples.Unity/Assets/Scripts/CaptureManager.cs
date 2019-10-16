using K4AdotNet.Sensor;
using System;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class CaptureManager : MonoBehaviour, IInitializable
    {
        private Device _device;

        public bool IsInitializationComplete { get; private set; }
        public bool IsAvailable => _device != null;
        public DeviceConfiguration Configuration { get; private set; }
        public Calibration Calibration { get; private set; }

        public event EventHandler<CaptureEventArgs> CaptureReady;

        private void Awake()
        {
            if (Device.TryOpen(out _device))
            {
                Configuration = new DeviceConfiguration
                {
                    ColorResolution = ColorResolution.R720p,
                    ColorFormat = ImageFormat.ColorBgra32,
                    DepthMode = DepthMode.NarrowViewUnbinned,
                    CameraFps = FrameRate.Thirty,
                };
                _device.GetCalibration(Configuration.DepthMode, Configuration.ColorResolution, out var calibration);
                Calibration = calibration;
            }
            else
            {
                Debug.Log("Cannot open device");
            }

            IsInitializationComplete = true;
        }

        private void Start()
        {
            if (IsAvailable)
            {
                _device.StartCameras(Configuration);
            }
        }

        private void Update()
        {
            if (IsAvailable)
            {
                if (_device.TryGetCapture(out var capture))
                {
                    using (capture)
                    {
                        CaptureReady?.Invoke(this, new CaptureEventArgs(capture));
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _device?.StopCameras();
            _device?.Dispose();
            _device = null;
        }
    }
}