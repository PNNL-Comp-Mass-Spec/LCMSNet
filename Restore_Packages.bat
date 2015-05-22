rem This batch file is used by Jenkins to restore packages prior to building the solutions
nuget.exe restore LcmsNet\SDK\LcmsNetSDKs.sln
nuget.exe restore LcmsNet\SQLiteTools\SQLiteTools.sln
nuget.exe restore LcmsNetPlugins\lcmsnetPlugins.sln
nuget.exe restore LcmsNetDmsTools\LcmsNetDmsTools.sln
nuget.exe restore LcmsNet\lcms\LCMSnet\LCMSNet.sln
