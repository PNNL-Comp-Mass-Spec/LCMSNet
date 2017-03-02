@echo off
rem Build.bat 
rem Batch file for building lcmsnet with dmstools and plugins as if using the PNNL internal installer.
rem Written by Christopher Walters 9/12/2014
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
@echo on
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%LcmsNet\SDK\LcmsNetSDKs.sln"
@echo off
if errorlevel 1 goto :ERROR
echo Done.

rem Build LcmsNetSQLiteTools solution
echo Building LcmsNetSQLiteTools solution...
@echo on
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%LcmsNet\SQLiteTools\SQLiteTools.sln"
@echo off
if errorlevel 1 goto :ERROR
echo Done.

rem Build LcmsNetDMSTools
echo Building LcmsNetDmsTools solution...
@echo on
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools.sln"
@echo off
if errorlevel 1 goto :ERROR
echo Done.

rem Build Plugins
echo Building LcmsNetPlugins...
@echo on
%MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%lcmsNetPlugins\lcmsnetPlugins.sln"
@echo off
if errorlevel 1 goto :ERROR
echo Done.

rem Build Main solution.
rem If we're not building in release mode, make sure to copy library files into proper
rem locations for testing purposes.
IF [%1]==[] (
   echo Building LcmsNet...
   @echo on
   %MSBuildExe% /p:Configuration=Debug /p:targetPlatform=x86 %BOPTS% "%ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNet.sln"
   @echo off

   echo Copying DmsTools to %APPDATA%\LCMSNet\dmsExtensions
   xcopy /y /q /d "%ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools\bin\x86\PNNLRelease\*" %APPDATA%\LCMSNet\dmsExtensions\
   xcopy /y /q /d "%ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools\prismDMS.config" %APPDATA%\LCMSNet\dmsExtensions\
   if errorlevel 1 goto :ERROR
   echo Done.

   echo Copying Plugins to "%ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins"
   xcopy /y /q /d "%ROOTPATH%pluginDLLs\*.dll" "%ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins\"
   xcopy /y /q /d "%ROOTPATH%LcmsNet\lib\FluidicsPack.dll" "%ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins"
   if errorlevel 1 goto :ERROR
   echo Done.

   echo Copying Core Validator to %APPDATA%\LCMSNet\SampleValidators
   xcopy /y /q /d "%ROOTPATH%\LcmsNet\SDK\CoreSampleValidator\bin\x86\PNNLRelease\*.dll" %APPDATA%\LCMSNet\SampleValidators\
   echo Done.

   echo Copying PAL Validator to %APPDATA%\LCMSNet\SampleValidators
   xcopy /y /q /d "%ROOTPATH%\LcmsNetPlugins\PalValidator\bin\x86\debug\*.dll" %APPDATA%\LCMSNet\SampleValidators\
   echo Done.
   goto :COMPLETE
)

rem If we're building in release mode, we don't need to do all the copying of files, since the
rem installer script already knows where to look for the library files, and knows where to put
rem them during install.
IF %1==release (
   echo Building LcmsNet in release mode...
   @echo on
   %MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNet.sln"
   @echo off
   if errorlevel 1 goto :ERROR
   echo Done.

   echo Building LogViewer...
   @echo on
   %MSBuildExe% %CONFIG% %BOPTS% "%ROOTPATH%\ExternalApplications\LogViewer\LogViewer.sln"
   @echo off
   echo Done.
   goto :COMPLETE
)

:ERROR
echo Error occured during build. See lcmsnetBuild.txt for more details.
goto :END

:COMPLETE
echo Cleaning up...
DEL %LOGFILE%

echo BUILD COMPLETE.

:END
