namespace K4AdotNet.NativeApiCallResults
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_RESULT_SUCCEEDED = 0,
    //     K4A_RESULT_FAILED,
    // } k4a_result_t;
    /// <summary>Result code returned by Azure Kinect APIs.</summary>
    internal enum Result
    {
        /// <summary>The result was successful</summary>
        Succeeded = 0,

        /// <summary>The result was a failure</summary>
        Failed,
    }
}
