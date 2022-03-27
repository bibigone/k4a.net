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

        private Transform _rootJointTransform;
        private float _rootJointTPoseY;
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
            _rootJointTransform = animator.GetBoneTransform(HumanBodyBones.Hips);

            // Align root joint with character
            // Now we can place character in camera coord space
            _rootJointTransform.localPosition = Vector3.zero;

            var bones = JointTypes.All
                .ToDictionary(jt => jt, jt => GetJointData(jt, animator));

            _joints = bones;
        }

        private JointData GetJointData(JointType jointType, Animator animator)
        {
            var hbb = MapKinectJoint(jointType);
            if (hbb == HumanBodyBones.LastBone)
                return null;

            var transform = animator.GetBoneTransform(hbb);
            if (transform == null)
                return null;

            var tposeOrientation = GetSkeletonBone(animator, transform.name).rotation;
            var t = transform;
            while (!ReferenceEquals(t, _rootJointTransform))
            {
                t = t.parent;
                tposeOrientation = GetSkeletonBone(animator, t.name).rotation * tposeOrientation;
            }

            var kinectTPoseOrientationInverse = GetKinectTPoseOrientationInverse(jointType);

            return new JointData(jointType, transform, tposeOrientation, kinectTPoseOrientationInverse);
        }

        private static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
            => animator.avatar.humanDescription.skeleton.First(sb => sb.name == boneName);

        private static HumanBodyBones MapKinectJoint(JointType joint)
        {
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
            switch (joint)
            {
                case JointType.Pelvis: return HumanBodyBones.Hips;
                case JointType.SpineNavel: return HumanBodyBones.Spine;
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
            // Used this page as reference for T-pose orientations
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
            // Assuming T-pose as body facing Z+, with head at Y+. Same for target character
            // Coordinate system seems to be left-handed not right handed as depicted
            // Thus inverse T-pose rotation should align Y and Z axes of depicted local coords for a joint with body coords in T-pose
            switch (jointType)
            {
                case JointType.Pelvis:
                case JointType.SpineNavel:
                case JointType.SpineChest:
                case JointType.Neck:
                case JointType.Head:
                case JointType.HipLeft:
                case JointType.KneeLeft:
                case JointType.AnkleLeft:
                    return Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

                case JointType.FootLeft:
                    return Quaternion.AngleAxis(-90, Vector3.up);

                case JointType.HipRight:
                case JointType.KneeRight:
                case JointType.AnkleRight:
                    return Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

                case JointType.FootRight:
                    return Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

                case JointType.ClavicleLeft:
                case JointType.ShoulderLeft:
                case JointType.ElbowLeft:
                    return Quaternion.AngleAxis(90, Vector3.right);

                case JointType.WristLeft:
                    return Quaternion.AngleAxis(180, Vector3.right);

                case JointType.ClavicleRight:
                case JointType.ShoulderRight:
                case JointType.ElbowRight:
                    return Quaternion.AngleAxis(-90, Vector3.right);

                case JointType.WristRight:
                    return Quaternion.identity;

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
                    //文字スペースでの回転=文字スペースでのTポーズへの回転rel*文字スペースでのTポーズ
                    //文字スペースでのTポーズ=data.TPoseOrientation
                    //文字空間でのTポーズへの回転rel=Kinect回転*inv（Kinect Tポーズ）
                    //文字スペースでの回転=root.localRotation* ... * localRotation
                    //ローカル回転=inv（文字空間での親回転）*文字空間での回転
                    // inv（文字スペースでの親の回転）= inv（parent.localRotation）* ... * inv（root.localRotation）
                    var rotationRel2TPoseInCharacterSpace = orientation * data.KinectTPoseOrientationInverse;
                    var rotationInCharacterSpace = rotationRel2TPoseInCharacterSpace * data.TPoseOrientation;
                    var invParentRotationInCharacterSpace = Quaternion.identity;
                    var t = data.Transform;
                    while (!ReferenceEquals(t, _rootJointTransform))
                    {
                        t = t.parent;
                        invParentRotationInCharacterSpace = invParentRotationInCharacterSpace * Quaternion.Inverse(t.localRotation);
                    }
                    data.Transform.localRotation = invParentRotationInCharacterSpace * rotationInCharacterSpace;
                }
            }
        }


        private static Vector3 ConvertKinectPos(Float3 pos)
        {
            // Kinect Y軸が下を向いているため、Y座標を無効にします
            //ミリメートルをメートルに変換するためのスケール
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
            //その他の変換（シーン内のスケルトンの配置、ミラーリング）
            //アセンダントGameObjectのプロパティによって処理されます
            return 0.001f * new Vector3(pos.X, -pos.Y, pos.Z);
        }

        private static Quaternion ConvertKinectQ(K4AdotNet.Quaternion q)
        {
            //回転のKinect座標系は
            //左利きのY+を上に、Z+をカメラに向けて
            //したがって、Yの周りに180回転を適用して、Unity座標（カメラからZを離して）に合わせます
            return Quaternion.AngleAxis(180, Vector3.up) * new Quaternion(q.X, q.Y, q.Z, q.W);
        }
    }
}