# Mon-Resto — Test et Qualité Logiciel

## 1) Cas de test fonctionnels

### 1.1 CRUD Articles

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| ART-CRUD-01 | Créer un article valide | Compte admin authentifié (JWT) | `name="Pizza Margherita"`, `description="Tomate mozzarella"`, `price=9.90`, `categoryId=1` | 1. POST `/api/articles` avec le payload.<br>2. GET `/api/articles/{id}`. | 201 Created et article retourné avec les champs saisis. | Équivalence | Intégration |
| ART-CRUD-02 | Refus création article sans nom | Compte admin authentifié | `name=""`, `price=9.90`, `categoryId=1` | 1. POST `/api/articles` avec `name` vide. | 400 BadRequest + message de validation. | Limites (champ obligatoire) | Intégration |
| ART-CRUD-03 | Lire un article existant | Article existant `id=10` | `id=10` | 1. GET `/api/articles/10`. | 200 OK + détail article. | Équivalence | Système |
| ART-CRUD-04 | Mettre à jour un article | Compte admin authentifié, article `id=10` | `name="Pizza 4 Fromages"`, `price=12.50` | 1. PUT `/api/articles/10` avec les champs modifiés.<br>2. GET `/api/articles/10`. | 200 OK, article modifié persisté. | Équivalence | Intégration |
| ART-CRUD-05 | Supprimer un article | Compte admin authentifié, article `id=10` | `id=10` | 1. DELETE `/api/articles/10`.<br>2. GET `/api/articles/10`. | 204 NoContent puis 404 NotFound. | Équivalence | Intégration |
| ART-CRUD-06 | Refus suppression article inexistant | Compte admin authentifié | `id=9999` | 1. DELETE `/api/articles/9999`. | 404 NotFound. | Équivalence | Intégration |

### 1.2 CRUD Catégories

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| CAT-CRUD-01 | Créer une catégorie valide | Compte admin authentifié | `name="Desserts"`, `description="Sucré"` | 1. POST `/api/categories`.<br>2. GET `/api/categories`. | 201 Created + catégorie listée. | Équivalence | Intégration |
| CAT-CRUD-02 | Refus création catégorie sans nom | Compte admin authentifié | `name=""` | 1. POST `/api/categories` sans nom. | 400 BadRequest + message de validation. | Limites | Intégration |
| CAT-CRUD-03 | Lire une catégorie existante | Catégorie `id=3` | `id=3` | 1. GET `/api/categories/3` (si endpoint disponible) ou filtrage via liste. | 200 OK + catégorie. | Équivalence | Système |
| CAT-CRUD-04 | Mettre à jour une catégorie | Compte admin authentifié, catégorie `id=3` | `name="Desserts maison"` | 1. PUT `/api/categories/3`.<br>2. GET `/api/categories`. | 200 OK, libellé modifié. | Équivalence | Intégration |
| CAT-CRUD-05 | Supprimer une catégorie | Compte admin authentifié, catégorie `id=3` | `id=3` | 1. DELETE `/api/categories/3`.<br>2. GET `/api/categories`. | 204 NoContent et catégorie absente. | Équivalence | Intégration |
| CAT-CRUD-06 | Refus suppression catégorie inexistante | Compte admin authentifié | `id=9999` | 1. DELETE `/api/categories/9999`. | 404 NotFound. | Équivalence | Intégration |

