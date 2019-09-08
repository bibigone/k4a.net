using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class CharacterAnimator : MonoBehaviour
    {
        private IReadOnlyDictionary<HumanBodyBones, Transform> _boneTransforms;

        private void Awake()
        {
            var animator = GetComponent<Animator>();
            _boneTransforms = typeof(HumanBodyBones).GetEnumValues().Cast<HumanBodyBones>()
                .Except(new[] { HumanBodyBones.LastBone })
                .ToDictionary(b => b, b => animator.GetBoneTransform(b));
        }

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
        }
    }
}