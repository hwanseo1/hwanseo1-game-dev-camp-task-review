<#
  Saves the current clipboard image into a task's screenshots folder.

  Usage:
    .\tools\save-clip.ps1 Task-01-hello error-nullref
    .\tools\save-clip.ps1 Task-01-hello           # name defaults to "shot"

  Take the screenshot with Win+Shift+S first, then run this.
#>
param(
  [Parameter(Mandatory = $true)][string]$Task,
  [string]$Name = 'shot'
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$dir  = Join-Path $repo "Tasks\$Task\screenshots"

$img = Get-Clipboard -Format Image
if ($null -eq $img) {
  Write-Host "No image on the clipboard. Press Win+Shift+S, select an area, then run this again." -ForegroundColor Yellow
  exit 1
}

if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

$existing = @(Get-ChildItem -Path $dir -Filter '*.png' -ErrorAction SilentlyContinue)
$n = '{0:d2}' -f ($existing.Count + 1)
$safe = ($Name -replace '[^A-Za-z0-9._-]', '-')
$path = Join-Path $dir "$n-$safe.png"

$img.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
$img.Dispose()

$size = [math]::Round((Get-Item $path).Length / 1KB, 1)
Write-Host "Saved: $path ($size KB)" -ForegroundColor Green
