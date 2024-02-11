using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

[assembly: CLSCompliant(isCompliant: true)]

namespace K4AdotNet
{
    /// <summary>Static class with common basic things for Sensor, Record and Body Tracking APIs like logging, initializing, loading of dependencies, etc.</summary>
    public static class Sdk
    {
        /// <summary>Is underlying level the OrbbecSDK K4A Wrapper rather than original Kinect for Azure SDK?</summary>
        /// <remarks>
        /// K4A.Net library can be compiled in version that supports Orbbec Femto depth sensors via Orbbec SDK K4A Wrapper.
        /// There is no full compatibility between original Kinect for Azure SDK and OrbbecSDK-K4A-Wrapper.
        /// To distinguish between them in client code, one can use this property.
        /// </remarks>
        public static bool IsOrbbecSdkK4AWrapper
#if ORBBECSDK_K4A_WRAPPER
            => true;
#else
            => false;
#endif

        #region Dependencies

        /// <summary>Name of main library (DLL) from Azure Kinect Sensor SDK.</summary>
        /// <remarks>This library is required for the most of API including Record and Body Tracking parts.</remarks>
        public const string SENSOR_DLL_NAME = "k4a";

        /// <summary>Name of depth engine library (DLL) from Azure Kinect Sensor SDK.</summary>
        /// <remarks>This library is required for <see cref="Sensor.Device.StartCameras"/> and <see cref="Sensor.Transformation.Transformation"/>.</remarks>
        public const string DEPTHENGINE_DLL_NAME = "depthengine_2_0";

        /// <summary>Name of record library (DLL) from Azure Kinect Sensor SDK.</summary>
        /// <remarks>This library is required for Record part of API (see <c>K4AdotNet.Record</c> namespace).</remarks>
        public const string RECORD_DLL_NAME = "k4arecord";

        /// <summary>Name of body tracking library (DLL) from Azure Kinect Body Tracking SDK.</summary>
        /// <remarks>This library is required for Body Tracking part of API (see <c>K4AdotNet.BodyTracking</c> namespace).</remarks>
        public const string BODY_TRACKING_DLL_NAME = "k4abt";

        /// <summary>Expected major version of Body Tracking runtime. This version of K4AdotNet assembly is built and tested against this version of Body Tracking.</summary>
        /// <seealso cref="BODY_TRACKING_EXPECTED_VERSION_MINOR"/>
        public static readonly int BODY_TRACKING_EXPECTED_VERSION_MAJOR = 1;

        /// <summary>Expected minor version of Body Tracking runtime. This version of K4AdotNet assembly is built and tested against this version of Body Tracking.</summary>
        /// <seealso cref="BODY_TRACKING_EXPECTED_VERSION_MAJOR"/>
        public static readonly int BODY_TRACKING_EXPECTED_VERSION_MINOR = 1;

        /// <summary>Name of ONNX runtime library (DLL) which is used by <see cref="BODY_TRACKING_DLL_NAME"/>.</summary>
        /// <remarks>This library is required for Body Tracking part of API (see <c>K4AdotNet.BodyTracking</c> namespace).</remarks>
        public const string ONNX_RUNTIME_DLL_NAME = "onnxruntime";

        /// <summary>Name of ONNX file with model of neural network used by <see cref="BODY_TRACKING_DLL_NAME"/>.</summary>
        /// <remarks>This data file is required for Body Tracking part of API (see <c>K4AdotNet.BodyTracking</c> namespace).</remarks>
        public const string BODY_TRACKING_DNN_MODEL_FILE_NAME = "dnn_model_2_0_op11.onnx";

        /// <summary>Name of ONNX file with model of lite neural network used by <see cref="BODY_TRACKING_DLL_NAME"/>.</summary>
        /// <remarks>This data file is required for Body Tracking part of API (see <c>K4AdotNet.BodyTracking</c> namespace).</remarks>
        public const string BODY_TRACKING_DNN_MODEL_LITE_FILE_NAME = "dnn_model_2_0_lite_op11.onnx";

