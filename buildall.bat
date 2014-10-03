@echo off
REM Build.bat 
REM Batch file for building lcmsnet with dmstools and plugins as if using the PNNL internal installer.
REM Written by Christopher Walters 9/12/2014
REM Last Modified 9/29/2014
REM Path to MSBuild must be in your PATH environment variable in order for this batch file to work.

SETLOCAL
SET ROOTPATH=%~dp0
SET CONFIG=/p:Configuration=PNNLRelease /p:Platform=x86
SET VERB=/verbosity:quiet
SET LOG=/fileLogger
SET LOGFILE=%ROOTPATH%lcmsnetBuild.txt
SET LGPARAMS=/fileloggerparameters:logfile=%LOGFILE%;append=true
REM SET NOLOGO=/nologo
SET BOPTS=%VERB% %LOG% %LGPARAMS% 

if exist %LOGFILE% DEL %LOGFILE%
REM Build LcmsNetSDK solution
ECHO Building LcmsNetSDK solution...
MSBuild %CONFIG% %BOPTS% %ROOTPATH%LcmsNet\SDK\LcmsNetSDKs.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build LcmsNetSQLiteTools solution
ECHO Building LcmsNetSQLiteTools solution...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%LcmsNet\SQLiteTools\SQLiteTools.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build LcmsNetDMSTools
ECHO Building LcmsNetDmsTools solution...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build Plugins
ECHO Building LcmsNetPlugins...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%lcmsNetPlugins\lcmsnetPlugins.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build Main solution.
REM If we're not building in release mode, make sure to copy library files into proper
REM locations for testing purposes.
IF [%1]==[] (
ECHO Building LcmsNet...
MSBUILD /p:Configuration=Debug /p:targetPlatform=x86 %BOPTS% %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNet.sln 

ECHO Copying DmsTools to %APPDATA%\LCMSNet\dmsExtensions
xcopy /y /q %ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools\bin\x86\PNNLRelease\* %APPDATA%\LCMSNet\dmsExtensions\
xcopy /y /q %ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools\prismDMS.config %APPDATA%\LCMSNet\dmsExtensions\
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Copying Plugins to %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins
xcopy /y /q %ROOTPATH%pluginDLLs\*.dll %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins\
xcopy /y /q %ROOTPATH%LcmsNet\lib\FluidicsPack.dll %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Copying Core Validator to %APPDATA%\LCMSNet\SampleValidators
xcopy /y /q %ROOTPATH%\LcmsNet\SDK\CoreSampleValidator\bin\x86\PNNLRelease\*.dll %APPDATA%\LCMSNet\SampleValidators\
ECHO Done.

ECHO Copying PAL Validator to %APPDATA%\LCMSNet\SampleValidators
xcopy /y /q %ROOTPATH%\LcmsNetPlugins\PalValidator\bin\x86\debug\*.dll %APPDATA%\LCMSNet\SampleValidators\
ECHO Done.
goto :COMPLETE)


REM If we're building in release mode, we don't need to do all the copying of files, since the
REM installer script already knows where to look for the library files, and knows where to put
REM them during install.
IF %1==release (
ECHO Building LcmsNet in release mode...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNet.sln
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Building LogViewer...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%\ExternalApplications\LogViewer\LogViewer.sln
ECHO Done.
goto :COMPLETE)

:ERROR
ECHO Error occured during build. See lcmsnetBuild.txt for more details.
goto :END

:COMPLETE
ECHO Cleaning up...
DEL %LOGFILE%
ECHO BUILD COMPLETE.

:END