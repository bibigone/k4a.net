using K4AdotNet.BodyTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class SkeletonRenderer : MonoBehaviour
    {
        private Dictionary<JointType, GameObject> _joints;
        private Tracker _tracker;

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            _joints = typeof(JointType).GetEnumValues().Cast<JointType>()
                .ToDictionary(
                    jt => jt,
                    jt =>
                    {
                        var joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        joint.transform.parent = transform;
                        joint.transform.localScale = 0.1f * Vector3.one;
                        joint.name = jt.ToString();
                        joint.SetActive(false);
                        return joint;
                    });
            SetJointScale(0.05f, JointType.EyeLeft, JointType.EyeRight, JointType.Nose, JointType.EarLeft, JointType.EarRight);
            SetJointColor(Color.cyan, JointType.EyeLeft, JointType.EyeRight);
            SetJointColor(Color.magenta, JointType.Nose);
            SetJointColor(Color.yellow, JointType.EarLeft, JointType.EarRight);
        }

        private void SetJointScale(float scale, params JointType[] jointTypes)
        {
            foreach (var jt in jointTypes)
                _joints[jt].transform.localScale = scale * Vector3.one;
        }

        private void SetJointColor(Color color, params JointType[] jointTypes)
        {
            foreach (var jt in jointTypes)
                _joints[jt].GetComponent<Renderer>().material.color = color;
        }

        private void Start()
        {
            StartCoroutine(DelayedInitialize());
        }

        private IEnumerator DelayedInitialize()
        {
            var attempt = 1;
            do
            {
                yield return new WaitForSeconds(2);

                try
                {
                    IsInitialized = Sdk.TryInitializeBodyTrackingRuntime(out var message);
                    if (!IsInitialized)
                    {
                        Debug.Log($"Cannot initialize body tracking: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Exception on {nameof(Sdk.TryInitializeBodyTrackingRuntime)}\r\n{ex}");
                }
            } while (!IsInitialized && ++attempt <= 3);

            if (IsInitialized)
            {
                var captureManager = FindObjectOfType<CaptureManager>();
                if (captureManager?.IsInitialized == true)
                {
                    var calibration = captureManager.Calibration;
                    _tracker = new Tracker(ref calibration);

                    captureManager.CaptureReady += DeviceManager_CaptureReady;
                }
                else
                {
                    IsInitialized = false;
                }
            }
            else
            {
                FindObjectOfType<ErrorMessage>().Show("Cannot initialize Azure Kinect Body Tracking runtime");
            }
        }

        private void OnDestroy()
        {
            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager != null) captureManager.CaptureReady -= DeviceManager_CaptureReady;
            _tracker?.Dispose();
        }

        private void DeviceManager_CaptureReady(object sender, CaptureEventArgs e)
        {
            if (IsInitialized)
            {
                _tracker.TryEnqueueCapture(e.Capture);
            }
        }

        private void Update()
        {
            if (IsInitialized)
            {
                if (_tracker.TryPopResult(out var bodyFrame))
                {
                    using (bodyFrame)
                    {
                        if (bodyFrame.BodyCount > 0)
                        {
                            bodyFrame.GetBodySkeleton(0, out var skeleton);
                            RenderSkeleton(skeleton);
                        }
                        else
                        {
                            HideSkeleton();
                        }
                    }
                }
            }
        }

        private void RenderSkeleton(Skeleton skeleton)
        {
            foreach (var item in _joints)
            {
                var j = skeleton[item.Key];
                item.Value.transform.localPosition = 0.001f * new Vector3(j.PositionMm.X, -j.PositionMm.Y, j.PositionMm.Z);
                item.Value.SetActive(true);
            }
        }

        private void HideSkeleton()
        {
            foreach (var joint in _joints.Values)
                joint.SetActive(false);
        }
    }
}