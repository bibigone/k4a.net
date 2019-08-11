using K4AdotNet.Sensor;
using System;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class CaptureManager : MonoBehaviour
    {
        private Device _device;

        public bool IsInitialized => _device != null;
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
                    CameraFps = FrameRate.Fifteen,
                };
                _device.GetCalibration(Configuration.DepthMode, Configuration.ColorResolution, out var calibration);
                Calibration = calibration;
            }
            else
            {
                Debug.Log("Cannot open device");
            }
        }

        private void Start()
        {
            if (IsInitialized)
            {
                _device.StartCameras(Configuration);
            }
            else
            {
                FindObjectOfType<ErrorMessage>().Show("Azure Kinect is not connected");
            }
        }

        private void Update()
        {
            if (IsInitialized)
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
        }
    }
}