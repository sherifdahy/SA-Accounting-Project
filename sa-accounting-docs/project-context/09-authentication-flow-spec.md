# Authentication Flow Specification

This document defines the expected behavior of the authentication flow.

Use it as the evaluation baseline before implementing or refactoring backend authentication code.

## Flow Goal

The authentication flow proves the user's identity and creates a trusted session context for the rest of the system.

It must answer:

```text
Who is trying to use the system?
Can the system trust that this person is really that user?
Is this account currently allowed to start a session?
```

Authentication is not the same as authorization.

- Authentication proves identity.
- Authorization decides what the authenticated user can see or do.

The authentication flow may return roles and effective permissions as session context, but permission enforcement belongs to the authorization layer.

## Primary Outcome

When the flow succeeds, the API returns a valid access token and enough session context for clients to continue safely.

Minimum expected output:

- access token
- token expiry information
- authenticated user id
- user email
- user display name if available
- effective permissions if the frontend needs immediate permission rendering
- company access summary if the frontend needs immediate scoped rendering

## Scope

This flow includes:

- login with email and password
- account eligibility checks
- access token generation
- authenticated user/session context
- current user profile endpoint
- password reset request
- password reset confirmation
- email confirmation
- resend confirmation email
- change password for authenticated users

This flow does not include:

- role management
- permission management
- assigning users to companies
- deciding whether a user can perform a business action
- frontend page rendering rules

Those belong to later permission, user, and frontend flows.

## Main Actors

### Anonymous User

An unauthenticated person trying to start a session.

Allowed authentication actions:

- login
- request password reset
- reset password using a valid reset token
- confirm email
- resend confirmation email when applicable

### Authenticated User

A user with a valid access token.

Allowed authentication actions:

- get current profile
- change password
- continue calling protected APIs until the token expires

### System Administrator

An authenticated user with user-management permissions.

The administrator may affect authentication indirectly by:

- creating users
- disabling users
- locking/unlocking users if supported
- changing roles or permissions
- changing company assignments

These actions are outside the authentication flow, but their results must be respected by it.

## Login Flow

### Purpose

Allow a valid active user to start a trusted session without exposing sensitive account information.

### Input

```text
Email
Password
```

### Expected Steps

1. Receive the login transport request.
2. Map the request to `LoginCommand` and validate the command through the MediatR
   validation pipeline.
3. Normalize and find the user by email.
4. If the user does not exist, return a generic invalid credentials response.
5. Check the password using the identity provider.
6. Track failed attempts and lockout according to identity policy.
7. If credentials are valid, check account eligibility.
8. Build the authenticated session context.
9. Generate an access token.
10. Return authentication response.

### Command Validation

The command is the Application boundary for the login use case. Validation must
be attached to `LoginCommand`, not only to the API request DTO.

The expected implementation is:

```text
LoginRequest (API transport DTO)
    -> LoginCommand (Application use case input)
    -> LoginCommandValidator (FluentValidation)
    -> MediatR ValidationBehavior
    -> LoginCommandHandler
```

The handler must not run when command validation fails. This guarantees the same
rules are applied when the command is invoked from a controller, a test, a job,
or another application entry point.

`LoginRequest` remains a transport-only DTO. It may be used for HTTP model
binding and mapping, but it must not be the only place where login use-case
validation is defined.

Validation should ensure:

- email is required
- email has a valid email format
- password is required

Password strength rules are required when creating or resetting passwords.

For login, password strength validation should be used carefully. A user may have an older password that does not match the newest policy. The identity password check should be the final source of truth.

### Credential Failure Behavior

For these cases:

- user does not exist
- password is wrong

The API should return the same generic message:

```text
Invalid email or password.
```

The response should not reveal whether the email exists.

### Account Eligibility Checks

A user with correct credentials should still be blocked when:

- email is not confirmed and confirmed email is required
- account is locked out
- account is disabled by administration
- account is soft-deleted or inactive if the domain supports it
- account violates a future tenant/company access policy

Expected responses should be clear enough for legitimate users, but should not leak unnecessary sensitive details.

Examples:

- invalid credentials
- email not confirmed
- account locked
- account disabled

## Token Generation

### Purpose

The access token allows the API to identify the user on later requests without receiving the password again.

### Token Requirements

The token must be:

- signed by the backend
- time-limited
- validated for issuer
- validated for audience
- validated for lifetime
- rejected after expiry

### Recommended Claims

```text
sub             user id
email           user email
name            user display name when useful
jti             unique token id
roles           role names, if needed by clients
permissions     effective permission values, if authorization uses token claims
```

Role names should not be the final business authorization rule.

Runtime authorization should rely on effective permissions, not hard-coded role names.

## Effective Permissions In Session Context

If permissions are included in the token or login response, they must represent the final effective permissions for the user.

Effective permissions should consider:

- permissions inherited from roles
- user-level permission overrides
- denied permissions
- disabled/deleted roles if applicable
- duplicate permission removal

The final list must be distinct.

If permissions are not embedded in the token, the API must provide a reliable current-session endpoint that returns them.

## Company Scope In Session Context

The system is company-scoped.

Authentication may return a company access summary if the frontend needs it immediately after login.

Possible output:

- assigned company ids
- all-company access flag

Backend authorization and queries must still enforce company access. The frontend must not be the only protection.

## Current User Profile Flow

### Purpose

Return the current authenticated user's session profile.

This endpoint is important because clients may need to refresh session context after page reload or application restart.

### Expected Output

- user id
- email
- display name
- roles if needed
- effective permissions
- company access summary
- account status information that is safe to expose

