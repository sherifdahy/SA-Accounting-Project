# User Management Flow Specification

This document defines the expected backend behavior for managing users after the login flow has been completed.

Use it as the implementation checklist before refactoring or extending user-related commands, handlers, repositories, and permissions.

## Flow Goal

The user management flow prepares real system users who can authenticate and then access only the companies and actions they are allowed to use.

It must answer:

```text
Who can use the system?
Which companies can this user access?
Which actions can this user perform?
Is this account currently enabled?
```

This flow is not the same as authentication.

- Authentication proves that the user is who they claim to be.
- User management defines the user's account, roles, company access, and permission overrides.
- Authorization enforces those roles, permissions, and company access at runtime.

## Current Scope

This flow includes:

- creating users
- updating user basic data
- assigning roles
- assigning company access
- assigning all-company access when needed
- removing company access
- disabling or enabling users
- managing denied permission overrides
- reading users
- reading user companies
- reading user permission overrides

This flow does not include:

- login
- refresh tokens
- logout or token revocation
- confirm email
- resend confirmation email
- forgot password
- reset password

Those authentication features are intentionally postponed until later project phases.

## Main Concepts

### User Account

The user account is represented by `ApplicationUser`.

Expected important fields:

- `Id`
- `Name`
- `Email`
- `UserName`
- `SSN`
- `PhoneNumber`
- `IsDisabled`
- `EmailConfirmed`
- `LockoutEnabled`

Implementation rule:

- `UserName` should be set to the email unless the project later introduces a separate username.
- `LockoutEnabled` must be `true` for created users.
- Since email confirmation is postponed, `EmailConfirmed` should be `true` for users created by the admin flow until email confirmation is implemented.
- `IsDisabled` should be `false` by default.

### Role Access

Roles are permission templates.

The system should not depend on a single role name as the final authorization decision. Runtime authorization should use effective permissions.

Target create/update user behavior should support multiple roles if the business needs it.

Recommended command model:

```csharp
public record CreateUserCommand : IRequest<Result<UserResponse>>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string SSN { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public IReadOnlyList<string> Roles { get; init; } = [];
    public IReadOnlyList<int> CompanyIds { get; init; } = [];
    public bool HasAccessToAllCompanies { get; init; }
}
```

If the business guarantees one role per user, keep one role for now, but document that decision explicitly.

### Company Access

Company access decides the data scope.

A permission answers:

```text
What can the user do?
```

Company access answers:

```text
Where can the user do it?
```

Example:

```text
User has expenseClaims:read
User is assigned to Company 5 only
Result: user can read expense claims for Company 5 only
```

Implementation rules:

- Normal users must have at least one assigned company unless they have all-company access.
- Company ids must exist.
- Deleted companies must not be assigned.
- Duplicate company ids must be ignored or rejected consistently.
- Data queries must later filter company-scoped data by the current user's assigned companies.

### All-Company Access

Avoid representing all-company access by passing `null` company ids.

Use an explicit model:

```text
HasAccessToAllCompanies = true
CompanyIds = []
```

or an explicit command:

```text
AssignAllCompaniesToUserCommand
```

The implementation must make the difference clear between:

- user has no company access
- user has selected company access
- user has all-company access

Current code already contains separate commands for assigning one company and assigning all companies. Keep that idea, but make sure the database model can represent it clearly.

### Permission Overrides

Role permissions are the default permissions.

User permission overrides are exceptions applied to one user.

Current project direction:

```text
Effective permissions = role permissions - denied permissions
```

Rules:

- Denied permissions should store permission values, not display names.
- Denied permissions must be valid known permissions.
- Duplicate denied permissions must not be stored.
- Permission overrides should affect the next login token.
- Existing access tokens may keep old permissions until they expire unless token revocation is implemented later.

## Main Use Cases

### 1. Create User

Purpose:

- create an account that can log in
- assign role permissions
- assign company scope

Expected input:

- name
- email
- password
- ssn
- phone number
- roles
- company ids or all-company access flag

Expected validations:

- name is required
- email is required and valid
- email is unique
- password is required and follows Identity password policy
- SSN is unique when provided
- roles exist and are enabled
- company ids exist and are not deleted
- normal user must have company access
- company ids must not contain duplicates

Expected implementation steps:

