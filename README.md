# CampusEquipment

An enterprise-style university equipment management application built with ASP.NET Core, Entity Framework Core (Database-First), and SQL Server.

## Architecture

```
CampusEquipment.Web (MVC)          CampusEquipment.API (REST)
        │                                   │
        └───────────────┬───────────────────┘
                        ▼
            CampusEquipment.Core
            ├── DTOs
            ├── Entities (scaffolded from DB)
            ├── Repositories (interfaces)
            └── Services (interfaces)
                        ▲
                        │
            CampusEquipment.Infrastructure
            ├── Data (AppDbContext)
            ├── Repositories (implementations)
            └── Services (implementations + business rules)
                        │
                        ▼
                  SQL Server
                  CampusEquipmentDb
```

**Key principle:** MVC and API share the same service layer. Business rules live in `EquipmentService` and are not duplicated across controllers.

## Projects

| Project | Purpose |
|---------|---------|
| `CampusEquipment.Core` | DTOs, entity classes, repository and service interfaces |
| `CampusEquipment.Infrastructure` | EF Core `AppDbContext`, repository and service implementations |
| `CampusEquipment.Web` | ASP.NET Core MVC frontend |
| `CampusEquipment.API` | ASP.NET Core Web API with Swagger |

## Database-First Development

This project uses **Database-First** development:

1. SQL Server database `CampusEquipmentDb` was created first (`CampusEquipmentDb.sql`)
2. EF Core scaffolding generated the entities and `AppDbContext`:

   ```
   Scaffold-DbContext "Server=(local);Database=CampusEquipmentDb;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities -ContextDir Data -Context AppDbContext -DataAnnotations
   ```

3. Entities were moved from `Infrastructure/Entities` to `Core/Entities` to maintain correct dependency direction (`Core` → no dependencies).

## Business Rules

All enforced in `EquipmentService` (`Infrastructure/Services/EquipmentService.cs`):

| # | Rule |
|---|------|
| 1 | Asset Code must be unique across all equipment |
| 2 | Retired equipment cannot be assigned |
| 3 | Equipment under maintenance cannot be assigned |
| 4 | Department must exist when creating or updating equipment |
| 5 | Required fields (AssetCode, Name, Category, Status, DepartmentId) must be valid |

Violations throw `BusinessRuleException`, which the API converts to **HTTP 409 Conflict** and the MVC layer converts to a friendly error message.

## Features

### MVC Frontend
- Equipment list with search (asset code, name, brand) and filters (category, status, department)
- Create / Details / Edit pages
- Retire (soft delete) and Delete (hard delete) actions with confirmations
- Color-coded status badges

### REST API
- `GET /api/equipment`
- `GET /api/equipment/{id}`
- `POST /api/equipment`
- `PUT /api/equipment/{id}`
- `DELETE /api/equipment/{id}`
- `GET /api/departments`

Swagger UI available at `/swagger` when running the API project in Development.

## Getting Started

1. Run `CampusEquipmentDb.sql` in SQL Server Management Studio to create the database.
2. Open `CampusEquipment.sln` in Visual Studio 2022.
3. Restore NuGet packages (they auto-restore on first build).
4. Set `CampusEquipment.Web` or `CampusEquipment.API` as the startup project.
5. Press `F5`.

The connection string is in `appsettings.json` of both the Web and API projects:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=CampusEquipmentDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

## Project Structure

```
CampusEquipment
├── CampusEquipment.Core
│   ├── DTOs
│   ├── Entities
│   ├── Repositories
│   └── Services
├── CampusEquipment.Infrastructure
│   ├── Data
│   ├── Repositories
│   └── Services
├── CampusEquipment.Web
│   ├── Controllers
│   ├── Middleware
│   └── Views/Equipment
├── CampusEquipment.API
│   ├── Controllers
│   └── Middleware
├── CampusEquipmentDb.sql
└── CampusEquipment.sln
```

## Group Responsibilities

| Student | Responsibility |
|---------|----------------|
| Student 1 | Database, EF Core scaffolding, repository layer, service layer, business rules, DI wiring |
| Student 2 | MVC frontend (views and controller), REST API controllers, Swagger, error middleware, UI validation |

## Tech Stack

- ASP.NET Core 9 MVC
- ASP.NET Core 9 Web API
- Entity Framework Core 9 (Database-First)
- SQL Server
- Swashbuckle (Swagger)
- Repository + Service pattern
- Dependency Injection