        /// <summary>ONNX runtime depends on NVIDIA cuDNN library. This list contains all required components for cuDNN under Windows.</summary>
        public static readonly IReadOnlyList<string> CUDNN_DLL_NAMES = new[]
        {
            "cublas64_11.dll",
            "cublasLt64_11.dll",
            "cudart64_110.dll",
            "cudnn_cnn_infer64_8.dll",
            "cudnn_ops_infer64_8.dll",
            "cudnn64_8.dll",
            "cufft64_10.dll",
        }.ToList().AsReadOnly();

        /// <summary>Extension of Dynamic Link Libraries (DLL) under Windows.</summary>
        private const string DLL_EXTENSION_WIN = ".dll";

        #endregion

        #region Logging

        /// <summary>
        /// The K4A.Net can log data to a regular .Net Trace.
        /// Use this property to choose the level of such logging, or set it to <see cref="TraceLevel.Off"/> to turn it off.
        /// Default value is <see cref="TraceLevel.Off"/>.
        /// </summary>
        public static TraceLevel TraceLevel
        {
            get => Logging.LogImpl.TraceLevel;
            set => Logging.LogImpl.TraceLevel = value;
        }

        /// <summary>The Sensor SDK can log data to the console, files, or to a custom handler.</summary>
        /// <param name="level">Level of logging.</param>
        /// <param name="logToStdout">Log messages to STDOUT?</param>
        /// <param name="logToFile">
        /// Log all messages to the path and file specified.
        /// Must end in '.log' to be considered a valid entry.
        /// Use <see langword="null"/> or empty string to completely disable logging to a file.
        /// </param>
        /// <remarks><para>
        /// Call this method before any usage of Sensor API (classes from <c>K4AdotNet.Sensor</c> namespace).
        /// </para><para>
        /// Don't use one and the same destination log file for different parts of API (Sensor, Record, Body Tracking).
        /// Each part uses separate logger instance that do not allowed shared access to the file being written to.
        /// </para></remarks>
        public static void ConfigureLogging(TraceLevel level, bool logToStdout = false, string? logToFile = null)
        {
            const string PREFIX = "K4A_";
            Logging.LogImpl.ConfigureLogging(PREFIX, level, logToStdout, logToFile);
        }

        /// <summary>Record part of Sensor SDK can log data to the console, files, or to a custom handler.</summary>
        /// <param name="level">Level of logging.</param>
        /// <param name="logToStdout">Log messages to STDOUT?</param>
        /// <param name="logToFile">
        /// Log all messages to the path and file specified.
        /// Must end in '.log' to be considered a valid entry.
        /// Use <see langword="null"/> or empty string to completely disable logging to a file.
        /// </param>
        /// <remarks><para>
        /// Call this method before any usage of Record API (classes from <c>K4AdotNet.Record</c> namespace).
        /// </para><para>
        /// Don't use one and the same destination log file for different parts of API (Sensor, Record, Body Tracking).
        /// Each part uses separate logger instance that do not allowed shared access to the file being written to.
        /// </para></remarks>
        public static void ConfigureRecordLogging(TraceLevel level, bool logToStdout = false, string? logToFile = null)
        {
            const string PREFIX = "K4A_RECORD_";
            Logging.LogImpl.ConfigureLogging(PREFIX, level, logToStdout, logToFile);
        }

