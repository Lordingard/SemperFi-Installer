# Semper Fi Installer

**Retrouvez Semper Fi (1998) sur un Windows moderne, avec votre propre copie du jeu.**

[![Windows build](https://github.com/Lordingard/SemperFi-Installer/actions/workflows/build.yml/badge.svg)](https://github.com/Lordingard/SemperFi-Installer/actions/workflows/build.yml)
[![Version 0.8](https://img.shields.io/badge/version-0.8-olive)](https://github.com/Lordingard/SemperFi-Installer/releases/tag/v0.8)
[![License MIT](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

[English](README.en.md) Â· [TÃ©lÃ©chargements](https://github.com/Lordingard/SemperFi-Installer/releases/latest) Â· [Signaler un problÃ¨me](https://github.com/Lordingard/SemperFi-Installer/issues)

Cet installateur communautaire remplace le setup 16 bits du CD, installe les
fichiers originaux et applique les corrections de compatibilitÃ© validÃ©es.
Il accepte directement l'ISO ou le dossier extrait du CD. AprÃ¨s installation,
les donnÃ©es, vidÃ©os et musiques sont lues localement : le CD n'est plus nÃ©cessaire.

**Le jeu et son ISO ne sont pas fournis.** Une copie personnelle de l'Ã©dition
prise en charge est nÃ©cessaire. Le projet n'est pas affiliÃ© Ã  l'Ã©diteur original.

## TÃ©lÃ©charger et installer

1. TÃ©lÃ©charger [SemperFi-Installer-v0.8-windows.zip](https://github.com/Lordingard/SemperFi-Installer/releases/download/v0.8/SemperFi-Installer-v0.8-windows.zip), puis extraire l'archive.
2. Ouvrir **Installer-SemperFi-Autonome-v0.8.exe**.
3. SÃ©lectionner l'ISO du jeu ou le dossier contenant `SF.EXE`.
4. Choisir un nouveau dossier. `C:\Program Files (x86)\SemperFi` est proposÃ© et reste modifiable.
5. Lancer **Semper Fi** depuis le raccourci crÃ©Ã© sur le Bureau.

Les dossiers protÃ©gÃ©s demandent une autorisation administrateur Ã  l'installation
et Ã  la dÃ©sinstallation. Le jeu se lance normalement, sans Ã©lÃ©vation.
L'installateur refuse une destination existante et vÃ©rifie les fichiers du CD.

## Ce que la version 0.8 apporte

| Fonction | RÃ©sultat |
| --- | --- |
| Langue des outils | Choix Français / English, mémorisé pour la désinstallation |
| Installation depuis ISO ou dossier | Aucun setup 16 bits ni montage d'ISO requis |
| Campagnes | Correction du dÃ©bordement de chemin qui provoquait un plantage |
| Affichage | DDrawCompat 0.7.1, palettes et six icÃ´nes des onglets corrigÃ©es |
| VidÃ©os et musiques | Lecture locale, y compris aprÃ¨s le choix d'une campagne |
| Bureau | Raccourci automatique avec l'icÃ´ne originale du jeu |
| DÃ©sinstallation Windows | EntrÃ©e Semper Fi dans les applications installÃ©es |
| Sauvegardes et prÃ©fÃ©rences | Choix de les conserver ou de les supprimer |

## Installation 0.6 dÃ©jÃ  prÃ©sente ?

Fermer le jeu, ouvrir **Correctif-Musiques-SemperFi.exe**, puis choisir le dossier
de l'installation autonome. Ce correctif rÃ©tablit les chemins des musiques
sans rÃ©installer le jeu ni modifier les sauvegardes ou les prÃ©fÃ©rences.
Une copie de l'exÃ©cutable prÃ©cÃ©dent est conservÃ©e pour revenir en arriÃ¨re.
Il cible les exÃ©cutables autonomes reconnus, pas une installation originale non corrigÃ©e.

## DÃ©sinstaller

Dans les **applications installÃ©es de Windows**, choisir **Semper Fi**, puis
**DÃ©sinstaller**. Le mÃªme outil est disponible sous `Desinstaller.exe` dans
le dossier du jeu. La conservation des sauvegardes et prÃ©fÃ©rences est cochÃ©e
par dÃ©faut ; leur suppression demande une confirmation supplÃ©mentaire.

En cas de conservation, `SCEN`, `CAMPAIGN` et `PREFS.DAT` restent dans le dossier.
Les autres fichiers personnels sont Ã©galement prÃ©servÃ©s. Garder l'installation
Ã  son emplacement initial pour que le raccourci et l'entrÃ©e Windows restent valides.

## CompatibilitÃ© et validation

La version 0.7 a Ã©tÃ© testÃ©e sur le poste Windows rÃ©cent du mainteneur, depuis
l'ISO et son contenu extrait. Une installation fraÃ®che, les campagnes,
l'affichage et les musiques ont Ã©tÃ© confirmÃ©s par l'utilisateur. L'installation,
les empreintes des fichiers, la dÃ©sinstallation et la prÃ©servation des donnÃ©es
ont aussi Ã©tÃ© vÃ©rifiÃ©es automatiquement.

Ce rÃ©sultat ne couvre pas toutes les versions de Windows ni toutes les Ã©ditions
du CD. Une campagne entiÃ¨re et le multijoueur n'ont pas Ã©tÃ© validÃ©s.
L'interface des outils est en franÃ§ais ; le jeu conserve sa langue d'origine.

## Documentation et dÃ©veloppement

- [Guide d'utilisation et dÃ©pannage](docs/usage.md)
- [Compiler depuis les sources](docs/build.md)
- [Corrections et structure du projet](docs/technical.md)
- [Validation et tests](docs/testing.md)
- [Droits du jeu et abandonware](docs/rights.md)
- [Historique des versions](CHANGELOG.md)
- [Contribuer](CONTRIBUTING.md)

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1
```

Les exÃ©cutables sont produits dans `bin`. Le build ne nÃ©cessite aucun fichier
du jeu. Les tests d'installation complets nÃ©cessitent votre propre ISO ou dossier.

Le code original du projet est sous [licence MIT](LICENSE).
DDrawCompat conserve [sa propre licence](THIRD_PARTY_NOTICES.md).
