#define MyAppName "Import Studio"
#define MyAppVersion "1.8.13"
#define MyAppPublisher "mat1jaczyyy"
#define MyAppURL "import.mat1jaczyyy.com"
#define MyAppExeName "Import.exe"

[Setup]
AppId={BE7DB952-7C93-4DF7-B256-3C14F64088CF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=auto
DisableProgramGroupPage=yes
UsedUserAreasWarning=no
LicenseFile=..\LICENSE
OutputDir=..\Dist\
OutputBaseFilename=Import-{#MyAppVersion}-Win
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ChangesAssociations=yes
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "clearpreferences"; Description: "Clear Preferences"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "novationusbmidi.exe"; DestDir: {tmp}; Flags: deleteafterinstall
Source: "..\Build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Registry]
Root: HKLM; Subkey: "Software\Classes\.approj"; ValueType: string; ValueName: ""; ValueData: "ImportStudioProject"; Flags: uninsdeletevalue 
Root: HKLM; Subkey: "Software\Classes\ImportStudioProject"; ValueType: string; ValueName: ""; ValueData: "Import Studio Project"; Flags: uninsdeletekey 
Root: HKLM; Subkey: "Software\Classes\ImportStudioProject\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Import\{#MyAppExeName},0" 
Root: HKLM; Subkey: "Software\Classes\ImportStudioProject\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Import\{#MyAppExeName}"" ""%1""" 
Root: HKLM; Subkey: "Software\Classes\.aptrk"; ValueType: string; ValueName: ""; ValueData: "ImportStudioTrack"; Flags: uninsdeletevalue 
Root: HKLM; Subkey: "Software\Classes\ImportStudioTrack"; ValueType: string; ValueName: ""; ValueData: "Import Studio Track"; Flags: uninsdeletekey 
Root: HKLM; Subkey: "Software\Classes\ImportStudioTrack\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Import\{#MyAppExeName},0" 
Root: HKLM; Subkey: "Software\Classes\ImportStudioTrack\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Import\{#MyAppExeName}"" ""%1""" 
Root: HKLM; Subkey: "Software\Classes\.apchn"; ValueType: string; ValueName: ""; ValueData: "ImportStudioChain"; Flags: uninsdeletevalue 
Root: HKLM; Subkey: "Software\Classes\ImportStudioChain"; ValueType: string; ValueName: ""; ValueData: "Import Studio Chain"; Flags: uninsdeletekey 
Root: HKLM; Subkey: "Software\Classes\ImportStudioChain\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Import\{#MyAppExeName},0" 
Root: HKLM; Subkey: "Software\Classes\ImportStudioChain\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Import\{#MyAppExeName}"" ""%1""" 
Root: HKLM; Subkey: "Software\Classes\.apdev"; ValueType: string; ValueName: ""; ValueData: "ImportStudioDevice"; Flags: uninsdeletevalue 
Root: HKLM; Subkey: "Software\Classes\ImportStudioDevice"; ValueType: string; ValueName: ""; ValueData: "Import Studio Device"; Flags: uninsdeletekey 
Root: HKLM; Subkey: "Software\Classes\ImportStudioDevice\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Import\{#MyAppExeName},0" 
Root: HKLM; Subkey: "Software\Classes\ImportStudioDevice\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Import\{#MyAppExeName}"" ""%1""" 

[InstallDelete]
Type: files; Name: "{%USERPROFILE}\.importstudio\Import.config"; Tasks: clearpreferences

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\Import\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\Import\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\novationusbmidi.exe"; StatusMsg: Installing Novation USB Driver...
Filename: "{app}\Import\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent