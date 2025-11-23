# MonResto – API REST + EF Core + Repository + JWT + Blazor

MonResto est une solution .NET complète qui propose une API REST sécurisée et un client Blazor WebAssembly pour gérer un catalogue de restauration (catégories, articles, menus), un panier et des commandes. L’architecture suit les bonnes pratiques Clean Architecture avec séparation des responsabilités, mapping DTO/entités, et authentification JWT via ASP.NET Core Identity.

## 📁 Architecture de la solution
```
MonResto.sln
├─ MonResto.Domain      // Entités métier, DTOs, interfaces de repository, enums
├─ MonResto.Data        // DbContext EF Core, configurations, repositories implémentés
├─ MonResto.WebAPI      // API REST ASP.NET Core, JWT, controllers, AutoMapper, Swagger
└─ MonResto.BlazorClient// Client Blazor WASM, services HttpClient, pages et modèles
```

### Domain
- Entités : `Category`, `Article`, `Menu` (many-to-many Articles), `CartItem`, `Order`, `OrderItem`.
- Enum : `OrderStatus` (`Pending`, `Paid`, `Delivered`).
- DTOs pour exposer les données côté API/Blazor.
- Interfaces de repository pour chaque agrégat (CRUD async).

### Data
- `AppDbContext` hérite de `IdentityDbContext` pour inclure les tables Identity.
- Mapping Fluent API pour relations one-to-many et many-to-many (Menu–Article, Order–OrderItem).
- Repositories concrets utilisant EF Core et LINQ.

### WebAPI
- Configurations `appsettings.json` (connection strings, JWT) et `Program.cs` (DI, Auth, Swagger).
- Controllers : catégories, articles (recherche par nom, filtre par catégorie), menus (ajout/retrait d’article), panier, commandes (calcul du total + historique), compte (register/login avec JWT).
- AutoMapper profile pour convertir entités ⇔ DTOs.
- Swagger configuré avec schéma de sécurité bearer JWT.

### Blazor Client
- Services `HttpClient` pour categories, articles, panier, commandes, authentification (gestion token + `Authorization` header).
- Pages : accueil (catégories + menus), liste d’articles par catégorie, détail d’un article (ajout au panier), panier (édition/suppression), commande, historique, login/register.

## 🧰 Prérequis
- .NET 7 SDK ou supérieur installé.
- PostgreSQL accessible (local ou conteneur). Créez une base de données (ex: `monresto`).
- `dotnet-ef` pour appliquer les migrations si vous souhaitez les exécuter en local.

## ⚙️ Configuration
Modifiez `MonResto.WebAPI/appsettings.json` (ou variables d’environnement) :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=monresto;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "MonResto",
    "Audience": "MonRestoClient",
    "SecretKey": "votre_cle_super_secrete_au_moins_32_caracteres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🚀 Mise en route
### 1) Restaurer et compiler
> ℹ️ Sous Windows, **ne définissez pas** `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` : les commandes échouent. Utilisez simplement `dotnet restore`/`dotnet build`.

```bash
cd MonResto.WebAPI

# Linux/macOS (si vous avez des soucis de locales)
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet restore
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet build

# Windows
dotnet restore
dotnet build
```

### Données de démo automatiques
- Un administrateur par défaut est créé avec l'email `admin@monresto.com` et le mot de passe `Passw0rd!` (rôle `Admin`).
- Des exemples de catégories, articles et un menu "Menu Gourmand" sont insérés lors du premier lancement.
Ces données sont générées automatiquement au démarrage de l'API si la base est vide.

### 2) Appliquer les migrations (PostgreSQL)
```bash
# Linux/macOS
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef database update --project ../MonResto.Data

# Windows
dotnet ef database update --project ../MonResto.Data
```

### 3) Lancer l’API
```bash
# Linux/macOS
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet run

# Windows
dotnet run
```
- Swagger disponible sur `https://localhost:5001/swagger` (ou `http://localhost:5000`).
- Ajoutez un token JWT via le bouton **Authorize** pour tester les endpoints protégés.

### 4) Lancer le client Blazor WebAssembly
```bash
cd ../MonResto.BlazorClient
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet restore  # Linux/macOS
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet run      # Linux/macOS

dotnet restore  # Windows
dotnet run      # Windows
```
- L’application consomme l’API configurée dans `Program.cs`/`appsettings` du client. Adaptez l’URL si besoin.

