@echo off

set arch="%~1"
set user="%~2"
set pass="%~3"
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

if not "%~1"=="" if not "%~2"=="" if not "%~3"=="" (
    echo.
    echo Installing [%arch%] [%serv%]...
    echo.
    %util%\InstallUtil /user=%user% /password=%pass% /unattended /i ..\%serv%.exe
    net start %serv%
    goto end
)

echo.
echo Installs [%serv%]
echo Syntax - Install [architecture] [username] [password]
echo Domain Account - Install 64 "domain\user" "password"
echo Local Account  - Install 64 "." "."
echo.

:end
