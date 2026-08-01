# Permission-Based Flows

This project is permission-based, not simple role-based.

Roles are useful as permission templates, but runtime access should be decided by explicit permissions and assigned companies.

Important UI rule:

- Users can enter the same pages.
- What they can see and do inside those pages changes by permission.
- Company-scoped data must also be filtered by the companies assigned to the user.

Do not design the frontend as completely separate pages per role.

Design shared pages with permission-gated actions, sections, buttons, filters, and data.

## Permission Model Principles

### Role Permission Based, Not Role Based

The system should not ask only:

```text
Is user Admin?
Is user Accountant?
Is user Employee?
```

It should ask:

```text
Can user view companies?
Can user create expense claim?
Can user review expense claim?
Can user settle expense claim?
Can user manage custody?
Can user view company credentials?
```

### Same Page, Different Surface

Example: Company details page.

Every allowed user may open it, but:

- one user can only view company basic data
- another can edit company data
- another can view platform accounts
- another can add attachments
- another can assign users

### Company Assignment Is A Data Filter

Even if a user has `Companies.View`, the query should only return companies the user is allowed to access, unless they have a global/all-companies permission.

The main access filter is:

```text
UserCompany
```

## Flow 1: Authentication And Session Context

Purpose:

- user logs in
- API returns identity and permissions
- frontend builds the visible UI from permissions

Typical steps:

1. User enters credentials.
2. API validates credentials.
3. API returns token and user profile.
4. Frontend loads permissions and assigned companies.
5. Frontend shows shared navigation but hides/locks unavailable actions.

Permission checks:

- authenticated user required
- disabled users cannot continue

Output needed by frontend:

- user id
- user name
- effective permissions
- assigned company ids or all-company access indicator

## Flow 2: User And Permission Management

Purpose:

- manage employees
- assign roles/permissions
- override permissions per user
- assign companies to users

Shared page:

- user management

Possible actions:

- view users
- create user
- update user
- disable/enable user
- assign roles
- update permission overrides
- assign company
- remove company
- assign all companies

Permission examples:

- `Users.View`
- `Users.Create`
- `Users.Update`
- `Users.ToggleStatus`
- `Users.AssignCompanies`
- `Users.UpdatePermissions`
- `Roles.View`
- `Roles.Manage`

Data integrity:

- user-company access is explicit through `UserCompany`
- role permissions are defaults
- user permission overrides can allow/deny specific permissions

## Flow 3: Company Management

Purpose:

- manage client/company records
- preserve company history through related records

Shared page:

- companies list
- company details

Possible actions:

- view companies
- create company
- update company
- disable/delete company
- view owners
- manage owners
- view platform accounts
- manage platform accounts
- view company attachments
- upload company attachment

Permission examples:

- `Companies.View`
- `Companies.Create`
- `Companies.Update`
- `Companies.ToggleStatus`
- `Owners.View`
- `Owners.Manage`
- `Accounts.View`
- `Accounts.Manage`
- `Attachments.View`
- `Attachments.Upload`

Data filter:

- users without all-company access should only see assigned companies

Important:

- attachments belong to company first
- direct company uploads should have `Attachment.CompanyId`
- direct company uploads should have `ExpenseClaimItemId = null`

## Flow 4: Platform And Account Management

Purpose:

- define online platforms
- store selectors for automation/scraping
- store company accounts per platform

Shared pages:

- platforms
- selectors
- company accounts

Possible actions:

- view platforms
- create/update platform
- disable platform
- view selectors
- manage selectors
- view account metadata
- manage account credentials

Permission examples:

- `Platforms.View`
- `Platforms.Create`
- `Platforms.Update`
- `Platforms.ToggleStatus`
- `Selectors.View`
- `Selectors.Manage`
- `Accounts.View`
- `Accounts.Manage`
- `Accounts.ViewCredentials`

Security note:

- viewing account existence is different from viewing credentials
- credentials should have a stricter permission than account metadata

## Flow 5: Daily Expense Claim Creation

Purpose:

- employee records daily expenses/work items by company
- employee attaches receipts, images, extracts, or PDFs

