#[ModKit Wiki](../../wiki)

# Mod pour l'activité du CIA lors de l'intégration 2026-2027

## Description

Le but est de faire un mod pour le jeu Keep Talking and Nobody Explodes, contenant différents modules liés à l'informatique.

## Installation pour Développement

1. Télécharger le repository.
2. Ouvrez Unity Hub et cliquez sur "Add" pour ajouter le projet.
3. Ouvrez le projet dans Unity avec la version 2017.4.22f1.
4. Un menu "Keep Talking ModKit" devrait apparaître dans la barre de menu (en haut). Cliquez dessus et sélectionnez "Build AssetBundle" pour générer le mod.
5. Pour un aperçu du mod dans unity, cliquez sur "Play" dans la barre de menu. Cela lancera le jeu avec le mod chargé.

## Installation du Mod sur KTANE

1. Récupérer le projet déjà construit **OU** générer le dans Unity (voir [Installation pour Développement](#installation-pour-développement) )
2. Créer un dossier "*mods*" dans "*/Steam/steamapps/common/Keep Talking and Nobody Explodes/mods*".
3. Relancer le jeu et activer les mods.
4. Une fois sur la page d'accueil, allez dans "*Mods*", puis "*Gérer les mods*", et enfin "*Gérer les mods installés*". Le mod devrait apparaître. 
5. Pour lancer une partie avec le mod,  revenir sur la page d'accueil, puis aller dans "*Jeux Libre*". Un button "*Mods Seulement*" devrait être présent. 

## Informations supplémentaires

- Les modules sont situés dans le dossier "Assets/Integration".
- Le premier module étant le module "hexModule", il est conseillé de s'en inspirer pour créer de nouveaux modules.
- Vous y trouverez les composants nécessaires pour créer un module, tels que KMSelectable, KMBombModule, KMBombInfo, etc.
- Différents Assets sont disponibles pour vous aider à créer vos modules, tels que des boutons, des zones de texte, des leds, etc.
- N'implémentez pas de sons pour les modules, cela n'est pas nécessaire pour le projet.
- Si vous avez des questions, contactez moi sur Discord.