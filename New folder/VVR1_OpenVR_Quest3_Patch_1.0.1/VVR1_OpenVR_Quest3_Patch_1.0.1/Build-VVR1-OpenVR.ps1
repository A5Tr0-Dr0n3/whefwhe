[CmdletBinding()]
param(
    [string]$ProjectRoot = 'E:\A5Tr0\Documents\GitHub\whefwhe\New folder (2)',
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

function Find-MSBuild {
    $command = Get-Command 'msbuild.exe' -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }

    $known = @(
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'),
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe'),
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe')
    )
    foreach ($candidate in $known) {
        if ($candidate -and (Test-Path -LiteralPath $candidate -PathType Leaf)) { return $candidate }
    }

    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $found = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
            Select-Object -First 1
        if ($found -and (Test-Path -LiteralPath $found -PathType Leaf)) { return $found }
    }

    throw 'MSBuild was not found. Visual Studio 2026/2022 with MSBuild is required.'
}

if (-not (Test-Path -LiteralPath $ProjectRoot -PathType Container)) {
    throw "Project folder not found: $ProjectRoot"
}
$ProjectRoot = (Resolve-Path -LiteralPath $ProjectRoot).Path

$projectFiles = @(Get-ChildItem -LiteralPath $ProjectRoot -Filter '*.csproj' -File -Recurse |
    Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' -and $_.Name -like '*Assembly-CSharp*' })
if ($projectFiles.Count -eq 0) {
    $projectFiles = @(Get-ChildItem -LiteralPath $ProjectRoot -Filter '*.csproj' -File -Recurse |
        Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' })
}
if ($projectFiles.Count -ne 1) {
    $names = ($projectFiles | ForEach-Object { $_.FullName }) -join "`n"
    throw "Expected one source project; found $($projectFiles.Count).`n$names"
}
$project = $projectFiles[0]

$patchSource = Join-Path $PSScriptRoot 'TButt.InputMaps\TBSteamVRInput.cs'
if (-not (Test-Path -LiteralPath $patchSource -PathType Leaf)) {
    throw "Patch source not found: $patchSource. Extract the complete ZIP before running this script."
}

$targetSource = Join-Path $ProjectRoot 'TButt.InputMaps\TBSteamVRInput.cs'
if (-not (Test-Path -LiteralPath $targetSource -PathType Leaf)) {
    $matches = @(Get-ChildItem -LiteralPath $ProjectRoot -Filter 'TBSteamVRInput.cs' -File -Recurse |
        Where-Object { $_.FullName -notmatch 'VVR1Patch_Backup|VVR1_OpenVR_SourceBackups|\\(obj|bin)\\' })
    if ($matches.Count -ne 1) {
        throw "Could not uniquely locate TButt.InputMaps\TBSteamVRInput.cs under $ProjectRoot. No source was changed."
    }
    $targetSource = $matches[0].FullName
}

# Keep source backups outside the project tree so wildcard Compile includes
# cannot accidentally compile backup copies and trigger duplicate-type errors.
$backupRoot = Join-Path (Split-Path -Parent $ProjectRoot) 'VVR1_OpenVR_SourceBackups'
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
$backupFile = Join-Path $backupRoot ("TBSteamVRInput_{0}.cs" -f (Get-Date -Format 'yyyyMMdd_HHmmss'))
Copy-Item -LiteralPath $targetSource -Destination $backupFile -Force
Copy-Item -LiteralPath $patchSource -Destination $targetSource -Force

Write-Host "Patched source: $targetSource" -ForegroundColor Cyan
Write-Host "Original source backup: $backupFile" -ForegroundColor Yellow
Write-Host "Project: $($project.FullName)"

$msbuild = Find-MSBuild
$outputDir = Join-Path $PSScriptRoot 'BuildOutput'
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
$logFile = Join-Path $outputDir 'MSBuild.log'

Write-Host "`nBuilding with $msbuild ..." -ForegroundColor Cyan
& $msbuild $project.FullName /t:Rebuild "/p:Configuration=$Configuration" /verbosity:minimal 2>&1 |
    Tee-Object -FilePath $logFile
$buildExitCode = $LASTEXITCODE
if ($buildExitCode -ne 0) {
    throw "MSBuild failed with exit code $buildExitCode. No game DLL was installed. Build log: $logFile. Original source backup: $backupFile"
}

$builtDlls = @(Get-ChildItem -LiteralPath $project.DirectoryName -Filter 'Assembly-CSharp.dll' -File -Recurse |
    Where-Object { $_.FullName -notmatch 'VVR1Patch_Backup|VVR1_OpenVR_SourceBackups|\\obj\\' } |
    Sort-Object LastWriteTime -Descending)
if ($builtDlls.Count -eq 0) {
    throw "MSBuild succeeded but no Assembly-CSharp.dll was found outside obj/bin. Check the project output path. Log: $logFile"
}

$builtOutput = Join-Path $outputDir 'Assembly-CSharp.dll'
Copy-Item -LiteralPath $builtDlls[0].FullName -Destination $builtOutput -Force
Write-Host "`nBuild succeeded." -ForegroundColor Green
Write-Host "Built assembly copied to: $builtOutput" -ForegroundColor Green
Write-Host 'This script does not install or overwrite the game DLL.' -ForegroundColor Yellow
