using System;
using System.Diagnostics;

namespace K4AdotNet.Logging
{
    /// <summary>
    /// Internal helper class which implements logging-related logic.
    /// </summary>
    internal sealed class LogImpl
    {
        public static readonly LogImpl Azure = new(NativeApi.Azure.Instance);

        public static readonly LogImpl Orbbec = new(NativeApi.Orbbec.Instance);

        private readonly NativeApi nativeApi;
        private readonly NativeApi.LoggingMessageCallback debugMessageHandler = OnDebugMessage;
        private TraceLevel traceLevel = TraceLevel.Off;
        private readonly object traceLevelSync = new();

        private LogImpl(NativeApi nativeApi)
            => this.nativeApi = nativeApi;

        public TraceLevel TraceLevel
        {
            get
            {
                lock (traceLevelSync) return traceLevel;
            }

            set
            {
                lock (traceLevelSync)
                {
                    if (value == traceLevel)
                        return;

                    if (traceLevel != TraceLevel.Off)
                    {
                        var res = nativeApi.SetDebugMessageHandler(null, IntPtr.Zero, ToLogLevel(traceLevel));
                        if (res != NativeCallResults.Result.Succeeded)
                            throw new InvalidOperationException("Failed to clear the debug message handler.");
                    }

                    if (value != TraceLevel.Off)
                    {
                        var res = nativeApi.SetDebugMessageHandler(debugMessageHandler, IntPtr.Zero, ToLogLevel(value));
                        if (res != NativeCallResults.Result.Succeeded)
                            throw new InvalidOperationException("Failed to set the debug message handler.");

                        if (traceLevel == TraceLevel.Off)
                        {
                            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_Exit;
                            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_Exit;
                        }
                    }
                    else
                    {
                        AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_Exit;
                        AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_Exit;
                    }

                    traceLevel = value;
                }
            }
        }

        private static void OnDebugMessage(IntPtr _, LogLevel level, string file, int line, string message)
        {
            var text = $"#K4A [{System.IO.Path.GetFileName(file)}:{line}] {message}";

            switch (level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Trace.TraceError(text);
                    break;
                case LogLevel.Warning:
                    Trace.TraceWarning(text);
                    break;
                case LogLevel.Information:
                    Trace.TraceInformation(text);
                    break;
                default:
                    Trace.WriteLine(text);
                    break;
            }
        }

        private void CurrentDomain_Exit(object? sender, EventArgs e)
        {
            var res = nativeApi.SetDebugMessageHandler(null, IntPtr.Zero, ToLogLevel(traceLevel));
            if (res != NativeCallResults.Result.Succeeded)
                Trace.TraceWarning("Failed to clear the debug message handler");
        }

        private static LogLevel ToLogLevel(TraceLevel level)
            => level switch
            {
                TraceLevel.Off => LogLevel.Off,
                TraceLevel.Error => LogLevel.Error,
                TraceLevel.Warning => LogLevel.Warning,
                TraceLevel.Info => LogLevel.Information,
                TraceLevel.Verbose => LogLevel.Trace,
                _ => throw new ArgumentOutOfRangeException(nameof(level)),
            };

        public static void ConfigureLogging(string variableNamePrefix, TraceLevel level, bool logToStdout, string? logToFile)
        {
            Environment.SetEnvironmentVariable(variableNamePrefix + "LOG_LEVEL", ToSdkLogLevelLetter(level), EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(variableNamePrefix + "ENABLE_LOG_TO_STDOUT", logToStdout ? "1" : "0", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(variableNamePrefix + "ENABLE_LOG_TO_A_FILE", string.IsNullOrWhiteSpace(logToFile) ? "0" : logToFile, EnvironmentVariableTarget.Process);
        }

        private static string ToSdkLogLevelLetter(TraceLevel level)
            => level switch
            {
                TraceLevel.Off => "c",
                TraceLevel.Error => "e",
                TraceLevel.Warning => "w",
                TraceLevel.Info => "i",
                TraceLevel.Verbose => "t",
                _ => throw new ArgumentOutOfRangeException(nameof(level)),
            };
    }
}
