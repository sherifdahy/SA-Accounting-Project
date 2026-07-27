# Current State And Decisions

Last updated: 2026-07-27

## Decisions Made

### No Generic ActivityLog For Now

The project will not add a generic `ActivityLog` entity yet.

Historical lookup should initially be supported through real business records:

- company attachments
- expense claim items
- expense claim history
- custody movements
- platform accounts

### Attachments Are Company-Owned

An attachment always belongs to a company.

It may optionally come from an expense claim item.

This allows both:

- direct company document uploads
- documents generated or collected during an expense claim

Interpretation:

- `ExpenseClaimItemId = null`: direct company attachment
- `ExpenseClaimItemId != null`: expense claim item attachment

### ExpenseClaim To Movement Is Not One-To-Many

The old `ExpenseClaim.Movements` collection was misleading.

The intended relationship is:

```text
ExpenseClaim 0..1 -> 1 CustodyMovement
```

In Core this is represented by:

```csharp
public virtual CustodyMovement? SettlementMovement { get; set; }
```

The custody remains:

```text
Custody 1 -> many Movement
```

### ExpenseClaimItem No Longer Has FileUrl

`ExpenseClaimItem.FileUrl` was removed because each item may have multiple attachments.

Use:

```csharp
ExpenseClaimItem.Attachments
```

## Files Already Updated

Core:

- `Entities/Attachments/Attachment.cs`
- `Entities/Companies/Company.cs`
- `Entities/ExpenseClaims/ExpenseClaimItem.cs`
- `Entities/ExpenseClaims/ExpenseClaim.cs`

Infrastructure:

- `Presistance/Data/ApplicationDbContext.cs`
- `Presistance/Data/Config/AttachmentConfig.cs`
- `Presistance/Data/Config/ExpenseClaimItemConfig.cs`
- `Presistance/Data/Config/MovementConfig.cs`

Docs:

- `sa-accounting-docs/ERD.drawio`

## Known Incomplete Work

The GitHub update introduced upload endpoints and a file service. We kept the upload capability but aligned the domain to our `Attachment` model instead of `UploadedFile`.

Current rule:

- API/application request fields may still be called `Files` because users upload files.
- Domain and persistence should use `Attachment` and `Attachments`.
- Do not reintroduce `UploadedFile` as a parallel domain entity.

Migrations are not created yet for the attachment/domain changes.

The full solution build may fail until Application is updated.

Core build has passed after domain changes.

Infrastructure build has passed after config changes.

## Current ERD Notes

The ERD was updated to include:

- `Attachment`
- `ExpenseClaimItem -> Attachment`
- `Company -> Attachment`
- `ExpenseClaim -> Movement` as settlement

The ERD is in draw.io XML format and can be edited directly in draw.io.

There is also a backup file:

- `sa-accounting-docs/ERD.drawio.bak`
