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
    /// <summary>Skeleton joint index.</summary>
    public enum JointType
    {
        Pelvis = 0,
        SpineNaval,
        SpineChest,
        Neck,
        ClavicleLeft,
        ShoulderLeft,
        ElbowLeft,
        WristLeft,
        ClavicleRight,
        ShoulderRight,
        ElbowRight,
        WristRight,
        HipLeft,
        KneeLeft,
        AnkleLeft,
        FootLeft,
        HipRight,
        KneeRight,
        AnkleRight,
        FootRight,
        Head,
        Nose,
        EyeLeft,
        EarLeft,
        EyeRight,
        EarRight,
    }
}
