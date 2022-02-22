; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
                                                           
#define MyAppVersion GetFileVersion('..\LCMSNetProg\bin\x86\Release\LcmsNet.exe')
#define MySource "..\"
#define MyLib    "lib"
#define MyExtLib  "ExtLib"
#define MyPlugins "PluginDlls" 
#define MyValidators "SampleValidators"
#define MyAppName "LCMSNet"
#define MyAppDataPath "C:\LCMSNet"
#define MyAppVis  "PNNL"
#define MyAppPublisher "Battelle"
#define MyAppExeName "LcmsNet.exe"  
#define MyDateTime GetDateTimeString('mm_dd_yyyy', "_","_");

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7A59E1E4-236D-47F7-996E-F9888D99F017}
AppName={#MyAppName}
AppVersion={#MyAppVersion} 
;AppVerName={#MyAppVerName} ; Using this overrides AppName
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Installer\Output
OutputBaseFilename={#MyAppName}_{#MyAppVis}_{#MyAppVersion}_{#MyDateTime}
SourceDir={#MySource}
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Dirs]
Name: "{#MyAppDataPath}\Log"
Name: "{app}\SampleValidators"
Name: "{app}\Plugins"
Name: "{app}\x86"
Name: "{app}\x64"

[Files]
; Exe
Source: LCMSNetProg\bin\x86\Release\LcmsNet.exe;                               DestDir: "{app}";          Flags: ignoreversion

; Application configuration: binding redirects and default settings. ConfirmOverwrite is a reminder that it contains machine-specific settings on older versions, that should be backed up and re-set (using LcmsNet_PersistentSettings.config)
; The ConfirmOverwrite flag can undoubtedly be removed at some future point, when the older versions of LcmsNet are no longer in use.
Source: LCMSNetProg\bin\x86\Release\LcmsNet.exe.config;                        DestDir: "{app}";          Flags: ignoreversion confirmoverwrite

; Nuget DLLs
Source: LCMSNetProg\bin\x86\Release\CsvHelper.dll;                             DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\DynamicData.dll;                           DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\Microsoft.Bcl.AsyncInterfaces.dll;         DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\Microsoft.WindowsAPICodePack*.dll;         DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\OxyPlot*.dll;                              DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\Reactive*.dll;                             DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\Splat*.dll;                                DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\System.Data.SQLite.dll;                    DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\System.Reactive*.dll;                      DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\System.Runtime.CompilerServices.Unsafe.dll; DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\System.Threading.Tasks.Extensions.dll;     DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\System.ValueTuple.dll;                     DestDir: "{app}";          Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\Xceed.Wpf*.dll;                            DestDir: "{app}";          Flags: ignoreversion

; DLLs are copied below in the "DLLs" section, sourced from {#MyLib}\*

; Copy SQLite.Interop.dll (both x86 and x64, even though we're likely compiling against x86)
Source: LCMSNetProg\bin\x86\Release\x86\*.dll;                                 DestDir: "{app}\x86";      Flags: ignoreversion
Source: LCMSNetProg\bin\x86\Release\x64\*.dll;                                 DestDir: "{app}\x64";      Flags: ignoreversion

Source: "LCMSNetProg\PrismDMS.config";                                         DestDir: "{app}";          Flags: ignoreversion

;DLLs
Source: "{#MyLib}\*.dll";                                                      DestDir: "{app}";          Flags: ignoreversion
Source: "{#MyExtLib}\*.dll";                                                   DestDir: "{app}";          Flags: ignoreversion

;Plugins
Source: "{#MyPlugins}\*.dll";                                                  DestDir: "{app}\Plugins\"; Flags: ignoreversion
Source: "{#MyPlugins}\x86\*.dll";                                              DestDir: "{app}\Plugins\x86\"; Flags: ignoreversion
Source: "{#MyPlugins}\x64\*.dll";                                              DestDir: "{app}\Plugins\x64\"; Flags: ignoreversion
Source: "Plugins\PALAutoSampler\paldriv.exe";                                  DestDir: "{sys}";          Flags: ignoreversion

;SQLite Database Log Viewer program
Source: "ExternalApplications\LogViewer\bin\Release\*.exe";                    DestDir: "{app}";          Flags: ignoreversion

; PERSISTED SETTINGS FILE-------------------------------------------------------------------------------------------------------------------------------------------------------------
; Only copy it once, never overwrite the existing file, and don't remove it during an uninstall
Source: LCMSNetProg\bin\x86\Release\LcmsNet_PersistentSettings.config;         DestDir: "{app}";          Flags: ignoreversion onlyifdoesntexist uninsneveruninstall
; END SETTINGS FILE---------------------------------------------------------------------------------------------------------------------------------------------------------


[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon
;Name: "{commondesktop}\{#MyAppName}-LogViewer"; Filename: "{app}\LogViewer.exe"; WorkingDir:"{userappdata}\LCMSNet\Log"; Tasks: desktopicon

[Run]
;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
Filename: "{sys}\paldriv.exe"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
var
    key: string;
    NetFrameWorkInstalled : Boolean;
    release, minVersion: cardinal;
begin
    // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
    // http://kynosarges.org/DotNetVersion.html
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
    minVersion := 461808;
    Result := false;
    NetFrameWorkInstalled := RegKeyExists(HKLM, key);
    if NetFrameWorkInstalled =true then
    begin
        Result := RegQueryDWordValue(HKLM, key, 'Release', release);
        Result := Result and (release >= minVersion);
    end;

    if Result =false then
    begin
        MsgBox('This setup requires the .NET Framework version 4.7.2. Please install the .NET Framework and run this setup again.',
            mbInformation, MB_OK);
    end;
end;


