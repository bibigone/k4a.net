using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    // Defined in k4abttypes.h:
    // typedef struct _k4abt_joint_t
    // {
    //     k4a_float3_t position;    /**< The position of the joint specified in millimeters*/
    //     k4a_quaternion_t orientation; /**< The orientation of the joint specified in normalized quaternion*/
    // } k4abt_joint_t;
    //
    /// <summary>Structure to define a single joint.</summary>
    /// <remarks>
    /// The position and orientation together defines the coordinate system for the given joint.
    /// They are defined relative to the sensor global coordinate system.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct Joint
    {
        /// <summary>The position of the joint specified in millimeters.</summary>
        public Float3 PositionMm;

        /// <summary>The orientation of the joint specified in normalized quaternion.</summary>
        public Quaternion Orientation;
    }
}