1. Validate `CreateUserCommand` using FluentValidation in the application pipeline.
2. Check duplicate email through `UserManager`.
3. Check duplicate SSN when SSN is provided.
4. Validate requested roles.
5. Validate requested company access.
6. Create `ApplicationUser`.
7. Set `UserName = Email`.
8. Set `LockoutEnabled = true`.
9. Set `EmailConfirmed = true` temporarily because email confirmation is postponed.
10. Set `IsDisabled = false`.
11. Create the user with `UserManager.CreateAsync`.
12. Assign roles using `UserManager.AddToRolesAsync`.
13. Assign company access through `UserCompany`.
14. If any role or company assignment fails, roll back the created user or wrap the operation in a transaction.
15. Return `UserResponse`.

Important current gap:

The existing `CreateUserCommand` has a single `Role` and no company access input. Refactor it before considering this flow complete.

### 2. Update User

Purpose:

- update user basic profile data
- optionally update roles and company access depending on endpoint design

Expected validations:

- user exists
- email remains unique if changed
- SSN remains unique if changed
- roles exist if roles are updated
- company ids exist if companies are updated

Recommended approach:

- Keep basic data update separate from role/company assignment if the UI has separate screens.
- Use dedicated commands for role assignment and company assignment when those actions have different permissions.

### 3. Assign Roles

Purpose:

- change which permission template applies to the user

Expected implementation steps:

1. Load user.
2. Validate roles exist.
3. Load current user roles.
4. Calculate roles to add and remove.
5. Apply changes using `UserManager`.
6. Return success or Identity error.

Rules:

- Do not create roles dynamically from user input.
- Do not assign disabled roles if the role model supports disabled status.
- A user's next login should reflect updated role permissions.

### 4. Assign Company To User

Purpose:

- give a user access to one company

Expected validations:

- user exists
- company exists
- company is not deleted
- user-company relation does not already exist

Expected implementation steps:

1. Load user.
2. Load company.
3. Check existing `UserCompany`.
4. Add `UserCompany`.
5. Save changes.

### 5. Assign All Companies To User

Purpose:

- allow high-trust users to access every company

Expected design decision:

Choose one of these approaches:

```text
Option A: explicit boolean on user, such as HasAccessToAllCompanies
Option B: explicit permission, such as companies:accessAll
Option C: materialize UserCompany rows for every company
```

Recommended approach:

- Use a permission such as `companies:accessAll` for authorization meaning.
- Use `UserCompany` for selected company access.
- Avoid materializing all company rows unless the business explicitly needs snapshots.

Runtime rule:

```text
if user has companies:accessAll:
    allow all companies
else:
    filter by UserCompany
```

### 6. Remove Company From User

Purpose:

- revoke access to one company

Expected validations:

- user exists
- relation exists

Expected implementation steps:

1. Load `UserCompany`.
2. Remove it.
3. Save changes.

Rule:

- Prevent removing the last company from a normal enabled user unless all-company access is present or the business allows users with no company access.

### 7. Remove All Companies From User

Purpose:

- revoke all selected company access

Expected validations:

- user exists

Rule:

- After removal, the user should either have all-company access, be disabled, or be considered a valid user with no company-scoped data access.
- Pick one business rule before implementation.

### 8. Toggle User Status

Purpose:

- enable or disable a user account

Expected behavior:

- disabled users cannot login
- enabled users can login if credentials and other account checks pass

Important token rule:

- Disabling a user does not automatically invalidate already issued JWT access tokens.
- Since refresh token and token revocation are postponed, keep access token expiry short enough to reduce risk.
- If immediate blocking is required later, add per-request user status validation or token revocation.

### 9. Update Permission Overrides

Purpose:

- deny specific permissions for one user even if roles grant them

Expected validations:

- user exists
- permission values are known
- duplicates are removed

Expected implementation steps:

1. Load user.
2. Load current denied permissions.
3. Remove old denied permissions.
4. Add new denied permissions.
5. Save changes.

Rule:

- Overrides should be stored as denied permissions only in the current design.
- If the system later needs user-specific allowed permissions, introduce a separate model instead of overloading denied permissions.

### 10. Get User And User Lists

Purpose:

- allow admins to inspect users and their access setup

Expected list filters:

- search by name or email
- include disabled
- role
- company

Expected response should include enough summary data:

- id
- name
- email
- phone number
- is disabled
- roles
- company access summary

Avoid returning sensitive fields:

- password hash
- security stamp
- reset tokens
- internal identity fields

## Recommended API Surface

Suggested endpoints:

