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
    // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
    /// <summary>Structure to define joints for skeleton.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Skeleton : IEnumerable<Joint>
    {
        [MarshalAs(UnmanagedType.Struct)]
        public Joint Pelvis;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint SpineNaval;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint SpineChest;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint Neck;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ClavicleLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ShoulderLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ElbowLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint WristLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ClavicleRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ShoulderRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint ElbowRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint WristRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint HipLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint KneeLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint AnkleLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint FootLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint HipRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint KneeRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint AnkleRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint FootRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint Head;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint Nose;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint EyeLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint EarLeft;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint EyeRight;

        [MarshalAs(UnmanagedType.Struct)]
        public Joint EarRight;

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

        public Joint this[int index]
        {
            get => this[(JointType)index];
            set => this[(JointType)index] = value;
        }

        public Joint[] ToArray()
        {
            var res = new Joint[JointTypes.All.Count];
            for (var i = 0; i < res.Length; i++)
                res[i] = this[i];
            return res;
        }

        public IEnumerator<Joint> GetEnumerator()
        {
            foreach (var jointType in JointTypes.All)
                yield return this[jointType];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var jointType in JointTypes.All)
                yield return this[jointType];
        }
    }
}
