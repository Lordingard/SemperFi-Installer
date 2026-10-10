# Changelog

## 0.8 — 2026-10-10 · French / English tools

- Language selector at installer and music-patch startup, defaulting to the Windows UI language (French, otherwise English).
- English screens, progress, errors, shortcut description and generated installation/uninstall notices.
- Selected language retained for uninstall and forwarded to elevated processes and uninstall workers.
- Optional `--lang fr` / `--lang en` on every tool, including unattended commands.
- Bilingual installation/uninstall checks and public-source localization tests.
- Game compatibility patch, graphics dependency and music behavior unchanged from validated 0.7.

## 0.7 â€” 2026-10-10 Â· First public release

- Public sources, French/English documentation, Windows build and local integration test scripts.
- Campaign music uses the absolute installation folder, preserving access after working-directory changes.
- Reversible music patch for existing autonomous 0.6 installations.
- Retains the validated 0.6 features: direct ISO/folder installation, local movies/music, campaign and bitmap fixes, DDrawCompat 0.7.1, desktop shortcut, editable destination, Windows uninstall registration and saves/preferences choice.
- Fresh-installation gameplay acceptance was confirmed on 2026-10-09.
- No original game data or ISO included.

## 0.6 â€” Private validation

Editable Program Files destination and standard Windows uninstall registration.
Removed the separate action for integrating shortcuts into an existing installation.

## 0.5 â€” Private validation

Automatic desktop shortcut with original icon; uninstaller with save/preferences choice.

## 0.4 â€” Private validation

Direct ISO reader and locally installed media; no disc required after installation.

## 0.3 â€” Private validation

Campaign path overflow fix, restored tab bitmaps and validated DirectDraw renderer.
