SetCompressor /SOLID lzma

!define PRODUCT_VERSION "1.0.0.0"
!define PRODUCT_NAME "GlucaTrack"
!define PRODUCT_PUBLISHER "GlucaTrack"
!define PRODUCT_SERVICE_EXE "GlucaTrack.Services.Windows"
!define PRODUCT_MAIN_EXE "GlucaTrack.Services.Windows.NotifyIcon"
!define EXE_EXTENSION ".exe"
!define PRODUCT_WEB_SITE "http://www.${PRODUCT_PUBLISHER}.com"
!define RESOURCES "..\Resources\"
!define SOURCEDIR "${BUILD_MODE}"
!define REQUIRED_DOTNET_VERSION "4.0"
!define ICON_FILENAME "icon.ico"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${PRODUCT_NAME}"
!define PRODUCT_GUID_KEY "Software\${PRODUCT_NAME}"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define WINDOWS_AUTORUN_KEY "Software\Microsoft\Windows\CurrentVersion\Run"
!define HKEY_LOCAL_MACHINE "HKLM"
!define HKEY_CURRENT_USER "HKCU"

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