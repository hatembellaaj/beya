# GUI pour lancer les tests

Cette mini-interface web permet de lancer les tests **API** et **UI** depuis un navigateur et d'afficher le résultat dans une page HTML.

## Prérequis

```bash
pip install flask pytest pytest-html
```

## Lancer l'interface

```bash
python tests/gui/app.py
```

Ouvrez ensuite : `http://localhost:5050`.

## Notes
- Les rapports HTML sont générés dans `tests/gui/reports/`.
- Pour les tests API, renseignez `BASE_URL` (ex: `http://localhost:5000`).
- Pour les tests UI, renseignez `UI_BASE_URL` (ex: `http://localhost:5002`).
- Si l'API expose un certificat auto-signé, cochez **Désactiver la vérification SSL** ou définissez `API_SSL_VERIFY=false`.
