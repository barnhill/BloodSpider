!include "MUI2.nsh"
!include WordFunc.nsh
!insertmacro VersionCompare
!include LogicLib.nsh
!include Sections.nsh
!include "Plugins\DotNetVer.nsh"
!include "FileFunc.nsh"
!include "Plugins\nsProcess.nsh"
!include "Variables.nsh"
!include "UI.nsh"

CRCCheck force
;TargetMinimalOS 6.0    
BrandingText "Copyright © GlucaTrack"

; ----------------------------------------------------------------------------------
; *************************** SECTION FOR INSTALLING *******************************
; ----------------------------------------------------------------------------------
Name "${PRODUCT_NAME}"
OutFile "${PRODUCT_PUBLISHER}_Windows_Setup.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_PUBLISHER}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" "$INSTDIR"
ShowInstDetails show
ShowUnInstDetails show

;Uninstall icon
UninstallIcon "${RESOURCES}${ICON_FILENAME}"
  
Var InstallDotNET

Function .onInit
    
   ;closes the notify icon
   ${nsProcess::KillProcess} "${PRODUCT_MAIN_EXE}${EXE_EXTENSION}" $R0
   ${nsProcess::Unload}

   ;remove the service
   ExecWait "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION} -uninstall"

   # call userInfo plugin to get user info.  The plugin puts the result in the stack
   userInfo::getAccountType

   # pop the result from the stack into $0
   pop $0

   # compare the result with the string "Admin" to see if the user is admin.
   strCmp $0 "Admin" IsAdmin NotAdmin
   messageBox MB_OK $0

   # if there is not a match, print message and return
   NotAdmin:
     messageBox MB_OK "You Must be logged in as Administrator! $0"
     Quit

   # otherwise, confirm and return
   IsAdmin:
	
  !insertmacro MUI_LANGDLL_DISPLAY

  ; ========== Check .NET version ==========
  ${If} ${HasDotNet4.0}
  ${Else}
	MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the .NET Framework ${REQUIRED_DOTNET_VERSION} is installed.$\n$\n The .NET Framework will be downloaded and installed automatically during installation of ${PRODUCT_NAME}."
	Return
  ${EndIf}

FunctionEnd


Section "${PRODUCT_NAME}" SEC01
  
  SectionIn RO ;Make section required in components page

  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer

  ; Get .NET if required
  ${If} ${HasDotNet4.0}
  ${Else}
    SetDetailsView hide
    inetc::get /caption "Downloading .NET Framework 4.0" /canceltext "Cancel" "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe" "$INSTDIR\dotnetfx.exe" /end
    Pop $1

    ${If} $1 != "OK"
      Delete "$INSTDIR\dotnetfx.exe"
      Abort "Installation cancelled."
    ${EndIf}

    ExecWait "$INSTDIR\dotnetfx.exe"
    Delete "$INSTDIR\dotnetfx.exe"
     
    ; Re-Check .NET version and if still not installer the user must have cancelled the installation ... so exit
    StrCpy $InstallDotNET "No"

	${If} ${HasDotNet4.0}
	${Else}
        StrCpy $InstallDotNET "Yes"
		MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the .NET Framework ${REQUIRED_DOTNET_VERSION} is installed.$\n$\n The installer will now exit."
        RMDir "$INSTDIR"
        Quit
    ${EndIf}

  ${EndIf}

SetOverwrite try
  SetOutPath "$INSTDIR"
  File "${SOURCEDIR}\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION}"
  File "${SOURCEDIR}\GlucaTrack.Communication.dll"
  File "${SOURCEDIR}\GlucaTrack.Services.Common.dll"
  File "${SOURCEDIR}\GlucaTrack.Services.Windows.exe.config"
  File "${SOURCEDIR}\UsbLibrary.dll"
  File "${SOURCEDIR}\..\..\..\GlucaTrack.Services.Windows.NotifyIcon\bin\Debug\${PRODUCT_MAIN_EXE}${EXE_EXTENSION}"

  ;Create the program  data folder
  CreateDirectory "$APPDATA\${PRODUCT_NAME}"
SectionEnd

;set the descriptions for the sections
LangString DESC_Section1 ${LANG_ENGLISH} "${PRODUCT_NAME}"

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} $(DESC_Section1)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Section -Post
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION}"
  WriteRegStr ${HKEY_CURRENT_USER} "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "NoModify" "1"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "NoRepair" "1"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "RegCompany" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "RegOwner" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegDWORD ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "EstimatedSize" "$0"

  ;start on windows startup
  WriteRegStr ${HKEY_CURRENT_USER} "${WINDOWS_AUTORUN_KEY}" "${PRODUCT_NAME}" "$INSTDIR\${PRODUCT_MAIN_EXE}${EXE_EXTENSION}"

  ;start service and notify icon after install
  Exec "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION} -install"
  Exec "$INSTDIR\${PRODUCT_MAIN_EXE}${EXE_EXTENSION}"
SectionEnd

Function .onInstSuccess
  HideWindow

  ReadRegStr $2 ${HKEY_LOCAL_MACHINE} "${PRODUCT_DIR_REGKEY}" ""

  ${If} ${ALREADYINSTALLED} != "1"
    Abort
  ${EndIf}

  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully installed."
FunctionEnd

; ----------------------------------------------------------------------------------
; ************************** SECTION FOR UNINSTALLING ******************************
; ----------------------------------------------------------------------------------
Function un.onUninstSuccess
  ;HideWindow
  ;MessageBox MB_ICONQUESTION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Section Uninstall
  ;closes the notify icon
  ${nsProcess::KillProcess} "${PRODUCT_MAIN_EXE}${EXE_EXTENSION}" $R0
  ${nsProcess::Unload}

  ;remove the service
  ExecWait "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION} -uninstall"
    
  Delete "$INSTDIR\${PRODUCT_SERVICE_EXE}${EXE_EXTENSION}"
  Delete "$INSTDIR\GlucaTrack.Communication.dll"
  Delete "$INSTDIR\GlucaTrack.Services.Common.dll"
  Delete "$INSTDIR\GlucaTrack.Services.Windows.exe.config"
  Delete "$INSTDIR\UsbLibrary.dll"
  Delete "$INSTDIR\${PRODUCT_MAIN_EXE}${EXE_EXTENSION}"

  Delete "$INSTDIR\GlucaTrack.Services.Windows.InstallLog"
  Delete "$INSTDIR\uninst.exe"
	
  ;Remove Installation Directory
  RMDir $INSTDIR

  ;Remove APPDATA saved settings and temporary files
  RMDir /r "$APPDATA\${PRODUCT_NAME}"

  DeleteRegKey ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey ${HKEY_LOCAL_MACHINE} "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey ${HKEY_CURRENT_USER} "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

; ----------------------------------------------------------------------------------
; ********************** SECTION FOR SUPPORTING FUNCTIONS **************************
; ----------------------------------------------------------------------------------
Function Update_Pre
  ReadRegStr $2 ${HKEY_CURRENT_USER} "${PRODUCT_DIR_REGKEY}" ""

  ${If} $2 != ""
    Abort
  ${EndIf}
FunctionEnd 

Function Update_Pre_Finish
  ReadRegStr $2 ${HKEY_CURRENT_USER} "${PRODUCT_DIR_REGKEY}" ""

  ${If} $2 != ""
    Abort
  ${EndIf}
FunctionEnd



