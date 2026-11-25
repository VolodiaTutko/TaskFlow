# TaskFlow

TaskFlow is a team task management platform
built with ASP.NET Core.

## Planned Features

- Authentication and role-based access control
- Workspaces and projects
- Kanban task management
- Real-time collaboration using SignalR
- Smart task dependencies
- Automation Rules Engine
- Task history and version restoration

## Tech Stack

**Backend:** ASP.NET Core, C#, EF Core, PostgreSQL

**Architecture:** Modular Monolith, Clean Architecture

**Frontend:** React, TypeScript

**Infrastructure:** Docker, Hangfire, SignalR

**Testing:** xUnit, Testcontainers

## Project Status

Under active development.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### 1. Start PostgreSQL

```bash
copy .env.example .env
```

Open `.env` and set `POSTGRES_PASSWORD`, then:

```bash
docker compose up -d
```

PostgreSQL is available on `localhost:5433`.

### 2. Configure the connection string (User Secrets)

The password is never stored in Git. Set the full connection string locally:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=taskflow_db;Username=taskflow_user;Password=<POSTGRES_PASSWORD>" --project src/TaskFlow.Api
```

In Production, pass it via the `ConnectionStrings__DefaultConnection` environment variable.

### 3. Run the API

```bash
dotnet run --project src/TaskFlow.Api --launch-profile https
```

### 4. Run the tests

```bash
dotnet test
```

## Endpoints

| URL | Description | Environment |
|---|---|---|
| `/health` | Liveness: the API process is running | All |
| `/health/ready` | Readiness: the API can reach PostgreSQL | All |
| `/openapi/v1.json` | OpenAPI document | Development |
| `/swagger` | Swagger UI | Development |

## Architecture

See [Architecture](docs/architecture/README.md).

## License

No license has been selected yet.