@echo off
setlocal
@echo Build aoTools
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1"
if not "%1"=="/nopause" pause
