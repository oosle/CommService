@echo off

set arch="%~1"
set serv=CommService

if not "%~1"=="" (
   if %arch% == "64" (
      set arch=64-bit
      set util="C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
   ) else (
      set arch=32-bit
      set util="C:\Windows\Microsoft.NET\Framework\v4.0.30319"
   )
)

if not "%~1"=="" (
    echo.
    echo Uninstalling [%arch%] [%serv%]...
    echo.
    %util%\InstallUtil /u ..\%serv%.exe
    net start %serv%
    goto end
)

echo.
echo Uninstalls [%serv%]
echo Syntax  - Uninstall [architecture]
echo Example - Uninstall 64
echo.

:end
