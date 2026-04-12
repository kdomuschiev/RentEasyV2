# Story 1.3: Authentication API (Login, Password Change, Password Reset)

Status: done

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a landlord or tenant,
I want to log in, change my password, and reset a forgotten password via email,
So that my account is secure and I can always regain access.

## Acceptance Criteria

**AC1 — Login endpoint**
**Given** a user with valid credentials
**When** `POST /api/auth/login` is called with correct email and password
**Then** a JWT is returned containing role claim (`Landlord` or `Tenant`), `account_state` claim, and 7-day expiry
**And** the JWT's `iat` (issued at) is ≥ the user's `token_valid_from` value

**AC2 — Login failure**
**Given** a user submits a login request with an incorrect password
**When** `POST /api/auth/login` is processed
**Then** a 401 Unauthorized response with an RFC 7807 `ProblemDetails` body is returned

**AC3 — Change password**
**Given** an authenticated user
**When** `POST /api/auth/change-password` is called with the correct current password and a valid new password
**Then** the password is updated and `token_valid_from` is set to the current UTC timestamp
**And** any previously issued JWT is immediately invalid (iat < new token_valid_from)

**AC4 — TokenValidFromMiddleware**
**Given** `TokenValidFromMiddleware` is registered in the request pipeline
**When** any authenticated API request is processed
**Then** the JWT's `iat` is compared to the user's current `token_valid_from` in the database
**And** if `iat < token_valid_from`, a 401 Unauthorized ProblemDetails response is returned

**AC5 — RequiresPasswordChange enforcement**
**Given** a user with `account_state = 'RequiresPasswordChange'`
**When** any API request other than `POST /api/auth/change-password` is made
**Then** a 403 Forbidden ProblemDetails response is returned

**AC6 — Forgot password**
**Given** a valid registered email address
**When** `POST /api/auth/forgot-password` is called with that email
**Then** a password reset email is sent containing a time-limited link (valid 1 hour)
**And** if the email is not found, a 200 OK response is still returned (no email enumeration)

**AC7 — Reset password**
**Given** a valid, unexpired reset token
**When** `POST /api/auth/reset-password` is called with the token and a new password
**Then** the password is updated, `token_valid_from` is refreshed, and the token is invalidated

## Tasks / Subtasks

- [x] **Task 1: Add NuGet packages for JWT** (AC: 1)
  - [x] Install `Microsoft.AspNetCore.Authentication.JwtBearer`
  - [x] No need for `System.IdentityModel.Tokens.Jwt` separately — it's a transitive dependency

- [x] **Task 2: Add JWT configuration to appsettings.json** (AC: 1)
  - [x] Add `Jwt` section with `Key`, `Issuer`, `Audience`, `ExpiryDays` (7)
  - [x] Add real values to `appsettings.Development.json` (git-ignored)

- [x] **Task 3: Add `RequiresPasswordChange` to `AccountState` enum** (AC: 5)
  - [x] Add new value to `Domain/Enums/AccountState.cs`
  - [x] Create EF Core migration for the enum change (stored as string, so just needs migration awareness)

- [x] **Task 4: Add CORS policy to Program.cs** (AC: 1, deferred work item)
  - [x] Add `AddCors` with allowed origin for Azure SWA domain (configurable via appsettings)
  - [x] Add `UseCors` in middleware pipeline before `UseAuthentication`

- [x] **Task 5: Configure JWT authentication in Program.cs** (AC: 1, 4)
  - [x] Add `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)` with `.AddJwtBearer()`
  - [x] Configure token validation parameters: issuer, audience, signing key, validate lifetime
  - [x] Keep existing `AddIdentity` registration — JWT bearer scheme must be set as default authentication scheme

