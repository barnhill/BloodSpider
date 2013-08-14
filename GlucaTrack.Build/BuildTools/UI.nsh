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
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"