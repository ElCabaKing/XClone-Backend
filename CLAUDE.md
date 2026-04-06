# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet restore                        # Restore NuGet packages
dotnet build                          # Build the solution
dotnet run --project X.WebApi         # Run the API (http://localhost:5223)
dotnet watch run --project X.WebApi   # Run with hot reload

# EF Core migrations (run from solution root)
dotnet ef migrations add <Name> --project X.Infrastructure --startup-project X.WebApi
dotnet ef database update --project X.Infrastructure --startup-project X.WebApi
```

## Architecture

This is a **Clean Architecture** Twitter/X clone backend (.NET 10, C#). The solution (`X.slnx`) has five projects with strict dependency flow:

```
X.WebApi → X.Application → X.Domain
X.WebApi → X.Infrastructure → X.Domain
X.Shared (cross-cutting, currently empty)
```

**X.Domain** — No external dependencies. Contains domain entities (`User`), value objects with validation (`Email`, `Username`), enums (`UserStatusEnum`), and repository interfaces (`IUserRepository`).

**X.Application** — Orchestrates use cases. Commands go in `DTOs/Command/`, responses in `DTOs/Response/`, and business logic in `UseCases/<Entity>/`. Currently only `CreateUser` exists as a stub.

**X.Infrastructure** — EF Core with SQL Server. Persistence models live in `Persistence/` (separate from domain entities). `Mappers/` handles bidirectional conversion between domain and persistence layers. `Repository/` implements the interfaces defined in Domain.

**X.WebApi** — Minimal API entry point. No endpoints exist yet. Middleware goes in `Middlewares/`.

## Database

- **ORM**: Entity Framework Core 10.0.5 with SQL Server
- **DbContext**: `X.Infrastructure/Database/SqlServer/Context/XDbContext.cs`
- **Connection string** in `X.WebApi/appsettings.json` → `ConnectionStrings:DefaultConnection`
- Default config points to `localhost:1433` with user `sa`
- The database schema is fully defined (15 tables): users, posts, comments, surveys, hashtags, follows, likes, bans, mutes, chat rooms, messages, notifications
- Domain entities and persistence models are **separate** — always use `UserMapper` (or equivalent) to convert between them

## Key Patterns

- **Value Objects**: Domain validation lives in value objects, not in services. `Email` and `Username` throw on invalid input in their constructors.
- **Repository Pattern**: Domain layer defines interfaces; Infrastructure implements them. Use cases depend only on the interface.
- **Persistence vs Domain entities**: `X.Infrastructure/Persistence/User.cs` is the EF model; `X.Domain/Entities/User.cs` is the domain entity. Do not conflate them.
- **Use Cases**: Each use case is a class in `X.Application/UseCases/<Entity>/`. Keep use cases thin — delegate validation to domain, persistence to repositories.
