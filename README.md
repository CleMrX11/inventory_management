# Inventory Management

Application de gestion d'articles et de stock, construite avec .NET, SQLite, Entity Framework Core, React, TypeScript et Vite.

## Fonctionnalités

- Gestion des articles avec référence EAN-13 unique, nom, catégorie et prix HT/TTC.
- Deux catégories d'articles : produits alimentaires (`FoodItem`) et marchandises (`Merchandise`).
- Suivi du stock par mouvements : réception, retrait et ajustement pour l'inventaire.
- Calcul du stock courant, du stock vendable et de la valeur vendable TTC.
- Historique des mouvements de stock par article.
- Règles métier portées par le domaine : validité EAN-13, unicité de la référence, prix positifs, suppression bloquée si un article a déjà du stock.

## Architecture

### Backend

Le projet est structuré avec une approche DDD / hexagonale légère côté backend, et une application React côté frontend :

- `Domain` : contient les agrégats, value objects et règles métier. Cette couche ne dépend d’aucune couche applicative ou technique.
- `Application` : orchestre les cas d’usage, expose les DTOs et définit les ports nécessaires aux adaptateurs. Cette couche dépend uniquement du domaine.
- `Infrastructure` : implémente les ports techniques, notamment la persistance SQLite / Entity Framework Core et les repositories. Cette couche dépend de l’application et du domaine.
- `Api` : l’API expose les endpoints REST et branche les dépendances backend;

Les deux aggregate roots principaux sont :

- `Article` : représente le catalogue article et ses invariants.
- `StockItem` : représente un lot de stock pour un article donné, avec ses mouvements.


### Frontend
Le frontend est construit avec React, React Router, shadcn/ui et Tailwind CSS pour l’interface utilisateur et consomme l'API en backend.

## Choix et hypothèses
- L'approche DDD a été retenue pour isoler les règles métier et garder le domaine indépendant de l'API, de l'UI et de la persistance.
- Les lots de stock sont identifiés par leurs spécificités métier : DLC et disponibilité à emporter pour les produits alimentaires, niveau de packaging pour les marchandises.
- L'UI reste volontairement simple pour l'exercice, avec une navigation Articles / Stock / Historique.

Simplifications pour le projet, qui devraient être revues dans le cas d'une app en production :
- Pas de gestion d'authentification ni des autorisations.
- Pas de migration de données, la base SQLite est créée automatiquement au lancement.
- Pas de test pour le frontend.


## Non terminé / améliorations connues

- Dans l'UI, pour retirer un produit alimentaire, il faut sélectionner exactement la date de DLC du lot concerné. Ce n'est pas très pratique; il faudrait revoir l'interface de stock pour sélectionner directement le batch / lot de `StockItem` au lieu de ressaisir ses caractéristiques.

## Utilisation de l'IA

- IA utilisée : ChatGPT 5.5 pour le raisonnement complexe et ChatGPT 5.4 Mini pour les tâches simples.
- Outil principal : Codex CLI.
- Usages : aide à la conception, génération de code, revue de code, corrections ciblées.
- Organisation : parallélisation de certaines features avec des worktrees Git pour avancer sur plusieurs sujets en même temps.

## Temps passé

Environ 5 heures.

## Lancer le projet

Prérequis :

- .NET SDK 10.
- Node.js et npm.

Installer les dépendances frontend une première fois :

```bash
cd frontend && npm install
```

Lancer le backend et le frontend dans deux terminaux séparés :

```bash
dotnet run --project backend/InventoryManagement.Api/InventoryManagement.Api.csproj --launch-profile http
cd frontend && npm run dev
```

URLs :

- API : `http://localhost:5056`
- OpenAPI : `http://localhost:5056/openapi/v1.json`
- Frontend : `http://localhost:5173`

Le serveur Vite proxy les appels `/api` vers `http://localhost:5056`.

## Endpoints principaux

```txt
GET    /api/articles
GET    /api/articles/{id}
POST   /api/articles
PUT    /api/articles/{id}
DELETE /api/articles/{id}
GET    /api/articles/{articleId}/stock
POST   /api/articles/{articleId}/stock/movements
```

## Vérification

Backend :

```bash
dotnet build InventoryManagement.slnx
dotnet test InventoryManagement.slnx
```

Frontend :

```bash
cd frontend && npm run build
```
