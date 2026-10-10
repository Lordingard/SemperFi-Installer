# Build from source

## Requirements

- Windows.
- .NET Framework 4.x with `C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe`.
- PowerShell; Git for cloning. Python 3 is used only by the public-content check.

No NuGet packages, Visual Studio installation, game ISO or proprietary runtime
files are required for compilation. The original game's runtimes are copied
from the user's source media during installation, not bundled here.

```powershell
git clone https://github.com/Lordingard/SemperFi-Installer.git
cd SemperFi-Installer
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1
python .\scripts\check-public-content.py
```

## Output

`bin/` contains the installer, music patch, uninstaller and license notices.
The uninstaller is also embedded in the installer. Build verifies the checksum
of the unmodified DDrawCompat DLL before embedding it.

The supplied `patches/sf-autonomous.patch` is the compact compatibility delta
unchanged since version 0.7. It is not a complete SF.EXE. `src/payload.tsv` lists file
names, sizes and checksums rather than containing the game files.

The Windows CI builds the three tools, checks public repository contents and runs
`scripts/test-localization.ps1` without game files.
It cannot run game installation or gameplay tests without privately supplied media.
Framework compiler builds may differ byte-for-byte from release binaries due
to compiler-generated metadata; release checksums identify the published artifacts.

## Local integration tests

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\test-installation.ps1 -Source "C:\media\SEMPERFI.iso" -Language en
```

Use a test copy of your source media. Tests install into a unique folder below
`.local`, create shortcuts in a fake desktop folder, check registration in the
current user's Windows uninstall list and remove their own installation entries.
They never launch gameplay or modify existing installed games.

## Differential patch regeneration

The compact published delta can be regenerated from your privately held
original and patched executable, with both complete hashes checked:

```powershell
python scripts/generate-delta.py "C:\private\original\SF.EXE" "C:\private\patched\SF.EXE" --output "C:\private\sf-autonomous.patch"
```

Never commit either executable or other game files.

## Package a release

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
```

The ZIP, two standalone tools and SHA256SUMS.txt are created in artifacts/.
The ZIP includes the usage guide and both project/dependency license notices.
Packaging can select an already validated build with -BinaryDirectory.
