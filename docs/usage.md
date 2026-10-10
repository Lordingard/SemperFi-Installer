# Utilisation et dÃ©pannage

## Source acceptÃ©e

Choisir l'ISO 9660 du CD pris en charge ou son dossier extrait contenant SF.EXE.
Les 261 fichiers originaux sont vÃ©rifiÃ©s par SHA-256 avant la copie. Une autre
Ã©dition, des fichiers dÃ©jÃ  modifiÃ©s ou un CD incomplet sont refusÃ©s.
L'ISO est lu directement : inutile de le monter. Les donnÃ©es du jeu ne sont
pas tÃ©lÃ©chargÃ©es par l'installateur.

## Installation et sauvegardes

Utiliser un nouveau dossier. La destination proposÃ©e reste modifiable.
Le Bureau utilisÃ© est celui de l'utilisateur ayant lancÃ© l'installation,
mÃªme si un autre compte administrateur autorise l'opÃ©ration. Un raccourci
appartenant Ã  une autre installation est conservÃ©, avec un nom libre pour le nouveau.
Chaque installation possÃ¨de sa propre entrÃ©e de dÃ©sinstallation Windows.

Les sauvegardes et prÃ©fÃ©rences se trouvent dans SCEN, CAMPAIGN et PREFS.DAT.
Le choix de conservation Ã  la dÃ©sinstallation laisse les deux dossiers entiers,
y compris leurs donnÃ©es de base. Sauvegarder ces Ã©lÃ©ments avant tout dÃ©placement
ou toute suppression manuelle du jeu. Ne pas dÃ©placer une installation enregistrÃ©e.

## Musiques d'une ancienne installation autonome

Fermer le jeu, lancer Correctif-Musiques-SemperFi.exe et choisir son dossier.
La copie prÃ©cÃ©dente est conservÃ©e dans compat-backup/SF.before-music-v07.exe.
Pour la rÃ©tablir :

```powershell
.\Correctif-Musiques-SemperFi.exe --restore "C:\chemin\du\jeu"
```

La musique doit Ãªtre activÃ©e dans les prÃ©fÃ©rences du jeu avec un volume audible.
La version 0.7 utilise le dossier absolu de SONG pour retrouver les morceaux
aprÃ¨s les changements de dossier effectuÃ©s par le jeu.
Le morceau Build_2B.wav, rÃ©fÃ©rencÃ© par l'exÃ©cutable, est absent du CD fourni ;
il n'est pas remplacÃ© par une musique inventÃ©e. Les morceaux de briefing et
de campagne sont prÃ©sents dans l'Ã©dition testÃ©e.

## Erreurs

| Message ou symptÃ´me | Action |
| --- | --- |
| Destination dÃ©jÃ  existante | Choisir un autre dossier ; ne pas supprimer les sauvegardes pour contourner le refus |
| Source manquante ou diffÃ©rente | VÃ©rifier l'Ã©dition et l'intÃ©gritÃ© de votre CD ou ISO |
| Jeu ouvert lors d'un correctif ou d'une dÃ©sinstallation | Fermer Semper Fi avant de continuer |
| Installation non reconnue ou dÃ©placÃ©e | RÃ©tablir le dossier Ã  son emplacement enregistrÃ© |
| Musique du menu mais pas en campagne sur 0.6 | Appliquer le correctif musical ou installer 0.8 |

Les dÃ©tails peuvent se trouver dans installation.log, DDrawCompat-SF.log,
installer-error.txt ou music-patch-error.txt. Les fichiers temporaires
*.installation-* d'une installation Ã©chouÃ©e restent disponibles pour diagnostic.
Les supprimer manuellement seulement aprÃ¨s avoir identifiÃ© l'Ã©chec.
Les exÃ©cutables de release ne sont pas signÃ©s numÃ©riquement.

## Ligne de commande

```powershell
.\Installer-SemperFi-Autonome-v0.8.exe --install "C:\media\SEMPERFI.iso" "C:\Jeux\SemperFi"
.\Installer-SemperFi-Autonome-v0.8.exe --verify "C:\Jeux\SemperFi"
.\Correctif-Musiques-SemperFi.exe --apply "C:\Jeux\SemperFi"
```

--verify compare les fichiers installÃ©s aux empreintes attendues. Un PREFS.DAT
modifiÃ© par le jeu peut donc Ãªtre signalÃ© aprÃ¨s une partie : ce mode sert surtout
Ã  vÃ©rifier une installation fraÃ®che, pas Ã  rÃ©parer les prÃ©fÃ©rences.
