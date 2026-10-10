param([string]$OutputName='Installer-SemperFi-Autonome-v0.7.exe')
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$src=Join-Path $root 'src'
$bin=Join-Path $root 'bin'
New-Item -ItemType Directory -Path $bin -Force | Out-Null
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
if((Get-FileHash -LiteralPath (Join-Path $root 'third_party\DDrawCompat\ddraw.dll')).Hash -ne 'F75F0AC48D2782F225C483DC2F1142303A513E8DD8A60793891ADE89F64755EA'){throw 'DDrawCompat checksum mismatch'}
& $csc /nologo /target:winexe /platform:anycpu /optimize+ ("/win32manifest:"+(Join-Path $src "Application.manifest")) /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ("/resource:"+(Join-Path $src 'payload.tsv')+',payload.tsv') ("/out:"+(Join-Path $bin 'Desinstaller.exe')) (Join-Path $src 'Uninstaller.cs') (Join-Path $src 'InstallSupport.cs')
if($LASTEXITCODE -ne 0){throw 'Compilation du desinstallateur echouee'}
& $csc /nologo /target:winexe /platform:anycpu /optimize+ ("/win32manifest:"+(Join-Path $src "Application.manifest")) /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ("/resource:"+(Join-Path $src 'payload.tsv')+',payload.tsv') ("/resource:"+(Join-Path $root 'third_party\DDrawCompat\ddraw.dll')+',ddraw.dll') ("/resource:"+(Join-Path $root 'patches\sf-autonomous.patch')+',sf-compat.patch') ("/resource:"+(Join-Path $bin 'Desinstaller.exe')+',uninstaller.exe') ("/out:"+(Join-Path $bin $OutputName)) (Join-Path $src 'InstallerAutonomous.cs') (Join-Path $src 'GameSource.cs') (Join-Path $src 'Integration.cs') (Join-Path $src 'InstallSupport.cs') (Join-Path $src 'Compatibility.cs') (Join-Path $root 'src\AutonomousPatch.cs')
if($LASTEXITCODE -ne 0){throw 'Compilation echouee'}

& $csc /nologo /target:winexe /platform:anycpu /optimize+ /reference:System.Windows.Forms.dll ("/win32manifest:"+(Join-Path $src 'Application.manifest')) ("/resource:"+(Join-Path $root 'patches\sf-autonomous.patch')+',sf-compat.patch') ("/out:"+(Join-Path $bin 'Correctif-Musiques-SemperFi.exe')) (Join-Path $src 'MusicPatch.cs') (Join-Path $src 'AutonomousPatch.cs') (Join-Path $src 'InstallSupport.cs')
if($LASTEXITCODE -ne 0){throw 'Compilation du correctif musical echouee'}
Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination (Join-Path $bin 'LICENSE.txt')
Copy-Item -LiteralPath (Join-Path $root 'third_party\DDrawCompat\LICENSE.txt') -Destination (Join-Path $bin 'DDrawCompat-LICENSE.txt')
Write-Output "Build complete: $bin"
