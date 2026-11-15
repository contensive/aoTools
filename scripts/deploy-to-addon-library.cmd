@echo off
setlocal

rem Uploads the built collection to the Addon Collection Library on contensive.com.
rem TODO: verify CollectionName, CollectionPath, and DeploymentPath are correct for this repo.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0deploy-to-addon-library.ps1" ^
    -CollectionName "Tool Basics" ^
    -CollectionPath "%~dp0..\collections\Tool Basics" ^
    -DeploymentPath "C:\Deployments\aoTools" ^
    -UiPath "%~dp0..\ui"

set exitCode=%errorlevel%
pause
exit /b %exitCode%