## 🔐 Authentification & Autorisations
- Enregistrement (`/api/account/register`) et connexion (`/api/account/login`) retournent un JWT.
- Les routes panier/commandes exigent l’en-tête `Authorization: Bearer <token>`.
- Un rôle `Admin` est créé automatiquement ; il peut mettre à jour l’état d’une commande via `PATCH /api/order/{id}/status` (payload : `{ "status": "Paid" | "Delivered" }`).
- Identity gère les utilisateurs, mots de passe hashés et rôles extensibles.

## 📦 Fonctionnalités principales
- CRUD Catégories & Articles, recherche par nom, filtre par catégorie.
- Gestion des Menus avec relation many-to-many (ajout/suppression d’articles).
- Panier utilisateur : ajout, modification de quantité, suppression, consultation, résumé (`/api/cart/summary`) avec total quantité/prix.
- Commandes : création avec calcul automatique du total, historique par utilisateur, statut (`Pending`, `Paid`, `Delivered`), mise à jour du statut par un administrateur.
- Documentation Swagger sécurisée.
- Front-end Blazor : navigation des catégories/menus, détails article, panier, commandes, authentification.

## 🔧 Personnalisation et extension
- Ajoutez des profils AutoMapper pour de nouveaux DTOs dans `MonResto.WebAPI/Services/MappingProfile.cs`.
- Étendez le modèle (ex : photos d’articles, options de livraison) en ajoutant une entité dans `MonResto.Domain/Entities`, la configuration dans `MonResto.Data/Context/AppDbContext.cs`, et le repository correspondant.
- Migrations : générez-en de nouvelles avec `dotnet ef migrations add <Nom>` dans `MonResto.Data`.

## 🧪 Tests
- Des scénarios manuels sont disponibles via Swagger. Vous pouvez ajouter des tests d’intégration ou unitaires selon vos besoins (xUnit, NUnit…).

### Tester le backend (API)
1. **Lancer l’API** : assurez-vous que `MonResto.WebAPI` tourne (voir section "Mise en route").
2. **Tester via Swagger** (recommandé) :
   - Ouvrez `https://localhost:5001/swagger`.
   - Cliquez sur **Authorize** et collez un token JWT obtenu via `/api/account/login` (format `Bearer <token>`).
   - Exécutez les endpoints protégés (panier/commandes) ou publics (catégories/articles).
3. **Tester via cURL** (exemples) :
   ```bash
   # Récupérer les catégories (public)
   curl -k https://localhost:5001/api/categories

   # Login pour obtenir un token
   curl -k -X POST https://localhost:5001/api/account/login \
     -H "Content-Type: application/json" \
     -d '{"email":"demo@monresto.com","password":"Passw0rd!"}'

   # Appel protégé avec le token reçu
   curl -k https://localhost:5001/api/orders \
     -H "Authorization: Bearer <votre_token>"
   ```
4. **Tester via Postman/Bruno** :
   - Importez l’URL Swagger (`https://localhost:5001/swagger/v1/swagger.json`) pour générer la collection.
   - Ajoutez une variable d’environnement `token` et configurez l’auth Bearer pour les routes protégées.
5. **Tests automatiques (optionnel)** :
   - Ajoutez un projet de tests (xUnit/NUnit) et référencez `MonResto.WebAPI`/`MonResto.Data`.
   - Utilisez `WebApplicationFactory` pour démarrer l’API en mémoire et tester les endpoints.

## 🤝 Contribution
- Forkez le repo, créez une branche, validez vos modifications et ouvrez une PR.
- Respectez l’architecture existante (Domain/Data/WebAPI/BlazorClient) et le pattern Repository.

## 🗺 Points d’entrée clés
- **DbContext** : `MonResto.Data/Context/AppDbContext.cs`
- **Repositories** : `MonResto.Data/Repositories/*`
- **Controllers** : `MonResto.WebAPI/Controllers/*`
- **Mappings** : `MonResto.WebAPI/Services/MappingProfile.cs`
- **Blazor services/pages** : `MonResto.BlazorClient/Services/*`, `MonResto.BlazorClient/Pages/*`

