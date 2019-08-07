namespace K4AdotNet.NativeCallResults
{
    // Defined in k4atypes.h:
    // typedef enum
    // {
    //     K4A_BUFFER_RESULT_SUCCEEDED = 0,
    //     K4A_BUFFER_RESULT_FAILED,
    //     K4A_BUFFER_RESULT_TOO_SMALL,
    // } k4a_buffer_result_t;
    //
    /// <summary>Result code returned by Azure Kinect APIs.</summary>
    internal enum BufferResult
    {
        /// <summary>The result was successful</summary>
        Succeeded = 0,

        /// <summary>The result was a failure</summary>
        Failed,

        /// <summary>The input buffer was too small</summary>
        TooSmall,
    }
}
