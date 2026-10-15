<#
.SYNOPSIS
    Downloads and installs the latest BoTech.ApiClient.Base.Editor build.

.DESCRIPTION
    1. Downloads latest.zip into the parent directory of the current/target directory.
    2. Renames that directory to "old".
    3. Extracts the zip into a "latest" directory in the parent directory.
    4. Deletes the downloaded zip.
    5. Deletes the "old" directory - but ONLY if every previous step succeeded.

.PARAMETER TargetDir
    The directory to replace. Defaults to the current working directory.
#>

param(
    [string]$TargetDir = (Get-Location).Path,
    [string]$DownloadUrl = "https://assets.botech.dev/static-app-update/BoTech.ApiClient.Base.Editor/latest.zip"
)

$ErrorActionPreference = "Stop"

function Write-Step($msg) { Write-Host "==> $msg" -ForegroundColor Cyan }
function Write-Ok($msg)   { Write-Host "    OK: $msg" -ForegroundColor Green }
function Write-Fail($msg) { Write-Host "    FAILED: $msg" -ForegroundColor Red }

$currentDir = (Resolve-Path -Path $TargetDir).Path.TrimEnd('\')
$zipPath    = Join-Path $currentDir "latest.zip"
$oldPath    = Join-Path $currentDir "old"
$latestPath = Join-Path $currentDir "latest"

if (Test-Path $oldPath) {
    Write-Fail "'$oldPath' already exists. Remove or rename it before running this script."
    exit 1
}

$zipDownloaded = $false
$dirRenamed    = $false

try {
    # 1. Download latest.zip into the parent directory
    Write-Step "Downloading $DownloadUrl -> $zipPath"
    Invoke-WebRequest -Uri $DownloadUrl -OutFile $zipPath -UseBasicParsing
    $zipDownloaded = $true
    Write-Ok "Downloaded $((Get-Item $zipPath).Length) bytes"

    # 2. Rename the current directory to "old"
    Write-Step "Renaming '$currentDir' -> '$oldPath'"
    #Set-Location $currentDir   # release this process's lock on $currentDir first
    Rename-Item -Path "latest" -NewName "old"
    $dirRenamed = $true
    Write-Ok "Renamed"

    # 3. Extract the zip into "latest"
    Write-Step "Extracting '$zipPath' -> '$latestPath'"
    Expand-Archive -Path $zipPath -DestinationPath $latestPath -Force
    Write-Ok "Extracted"

    # 4. Delete the downloaded zip
    Write-Step "Deleting '$zipPath'"
    Remove-Item -Path $zipPath -Force
    $zipDownloaded = $false
    Write-Ok "Deleted zip"

    # 5. Only now that everything above succeeded, delete "old"
    Write-Step "Deleting '$oldPath'"
    Remove-Item -Path $oldPath -Recurse -Force
    Write-Ok "Deleted old"

    Write-Host "`nUpdate completed successfully." -ForegroundColor Green
    Write-Host "You can restart the application now!!!" -ForegroundColor Green
    Write-Host "Closing in 20 seconds...."
    Start-Sleep -Seconds 20
    exit 0
}
catch {
    Write-Fail $_.Exception.Message
    Write-Host "`nSomething went wrong. Nothing further was deleted automatically." -ForegroundColor Yellow
    if ($dirRenamed)    { Write-Host "Your previous version is safe at: $oldPath" -ForegroundColor Yellow }
    if ($zipDownloaded -and (Test-Path $zipPath)) { Write-Host "The downloaded zip is still at: $zipPath" -ForegroundColor Yellow }
    exit 1
}