Shared page:

- expense claims
- expense claim details

Typical steps:

1. Employee creates an `ExpenseClaim`.
2. Employee adds one or more `ExpenseClaimItem`.
3. Each item is linked to:
   - company
   - expense category
   - amount
   - note
   - attachments when needed
4. Employee submits the claim.

Permission examples:

- `ExpenseClaims.ViewOwn`
- `ExpenseClaims.Create`
- `ExpenseClaims.UpdateOwnDraft`
- `ExpenseClaims.Submit`
- `ExpenseClaimItems.Add`
- `ExpenseClaimItems.Update`
- `ExpenseClaimItems.Remove`
- `Attachments.Upload`

Data rules:

- employees usually see their own claims
- claim items must reference companies the user can access
- categories with `RequiresAttachment = true` require attachments

State rules:

- editable states:
  - `Draft`
  - `ReturnedForEdit`
- after submission, employee should not freely edit unless returned

## Flow 6: Expense Claim Review

Purpose:

- reviewer checks submitted claims
- reviewer approves/rejects items
- claim state becomes approved, partially approved, or rejected

Shared page:

- expense claims
- review panel inside claim details

Typical steps:

1. Reviewer opens submitted claim.
2. Reviewer checks every item.
3. Reviewer approves or rejects each item.
4. Rejected item requires rejection reason.
5. System updates claim state.
6. System writes `ExpenseClaimHistory`.

Permission examples:

- `ExpenseClaims.ViewSubmitted`
- `ExpenseClaims.Review`
- `ExpenseClaimItems.Review`

Data filter:

- reviewer may see all claims or only claims within accessible companies depending on permissions

Important:

- review is not settlement
- review changes claim/item states only
- no custody movement should be created during review

## Flow 7: Expense Claim Settlement

Purpose:

- approved expenses become a financial movement against employee custody

Shared page:

- expense claim details
- settlement action

Typical steps:

1. Claim must be `Approved` or `PartiallyApproved`.
2. System calculates approved item total.
3. System finds active custody for claim owner.
4. System checks custody balance.
5. System creates one `CustodyMovement` of type `ApprovedExpense`.
6. Movement links to `ExpenseClaimId`.
7. Claim state becomes `Settled`.
8. System writes `ExpenseClaimHistory`.

Permission examples:

- `ExpenseClaims.Settle`
- `CustodyMovements.CreateApprovedExpense`

Data integrity:

- `ExpenseClaim 0..1 -> CustodyMovement` through `SettlementMovement`
- `Custody 1 -> many CustodyMovement`
- only `ApprovedExpense` movement type should have `ExpenseClaimId`
- non-expense movement types should not have `ExpenseClaimId`

Important:

- never model claim settlement as multiple movements for one claim
- avoid settling the same claim twice

## Flow 8: Custody Management

Purpose:

- manage employee custody/funds
- track deposits, returns, adjustments, and approved expense settlements

Shared pages:

- custodies
- custody details
- custody movements

Possible actions:

- view custodies
- create custody
- close/disable custody
- add deposit
- add return
- add adjustment in/out
- view movements

Permission examples:

- `Custodies.View`
- `Custodies.Create`
- `Custodies.Close`
- `CustodyMovements.View`
- `CustodyMovements.CreateDeposit`
- `CustodyMovements.CreateReturn`
- `CustodyMovements.CreateAdjustment`

Data integrity:

- one active custody per user
- movement amount must be positive
- movement type must be valid
- insufficient balance should block outgoing movements

## Flow 9: Attachment And File Access

Purpose:

- upload and retrieve files related to company work

Shared pages:

- company details attachments tab
- expense claim item attachments section
- file download endpoint

Attachment sources:

- direct company upload:
  - `CompanyId` required
  - `ExpenseClaimItemId = null`
- expense claim item upload:
  - `CompanyId` required
  - `ExpenseClaimItemId` required

Permission examples:

- `Attachments.View`
- `Attachments.Upload`
- `Attachments.Download`
- `Attachments.Delete`

Data filter:

- users can only access attachments for companies they can access
- file download must check company access through `Attachment.CompanyId`