### Rules

- requires authentication
- must reject expired or invalid tokens
- must reject disabled users
- should return current data from the database when permission/company assignments may have changed

## Password Reset Flow

### Request Reset

Purpose:

- let a user request a reset link or code without revealing whether an email exists

Expected behavior:

1. Receive email.
2. If the email exists and the account is eligible, generate reset token.
3. Send reset email.
4. Return a neutral success response.

Recommended response:

```text
If the email exists, a reset message has been sent.
```

### Confirm Reset

Purpose:

- allow the user to set a new password using a valid reset token

Expected behavior:

1. Receive email, reset code, and new password.
2. Validate new password against password policy.
3. Decode and validate reset code.
4. Reset password through identity provider.
5. Return success or a safe error.

## Email Confirmation Flow

### Confirm Email

Purpose:

- activate a user's email after account creation.

Expected behavior:

1. Receive user identifier and confirmation code.
2. Decode and validate confirmation code.
3. Confirm email through identity provider.
4. Return success or safe error.

### Resend Confirmation

Purpose:

- resend confirmation email when the account exists and is not already confirmed.

Expected behavior:

1. Receive email or user identifier.
2. Verify account exists.
3. If already confirmed, return an appropriate response.
4. Generate a new confirmation code.
5. Send confirmation email.

## Change Password Flow

Purpose:

- let an authenticated user change their password.

Expected behavior:

1. Require valid authentication.
2. Receive current password and new password.
3. Validate new password against password policy.
4. Verify current password.
5. Change password through identity provider.
6. Return success or safe error.

## Error Response Expectations

Authentication errors should be consistent across the API.

Recommended error categories:

- validation error
- invalid credentials
- email not confirmed
- account locked
- account disabled
- invalid token
- expired token
- invalid reset/confirmation code
- internal server error

Error responses should use a consistent problem-details shape.

They should include:

- HTTP status code
- stable error code
- user-safe message
- validation field errors when applicable

## Security Rules

- Never return password hashes.
- Never log passwords or raw tokens.
- Never expose whether an email exists during login or password-reset request.
- Tokens must expire.
- Token signing key must come from secure configuration.
- Disabled users must not be able to start or continue sessions.
- Backend must enforce authentication and authorization even if the frontend hides actions.
- Password reset and email confirmation codes must be time-limited through the identity provider.

## Expected HTTP Behavior

Suggested status codes:

- `200 OK` for successful login/profile/change-password operations
- `204 No Content` for successful commands that return no body
- `400 Bad Request` for validation errors or invalid reset/confirmation code
- `401 Unauthorized` for invalid credentials or invalid/expired token
- `403 Forbidden` for authenticated users who cannot perform an authenticated action
- `423 Locked` or `401 Unauthorized` for locked account, depending on chosen API standard

Pick one standard and apply it consistently.

## Acceptance Criteria

The authentication flow is considered ready when:

- valid active users can log in successfully
- invalid credentials return a generic safe error
- unconfirmed email users are blocked when confirmation is required
- locked users are blocked
- disabled users are blocked
- successful login returns a signed expiring token
- token validation rejects expired, malformed, or wrongly signed tokens
- current user endpoint returns reliable session context
- effective permissions are accurate if returned in token/profile
- duplicate permissions are removed
- user permission overrides are reflected if permissions are part of session context
- password reset request does not reveal whether an email exists
- password reset confirmation enforces password policy
- email confirmation works with valid code and rejects invalid code
- change password requires authentication and current password
- all auth errors use a consistent response format

## Backend Evaluation Checklist

```text
API endpoints
- [ ] Login endpoint
- [ ] Current user/profile endpoint
- [ ] Forgot password endpoint
- [ ] Reset password endpoint
- [ ] Confirm email endpoint
- [ ] Resend confirmation endpoint
- [ ] Change password endpoint

Identity rules
- [ ] Email lookup is normalized
- [ ] Invalid credentials response is generic
- [ ] Lockout is applied on failed password attempts
- [ ] Email confirmation is enforced
- [ ] Disabled users are blocked
- [ ] Password policy is enforced where appropriate

Token/session
- [ ] JWT issuer is validated
- [ ] JWT audience is validated
- [ ] JWT lifetime is validated
- [ ] JWT signing key is validated
- [ ] Token includes required identity claims
- [ ] Expiry is returned to client

Permissions/session context
- [ ] Role permissions are loaded
- [ ] User overrides are applied if required
- [ ] Denied permissions are removed
- [ ] Final permissions are distinct
- [ ] Company access summary is available if required

Errors/security
- [ ] Problem response shape is consistent
- [ ] Sensitive details are not leaked
- [ ] Passwords and tokens are not logged
- [ ] Invalid/expired tokens are rejected

Verification
- [ ] Happy path login
- [ ] Invalid email/password
- [ ] Unconfirmed email
- [ ] Locked account
- [ ] Disabled account
- [ ] Expired token
- [ ] Invalid token signature
- [ ] Password reset happy path
- [ ] Password reset invalid code
- [ ] Change password happy path
- [ ] Change password wrong current password
```

## Open Design Decisions

These decisions should be finalized before implementation:

- Should login response include only token, or token plus full session profile?
- Should effective permissions be embedded in JWT, loaded from `/me`, or both?
- Should company access summary be part of authentication response?
- Should disabled users be blocked only at login, or also on every authenticated request?
- Should locked accounts return `401`, `403`, or `423`?
- Should refresh tokens be introduced now, or should the first version use short-lived access tokens only?
