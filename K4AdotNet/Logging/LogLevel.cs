namespace K4AdotNet.Logging
{
    // k4a_log_level_t
    /// <summary>Verbosity levels of debug messaging.</summary>
    internal enum LogLevel
    {
        // K4A_LOG_LEVEL_CRITICAL
        /// <summary>The most severe level of debug messaging.</summary>
        Critical = 0,

        // K4A_LOG_LEVEL_ERROR
        /// <summary>The second most severe level of debug messaging.</summary>
        Error,

        // K4A_LOG_LEVEL_WARNING
        /// <summary>The third most severe level of debug messaging.</summary>
        Warning,

        // K4A_LOG_LEVEL_INFO
        /// <summary>The second least severe level of debug messaging.</summary>
        Information,

        // K4A_LOG_LEVEL_TRACE
        /// <summary>The lest severe level of debug messaging. This is the most verbose messaging possible.</summary>
        Trace,

        // K4A_LOG_LEVEL_OFF
        /// <summary>No logging is performed.</summary>
        Off,
    }
}
