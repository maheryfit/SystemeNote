$ErrorActionPreference = 'Stop'
$webroot = Join-Path (Get-Location) 'wwwroot\Rotativa'
if (-not (Test-Path $webroot)) { New-Item -ItemType Directory -Path $webroot | Out-Null }
$release = Invoke-RestMethod -Uri 'https://api.github.com/repos/wkhtmltopdf/packaging/releases/latest' -Headers @{ 'User-Agent' = 'request' }
$asset = $release.assets | Where-Object { $_.name -match 'win64|msvc|windows' } | Select-Object -First 1
if (-not $asset) { Write-Error 'No Windows asset found in latest packaging release.'; exit 2 }
$downloadUrl = $asset.browser_download_url
$filename = $asset.name
$outPath = Join-Path $webroot $filename
Write-Host "Downloading $filename from $downloadUrl to $outPath"
Invoke-WebRequest -Uri $downloadUrl -OutFile $outPath
# If zip, extract
if ($outPath -match '\.zip$') {
    Write-Host 'Extracting zip...'
    Expand-Archive -Path $outPath -DestinationPath $webroot -Force
    Remove-Item $outPath -Force
} elseif ($outPath -match '\.7z$') {
    Write-Host '7z archive downloaded. Attempting to extract using 7z if available.'
    if (Get-Command 7z -ErrorAction SilentlyContinue) { & 7z x $outPath -o$webroot -y; Remove-Item $outPath -Force }
    else { Write-Host '7z not installed; please extract manually.' }
} else {
    Write-Host "Downloaded asset saved as $outPath"
}
Write-Host 'Done.'
