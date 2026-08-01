# System Module Flows

This file tracks the system by module-style flows.

Use this as the main delivery tracker when asking:

```text
What part of the system is done?
What flow should we work on next?
```

The system is permission-based, not role-based:

- users may open the same page
- visible data/actions depend on effective permissions
- backend must enforce permissions
- frontend hides or disables unavailable actions
- company-scoped data is filtered by assigned companies unless the user has all-company access

## Status Values

- `Not Started`
- `In Progress`
- `Blocked`
- `Needs Review`
- `Done`

## Flow 1: Auth Flow

Status: Not Started

Purpose:

- login
- token generation
- current user profile
- session refresh if needed
- user disabled checks

Core scope:

- `ApplicationUser`
- auth service
- JWT provider

Main operations:

- login
- forget password
- reset password
- confirm email
- resend confirmation
- get current profile
- change password

Permission behavior:

- authentication is required before any permission checks
- disabled users should not use the system

Done when:

- users can authenticate safely
- API returns enough session context for frontend permission rendering

## Flow 2: Permission And Role Flow

Status: Not Started

Purpose:

- manage permissions
- use roles as permission templates
- support user-level permission overrides

Core idea:

- this is permission-based, not role-based
- role names should not drive business logic
- effective permissions drive access

Main operations:

- list permissions
- list roles
- create/update/remove role
- assign permissions to role
- assign role to user
- update user permission overrides

Permission behavior:

- role permissions provide defaults
- user overrides can allow or deny specific permissions
- final authorization checks effective permissions

Done when:

- code checks permissions, not role names
- admin can change user capabilities without changing code

## Flow 3: User Flow

Status: Not Started

Purpose:

- manage employees/users
- assign companies to users
- control access scope

Main operations:

- list users
- create user
- update user
- enable/disable user
- assign company
- remove company
- assign all companies if allowed
- view user's effective permissions

Permission behavior:

- user management actions are permission-gated
- company assignment controls data scope

Done when:

- users can be managed
- user-company access works
- disabled users are blocked

## Flow 4: Company Flow

Status: Not Started

Purpose:

- manage client/company records
- keep company data as the center of office work

Main operations:

- list companies
- create company
- update company
- soft delete/disable company
- view company details
- view company history from related records

Related entities:

- `Company`
- `Owner`
- `Account`
- `Attachment`
- `ExpenseClaimItem`
- `UserCompany`

Permission behavior:

- users see assigned companies only
- all-company visibility requires explicit permission
- company actions are permission-gated

Soft delete:

- company deletion should be soft delete using `IsDeleted`
- active unique indexes should account for `IsDeleted`

Done when:

- company CRUD works
- company access isolation works
- soft-deleted companies are hidden from normal views

## Flow 5: Owner Flow

Status: Not Started

Purpose:

- manage company owners

Main operations:

- list owners by company
- create owner
- update owner
- soft delete/disable owner

Related entities:

- `Owner`
- `Company`

Permission behavior:

- user must have access to the company
- owner actions require owner/company permissions

Done when:

- company owners can be managed from company context
- owner records do not disappear from history

## Flow 6: Platform Flow

Status: Not Started

Purpose:

- manage platforms used by company accounts and automation

Main operations:

- list platforms
- create platform
- update platform
- soft delete/disable platform
- manage platform image/url

Related entities:

- `Platform`
- `Account`
- `Selector`

Permission behavior:

- viewing platforms is separate from managing platforms

Done when:

- platforms can be managed
- soft-deleted platforms are hidden from normal views

## Flow 7: Selector Flow

Status: Not Started

Purpose:

- manage scraping/automation selectors per platform

Main operations:

- list selectors by platform
- create selector
- update selector
- soft delete selector
- order/select by priority

Related entities:

- `Selector`
- `Platform`

Permission behavior:

- selector management requires explicit automation/platform permission

Done when:

- platform automation metadata can be maintained safely

## Flow 8: Account Flow

Status: Not Started

