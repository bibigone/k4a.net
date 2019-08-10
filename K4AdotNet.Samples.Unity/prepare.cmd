rem Run this script from VS Developer Command Prompt before first opening Unity project
rem It will build K4AdotNet project and copy necessary binaries to Unity project folders
rem You may run it again anytime to update binaries with changes made to K4AdotNet library

msbuild ..\K4AdotNet -p:Configuration=Debug
xcopy ..\K4AdotNet\bin\Debug\netstandard2.0\* Assets\Plugins\K4AdotNet\ /A /Y
xcopy ..\externals\k4a\windows-desktop\amd64\*.dll .\ /A /Y
xcopy ..\externals\k4a\windows-desktop\amd64\*.dll Assets\Plugins\K4AdotNet\ /A /Y