param([Parameter(Mandatory=$true)][string]$Source, [ValidateSet("fr","en")][string]$Language="fr")
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$Source=[IO.Path]::GetFullPath($Source)
$test=Join-Path $root ('.local\validation-'+[guid]::NewGuid().ToString('N'))
if(Test-Path -LiteralPath $test){throw 'Le dossier de test existe deja'}
New-Item -ItemType Directory -Path $test | Out-Null
$desktop=Join-Path $test 'Bureau'
New-Item -ItemType Directory -Path $desktop | Out-Null
$result=@()
function Run([string]$exe,[string]$arguments,[bool]$Localized=$true){if($Localized){$arguments+=' --lang '+$Language};$process=Start-Process -FilePath $exe -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru;if($process.ExitCode -ne 0){throw "Echec du processus : $($process.ExitCode)"}}
foreach($mode in @('keep','delete')){
 $folder=Join-Path $test ('Jeu avec espaces '+$mode)
 $src=$Source
 Run (Join-Path $root 'bin\Installer-SemperFi-Autonome-v0.8.exe') ('--install "'+$src+'" "'+$folder+'" --desktop "'+$desktop+'"')
 Run (Join-Path $root 'bin\Installer-SemperFi-Autonome-v0.8.exe') ('--verify "'+$folder+'"')
 if((Get-Content -Raw -LiteralPath (Join-Path $folder '.semperfi-language.txt')).Trim() -ne $Language){throw 'Langue incorrecte'}
 $state=Get-Content -LiteralPath (Join-Path $folder '.semperfi-install.txt')
 if($state.Length -ne 5 -or $state[0] -ne 'SemperFi-0.8'){throw 'Etat incorrect'}
 $key='Registry::HKEY_USERS\'+$state[4]+'\Software\Microsoft\Windows\CurrentVersion\Uninstall\SemperFi-'+$state[1]
 $entry=Get-ItemProperty -LiteralPath $key
 if($entry.DisplayVersion -ne '0.8' -or $entry.InstallLocation -ne $folder -or $entry.UninstallString -ne ('"'+(Join-Path $folder 'Desinstaller.exe')+'"') -or $entry.NoModify -ne 1){throw 'Entree Windows incorrecte'}
 $shell=New-Object -ComObject WScript.Shell
 $link=$shell.CreateShortcut($state[3])
 if($link.TargetPath -ne (Join-Path $folder 'SF.EXE') -or $link.WorkingDirectory -ne $folder -or $link.IconLocation -ne ((Join-Path $folder 'SF.EXE')+',0')){throw 'Raccourci incorrect'}
 $sid=[System.Security.Principal.WindowsIdentity]::GetCurrent().User.Value
 foreach($name in @('SCEN','CAMPAIGN','PREFS.DAT','DDrawCompat-SF.log')){
  $acl=Get-Acl -LiteralPath (Join-Path $folder $name)
  $rights=@($acl.Access | Where-Object {$_.IdentityReference.Translate([System.Security.Principal.SecurityIdentifier]).Value -eq $sid -and ($_.FileSystemRights -band [System.Security.AccessControl.FileSystemRights]::Modify) -eq [System.Security.AccessControl.FileSystemRights]::Modify})
  if($rights.Count -eq 0){throw "Droits manquants : $name"}
 }
 $notice=Get-Content -Raw -LiteralPath (Join-Path $folder 'INSTALLATION.txt')
 if($Language -eq 'en' -and !$notice.StartsWith('Local installation')){throw 'English notice missing'}
 if($Language -eq 'fr' -and !$notice.StartsWith('Installation locale')){throw 'French notice missing'}
 $uninstallNotice=Get-Content -Raw -LiteralPath (Join-Path $folder 'DESINSTALLATION.txt')
 if($Language -eq 'en' -and !$uninstallNotice.StartsWith('Use Windows')){throw 'English uninstall notice missing'}
 if($Language -eq 'fr' -and !$uninstallNotice.StartsWith('Utiliser')){throw 'French uninstall notice missing'}
 # The graphical patch and game data must be identical regardless of UI language.
 if((Get-FileHash -LiteralPath (Join-Path $folder 'SF.EXE')).Hash -ne 'E46DBA94E8CECEB313465A1FE4EB824BE1FAAC864CDB6399A4DF8A7097F75EB9'){throw 'Game patch changed'}
 Run (Join-Path $root 'bin\Correctif-Musiques-SemperFi.exe') ('--apply "'+$folder+'"')
 $newSave=Join-Path $folder 'SCEN\partie-personnelle.sav'
 [IO.File]::WriteAllText($newSave,'sauvegarde test')
 $prefs=Join-Path $folder 'PREFS.DAT'
 [IO.File]::WriteAllText($prefs,'preferences test')
 $personal=Join-Path $folder 'notes-personnelles.txt'
 [IO.File]::WriteAllText($personal,'conserver')
 Run (Join-Path $root 'bin\Desinstaller.exe') ('--uninstall "'+$folder+'" --'+$mode+'-saves') $false
 if(Test-Path -LiteralPath $key){throw 'Entree Windows restante'}
 if(Test-Path -LiteralPath $state[3]){throw 'Raccourci restant'}
 if(Test-Path -LiteralPath (Join-Path $folder '.semperfi-language.txt')){throw 'Fichier de langue restant'}
 if(Test-Path -LiteralPath (Join-Path $folder 'SF.EXE')){throw 'Jeu restant'}
 if(!(Test-Path -LiteralPath $personal)){throw 'Fichier personnel perdu'}
 if($mode -eq 'keep'){if(!(Test-Path -LiteralPath $newSave) -or (Get-Content -Raw -LiteralPath $prefs) -ne 'preferences test'){throw 'Sauvegardes ou preferences perdues'}}else{if((Test-Path -LiteralPath $newSave) -or (Test-Path -LiteralPath $prefs)){throw 'Sauvegardes restantes'}}
 $result+=[pscustomobject]@{mode=$mode;language=$Language;source=$src;registered=$true;verified=$true;shortcut=$true;dataPermissions=$true;unregistered=$true;savesChoice=$true;personalPreserved=$true}
}
$result | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $test 'validation.json') -Encoding UTF8
$result | ConvertTo-Json
Write-Output ('Validation directory: '+$test)