Purpose:

- manage company platform accounts

Main operations:

- list accounts by company
- create account
- update account
- soft delete account
- use account internally for automation

Related entities:

- `Account`
- `Company`
- `Platform`

Permission behavior:

- viewing account metadata is separate from viewing credentials
- credentials require stricter permission
- user must have access to the company

Security:

- never expose credentials casually
- account passwords are sensitive

Done when:

- company accounts can be managed
- credential access is tightly permission-gated

## Flow 9: Attachment Flow

Status: Not Started

Purpose:

- upload/download files related to company work

Main operations:

- upload direct company attachment
- upload expense claim item attachment
- list attachments by company
- list attachments by expense claim item
- download attachment
- delete/soft delete attachment if supported later

Related entities:

- `Attachment`
- `Company`
- `ExpenseClaimItem`

Rules:

- every attachment must have `CompanyId`
- `ExpenseClaimItemId` is optional
- `ExpenseClaimItemId = null` means direct company attachment
- `ExpenseClaimItemId != null` means claim-item attachment

Permission behavior:

- user must have company access
- upload/download/delete require explicit permissions

Done when:

- attachments are traceable by company
- file access is permission-gated

## Flow 10: Expense Category Flow

Status: Not Started

Purpose:

- manage categories used by expense claim items

Main operations:

- list categories
- create category
- update category
- disable category
- set `RequiresAttachment`

Related entities:

- `ExpenseCategory`
- `ExpenseClaimItem`

Permission behavior:

- category management is admin/manager permission
- disabled categories should not be selectable for new items

Done when:

- categories can control whether attachments are required

## Flow 11: Expense Claim Flow

Status: Not Started

Purpose:

- manage the main daily claim header and lifecycle

Main operations:

- create claim
- update draft claim
- submit claim
- cancel claim
- return for edit
- review claim
- settle claim
- view claim history

Related entities:

- `ExpenseClaim`
- `ExpenseClaimItem`
- `ExpenseClaimHistory`
- `CustodyMovement`

State flow:

```text
Draft
Submitted
UnderReview
Approved
PartiallyApproved
Rejected
ReturnedForEdit
Cancelled
Settled
```

Permission behavior:

- employee can manage own draft/returned claims
- reviewer can review submitted claims
- settler can settle approved/partially approved claims

Done when:

- claim lifecycle is enforced
- invalid state transitions are blocked
- history records important state changes

## Flow 12: Expense Claim Item Flow

Status: Not Started

Purpose:

- manage claim line items

Main operations:

- add item to draft/returned claim
- update item
- remove item
- upload item attachments
- review item
- approve/reject item

Related entities:

- `ExpenseClaimItem`
- `ExpenseClaim`
- `Company`
- `ExpenseCategory`
- `Attachment`

Rules:

- item must belong to a company
- item must belong to a category
- amount must be positive
- rejected item requires rejection reason
- category requiring attachment must have at least one attachment

Permission behavior:

- item company must be accessible to user
- item actions depend on claim state and user permission

Done when:

- items can be managed independently inside a valid claim state

## Flow 13: Expense Review Flow

Status: Not Started

Purpose:

- review submitted claims and their items

Main operations:

- list submitted claims
- open claim for review
- approve/reject each item
- require rejection reason
- calculate final claim state
- write claim history

Related entities:

- `ExpenseClaim`
- `ExpenseClaimItem`
- `ExpenseClaimHistory`
- `Attachment`

Permission behavior:

- reviewing requires explicit permission
- visible claims may be scoped by company permissions

Done when:

- every reviewed claim has correct final state
- item decisions are complete and auditable

## Flow 14: Expense Settlement Flow

Status: Not Started

Purpose:

- convert approved expenses into one custody movement

Main operations:

- calculate approved item total
- find employee active custody
- validate balance
- create one `CustodyMovement` of type `ApprovedExpense`
- link movement to claim
- mark claim as settled
- write claim history

Related entities:

