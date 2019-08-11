using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef struct _k4abt_skeleton_t
    // {
    //     k4abt_joint_t joints[K4ABT_JOINT_COUNT];
    // } k4abt_skeleton_t;
    //
    /// <summary>Structure to define joints for skeleton.</summary>
    /// <remarks>See https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints#joint-hierarchy for details.</remarks>
    /// <seealso cref="JointType"/>
    /// <seealso cref="JointTypes"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct Skeleton : IEnumerable<Joint>
    {
        #region Fields

        /// <summary>Pelvis joint.</summary>
        public Joint Pelvis;

        /// <summary>Naval spine joint.</summary>
        public Joint SpineNaval;

        /// <summary>Chest spine joint.</summary>
        public Joint SpineChest;

        /// <summary>Neck joint.</summary>
        public Joint Neck;

        /// <summary>Left clavicle joint.</summary>
        public Joint ClavicleLeft;

        /// <summary>Left shoulder joint.</summary>
        public Joint ShoulderLeft;

        /// <summary>Left elbow joint.</summary>
        public Joint ElbowLeft;

        /// <summary>Left wrist joint.</summary>
        public Joint WristLeft;

        /// <summary>Right clavicle joint.</summary>
        public Joint ClavicleRight;

        /// <summary>Right shoulder joint.</summary>
        public Joint ShoulderRight;

        /// <summary>Right elbow joint.</summary>
        public Joint ElbowRight;

        /// <summary>Right wrist joint.</summary>
        public Joint WristRight;

        /// <summary>Left hip joint.</summary>
        public Joint HipLeft;

        /// <summary>Left knee joint.</summary>
        public Joint KneeLeft;

        /// <summary>Left ankle joint.</summary>
        public Joint AnkleLeft;

        /// <summary>Left foot joint.</summary>
        public Joint FootLeft;

        /// <summary>Right hip joint.</summary>
        public Joint HipRight;

        /// <summary>Right knee joint.</summary>
        public Joint KneeRight;

        /// <summary>Right ankle joint.</summary>
        public Joint AnkleRight;

        /// <summary>Right fool joint.</summary>
        public Joint FootRight;

        /// <summary>Head joint.</summary>
        public Joint Head;

        /// <summary>Nose.</summary>
        public Joint Nose;

        /// <summary>Left eye.</summary>
        public Joint EyeLeft;

        /// <summary>Left ear.</summary>
        public Joint EarLeft;

        /// <summary>Right eye.</summary>
        public Joint EyeRight;

        /// <summary>Right ear.</summary>
        public Joint EarRight;

        #endregion

        #region Index access

        /// <summary>Access to joint by index of type <see cref="JointType"/>.</summary>
        /// <param name="index">Index of joint.</param>
        /// <returns>Joint information.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Unknown value of <paramref name="index"/>.</exception>
        /// <seealso cref="JointTypes.All"/>
        public Joint this[JointType index]
        {
            get
            {
                switch (index)
                {
                    case JointType.Pelvis: return Pelvis;
                    case JointType.SpineNaval: return SpineNaval;
                    case JointType.SpineChest: return SpineChest;
                    case JointType.Neck: return Neck;
                    case JointType.ClavicleLeft: return ClavicleLeft;
                    case JointType.ShoulderLeft: return ShoulderLeft;
                    case JointType.ElbowLeft: return ElbowLeft;
                    case JointType.WristLeft: return WristLeft;
                    case JointType.ClavicleRight: return ClavicleRight;
                    case JointType.ShoulderRight: return ShoulderRight;
                    case JointType.ElbowRight: return ElbowRight;
                    case JointType.WristRight: return WristRight;
                    case JointType.HipLeft: return HipLeft;
                    case JointType.KneeLeft: return KneeLeft;
                    case JointType.AnkleLeft: return AnkleLeft;
                    case JointType.FootLeft: return FootLeft;
                    case JointType.HipRight: return HipRight;
                    case JointType.KneeRight: return KneeRight;
                    case JointType.AnkleRight: return AnkleRight;
                    case JointType.FootRight: return FootRight;
                    case JointType.Head: return Head;
                    case JointType.Nose: return Nose;
                    case JointType.EyeLeft: return EyeLeft;
                    case JointType.EarLeft: return EarLeft;
                    case JointType.EyeRight: return EyeRight;
                    case JointType.EarRight: return EarRight;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                switch (index)
                {
                    case JointType.Pelvis: Pelvis = value; break;
                    case JointType.SpineNaval: SpineNaval = value; break;
                    case JointType.SpineChest: SpineChest = value; break;
                    case JointType.Neck: Neck = value; break;
                    case JointType.ClavicleLeft: ClavicleLeft = value; break;
                    case JointType.ShoulderLeft: ShoulderLeft = value; break;
                    case JointType.ElbowLeft: ElbowLeft = value; break;
                    case JointType.WristLeft: WristLeft = value; break;
                    case JointType.ClavicleRight: ClavicleRight = value; break;
                    case JointType.ShoulderRight: ShoulderRight = value; break;
                    case JointType.ElbowRight: ElbowRight = value; break;
                    case JointType.WristRight: WristRight = value; break;
                    case JointType.HipLeft: HipLeft = value; break;
                    case JointType.KneeLeft: KneeLeft = value; break;
                    case JointType.AnkleLeft: AnkleLeft = value; break;
                    case JointType.FootLeft: FootLeft = value; break;
                    case JointType.HipRight: HipRight = value; break;
                    case JointType.KneeRight: KneeRight = value; break;
                    case JointType.AnkleRight: AnkleRight = value; break;
                    case JointType.FootRight: FootRight = value; break;
                    case JointType.Head: Head = value; break;
                    case JointType.Nose: Nose = value; break;
                    case JointType.EyeLeft: EyeLeft = value; break;
                    case JointType.EarLeft: EarLeft = value; break;
                    case JointType.EyeRight: EyeRight = value; break;
                    case JointType.EarRight: EarRight = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>Access to joint by integer index.</summary>
        /// <param name="index">Index of joint.</param>
        /// <returns>Joint information.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of <paramref name="index"/>.</exception>
        public Joint this[int index]
        {
            get => this[(JointType)index];
            set => this[(JointType)index] = value;
        }

        /// <summary>Converts structure to array representation.</summary>
        /// <returns>Array representation of skeletal data. Not <see langword="null"/>.</returns>
        public Joint[] ToArray()
        {
            var res = new Joint[JointTypes.All.Count];
            for (var i = 0; i < res.Length; i++)
                res[i] = this[i];
            return res;
        }

        #endregion

        #region IEnumerable

        /// <summary>Implementation of <see cref="IEnumerable{Joint}"/>.</summary>
        /// <returns>Enumerator for all joints. Not <see langword="null"/>.</returns>
        public IEnumerator<Joint> GetEnumerator()
        {
            foreach (var jointType in JointTypes.All)
                yield return this[jointType];
        }

        /// <summary>Implementation of <see cref="IEnumerable"/>.</summary>
        /// <returns>Enumerator for all joints. Not <see langword="null"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var jointType in JointTypes.All)
                yield return this[jointType];
        }

        #endregion
    }
}
