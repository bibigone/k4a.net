using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.IO;

namespace K4AdotNet
{
    public static partial class Sdk
    {
        /// <summary>
        /// Common basic things that are relevant only for `Orbbec SDK K4A Wrapper` implementation.
        /// </summary>
        /// <see cref="ComboMode"/>
        public static class Orbbec
        {
            /// <summary>
            /// Name of subdirectory with native libraries for `Orbbec SDK K4A Wrapper` implementation.
            /// </summary>
            public const string NATIVE_LIB_SUBDIR = "k4a-orbbec";

            /// <summary>
            /// Name of main library (DLL) from Orbbec SDK K4A Wrapper with subdirectory.
            /// </summary>
            /// <remarks>Is used for <c>DllImport</c>s.</remarks>
            public const string SENSOR_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.SENSOR_DLL_NAME;

            /// <summary>
            /// Name of record library (DLL) from Orbbec SDK K4A Wrapper with subdirectory.
            /// </summary>
            /// <remarks>Is used for <c>DllImport</c>s.</remarks>
            public const string RECORD_DLL_NAME = NATIVE_LIB_SUBDIR + "/" + Sdk.RECORD_DLL_NAME;

            private static DepthEngineHelper? depthEngineHelper;
            private static volatile bool isInitializing;
            private static readonly object initSync = new();

            /// <summary>
            /// Implementation of <see cref="Sdk.TraceLevel"/> property
            /// for `Orbbec SDK K4A Wrapper` library.
            /// </summary>
            /// <seealso cref="Sdk.TraceLevel"/>
            /// <seealso cref="Azure.TraceLevel"/>
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

            /// <summary>
            /// Checks that Orbbec implementation is enabled (initialized).
            /// </summary>
            /// <exception cref="InvalidOperationException">Orbbec implementation is not available in the current mode.</exception>
            /// <seealso cref="Sdk.Init(ComboMode)"/>
            /// <seealso cref="ComboMode"/>
            internal static void CheckEnabled()
            {
                if (isInitializing || (ComboMode & ComboMode.Orbbec) == ComboMode.Orbbec)
                    return;
                throw new InvalidOperationException($"This functionality requires SDK initialization in {nameof(ComboMode)}.{nameof(ComboMode.Orbbec)} or {nameof(ComboMode.Both)}.");
            }

            /// <summary>
            /// Initializes Orbbec implementation and forces native library loading of `Orbbec SDK K4A Wrapper`.
            /// </summary>
            /// <seealso cref="Sdk.Init(ComboMode)"/>
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
