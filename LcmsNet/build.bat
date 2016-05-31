@echo off
REM Build.bat 
REM Batch file for building lcmsnet with dmstools and plugins as if using the PNNL internal installer.
REM Written by Christopher Walters 10/22/2014
REM Last Modified 10/22/2014
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
MSBuild %CONFIG% %BOPTS% %ROOTPATH%SDK\LcmsNetSDKs.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build LcmsNetSQLiteTools solution
ECHO Building LcmsNetSQLiteTools solution...
MSBUILD %CONFIG% %BOPTS% %ROOTPATH%SQLiteTools\SQLiteTools.sln
if errorlevel 1 goto :ERROR
ECHO Done.

REM Build Main solution.
ECHO Building LcmsNet...
MSBUILD /p:Configuration=Debug /p:targetPlatform=x86 %BOPTS% %ROOTPATH%lcms\LCMSNet\LCMSNet.sln
if errorlevel 1 goto :ERROR
ECHO Done.

ECHO Copying Core Validator to %APPDATA%\LCMSNet\SampleValidators
xcopy /y /q %ROOTPATH%SDK\CoreSampleValidator\bin\x86\PNNLRelease\*.dll %APPDATA%\LCMSNet\SampleValidators\
ECHO Done. 
if errorlevel 1 goto :ERROR
goto :COMPLETE

:ERROR
ECHO Error occured during build. See lcmsnetBuild.txt for more details.
goto :END

:COMPLETE
ECHO Cleaning up...
DEL %LOGFILE%
ECHO BUILD COMPLETE.

:END