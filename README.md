# Inventory Management

Article CRUD platform built with .NET 10, SQLite, Entity Framework Core 10, React 19, TypeScript and Vite.

## Scope

The application manages articles with:

- Unique EAN-13 reference
- Name
- Price excluding tax (HT)
- Price including tax (TTC)

Business rules are enforced in the domain:

- Reference must be a valid EAN-13 code
- Reference must be unique
- Name is required
- Prices cannot be negative
- TTC price must be greater than or equal to HT price

## Architecture

Backend follows a lightweight hexagonal / DDD structure:

```txt
Api -> Application -> Domain
Api -> Infrastructure -> Application -> Domain
Domain -> no dependency
```

Projects:

- `InventoryManagement.Domain`: aggregate, value objects and business rules
- `InventoryManagement.Application`: use cases and ports
- `InventoryManagement.Infrastructure`: SQLite, EF Core and repository adapters
- `InventoryManagement.Api`: REST controller adapter
- `InventoryManagement.Tests`: domain tests

## Backend

Run the API:

```bash
dotnet run --project backend/InventoryManagement.Api/InventoryManagement.Api.csproj --launch-profile http
```

API base URL:

```txt
http://localhost:5056
```

OpenAPI document:

```txt
http://localhost:5056/openapi/v1.json
```

Endpoints:

```txt
GET    /api/articles
GET    /api/articles/{id}
POST   /api/articles
PUT    /api/articles/{id}
DELETE /api/articles/{id}
```

SQLite database is created automatically as `inventory.db` when the API starts.

## Frontend

Run the React app:

```bash
cd frontend
npm install
npm run dev
```

Frontend URL:

```txt
http://localhost:5173
```

The Vite dev server proxies `/api` to `http://localhost:5056`.

## Verification

Backend:

```bash
dotnet build InventoryManagement.slnx
dotnet test InventoryManagement.slnx
```

Frontend:

```bash
cd frontend
npm run build
```