- [x] **Task 6: Create Auth DTOs** (AC: 1, 2, 3, 6, 7)
  - [x] `Application/DTOs/Auth/LoginRequest.cs` — `Email`, `Password`
  - [x] `Application/DTOs/Auth/LoginResponse.cs` — `Token`, `Role`, `AccountState`
  - [x] `Application/DTOs/Auth/ChangePasswordRequest.cs` — `CurrentPassword`, `NewPassword`
  - [x] `Application/DTOs/Auth/ForgotPasswordRequest.cs` — `Email`
  - [x] `Application/DTOs/Auth/ResetPasswordRequest.cs` — `Email`, `Token`, `NewPassword`

- [x] **Task 7: Create AuthService** (AC: 1, 2, 3, 6, 7)
  - [x] `Application/Services/AuthService.cs`
  - [x] `LoginAsync` — validate credentials via `UserManager.CheckPasswordAsync`, generate JWT with claims (role, account_state, landlord_id for landlord users), return token + user info
  - [x] `ChangePasswordAsync` — verify current password, update via `UserManager.ChangePasswordAsync`, update `TokenValidFrom` to `DateTimeOffset.UtcNow`
  - [x] `ForgotPasswordAsync` — generate reset token via `UserManager.GeneratePasswordResetTokenAsync`, send email (stub/log in this story, full email in Epic 3)
  - [x] `ResetPasswordAsync` — validate token, reset password via `UserManager.ResetPasswordAsync`, update `TokenValidFrom`

- [x] **Task 8: Create AuthController** (AC: 1, 2, 3, 6, 7)
  - [x] `Controllers/AuthController.cs` at route `/api/auth`
  - [x] `POST /api/auth/login` → `LoginAsync` → 200 + LoginResponse or 401 ProblemDetails
  - [x] `POST /api/auth/change-password` → `[Authorize]` → `ChangePasswordAsync` → 200 or 400/401 ProblemDetails
  - [x] `POST /api/auth/forgot-password` → anonymous → `ForgotPasswordAsync` → always 200 (no email enumeration)
  - [x] `POST /api/auth/reset-password` → anonymous → `ResetPasswordAsync` → 200 or 400 ProblemDetails

- [x] **Task 9: Create TokenValidFromMiddleware** (AC: 4)
  - [x] `Common/Middleware/TokenValidFromMiddleware.cs`
  - [x] On every authenticated request: extract `iat` from JWT claims, load user's `TokenValidFrom` from DB
  - [x] If `iat < TokenValidFrom` → return 401 ProblemDetails
  - [x] Register in Program.cs pipeline between `UseAuthentication()` and `UseAuthorization()`

- [x] **Task 10: Create RequiresPasswordChangeMiddleware** (AC: 5)
  - [x] `Common/Middleware/RequiresPasswordChangeMiddleware.cs`
  - [x] On every authenticated request: check `account_state` claim in JWT
  - [x] If `RequiresPasswordChange` AND request path is NOT `/api/auth/change-password` → return 403 ProblemDetails
  - [x] Register in pipeline after `TokenValidFromMiddleware`

- [x] **Task 11: Create ErrorHandlingMiddleware** (AC: 2)
  - [x] `Common/Middleware/ErrorHandlingMiddleware.cs`
  - [x] Catches unhandled exceptions → returns RFC 7807 ProblemDetails (500)
  - [x] Never expose stack traces in non-development environments
  - [x] Register as first middleware in pipeline

- [x] **Task 12: Seed landlord account** (AC: 1)
  - [x] Create a seed method in Program.cs or a dedicated seeder
  - [x] Seed Kiril's landlord account with role `Landlord` on first startup (idempotent)
  - [x] Use `UserManager.CreateAsync` + `UserManager.AddToRoleAsync`
  - [x] Create `Landlord` and `Tenant` roles if they don't exist

- [x] **Task 13: Add JWT constants to Constants.cs** (AC: 1)
  - [x] `JwtExpiryDays` = 7
  - [x] `PasswordResetTokenExpiryHours` = 1

- [x] **Task 14: Run EF Core migration** (AC: 3, 5)
  - [x] `dotnet ef migrations add AddRequiresPasswordChangeState --project renteasy-api --connection "<direct_connection_string>"`
  - [x] `dotnet ef database update --project renteasy-api --connection "<direct_connection_string>"`

