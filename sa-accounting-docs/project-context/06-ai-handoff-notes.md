# AI Handoff Notes

Read this before starting work in a new session.

## Project Intent

This project is for accounting and law offices.

The office needs to manage companies, accounts, employees, daily expense claims, attachments, and custody movements.

The most important domain idea:

- documents and attachments should be easy to find later by company and by source.

## Work Style Preference

When changing the backend:

1. Discuss and stabilize the domain first.
2. Then align Infrastructure/database integrity.
3. Then update Application/API.
4. Avoid making broad changes across layers before the domain is agreed.
5. Keep repository + unit of work usage consistent.
6. Respect soft delete; do not hard delete business history unless explicitly requested.

The user prefers reviewing concepts before large implementation passes.

## Current High-Priority Next Steps

1. Create/update EF migration for the current `Attachment` model.
2. Decide whether direct company attachment uploads need their own commands/controllers now or later.
3. Update frontend after API contracts are stable.
4. Consider whether API response fields should be renamed from `Files` to `Attachments`.

## Persistence Pattern Reminder

The project uses:

- `IUnitOfWork`
- generic repositories
- specialized repositories only when needed

Application handlers should normally use `IUnitOfWork` instead of directly injecting `ApplicationDbContext`.

## Application Validation Reminder

Use-case validation belongs to Commands and Queries, not only to API request
contracts.

For authentication:

- `LoginRequest` is an API transport DTO.
- `LoginCommand` is the Application use-case input.
- `LoginCommandValidator` owns login input validation.
- MediatR `ValidationBehavior` must run before `LoginCommandHandler`.

Do not move application rules back into request-only validators. Keep request
validators only for rules that are specific to HTTP transport concerns.

## Soft Delete Reminder

The project uses soft delete/status flags for business records.

Examples:

- `Company.IsDeleted`
- `Account.IsDeleted`
- `Owner.IsDeleted`
- `Platform.IsDeleted`
- `Selector.IsDeleted`
- `ExpenseCategory.IsDisabled`
- `Custody.IsDisabled`

Normal user-facing queries should exclude deleted/disabled records unless the flow explicitly needs historical/admin visibility.

## Attachment Design Reminder

Attachment belongs to company first.

```text
Attachment.CompanyId required
Attachment.ExpenseClaimItemId nullable
```

This answers:

- "Which company does this attachment belong to?"
- "Did it come from an expense claim item?"

Do not reintroduce `ExpenseClaimItem.FileUrl`.

## Expense Settlement Reminder

An `ExpenseClaim` is not money movement by itself.

When it is settled, one `Movement` of type `ApprovedExpense` is created.

```text
ExpenseClaim 0..1 -> CustodyMovement
Custody 1 -> many Movement
```

Do not model `ExpenseClaim` as one-to-many with `Movement`.

## Activity Log Reminder

Do not add a generic `ActivityLog` yet.

Use real domain entities for history until the user asks for generic logging.

## Useful Files

Core:

- `sa-accounting-back-end/SA.Accounting.Core/Entities`
- `sa-accounting-back-end/SA.Accounting.Core/Enums`

Infrastructure:

- `sa-accounting-back-end/SA.Accounting.Infrastructure/Presistance/Data/ApplicationDbContext.cs`
- `sa-accounting-back-end/SA.Accounting.Infrastructure/Presistance/Data/Config`

Application:

- `sa-accounting-back-end/SA.Accounting.Application/Contracts`
- `sa-accounting-back-end/SA.Accounting.Application/Handlers`

ERD:

- `sa-accounting-docs/ERD.drawio`
