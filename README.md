# CSharpLibraryApi

API REST ASP.NET Core pour gérer une bibliothèque de livres (recherche, consultation, création, modification et suppression).

## Endpoints `BooksController`

La ressource est accessible sous `/api/books`. Les réponses des endpoints de lecture et d'écriture contiennent les champs `id`, `title`, `author`, `isbn`, `genre`, `publicationYear` et `isAvailable`.

| Méthode et URL | Description | Réponses principales |
| --- | --- | --- |
| `GET /api/books` | Liste les livres, triés par titre. Les paramètres de recherche sont facultatifs et cumulables : `title`, `author` et `genre` filtrent par correspondance partielle, `isbn` par correspondance exacte, et `available` par disponibilité (`true` ou `false`). | `200 OK` |
| `GET /api/books/{id}` | Retourne le livre correspondant à son identifiant. | `200 OK`, `404 Not Found` |
| `POST /api/books` | Crée un livre. La disponibilité est initialisée à `true`. | `201 Created` (avec l'en-tête `Location`), `400 Bad Request` si les données sont invalides, `409 Conflict` si l'ISBN existe déjà |
| `PUT /api/books/{id}` | Remplace les informations du livre identifié, y compris sa disponibilité. | `200 OK`, `400 Bad Request` si les données sont invalides, `404 Not Found`, `409 Conflict` si l'ISBN est déjà attribué à un autre livre |
| `DELETE /api/books/{id}` | Supprime le livre identifié. | `204 No Content`, `404 Not Found` |

Les corps des requêtes `POST` et `PUT` sont au format JSON. `title` et `author` sont obligatoires (respectivement 1 à 200 et 1 à 150 caractères), `isbn` est obligatoire et doit être un ISBN valide à 10 ou 13 caractères, `genre` est facultatif (60 caractères maximum) et `publicationYear` est facultatif (de 1450 à 2100). Pour `PUT`, `isAvailable` est également accepté et vaut `true` par défaut.

Exemple de création :

```json
{
  "title": "Le Hobbit",
  "author": "J. R. R. Tolkien",
  "isbn": "978-0618002214",
  "genre": "Fantasy",
  "publicationYear": 1937
}
```

Exemples de recherche :

- `GET /api/books?title=hobbit`
- `GET /api/books?author=tolkien&available=true`

## Swagger en local

Depuis la racine du projet, démarrez l'application avec `dotnet run` (profil HTTP local sur le port `5238`).
En environnement de développement, l'interface Swagger est disponible à l'adresse :

**http://localhost:5238/swagger/index.html**