```text
GET    /api/users
GET    /api/users/{userId}
POST   /api/users
PUT    /api/users/{userId}
PATCH  /api/users/{userId}/status
PUT    /api/users/{userId}/roles
GET    /api/users/{userId}/companies
POST   /api/users/{userId}/companies
POST   /api/users/{userId}/companies/all
DELETE /api/users/{userId}/companies/{companyId}
DELETE /api/users/{userId}/companies
GET    /api/users/{userId}/permission-overrides
PUT    /api/users/{userId}/permission-overrides
```

Keep request DTOs in the API/application contract layer as transport models.

Map them to commands.

Validation must target commands and queries, not request DTOs, for use-case rules.

## Permission Requirements

Suggested permission names, aligned with the current lowercase style in seed data:

```text
users:read
users:create
users:update
users:toggleStatus
users:assignRoles
users:assignCompanies
users:updatePermissionOverrides
```

Company assignment may also require:

```text
companies:read
companies:accessAll
```

Rules:

- Every user-management endpoint must require authentication.
- Every command endpoint must require the matching permission.
- Read endpoints should use `users:read`.
- Permission override updates should have their own permission because they are more sensitive than normal user updates.

## Data Integrity Rules

### Identity And User

- Email must be unique.
- SSN must be unique when provided.
- Created users must have `LockoutEnabled = true`.
- Admin-created users should have `EmailConfirmed = true` while email confirmation is postponed.

### User Roles

- Roles must exist before assignment.
- Role assignment must be transactional with user creation.
- Multiple roles should be supported if the business allows it.

### User Companies

- Do not allow duplicate `(UserId, CompanyId)` rows.
- Do not assign deleted companies.
- Define how all-company access is represented before final implementation.

### Permission Overrides

- Do not allow duplicate denied permission values per user.
- Do not store invalid permission values.

## Transaction Boundary

Create user is a multi-step operation:

```text
create identity user
assign roles
assign companies
```

This should be treated as one business transaction.

If the project keeps using `UserManager` directly, make sure failure in later steps cleans up the user.

Preferred long-term approach:

- use the same database transaction when possible
- or centralize identity operations in an identity service that can coordinate rollback clearly

## Implementation Plan

### Step 1: Review Current User Commands

Inspect current commands and handlers:

- `CreateUserCommand`
- `UpdateUserCommand`
- `ToggleStatusUserCommand`
- `AssignCompanyToUserCommand`
- `AssignAllCompaniesToUserCommand`
- `RemoveCompanyFromUserCommand`
- `RemoveAllCompaniesFromUserCommand`
- `UpdatePermissionOverridesCommand`

Mark which handlers still depend directly on Microsoft Identity.

### Step 2: Refactor Create User Command

Update `CreateUserCommand` to include:

- roles
- company ids
- all-company access decision if needed

Move validation from request validators to command validators.

### Step 3: Fix Create User Handler

Required changes:

- set `UserName`
- set `LockoutEnabled = true`
- set `EmailConfirmed = true` temporarily
- assign roles
- assign company access
- rollback on failure

### Step 4: Add Or Fix Company Assignment Rules

Update company assignment handlers to:

- validate user exists
- validate company exists
- prevent duplicates
- reject deleted companies
- handle all-company access clearly

### Step 5: Add Or Fix Permission Override Rules

Update permission override handler to:

- validate permission values
- remove duplicates
- replace existing denied permissions atomically

### Step 6: Add Endpoint Permissions

Apply permission attributes to user-management endpoints.

Do not rely on role names in controllers.

### Step 7: Manual Test Scenario

Minimum scenario:

1. Create a role with permissions.
2. Create a company.
3. Create a user with the role and company access.
4. Login with the created user.
5. Confirm JWT contains expected permissions.
6. Call a permission-protected endpoint.
7. Confirm company-scoped queries return only assigned companies.
8. Add denied permission override.
9. Login again.
10. Confirm denied permission is removed from effective permissions.
11. Disable user.
12. Confirm login fails.

## Completion Criteria

The user management flow is complete when:

- users can be created with account data, roles, and company access
- created users can login successfully
- disabled users cannot login
- roles affect generated permissions
- denied permission overrides affect generated permissions
- company access is represented clearly
- duplicate company assignments are prevented
- command validation is used for use-case validation
- endpoints are protected by permission attributes
- manual tests prove create user to login to authorization end-to-end

