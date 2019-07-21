using System.Collections.Generic;

namespace K4AdotNet.BodyTracking
{
    public static class JointTypes
    {
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
            JointType.ClavicleRight,
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.WristRight,
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

        public static JointType Mirror(this JointType jointType)
        {
            switch (jointType)
            {
                case JointType.ClavicleLeft: return JointType.ClavicleRight;
                case JointType.ShoulderLeft: return JointType.ShoulderRight;
                case JointType.ElbowLeft: return JointType.ElbowRight;
                case JointType.WristLeft: return JointType.WristRight;
                case JointType.ClavicleRight: return JointType.ClavicleLeft;
                case JointType.ShoulderRight: return JointType.ShoulderLeft;
                case JointType.ElbowRight: return JointType.ElbowLeft;
                case JointType.WristRight: return JointType.WristLeft;
                case JointType.HipLeft: return JointType.HipRight;
                case JointType.KneeLeft: return JointType.KneeRight;
                case JointType.AnkleLeft: return JointType.AnkleRight;
                case JointType.FootLeft: return JointType.FootRight;
                case JointType.HipRight: return JointType.HipLeft;
                case JointType.KneeRight: return JointType.KneeLeft;
                case JointType.AnkleRight: return JointType.AnkleLeft;
                case JointType.FootRight: return JointType.FootLeft;
                case JointType.EyeLeft: return JointType.EyeRight;
                case JointType.EarLeft: return JointType.EarRight;
                case JointType.EyeRight: return JointType.EyeLeft;
                case JointType.EarRight: return JointType.EarLeft;
                default: return jointType;
            }
        }
    }
}
