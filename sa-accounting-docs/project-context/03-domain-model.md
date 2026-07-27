# Domain Model

This document describes the current intended domain model.

The ERD source is:

- `sa-accounting-docs/ERD.drawio`

## Company Area

### Company

Represents a client/company handled by the office.

Important fields:

- `Id`
- `Name`
- `TaxRegistrationNumber`
- `TaxFileNumber`
- `Address`
- `IsDeleted`

Important relationships:

- `Company 1 -> many Owner`
- `Company 1 -> many Account`
- `Company many <-> many User` through `UserCompany`
- `Company 1 -> many ExpenseClaimItem`
- `Company 1 -> many Attachment`

### Owner

Represents a company owner.

Current relationship:

- `Owner many -> 1 Company`

### Account

Represents company credentials for a platform.

Important relationships:

- `Account many -> 1 Company`
- `Account many -> 1 Platform`

Stores:

- `Email`
- `Password`

Security note: credentials are sensitive and should not be casually exposed through application responses.

## Platform Area

### Platform

Represents an online platform that companies can have accounts on.

Important relationships:

- `Platform 1 -> many Account`
- `Platform 1 -> many Selector`

### Selector

Represents scraping/automation selector metadata for a platform.

Important fields:

- `Value`
- `ContentType`
- `Type`
- `Priority`

## Expense Claim Area

### ExpenseClaim

Represents a daily expense report submitted by an employee.

Important relationships:

- `ExpenseClaim many -> 1 User`
- `ExpenseClaim 1 -> many ExpenseClaimItem`
- `ExpenseClaim 1 -> many ExpenseClaimHistory`
- `ExpenseClaim 0..1 -> 1 Movement` through `SettlementMovement`

`SettlementMovement` means the financial movement created when the claim is settled against custody.

### ExpenseClaimItem

Represents a single expense/work item inside a claim.

Important relationships:

- `ExpenseClaimItem many -> 1 ExpenseClaim`
- `ExpenseClaimItem many -> 1 Company`
- `ExpenseClaimItem many -> 1 ExpenseCategory`
- `ExpenseClaimItem 1 -> many Attachment`

Important note:

- `ExpenseClaimItem` no longer owns a single `FileUrl`.
- Attachments are now separate entities.

### ExpenseCategory

Classifies expense items.

Important fields:

- `Name`
- `RequiresAttachment`
- `IsDisabled`

If `RequiresAttachment = true`, Application rules should require at least one attachment for the related expense item.

### ExpenseClaimHistory

Tracks state changes for an expense claim.

This is not a generic project-wide activity log.

It records:

- `ExpenseClaimId`
- `FromState`
- `ToState`
- `Note`
- audit fields from `AuditableEntity`

## Attachment Area

### Attachment

Represents a file related to company work.

Attachments are company-owned first.

Important fields:

- `Id`
- `FileUrl`
- `FileName`
- `ContentType`
- `Note`
- `CompanyId`
- `ExpenseClaimItemId`

Important relationships:

- `Attachment many -> 1 Company`
- `Attachment many -> 0..1 ExpenseClaimItem`

Meaning:

- If `ExpenseClaimItemId` is null, the attachment was uploaded directly to the company.
- If `ExpenseClaimItemId` has a value, the attachment came from an expense claim item.

This supports the requirement:

- "I want to know whether this attachment came from an expense claim or from someone uploading a company document directly."

## Custody Area

### Custody

Represents money held by an employee.

Important relationships:

- `Custody many -> 1 User`
- `Custody 1 -> many Movement`

Business rule:

- A user should have at most one active custody.

### Movement

Represents a financial movement on a custody.

Movement types include:

- `Deposit`
- `ApprovedExpense`
- `Return`
- `AdjustmentIn`
- `AdjustmentOut`

Important relationships:

- `Movement many -> 1 Custody`
- `Movement 0..1 -> 1 ExpenseClaim`

Rule:

- `ApprovedExpense` movement must be linked to an `ExpenseClaim`.
- Non-expense movement types should not be linked to an `ExpenseClaim`.

## Access Control Area

### User

Uses ASP.NET Identity through `ApplicationUser`.

Important relationships:

- `User many <-> many Company` through `UserCompany`
- `User many <-> many Role`
- `User 1 -> many Custody`
- `User 1 -> many ExpenseClaim`

### Role And Permissions

Roles and permissions are based on ASP.NET Identity plus custom permission override concepts.

The project supports:

- roles
- role claims/permissions
- user-level permission overrides
- company assignment per user
