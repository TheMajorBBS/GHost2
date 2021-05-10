; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "GHost2"
#define MyAppVersion "1.0"
#define MyAppPublisher "Major BBS"
#define MyAppURL "https://www.themajorbbs.com/"
#define MyAppExeName "GHost.exe"
#define CPU "x86"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AAD4F1D4-708D-48CB-B1A0-DF2B481AD289}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\GHost2
DisableProgramGroupPage=yes
LicenseFile=LICENSE
InfoAfterFile=README.md
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=dist\publish\
OutputBaseFilename=ghost2-{#CPU}-{#MyAppVersion}
SetupIconFile=GHost2\ghost.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\{#CPU}\net5.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\CHANGELOG.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\*.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\*.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\DoorEditor.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\GHost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\GHostConfig.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0-windows\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0\*.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0\*.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#CPU}\net5.0\rlogin.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: bin\Release\{#CPU}\net5.0\ref\*; DestDir: {app}; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0\runtimes\*; DestDir: {app}; Flags: recursesubdirs
; Source: bin\Release\{#CPU}\net5.0\win-{#CPU}\*; DestDir: {app}; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\ansi\*; DestDir: {app}\ansi\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\config\*; DestDir: {app}\config\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\doors\*; DestDir: {app}\doors\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\fixed-events\*; DestDir: {app}\fixed-events\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\logs\*; DestDir: {app}\logs\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\platforms\*; DestDir: {app}\platforms\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\ref\*; DestDir: {app}\ref\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\runtimes\*; DestDir: {app}\runtimes\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\timed-events\*; DestDir: {app}\timed-events\; Flags: recursesubdirs
Source: bin\Release\{#CPU}\net5.0-windows\tools\*; DestDir: {app}\tools\; Flags: recursesubdirs
;Source: bin\Release\{#CPU}\net5.0-windows\win-{#CPU}\*; DestDir: {app}; Flags: recursesubdirs


[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

