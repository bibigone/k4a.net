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

        public static readonly string BodyTrackingSdkInstallationGuideUrl = "https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup";

        public static bool CheckPrerequisitesForBodyTracking(out string message)
        {
            if (!CheckOSForBodyTracking(out message))
                return false;

            var cudaBinPath = CheckCudaForBodyTracking(out message);
            if (string.IsNullOrEmpty(cudaBinPath))
                return false;

            if (!CheckCudnnForBodyTracking(cudaBinPath, out message))
                return false;

            message = null;
            return true;
        }

        private static bool CheckOSForBodyTracking(out string message)
        {
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

            message = null;
            return true;
        }

        private static string CheckCudaForBodyTracking(out string message)
        {
            const string CUDA_VERSION = "10.0";
            const string CUDA_PATH_VARIABLE_NAME = "CUDA_PATH";
            const string CUDA_10_0_PATH_VARIABLE_NAME = "CUDA_PATH_V10_0";
            const string PATH_VARIABLE_NAME = "Path";
            const string CUDA_RUNTIME_DLL = "cudart64_100.dll";

            var cudaPath = Environment.GetEnvironmentVariable(CUDA_10_0_PATH_VARIABLE_NAME);
            if (string.IsNullOrWhiteSpace(cudaPath))
                cudaPath = Environment.GetEnvironmentVariable(CUDA_PATH_VARIABLE_NAME);
            if (string.IsNullOrWhiteSpace(cudaPath)
                || cudaPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                || !Directory.Exists(cudaPath))
            {
                message = $"CUDA {CUDA_VERSION} must be installed.";
                return null;
            }
            var cudaDir = new DirectoryInfo(cudaPath);

            var pathVariable = Environment.GetEnvironmentVariable(PATH_VARIABLE_NAME) ?? string.Empty;
            var pathVariableItems = pathVariable.Split(';');
            string cudaBinPath = null;
            var wasButWrongVersion = false;
            foreach (var item in pathVariableItems)
            {
                if (!string.IsNullOrWhiteSpace(item) && item.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    var itemDir = new DirectoryInfo(item);
                    if (Helpers.IsSubdirOf(itemDir, cudaDir) && itemDir.Exists)
                    {
                        var cudaRuntimeDllPath = Path.Combine(itemDir.FullName, CUDA_RUNTIME_DLL);
                        if (File.Exists(cudaRuntimeDllPath))
                        {
                            var ver = FileVersionInfo.GetVersionInfo(cudaRuntimeDllPath);
                            if (ver != null && ver.FileDescription != null && ver.FileDescription.Contains($"Version {CUDA_VERSION}."))
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
                    ? $"Version {CUDA_VERSION} of CUDA is required. Different version found. Please install CUDA {CUDA_VERSION}."
                    : $"CUDA {CUDA_VERSION} is not installed or environment variable {PATH_VARIABLE_NAME} does not contain binary directory of CUDA {CUDA_VERSION}.";
                return null;
            }

            message = null;
            return cudaBinPath;
        }

        private static bool CheckCudnnForBodyTracking(string cudaBinPath, out string message)
        {
            const string CUDNN_DLL = "cudnn64_7.dll";

            var cudnnPath = Path.Combine(cudaBinPath, CUDNN_DLL);
            if (!File.Exists(cudnnPath))
            {
                message = $"{CUDNN_DLL} library is not found in CUDA binary directory \"{cudaBinPath}\".";
                return false;
            }

            message = null;
            return true;
        }
    }
}
