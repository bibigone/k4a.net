@ECHO OFF

set MSBUILD_PATH="%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MsBuild.exe"

cd ..

package\nuget.exe restore
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%MSBUILD_PATH% K4AdotNet.sln /t:Clean /p:Configuration=Release /p:Platform="x64" /v:m
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%MSBUILD_PATH% K4AdotNet.sln /t:Rebuild /p:Configuration=Release /p:Platform="x64" /v:m
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%MSBUILD_PATH% K4AdotNet.sln /t:Clean /p:Configuration=Release /p:Platform="Any CPU" /v:m
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%MSBUILD_PATH% K4AdotNet.sln /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU" /v:m
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

cd package

nuget.exe pack k4adotnet-combo.nuspec 
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo DONE!
exit /B 0

:ERROR
echo FAIL!
pause
exit /B -1
