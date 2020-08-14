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
            JointType.SpineNavel,
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
        public static JointType GetParent(this JointType jointType) => jointType switch
        {
            // Spine
            JointType.Pelvis => JointType.Pelvis,
            JointType.SpineNavel => JointType.Pelvis,
            JointType.SpineChest => JointType.SpineNavel,
            JointType.Neck => JointType.SpineChest,
            // Left arm
            JointType.ClavicleLeft => JointType.SpineChest,
            JointType.ShoulderLeft => JointType.ClavicleLeft,
            JointType.ElbowLeft => JointType.ShoulderLeft,
            JointType.WristLeft => JointType.ElbowLeft,
            // Left hand
            JointType.HandLeft => JointType.WristLeft,
            JointType.HandTipLeft => JointType.HandLeft,
            JointType.ThumbLeft => JointType.WristLeft,
            // Right arm
            JointType.ClavicleRight => JointType.SpineChest,
            JointType.ShoulderRight => JointType.ClavicleRight,
            JointType.ElbowRight => JointType.ShoulderRight,
            JointType.WristRight => JointType.ElbowRight,
            // Right hand
            JointType.HandRight => JointType.WristRight,
            JointType.HandTipRight => JointType.HandRight,
            JointType.ThumbRight => JointType.WristRight,
            // Left leg
            JointType.HipLeft => JointType.Pelvis,
            JointType.KneeLeft => JointType.HipLeft,
            JointType.AnkleLeft => JointType.KneeLeft,
            JointType.FootLeft => JointType.AnkleLeft,
            // Right leg
            JointType.HipRight => JointType.Pelvis,
            JointType.KneeRight => JointType.HipRight,
            JointType.AnkleRight => JointType.KneeRight,
            JointType.FootRight => JointType.AnkleRight,
            // Head and face
            JointType.Head => JointType.Neck,
            JointType.Nose => JointType.Head,
            JointType.EyeLeft => JointType.Head,
            JointType.EyeRight => JointType.Head,
            JointType.EarLeft => JointType.Head,
            JointType.EarRight => JointType.Head,
            // Unknown
            _ => throw new ArgumentOutOfRangeException(nameof(jointType)),
        };

        /// <summary>Mirrors left joint to appropriate right one and vice versa. Doesn't change central joints like joints of spine, neck, head.</summary>
        /// <param name="jointType">Joint type asked about.</param>
        /// <returns>Mirrored joint type.</returns>
        public static JointType Mirror(this JointType jointType) => jointType switch
        {
            // Left arm -> Right arm
            JointType.ClavicleLeft => JointType.ClavicleRight,
            JointType.ShoulderLeft => JointType.ShoulderRight,
            JointType.ElbowLeft => JointType.ElbowRight,
            JointType.WristLeft => JointType.WristRight,
            // Left hand -> Right hand
            JointType.HandLeft => JointType.HandRight,
            JointType.HandTipLeft => JointType.HandTipRight,
            JointType.ThumbLeft => JointType.ThumbRight,
            // Right arm -> Left arm
            JointType.ClavicleRight => JointType.ClavicleLeft,
            JointType.ShoulderRight => JointType.ShoulderLeft,
            JointType.ElbowRight => JointType.ElbowLeft,
            JointType.WristRight => JointType.WristLeft,
            // Right hand -> Left hand
            JointType.HandRight => JointType.HandLeft,
            JointType.HandTipRight => JointType.HandTipLeft,
            JointType.ThumbRight => JointType.ThumbLeft,
            // Left leg -> Right leg
            JointType.HipLeft => JointType.HipRight,
            JointType.KneeLeft => JointType.KneeRight,
            JointType.AnkleLeft => JointType.AnkleRight,
            JointType.FootLeft => JointType.FootRight,
            JointType.HipRight => JointType.HipLeft,
            // Right leg -> Left leg
            JointType.KneeRight => JointType.KneeLeft,
            JointType.AnkleRight => JointType.AnkleLeft,
            JointType.FootRight => JointType.FootLeft,
            // Face
            JointType.EyeLeft => JointType.EyeRight,
            JointType.EarLeft => JointType.EarRight,
            JointType.EyeRight => JointType.EyeLeft,
            JointType.EarRight => JointType.EarLeft,
            // Invariant joints
            _ => jointType,
        };
    }
}
