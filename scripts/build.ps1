#Requires -Version 5.1
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot '..\..\Contensive5\scripts\contensive-build.psm1') -Force

$projectRoot = (Resolve-Path "$PSScriptRoot\..").Path

Invoke-ContensiveBuild `
    -CollectionName    'Tool Basics' `
    -CollectionPath    "$projectRoot\Collections\Tool Basics" `
    -SolutionPath      "$projectRoot\server\tools\Tools.sln" `
    -BinPath           "$projectRoot\server\tools\bin\Debug" `
    -DeploymentRoot    'C:\Deployments\aoTools' `
    -Configuration     'Debug' `
    -CleanFolders      @(
                           "$projectRoot\server\tools\bin"
                           "$projectRoot\server\tools\obj"
                       ) `
    -UiPath            "$projectRoot\ui" `
    -DotnetProjectPath "$projectRoot\server\tools\Tools.csproj"
