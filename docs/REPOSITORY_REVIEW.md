# Repository review and preparation

This review covers the uploaded DWA Game Monitoring Platform source and the preparation of a separate GitHub-ready copy. It is a static review, not a comprehensive security audit or evidence of a successful runtime deployment.

## Changes made for GitHub

- Excluded 539 files from Visual Studio state, `bin`/`obj` build output and user-specific settings.
- Removed the machine-specific SQL connection string and the JWT signing key from tracked API configuration. Added a `UserSecretsId` to the API project and setup instructions for local secrets.
- Added a README, `.gitignore` and `.gitattributes`.
- Corrected the outdated demonstration passwords in the architecture notes by pointing to the verified credentials in the README.
- Replaced demonstration emails with `example.com` addresses and demonstration phone numbers with SQL `NULL` values.
- Excluded the existing local review image. The base SQL script now uses an external game-image URL instead of referencing that local upload.
- Retained the bundled third-party libraries and their license files.

Application C# source files and Razor views were not refactored. The authorization gaps below remain present and are not claimed as fixed.

If the original database password or JWT key is used anywhere beyond this disposable project, replace it in that environment before sharing or deploying the application. The removed values are not reproduced here.

## Findings to address

| Priority | Finding | Relevant source |
| --- | --- | --- |
| High | API catalogue writes, review moderation and user-profile operations do not consistently require an authenticated caller or enforce roles/ownership. Checking route and body IDs alone does not establish ownership. | [GamesController](../WebAPI/Controllers/GamesController.cs), [ReviewsController](../WebAPI/Controllers/ReviewsController.cs), [UsersController](../WebAPI/Controllers/UsersController.cs), [GenresController](../WebAPI/Controllers/GenresController.cs), [GameTypesController](../WebAPI/Controllers/GameTypesController.cs) |
| High | MVC management controllers do not enforce the administrator role. Hiding management navigation links is not an access-control boundary. | [GamesController](../WebApp/Controllers/GamesController.cs), [ReviewsController](../WebApp/Controllers/ReviewsController.cs), [GenresController](../WebApp/Controllers/GenresController.cs), [GameTypesController](../WebApp/Controllers/GameTypesController.cs) |
| High | User-list/profile API responses are public, and game/review responses may include unapproved reviews. Review approval is filtered for display in the browse view, not consistently at the API boundary. | [UsersController](../WebAPI/Controllers/UsersController.cs), [GamesController](../WebAPI/Controllers/GamesController.cs), [ReviewsController](../WebAPI/Controllers/ReviewsController.cs), [browse details view](../WebApp/Views/Browse/Details.cshtml) |
| Medium | Review upload handling checks extensions, but does not validate image signatures or apply a dedicated per-file size cap. | [BrowseController](../WebApp/Controllers/BrowseController.cs) |
| Medium | JSON profile-update actions use session identity but lack antiforgery validation. | [ProfileController](../WebApp/Controllers/ProfileController.cs), [UserProfileController](../WebApp/Controllers/UserProfileController.cs) |
| Medium | Request validation and failure handling need improvement; examples include uncapped search page sizes, invalid review references on updates and GET API calls throwing on non-success responses. | [GamesController](../WebAPI/Controllers/GamesController.cs), [ReviewsController](../WebAPI/Controllers/ReviewsController.cs), [API client](../WebApp/Services/GameMonitoringApiClient.cs) |

The log API has `[Authorize]`, but does not restrict access to administrators. Treat the current HTTP launch profiles and seeded credentials as local-demo settings.

## Verification status

Completed during preparation:

- Reviewed the solution/project structure, API and MVC flows, DTO mappings, configuration and SQL setup scripts.
- Checked both seeded BCrypt hashes against the passwords documented in the README.
- Checked the prepared package for the removed database credentials/signing key, local machine references, generated build output and local uploaded files.
- Checked JSON and project XML syntax, solution/project references, local Markdown links and `.gitignore` coverage.

Not run in this environment:

- `dotnet restore` or `dotnet build` (.NET SDK is not installed here).
- SQL script execution (no local SQL Server instance).
- API/MVC startup, browser flows or end-to-end tests.
- Package vulnerability scanning or automated security testing.

## Local checks before presenting a live demo

1. Configure local secrets, run the SQL setup against a disposable database, and restore/build the solution.
2. Start the API and MVC app; check the catalogue, search, type filter, pagination and game details.
3. Register a new user and verify login/logout and profile updates.
4. Submit a review with an image; confirm it remains hidden on the browse page until approved.
5. Use the management screens to approve/reject reviews and create/edit/delete a test game.
6. Verify that the log endpoint rejects requests without a valid token.
7. After implementing authorization, verify that anonymous users, ordinary users and administrators receive the correct access, including direct API requests.

## Portfolio wording

Suggested description, grounded in the source:

> Built an individual university game catalogue and review application using ASP.NET Core MVC, a separate REST API, EF Core and SQL Server, with DTO mapping, paginated search, BCrypt/JWT login and review moderation workflows.

Do not describe the current version as production-ready, comprehensively access-controlled, deployed or automatically tested. Add genuine runtime screenshots and test evidence after local verification.
