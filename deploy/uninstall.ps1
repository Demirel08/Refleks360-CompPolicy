#requires -Version 5.1
#requires -RunAsAdministrator

param(
    [string] $InstallPath = "C:\Refleks360",
    [string] $ServiceName = "Refleks360",
    [int]    $Port = 5000,
    [switch] $KeepData
)

$ErrorActionPreference = "Continue"
Write-Host "==> Refleks 360 UP kaldiriliyor"

$svc = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($svc) {
    if ($svc.Status -eq 'Running') { Stop-Service $ServiceName -Force }
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

$ruleName = "Refleks360-$Port"
try { Remove-NetFirewallRule -DisplayName $ruleName -ErrorAction Stop } catch {}

if (-not $KeepData -and (Test-Path $InstallPath)) {
    Remove-Item -Recurse -Force $InstallPath
    Write-Host "Klasor silindi: $InstallPath"
} elseif ($KeepData) {
    Write-Host "Klasor korunuyor (data icin -KeepData): $InstallPath"
}

Write-Host "Tamam. Veritabani manuel silinmeli (DROP DATABASE Refleks360)."
