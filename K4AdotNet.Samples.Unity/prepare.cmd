rem Run this script from VS Developer Command Prompt before first opening Unity project
rem It will build K4AdotNet project and copy necessary binaries to Unity project folders
rem You may run it again anytime to update binaries with changes made to K4AdotNet library

rem Build fresh K4AdotNet binaries (requires path to msbuild.exe set by Developer Command Prompt)
msbuild ..\K4AdotNet -p:Configuration=Debug

rem Copy K4AdotNet binaries to a plugin folder
xcopy ..\K4AdotNet\bin\Debug\netstandard2.0\* Assets\Plugins\K4AdotNet\ /A /Y

rem Copy K4A Sensor runtime to a plugin folder
xcopy ..\externals\k4a\windows-desktop\amd64\*.dll Assets\Plugins\K4AdotNet\ /A /Y

rem Copy K4A Body Tracking runtime to a plugin folder (assumes Body Tracking SDK installed into a standard location)
rem If installed to another location then copy k4abt.dll manually
copy /y "C:\Program Files\Azure Kinect Body Tracking SDK\sdk\windows-desktop\amd64\release\bin\k4abt.dll" Assets\Plugins\K4AdotNet\