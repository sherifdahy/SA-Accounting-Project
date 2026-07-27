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

The user prefers reviewing concepts before large implementation passes.

## Current High-Priority Next Steps

1. Update Application layer to replace old `FileUrl` with multiple attachments.
2. Decide request/response shape for attachment creation.
3. Decide whether direct company attachment uploads need their own commands/controllers now or later.
4. Create EF migration after Application/Infrastructure shape is stable.
5. Update frontend after API contracts are stable.

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
ExpenseClaim 0..1 -> Movement
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
