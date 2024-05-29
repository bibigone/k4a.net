using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.IO;

namespace K4AdotNet
{
    static partial class Sdk
    {
        /// <summary>
        /// Common basic things that are relevant only for `orgiginal K4A` implementation.
        /// </summary>
        /// <see cref="ComboMode"/>
        public static class Azure
        {
            /// <summary>
            /// Name of subdirectory with native libraries for `original K4A` implementation.
            /// </summary>
            public const string NATIVE_LIB_SUBDIR = "k4a-azure";

            /// <summary>
            /// Name of main library (DLL) from Azure Kinect Sensor SDK with subdirectory.
            /// </summary>
            /// <remarks>Is used for <c>DllImport</c>s.</remarks>
            public const string SENSOR_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.SENSOR_DLL_NAME;

            /// <summary>
            /// Name of record library (DLL) from Azure Kinect Sensor SDK with subdirectory.
            /// </summary>
            /// <remarks>Is used for <c>DllImport</c>s.</remarks>
            public const string RECORD_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.RECORD_DLL_NAME;

            private static volatile bool isInitialized;
            private static volatile bool isInitializing;
            private static readonly object initSync = new();

            /// <summary>
            /// Implementation of <see cref="Sdk.TraceLevel"/> property
            /// for `original K4A` library.
            /// </summary>
            /// <seealso cref="Sdk.TraceLevel"/>
            /// <seealso cref="Orbbec.TraceLevel"/>
            public static TraceLevel TraceLevel
            {
                get
                {
                    CheckEnabled();
                    return Logging.LogImpl.Azure.TraceLevel;
                }

                set
                {
                    CheckEnabled();
                    Logging.LogImpl.Azure.TraceLevel = value;
                }
            }

            /// <summary>
            /// Checks that Azure implementation is enabled (initialized).
            /// </summary>
            /// <exception cref="InvalidOperationException">Azure implementation is not available in the current mode.</exception>
            /// <seealso cref="Sdk.Init(ComboMode)"/>
            /// <seealso cref="ComboMode"/>
            internal static void CheckEnabled()
            {
                if (isInitializing || (ComboMode & ComboMode.Azure) == ComboMode.Azure)
                    return;
                throw new InvalidOperationException($"This functionality requires SDK initialization in {nameof(ComboMode)}.{nameof(ComboMode.Azure)} or {nameof(ComboMode.Both)}.");
            }

            /// <summary>
            /// Initializes Azure implementation and forces native library loading of `original K4A`.
            /// </summary>
            /// <seealso cref="Sdk.Init(ComboMode)"/>
            internal static void Init()
            {
                lock (initSync)
                {
                    if (isInitialized) return;

                    var dirBak = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(Helpers.GetFullPathToSubdir(NATIVE_LIB_SUBDIR));
                    isInitializing = true;
                    try
                    {
                        // Force k4a.dll and depthengine2_0.dll loading
                        var depthMode = DepthMode.NarrowViewUnbinned;
                        CalibrationData.CreateDummy(depthMode, ColorResolution.Off, false, out var calibration);
                        using var tranform = new Transformation.Azure(in calibration);
                        using var depthImage = new Image.Azure(ImageFormat.Depth16, depthMode.WidthPixels(), depthMode.HeightPixels());
                        using var xyzImage = new Image.Azure(ImageFormat.Custom, depthMode.WidthPixels(), depthMode.HeightPixels(), depthMode.WidthPixels() * 6);
                        tranform.DepthImageToPointCloud(depthImage, CalibrationGeometry.Depth, xyzImage);

                        // Set TraceLevel to default value
                        TraceLevel = TraceLevel.Off;

                        // Done
                        isInitialized = true;
                    }
                    finally
                    {
                        isInitializing = false;
                        Directory.SetCurrentDirectory(dirBak);
                    }
                }
            }
        }
    }
}
