cls
@echo off

ECHO ------------------------------------------
ECHO      GlucaTrack Windows Service Build
ECHO ------------------------------------------

del *.exe
del BuildTools\BuildLog.txt

if /I "X%1" == "XR" GOTO Release
if /I "X%1" == "XB" GOTO Beta

:QUESTION
set /p BTYPE=(B)eta or (R)elease?

if /I "%BTYPE%" == "R" GOTO Release
if /I "%BTYPE%" == "B" GOTO Beta

:Beta

ECHO Type: BETA

ECHO Cleaning solution
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" /Clean Debug "../GlucaTrack.sln" /out "BuildTools\BuildLog.txt"
ECHO Building solution
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" /Build Debug "../GlucaTrack.sln" /out "BuildTools\BuildLog.txt"
ECHO Building installer
"C:\Program Files (x86)\NSIS\makensis.exe" /DBUILD_MODE=..\..\GlucaTrack.Services.Windows\bin\Debug\ BuildTools\GlucaTrack_WindowsService_Nullsoft.nsi >> "BuildTools\BuildLog.txt"

ECHO Moving installer
move BuildTools\GlucaTrack_Windows_Setup.exe GlucaTrack_Windows_Setup_Beta.exe >> BuildTools\BuildLog.txt

ECHO ----------------------------------------------------
ECHO       End GlucaTrack Windows Service BETA Build     
ECHO ----------------------------------------------------
@echo on

pause

GOTO EXIT

:Release

ECHO Type: RELEASE

ECHO Cleaning solution
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" /Clean Release "../GlucaTrack.sln" /out "BuildTools\BuildLog.txt"
ECHO Building solution
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" /Build Release "../GlucaTrack.sln" /out "BuildTools\BuildLog.txt"
ECHO Building installer
"C:\Program Files (x86)\NSIS\makensis.exe" /DBUILD_MODE=..\GlucaTrack.Services.Windows\bin\Release\ BuildTools\GlucaTrack_WindowsService_Nullsoft.nsi >> "BuildTools\BuildLog.txt"

ECHO Moving installer
move BuildTools\GlucaTrack_Windows_Setup.exe GlucaTrack_Windows_Setup.exe >> BuildTools\BuildLog.txt

ECHO -----------------------------------------------------
ECHO     End GlucaTrack Windows Service RELEASE Build    
ECHO -----------------------------------------------------
@echo on

pause


:EXIT