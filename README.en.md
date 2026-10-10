# Semper Fi Installer

**Play Semper Fi (1998) on modern Windows using your own copy of the game.**

[FranÃ§ais](README.md) Â· [Download v0.8](https://github.com/Lordingard/SemperFi-Installer/releases/tag/v0.8) Â· [Report a problem](https://github.com/Lordingard/SemperFi-Installer/issues)

This community installer replaces the original 16-bit setup. It accepts the
supported CD ISO or an extracted folder, verifies the original files and applies
compatibility fixes. Movies, music and game data are installed locally;
no CD or mounted ISO is needed after installation.

**No game or ISO is included. You must supply your own supported copy.**
This project is not affiliated with the original publisher.

## Install

1. Download and extract [SemperFi-Installer-v0.8-windows.zip](https://github.com/Lordingard/SemperFi-Installer/releases/download/v0.8/SemperFi-Installer-v0.8-windows.zip).
2. Run **Installer-SemperFi-Autonome-v0.8.exe**.
3. Choose **English** or **Français**, then select your ISO or the folder containing `SF.EXE`.
4. Choose a new destination. The editable default is `C:\Program Files (x86)\SemperFi`.
5. Launch the game from the desktop shortcut.

The installer and music patch offer French and English at startup. The selected
language is remembered for uninstall. Installation into protected folders requests
administrator permission; playing the game does not need elevation. Existing
folders and unexpected game editions are rejected to protect user files.

## Features

- French and English interfaces, including errors and uninstall.
- Direct installation from ISO or extracted CD contents; no 16-bit setup.
- Campaign path overflow fix.
- DDrawCompat 0.7.1, correct palettes and six restored campaign tab icons.
- Local movies and music, including campaign music after directory changes.
- Desktop shortcut with the original game icon.
- Standard Windows uninstall entry, with a keep/delete saves choice.

For an existing autonomous 0.6 installation, close the game and run
**Correctif-Musiques-SemperFi.exe**. Select the installed game folder. The patch
preserves saves and preferences and backs up the previous executable.

## Uninstall

Select **Semper Fi** in Windows installed apps, or run `Desinstaller.exe` from
the game folder. **Keep saves and preferences** is selected by default. Keeping
them leaves `SCEN`, `CAMPAIGN` and `PREFS.DAT` in place; other personal files
are preserved either way. Keep the installation at its original location.

## Development and scope

Build on Windows with the .NET Framework 4.x C# compiler:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1
```

Outputs are placed in `bin`. No game files are needed to build. Complete
installation tests require your own source media. See [build instructions](docs/build.md)
and [technical notes](docs/technical.md).

Version 0.8 retains the same game fixes as 0.7. Version 0.7 passed user tests on the maintainer's modern Windows machine,
including a fresh installation and campaign music. This is not a guarantee
for every Windows release or every game edition; multiplayer and a complete
campaign have not been validated.

Original project code is [MIT licensed](LICENSE). [DDrawCompat](THIRD_PARTY_NOTICES.md)
retains its upstream license. The game is not licensed by this repository.
Calling a game abandonware does not establish permission to redistribute it;
see [rights notes](docs/rights.md).
