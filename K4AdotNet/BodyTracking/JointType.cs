namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef enum
    // {
    //     K4ABT_JOINT_PELVIS = 0,
    //     K4ABT_JOINT_SPINE_NAVAL,
    //     K4ABT_JOINT_SPINE_CHEST,
    //     K4ABT_JOINT_NECK,
    //     K4ABT_JOINT_CLAVICLE_LEFT,
    //     K4ABT_JOINT_SHOULDER_LEFT,
    //     K4ABT_JOINT_ELBOW_LEFT,
    //     K4ABT_JOINT_WRIST_LEFT,
    //     K4ABT_JOINT_CLAVICLE_RIGHT,
    //     K4ABT_JOINT_SHOULDER_RIGHT,
    //     K4ABT_JOINT_ELBOW_RIGHT,
    //     K4ABT_JOINT_WRIST_RIGHT,
    //     K4ABT_JOINT_HIP_LEFT,
    //     K4ABT_JOINT_KNEE_LEFT,
    //     K4ABT_JOINT_ANKLE_LEFT,
    //     K4ABT_JOINT_FOOT_LEFT,
    //     K4ABT_JOINT_HIP_RIGHT,
    //     K4ABT_JOINT_KNEE_RIGHT,
    //     K4ABT_JOINT_ANKLE_RIGHT,
    //     K4ABT_JOINT_FOOT_RIGHT,
    //     K4ABT_JOINT_HEAD,
    //     K4ABT_JOINT_NOSE,
    //     K4ABT_JOINT_EYE_LEFT,
    //     K4ABT_JOINT_EAR_LEFT,
    //     K4ABT_JOINT_EYE_RIGHT,
    //     K4ABT_JOINT_EAR_RIGHT,
    //     K4ABT_JOINT_COUNT
    // } k4abt_joint_id_t;
    //
    /// <summary>Skeleton joint index.</summary>
    /// <remarks>See https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints#joint-hierarchy for details.</remarks>
    /// <seealso cref="JointTypes"/>
    /// <seealso cref="Skeleton"/>
    public enum JointType
    {
        /// <summary>Pelvis joint.</summary>
        Pelvis = 0,

        /// <summary>Naval spine joint.</summary>
        SpineNaval,

        /// <summary>Chest spine joint.</summary>
        SpineChest,

        /// <summary>Neck joint.</summary>
        Neck,

        /// <summary>Left clavicle joint.</summary>
        ClavicleLeft,

        /// <summary>Left shoulder joint.</summary>
        ShoulderLeft,

        /// <summary>Left elbow joint.</summary>
        ElbowLeft,

        /// <summary>Left wrist joint.</summary>
        WristLeft,

        /// <summary>Right clavicle joint.</summary>
        ClavicleRight,

        /// <summary>Right shoulder joint.</summary>
        ShoulderRight,

        /// <summary>Right elbow joint.</summary>
        ElbowRight,

        /// <summary>Right wrist joint.</summary>
        WristRight,

        /// <summary>Left hip joint.</summary>
        HipLeft,

        /// <summary>Left knee joint.</summary>
        KneeLeft,

        /// <summary>Left ankle joint.</summary>
        AnkleLeft,

        /// <summary>Left foot joint.</summary>
        FootLeft,

        /// <summary>Right hip joint.</summary>
        HipRight,

        /// <summary>Right knee joint.</summary>
        KneeRight,

        /// <summary>Right ankle joint.</summary>
        AnkleRight,

        /// <summary>Right fool joint.</summary>
        FootRight,

        /// <summary>Head joint.</summary>
        Head,

        /// <summary>Nose.</summary>
        Nose,

        /// <summary>Left eye.</summary>
        EyeLeft,

        /// <summary>Left ear.</summary>
        EarLeft,

        /// <summary>Right eye.</summary>
        EyeRight,

        /// <summary>Right ear.</summary>
        EarRight,
    }
}
