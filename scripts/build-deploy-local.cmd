@echo off
setlocal

set defaultsFile=%LOCALAPPDATA%\contensive\local-deploy-appname.txt

rem load saved default app name if it exists
set savedApp=
if exist "%defaultsFile%" (
    set /p savedApp=<"%defaultsFile%"
)

rem prompt for app name, showing saved default
if defined savedApp (
    set /p appName="Enter local site name [%savedApp%]: "
) else (
    set /p appName="Enter local site name: "
)

rem if nothing entered, use saved default
if "%appName%"=="" (
    if defined savedApp (
        set appName=%savedApp%
    ) else (
        echo No site name entered.
        pause
        exit /b 1
    )
)

rem save the app name for next time (shared across all addons)
if not exist "%LOCALAPPDATA%\contensive" mkdir "%LOCALAPPDATA%\contensive"
echo %appName%> "%defaultsFile%"

@echo Build project and install on site: %appName%
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "& '%~dp0build.ps1' -LocalDeployTarget '%appName%'"
set deployExit=%errorlevel%
pause
exit /b %deployExit%
