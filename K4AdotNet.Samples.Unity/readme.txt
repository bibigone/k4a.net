Before first opening a Unity project, run prepare.cmd from VS Developer Command Prompt.
It will build K4AdotNet project and copy necessary binaries to Unity project folders.
You may run the script anytime later to update binaries with changes made to K4AdotNet library.

Also the script assumes that Azure Kinect Body Tracking SDK is installed into default location under C:\Program Files.
If not copy k4abt.dll manually to Assets\Plugins\K4AdotNet.
It is commonly found in sdk\windows-desktop\amd64\release\bin in the installation directory of the SDK.