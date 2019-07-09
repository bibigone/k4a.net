namespace K4AdotNet.NativeCallResults
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_WAIT_RESULT_SUCCEEDED = 0,
    //     K4A_WAIT_RESULT_FAILED,
    //     K4A_WAIT_RESULT_TIMEOUT,
    // } k4a_wait_result_t;
    /// <summary>Result code returned by Azure Kinect APIs.</summary>
    internal enum WaitResult
    {
        /// <summary>The result was successful</summary>
        Succeeded = 0,

        /// <summary>The result was a failure</summary>
        Failed,

        /// <summary>The operation timed out</summary>
        Timeout,
    }
}
