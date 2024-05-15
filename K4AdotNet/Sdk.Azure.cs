using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.IO;

namespace K4AdotNet
{
    static partial class Sdk
    {
        public static class Azure
        {
            public const string NATIVE_LIB_SUBDIR = "k4a-azure";

            public const string SENSOR_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.SENSOR_DLL_NAME;

            public const string RECORD_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.RECORD_DLL_NAME;

            private static volatile bool isInitialized;
            private static volatile bool isInitializing;
            private static readonly object initSync = new();

            /// <summary>
            /// The K4A.Net can log data to a regular .Net Trace.
            /// Use this property to choose the level of such logging, or set it to <see cref="TraceLevel.Off"/> to turn it off.
            /// Default value is <see cref="TraceLevel.Off"/>.
            /// </summary>
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

            internal static void CheckEnabled()
            {
                if (isInitializing || (ComboMode & ComboMode.Azure) == ComboMode.Azure)
                    return;
                throw new InvalidOperationException($"This functionality requires SDK initialization in {nameof(ComboMode)}.{nameof(ComboMode.Azure)} or {nameof(ComboMode.Both)}.");
            }

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