- [x] **Task 15: Verify build and test manually** (AC: all)
  - [x] `dotnet build` passes
  - [x] Test login with seeded landlord credentials
  - [x] Test change password and confirm old JWT is rejected
  - [x] Test forgot-password returns 200 for unknown email
  - [x] Test reset-password with valid token

## Dev Notes

### What Already Exists (from Stories 1.1 + 1.2 — DO NOT RE-CREATE)

**Packages already installed:**
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.5`
- `Npgsql.EntityFrameworkCore.PostgreSQL 10.0.1`
- `EFCore.NamingConventions 10.0.1`
- `Azure.Storage.Blobs 12.27.0`
- `QuestPDF 2026.2.4`
- `Resend 0.2.2`
- `Scalar.AspNetCore 2.13.22`

**NOT installed (must add):**
- `Microsoft.AspNetCore.Authentication.JwtBearer` — required for JWT bearer auth scheme

**Program.cs already has:**
- `AddControllers()` with `CamelCase` JSON naming
- `AddOpenApi()` + Scalar UI (dev only)
- `AddHttpContextAccessor()`
- `AddDbContext<AppDbContext>` with Npgsql + snake_case naming
- `AddIdentity<ApplicationUser, IdentityRole<Guid>>` with password policy (min 8 chars, 1 digit, 1 lowercase, no uppercase/special required)
- `UseAuthentication()` / `UseAuthorization()` already in pipeline
- Default token providers registered (needed for password reset tokens)

**Domain layer already has:**
- `ApplicationUser` extends `IdentityUser<Guid>` with `TokenValidFrom` (DateTimeOffset) and `AccountState` (AccountState enum, default Active)
- `AccountState` enum: `Active`, `ReadOnly`, `Expired` — **must add `RequiresPasswordChange`**
- All other domain entities (Property, Tenancy, etc.) — don't touch
- `AppDbContext` with `HasQueryFilter` on landlord-scoped entities, enums as strings
- `Constants.cs` with file/URL/nudge constants

**Folder structure:**
- `Application/DTOs/` — exists, empty (create `Auth/` subfolder)
- `Application/Services/` — exists, empty (create `AuthService.cs`)
- `Common/Middleware/` — exists, empty (create middleware files)
- `Controllers/` — has `HealthController.cs` and `WeatherForecastController.cs` (patterns to follow)

### Critical Architecture Patterns

**JWT Claims — must include:**
- `sub` = user ID (Guid)
- `email` = user email
- `role` = "Landlord" or "Tenant" (standard claim name: `ClaimTypes.Role`)
- `account_state` = "Active", "ReadOnly", "Expired", or "RequiresPasswordChange"
- `landlord_id` = user ID for landlords (AppDbContext's `HasQueryFilter` reads this claim to scope queries)
- `iat` = issued at (standard JWT claim, used by `TokenValidFromMiddleware`)
- Expiry: 7 days

**landlord_id claim is CRITICAL:** `AppDbContext.GetCurrentLandlordId()` reads the `landlord_id` claim from `IHttpContextAccessor`. For Landlord users, `landlord_id` MUST equal the user's own `Id`. For Tenant users, `landlord_id` is NOT included in the JWT (tenants don't have their own properties). The `HasQueryFilter` logic already handles missing `landlord_id` claims — verify this by reading `AppDbContext.cs` lines 33–48.

**TokenValidFrom flow:**
1. On login → JWT issued with `iat` = current UTC time
2. On password change → `TokenValidFrom` updated to `DateTimeOffset.UtcNow`
3. On every request → `TokenValidFromMiddleware` compares `JWT.iat` vs `DB.TokenValidFrom`
4. If `iat < TokenValidFrom` → 401 (token was issued before the last invalidation event)
5. This means password change instantly invalidates all existing sessions

**RequiresPasswordChange flow (for tenant first login):**
1. Landlord creates tenant account (Story 2.4) → sets `AccountState = RequiresPasswordChange`
2. Tenant logs in → JWT contains `account_state: "RequiresPasswordChange"`
3. `RequiresPasswordChangeMiddleware` blocks all API requests except `POST /api/auth/change-password`
4. Tenant changes password → `AccountState` updated to `Active`, new JWT issued
5. This story only builds the middleware enforcement; tenant creation is Story 2.4

**RFC 7807 ProblemDetails — ALL errors must use this format:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid email or password."
}
```
Never custom error envelopes. Use `Results.Problem()` or manually construct `ProblemDetails` responses.

