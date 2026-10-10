$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$test=Join-Path $root ('.local\localization-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $test | Out-Null
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$exe=Join-Path $test 'LocalizationTests.exe'
& $csc /nologo /target:exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ("/out:"+$exe) (Join-Path $root 'scripts\LocalizationTests.cs') (Join-Path $root 'src\Language.cs')
if($LASTEXITCODE -ne 0){throw 'Localization test compilation failed'}
& $exe (Join-Path $root 'bin\Installer-SemperFi-Autonome-v0.8.exe') $test
if($LASTEXITCODE -ne 0){throw 'Localization tests failed'}
Write-Output "Localization evidence: $test"
