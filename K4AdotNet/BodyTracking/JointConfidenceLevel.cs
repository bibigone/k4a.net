namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef enum
    // {
    //     K4ABT_JOINT_CONFIDENCE_NONE = 0,
    //     K4ABT_JOINT_CONFIDENCE_LOW = 1,
    //     K4ABT_JOINT_CONFIDENCE_MEDIUM = 2,
    //     K4ABT_JOINT_CONFIDENCE_HIGH = 3,
    //     K4ABT_JOINT_CONFIDENCE_LEVELS_COUNT = 4,
    // } k4abt_joint_confidence_level_t;
    //
    /// <summary>This enumeration specifies the joint confidence level.</summary>
    public enum JointConfidenceLevel
    {
        /// <summary>The joint is out of range (too far from depth camera).</summary>
        None = 0,

        /// <summary>The joint is not observed (likely due to occlusion), predicted joint pose.</summary>
        Low = 1,

        /// <summary>Medium confidence in joint pose. Current SDK will only provide joints up to this confidence level.</summary>
        Medium = 2,

        /// <summary>High confidence in joint pose. Placeholder for future SDK.</summary>
        High = 3,
    }
}