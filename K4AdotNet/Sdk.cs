using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace K4AdotNet
{
    public static class Sdk
    {
        /// <summary>Name of Kinect for Azure Sensor SDK DLL.</summary>
        public const string SENSOR_DLL_NAME = "k4a";

        /// <summary>Name of record DLL in Kinect for Azure Sensor SDK.</summary>
        public const string RECORD_DLL_NAME = "k4arecord";

        /// <summary>Name of Kinect for Azure Body Tracking SDK DLL.</summary>
        public const string BODY_TRACKING_DLL_NAME = "k4abt";

        /// <summary>The Sensor SDK can log data to the console, files, or to a custom handler.</summary>
        /// <param name="level">Level of logging.</param>
        /// <param name="logToStdout">Log messages to STDOUT?</param>
        /// <param name="logToFile">
        /// Log all messages to the path and file specified.
        /// Must end in '.log' to be considered a valid entry.
        /// Use <see langword="null"/> or empty string to completely disable logging to a file.
        /// </param>
        public static void ConfigureLogging(TraceLevel level, bool logToStdout = false, string logToFile = null)
        {
            Environment.SetEnvironmentVariable("K4A_LOG_LEVEL", level.ToSdkLogLevelLetter(), EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("K4A_ENABLE_LOG_TO_STDOUT", logToStdout ? "1" : "0", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("K4A_ENABLE_LOG_TO_A_FILE", string.IsNullOrWhiteSpace(logToFile) ? "0" : logToFile, EnvironmentVariableTarget.Process);
        }

        /// <summary>Current version of Body Tracking SDK can log data only to the console.</summary>
        /// <param name="level">Level of logging.</param>
        public static void ConfigureBodyTrackingLogging(TraceLevel level)
        {
            Environment.SetEnvironmentVariable("K4ABT_LOG_LEVEL", level.ToSdkLogLevelLetter(), EnvironmentVariableTarget.Process);
#if VERSION_0_9_DOESNOT_SUPPORT_THE_FOLLOWING_SETTINGS
            // Setting to "0" crashes internal of Body Tracking SDK 0.9 (Memory Access Violation exception)
            Environment.SetEnvironmentVariable("K4ABT_ENABLE_LOG_TO_STDOUT", logToStdout ? "1" : "0", EnvironmentVariableTarget.Process);
            // Creates log file but doesn't write to it anything. Most likely, because of impossibility to turn off logging to STDOUT
            Environment.SetEnvironmentVariable("K4ABT_ENABLE_LOG_TO_A_FILE", string.IsNullOrWhiteSpace(logToFile) ? "0" : logToFile, EnvironmentVariableTarget.Process);
#endif
        }

        private static string ToSdkLogLevelLetter(this TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Off: return "c";
                case TraceLevel.Error: return "e";
                case TraceLevel.Warning: return "w";
                case TraceLevel.Info: return "i";
                case TraceLevel.Verbose: return "t";
                default: throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        public static bool CheckPrerequisitesForBodyTracking(out string message)
        {
            const string SEE_FOR_DETAILS = " See https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup for details.";

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                message = "Current version of Body Tracking supports only Windows.";
                return false;
            }

            if (!Environment.Is64BitProcess)
            {
                message = "Process must be 64-bit.";
                return false;
            }

            var cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH_V10_0");
            if (string.IsNullOrWhiteSpace(cudaPath))
                cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH");
            if (string.IsNullOrWhiteSpace(cudaPath)
                || cudaPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                || !Directory.Exists(cudaPath))
            {
                message = "CUDA 10.0 must be installed." + SEE_FOR_DETAILS;
                return false;
            }
            var cudaDir = new DirectoryInfo(cudaPath);

            var pathVariable = Environment.GetEnvironmentVariable("Path") ?? string.Empty;
            var pathVariableItems = pathVariable.Split(';');
            string cudaBinPath = null;
            var wasButWrongVersion = false;
            foreach (var item in pathVariableItems)
            {
                if (!string.IsNullOrWhiteSpace(item) && item.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    var itemDir = new DirectoryInfo(item);
                    if (IsSubdirOf(itemDir, cudaDir) && itemDir.Exists)
                    {
                        var cudaRuntimeDllPath = Path.Combine(itemDir.FullName, "cudart64_100.dll");
                        if (File.Exists(cudaRuntimeDllPath))
                        {
                            var ver = FileVersionInfo.GetVersionInfo(cudaRuntimeDllPath);
                            if (ver != null && ver.FileDescription != null && ver.FileDescription.Contains("Version 10.0."))
                            {
                                cudaBinPath = itemDir.FullName;
                                break;
                            }
                            else
                            {
                                wasButWrongVersion = true;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(cudaBinPath))
            {
                message = wasButWrongVersion
                    ? "Version 10.0 of CUDA is required. Different version found. Please install CUDA 10.0." + SEE_FOR_DETAILS
                    : "CUDA 10.0 is not installed or environment variable 'Path' does not contain binary directory of CUDA 10.0." + SEE_FOR_DETAILS;
                return false;
            }

            var cudnnPath = Path.Combine(cudaBinPath, "cudnn64_7.dll");
            if (!File.Exists(cudnnPath))
            {
                message = $"cudnn64_7.dll library is not found in CUDA binary directory {cudaBinPath}." + SEE_FOR_DETAILS;
                return false;
            }

            message = null;
            return true;
        }

        private static bool IsSubdirOf(DirectoryInfo subdir, DirectoryInfo parentDir)
        {
            for (; subdir != null; subdir = subdir.Parent)
                if (subdir.FullName.Equals(parentDir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
