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
        private Tracker _tracker;

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            _root = new GameObject();
            _root.name = "skeleton:root";
            _root.transform.parent = transform;
            _root.transform.localScale = Vector3.one;
            _root.transform.localPosition = Vector3.zero;
            _root.SetActive(false);

            CreateJoints();
            CreateBones();
            CreateHead();
        }

        #region Render objects

        private GameObject _root;
        private IReadOnlyDictionary<JointType, GameObject> _joints;
        private IReadOnlyCollection<Bone> _bones;
        private GameObject _head;

        private class Bone
        {
            public Bone(JointType parentJoint, JointType childJoint)
            {
                ParentJoint = parentJoint;
                ChildJoint = childJoint;

                Object = new GameObject();
                Object.name = $"{parentJoint}->{childJoint}:pos";

                var bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bone.name = $"{parentJoint}->{childJoint}:bone";
                bone.transform.parent = Object.transform;
                bone.transform.localScale = new Vector3(0.033f, 0.5f, 0.033f);
                bone.transform.localPosition = 0.5f * Vector3.up;
            }

            public JointType ParentJoint { get; }
            public JointType ChildJoint { get; }
            public GameObject Object { get; }
        }

        private void CreateJoints()
        {
            // Joints are rendered as spheres
            _joints = typeof(JointType).GetEnumValues().Cast<JointType>()
                .ToDictionary(
                    jt => jt,
                    jt =>
                    {
                        var joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        joint.transform.parent = _root.transform;
                        joint.transform.localScale = 0.075f * Vector3.one;
                        joint.name = jt.ToString();
                        return joint;
                    });

            // Set green as default color
            SetJointColor(Color.green, typeof(JointType).GetEnumValues().Cast<JointType>().ToArray());

            // Set slightly decreased size for some joints
            SetJointScale(0.05f, JointType.Neck, JointType.Head, JointType.ClavicleLeft, JointType.ClavicleRight, JointType.EarLeft, JointType.EarRight);

            // Set greatly decreased size and specific colors for face joints
            SetJointScale(0.033f, JointType.EyeLeft, JointType.EyeRight, JointType.Nose);
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

        private void CreateBones()
        {
            var bones = new List<Bone>();
            
            // Spine
            bones.Add(new Bone(JointType.Pelvis, JointType.SpineNaval));
            bones.Add(new Bone(JointType.SpineNaval, JointType.SpineChest));
            bones.Add(new Bone(JointType.SpineChest, JointType.Neck));
            bones.Add(new Bone(JointType.Neck, JointType.Head));

            // Right arm
            bones.Add(new Bone(JointType.SpineChest, JointType.ClavicleRight));
            bones.Add(new Bone(JointType.ClavicleRight, JointType.ShoulderRight));
            bones.Add(new Bone(JointType.ShoulderRight, JointType.ElbowRight));
            bones.Add(new Bone(JointType.ElbowRight, JointType.WristRight));

            // Left arm
            bones.Add(new Bone(JointType.SpineChest, JointType.ClavicleLeft));
            bones.Add(new Bone(JointType.ClavicleLeft, JointType.ShoulderLeft));
            bones.Add(new Bone(JointType.ShoulderLeft, JointType.ElbowLeft));
            bones.Add(new Bone(JointType.ElbowLeft, JointType.WristLeft));

            // Right leg
            bones.Add(new Bone(JointType.Pelvis, JointType.HipRight));
            bones.Add(new Bone(JointType.HipRight, JointType.KneeRight));
            bones.Add(new Bone(JointType.KneeRight, JointType.AnkleRight));
            bones.Add(new Bone(JointType.AnkleRight, JointType.FootRight));

            // Left leg
            bones.Add(new Bone(JointType.Pelvis, JointType.HipLeft));
            bones.Add(new Bone(JointType.HipLeft, JointType.KneeLeft));
            bones.Add(new Bone(JointType.KneeLeft, JointType.AnkleLeft));
            bones.Add(new Bone(JointType.AnkleLeft, JointType.FootLeft));

            _bones = bones;
            foreach (var b in _bones)
            {
                b.Object.transform.parent = _root.transform;
            }
        }

        private void CreateHead()
        {
            _head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _head.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f);
            _head.transform.parent = _root.transform;
        }

        #endregion

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
                item.Value.transform.localPosition = ConvertKinectPos(skeleton[item.Key].PositionMm);
            }

            foreach (var bone in _bones)
            {
                PositionBone(bone, skeleton);
            }

            PositionHead(skeleton);

            _root.SetActive(true);
        }

        private static void PositionBone(Bone bone, Skeleton skeleton)
        {
            var parentPos = ConvertKinectPos(skeleton[bone.ParentJoint].PositionMm);
            var direction = ConvertKinectPos(skeleton[bone.ChildJoint].PositionMm) - parentPos;
            bone.Object.transform.localPosition = parentPos;
            bone.Object.transform.localScale = new Vector3(1, direction.magnitude, 1);
            bone.Object.transform.localRotation = UnityEngine.Quaternion.FromToRotation(Vector3.up, direction);
        }

        private void PositionHead(Skeleton skeleton)
        {
            var headPos = ConvertKinectPos(skeleton[JointType.Head].PositionMm);
            var earPosR = ConvertKinectPos(skeleton[JointType.EarRight].PositionMm);
            var earPosL = ConvertKinectPos(skeleton[JointType.EarLeft].PositionMm);
            var headCenter = 0.5f * (earPosR + earPosL);
            var d = (earPosR - earPosL).magnitude;
            _head.transform.localPosition = headCenter;
            _head.transform.localRotation = UnityEngine.Quaternion.FromToRotation(Vector3.up, headCenter - headPos);
            _head.transform.localScale = new Vector3(d, 2 * (headCenter - headPos).magnitude, d);
        }

        private static Vector3 ConvertKinectPos(Float3 pos)
        {
            // Kinect Y axis points down, so negate Y coordinate
            // Scale to convert millimeters to meters
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
            // Other transforms (positioning of the skeleton in the scene, mirroring)
            // are handled by properties of ascendant GameObject's
            return 0.001f * new Vector3(pos.X, -pos.Y, pos.Z);
        }

        private void HideSkeleton()
        {
            _root.SetActive(false);
        }
    }
}