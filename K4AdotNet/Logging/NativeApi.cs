﻿using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Logging
{
    /// <summary>DLL imports from <c>k4a.h</c> header file for native functions that are connected with logging.</summary>
    internal static class NativeApi
    {
        // typedef void (k4a_logging_message_cb_t) (void* context,
        //                                          k4a_log_level_t level,
        //                                          const char* file,
        //                                          const int line,
        //                                          const char* message);
        /// <summary>Callback function for debug messages being generated by the Azure Kinect SDK.</summary>
        /// <param name="context">The context of the callback function. This is the context that was supplied by the caller to <see cref="SetDebugMessageHandler(LoggingMessageCallback?, IntPtr, LogLevel)"/>.</param>
        /// <param name="level">The level of the message that has been created.</param>
        /// <param name="file">The file name of the source file that generated the message.</param>
        /// <param name="line">The line number of the source file that generated the message.</param>
        /// <param name="message">The messaged generated by the Azure Kinect SDK.</param>
        /// <remarks><para>
        /// The callback is called asynchronously when the Azure Kinext SDK generates a message at a <paramref name="level"/> that is equal to
        /// or more critical than the level specified when calling <see cref="SetDebugMessageHandler(LoggingMessageCallback?, IntPtr, LogLevel)"/> to register the callback.
        /// </para><para>
        /// This callback can occur from any thread and blocks the calling thread.This callback user
        /// must protect it's logging resources from concurrent calls. All care should be made to minimize the amount of time
        /// locks are held.
        /// </para></remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LoggingMessageCallback(
            IntPtr context,
            LogLevel level,
            [MarshalAs(UnmanagedType.LPStr)] string file,
            int line,
            [MarshalAs(UnmanagedType.LPStr)] string message);

        // K4A_EXPORT k4a_result_t k4a_set_debug_message_handler(k4a_logging_message_cb_t* message_cb,
        //                                                       void* message_cb_context,
        //                                                       k4a_log_level_t min_level);
        /// <summary>Sets and clears the callback function to receive debug messages from the Azure Kinect device.</summary>
        /// <param name="messageCallback">The callback function to receive messages from. Set to <see langword="null"/> to unregister the callback function.</param>
        /// <param name="messageCallbackContext">The callback functions context.</param>
        /// <param name="minLevel">The least critical error the user wants to be notified about.</param>
        /// <returns>
        ///     <see cref="NativeCallResults.Result.Succeeded"/> if the callback function was set or cleared successfully.
        ///     <see cref="NativeCallResults.Result.Failed"/> if an error is encountered or the callback function has already been set.
        /// </returns>
        /// <remarks><para>
        /// Call this function to set or clear the callback function that is used to deliver debug messages to the caller. This
        /// callback may be called concurrently, it is up to the implementation of the callback function to ensure the
        /// parallelization is handled.
        /// </para><para>
        /// Clearing the callback function will block until all pending calls to the callback function have completed.
        /// </para><para>
        /// To update <paramref name="minLevel"/>, this method can be called with the same value <paramref name="messageCallback"/> and by
        /// specifying a new <paramref name="minLevel"/>.
        /// </para><para>
        /// Logging provided via this API is independent of the logging controlled by the environmental variable controls
        /// <c>K4A_ENABLE_LOG_TO_STDOUT</c>, <c>K4A_ENABLE_LOG_TO_A_FILE</c>, and <c>K4A_LOG_LEVEL</c>. However there is a slight change in
        /// default behavior when using this function.By default, when \p k4a_set_debug_message_handler() has not been used to
        /// register a message callback, the default for environmental variable controls is to send debug messages as if
        /// <c>K4A_ENABLE_LOG_TO_STDOUT= 1</c> were set. If this method registers a callback function before
        /// <see cref="Sensor.NativeApi.DeviceOpen(uint, out NativeHandles.DeviceHandle)"/> is called, then the default for environmental controls
        /// is as if <c>K4A_ENABLE_LOG_TO_STDOUT= 0</c> was specified. Physically specifying the environmental control will override the default.
        /// </para></remarks>
        [DllImport(Sdk.SENSOR_DLL_NAME, EntryPoint = "k4a_set_debug_message_handler", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeCallResults.Result SetDebugMessageHandler(
            LoggingMessageCallback? messageCallback,
            IntPtr messageCallbackContext,
            LogLevel minLevel);
    }
}
