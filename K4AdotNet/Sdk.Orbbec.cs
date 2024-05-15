using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.IO;

namespace K4AdotNet
{
    public static partial class Sdk
    {
        public static class Orbbec
        {
            public const string NATIVE_LIB_SUBDIR = "k4a-orbbec";

            public const string SENSOR_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.SENSOR_DLL_NAME;

            public const string RECORD_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.RECORD_DLL_NAME;

            private static DepthEngineHelper? depthEngineHelper;
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
                    return Logging.LogImpl.Orbbec.TraceLevel;
                }

                set
                {
                    CheckEnabled();
                    Logging.LogImpl.Orbbec.TraceLevel = value;
                }
            }

            internal static void CheckEnabled()
            {
                if (isInitializing || (ComboMode & ComboMode.Orbbec) == ComboMode.Orbbec)
                    return;
                throw new InvalidOperationException($"This functionality requires SDK initialization in {nameof(ComboMode)}.{nameof(ComboMode.Orbbec)} or {nameof(ComboMode.Both)}.");
            }

            internal static void Init()
            {
                lock (initSync)
                {
                    // Already initialized?
                    if (depthEngineHelper is not null)
                        return;

                    var dirBak = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(Helpers.GetFullPathToSubdir(NATIVE_LIB_SUBDIR));
                    isInitializing = true;
                    try
                    {
                        // Force depthengine_2_0.dll loading and initialization
                        depthEngineHelper = DepthEngineHelper.Create();

                        // Force k4a.dll loading and initialization
                        NativeApi.Orbbec.Instance.DeviceGetInstalledCount();

                        // Set TraceLevel to default value
                        TraceLevel = TraceLevel.Off;
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
