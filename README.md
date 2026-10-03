# Post Office REST API - Clean Architecture

Backend implementation of the supplied Post Office developer task, using ASP.NET Core/.NET 10, PostgreSQL and EF Core. The original task asks for TypeScript/Node.js, but this implementation intentionally uses .NET as requested.

## Architecture

- `PostOffice.Domain` - entities, enums and domain rules. No framework dependencies.
- `PostOffice.Application` - use cases/services, DTOs, repository abstractions. No EF Core.
- `PostOffice.Infrastructure` - EF Core/PostgreSQL and repository implementations.
- `PostOffice.Api` - HTTP controllers, middleware/exception handling and composition root.
- `PostOffice.Tests` - unit tests.

MediatR is deliberately not used. Controllers call application services directly.

## Features

- CRUD for post offices.
- CRUD for shipments.
- Letter and Package inheritance.
- Unique shipment number and ZIP code.
- Shipment status history.
- Current post-office tracking.
- Package/letter filtering by status, current location, weight category and shipment number.
- Pagination.
- PostgreSQL with EF Core.
- Global ProblemDetails exception handling.
- Unit tests.

## Run

Prerequisites:
- .NET 10 SDK
- Docker Desktop

Start PostgreSQL:

```bash
docker compose up -d
```

Run API:

```bash
dotnet restore
dotnet build
dotnet run --project src/PostOffice.Api
```

EF Core migrations are applied automatically at startup.

## Example requests

Create post office:

```http
POST /api/post-offices
Content-Type: application/json

{
  "zipCode": "500001",
  "name": "Hyderabad Central",
  "city": "Hyderabad"
}
```

Create package:

```http
POST /api/shipments
Content-Type: application/json

{
  "shipmentNumber": "PKG-10001",
  "shipmentType": "Package",
  "weightKg": 2.5,
  "originPostOfficeId": "<origin-guid>",
  "destinationPostOfficeId": "<destination-guid>"
}
```

Filter packages, page 1:

```http
GET /api/shipments?shipmentType=Package&status=ReceivedAtOrigin&weight=Between1And5Kg&pageNumber=1&pageSize=20
```

Filter by current post office:

```http
GET /api/shipments?shipmentType=Package&locationPostOfficeId=<post-office-guid>
```

Update status:

```http
PATCH /api/shipments/<shipment-guid>/status
Content-Type: application/json

{
  "status": "ReceivedAtDestination",
  "postOfficeId": "<destination-guid>"
}
```

## Design note

The task says a shipment can have multiple statuses. Instead of overwriting the history, the aggregate stores the current status plus `ShipmentStatusHistory`. This keeps the current state fast to query while preserving the status transitions.
