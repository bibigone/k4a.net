# Unity Sample

This sample demonstrate how to work with Sensor and Body data streams from Unity
and how to animate 3D character using body data.

## Preparations

Before first opening a Unity project, run `prepare.cmd` from **VS Developer Command Prompt**.
It will build **K4AdotNet** project and copy necessary binaries to Unity project folders.
You may run the script anytime later to update binaries with changes made to **K4AdotNet** library.

The script assumes that **Azure Kinect Body Tracking SDK** is installed into default location under Program Files.
If it doesn't take place then copy the following files from `tools` folder of Body Tracking SDK to `Assets\Plugins\K4AdotNet` folder of this plugin:
* `k4abt.dll`,
* `dnn_model_2_0_op11.onnx` and/or `dnn_model_2_0_lite_op11.onnx`,
* `cublas64_11.dll`,
* `cublasLt64_11.dll`,
* `cudart64_110.dll`,
* `cudnn_cnn_infer64_8.dll`,
* `cudnn_ops_infer64_8.dll`,
* `cudnn64_8.dll`,
* `cufft64_10.dll`,
* `onnxruntime.dll`,
* `vcomp140.dll`.

## Dependencies

This plugin depends on the following managed and native libraries and data files:
* **K4AdotNet** managed library (see `..\K4AdotNet\bin\Debug\netstandard2.0` or ``..\K4AdotNet\bin\Release\netstandard2.0`` folder):
  * `K4AdotNet.dll`
  * `K4AdotNet.deps.json`
* native libraries from **Azure Kinect Sensor SDK 1.4.1** (see `..\externals\k4a\windows-desktop\amd64` folder):
  * `depthengine_2_0.dll`
  * `k4a.dll`
  * `k4arecord.dll`
* native libraries and ONNX-file from **Azure Kinect Body Tracking SDK 1.1.x** (you can download and install MSI package from [here](https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download) and find required files in `tools` subdirectory of installation destination directory):
  * `k4abt.dll`,
  * `dnn_model_2_0_op11.onnx` and/or `dnn_model_2_0_lite_op11.onnx`,
  * `cublas64_11.dll`,
  * `cublasLt64_11.dll`,
  * `cudart64_110.dll`,
  * `cudnn_cnn_infer64_8.dll`,
  * `cudnn_ops_infer64_8.dll`,
  * `cudnn64_8.dll`,
  * `cufft64_10.dll`,
  * `onnxruntime.dll`,
  * `vcomp140.dll`.

As a rule `prepare.cmd` does the trick, but you can copy all dependencies to `Assets\Plugins\K4AdotNet` folder of this plugin manually.
 