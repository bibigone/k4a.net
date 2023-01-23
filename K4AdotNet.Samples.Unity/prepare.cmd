rem Run this script from VS Developer Command Prompt before first opening Unity project
rem It will build K4AdotNet project and copy necessary binaries to Unity project folders
rem You may run it again anytime to update binaries with changes made to K4AdotNet library

rem Build fresh K4AdotNet binaries (requires path to msbuild.exe set by Developer Command Prompt)
msbuild ..\K4AdotNet -p:Configuration=Debug -p:Platform="Any CPU"

rem Copy K4AdotNet binaries to "Assets\Plugins\K4AdotNet" folder
xcopy ..\K4AdotNet\bin\Debug\netstandard2.0\* Assets\Plugins\K4AdotNet\ /A /Y

rem Copy K4A Sensor runtime to "Assets\Plugins\K4AdotNet" folder
xcopy ..\externals\k4a\windows-desktop\amd64\*.dll Assets\Plugins\K4AdotNet\ /A /Y

rem Copy k4abt.dll from K4A Body Tracking runtime to "Assets\Plugins\K4AdotNet" folder (assumes Body Tracking SDK installed into a standard location)
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\k4abt.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\dnn_model_2_0_op11.onnx" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\dnn_model_2_0_lite_op11.onnx" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cublas64_11.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cublasLt64_11.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cudart64_110.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cudnn_cnn_infer64_8.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cudnn_ops_infer64_8.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cudnn64_8.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\cufft64_10.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\onnxruntime.dll" Assets\Plugins\K4AdotNet\
copy /y "%ProgramFiles%\Azure Kinect Body Tracking SDK\tools\vcomp140.dll" Assets\Plugins\K4AdotNet\

rem If Body Tracking SDK is installed to another location then copy manually the following libraries and data files
rem from "tools" folder of Body Tracking SDK to "Assets\Plugins\K4AdotNet" folder of this plugin:
rem k4abt.dll, dnn_model_2_0_op11.onnx and/or dnn_model_2_0_lite_op11.onnx, cublas64_11.dll, cublasLt64_11.dll, cudart64_110.dll,
rem cudnn_cnn_infer64_8.dll, cudnn_ops_infer64_8.dll, cudnn64_8.dll, cufft64_10.dll, onnxruntime.dll, vcomp140.dll. 