        /// <summary>The Body Tracking SDK can log data to the console, files, or to a custom handler.</summary>
        /// <param name="level">Level of logging.</param>
        /// <param name="logToStdout">Log messages to STDOUT?</param>
        /// <param name="logToFile">
        /// Log all messages to the path and file specified.
        /// Must end in '.log' to be considered a valid entry.
        /// Use <see langword="null"/> or empty string to completely disable logging to a file.
        /// </param>
        /// <remarks><para>
        /// Call this method before any usage of Body Tracking API (classes from <c>K4AdotNet.BodyTracking</c> namespace).
        /// </para><para>
        /// Don't use one and the same destination log file for different parts of API (Sensor, Record, Body Tracking).
        /// Each part uses separate logger instance that do not allowed shared access to the file being written to.
        /// </para></remarks>
        public static void ConfigureBodyTrackingLogging(TraceLevel level, bool logToStdout = false, string? logToFile = null)
        {
            const string PREFIX = "K4ABT_";
            Logging.LogImpl.ConfigureLogging(PREFIX, level, logToStdout, logToFile);
        }

        #endregion

        #region Custom memory allocation

        // to keep callbacks alive
        private static readonly ConcurrentBag<Sensor.NativeApi.MemoryAllocateCallback> allocateCallbacks = new();
        private static readonly ConcurrentBag<Sensor.NativeApi.MemoryDestroyCallback> destroyCallbacks = new();

