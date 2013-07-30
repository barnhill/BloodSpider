!include "MUI2.nsh"
!include WordFunc.nsh
!insertmacro VersionCompare
!include LogicLib.nsh
!include Sections.nsh
!include "DotNetVer.nsh"
!include "FileFunc.nsh"

!define PRODUCT_NAME "GlucaTrack Windows Service"
!define PRODUCT_PUBLISHER "GlucaTrack"
!define PRODUCT_SERVICE_EXE "GlucaTrack.Services.Windows.exe"
!define PRODUCT_MAIN_EXE "GlucaTrack.Services.Windows.NotifyIcon.exe"
!define PRODUCT_WEB_SITE "http://www.${PRODUCT_PUBLISHER}.com"
!define RESOURCES "..\"
!define SOURCEDIR "${BUILD_MODE}"
!define REQUIRED_DOTNET_VERSION "4.0"
!define ICON_FILENAME "icon.ico"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${PRODUCT_SERVICE_EXE}"
!define PRODUCT_GUID_KEY "Software\${PRODUCT_NAME}"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define HKEY_LOCAL_MACHINE "HKLM"

CRCCheck force
;SetCompressor /SOLID lzma
BrandingText "Copyright © GlucaTrack"

; ----------------------------------------------------------------------------------
; ****************************** SECTION FOR MUI ***********************************
; ----------------------------------------------------------------------------------

!define MUI_ABORTWARNING
!define MUI_ICON "${RESOURCES}${ICON_FILENAME}"
!define MUI_UNICON "${RESOURCES}${ICON_FILENAME}"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "${RESOURCES}NSIS_installheader.bmp"
!define MUI_HEADERIMAGE_UNBITMAP "${RESOURCES}NSIS_installheader.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${RESOURCES}NSIS_WelcomeImage.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${RESOURCES}NSIS_WelcomeImage.bmp"

; ----------------------------------------------------------------------------------
; *********************************** PAGES ****************************************
; ----------------------------------------------------------------------------------

; Welcome page
!define MUI_PAGE_CUSTOMFUNCTION_PRE Update_Pre
!define MUI_WELCOMEPAGE_TITLE 'Welcome to the ${PRODUCT_NAME} Setup'
!insertmacro MUI_PAGE_WELCOME

; License page
!define MUI_PAGE_CUSTOMFUNCTION_PRE Update_Pre
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "${RESOURCES}EULA.txt"

; Instfiles page
!insertmacro MUI_PAGE_INSTFILES

; Finish page
!define MUI_PAGE_CUSTOMFUNCTION_PRE Update_Pre_Finish
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; ----------------------------------------------------------------------------------
; *************************** SECTION FOR INSTALLING *******************************
; ----------------------------------------------------------------------------------

Name "${PRODUCT_NAME}"
OutFile "${PRODUCT_PUBLISHER}_Windows_Setup.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_PUBLISHER}\${PRODUCT_NAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" "$INSTDIR"
ShowInstDetails show
ShowUnInstDetails show

;Uninstall icon
UninstallIcon "${RESOURCES}${ICON_FILENAME}"
  
Var InstallDotNET

Function .onInit
    
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
        RMDir "$PROGRAMFILES\${PRODUCT_PUBLISHER}"
        Quit
    ${EndIf}

  ${EndIf}

SetOverwrite try
  SetOutPath "$INSTDIR"
  File "${SOURCEDIR}\${PRODUCT_SERVICE_EXE}"
  File "${SOURCEDIR}\GlucaTrack.Communication.dll"
  File "${SOURCEDIR}\GlucaTrack.Services.Common.dll"
  File "${SOURCEDIR}\GlucaTrack.Services.Windows.exe.config"
  File "${SOURCEDIR}\UsbLibrary.dll"
  File "${SOURCEDIR}\..\..\..\GlucaTrack.Services.Windows.NotifyIcon\bin\Debug\${PRODUCT_MAIN_EXE}"

  ;start service and notify icon after install
  Exec "$INSTDIR\${PRODUCT_SERVICE_EXE} -install"
  Exec "$INSTDIR\${PRODUCT_MAIN_EXE}"
SectionEnd

;set the descriptions for the sections
LangString DESC_Section1 ${LANG_ENGLISH} "GlucaTrack Windows Service"

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} $(DESC_Section1)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Section -Post
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${PRODUCT_SERVICE_EXE}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${PRODUCT_SERVICE_EXE}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "NoModify" "1"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "NoRepair" "1"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "RegCompany" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "RegOwner" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegDWORD ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}" "EstimatedSize" "$0"
SectionEnd

Function .onInstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully installed."
FunctionEnd

; ----------------------------------------------------------------------------------
; ************************** SECTION FOR UNINSTALLING ******************************
; ----------------------------------------------------------------------------------
Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  ;closes the notify icon
  ExecWait "$INSTDIR\${PRODUCT_MAIN_EXE} -kill"

  ;remove the service
  ExecWait "$INSTDIR\${PRODUCT_SERVICE_EXE} -uninstall"
    
  Delete "$INSTDIR\${PRODUCT_SERVICE_EXE}"
  Delete "$INSTDIR\GlucaTrack.Communication.dll"
  Delete "$INSTDIR\GlucaTrack.Services.Common.dll"
  Delete "$INSTDIR\GlucaTrack.Services.Windows.exe.config"
  Delete "$INSTDIR\UsbLibrary.dll"
  Delete "$INSTDIR\${PRODUCT_MAIN_EXE}"
	
  ;remove installation folder
  RMDir /r $INSTDIR

  ;Remove APPDATA xml files and cars directory
  Delete "$APPDATA\${PRODUCT_NAME}\glucatrack.sav"
  RMDir /r "$APPDATA\${PRODUCT_NAME}"

  ;Remove Installation Directory
  RMDir "$INSTDIR"

  DeleteRegKey ${HKEY_LOCAL_MACHINE} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey ${HKEY_LOCAL_MACHINE} "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

; ----------------------------------------------------------------------------------
; ********************** SECTION FOR SUPPORTING FUNCTIONS **************************
; ----------------------------------------------------------------------------------
Function Update_Pre
  ReadRegStr $2 HKLM "${PRODUCT_DIR_REGKEY}" ""

  ${If} $2 != ""
    Abort
  ${EndIf}
FunctionEnd 

Function Update_Pre_Finish
  ReadRegStr $2 HKLM "${PRODUCT_DIR_REGKEY}" ""

  ${If} $2 != ""
    Abort
  ${EndIf}
FunctionEnd