**CORS (deferred work item from Story 1.2):**
The deferred work log notes: "CORS policy absent in Program.cs — no `AddCors`/`UseCors` call; cross-origin requests from Azure SWA will be blocked." This MUST be addressed in this story since the Next.js BFF (Story 1.4) will call the API cross-origin. Configure allowed origin from `appsettings.json` for flexibility.

### Email for Password Reset

Resend is already installed (`Resend 0.2.2`). However, full email infrastructure is not set up until later stories. For this story:
- Implement `ForgotPasswordAsync` to generate the token and construct the reset URL
- **Log the reset URL to console/logger** in development — do not skip the endpoint implementation
- Wire up actual Resend email sending when `IEmailService` is built (later story), OR implement a minimal send now if straightforward
- The forgot-password endpoint must still return 200 regardless of email existence (no enumeration)

### Identity Integration Notes

- `UserManager<ApplicationUser>` is already registered via `AddIdentity` — inject it, don't create a new one
- `SignInManager` is also registered — but DON'T use it for API auth (it's cookie-based). Use `UserManager.CheckPasswordAsync` for credential validation
- `RoleManager<IdentityRole<Guid>>` is registered — use for role management in seeder
- `UserManager.GeneratePasswordResetTokenAsync` / `ResetPasswordAsync` — use these for forgot/reset password flow
- Token providers are already registered via `AddDefaultTokenProviders()`

### Namespace Convention

The project namespace is `renteasy_api` (from the .csproj). Follow this pattern:
- `renteasy_api.Controllers`
- `renteasy_api.Application.Services`
- `renteasy_api.Application.DTOs.Auth`
- `renteasy_api.Common.Middleware`
- `renteasy_api.Domain.Enums`

### Project Structure Notes

All files align with the established project structure from architecture.md:
```
renteasy-api/
├── Controllers/
│   └── AuthController.cs               ← NEW
├── Application/
│   ├── Services/
│   │   └── AuthService.cs              ← NEW
│   └── DTOs/
│       └── Auth/
│           ├── LoginRequest.cs          ← NEW
│           ├── LoginResponse.cs         ← NEW
│           ├── ChangePasswordRequest.cs ← NEW
│           ├── ForgotPasswordRequest.cs ← NEW
│           └── ResetPasswordRequest.cs  ← NEW
├── Common/
│   ├── Middleware/
│   │   ├── ErrorHandlingMiddleware.cs       ← NEW
│   │   ├── TokenValidFromMiddleware.cs      ← NEW
│   │   └── RequiresPasswordChangeMiddleware.cs ← NEW
│   └── Constants.cs                    ← UPDATE (add JWT constants)
├── Domain/
│   └── Enums/
│       └── AccountState.cs             ← UPDATE (add RequiresPasswordChange)
└── Program.cs                          ← UPDATE (JWT config, CORS, middleware, seeder)
```

### References

- [Source: _bmad-output/planning-artifacts/epics.md — Story 1.3 acceptance criteria]
- [Source: _bmad-output/planning-artifacts/architecture.md — Authentication & Security decisions]
- [Source: _bmad-output/planning-artifacts/architecture.md — Implementation Patterns & Consistency Rules]
- [Source: _bmad-output/implementation-artifacts/deferred-work.md — CORS policy item, HasQueryFilter + background jobs item]
- [Source: _bmad-output/implementation-artifacts/1-2-database-schema-and-ef-core-foundation.md — Previous story patterns]
- [Source: renteasy-api/Program.cs — Current service registrations]
- [Source: renteasy-api/Domain/Entities/ApplicationUser.cs — Current user model]
- [Source: renteasy-api/Infrastructure/Data/AppDbContext.cs — HasQueryFilter reads landlord_id claim]

