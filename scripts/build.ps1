#Requires -Version 5.1
[CmdletBinding()]
param(
    [string]   $LocalDeployTarget  = '',
    [hashtable]$RemoteDeployTarget = $null
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot '..\..\Contensive5\scripts\contensive-build.psm1') -Force

$projectRoot = (Resolve-Path "$PSScriptRoot\..").Path

Invoke-ContensiveBuild `
    -CollectionName    'Tool Basics' `
    -CollectionPath    "$projectRoot\Collections\Tool Basics" `
    -SolutionPath      "$projectRoot\server\tools\Tools.sln" `
    -BinPath           "$projectRoot\server\tools\bin\Release\netstandard2.0" `
    -DeploymentRoot    'C:\Deployments\aoTools' `
    -CleanFolders      @(
                           "$projectRoot\server\tools\bin"
                           "$projectRoot\server\tools\obj"
                       ) `
    -UiPath            "$projectRoot\ui" `
    -LocalDeployTarget  $LocalDeployTarget `
    -RemoteDeployTarget $RemoteDeployTarget
