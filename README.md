# K4A.Net (K4AdotNet)

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
* No unsafe code in library **K4AdotNet** itself (only `DllImports`)
* CLS-complaint (can be used from any .Net-compatible language, including C#, VB.Net)
* Library **K4AdotNet** is compiled against **.NET Standard 2.0** and **.NET Framework 4.6.1** target frameworks
  * This makes it compatible with **.NET Core 2.0** and later, **.NET Framework 4.6.1** and later, **Unity 2018.1** and later, etc.
  * See https://docs.microsoft.com/en-us/dotnet/standard/net-standard for details
* Clean API, which is close to C/C++ native API from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download). Plus useful helper methods.
* No additional dependencies
  * Except dependencies on native libraries (DLLs) from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download)
  * Native libraries from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) are included to repository (see `externals` directory)
  * But native libraries from [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download) are *not* included to repository
  * For details see below
* Plenty of powerful samples
  * More samples will be available soon (stay tuned)
* Unit-tested
  * To be done ASAP
* Well documented
  * To be done ASAP
* Potentially multi-platform (Windows, Linux)
  * But currently tested only under Windows
  * Plus there's no Linux-version of [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download) yet
  * And most of samples are written using WPF


## Dependencies

**K4AdotNet** depends on the following native libraries (DLLs) from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download):

| Library "component" | Depend on                        | Version in use | Location in source code                 |
|---------------------|----------------------------------|----------------|-----------------------------------------|
| Sensor API          | `k4a.dll`, `depthengine_1_0.dll` | 1.1.1          | `externals/k4a/windows-desktop/amd64`   |
| Record API          | `k4arecord.dll`                  | 1.1.1          | `externals/k4a/windows-desktop/amd64`   |
| Body Tracking API   | `k4abt.dll`, `onnxruntime.dll`   | 0.9            |                                         |

Some important notes:
* `depthengine_1_0.dll` is required only if you are using `Transformation` or `Device` classes. All other Sensor API (types from `K4AdotNet.Sensor` namespace) depends only on `k4a.dll`.
* Native libraries from Body Tacking runtime are not included to repository because they, in turn, depend on:
  * bulky `dnn_model.onnx` file (159 MB)
  * [NVIDIA CUDA 10.0](https://developer.nvidia.com/cuda-10.0-download-archive)
  * [NVIDIA cuDNN](https://developer.nvidia.com/cudnn)
* The easiest way to use Body Tracking is to ask user to install **Body Tracking SDK** and all required components by him/herself based on the following step-by-step instruction: https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup
* **K4AdotNet** is trying to find Body Tracking runtime in the following locations:
  * directory with executable file
  * directory with **K4AdotNet** assembly
  * installation directory of **Body Tracking SDK** under `Program Files`
* Use `bool Sdk.IsBodyTrackingRuntimeAvailable(out string message)` method to check if Body Tracking runtime and all required components are available/installed
* Also, you can optionally call `bool Sdk.TryInitializeBodyTrackingRuntime(out string message)` method on start of your application to initialize Body Tracking runtime (it can take a few seconds)


## Current status

It's almost ready to use


## Roadmap

* Nuget package(s)
* Documentation comments
* More unit-tests
* More samples (Recording, 3D view, Box-man, IMU...)
* Samples for Unity3D
* Find out how to convert MJPEG -> BGRA faster (implementation in `k4a.dll` is very slow)
* Test under Linux, samples for Linux (using [Avalonia UI Framework](http://avaloniaui.net/)?)
* Some hosting for documentation


## How to build

* Open `K4AdotNet.sln` in Visual Studio 2017 or Visual Studio 2019
* Build solution (`Ctrl+Shift+B`)
* After that you can run and explore samples
