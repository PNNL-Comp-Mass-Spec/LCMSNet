@echo off
REM Build.bat 
REM Batch file for building lcmsnet with dmstools and plugins as if using the PNNL internal installer.
REM Written by Christopher Walters 9/12/2014
REM Path to MSBuild must be in your PATH environment variable in order for this batch file to work.

SETLOCAL
SET ROOTPATH=%~dp0
SET CONFIG=/p:Configuration=Debug /p:Platform=x86
SET VERB=/verbosity:quiet
SET LOG=/fileLogger
SET LOGFILE=%ROOTPATH%lcmsnetBuild.txt
SET LGPARAMS=/fileloggerparameters:logfile=%LOGFILE%;append=true
SET NOLOGO=/nologo
SET BOPTS=%CONFIG% %VERB% %LOG% %LGPARAMS% %NOLOGO%

if exist %LOGFILE% DEL %LOGFILE%
REM Build LcmsNetSDK solution
ECHO Building LcmsNetSDK solution...
MSBuild %BOPTS% %ROOTPATH%LcmsNet\SDK\LcmsNetSDKs.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build LcmsNetSQLiteTools solution
ECHO Building LcmsNetSQLiteTools solution...
MSBUILD %BOPTS% %ROOTPATH%LcmsNet\SQLiteTools\SQLiteTools.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build LcmsNetDMSTools
ECHO Building LcmsNetDmsTools solution...
MSBUILD %BOPTS% %ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build Plugins
ECHO Building LcmsNetPlugins...
MSBUILD %BOPTS% %ROOTPATH%lcmsNetPlugins\lcmsnetPlugins.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build Main solution.
ECHO Building LcmsNet...
MSBUILD %BOPTS% %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNet.sln 
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Copying DmsTools to %APPDATA%\LCMSNet\dmsExtensions
xcopy /y /q %ROOTPATH%LcmsNetDmsTools\LcmsNetDmsTools\bin\Release\*.dll %APPDATA%\LCMSNet\dmsExtensions\
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Copying Plugins to %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins
xcopy /y /q %ROOTPATH%pluginDLLs\*.dll %ROOTPATH%LcmsNet\lcms\LCMSNet\LCMSNetProg\bin\x86\debug\plugins\
if errorlevel 1 goto :ERROR
ECHO Done.
goto :COMPLETE

:ERROR
ECHO Error occured during build. See lcmsnetBuild.txt for more details.
goto :END

:COMPLETE
ECHO Cleaning up...
DEL %LOGFILE%
ECHO BUILD COMPLETE.

:END