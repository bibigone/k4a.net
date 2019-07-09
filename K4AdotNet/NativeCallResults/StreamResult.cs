namespace K4AdotNet.NativeCallResults
{
    // Defined in record/types.h:
    // typedef enum
    // {
    //     K4A_BUFFER_RESULT_SUCCEEDED = 0,
    //     K4A_BUFFER_RESULT_FAILED,
    //     K4A_STREAM_RESULT_EOF,
    // } k4a_stream_result_t;
    /// <summary>Return codes returned by Azure Kinect playback API.</summary>
    internal enum StreamResult
    {
        /// <summary>The result was successful</summary>
        Succeeded = 0,

        /// <summary>The result was a failure</summary>
        Failed,

        /// <summary>The end of the data stream was reached</summary>
        Eof,
    }
}
