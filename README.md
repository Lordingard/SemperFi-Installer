# Semper Fi Installer

**Retrouvez Semper Fi (1998) sur un Windows moderne, avec votre propre copie du jeu.**

[![Windows build](https://github.com/Lordingard/SemperFi-Installer/actions/workflows/build.yml/badge.svg)](https://github.com/Lordingard/SemperFi-Installer/actions/workflows/build.yml)
[![Version 0.7](https://img.shields.io/badge/version-0.7-olive)](https://github.com/Lordingard/SemperFi-Installer/releases/tag/v0.7)
[![License MIT](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

[English](README.en.md) · [Téléchargements](https://github.com/Lordingard/SemperFi-Installer/releases/latest) · [Signaler un problème](https://github.com/Lordingard/SemperFi-Installer/issues)

Cet installateur communautaire remplace le setup 16 bits du CD, installe les
fichiers originaux et applique les corrections de compatibilité validées.
Il accepte directement l'ISO ou le dossier extrait du CD. Après installation,
les données, vidéos et musiques sont lues localement : le CD n'est plus nécessaire.

**Le jeu et son ISO ne sont pas fournis.** Une copie personnelle de l'édition
prise en charge est nécessaire. Le projet n'est pas affilié à l'éditeur original.

## Télécharger et installer

1. Télécharger [SemperFi-Installer-v0.7-windows.zip](https://github.com/Lordingard/SemperFi-Installer/releases/download/v0.7/SemperFi-Installer-v0.7-windows.zip), puis extraire l'archive.
2. Ouvrir **Installer-SemperFi-Autonome-v0.7.exe**.
3. Sélectionner l'ISO du jeu ou le dossier contenant `SF.EXE`.
4. Choisir un nouveau dossier. `C:\Program Files (x86)\SemperFi` est proposé et reste modifiable.
5. Lancer **Semper Fi** depuis le raccourci créé sur le Bureau.

Les dossiers protégés demandent une autorisation administrateur à l'installation
et à la désinstallation. Le jeu se lance normalement, sans élévation.
L'installateur refuse une destination existante et vérifie les fichiers du CD.

## Ce que la version 0.7 apporte

| Fonction | Résultat |
| --- | --- |
| Installation depuis ISO ou dossier | Aucun setup 16 bits ni montage d'ISO requis |
| Campagnes | Correction du débordement de chemin qui provoquait un plantage |
| Affichage | DDrawCompat 0.7.1, palettes et six icônes des onglets corrigées |
| Vidéos et musiques | Lecture locale, y compris après le choix d'une campagne |
| Bureau | Raccourci automatique avec l'icône originale du jeu |
| Désinstallation Windows | Entrée Semper Fi dans les applications installées |
| Sauvegardes et préférences | Choix de les conserver ou de les supprimer |

## Installation 0.6 déjà présente ?

Fermer le jeu, ouvrir **Correctif-Musiques-SemperFi.exe**, puis choisir le dossier
de l'installation autonome. Ce correctif rétablit les chemins des musiques
sans réinstaller le jeu ni modifier les sauvegardes ou les préférences.
Une copie de l'exécutable précédent est conservée pour revenir en arrière.
Il cible les exécutables autonomes reconnus, pas une installation originale non corrigée.

## Désinstaller

Dans les **applications installées de Windows**, choisir **Semper Fi**, puis
**Désinstaller**. Le même outil est disponible sous `Desinstaller.exe` dans
le dossier du jeu. La conservation des sauvegardes et préférences est cochée
par défaut ; leur suppression demande une confirmation supplémentaire.

En cas de conservation, `SCEN`, `CAMPAIGN` et `PREFS.DAT` restent dans le dossier.
Les autres fichiers personnels sont également préservés. Garder l'installation
à son emplacement initial pour que le raccourci et l'entrée Windows restent valides.

## Compatibilité et validation

La version 0.7 a été testée sur le poste Windows récent du mainteneur, depuis
l'ISO et son contenu extrait. Une installation fraîche, les campagnes,
l'affichage et les musiques ont été confirmés par l'utilisateur. L'installation,
les empreintes des fichiers, la désinstallation et la préservation des données
ont aussi été vérifiées automatiquement.

Ce résultat ne couvre pas toutes les versions de Windows ni toutes les éditions
du CD. Une campagne entière et le multijoueur n'ont pas été validés.
L'interface des outils est en français ; le jeu conserve sa langue d'origine.

## Documentation et développement

- [Guide d'utilisation et dépannage](docs/usage.md)
- [Compiler depuis les sources](docs/build.md)
- [Corrections et structure du projet](docs/technical.md)
- [Validation et tests](docs/testing.md)
- [Droits du jeu et abandonware](docs/rights.md)
- [Historique des versions](CHANGELOG.md)
- [Contribuer](CONTRIBUTING.md)

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1
```

Les exécutables sont produits dans `bin`. Le build ne nécessite aucun fichier
du jeu. Les tests d'installation complets nécessitent votre propre ISO ou dossier.

Le code original du projet est sous [licence MIT](LICENSE).
DDrawCompat conserve [sa propre licence](THIRD_PARTY_NOTICES.md).
