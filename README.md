# Game Monitoring Platform

A game catalogue and review application built as an individual project for the DWA university course at Algebra Bernays. Users can browse games, submit ratings and reviews, and attach images. Management screens support maintaining the catalogue and moderating reviews.

**Author:** Ardian Abdija  
**Stack:** C# · .NET 8 · ASP.NET Core MVC / Web API · Entity Framework Core · SQL Server · Razor · Bootstrap

This repository presents a university project for local demonstration. The code contains the features described below; an end-to-end run of this prepared repository has not yet been verified. Authentication is implemented, but authorization is incomplete. See [known limitations](#known-limitations).

## Features in the source

- Game browsing with name search, game-type filtering and pagination.
- Game detail pages with genres, approved reviews and average ratings calculated from approved reviews.
- Registration and login using BCrypt password hashing and API-issued JWTs; the MVC application stores login details in its session.
- Reviews with ratings from 1 to 10 and a database constraint limiting each user to one review per game.
- Review image uploads and an approval/rejection workflow. The browse view shows approved reviews.
- CRUD screens and API endpoints for games, genres, game types and reviews.
- User profile editing.
- Database log entries for game creation, updates and deletion, plus a JWT-protected log endpoint and a small HTML log viewer.
- Swagger UI for exploring the API in Development.

## Architecture

The MVC application calls the Web API through a typed `HttpClient`. The API uses EF Core to read and write SQL Server data. DTOs are mapped into MVC view models before rendering Razor views.

| Project / folder | Responsibility |
| --- | --- |
| `WebApp` | MVC controllers, Razor views, view models, session handling and `GameMonitoringApiClient` |
| `WebAPI` | REST endpoints, JWT issuance and validation, database operations and Swagger |
| `GameMonitoring.Business` | Shared DTOs and AutoMapper profiles |
| `GameMonitoring.Data` | EF Core entities, `GameMonitoringContext` and relationship configuration |
| `Database` | SQL Server schema and demonstration data |

The `Business` project currently contains data contracts and mappings; it is not a separate domain-service layer. The API controllers perform the application and database operations.

The relational model includes games, game types, genres, users, reviews, images and logs. `GameGenre` represents the many-to-many game/genre relationship; review images belong to individual reviews.

## Run locally

### 1. Prerequisites

- .NET 8 SDK.
- A local SQL Server instance and a SQL client such as SQL Server Management Studio.
- Visual Studio with ASP.NET/web development support, or another editor plus the .NET CLI.

Run the commands below from the repository root. Configuration examples use PowerShell.

### 2. Create the demonstration database

Open [Database/Database.sql](Database/Database.sql) in your SQL client and execute it against a local test SQL Server instance. It creates `GameMonitoringPlatform`, its tables, six games and two demonstration accounts.

**This script drops and recreates existing project tables, deleting their data. Use a disposable local database.**

Optionally execute [Database/Seed.sql](Database/Seed.sql) afterwards to add more games and reviews. That script also clears existing `ReviewImage` rows before inserting demonstration image links; do not run it against data you want to retain.

The project uses SQL setup scripts. No EF Core migrations are included.

### 3. Configure the API locally

The database connection string and JWT signing key are intentionally absent from tracked configuration. The API project has a `UserSecretsId`, so Development can load these settings from .NET User Secrets.

Set a connection string for your local SQL Server instance. This example uses Windows authentication; change `localhost` if your instance has another name:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GameMonitoringPlatform;Trusted_Connection=True;TrustServerCertificate=True;" --project WebAPI
```

If your server requires SQL authentication, store your own connection string with the same command. Keep credentials out of `appsettings.json`. `TrustServerCertificate=True` is intended for this local demonstration setup.

Generate a local JWT signing key:

```powershell
$jwtBytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtBytes)
$rng.Dispose()
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String($jwtBytes)) --project WebAPI
```

User Secrets are for local development and are not an encrypted production secret store. Environment variables `ConnectionStrings__DefaultConnection` and `Jwt__Key` can also supply these settings. Both values must be configured before using the API.

### 4. Restore and build

```powershell
dotnet restore GameMonitoringPlatform.sln
dotnet build GameMonitoringPlatform.sln --no-restore
```

### 5. Start both applications

In the first terminal:

```powershell
dotnet run --project WebAPI --launch-profile http
```

In a second terminal:

```powershell
dotnet run --project WebApp --launch-profile http
```

| Application | Local address |
| --- | --- |
| MVC website | `http://localhost:5014` |
| API Swagger UI | `http://localhost:5111/swagger` |
| API base URL | `http://localhost:5111/api/` |
| HTML log viewer login | `http://localhost:5111/login.html` |

The MVC application's `ApiSettings:BaseUrl` must match the API address, including the trailing `/api/`. Both `WebApp/appsettings.json` and `WebApp/appsettings.Development.json` currently specify the same local URL. For a local override, set the `ApiSettings__BaseUrl` environment variable in the terminal that starts the MVC app.

### 6. Demonstration accounts

These accounts are created by `Database.sql` and are only for a disposable local demo:

| Username | Password | Role |
| --- | --- | --- |
| `admin` | `Admin123!` | Admin |
| `user1` | `User123!` | User |

The optional seed adds `user2`, `user3` and `user4`, each using the same demonstration password as `user1`. The stored BCrypt hashes were checked against these documented passwords during repository preparation; application login still needs a runtime check.

## Selected API endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Register an account |
| `POST` | `/api/auth/login` | Obtain a JWT and basic user details |
| `GET` | `/api/games/search?name=Elden&page=1&count=6` | Search and paginate games |
| `GET` | `/api/games/{id}` | Get game details |
| `PUT` | `/api/games/{id}/genres` | Assign genres to a game |
| `POST` | `/api/reviews` | Submit a review |
| `PUT` | `/api/reviews/{id}/approve` | Approve a review |
| `GET` | `/api/logs/get/10` | Get recent logs; requires a bearer token |

The presence of a JWT login flow does not mean every endpoint enforces authorization.

## Known limitations

- Several catalogue, review and user endpoints lack authentication/role/ownership checks. Management links vary by session role, but management controllers also need server-side authorization. Keep the application local until those checks are implemented.
- Public API responses can include pending reviews and user profile data. The approved-only filter is applied in the browse view, so API visibility rules need improvement.
- Uploads are checked by extension, without a content-signature check or a dedicated per-file size limit.
- Error handling, request validation, pagination limits and automated testing need further work.
- There is no included automated test project, CI pipeline or deployment configuration.

See [review and preparation notes](docs/REPOSITORY_REVIEW.md) for the exact scope of the review, changes made for GitHub and the local verification steps still outstanding.

## Third-party assets

Bootstrap, jQuery and validation libraries are bundled under `WebApp/wwwroot/lib`; their license files are retained. Seed data references external Steam game artwork, which requires internet access and remains the property of its respective owners. Local user-uploaded images are excluded from version control.