Important:

- domain entity is `Attachment`
- API request/response may still use the word `Files`
- do not reintroduce `UploadedFile` as a parallel domain entity

## Flow 10: Search And History Lookup

Purpose:

- find anything that happened for a company
- retrieve documents and claim-related history

Shared page:

- company details
- company history/search tab

Possible search dimensions:

- company
- date range
- employee
- expense claim number
- expense category
- attachment filename
- platform/account
- claim state

Sources:

- `Attachment`
- `ExpenseClaimItem`
- `ExpenseClaim`
- `ExpenseClaimHistory`
- `CustodyMovement`
- `Account`

Permission examples:

- `Companies.View`
- `Attachments.View`
- `ExpenseClaims.View`
- `CustodyMovements.View`
- `Accounts.View`

Important:

- no generic `ActivityLog` for now
- history should come from real business entities

## Flow 11: Dashboard And Navigation

Purpose:

- provide one shared home/navigation experience
- show different cards/actions based on permissions

Shared page:

- dashboard

Possible widgets:

- my draft claims
- submitted claims awaiting review
- claims ready for settlement
- active custodies
- recent company attachments
- companies assigned to me
- platform/account warnings

Permission behavior:

- same dashboard route
- widgets appear only if permission allows
- widget data is filtered by assigned companies and user permissions

## Flow 12: Admin/System Setup

Purpose:

- seed and maintain system-level data

Possible actions:

- manage roles
- manage permissions
- manage expense categories
- manage platforms
- manage selectors
- manage default users/roles

Permission examples:

- `Permissions.View`
- `Roles.Manage`
- `ExpenseCategories.Manage`
- `Platforms.Manage`
- `Selectors.Manage`

Important:

- roles should not be treated as the final authorization rule
- roles provide a starting permission set
- effective permissions come from roles plus user overrides

## Frontend Implementation Guidance

For every shared page:

1. Load user effective permissions.
2. Load accessible company scope.
3. Render the same route/page.
4. Hide or disable actions without permission.
5. Filter data on the backend, not only frontend.
6. Return `403` when action permission is missing.
7. Return empty/filtered results when the user lacks company access.

Backend must enforce permissions even if the frontend hides buttons.

## Suggested Permission Groups

Company:

- `Companies.View`
- `Companies.Create`
- `Companies.Update`
- `Companies.ToggleStatus`

Company access:

- `Companies.ViewAll`
- `Companies.AssignUsers`

Owner:

- `Owners.View`
- `Owners.Create`
- `Owners.Update`
- `Owners.ToggleStatus`

Account/platform:

- `Accounts.View`
- `Accounts.Manage`
- `Accounts.ViewCredentials`
- `Platforms.View`
- `Platforms.Manage`
- `Selectors.View`
- `Selectors.Manage`

Expense claims:

- `ExpenseClaims.ViewOwn`
- `ExpenseClaims.ViewAll`
- `ExpenseClaims.Create`
- `ExpenseClaims.UpdateOwnDraft`
- `ExpenseClaims.Submit`
- `ExpenseClaims.Review`
- `ExpenseClaims.Settle`
- `ExpenseClaims.Cancel`
- `ExpenseClaims.ReturnForEdit`

Expense claim items:

- `ExpenseClaimItems.Add`
- `ExpenseClaimItems.Update`
- `ExpenseClaimItems.Remove`
- `ExpenseClaimItems.Review`

Attachments:

- `Attachments.View`
- `Attachments.Upload`
- `Attachments.Download`
- `Attachments.Delete`

Custody:

- `Custodies.View`
- `Custodies.Create`
- `Custodies.Close`
- `CustodyMovements.View`
- `CustodyMovements.CreateDeposit`
- `CustodyMovements.CreateReturn`
- `CustodyMovements.CreateAdjustment`

Users and permissions:

- `Users.View`
- `Users.Create`
- `Users.Update`
- `Users.ToggleStatus`
- `Users.AssignCompanies`
- `Users.UpdatePermissions`
- `Roles.View`
- `Roles.Manage`
- `Permissions.View`
