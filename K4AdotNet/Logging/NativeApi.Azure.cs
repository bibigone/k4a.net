using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Logging
{
    partial class NativeApi
    {
        public sealed class Azure : NativeApi
        {
            public static readonly Azure Instance = new();

            private Azure() { }

            public override NativeCallResults.Result SetDebugMessageHandler(LoggingMessageCallback? messageCallback, IntPtr messageCallbackContext, LogLevel minLevel)
                => k4a_set_debug_message_handler(messageCallback, messageCallbackContext, minLevel);

            [DllImport(Sdk.Azure.SENSOR_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern NativeCallResults.Result k4a_set_debug_message_handler(
                LoggingMessageCallback? messageCallback,
                IntPtr messageCallbackContext,
                LogLevel minLevel);
        }
    }
}
