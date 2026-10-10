param([string]$BinaryDirectory, [string]$Version = '0.8')
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
if (-not $BinaryDirectory) { $BinaryDirectory = Join-Path $root 'bin' }
$BinaryDirectory = [IO.Path]::GetFullPath($BinaryDirectory)
if ($Version -ne '0.8') { throw 'Update versioned filenames and documentation before packaging another version.' }
$artifacts = Join-Path $root 'artifacts'
$stage = Join-Path $root ('.local\package-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage,$artifacts -Force | Out-Null
$executables = @('Installer-SemperFi-Autonome-v0.8.exe', 'Correctif-Musiques-SemperFi.exe')
foreach ($name in $executables) {
    Copy-Item -LiteralPath (Join-Path $BinaryDirectory $name) -Destination (Join-Path $stage $name)
    Copy-Item -LiteralPath (Join-Path $BinaryDirectory $name) -Destination (Join-Path $artifacts $name)
}
Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination (Join-Path $stage 'LICENSE.txt')
Copy-Item -LiteralPath (Join-Path $root 'THIRD_PARTY_NOTICES.md') -Destination $stage
Copy-Item -LiteralPath (Join-Path $root 'third_party\DDrawCompat\LICENSE.txt') -Destination (Join-Path $stage 'DDrawCompat-LICENSE.txt')
$readme = "Semper Fi Installer 0.8`r`n`r`nFR : Ouvrir Installer-SemperFi-Autonome-v0.8.exe, choisir Francais ou English, puis votre ISO ou dossier extrait contenant SF.EXE, puis un nouveau dossier de destination. Le Bureau recoit un raccourci et Windows une entree de desinstallation.`r`n`r`nInstallation autonome 0.6 deja presente : fermer le jeu et ouvrir Correctif-Musiques-SemperFi.exe, puis choisir son dossier. Les sauvegardes et preferences sont conservees. Inutile de reinstaller.`r`n`r`nEN: Run Installer-SemperFi-Autonome-v0.8.exe, choose English, then select your own supported ISO or extracted game folder, then a new destination. For an existing autonomous 0.6 installation, close the game and run Correctif-Musiques-SemperFi.exe. Choose French or English at startup. Saves and preferences are preserved by the music patch. Uninstall via Windows installed apps.`r`n`r`nNo game files are included. Your own supported game copy is required.`r`nDocumentation / support: https://github.com/Lordingard/SemperFi-Installer`r`n`r`nMIT license for original project code. The game retains its own rights. DDrawCompat is distributed under its included upstream license.`r`n"
Set-Content -LiteralPath (Join-Path $stage 'README.txt') -Value $readme -Encoding UTF8
$zip = Join-Path $artifacts ('SemperFi-Installer-v' + $Version + '-windows.zip')
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -Force
$names = $executables + @(Split-Path $zip -Leaf)
$checksums = foreach ($name in $names) { ((Get-FileHash -LiteralPath (Join-Path $artifacts $name) -Algorithm SHA256).Hash.ToLowerInvariant() + '  ' + $name) }
$checksums | Set-Content -LiteralPath (Join-Path $artifacts 'SHA256SUMS.txt') -Encoding ASCII
Write-Output "Release files: $artifacts"
