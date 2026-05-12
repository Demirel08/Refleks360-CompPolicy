#requires -Version 5.1
<#
.SYNOPSIS
    Refleks 360 ÜP self-contained publish + ZIP paketi.

.DESCRIPTION
    Windows için self-contained yayını üretir (dotnet runtime gerekmez) ve
    deploy/dist/refleks360-vX.Y.Z-win-x64.zip içine paketler.

.PARAMETER Version
    Versiyon etiketi (ZIP adında kullanılır). Varsayılan: 1.0.0
#>

param(
    [string] $Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$webProj = Join-Path $repoRoot "src/Refleks360.Web/Refleks360.Web.csproj"
$outDir = Join-Path $repoRoot "deploy/publish"
$distDir = Join-Path $repoRoot "deploy/dist"

if (Test-Path $outDir) { Remove-Item -Recurse -Force $outDir }
if (-not (Test-Path $distDir)) { New-Item -ItemType Directory -Path $distDir | Out-Null }

Write-Host "==> dotnet publish (Release, win-x64, self-contained)"
dotnet publish $webProj `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishReadyToRun=true `
    -p:DebugType=none `
    --output $outDir

if ($LASTEXITCODE -ne 0) { throw "dotnet publish hata verdi: exit $LASTEXITCODE" }

# Production template'i ekle (kurulumda kopyalanır)
Copy-Item (Join-Path $repoRoot "src/Refleks360.Web/appsettings.Production.template.json") `
    (Join-Path $outDir "appsettings.Production.template.json") -Force

# Kurulum kılavuzu + install scripti yayına ekle
Copy-Item (Join-Path $PSScriptRoot "install.ps1") (Join-Path $outDir "install.ps1") -Force
Copy-Item (Join-Path $PSScriptRoot "uninstall.ps1") (Join-Path $outDir "uninstall.ps1") -Force
Copy-Item (Join-Path $repoRoot "docs/KURULUM-KILAVUZU.md") (Join-Path $outDir "KURULUM-KILAVUZU.md") -Force

$zipName = "refleks360-v$Version-win-x64.zip"
$zipPath = Join-Path $distDir $zipName
if (Test-Path $zipPath) { Remove-Item -Force $zipPath }

Write-Host "==> ZIP olustur: $zipName"
Compress-Archive -Path (Join-Path $outDir "*") -DestinationPath $zipPath -Force

$size = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host ""
Write-Host "Tamam: $zipPath ($size MB)"
Write-Host "Kurulum: ZIP'i hedef makineye kopyala, ac, install.ps1'i yonetici PowerShell'inde calistir."
