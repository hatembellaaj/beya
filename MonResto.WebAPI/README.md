# MonResto.WebAPI – Backend en détail

Ce dossier contient l’API REST ASP.NET Core qui alimente MonResto. Elle expose les opérations catalogue, panier et commandes, sécurisées par Identity + JWT, et orchestre les accès aux repositories EF Core implémentés dans `MonResto.Data`.

## Architecture et responsabilités
- **Program.cs** : point d’entrée, configuration DI, authentification JWT, CORS, AutoMapper, Swagger et lancement du seeder de données/Identity.
- **Controllers/** : endpoints REST fins orientés ressources (`Categories`, `Articles`, `Menu`, `Cart`, `Order`, `Account`). Ils s’appuient sur des DTOs et sur AutoMapper pour traduire les entités domaine en charges utiles API.
- **Services/MappingProfile.cs** : profil AutoMapper centralisant les mappings entités ⇔ DTOs (catégories, articles, menus, paniers, commandes).
- **Authentication/JwtSettings.cs** : options JWT (issuer/audience/secret/expiration) injectées depuis la configuration.
- **Intégration data** : les interfaces repository (`MonResto.Domain.Interfaces`) sont implémentées par `MonResto.Data.Repositories` et injectées ici via DI. `AppDbContext` (PostgreSQL) est configuré côté API.
- **Seed** : `MonResto.Data.Seed.DatabaseSeeder` crée les schémas Identity/catalogue au premier démarrage, insère un admin (`admin@monresto.com` / `Passw0rd!`) et des données exemples si la base est vide.

## Cycle de démarrage
1. **Configuration** : `appsettings.json` fournit la chaîne PostgreSQL `DefaultConnection` et la section `Jwt` (Issuer/Audience/SecretKey/ExpirationMinutes).
2. **DI** : enregistrement du DbContext PostgreSQL, d’Identity (utilisateurs + rôles), des repositories (catégorie, article, menu, panier, commande) et d’AutoMapper.
3. **Sécurité** : ajout de l’authentification JWT (Bearer) + autorisation.
4. **Middleware** : CORS permissif par défaut, redirection HTTPS, Swagger en développement.
5. **Seed** : lancement de `DatabaseSeeder.SeedAsync` pour appliquer migrations, créer les tables, l’admin et peupler le catalogue.
6. **Routing** : exposition des controllers via `app.MapControllers()`.

## Configuration détaillée (appsettings.json)
```json
{
  "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=monresto;Username=postgres;Password=postgres" },
  "Jwt": {
    "Issuer": "MonResto",
    "Audience": "MonRestoClient",
    "SecretKey": "votre_cle_super_secrete_au_moins_32_caracteres",
    "ExpirationMinutes": 60
  }
}
```
- **SecretKey** doit faire au moins 32 caractères pour la clé HMAC SHA-256.
- **ExpirationMinutes** pilote la durée de validité des tokens.
- Sur Linux/macOS, si nécessaire, exportez `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1` avant `dotnet restore/run` (voir README racine pour les variantes OS).

## Lancer le backend
```bash
# Depuis la racine du repo
cd MonResto.WebAPI
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet restore  # Linux/macOS uniquement si besoin
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet run       # idem
# Ou simplement : dotnet restore && dotnet run (Windows)
```
- Swagger : `https://localhost:5001/swagger` (ou `http://localhost:5000`).
- Ajoutez le token JWT via **Authorize** pour tester les routes protégées.

### Base de données & migrations
- Le seeder applique `context.Database.MigrateAsync()` et crée les tables si elles n’existent pas encore (Identity + catalogue).
- Pour générer/appliquer manuellement des migrations PostgreSQL depuis la racine :
```bash
dotnet tool restore
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef database update \
  --project MonResto.Data/MonResto.Data.csproj \
  --startup-project MonResto.WebAPI/MonResto.WebAPI.csproj
```

## Authentification & autorisations
- **Register** `POST /api/account/register` : crée un utilisateur Identity.
- **Login** `POST /api/account/login` : retourne `{ userName, token, expiration }` signé avec la clé `Jwt.SecretKey`.
- Les routes `Cart` et `Order` nécessitent un header `Authorization: Bearer <token>`.
- Le rôle `Admin` (créé au seed) peut mettre à jour le statut d’une commande via `PATCH /api/order/{id}/status`.

## DTOs & mapping (exemples)
- **ArticleDto** ajoute `CategoryName` via la navigation Category.
- **MenuDto** inclut la liste d’articles résolue depuis `MenuArticles`.
- **CartItemDto** fournit `ArticleName` et `Price` dérivés de l’article chargé.
- **OrderDto** embarque les `OrderItemDto` mappés depuis `OrderItems`.

## Endpoints (vue détaillée)
- **Categories**
  - `GET /api/categories` : liste des catégories.
  - `GET /api/categories/{id}` : détail.
  - `POST /api/categories` : crée (body `CategoryCreateDto`).
  - `PUT /api/categories/{id}` : met à jour.
  - `DELETE /api/categories/{id}` : supprime.
- **Articles**
  - `GET /api/articles` : liste.
  - `GET /api/articles/{id}` : détail.
  - `GET /api/articles/by-category/{categoryId}` : filtre par catégorie.
  - `GET /api/articles/search?term=...` : recherche par nom.
  - `POST /api/articles` / `PUT /api/articles/{id}` / `DELETE /api/articles/{id}` : CRUD complet.
- **Menus**
  - `GET /api/menu` / `GET /api/menu/{id}` : lecture.
  - `POST /api/menu` : crée un menu et associe les articles de `ArticleIds`.
  - `PUT /api/menu/{id}` : met à jour les métadonnées et remplace les associations articles.
  - `DELETE /api/menu/{id}` : supprime.
  - `POST /api/menu/{menuId}/articles/{articleId}` : ajoute un article au menu.
  - `DELETE /api/menu/{menuId}/articles/{articleId}` : retire un article du menu.
- **Cart** (auth requis)
  - `GET /api/cart` : récupère le panier de l’utilisateur connecté.
  - `GET /api/cart/summary` : total quantité/prix.
  - `POST /api/cart` : ajoute un article (ou incrémente si déjà présent).
  - `PUT /api/cart/{cartItemId}` : met à jour la quantité.
  - `DELETE /api/cart/{cartItemId}` : supprime une ligne.
- **Orders** (auth requis)
  - `GET /api/order` : historique des commandes de l’utilisateur connecté.
  - `POST /api/order` : crée une commande à partir du panier courant, calcule le total et vide le panier.
  - `PATCH /api/order/{id}/status` (Admin) : met à jour le statut (`Pending`, `Paid`, `Delivered`).

## Flux panier → commande
1. L’utilisateur ajoute des articles via `POST /api/cart` (création ou incrément).
2. Il ajuste les quantités avec `PUT /api/cart/{id}` ou supprime avec `DELETE /api/cart/{id}`.
3. `GET /api/cart/summary` calcule le total (quantité/prix) avant validation.
4. `POST /api/order` transforme le panier en commande : création des `OrderItems`, somme `TotalPrice`, statut initial `Pending`, puis nettoyage du panier.

## Bonnes pratiques et extensions
- Centralisez les nouveaux mappings dans `Services/MappingProfile.cs`.
- Ajoutez de nouveaux repositories côté `MonResto.Data` et injectez-les dans Program.cs.
- Protégez les endpoints sensibles avec `[Authorize]` et, si besoin, `[Authorize(Roles = "RoleName")]`.
- Étendez le domaine (ex. photos d’articles, options de livraison) en ajoutant l’entité dans `MonResto.Domain`, la configuration EF Core dans `MonResto.Data`, puis les endpoints ici.
