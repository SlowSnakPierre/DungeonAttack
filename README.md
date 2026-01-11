# Dungeon Attack

**Version 1.0.0**

Un jeu de rôle roguelike basé en console où vous explorez des donjons dangereux, combattez des ennemis et améliorez votre héros.

## 📋 Table des Matières

- [Introduction](#introduction)
- [Installation](#installation)
- [Comment Jouer](#comment-jouer)
- [Mécaniques de Jeu](#mécaniques-de-jeu)
- [Camp](#camp)
- [Conseils et Astuces](#conseils-et-astuces)

## 🎮 Introduction

Dungeon Attack est un jeu roguelike où vous créez un héros personnalisé et explorez trois donjons différents remplis d'ennemis. Chaque run est unique, avec des combats aléatoires, des événements spéciaux et des récompenses à collecter.

## 💾 Installation

1. Clonez le dépôt ou téléchargez le projet
2. Ouvrez le projet dans Visual Studio 2022 ou supérieur
3. Assurez-vous que .NET 9 est installé
4. Compilez et lancez le projet `DungeonAttack.App`

## 🎯 Comment Jouer

### Menu Principal

Au lancement du jeu, vous avez plusieurs options :

- **Start Dungeon** - Commencer une nouvelle aventure ou charger une partie sauvegardée
- **Camp** - Accéder au camp pour gérer vos ressources et améliorations
- **Options** - Ajuster les paramètres du jeu
- **Credits** - Voir les crédits
- **Exit** - Quitter le jeu

### Création de Héros

1. **Entrer un nom** - Choisissez un nom pour votre héros (1-20 caractères, doit contenir au moins une lettre)
2. **Choisir un background** - Sélectionnez une classe de départ parmi plusieurs options :
   - Chaque background a des statistiques différentes (HP, MP, dégâts, précision, armure)
   - Chaque background démarre avec un nombre différent de points de compétence

3. **Choisir les compétences** :
   - **Active Skill** - Une compétence utilisable en combat (ex: Precise Strike, Strong Strike)
   - **Passive Skill** - Un bonus permanent (ex: Berserk, Shield Master, Concentration)
   - **Camp Skill** - Une compétence hors combat (ex: First Aid, Treasure Hunter, Bloody Ritual)

4. **Choisir un donjon** :
   - **Bandits** - Brigands et déserteurs
   - **Undeads** - Zombies, squelettes et fantômes
   - **Swamp** - Créatures des marais

### Exploration du Donjon

Le donjon alterne entre **combats** et **événements** :

#### Tours de Combat

1. Vous rencontrez 1 à 3 ennemis selon votre chance (modifiable avec la compétence Treasure Hunter)
2. Choisissez votre adversaire
3. Pendant le combat, vous pouvez :
   - **Attaquer** - Attaque normale
   - **Utiliser une compétence active** - Consomme du MP pour des attaques spéciales
   - **Fuir** - Tenter de s'échapper (pas toujours possible)

4. À chaque tour :
   - Vous et l'ennemi attaquez alternativement
   - La précision détermine si l'attaque touche
   - L'armure réduit les dégâts reçus
   - Le blocage peut annuler une partie des dégâts (avec un bouclier)

5. Après la victoire :
   - Vous gagnez de l'expérience
   - Vous récupérez du butin (pièces, ingrédients, points de Monolithe)
   - Vous pouvez trouver de l'équipement

#### Événements

Entre les combats, vous rencontrez des événements aléatoires :

- **Field Loot** - Trouvailles aléatoires
- **Secret Loot** - Coffres secrets
- **Bridge Keeper** - Énigme à résoudre
- **Boatman Eugene** - Marchand ambulant
- **Pig with Saucepan** - Événement comique
- **Black Mage** - Échange mystique
- **Gambler** - Pariez vos ressources
- **Altar of Blood** - Sacrifice HP pour des bonus
- **Warrior's Grave** - Améliorations d'équipement
- **Exit Run** - Possibilité de quitter le donjon

Chaque événement offre des choix qui affectent votre progression.

#### Camp de Feu

Tous les 2 niveaux, vous atteignez un feu de camp où vous pouvez :

- **Se reposer** - Récupère 10% HP et MP
- **Améliorer les statistiques** - Dépensez vos points de stats pour améliorer :
  - HP Max (+5)
  - MP Max (+3)
  - Dégâts (+1)
  - Précision (+3)
  - Armure (+2)
  - Régénération HP (+1)
  - Régénération MP (+1)
  - Pénétration d'armure (+1)
  - Chance de blocage (+2%)

- **Utiliser une compétence de camp** - Si vous en avez une
- **Sauvegarder et quitter** - Sauvegarde votre progression

### Montée en Niveau

- Tuez des ennemis pour gagner de l'expérience
- Chaque niveau vous donne :
  - +5 HP Max et HP restaurés
  - +3 MP Max et MP restaurés
  - +1 point de statistique
  - Tous les 3 niveaux : +1 point de compétence

## 🏕️ Camp

Le camp est votre base entre les runs. Vous y trouvez :

### 1. Entrepôt (Warehouse)
- Stocke vos pièces entre les runs
- Permet de transférer l'équipement de vos héros précédents

### 2. Boutique (Shop)
- Achetez de l'équipement avec vos pièces
- L'inventaire se renouvelle entre les runs
- Types d'équipement :
  - **Armes** - Augmente les dégâts et la précision
  - **Armures de corps** - Augmente l'armure
  - **Casques** - Augmente l'armure
  - **Gantelets** - Augmente l'armure
  - **Boucliers** - Augmente l'armure et la chance de blocage

### 3. Monolithe
- Dépensez vos points de Monolithe pour des améliorations permanentes :
  - HP (+5 par niveau)
  - MP (+3 par niveau)
  - Dégâts (+1 par niveau)
  - Précision (+3 par niveau)
  - Armure (+2 par niveau)
  - Points de stats (+1 par niveau)
  - Points de compétences (+1 par niveau)
  - Régénération HP (+1 par niveau)
  - Régénération MP (+1 par niveau)
  - Pénétration d'armure (+1 par niveau)
  - Chance de blocage (+2% par niveau)

### 4. Bibliothèque Occulte (Occult Library)
- Consultez et achetez des recettes de fabrication
- Utilisez les ingrédients collectés en donjon
- Créez des objets puissants pour améliorer votre héros

### 5. Statistiques
- Consultez vos statistiques totales
- Voyez combien d'ennemis vous avez vaincus
- Débloquez des bonus en tuant 30 ennemis d'un même type
- Débloquez le **Boss du Monolithe** en battant les 3 boss de donjon (Bandit Leader, Zombie Knight, Ancient Snail)

## 💡 Conseils et Astuces

1. **Sauvegardez régulièrement** - Utilisez les feux de camp pour sauvegarder votre progression

2. **Équilibrez vos statistiques** - Ne négligez pas la précision et l'armure au profit des dégâts

3. **Gestion du MP** - Les compétences actives sont puissantes mais coûteuses, utilisez-les judicieusement

4. **Événements** - Certains événements peuvent être risqués, évaluez les risques avant de choisir

5. **Progression permanente** - Les améliorations du Monolithe et les statistiques affectent TOUS vos futurs héros

6. **Synergies** - Certaines compétences fonctionnent mieux avec certains types d'équipement (ex: Shield Master avec un bouclier)

7. **Fuite** - N'ayez pas peur de fuir un combat difficile, surtout si vos HP sont bas

8. **Treasure Hunter** - Cette compétence de camp augmente vos chances d'avoir plus de choix d'ennemis et d'événements

9. **Collectez les ingrédients** - Ils sont utiles pour les recettes de la Bibliothèque Occulte

10. **Tuez 30 ennemis de chaque type** - Débloque des bonus permanents pour tous vos héros futurs

## ⚔️ Mécaniques de Combat

### Calcul des Dégâts
- Dégâts de base = aléatoire entre Min Dmg et Max Dmg
- Dégâts finaux = Dégâts de base - (Armure de la cible - Pénétration d'armure de l'attaquant)
- Les dégâts minimum sont toujours de 1

### Précision
- Chaque attaque a une chance de toucher basée sur la précision
- Précision élevée = plus de chances de toucher

### Blocage
- Nécessite un bouclier
- Réduit les dégâts reçus selon le coefficient de puissance de blocage
- La puissance de blocage augmente avec les HP actuels

### Régénération
- Régénération HP/MP : récupère des points à chaque tour
- Repos au feu de camp : récupère 10% HP/MP Max

## 🏆 Objectif Final

Battez les trois boss des donjons pour débloquer le **Boss du Monolithe**, le défi ultime !

---

**Bon courage, aventurier !** ⚔️🛡️✨