        /// <summary>
        /// Sets or clears custom memory allocator to be use by internals of Azure Kinect SDK.
        /// </summary>
        /// <param name="allocator">
        /// Instance of <see cref="ICustomMemoryAllocator"/> to allocate and free memory in internals of Azure Kinect SDK
        /// or <see langword="null"/> to "clear" custom memory allocator (that is, to switch to standard built-in SDK's allocator).
        /// </param>
        /// <exception cref="InvalidOperationException">Setting or clearing of custom memory allocator was failed. See log for details.</exception>
        /// <remarks>
        /// All instances of <paramref name="allocator"/> will be keeping alive forever because they can be used
        /// to free memory even after setting custom allocator to <see langword="null"/>.
        /// </remarks>
#if ORBBECSDK_K4A_WRAPPER
        [Obsolete("Not supported by OrbbecSDK-K4A-Wrapper")]
#endif
        public static void SetCustomMemoryAllocator(ICustomMemoryAllocator? allocator)
        {
            if (allocator is null)
            {
                var res = Sensor.NativeApi.SetAllocator(null, null);
                if (res != NativeCallResults.Result.Succeeded)
                    throw new InvalidOperationException("Cannot clear custom memory allocator");
                return;
            }

            var allocateCallback = new Sensor.NativeApi.MemoryAllocateCallback(allocator.Allocate);
            var destroyCallback = new Sensor.NativeApi.MemoryDestroyCallback(allocator.Free);

            // to keep callbacks alive
            allocateCallbacks.Add(allocateCallback);
            destroyCallbacks.Add(destroyCallback);

            var rs = Sensor.NativeApi.SetAllocator(allocateCallback, destroyCallback);
            if (rs != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException("Cannot set custom memory allocator");
        }

        #endregion

        #region Body tracking SDK availability and initialization

        /// <summary>URL to step-by-step instruction "How to set up Body Tracking SDK". Helpful for UI and user messages.</summary>
        public static readonly string BodyTrackingSdkInstallationGuideUrl
            = "https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup";

        /// <summary>
        /// Path to runtime with body tracking libraries and DNN-models.
        /// <see langword="null"/> on get means that body tracking runtime cannot be found.
        /// <see langword="null"/> on set means "please, try to find body tracking runtime automatically.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Specified path does not contain body tracking runtime.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Value of this property cannot be changed after successful initialization of Body tracking runtime.
        /// </exception>
        /// <seealso cref="IsBodyTrackingRuntimeAvailable(out string?)"/>
        /// <seealso cref="TryInitializeBodyTrackingRuntime(BodyTracking.TrackerProcessingMode, out string?)"/>
        public static string? BodyTrackingRuntimePath
        {
            get
            {
                lock (bodyTrackingRuntimeInitializationSync)
                {
                    if (bodyTrackingRuntimePath == null)
                        TryGetBodyTrackingRuntimePath(out bodyTrackingRuntimePath, out _);
                    return bodyTrackingRuntimePath;
                }
            }

            set
            {
                lock (bodyTrackingRuntimeInitializationSync)
                {
                    if (value == bodyTrackingRuntimePath)
                        return;
                    if (bodyTrackingRuntimeInitialized)
                        throw new InvalidOperationException($"{nameof(BodyTrackingRuntimePath)} cannot be changed after successful initialization of Body tracking runtime.");
                    if (!string.IsNullOrEmpty(value) && !ProbePathForBodyTrackingRuntime(value))
                        throw new FileNotFoundException("Body tracking runtime cannot be found in the path specified: " + value);
                    bodyTrackingRuntimePath = value;
                }
            }
        }

        /// <summary>
        /// Determines version of body tracking runtime in use
        /// or returns <see langword="null"/> in case when body tracking runtime cannot be found.
        /// </summary>
        /// <seealso cref="BodyTrackingRuntimePath"/>
        public static Version? BodyTrackingRuntimeVersion
        {
            get
            {
                var path = BodyTrackingRuntimePath;
                if (string.IsNullOrEmpty(path))
                    return null;
                return GetBodyTrackingRuntimeVersion(path);
            }
        }

        /// <summary>Checks that Body Tracking runtime is available.</summary>
        /// <param name="message">
        /// Detailed information about troubles with Body Tracking runtime if method returns <see langword="false"/>,
        /// or <see langword="null"/> if method returns <see langword="true"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if Body Tracking runtime is available and can be used, in this case <paramref name="message"/> is <see langword="null"/>,
        /// <see langword="false"/> if Body Tracking runtime is not available for some reason, in this case <paramref name="message"/> contains user-friendly description of this reason.
        /// </returns>
        /// <remarks><para>
        /// This method tries to find Body Tracking runtime in one of the following locations:
        /// directory specified in <see cref="BodyTrackingRuntimePath"/> property (if not <see langword="null"/>),
        /// directory with executable file,
        /// directory with <c>K4AdotNet</c> assembly,
        /// installation directory of Body Tracking SDK under <c>Program Files</c>.
        /// </para></remarks>
        /// <seealso cref="BodyTrackingRuntimePath"/>
        /// <seealso cref="TryInitializeBodyTrackingRuntime(BodyTracking.TrackerProcessingMode, out string)"/>
        public static bool IsBodyTrackingRuntimeAvailable([NotNullWhen(returnValue: false)] out string? message)
        {
            if (bodyTrackingRuntimeInitialized)
            {
                message = null;
                return true;
            }

            if (!IsOSCompatibleWithBodyTracking(out message))
                return false;

            if (!TryGetBodyTrackingRuntimePath(out var bodyTrackingRuntimePath, out message))
                return false;

            if (!CheckBodyTrackingRuntimeVersion(bodyTrackingRuntimePath, out message))
                return false;

            if (!IsCudnnAvailableForBodyTracking(bodyTrackingRuntimePath, out message))
                return false;

            message = null;
            return true;
        }

        /// <summary>Call this method to initialization of Body Tracking runtime.</summary>
        /// <param name="mode">
        /// Processing mode for which Body Tracking runtime should be initialized.
        /// </param>
        /// <param name="message">
        /// If Body Tracking runtime was initialized successfully, this parameter is <see langword="null"/>,
        /// otherwise it contains user-friendly description of failure reason.
        /// </param>
        /// <returns>
        /// <see langword="true"/> - if Body Tracking runtime was initialized successfully (in this case <paramref name="message"/> is <see langword="null"/>),
        /// <see langword="false"/> - otherwise and in this case <paramref name="message"/> contains user-friendly description of failure.
        /// </returns>
        /// <remarks><para>
        /// It is rather time consuming operation: initialization of ONNX runtime, loading and parsing of neural network model, etc.
        /// For this reason, it is recommended to initialize Body Tracking runtime in advance and show some progress window for user.
        /// But this initialization is optional. If it wasn't called explicitly, it will be called implicitly during first construction of
        /// <see cref="BodyTracking.Tracker"/> object.
        /// </para><para>
        /// This method tries to find Body Tracking runtime in one of the following locations:
        /// directory <see cref="BodyTrackingRuntimePath"/> if specified,
        /// directory with executable file,
        /// directory with <c>K4AdotNet</c> assembly,
        /// installation directory of Body Tracking SDK under <c>Program Files</c>.
        /// </para></remarks>
        /// <seealso cref="IsBodyTrackingRuntimeAvailable(out string)"/>
        /// <seealso cref="BodyTracking.Tracker.Tracker"/>
        public static bool TryInitializeBodyTrackingRuntime(BodyTracking.TrackerProcessingMode mode, [NotNullWhen(returnValue: false)] out string? message)
        {
            Sensor.Calibration.CreateDummy(Sensor.DepthMode.NarrowView2x2Binned, Sensor.ColorResolution.Off, out var calibration);

            var config = BodyTracking.TrackerConfiguration.Default;
            config.ProcessingMode = mode;

            if (!TryCreateTrackerHandle(in calibration, config, out var trackerHandle, out message))
            {
                return false;
            }

            trackerHandle.Release();
            message = null;
            return true;
        }

        private static Version? GetBodyTrackingRuntimeVersion(string bodyTrackingRuntimePath)
        {
            var path = Path.Combine(bodyTrackingRuntimePath, BODY_TRACKING_DLL_NAME + DLL_EXTENSION_WIN);
            if (File.Exists(path))
            {
                var fvi = FileVersionInfo.GetVersionInfo(path);
                // Do not use FileVersion property as it's not populated under Unity .NET runtime
                if (fvi != null)
                    return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }

            return null;
        }

        private static bool CheckBodyTrackingRuntimeVersion(string bodyTrackingRuntimePath, [NotNullWhen(returnValue: false)] out string? message)
        {
            var version = GetBodyTrackingRuntimeVersion(bodyTrackingRuntimePath);
            // NB! Ignore Build part: we assume back compatibility withing one and the same major and minor versions
            if (version != null
                && version.Major == BODY_TRACKING_EXPECTED_VERSION_MAJOR
                && version.Minor == BODY_TRACKING_EXPECTED_VERSION_MINOR)
            {
                message = null;
                return true;
            }

            message = $"Version {BODY_TRACKING_EXPECTED_VERSION_MAJOR}.{BODY_TRACKING_EXPECTED_VERSION_MINOR}.x of Body Tracking runtime is expected.";
            return false;
        }

        private static bool TryGetBodyTrackingRuntimePath(
            [NotNullWhen(returnValue: true)] out string? path,
            [NotNullWhen(returnValue: false)] out string? message)
        {
            const string BODY_TRACKING_SDK_BIN_PATH = @"Azure Kinect Body Tracking SDK\tools";

            message = null;

            // Try bodyTrackingRuntimePath if specified
            lock (bodyTrackingRuntimeInitializationSync)
            {
                if (!string.IsNullOrEmpty(bodyTrackingRuntimePath) && ProbePathForBodyTrackingRuntime(bodyTrackingRuntimePath))
                {
                    path = bodyTrackingRuntimePath;
                    return true;
                }
            }

            // Try current directory
            var currentDir = Path.GetFullPath(Environment.CurrentDirectory);
            if (ProbePathForBodyTrackingRuntime(currentDir))
            {
                path = currentDir;
                return true;
            }

            // Try base directory of current app domain
            var baseDir = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            if (!baseDir.Equals(currentDir, StringComparison.InvariantCultureIgnoreCase))
            {
                if (ProbePathForBodyTrackingRuntime(baseDir))
                {
                    path = baseDir;
                    return true;
                }
            }

#if NET461 || NETSTANDARD2_0
            // Try location of this assembly
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmDir = Path.GetFullPath(Path.GetDirectoryName(new Uri(asm.GetName().CodeBase!).LocalPath)!);
            if (!asmDir.Equals(currentDir, StringComparison.InvariantCultureIgnoreCase)
                && !asmDir.Equals(baseDir, StringComparison.InvariantCultureIgnoreCase))
            {
                if (ProbePathForBodyTrackingRuntime(asmDir))
                {
                    path = asmDir;
                    return true;
                }
            }
#endif

            // Try standard location of installed Body Tracking SDK
            var sdkBinDir = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), BODY_TRACKING_SDK_BIN_PATH));
            if (ProbePathForBodyTrackingRuntime(sdkBinDir))
            {
                path = sdkBinDir;
                return true;
            }

