# Database Integrity

This document describes intended database integrity rules.

Database mapping lives in:

- `sa-accounting-back-end/SA.Accounting.Infrastructure/Presistance/Data/ApplicationDbContext.cs`
- `sa-accounting-back-end/SA.Accounting.Infrastructure/Presistance/Data/Config`

## ApplicationDbContext

`ApplicationDbContext` applies all entity configurations from the Infrastructure assembly:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
```

It also changes cascade deletes to restrict by default:

```csharp
fk.DeleteBehavior = DeleteBehavior.Restrict
```

This is intentional. Avoid accidental deletion chains for company, expense, attachment, custody, and movement data.

## Attachment Integrity

Current intended rules:

- `Attachment.Id` is a `Guid`.
- `Attachment.FileName` is required.
- `Attachment.FileName` max length: `250`.
- `Attachment.StoredFileName` is required.
- `Attachment.StoredFileName` max length: `250`.
- `Attachment.ContentType` max length: `100`.
- `Attachment.FileExtension` max length: `10`.
- `Attachment.Note` max length: `1000`.
- `Attachment.CompanyId` is required.
- `Attachment.ExpenseClaimItemId` is optional.
- Index on `CompanyId`.
- Index on `ExpenseClaimItemId`.
- Delete behavior should be restricted.

Required constraints:

- `FileName` should not be empty or whitespace.
- `StoredFileName` should not be empty or whitespace.

## Expense Claim Item Integrity

Current intended rules:

- `Note` is required.
- `Note` max length: `1000`.
- `Amount` precision: `18, 2`.
- `Amount > 0`.
- `State` is required.
- `State` must be one of valid `ExpenseClaimItemState` enum values.
- `RejectionReason` max length: `1000`.
- If state is rejected, `RejectionReason` must be non-empty.
- Indexes on:
  - `ExpenseClaimId`
  - `CompanyId`
  - `ExpenseCategoryId`

Important:

- `ExpenseClaimItem.FileUrl` has been removed from Core.
- Attachments are now stored in the `Attachment` table.

## Expense Claim Integrity

Current intended rules:

- `Number` required, max length `50`, unique.
- `Note` max length `1000`.
- `ClaimDate` required.
- `CurrentState` required with default `Draft`.
- `CurrentState` must be a valid `ExpenseClaimState`.
- Index on `(UserId, ClaimDate)`.
- Audit foreign keys should restrict delete.

## Expense Claim To Movement Integrity

Business meaning:

- `ExpenseClaim` may have no movement before settlement.
- Once settled, it should have one settlement movement.

Configuration should express:

- `ExpenseClaim 0..1 -> 1 CustodyMovement` through `SettlementMovement`.
- `Movement.ExpenseClaimId` is nullable.
- `ApprovedExpense` movements require `ExpenseClaimId`.
- Other movement types must not have `ExpenseClaimId`.
- There should be a unique index to prevent duplicate settlement movements.

Current intended check:

```sql
([Type] = 2 AND [ExpenseClaimId] IS NOT NULL)
OR
([Type] <> 2 AND [ExpenseClaimId] IS NULL)
```

## Custody Integrity

Current intended rules:

- `Number` required, max length `50`, unique.
- `Note` max length `500`.
- `IsActive` default `true`.
- A user can have only one active custody.

This is represented by filtered unique index:

```text
UserId unique where IsActive = 1
```

## Company Integrity

Current intended rules:

- `Name` required, max length `256`.
- `TaxRegistrationNumber` required, max length `9`.
- `TaxRegistrationNumber` unique while `IsDeleted = 0`.
- `TaxFileNumber` required, max length `10`.
- `TaxFileNumber` unique while `IsDeleted = 0`.
- `Address` max length `256`.

## Platform And Account Integrity

Current platform rules:

- `Platform.Name` required, max length `256`.
- `Platform.Name` unique while `IsDeleted = 0`.
- `Platform.Url` required, max length `1000`.

Potential future account rule:

- Consider unique index on `(CompanyId, PlatformId, Email)` where `IsDeleted = 0`.

This is useful to prevent duplicated platform accounts for the same company.

It has not been treated as mandatory yet.
