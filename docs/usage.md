# Utilisation et dépannage

## Source acceptée

Choisir l'ISO 9660 du CD pris en charge ou son dossier extrait contenant SF.EXE.
Les 261 fichiers originaux sont vérifiés par SHA-256 avant la copie. Une autre
édition, des fichiers déjà modifiés ou un CD incomplet sont refusés.
L'ISO est lu directement : inutile de le monter. Les données du jeu ne sont
pas téléchargées par l'installateur.

## Installation et sauvegardes

Utiliser un nouveau dossier. La destination proposée reste modifiable.
Le Bureau utilisé est celui de l'utilisateur ayant lancé l'installation,
même si un autre compte administrateur autorise l'opération. Un raccourci
appartenant à une autre installation est conservé, avec un nom libre pour le nouveau.
Chaque installation possède sa propre entrée de désinstallation Windows.

Les sauvegardes et préférences se trouvent dans SCEN, CAMPAIGN et PREFS.DAT.
Le choix de conservation à la désinstallation laisse les deux dossiers entiers,
y compris leurs données de base. Sauvegarder ces éléments avant tout déplacement
ou toute suppression manuelle du jeu. Ne pas déplacer une installation enregistrée.

## Musiques d'une ancienne installation autonome

Fermer le jeu, lancer Correctif-Musiques-SemperFi.exe et choisir son dossier.
La copie précédente est conservée dans compat-backup/SF.before-music-v07.exe.
Pour la rétablir :

```powershell
.\Correctif-Musiques-SemperFi.exe --restore "C:\chemin\du\jeu"
```

La musique doit être activée dans les préférences du jeu avec un volume audible.
La version 0.7 utilise le dossier absolu de SONG pour retrouver les morceaux
après les changements de dossier effectués par le jeu.
Le morceau Build_2B.wav, référencé par l'exécutable, est absent du CD fourni ;
il n'est pas remplacé par une musique inventée. Les morceaux de briefing et
de campagne sont présents dans l'édition testée.

## Erreurs

| Message ou symptôme | Action |
| --- | --- |
| Destination déjà existante | Choisir un autre dossier ; ne pas supprimer les sauvegardes pour contourner le refus |
| Source manquante ou différente | Vérifier l'édition et l'intégrité de votre CD ou ISO |
| Jeu ouvert lors d'un correctif ou d'une désinstallation | Fermer Semper Fi avant de continuer |
| Installation non reconnue ou déplacée | Rétablir le dossier à son emplacement enregistré |
| Musique du menu mais pas en campagne sur 0.6 | Appliquer le correctif musical ou installer 0.7 |

Les détails peuvent se trouver dans installation.log, DDrawCompat-SF.log,
installer-error.txt ou music-patch-error.txt. Les fichiers temporaires
*.installation-* d'une installation échouée restent disponibles pour diagnostic.
Les supprimer manuellement seulement après avoir identifié l'échec.
Les exécutables de release ne sont pas signés numériquement.

## Ligne de commande

```powershell
.\Installer-SemperFi-Autonome-v0.7.exe --install "C:\media\SEMPERFI.iso" "C:\Jeux\SemperFi"
.\Installer-SemperFi-Autonome-v0.7.exe --verify "C:\Jeux\SemperFi"
.\Correctif-Musiques-SemperFi.exe --apply "C:\Jeux\SemperFi"
```

--verify compare les fichiers installés aux empreintes attendues. Un PREFS.DAT
modifié par le jeu peut donc être signalé après une partie : ce mode sert surtout
à vérifier une installation fraîche, pas à réparer les préférences.