### 1.3 Gestion du panier

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| CART-01 | Ajouter un article au panier | Utilisateur authentifié, article `id=5` | `articleId=5`, `quantity=2` | 1. POST `/api/cart`.<br>2. GET `/api/cart`. | 200 OK + ligne panier quantité 2. | Équivalence | Intégration |
| CART-02 | Incrémenter quantité existante | Utilisateur authentifié, article déjà au panier | `articleId=5`, `quantity=1` | 1. POST `/api/cart` (même article).<br>2. GET `/api/cart`. | Quantité incrémentée. | Équivalence | Intégration |
| CART-03 | Modifier quantité | Utilisateur authentifié, cartItem `id=7` | `quantity=5` | 1. PUT `/api/cart/7`.<br>2. GET `/api/cart`. | Quantité mise à jour. | Limites (valeur haute) | Intégration |
| CART-04 | Supprimer une ligne panier | Utilisateur authentifié, cartItem `id=7` | `id=7` | 1. DELETE `/api/cart/7`.<br>2. GET `/api/cart`. | Ligne supprimée. | Équivalence | Intégration |
| CART-05 | Consulter le résumé panier | Utilisateur authentifié, panier non vide | N/A | 1. GET `/api/cart/summary`. | Total + quantités conformes. | Équivalence | Système |
| CART-06 | Refus accès panier sans JWT | Aucun token | N/A | 1. GET `/api/cart`. | 401 Unauthorized. | Équivalence | Système |

### 1.4 Gestion des commandes

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| ORD-01 | Passer une commande depuis un panier valide | Utilisateur authentifié, panier non vide | N/A | 1. POST `/api/order`.<br>2. GET `/api/order`. | 201 Created + commande dans l’historique. | Équivalence | Système |
| ORD-02 | Refus commande panier vide | Utilisateur authentifié, panier vide | N/A | 1. POST `/api/order`. | 400 BadRequest (panier vide). | Équivalence | Intégration |
| ORD-03 | Consulter l’historique | Utilisateur authentifié | N/A | 1. GET `/api/order`. | 200 OK + liste commandes utilisateur. | Équivalence | Système |
| ORD-04 | Consulter détail d’une commande | Utilisateur authentifié, commande `id=12` | `id=12` | 1. GET `/api/order/12`. | 200 OK + items + total. | Équivalence | Système |
| ORD-05 | Mettre à jour le statut (admin) | Admin authentifié, commande `id=12` | `status="Paid"` | 1. PATCH `/api/order/12/status`.<br>2. GET `/api/order/12`. | Statut mis à jour. | Équivalence | Intégration |
| ORD-06 | Refus statut (utilisateur non admin) | Utilisateur authentifié sans rôle admin | `status="Paid"` | 1. PATCH `/api/order/12/status`. | 403 Forbidden. | Équivalence | Système |

### 1.5 Authentification et erreurs JWT

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| AUTH-01 | Inscription valide | Aucun compte existant | `userName="alice"`, `email="alice@example.com"`, `password="Passw0rd!"` | 1. POST `/api/account/register`. | 200 OK, utilisateur créé. | Équivalence | Intégration |
| AUTH-02 | Connexion valide | Utilisateur existant | `userName="alice"`, `password="Passw0rd!"` | 1. POST `/api/account/login`. | 200 OK + JWT reçu. | Équivalence | Intégration |
| AUTH-03 | Refus connexion mauvais mot de passe | Utilisateur existant | `userName="alice"`, `password="BadPass"` | 1. POST `/api/account/login`. | 401 Unauthorized. | Équivalence | Intégration |
| AUTH-04 | Accès protégé sans token | Aucun token | N/A | 1. GET `/api/cart`. | 401 Unauthorized. | Équivalence | Système |
| AUTH-05 | Accès protégé avec token invalide | Token altéré | `Authorization: Bearer xxx` | 1. GET `/api/cart`. | 401 Unauthorized. | Robustesse | Système |

### 1.6 Recherche d’articles

| ID | Titre | Préconditions | Données de test | Étapes | Résultat attendu | Technique de test | Niveau |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SRCH-01 | Rechercher par nom exact | Articles existants | `name="pizza"` | 1. GET `/api/articles?name=pizza`. | 200 OK + articles correspondants. | Équivalence | Intégration |
| SRCH-02 | Filtrer par catégorie | Catégorie `id=2` avec articles | `categoryId=2` | 1. GET `/api/articles?categoryId=2`. | 200 OK + articles de la catégorie. | Équivalence | Intégration |
| SRCH-03 | Aucun résultat | Nom inexistant | `name="zzzz"` | 1. GET `/api/articles?name=zzzz`. | 200 OK + liste vide. | Équivalence | Système |

