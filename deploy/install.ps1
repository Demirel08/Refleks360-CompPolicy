#requires -Version 5.1
#requires -RunAsAdministrator
<#
.SYNOPSIS
    Refleks 360 ÜP — Windows kurulum scripti.

.DESCRIPTION
    Bu klasörden çalışan self-contained yayını Windows servis olarak kurar.
    sc.exe ile servis oluşturulur; otomatik başlatma + LocalSystem hesabıyla.

.PARAMETER InstallPath
    Uygulamanın kopyalanacağı yer. Varsayılan: C:\Refleks360.

.PARAMETER ServiceName
    Windows servis adı. Varsayılan: Refleks360.

.PARAMETER Port
    Uygulamanın dinleyeceği HTTP portu. Varsayılan: 5000.
#>

param(
    [string] $InstallPath = "C:\Refleks360",
    [string] $ServiceName = "Refleks360",
    [int]    $Port = 5000
)

$ErrorActionPreference = "Stop"
$src = $PSScriptRoot

Write-Host "==> Refleks 360 ÜP kurulum"
Write-Host "    Kaynak: $src"
Write-Host "    Hedef:  $InstallPath"
Write-Host "    Servis: $ServiceName"
Write-Host "    Port:   $Port"

# 1) Eski servisi durdur
$svc = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($svc) {
    Write-Host "==> Mevcut servis durduruluyor"
    if ($svc.Status -eq 'Running') { Stop-Service $ServiceName }
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

# 2) Hedef klasör
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath | Out-Null
}

Write-Host "==> Dosyalari kopyala"
Copy-Item -Path (Join-Path $src "*") -Destination $InstallPath -Recurse -Force -Exclude @("install.ps1","uninstall.ps1","publish.ps1")

# 3) appsettings.Production.json yoksa template'ten oluştur
$prodSettings = Join-Path $InstallPath "appsettings.Production.json"
$templ = Join-Path $InstallPath "appsettings.Production.template.json"
if (-not (Test-Path $prodSettings) -and (Test-Path $templ)) {
    Copy-Item $templ $prodSettings
    Write-Host ""
    Write-Host "** UYARI: appsettings.Production.json olusturuldu. **"
    Write-Host "    DB connection string, Syncfusion lisansi ve admin sifresini DOLDUR:"
    Write-Host "      $prodSettings"
    Write-Host ""
}

# 4) Servisi olustur
$exe = Join-Path $InstallPath "Refleks360.Web.exe"
if (-not (Test-Path $exe)) { throw "Refleks360.Web.exe bulunamadi: $exe" }

Write-Host "==> Servis olusturuluyor"
$args = "--urls=http://+:$Port"
sc.exe create $ServiceName binPath= "`"$exe`" $args" start= auto DisplayName= "Refleks 360 UP" | Out-Null
sc.exe description $ServiceName "Refleks 360 Ucret Politikasi Yonetim Sistemi" | Out-Null

# Environment variable: Production
[System.Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "Machine")

# 5) Firewall kurali
Write-Host "==> Firewall kurali (port $Port)"
$ruleName = "Refleks360-$Port"
try {
    Get-NetFirewallRule -DisplayName $ruleName -ErrorAction Stop | Out-Null
} catch {
    New-NetFirewallRule -DisplayName $ruleName -Direction Inbound -Action Allow -Protocol TCP -LocalPort $Port | Out-Null
}

# 6) Servisi baslat
Write-Host "==> Servis basliyor"
Start-Service $ServiceName
Start-Sleep -Seconds 3
$svc = Get-Service -Name $ServiceName
Write-Host "    Servis durumu: $($svc.Status)"

Write-Host ""
Write-Host "Tamam. Tarayicidan: http://localhost:$Port"
Write-Host "Loglar: Event Viewer (Windows Logs > Application) veya $InstallPath\logs"
Write-Host ""
Write-Host "Onemli adimlar:"
Write-Host " 1) $prodSettings icindeki ConnectionStrings:Default'u guncelle"
Write-Host " 2) Syncfusion:LicenseKey'i (ucretsiz community license) ekle"
Write-Host " 3) AdminSeed:Password ile guvenli baslangic admin sifresi ata"
Write-Host " 4) HTTPS icin sertifika ekle (Kestrel.Endpoints.Https)"
Write-Host " 5) Servisi yeniden baslat: Restart-Service $ServiceName"
