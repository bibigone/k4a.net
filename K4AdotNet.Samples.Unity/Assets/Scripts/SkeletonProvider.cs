using K4AdotNet.BodyTracking;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class SkeletonProvider : MonoBehaviour, IInitializable
    {
        private Tracker _tracker;

        public bool IsInitializationComplete { get; private set; }
        public bool IsAvailable { get; private set; }

        public event EventHandler<SkeletonEventArgs> SkeletonUpdated;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);

            var task = Task.Run(() =>
            {
                var initialized = Sdk.TryInitializeBodyTrackingRuntime(TrackerProcessingMode.GpuCuda, out var message);
                return Tuple.Create(initialized, message);
            });
            yield return new WaitUntil(() => task.IsCompleted);

            var isAvailable = false;
            try
            {
                var result = task.Result;
                isAvailable = result.Item1;
                if (!isAvailable)
                {
                    Debug.Log($"Cannot initialize body tracking: {result.Item2}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Exception on {nameof(Sdk.TryInitializeBodyTrackingRuntime)}\r\n{ex}");
            }

            if (isAvailable)
            {
                var captureManager = FindObjectOfType<CaptureManager>();
                yield return new WaitUntil(() => captureManager?.IsInitializationComplete != false);
                if (captureManager?.IsAvailable == true)
                {
                    var calibration = captureManager.Calibration;

                    var config = TrackerConfiguration.Default;
                    config.ProcessingMode = TrackerProcessingMode.GpuCuda;
                    // Use lite version of DNN model for speed (comment next line to use default DNN model)
                    config.ModelPath = Sdk.BODY_TRACKING_DNN_MODEL_LITE_FILE_NAME;

                    _tracker = new Tracker(in calibration, config);

                    captureManager.CaptureReady += CaptureManager_CaptureReady;
                }
                else
                {
                    isAvailable = false;
                }
            }

            IsAvailable = isAvailable;
            IsInitializationComplete = true;
        }

        private void OnDestroy()
        {
            IsAvailable = false;

            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager != null) captureManager.CaptureReady -= CaptureManager_CaptureReady;
            _tracker?.Dispose();
        }

        private void CaptureManager_CaptureReady(object sender, CaptureEventArgs e)
        {
            if (IsAvailable)
            {
                using var capture = e.Capture;
                using var depthImage = capture.DepthImage;
                using var irImage = capture.IRImage;
                if (!(depthImage is null) && !(irImage is null))
                    _tracker.TryEnqueueCapture(capture);
            }
        }

        private void Update()
        {
            if (IsAvailable)
            {
                if (_tracker.TryPopResult(out var bodyFrame))
                {
                    using (bodyFrame)
                    {
                        if (bodyFrame.BodyCount > 0)
                        {
                            bodyFrame.GetBodySkeleton(0, out var skeleton);
                            SkeletonUpdated?.Invoke(this, new SkeletonEventArgs(skeleton));
                        }
                        else
                        {
                            SkeletonUpdated?.Invoke(this, SkeletonEventArgs.Empty);
                        }
                    }
                }
            }
        }
    }
}
