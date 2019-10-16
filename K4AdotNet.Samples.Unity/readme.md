# Unity Sample

This sample demonstrate how to work with Sensor and Body data streams from Unity
and how to animate 3D character using body data.

## Preparations

Before first opening a Unity project, run `prepare.cmd` from **VS Developer Command Prompt**.
It will build **K4AdotNet** project and copy necessary binaries to Unity project folders.
You may run the script anytime later to update binaries with changes made to **K4AdotNet** library.

The script assumes that **Azure Kinect Body Tracking SDK** is installed into default location under Program Files.
If it doesn't take place or you want to have portable solution which doesn't depend on presence of Body Tracking SDK on local machine
then copy the following files from `tools` folder of Body Tracking SDK to `Assets\Plugins\K4AdotNet` folder of this plugin:
* `k4abt.dll`,
* `dnn_model_2_0.onnx`,
* `cudnn64_7.dll`,
* `cublas64_100.dll`,
* `cudart64_100.dll`,
* `vcomp140.dll`.

## Dependencies

This plugin depends on the following managed and native libraries and data files:
* **K4AdotNet** managed library (see `..\K4AdotNet\bin\Debug\netstandard2.0` or ``..\K4AdotNet\bin\Release\netstandard2.0`` folder):
  * `K4AdotNet.dll`
  * `K4AdotNet.deps.json`
* native libraries from **Azure Kinect Sensor SDK 1.3.0** (see `..\externals\k4a\windows-desktop\amd64` folder):
  * `depthengine_2_0.dll`
  * `k4a.dll`
  * `k4arecord.dll`
* native libraries and ONNX-file from **Azure Kinect Body Tracking SDK 0.9.4** (you can download and install MSI package from [here](https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download) and find required files in `tools` subdirectory of installation destination directory):
  * `k4abt.dll`,
  * `dnn_model_2_0.onnx`,
  * `cudnn64_7.dll`,
  * `cublas64_100.dll`,
  * `cudart64_100.dll`,
  * `vcomp140.dll`.

As a rule `prepare.cmd` does the trick, but you can copy all dependencies to `Assets\Plugins\K4AdotNet` folder of this plugin manually.
 