- `ExpenseClaim`
- `ExpenseClaimItem`
- `Custody`
- `CustodyMovement`

Rules:

- one claim can have zero or one settlement movement
- settlement movement must be type `ApprovedExpense`
- settlement movement must have `ExpenseClaimId`

Permission behavior:

- settlement requires explicit permission

Done when:

- claim cannot be settled twice
- custody balance changes correctly

## Flow 15: Custody Flow

Status: Not Started

Purpose:

- manage employee custody records

Main operations:

- list custodies
- create custody
- close/disable custody
- view custody details
- view custody balance

Related entities:

- `Custody`
- `CustodyMovement`
- `ApplicationUser`

Rules:

- user can have at most one active custody
- custody uses status/soft-delete behavior, not hard delete

Permission behavior:

- custody viewing and management require explicit permissions

Done when:

- active custody uniqueness is enforced

## Flow 16: Custody Movement Flow

Status: Not Started

Purpose:

- manage non-claim custody movements

Main operations:

- add deposit
- add return
- add adjustment in
- add adjustment out
- list movements by custody
- calculate balance

Related entities:

- `CustodyMovement`
- `Custody`

Rules:

- amount must be positive
- non-expense movements must not have `ExpenseClaimId`
- outgoing movements must respect balance rules

Permission behavior:

- each movement action may have a separate permission

Done when:

- custody movement history is reliable
- balance calculation is correct

## Flow 17: Dashboard Flow

Status: Not Started

Purpose:

- one shared dashboard with permission-filtered widgets

Possible widgets:

- my draft claims
- submitted claims awaiting review
- claims ready for settlement
- active custodies
- recent company attachments
- assigned companies

Permission behavior:

- same dashboard page for users
- widgets appear based on permissions
- data is company-scoped

Done when:

- dashboard adapts by permission, not role name

## Flow 18: Search And History Flow

Status: Not Started

Purpose:

- search company history from real business entities

Search sources:

- `Attachment`
- `ExpenseClaimItem`
- `ExpenseClaim`
- `ExpenseClaimHistory`
- `CustodyMovement`
- `Account`

Search filters:

- company
- date range
- employee
- claim number
- filename
- category
- state

Permission behavior:

- search results must obey company access
- sensitive results need specific permissions

Done when:

- user can find company documents and related work history
- no generic `ActivityLog` is required yet

## Flow 19: Soft Delete And Restore Flow

Status: Not Started

Purpose:

- standardize soft delete across business records

Main operations:

- soft delete
- hide from normal lists
- optionally restore
- optionally view deleted records for admins

Related entities:

- `Company`
- `Owner`
- `Account`
- `Platform`
- `Selector`
- `ExpenseCategory`
- `Custody`

Rules:

- avoid hard delete for historical business data
- unique indexes should account for active records only when needed

Done when:

- delete behavior is consistent and safe

## Flow 20: Audit And History Flow

Status: Not Started

Purpose:

- preserve traceability through audit fields and entity histories

Current approach:

- no generic `ActivityLog` yet
- use real business records
- use `AuditableEntity`
- use `ExpenseClaimHistory` for claim state changes

Permission behavior:

- audit/history visibility requires explicit permission where sensitive

Done when:

- important changes are traceable without overcomplicating the domain

## Standard Checklist Per Flow

Use this checklist for each system flow:

```text
Status:

Backend:
- [ ] Domain
- [ ] Infrastructure config
- [ ] Migration
- [ ] Repository/IUnitOfWork access
- [ ] Application command/query
- [ ] Validator
- [ ] API endpoint
- [ ] Permission enforcement
- [ ] Soft delete behavior if applicable

Frontend:
- [ ] Shared page/route
- [ ] Permission-based rendering
- [ ] Forms
- [ ] API integration
- [ ] Company-scoped data filtering

Verification:
- [ ] Happy path
- [ ] Permission denied path
- [ ] Company access isolation
- [ ] Soft-deleted records hidden
- [ ] Database constraints

Notes:
- ...
```
