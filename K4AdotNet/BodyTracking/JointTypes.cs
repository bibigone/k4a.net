using System;
using System.Collections.Generic;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Extensions to <see cref="JointType"/> enumeration. Adds some metadata to <see cref="JointType"/> enumeration.</summary>
    /// <remarks>See https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints#joint-hierarchy for details.</remarks>
    /// <seealso cref="JointType"/>
    public static class JointTypes
    {
        /// <summary>All possible <see cref="JointType"/>s. May be helpful for UI, tests, etc.</summary>
        public static readonly IReadOnlyList<JointType> All = new[]
        {
            JointType.Pelvis,
            JointType.SpineNaval,
            JointType.SpineChest,
            JointType.Neck,
            JointType.ClavicleLeft,
            JointType.ShoulderLeft,
            JointType.ElbowLeft,
            JointType.WristLeft,
            JointType.HandLeft,
            JointType.HandTipLeft,
            JointType.ThumbLeft,
            JointType.ClavicleRight,
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.WristRight,
            JointType.HandRight,
            JointType.HandTipRight,
            JointType.ThumbRight,
            JointType.HipLeft,
            JointType.KneeLeft,
            JointType.AnkleLeft,
            JointType.FootLeft,
            JointType.HipRight,
            JointType.KneeRight,
            JointType.AnkleRight,
            JointType.FootRight,
            JointType.Head,
            JointType.Nose,
            JointType.EyeLeft,
            JointType.EarLeft,
            JointType.EyeRight,
            JointType.EarRight,
        };

        /// <summary>Is it root joint in skeleton structure?</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="jointType"/> is root joint in skeletal hierarchy,
        /// <see langword="false"/> for all other joints.
        /// </returns>
        public static bool IsRoot(this JointType jointType)
            => jointType == JointType.Pelvis;

        /// <summary>Is it face feature?</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="jointType"/> is actually face feature (nose, eye, ear) rather than actual joint of human skeleton,
        /// <see langword="false"/> for all other joints.
        /// </returns>
        public static bool IsFaceFeature(this JointType jointType)
            => jointType == JointType.Nose || jointType == JointType.EarLeft || jointType == JointType.EarRight
                || jointType == JointType.EyeLeft || jointType == JointType.EyeRight;

        /// <summary>Is it left side joint?</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="jointType"/> belongs to the left part of body,
        /// <see langword="false"/> - otherwise (right or center part of body).
        /// </returns>
        public static bool IsLeft(this JointType jointType)
            => jointType == JointType.ClavicleLeft || jointType == JointType.ShoulderLeft || jointType == JointType.ElbowLeft || jointType == JointType.WristLeft
                || jointType == JointType.HandLeft || jointType == JointType.HandTipLeft || jointType == JointType.ThumbLeft
                || jointType == JointType.HipLeft || jointType == JointType.KneeLeft || jointType == JointType.AnkleLeft || jointType == JointType.FootLeft
                || jointType == JointType.EyeLeft || jointType == JointType.EarLeft;

        /// <summary>Is it right side joint?</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="jointType"/> belongs to the right part of body,
        /// <see langword="false"/> - otherwise (left or center part of body).
        /// </returns>
        public static bool IsRight(this JointType jointType)
            => jointType == JointType.ClavicleRight || jointType == JointType.ShoulderRight || jointType == JointType.ElbowRight || jointType == JointType.WristRight
                || jointType == JointType.HandRight || jointType == JointType.HandTipRight || jointType == JointType.ThumbRight
                || jointType == JointType.HipRight || jointType == JointType.KneeRight || jointType == JointType.AnkleRight || jointType == JointType.FootRight
                || jointType == JointType.EyeRight || jointType == JointType.EarRight;

        /// <summary>Gets parent joint for a given one.</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>
        /// Parent joint of <paramref name="jointType"/> in skeletal hierarchy
        /// or value of <paramref name="jointType"/> if it is root joint (see <see cref="IsRoot(JointType)"/>).
        /// </returns>
        /// <remarks>See https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints#joint-hierarchy for details.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Unknown joint.</exception>
        public static JointType GetParent(this JointType jointType)
        {
            switch (jointType)
            {
                // Spine
                case JointType.Pelvis: return JointType.Pelvis;
                case JointType.SpineNaval: return JointType.Pelvis;
                case JointType.SpineChest: return JointType.SpineNaval;
                case JointType.Neck: return JointType.SpineChest;
                // Left arm
                case JointType.ClavicleLeft: return JointType.SpineChest;
                case JointType.ShoulderLeft: return JointType.ClavicleLeft;
                case JointType.ElbowLeft: return JointType.ShoulderLeft;
                case JointType.WristLeft: return JointType.ElbowLeft;
                // Left hand
                case JointType.HandLeft: return JointType.WristLeft;
                case JointType.HandTipLeft: return JointType.HandLeft;
                case JointType.ThumbLeft: return JointType.WristLeft;
                // Right arm
                case JointType.ClavicleRight: return JointType.SpineChest;
                case JointType.ShoulderRight: return JointType.ClavicleRight;
                case JointType.ElbowRight: return JointType.ShoulderRight;
                case JointType.WristRight: return JointType.ElbowRight;
                // Right hand
                case JointType.HandRight: return JointType.WristRight;
                case JointType.HandTipRight: return JointType.HandRight;
                case JointType.ThumbRight: return JointType.WristRight;
                // Left leg
                case JointType.HipLeft: return JointType.Pelvis;
                case JointType.KneeLeft: return JointType.HipLeft;
                case JointType.AnkleLeft: return JointType.KneeLeft;
                case JointType.FootLeft: return JointType.AnkleLeft;
                // Right leg
                case JointType.HipRight: return JointType.Pelvis;
                case JointType.KneeRight: return JointType.HipRight;
                case JointType.AnkleRight: return JointType.KneeRight;
                case JointType.FootRight: return JointType.AnkleRight;
                // Head and face
                case JointType.Head: return JointType.Neck;
                case JointType.Nose: return JointType.Head;
                case JointType.EyeLeft: return JointType.Head;
                case JointType.EyeRight: return JointType.Head;
                case JointType.EarLeft: return JointType.Head;
                case JointType.EarRight: return JointType.Head;
                // Unknown
                default: throw new ArgumentOutOfRangeException(nameof(jointType));
            }
        }

        /// <summary>Mirrors left joint to appropriate right one and vice versa. Doesn't change central joints like joints of spine, neck, head.</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>Mirrored joint type.</returns>
        public static JointType Mirror(this JointType jointType)
        {
            switch (jointType)
            {
                // Left arm -> Right arm
                case JointType.ClavicleLeft: return JointType.ClavicleRight;
                case JointType.ShoulderLeft: return JointType.ShoulderRight;
                case JointType.ElbowLeft: return JointType.ElbowRight;
                case JointType.WristLeft: return JointType.WristRight;
                // Left hand -> Right hand
                case JointType.HandLeft: return JointType.HandRight;
                case JointType.HandTipLeft: return JointType.HandTipRight;
                case JointType.ThumbLeft: return JointType.ThumbRight;
                // Right arm -> Left arm
                case JointType.ClavicleRight: return JointType.ClavicleLeft;
                case JointType.ShoulderRight: return JointType.ShoulderLeft;
                case JointType.ElbowRight: return JointType.ElbowLeft;
                case JointType.WristRight: return JointType.WristLeft;
                // Right hand -> Left hand
                case JointType.HandRight: return JointType.HandLeft;
                case JointType.HandTipRight: return JointType.HandTipLeft;
                case JointType.ThumbRight: return JointType.ThumbLeft;
                // Left leg -> Right leg
                case JointType.HipLeft: return JointType.HipRight;
                case JointType.KneeLeft: return JointType.KneeRight;
                case JointType.AnkleLeft: return JointType.AnkleRight;
                case JointType.FootLeft: return JointType.FootRight;
                case JointType.HipRight: return JointType.HipLeft;
                // Right leg -> Left leg
                case JointType.KneeRight: return JointType.KneeLeft;
                case JointType.AnkleRight: return JointType.AnkleLeft;
                case JointType.FootRight: return JointType.FootLeft;
                // Face
                case JointType.EyeLeft: return JointType.EyeRight;
                case JointType.EarLeft: return JointType.EarRight;
                case JointType.EyeRight: return JointType.EyeLeft;
                case JointType.EarRight: return JointType.EarLeft;
                // Invariant joints
                default: return jointType;
            }
        }
    }
}
