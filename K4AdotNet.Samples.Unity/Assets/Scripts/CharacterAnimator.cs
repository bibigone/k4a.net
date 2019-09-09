using K4AdotNet.BodyTracking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class CharacterAnimator : MonoBehaviour
    {
        private GameObject _skin;

        private void Awake()
        {
            CreateBones();
            _skin = GetComponentInChildren<SkinnedMeshRenderer>()?.gameObject;
        }

        #region Bones

        private IReadOnlyDictionary<HumanBodyBones, BoneData> _bones;

        private class BoneData
        {
            public BoneData(Transform transform, SkeletonBone skeletonBone)
            {
                Transform = transform;
                SkeletonBone = skeletonBone;
            }

            public Transform Transform { get; }
            public SkeletonBone SkeletonBone { get; }
        }

        private void CreateBones()
        {
            var animator = GetComponent<Animator>();
            var bones = typeof(HumanBodyBones).GetEnumValues().Cast<HumanBodyBones>()
                .Except(new[] { HumanBodyBones.LastBone })
                .ToDictionary(b => b, b => GetBoneData(b, animator));
            bones.Add(HumanBodyBones.LastBone, null);

            _bones = bones;
        }

        private static BoneData GetBoneData(HumanBodyBones bone, Animator animator)
        {
            var transform = animator.GetBoneTransform(bone);
            if (transform == null)
                return null;

            var skeletonBone = animator.avatar.humanDescription.skeleton.FirstOrDefault(sb => sb.name == transform.name);
            return new BoneData(transform, skeletonBone);
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
            foreach (var joint in joints)
            {
                var hbb = MapKinectJoint(joint);
                var data = _bones[hbb];
                if (data != null)
                {
                    data.Transform.localRotation = data.SkeletonBone.rotation;
                }
            }
        }

        private static UnityEngine.Quaternion ConvertKinectQ(Quaternion q)
        {
            // Kinect Y axis points down, so negate Y coordinate
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
            return new UnityEngine.Quaternion(q.X, -q.Y, q.Z, q.W);
        }

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
    }
}