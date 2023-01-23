# K4A.Net (K4AdotNet)

<img align="right" width="64" height="64" src="https://github.com/bibigone/k4a.net/raw/master/K4AdotNet-64.png">

**K4A.Net** &mdash; *Three-in-one* managed .NET library to work with [Azure Kinect](https://azure.microsoft.com/en-us/services/kinect-dk/) devices (also known as Kinect for Azure, K4A, Kinect v4). It consists of the following "components":
1. `Sensor API` &mdash; access to depth camera, RGB camera, accelerometer and gyroscope, plus device-calibration data and synchronization control
   * Corresponding namespace: `K4AdotNet.Sensor`
   * Corresponding native API: [`k4a.h`](https://github.com/bibigone/k4a.net/blob/master/externals/k4a/include/k4a/k4a.h)
2. `Record API` &mdash; data recording from device to MKV-files, and data reading from such files
   * Corresponding namespace: `K4AdotNet.Record`
   * Corresponding native API: [`record.h`](https://github.com/bibigone/k4a.net/blob/master/externals/k4a/include/k4arecord/record.h) and [`playback.h`](https://github.com/bibigone/k4a.net/blob/master/externals/k4a/include/k4arecord/playback.h)
3. `Body Tracking API` &mdash; body tracking of multiple skeletons including eyes, ears and nose
   * Corresponding namespace: `K4AdotNet.BodyTracking`
   * Corresponding native API: [`k4abt.h`](https://github.com/bibigone/k4a.net/blob/master/externals/k4abt/include/k4abt.h)


## Key features

* Written fully on C#
* CLS-compliant (can be used from any .Net-compatible language, including C#, F#, VB.Net)
* Library **K4AdotNet** is compiled against **NET 6.0**, **.NET Standard 2.0** and **.NET Framework 4.6.1** target frameworks
  * This makes it compatible with **.NET 5-7**, **.NET Core 2.0-3.1**, **.NET Framework 4.6.1** and later, **Unity 2018.1** and later, etc.
  * See https://docs.microsoft.com/en-us/dotnet/standard/net-standard for details
* Clean API, which is close to C/C++ native API from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download).
* Plus useful helper methods, additional checks and meaningful exceptions.
* Full feature set (all API provided by native SDKs are available in this C# wrapper)
* Up-to-date with the latest versions of native SDKs
* No additional dependencies
  * Except dependencies on native libraries (DLLs) from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download)
  * Native libraries from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) are included to repository (see `externals` directory) and [NuGet package](https://www.nuget.org/packages/K4AdotNet)
  * But native libraries from [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download) are *not* included to repository. It is recommended to install [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download) separately. For details see below
* Plenty of powerful samples:
  * .NET 7 Console Application
  * .NET 7 WPF Application (Windows only)
  * Unity Project
* Well documented
* Unit-tested
* Multi-platform (Windows, Linux)
  * But most of samples are written using WPF
* Available as NuGet package: https://www.nuget.org/packages/K4AdotNet


## Dependencies

**K4AdotNet** depends on the following native libraries (DLLs) from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download):

| Library "component" | Depends on                                   | Version in use | Location in repository                | Included in [NuGet package](https://www.nuget.org/packages/K4AdotNet) 
|---------------------|----------------------------------------------|----------------|---------------------------------------|--------------------------
| Sensor API          | `k4a.dll`, `depthengine_2_0.dll`<sup>(1)</sup> | 1.4.1          | `externals/k4a/windows-desktop/amd64` | YES
| Record API          | `k4arecord.dll`                              | 1.4.1          | `externals/k4a/windows-desktop/amd64` | YES
| Body Tracking API   | `k4abt.dll`<sup>(2)</sup>, `dnn_model_2_0_op11.onnx` and/or `dnn_model_2_0_lite_op11.onnx` | 1.1.2          |                                       | no<sup>(3)</sup>

Notes:
* <sup>(1)</sup> `depthengine_2_0.dll` is required only if you are using `Transformation` or `Device` classes. All other Sensor API (types from `K4AdotNet.Sensor` namespace) depends only on `k4a.dll`.
* <sup>(2)</sup> `k4abt.dll` uses [ONNX Runtime](https://github.com/microsoft/onnxruntime) &mdash; `onnxruntime.dll`, which in turn depends on the following [NVIDIA cuDNN](https://developer.nvidia.com/cudnn) and [NVIDIA CUDA 11.2.1](https://developer.nvidia.com/cuda-11.2.1-download-archive) libraries. Also, Visual C++ Redistributable for Visual Studio 2015 is required: `vcomp140.dll`.
* <sup>(3)</sup> The full list of libraries and data files required for Body Tracking: <br/>
`k4abt.dll` (4.4 MB), <br/>
`dnn_model_2_0_op11.onnx` (159 MB) and/or `dnn_model_2_0_lite_op11.onnx` (43 MB), <br/>
`cublas64_11.dll` (136 MB), <br/>
`cublasLt64_11.dll` (269 MB), <br/>
`cudart64_110.dll` (0.5 MB), <br/>
`cudnn_cnn_infer64_8.dll` (600 MB), <br/>
`cudnn_ops_infer64_8.dll` (346 MB), <br/>
`cudnn64_8.dll` (0.3 MB), <br/>
`cufft64_10.dll` (345 MB), <br/>
`vcomp140.dll` (0.2 MB). <br/>
(Plus additional libraries for `TensorRT` and `DirectML` tracking modes.)<br/>
It is mostly unpractical to have such bulky files in repositories. For this reason they are not included to the repository. Also, they are not included to [NuGet package](https://www.nuget.org/packages/K4AdotNet).

How to use Body Tracking runtime:
* The easiest way to use Body Tracking is to ask user to install **Body Tracking SDK** by him/herself: https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup
* But you can also put all required libraries and data files to the output directory of your project (on post-build step, for example). All required libraries and data files can be found in directory `tools` of Body Tracking SDK.
* **K4AdotNet** is trying to find Body Tracking runtime in the following locations:
  * directory with executable file
  * directory with **K4AdotNet** assembly
  * installation directory of **Body Tracking SDK** under `Program Files`
* Use `bool Sdk.IsBodyTrackingRuntimeAvailable(out string message)` method to check if Body Tracking runtime and all required components are available/installed
* Also, you can optionally call `bool Sdk.TryInitializeBodyTrackingRuntime(TrackerProcessingMode mode, out string message)` method on start of your application to initialize Body Tracking runtime (it can take a few seconds)


## Versions

See https://github.com/bibigone/k4a.net/releases


## How to build

* Open `K4AdotNet.sln` in Visual Studio 2022
* Build solution (`Ctrl+Shift+B`)
* After that you can run and explore samples:
  * `K4AdotNet.Samples.Console.BodyTrackingSpeed` &mdash; sample .NET 7 console application to measure speed of Body Tracking.
  * `K4AdotNet.Samples.Console.Recorder` &mdash; sample .NET 7 console application to record data from Azure Kinect device to MKV file.
  * `K4AdotNet.Samples.Wpf.Viewer` &mdash; sample WPF application to demonstrate usage of Sensor API and Record API.
  * `K4AdotNet.Samples.Wpf.BodyTracker` &mdash; sample WPF application to demonstrate usage of Body Tracking API.
  * `K4AdotNet.Samples.Wpf.BackgroundRemover` &mdash; sample WPF application which implements the background removal effect for color picture with the help of depth data.
* Instruction on building Unity sample can be found [here](https://github.com/bibigone/k4a.net/blob/master/K4AdotNet.Samples.Unity/readme.md).
