@echo off
rem Build.bat 
rem Batch file for building lcmsnet with dmstools and plugins as if using the PNNL internal installer.
rem Written by Christopher Walters 10/22/2014
rem Updated 03/02/2017 to auto-define the path to MSBuild.exe if necessary

SETLOCAL
SET ROOTPATH=%~dp0
SET CONFIG=/p:Configuration=PNNLRelease /p:Platform=x86
SET VERB=/verbosity:quiet
SET LOG=/fileLogger
SET LOGFILE="%ROOTPATH%lcmsnetBuild.txt"
SET LGPARAMS=/fileloggerparameters:logfile=%LOGFILE%;append=true
rem SET NOLOGO=/nologo
SET BOPTS=%VERB% %LOG% %LGPARAMS% 

rem Determine the path to MSBuild
where /q msbuild.exe
IF ERRORLEVEL 1 (
   echo "Using MSBuild.exe at C:\Program Files (x86)\MSBuild\14.0\Bin\"
   echo.
   SET MSBuildExe="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
) Else (
   echo "Using MSBuild.exe in your path"
   echo.
   Set MSBuildExe=msbuild.exe
)

if exist %LOGFILE% DEL %LOGFILE%

rem Build LcmsNetSDK solution
echo Building LcmsNetSDK solution...
echo %MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%SDK\LcmsNetSDKs.sln"
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%SDK\LcmsNetSDKs.sln"
if errorlevel 1 goto :ERROR
echo Done.
echo.

rem Build LcmsNetSQLiteTools solution
echo Building LcmsNetSQLiteTools solution...
echo %MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%SQLiteTools\SQLiteTools.sln"
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%SQLiteTools\SQLiteTools.sln"
if errorlevel 1 goto :ERROR
echo Done.
echo.

rem Build Main solution.
echo Building LcmsNet...
echo %MSBuildExe% /p:Configuration=Debug /p:targetPlatform=x86 %BOPTS% "%ROOTPATH%lcms\LCMSNet\LCMSNet.sln"
%MSBuildExe% /p:Configuration=Debug /p:targetPlatform=x86 %BOPTS% "%ROOTPATH%lcms\LCMSNet\LCMSNet.sln"
if errorlevel 1 goto :ERROR
echo Done.
echo.

echo Copying Core Validator to %APPDATA%\LCMSNet\SampleValidators
xcopy /y /q /d "%ROOTPATH%SDK\CoreSampleValidator\bin\x86\PNNLRelease\*.dll" %APPDATA%\LCMSNet\SampleValidators\
echo Done. 
echo.

if errorlevel 1 goto :ERROR
goto :COMPLETE

:ERROR
echo Error occured during build. See lcmsnetBuild.txt for more details.
goto :END

:COMPLETE
echo Cleaning up...
DEL %LOGFILE%

echo BUILD COMPLETE.

:END
