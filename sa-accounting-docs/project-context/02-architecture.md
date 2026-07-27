# Architecture

The backend follows Clean Architecture-style layering.

Repository path:

- `sa-accounting-back-end`

Main backend projects:

- `SA.Accounting.Core`
- `SA.Accounting.Application`
- `SA.Accounting.Infrastructure`
- `SA.Accounting.Services`
- `SA.Accounting.API`

Frontend path:

- `sa-accounting-front-end`

Docs path:

- `sa-accounting-docs`

## Layer Responsibilities

### Core

Path:

- `sa-accounting-back-end/SA.Accounting.Core`

Contains domain entities, enums, core interfaces, and constants.

This is where the business model should be expressed first.

Important current namespaces:

- `SA.Accounting.Core.Entities.Companies`
- `SA.Accounting.Core.Entities.ExpenseClaims`
- `SA.Accounting.Core.Entities.Custodies`
- `SA.Accounting.Core.Entities.Attachments`
- `SA.Accounting.Core.Entities.Platforms`
- `SA.Accounting.Core.Entities.Identity`
- `SA.Accounting.Core.Entities.Relations`

### Application

Path:

- `sa-accounting-back-end/SA.Accounting.Application`

Contains commands, queries, handlers, validators, request/response contracts, mapping, and application-level rules.

This layer may temporarily lag behind Core when domain decisions are being made.

### Infrastructure

Path:

- `sa-accounting-back-end/SA.Accounting.Infrastructure`

Contains EF Core persistence, `ApplicationDbContext`, entity configurations, migrations, repositories, and Unit of Work implementation.

Database integrity rules should live in entity config files where possible.

### API

Path:

- `sa-accounting-back-end/SA.Accounting.API`

Contains controllers, dependency wiring, exception handling, and HTTP surface.

### Services

Path:

- `sa-accounting-back-end/SA.Accounting.Services`

Contains domain-supporting services such as number generation, auth support, email support, and custody balance calculation.

## Patterns In Use

- Clean Architecture style separation
- CQRS-style commands and queries
- MediatR
- Repository pattern
- Unit of Work
- EF Core fluent configuration
- ASP.NET Identity

## Architecture Guideline

When changing a feature:

1. Start with the domain model in `Core`.
2. Reflect database constraints in `Infrastructure`.
3. Update `Application` requests, handlers, validators, and mapping.
4. Update API/frontend only after the model is stable.