## Dev Agent Record

### Agent Model Used
Claude Opus 4.6 (1M context)

### Debug Log References
- Build succeeded with 0 warnings, 0 errors
- EF Core migration `AddRequiresPasswordChangeState` created and applied to Neon DB

### Completion Notes List
- Installed `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.5`
- Added JWT config section to `appsettings.json` (Key, Issuer, Audience, ExpiryDays)
- Added `RequiresPasswordChange` to `AccountState` enum
- Added CORS policy with configurable allowed origins (`Cors:AllowedOrigins` in appsettings)
- Configured JWT bearer auth as default scheme with full token validation (issuer, audience, lifetime, signing key)
- Created 5 Auth DTOs: LoginRequest, LoginResponse, ChangePasswordRequest, ForgotPasswordRequest, ResetPasswordRequest
- Created `AuthService` with LoginAsync, ChangePasswordAsync, ForgotPasswordAsync, ResetPasswordAsync, GenerateNewTokenAsync
- JWT includes all required claims: sub, email, iat, jti, role, account_state, landlord_id (for landlords)
- Created `AuthController` at `/api/auth` with 4 POST endpoints (login, change-password, forgot-password, reset-password)
- All error responses use RFC 7807 ProblemDetails format
- Forgot-password returns 200 regardless of email existence (no enumeration)
- Created `ErrorHandlingMiddleware` — catches unhandled exceptions, returns ProblemDetails, hides stack traces in non-dev
- Created `TokenValidFromMiddleware` — validates JWT iat >= user's TokenValidFrom from DB
- Created `RequiresPasswordChangeMiddleware` — blocks all requests except change-password when account_state is RequiresPasswordChange
- Middleware pipeline: ErrorHandling → CORS → Authentication → TokenValidFrom → RequiresPasswordChange → Authorization
- Created `SeedDataAsync` in Program.cs — seeds Landlord/Tenant roles and landlord account from config (idempotent)
- Added `JwtExpiryDays` and `PasswordResetTokenExpiryHours` to Constants.cs
- Password reset URL logged to console in dev (email sending deferred to later story)
- Change password also transitions `RequiresPasswordChange` → `Active` and issues new token

### File List
- `renteasy-api/renteasy-api.csproj` — MODIFIED (added JwtBearer package)
- `renteasy-api/appsettings.json` — MODIFIED (added Jwt, Cors, Seed, App sections)
- `renteasy-api/Domain/Enums/AccountState.cs` — MODIFIED (added RequiresPasswordChange)
- `renteasy-api/Common/Constants.cs` — MODIFIED (added JWT constants)
- `renteasy-api/Program.cs` — MODIFIED (JWT auth, CORS, middleware registration, seeder, async Main)
- `renteasy-api/Application/DTOs/Auth/LoginRequest.cs` — NEW
- `renteasy-api/Application/DTOs/Auth/LoginResponse.cs` — NEW
- `renteasy-api/Application/DTOs/Auth/ChangePasswordRequest.cs` — NEW
- `renteasy-api/Application/DTOs/Auth/ForgotPasswordRequest.cs` — NEW
- `renteasy-api/Application/DTOs/Auth/ResetPasswordRequest.cs` — NEW
- `renteasy-api/Application/Services/AuthService.cs` — NEW
- `renteasy-api/Controllers/AuthController.cs` — NEW
- `renteasy-api/Common/Middleware/ErrorHandlingMiddleware.cs` — NEW
- `renteasy-api/Common/Middleware/TokenValidFromMiddleware.cs` — NEW
- `renteasy-api/Common/Middleware/RequiresPasswordChangeMiddleware.cs` — NEW
- `renteasy-api/Migrations/20260412081819_AddRequiresPasswordChangeState.cs` — NEW
- `renteasy-api/Migrations/20260412081819_AddRequiresPasswordChangeState.Designer.cs` — NEW
- `renteasy-api/Migrations/AppDbContextModelSnapshot.cs` — MODIFIED (updated snapshot)
- `renteasy-api.Tests/Application/Services/AuthServiceTests.cs` — NEW (7 tests)
- `renteasy-api.Tests/Common/Middleware/ErrorHandlingMiddlewareTests.cs` — NEW (3 tests)
- `renteasy-api.Tests/Common/Middleware/TokenValidFromMiddlewareTests.cs` — NEW (4 tests)
- `renteasy-api.Tests/Common/Middleware/RequiresPasswordChangeMiddlewareTests.cs` — NEW (5 tests)

