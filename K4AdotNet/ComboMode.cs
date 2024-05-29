using System;

namespace K4AdotNet
{
    /// <summary>
    /// Main operational mode of the library: how to combine `original K4A` and `Orbbec SDK K4A Wrapper`.
    /// Depending on the mode the library can work only with Azure Kinect devices or only with Orbbec Femto device
    /// or with both simultaneously.
    /// </summary>
    /// <seealso cref="Sdk.ComboMode"/>
    /// <seealso cref="Sdk.Init(ComboMode)"/>
    [Flags]
    public enum ComboMode
    {
        /// <summary>
        /// The library works only with Azure Kinect devices via `original K4A` native libraries.
        /// </summary>
        /// <remarks>In this mode the library operates almost as the `k4a.net` library.</remarks>
        Azure = 1,

        /// <summary>
        /// The library works only with Orbbec Femto devices via `Orbbec SDK K4A Wrapper` natives libraries.
        /// </summary>
        /// <remarks>In this mode the library operates almost as the `k4a.net-femto` library.</remarks>
        Orbbec = 2,

        /// <summary>
        /// The library supports Azure Kinect and Orbbec Femto devices simultaneously,
        /// but with some significant limitations and restrictions.
        /// Please, use this mode WITH CARE.
        /// </summary>
        /// <remarks>
        /// This mode is the most powerful and convenient, but at the same time this mode is the most dangerous.
        /// The this is that, in Windows only one DLL with one and the same name can be loaded to the process.
        /// For this reason both implementations (Azure and Orbbec) will share one and the same instance of 
        /// <c>depthengine_2_0.dll</c> library. And it looks like that <c>depthengine_2_0.dll</c> from the
        /// `Orbbec SDK K4A Wrapper` works pretty well with Azure Kinect devices, while <c>depthengine_2_0.dll</c> from
        /// the `original K4A` doesn't work correctly with Orbbec Femto devices.
        /// Moreover, while there are actually two instances of the <c>k4a.dll</c> library in the process
        /// (thanks to special DLL importing and loading mechanism in the .NET runtime), other DLLs can depend
        /// only from the one instance of <c>k4a.dll</c>. For this reason playback and recording functionality
        /// (implemented via <c>k4arecord.dll</c> native library which in turn depends on <c>k4a.dll</c>) will
        /// operate only via `Orbbec SDK K4A Wrapper` infrastructure. And body tracking functionality
        /// (implemented via <c>k4abt.dll</c> native library which in turn depends on <c>k4a.dll</c>) will also
        /// operate only via `Orbbec SDK K4A Wrapper` infrastructure.
        /// </remarks>
        Both = Azure | Orbbec,
    }
}
