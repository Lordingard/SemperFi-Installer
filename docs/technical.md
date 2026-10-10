# Compatibility notes

## Structure

| Path | Purpose |
| --- | --- |
| src/InstallerAutonomous.cs | Source selection, staged installation and file verification |
| src/GameSource.cs | Directory and ISO 9660 reader |
| src/AutonomousPatch.cs | Hash-checked executable delta application |
| src/Compatibility.cs | DirectDraw installation and file manifests |
| src/Integration.cs, InstallSupport.cs | Desktop shortcut, data permissions, Windows uninstall registration |
| src/Uninstaller.cs | Bounded removal with saves/preferences choice |
| src/MusicPatch.cs | Reversible 0.6 music-path update |
| src/payload.tsv | Expected original CD files |
| patches/sf-autonomous.patch | Compact version 0.7 delta |
| third_party/DDrawCompat/ | Official 0.7.1 x86 DLL and license |

## Fixed behaviors

1. Two unused sprintf calls overflowed an 80-byte campaign path buffer. They
   are removed while the original runtime libraries are retained.
2. The campaign tab image lists use a 24-bit format. Six bitmap loads use
   LoadImageA with LR_CREATEDIBSECTION to preserve the embedded bitmap palettes.
3. DDrawCompat 0.7.1 uses GdiInterops=all for the original DirectDraw/GDI renderer.
4. CD probing is adapted to locally installed AVI and SONG media.
5. Version 0.7 copies the game's absolute installation root into the music path
   buffer before appending SONG and the original WAV name. A relative path
   stopped working after campaign selection changed the process working directory.
   The original streaming and DirectSound playback code is preserved.

Original SF.EXE SHA-256:
`575F85F4FFE843FF7B49CDD7C8EF3EC9993843CA60ADA16D832B7D5279C9AA26`

Autonomous 0.6 SHA-256:
`624E19E1CC5BADF36423D9455CAA13E33FCF92695A0D06775C2B6D088D3F773E`

Autonomous 0.7 SHA-256:
`E46DBA94E8CECEB313465A1FE4EB824BE1FAAC864CDB6399A4DF8A7097F75EB9`

## Installation and removal

The installer verifies source files, copies into a separate staging directory,
applies the delta and verifies installed files before moving the directory to
its final location. The original SF.EXE is backed up under compat-backup.
The 261 original payload files plus the wrapper DLL and configuration yield
263 entries in installed-files.tsv; integration adds launcher and uninstall files.

Metadata .semperfi-install.txt records version, unique installation ID, absolute
folder, shortcut and owner SID. Each user/installation has its own Windows
uninstall entry. Elevation is requested for protected destinations, while
specific data folders and preference/log files get write permissions for the
initiating user. Executables retain the destination's inherited permissions.

The uninstaller validates metadata and paths, removes only known installed
files and the owned shortcut/registration, and preserves unknown personal files.
Keeping saves preserves entire SCEN and CAMPAIGN directories plus PREFS.DAT.
Its temporary worker handles removal of the running uninstaller itself.
