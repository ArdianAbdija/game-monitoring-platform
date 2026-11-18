# Architecture notes

- `GameMonitoring.Data` holds the shared EF Core entities and `GameMonitoringContext`.
- `GameMonitoring.Business` holds shared DTOs and AutoMapper profiles.
- `WebAPI` owns database operations and exposes REST endpoints.
- `WebApp` uses `GameMonitoringApiClient` to call the API and maps DTOs into MVC view models.

Run the API and MVC website together. The MVC application's API base URL is `http://localhost:5111/api/`; its own HTTP launch profile uses `http://localhost:5014`.

See [README.md](README.md) for database setup, local secret configuration and the correct demonstration account credentials. See [repository review](docs/REPOSITORY_REVIEW.md) for known limitations, including incomplete authorization.