            message = $"Cannot find Body Tracking {BODY_TRACKING_EXPECTED_VERSION_MAJOR}.{BODY_TRACKING_EXPECTED_VERSION_MINOR}.x runtime or some of its components (neither in application directory, nor in Body Tracking SDK directory).";
            path = null;
            return false;
        }

        private static bool IsOSCompatibleWithBodyTracking([NotNullWhen(returnValue: false)] out string? message)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT
                || Environment.OSVersion.Version < new Version(6, 2)
                || !Environment.Is64BitOperatingSystem)
            {
                message = "Current version of Body Tracking supports only 64-bit Windows 8.1/10.";
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

        private static bool IsCudnnAvailableForBodyTracking(string bodyTrackingRuntimePath, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (ProbePathForCudnn(bodyTrackingRuntimePath))
            {
                message = null;
                return true;
            }

            var cudaBinPath = GetCudaBinPath();
            if (!string.IsNullOrEmpty(cudaBinPath) && ProbePathForCudnn(cudaBinPath))
            {
                message = null;
                return true;
            }

            message = $"Body tracking uses ONNX runtime ({ONNX_RUNTIME_DLL_NAME}), which in turn depends on NVIDIA cuDNN v7.5.x and CUDA 10.0 libraries: {string.Join(", ", CUDNN_DLL_NAMES)}.";
            return false;
        }

        private static bool ProbePathForCudnn(string path)
        {
            if (!Directory.Exists(path))
                return false;

            foreach (var dllName in CUDNN_DLL_NAMES)
            {
                var fullPath = Path.Combine(path, dllName + DLL_EXTENSION_WIN);
                if (!File.Exists(fullPath))
                    return false;
            }

            return true;
        }

        private static string? GetCudaBinPath()
        {
            const string CUDA_PATH_VARIABLE_NAME = "CUDA_PATH";
            const string CUDA_10_0_PATH_VARIABLE_NAME = "CUDA_PATH_V10_0";
            const string PATH_VARIABLE_NAME = "Path";
            const char PATH_ITEMS_SEPARATOR = ';';

            var cudaPath = Environment.GetEnvironmentVariable(CUDA_10_0_PATH_VARIABLE_NAME);
            if (string.IsNullOrWhiteSpace(cudaPath))
                cudaPath = Environment.GetEnvironmentVariable(CUDA_PATH_VARIABLE_NAME);
            if (string.IsNullOrWhiteSpace(cudaPath)
                || cudaPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                || !Directory.Exists(cudaPath))
            {
                return null;
            }
            var cudaDir = new DirectoryInfo(cudaPath);

            var pathVariable = Environment.GetEnvironmentVariable(PATH_VARIABLE_NAME) ?? string.Empty;
            var pathVariableItems = pathVariable.Split(PATH_ITEMS_SEPARATOR);
            foreach (var item in pathVariableItems)
            {
                if (!string.IsNullOrWhiteSpace(item) && item.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    var itemDir = new DirectoryInfo(item);
                    if (Helpers.IsSubdirOf(itemDir, cudaDir) && itemDir.Exists)
                    {
                        return itemDir.FullName;
                    }
                }
            }

            return null;
        }

        private static bool ProbePathForBodyTrackingRuntime(string path)
        {
            if (!Directory.Exists(path))
                return false;

            var k4abt = Path.Combine(path, BODY_TRACKING_DLL_NAME + DLL_EXTENSION_WIN);
            if (!File.Exists(k4abt))
                return false;

            var onnxruntime = Path.Combine(path, ONNX_RUNTIME_DLL_NAME + DLL_EXTENSION_WIN);
            if (!File.Exists(onnxruntime))
                return false;

            return true;
        }

        internal static bool TryCreateTrackerHandle(in Sensor.Calibration calibration, BodyTracking.TrackerConfiguration config,
            [NotNullWhen(returnValue: true)] out NativeHandles.TrackerHandle trackerHandle,
            [NotNullWhen(returnValue: false)] out string? message)
        {
            string runtimePath;
            bool alreadyInitialized;

            lock (bodyTrackingRuntimeInitializationSync)
            {
                if (string.IsNullOrEmpty(bodyTrackingRuntimePath))
                {
                    if (!TryGetBodyTrackingRuntimePath(out bodyTrackingRuntimePath, out message))
                    {
                        trackerHandle = default;
                        return false;
                    }
                }

                runtimePath = bodyTrackingRuntimePath;
                alreadyInitialized = bodyTrackingRuntimeInitialized;
            }

            if (!alreadyInitialized)
            {
                // Check version of Body tracking runtime
                if (!CheckBodyTrackingRuntimeVersion(bodyTrackingRuntimePath, out message))
                {
                    trackerHandle = default;
                    return false;
                }

                // Force loading of k4a.dll,
                // because k4abt.dll depends on it
                var tmp = new Sensor.Capture();
                tmp.Dispose();

                // Append directory with Body Tracking Runtime to PATH environment variable
                const string PATH_ENV_NAME = "PATH";
                var pathEnvVar = Environment.GetEnvironmentVariable(PATH_ENV_NAME, EnvironmentVariableTarget.Process);
                if ((";" + pathEnvVar + ";").IndexOf(";" + bodyTrackingRuntimePath + ";", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    if (!string.IsNullOrWhiteSpace(pathEnvVar) && !pathEnvVar.TrimStart().StartsWith(";", StringComparison.OrdinalIgnoreCase))
                        pathEnvVar = ";" + pathEnvVar;
                    pathEnvVar = bodyTrackingRuntimePath + pathEnvVar;
                    Environment.SetEnvironmentVariable(PATH_ENV_NAME, pathEnvVar, EnvironmentVariableTarget.Process);
                }
            }

            // Default tracking model
            if (string.IsNullOrEmpty(config.ModelPath))
                config.ModelPath = BODY_TRACKING_DNN_MODEL_FILE_NAME;

            // If path is not fully qualified and model file does not exist, try runtime path
            if (!File.Exists(config.ModelPath) && !Path.IsPathRooted(config.ModelPath))
            {
                var fullPath = Path.Combine(runtimePath, config.ModelPath);
                if (File.Exists(fullPath) && Helpers.IsAsciiCompatible(fullPath))
                    config.ModelPath = fullPath;
            }

            // Try to create body tracker instance
            if (BodyTracking.NativeApi.TrackerCreate(in calibration, config, out trackerHandle) != NativeCallResults.Result.Succeeded
                || !trackerHandle.IsValid)
            {
                // Why we failed?
                if (IsBodyTrackingRuntimeAvailable(out message))
                {
                    message = File.Exists(config.ModelPath) && Helpers.IsAsciiCompatible(config.ModelPath)
                        ? "Cannot initialize body tracking runtime. See logs for details."
                        : ("Cannot find ONNX-model: " + config.ModelPath);
                }
                return false;
            }

            // Set the flag that body tracking already initialized
            lock (bodyTrackingRuntimeInitializationSync)
            {
                bodyTrackingRuntimeInitialized = true;
            }

            message = null;
            return true;
        }

        private static volatile bool bodyTrackingRuntimeInitialized;
        private static string? bodyTrackingRuntimePath;
        private static readonly object bodyTrackingRuntimeInitializationSync = new();

        #endregion
    }
}