## 2) Cas de test non fonctionnels

| ID | Type | Objectif | Préconditions | Étapes | Résultat attendu |
| --- | --- | --- | --- | --- | --- |
| NF-PERF-01 | Performance (latence API) | Vérifier la latence moyenne sur `GET /api/articles` | API en charge minimale, 100 appels | 1. Mesurer la latence moyenne et p95 via script (ex: `pytest -k perf`). | Latence moyenne < 300 ms, p95 < 600 ms. |
| NF-SEC-01 | Sécurité (contrôle d’accès) | Vérifier l’accès admin sur `PATCH /api/order/{id}/status` | Compte non admin | 1. Envoyer la requête avec token utilisateur standard. | 403 Forbidden. |
| NF-SEC-02 | Sécurité (injection simple) | Vérifier la robustesse aux entrées malformées | API en marche | 1. Envoyer `name="' OR '1'='1"` sur `GET /api/articles?name=...`.<br>2. Vérifier la réponse. | 200 OK + aucun dump anormal; pas d’erreur SQL. |

## 3) Scénarios de test système (utilisateur final)

### SCN-01 — Parcourir les menus
1. Ouvrir l’application Blazor.
2. Naviguer vers les catégories et menus.
3. Ouvrir le détail d’un menu pour voir la liste d’articles.

**Résultat attendu :** les menus et leurs articles s’affichent correctement, sans erreur.

### SCN-02 — Ajouter au panier
1. Se connecter avec un compte utilisateur.
2. Rechercher un article, ouvrir sa fiche.
3. Ajouter l’article au panier, quantité 2.
4. Ouvrir la page panier.

**Résultat attendu :** la ligne apparaît avec la quantité 2 et le total mis à jour.

### SCN-03 — Passer une commande
1. Depuis le panier, cliquer « Passer commande ».
2. Confirmer la création de commande.
3. Ouvrir le récapitulatif commande.

**Résultat attendu :** commande créée (statut Pending) avec total calculé.

### SCN-04 — Lire l’historique
1. Accéder à la page « Historique des commandes ».
2. Sélectionner une commande pour afficher le détail.

**Résultat attendu :** liste des commandes avec détails cohérents.

## 4) Scripts automatisés

### 4.1 Structure recommandée
```
/tests
  /api
    conftest.py
    test_articles.py
    test_categories.py
    test_cart.py
    test_orders.py
    test_auth.py
    test_search.py
  /ui
    config.py
    conftest.py
    /pages
      base_page.py
      login_page.py
      cart_page.py
      order_page.py
    /tests
      test_login.py
      test_cart_order.py
```

### 4.2 Setup / Teardown (principes)
- **API** : fixture `api_client` lit `BASE_URL` et gère les headers; fixture `auth_token` crée un utilisateur ou utilise un utilisateur existant.
- **UI** : fixture `driver` initialise Selenium, `yield`, puis `quit()`.

### 4.3 Tests API Pytest (extraits)
- Les tests couvrent CRUD, panier, commandes, auth, recherche avec assertions sur les codes HTTP et les payloads.

### 4.4 Tests Selenium POM
- Modèle Page Object pour `LoginPage`, `CartPage`, `OrderPage`.
- Les tests pilotent login, ajout au panier, passage de commande.

## 5) Collection Postman
- La collection JSON est fournie dans `postman/MonResto.postman_collection.json`.
- Variables : `baseUrl`, `token`.

## 6) Tableau de traçabilité (vide)

| Exigence | Scénario | Cas de test | Résultat |
| --- | --- | --- | --- |
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |

## 7) Squelette de rapport final

### Introduction
- Contexte du projet.
- Périmètre fonctionnel testé.

### Organisation des tests
- Environnements.
- Jeux de données.
- Stratégie (unitaires, intégration, système).

### Résultats
- Synthèse des exécutions (taux de succès).
- Défauts majeurs observés (placeholder).

### Graphiques
- _Placeholder : ajouter histogrammes / courbes de tendance._

### Conclusion
- _Placeholder : à compléter manuellement._
