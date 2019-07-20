# K4A.Net (K4AdotNet)

**K4A.Net** &mdash; *Three-in-one* managed .NET library to work with [Azure Kinect](https://azure.microsoft.com/en-us/services/kinect-dk/) devices (also known as Kinect for Azure, K4A, Kinect v4). It consists of the following "components":
1. `Sensor API` &mdash; access to depth camera, RGB camera, accelerometer and gyroscope, plus device-calibration data and synchronization control
   * Corresponding namespace: `K4AdotNet.Sensor`
2. `Record API` &mdash; data recording from device to MKV-files, and data reading from such files
   * Corresponding namespace: `K4AdotNet.Record`
3. `Body Tracking API` &mdash; body tracking of multiple skeletons including eyes, ears and nose
   * Corresponding namespace: `K4AdotNet.BodyTracking`


## Key features

* Written fully on C#
* No unsafe code in library **K4AdotNet** itself (only `DllImports`)
* CLS-complaint (can be used from any .Net-compatible language, including C#, VB.Net)
* Library **K4AdotNet** is compiled against .NET Standard 2.0 target framework
  * This makes it compatible with .NET Core 2.0 and later, .NET Framework 4.6.1 and later, Unity 2018.1 and later, etc.
  * See https://docs.microsoft.com/en-us/dotnet/standard/net-standard for details
* Clean API, which is close to C/C++ native API from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download). Plus useful helper methods.
* No additional dependencies
  * Except dependencies on native libraries (DLLs) from [Azure Kinect Sensor SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download) and [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-download)
  * These native libraries are included to repo (see `externals` directory)
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
| Body Tracking API   | `k4abt.dll`, `onnxruntime.dll`   | 0.9            | `externals/k4abt/windows-desktop/amd64` | 

Some important notes:
* `depthengine_1_0.dll` is required only if you are using `Transformation` class to transform images between cameras (depth to color, color to depth, depth to point cloud). All other Sensor API (types from `K4AdotNet.Sensor` namespace) depends only on `k4a.dll`.
* `k4abt.dll` (native body tracking library) depends on `onnxruntime.dll` &mdash; [ONNX](https://onnx.ai/) runtime for neural networks, which in turn, depends on [NVIDIA CUDA 10.0](https://developer.nvidia.com/cuda-10.0-download-archive) and `cudnn64_7.dll` library (part of [NVIDIA cuDNN](https://developer.nvidia.com/cudnn). For details see https://docs.microsoft.com/en-us/azure/Kinect-dk/body-sdk-setup
* Also, Body Tracking requires ONNX-file with model of neural network: `dnn_model.onnx` (159 MB). You can find it in `externals/k4abt/windows-desktop/amd64`.
* If you're not going to use some part of API, then you can ignore (don't copy to your project) appropriate native library(ies).


## Current status

It's almost ready to use


## Roadmap

* More samples (Body tracking WPF sample, 3D view, ...)
* Documentation comments
* More unit-tests
* Nuget package(s)
* Find out how to convert MJPEG -> BGRA faster (implementation in `k4a.dll` is very slow)
* Test under Linux, samples for Linux (using [Avalonia UI Framework](http://avaloniaui.net/)?)
* Samples for Unity3D
* Some hosting for documentation


## How to build

* Open `K4AdotNet.sln` in Visual Studio 2017 or Visual Studio 2019
* Build solution (`Ctrl+Shift+B`)
* After that you can run and explore samples
