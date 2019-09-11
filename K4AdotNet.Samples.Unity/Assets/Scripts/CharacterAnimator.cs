using K4AdotNet.BodyTracking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    using Quaternion = UnityEngine.Quaternion;

    public class CharacterAnimator : MonoBehaviour
    {
        private GameObject _skin;

        private void Awake()
        {
            CreateBones();
            _skin = GetComponentInChildren<SkinnedMeshRenderer>()?.gameObject;
        }

        #region Joints

        private IReadOnlyDictionary<JointType, JointData> _joints;

        private class JointData
        {
            public JointData(JointType jointType, Transform transform, Quaternion tposeOrientation, Quaternion kinectTPoseOrientationInverse)
            {
                Transform = transform;
                TPoseOrientation = tposeOrientation;
                KinectTPoseOrientationInverse = kinectTPoseOrientationInverse;
            }

            /// <summary>
            /// Joint's transform.
            /// </summary>
            public Transform Transform { get; }

            /// <summary>
            /// Joint orientation in T-pose, in coordinate space of a character
            /// </summary>
            public Quaternion TPoseOrientation { get; }

            /// <summary>
            /// Inverse quaternion to orientation of Kinect joint in T-pose, in coordinate space of a character
            /// </summary>
            public Quaternion KinectTPoseOrientationInverse { get; }
        }

        private void CreateBones()
        {
            var animator = GetComponent<Animator>();
            var bones = JointTypes.All
                .ToDictionary(jt => jt, jt => GetJointData(jt, animator));

            _joints = bones;
        }

        private static JointData GetJointData(JointType jointType, Animator animator)
        {
            var hbb = MapKinectJoint(jointType);
            if (hbb == HumanBodyBones.LastBone)
                return null;

            var transform = animator.GetBoneTransform(hbb);
            if (transform == null)
                return null;

            var rootName = animator.GetBoneTransform(HumanBodyBones.Hips).name;
            var tposeOrientation = GetSkeletonBoneRotation(animator, transform.name);
            var t = transform;
            while (t.name != rootName)
            {
                t = t.parent;
                tposeOrientation = GetSkeletonBoneRotation(animator, t.name) * tposeOrientation;
            }

            var kinectTPoseOrientationInverse = GetKinectTPoseOrientationInverse(jointType);

            return new JointData(jointType, transform, tposeOrientation, kinectTPoseOrientationInverse);
        }

        private static Quaternion GetSkeletonBoneRotation(Animator animator, string boneName)
            => animator.avatar.humanDescription.skeleton.First(sb => sb.name == boneName).rotation;

        private static HumanBodyBones MapKinectJoint(JointType joint)
        {
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
            switch (joint)
            {
                case JointType.Pelvis: return HumanBodyBones.Hips;
                case JointType.SpineNaval: return HumanBodyBones.Spine;
                case JointType.SpineChest: return HumanBodyBones.Chest;
                case JointType.Neck: return HumanBodyBones.Neck;
                case JointType.Head: return HumanBodyBones.Head;
                case JointType.HipLeft: return HumanBodyBones.LeftUpperLeg;
                case JointType.KneeLeft: return HumanBodyBones.LeftLowerLeg;
                case JointType.AnkleLeft: return HumanBodyBones.LeftFoot;
                case JointType.FootLeft: return HumanBodyBones.LeftToes;
                case JointType.HipRight: return HumanBodyBones.RightUpperLeg;
                case JointType.KneeRight: return HumanBodyBones.RightLowerLeg;
                case JointType.AnkleRight: return HumanBodyBones.RightFoot;
                case JointType.FootRight: return HumanBodyBones.RightToes;
                case JointType.ClavicleLeft: return HumanBodyBones.LeftShoulder;
                case JointType.ShoulderLeft: return HumanBodyBones.LeftUpperArm;
                case JointType.ElbowLeft: return HumanBodyBones.LeftLowerArm;
                case JointType.WristLeft: return HumanBodyBones.LeftHand;
                case JointType.ClavicleRight: return HumanBodyBones.RightShoulder;
                case JointType.ShoulderRight: return HumanBodyBones.RightUpperArm;
                case JointType.ElbowRight: return HumanBodyBones.RightLowerArm;
                case JointType.WristRight: return HumanBodyBones.RightHand;
                default: return HumanBodyBones.LastBone;
            }
        }

        private static Quaternion GetKinectTPoseOrientationInverse(JointType jointType)
        {
            switch (jointType)
            {
                case JointType.Pelvis:
                case JointType.SpineNaval:
                case JointType.SpineChest:
                case JointType.Neck:
                case JointType.Head:
                case JointType.HipLeft:
                case JointType.KneeLeft:
                case JointType.AnkleLeft:
                    return Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up);

                case JointType.FootLeft:
                    return Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.right);

                case JointType.HipRight:
                case JointType.KneeRight:
                case JointType.AnkleRight:
                    return Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up);

                case JointType.FootRight:
                    return Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.right);

                case JointType.ClavicleLeft:
                case JointType.ShoulderLeft:
                case JointType.ElbowLeft:
                    return Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);

                case JointType.WristLeft:
                    return Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(180, Vector3.right);

                case JointType.ClavicleRight:
                case JointType.ShoulderRight:
                case JointType.ElbowRight:
                    return Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.right);

                case JointType.WristRight:
                    return Quaternion.AngleAxis(180, Vector3.up);

                default:
                    return Quaternion.identity;
            }
        }

        #endregion

        private void OnEnable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();
            if (skeletonProvider != null)
            {
                skeletonProvider.SkeletonUpdated += SkeletonProvider_SkeletonUpdated;
            }
        }

        private void OnDisable()
        {
            var skeletonProvider = FindObjectOfType<SkeletonProvider>();
            if (skeletonProvider != null)
            {
                skeletonProvider.SkeletonUpdated -= SkeletonProvider_SkeletonUpdated;
            }
        }

        private void SkeletonProvider_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            if (e.Skeleton != null)
            {
                ApplySkeleton(e.Skeleton.Value);
                _skin?.SetActive(true);
            }
            else
            {
                _skin?.SetActive(false);
            }
        }

        private void ApplySkeleton(Skeleton skeleton)
        {
            var joints = JointTypes.All;
            var characterPos = ConvertKinectPos(skeleton.Pelvis.PositionMm);
            transform.localPosition = characterPos;

            foreach (var joint in joints)
            {
                var data = _joints[joint];
                if (data != null)
                {
                    var orientation = ConvertKinectQ(skeleton[joint].Orientation);
                    data.Transform.rotation = transform.rotation * orientation * data.KinectTPoseOrientationInverse * data.TPoseOrientation;
                }
            }
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

        private static Quaternion ConvertKinectQ(K4AdotNet.Quaternion q)
        {
            // Convert to Unity coordinates (right-handed Y down to left-handed Y up)
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
            // https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system
            return new Quaternion(-q.X, q.Y, -q.Z, q.W);
        }
    }
}