### Review Findings

- [x] [Review][Decision] Login endpoint does not check AccountState — **resolved**: block Expired accounts at login; ReadOnly users may still authenticate — fixed in LoginAsync
- [x] [Review][Patch] UpdateAsync result ignored after setting TokenValidFrom — ChangePassword and ResetPassword silently succeed even if the DB update fails [renteasy-api/Application/Services/AuthService.cs ~75, ~106]
- [x] [Review][Patch] Reset-password leaks email existence — returns "Invalid reset request." for unknown emails (400) vs Identity errors for bad token (different detail), enabling enumeration [renteasy-api/Application/Services/AuthService.cs ~95]
- [x] [Review][Patch] PasswordResetTokenExpiryHours constant is dead code — never wired into Identity token lifetime (default remains 1 day, not 1 hour) [renteasy-api/Program.cs]
- [x] [Review][Patch] TokenValidFromMiddleware silently passes through when iat or sub claims are missing — should treat missing claims as a validation failure (401) [renteasy-api/Common/Middleware/TokenValidFromMiddleware.cs]
- [x] [Review][Patch] GenerateNewTokenAsync returns null if user is deleted mid-request — ChangePassword controller returns Ok({token: null}) without error [renteasy-api/Controllers/AuthController.cs ~68]
- [x] [Review][Patch] ChangePassword response uses anonymous type instead of typed DTO — not discoverable, not testable, fragile to serialisation policy changes [renteasy-api/Controllers/AuthController.cs ~73]
- [x] [Review][Patch] Seeded landlord account does not explicitly set TokenValidFrom — relies on default(DateTimeOffset) being MinValue; should be set explicitly for resilience [renteasy-api/Program.cs, SeedDataAsync]
- [x] [Review][Patch] JWT auth silently disabled with no startup error when Jwt:Key is empty — all [Authorize] endpoints become public with no warning in non-development environments [renteasy-api/Program.cs]
- [x] [Review][Patch] Sub-second race between iat truncation and TokenValidFrom in change-password flow — new token iat (Unix seconds, truncated) can be < TokenValidFrom (sub-second precision), causing immediate 401 after password change [renteasy-api/Application/Services/AuthService.cs, renteasy-api/Controllers/AuthController.cs]

- [x] [Review][Defer] landlord_id claim missing from Tenant JWTs — AppDbContext.GetCurrentLandlordId() throws if claim absent; needs to be addressed in Story 2.4 when tenant creation is built [renteasy-api/Application/Services/AuthService.cs] — deferred, pre-existing design gap for V1 tenant scope
- [x] [Review][Defer] DB call per authenticated request in TokenValidFromMiddleware — known V1 scaling constraint; acceptable at current Neon free-tier load [renteasy-api/Common/Middleware/TokenValidFromMiddleware.cs] — deferred, pre-existing

### Change Log
- 2026-04-12: Implemented full authentication API — JWT login, change password, forgot/reset password, three middleware (error handling, token validation, password change enforcement), CORS policy, role/account seeder, EF Core migration
