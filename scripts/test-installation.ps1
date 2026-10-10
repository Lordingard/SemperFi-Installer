param([Parameter(Mandatory=$true)][string]$Source)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$Source=[IO.Path]::GetFullPath($Source)
$test=Join-Path $root ('.local\validation-'+[guid]::NewGuid().ToString('N'))
if(Test-Path -LiteralPath $test){throw 'Le dossier de test existe deja'}
New-Item -ItemType Directory -Path $test | Out-Null
$desktop=Join-Path $test 'Bureau'
New-Item -ItemType Directory -Path $desktop | Out-Null
$result=@()
function Run([string]$exe,[string]$arguments){$process=Start-Process -FilePath $exe -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru;if($process.ExitCode -ne 0){throw "Echec du processus : $($process.ExitCode)"}}
foreach($mode in @('keep','delete')){
 $folder=Join-Path $test ('Jeu avec espaces '+$mode)
 $src=$Source
 Run (Join-Path $root 'bin\Installer-SemperFi-Autonome-v0.7.exe') ('--install "'+$src+'" "'+$folder+'" --desktop "'+$desktop+'"')
 Run (Join-Path $root 'bin\Installer-SemperFi-Autonome-v0.7.exe') ('--verify "'+$folder+'"')
 $state=Get-Content -LiteralPath (Join-Path $folder '.semperfi-install.txt')
 if($state.Length -ne 5 -or $state[0] -ne 'SemperFi-0.7'){throw 'Etat incorrect'}
 $key='Registry::HKEY_USERS\'+$state[4]+'\Software\Microsoft\Windows\CurrentVersion\Uninstall\SemperFi-'+$state[1]
 $entry=Get-ItemProperty -LiteralPath $key
 if($entry.DisplayVersion -ne '0.7' -or $entry.InstallLocation -ne $folder -or $entry.UninstallString -ne ('"'+(Join-Path $folder 'Desinstaller.exe')+'"') -or $entry.NoModify -ne 1){throw 'Entree Windows incorrecte'}
 $shell=New-Object -ComObject WScript.Shell
 $link=$shell.CreateShortcut($state[3])
 if($link.TargetPath -ne (Join-Path $folder 'SF.EXE') -or $link.WorkingDirectory -ne $folder -or $link.IconLocation -ne ((Join-Path $folder 'SF.EXE')+',0')){throw 'Raccourci incorrect'}
 $sid=[System.Security.Principal.WindowsIdentity]::GetCurrent().User.Value
 foreach($name in @('SCEN','CAMPAIGN','PREFS.DAT','DDrawCompat-SF.log')){
  $acl=Get-Acl -LiteralPath (Join-Path $folder $name)
  $rights=@($acl.Access | Where-Object {$_.IdentityReference.Translate([System.Security.Principal.SecurityIdentifier]).Value -eq $sid -and ($_.FileSystemRights -band [System.Security.AccessControl.FileSystemRights]::Modify) -eq [System.Security.AccessControl.FileSystemRights]::Modify})
  if($rights.Count -eq 0){throw "Droits manquants : $name"}
 }
 $newSave=Join-Path $folder 'SCEN\partie-personnelle.sav'
 [IO.File]::WriteAllText($newSave,'sauvegarde test')
 $prefs=Join-Path $folder 'PREFS.DAT'
 [IO.File]::WriteAllText($prefs,'preferences test')
 $personal=Join-Path $folder 'notes-personnelles.txt'
 [IO.File]::WriteAllText($personal,'conserver')
 Run (Join-Path $root 'bin\Desinstaller.exe') ('--uninstall "'+$folder+'" --'+$mode+'-saves')
 if(Test-Path -LiteralPath $key){throw 'Entree Windows restante'}
 if(Test-Path -LiteralPath $state[3]){throw 'Raccourci restant'}
 if(Test-Path -LiteralPath (Join-Path $folder 'SF.EXE')){throw 'Jeu restant'}
 if(!(Test-Path -LiteralPath $personal)){throw 'Fichier personnel perdu'}
 if($mode -eq 'keep'){if(!(Test-Path -LiteralPath $newSave) -or (Get-Content -Raw -LiteralPath $prefs) -ne 'preferences test'){throw 'Sauvegardes ou preferences perdues'}}else{if((Test-Path -LiteralPath $newSave) -or (Test-Path -LiteralPath $prefs)){throw 'Sauvegardes restantes'}}
 $result+=[pscustomobject]@{mode=$mode;source=$src;registered=$true;verified=$true;shortcut=$true;dataPermissions=$true;unregistered=$true;savesChoice=$true;personalPreserved=$true}
}
$result | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $test 'validation.json') -Encoding UTF8
$result | ConvertTo-Json
Write-Output ('Validation directory: '+$test)
