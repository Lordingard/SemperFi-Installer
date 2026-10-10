# Validation

Version 0.7 was confirmed by the user on a fresh installation on 2026-10-09.
Campaign launch, correct colors, tab icons, briefing/campaign music and local
media access were tested during development on the maintainer's Windows machine.
This is acceptance evidence for the tested scenarios, not certification of all
Windows versions, hardware, complete campaigns or multiplayer.

Automated local checks covered:

- ISO and extracted-folder installation, 261 original file hashes and 263 patched payload entries.
- Desktop shortcut target, working directory and original executable icon.
- Per-install Windows uninstall entry and independent installations.
- Both save/preference keep/delete choices, with unrelated personal files preserved.
- Writable data folders and explicit asInvoker manifests.
- Music patch application, repeated application, exact rollback and rejection of unknown executables.
- WinMM opening of all 10 SONG WAVs from three working directories: absolute paths worked in all 30 cases; old relative paths failed outside the installation root.

Use scripts/test-installation.ps1 with your own media to repeat installation
and uninstall checks. CI checks source-tree contents and compilation only.
Private diagnostic dumps, user paths, source media and installed files are
excluded from this repository.

## Bilingual tools (0.8)

French ISO and English extracted-folder installations passed installation,
file verification, desktop shortcut, Windows registration and uninstall tests
with both saved-game choices. Selected language and generated notices are checked.
The installed SF.EXE remains identical to validated 0.7. Public CI checks both
installer interfaces, language arguments, background culture and persisted language
without requiring game files. Windows system dialogs retain OS button language.

```powershell
.\scripts\test-localization.ps1
.\scripts\test-installation.ps1 -Source "C:\media\SEMPERFI.iso" -Language fr
.\scripts\test-installation.ps1 -Source "C:\media\SEMPERFI" -Language en
```
