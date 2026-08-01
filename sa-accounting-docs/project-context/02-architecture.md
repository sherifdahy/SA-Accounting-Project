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

#### Validation Ownership

Validation for an application use case belongs to its command or query, not to the
transport request DTO.

Rules:

- API request models are transport contracts used for model binding and mapping.
- Application validators target `Command` or `Query` types.
- MediatR validation behavior runs validators before the handler executes.
- A handler must not depend on an API request validator having run first.
- Request-specific validation is allowed only when it is strictly about the HTTP
  transport and is not an application rule.

For example, login validation is owned by `LoginCommandValidator`. The API may
receive a `LoginRequest`, map it to `LoginCommand`, and send the command through
MediatR. This keeps the use case valid even when it is invoked outside an HTTP
controller.

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
- Soft delete for business records that should not disappear from history

## Data Access Pattern

Application handlers should access persistence through `IUnitOfWork` and repositories, not directly through `ApplicationDbContext`.

Current pattern:

- `IUnitOfWork` lives in `SA.Accounting.Core`.
- `UnitOfWork` implementation lives in `SA.Accounting.Infrastructure`.
- Generic repositories expose common data operations.
- Specialized repositories can be added only when the generic repository is not expressive enough.

Use this pattern consistently for commands and queries unless there is a clear project-level decision to do otherwise.

## Soft Delete Policy

The project uses soft delete for business entities where historical data matters.

Examples already using soft-delete style flags include:

- `Company.IsDeleted`
- `Account.IsDeleted`
- `Owner.IsDeleted`
- `Platform.IsDeleted`
- `Selector.IsDeleted`

Some entities use status flags instead of `IsDeleted`, such as disabled categories or disabled custodies.

Soft-deleted records should normally be hidden from regular queries, but kept in the database for history, integrity, and auditability.

When adding new business entities, decide explicitly whether they need:

- `IsDeleted`
- `IsDisabled`
- another status field
- no soft-delete behavior

## Architecture Guideline

When changing a feature:

1. Start with the domain model in `Core`.
2. Reflect database constraints in `Infrastructure`.
3. Update `Application` commands/queries, handlers, validators, and mapping. Keep
   use-case validation on commands/queries rather than transport request DTOs.
4. Update API/frontend only after the model is stable.
