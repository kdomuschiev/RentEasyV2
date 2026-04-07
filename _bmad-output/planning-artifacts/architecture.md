---
stepsCompleted: [1, 2, 3, 4, 5, 6, 7, 8]
workflowStatus: complete
completedAt: "2026-04-06"
lastStep: 8
inputDocuments:
  - _bmad-output/planning-artifacts/prd.md
  - _bmad-output/planning-artifacts/ux-design-specification.md
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2.md
  - _bmad-output/planning-artifacts/research/domain-private-landlord-tools-software-research-2026-04-02.md
  - _bmad-output/planning-artifacts/research/technical-renteasy-v1-stack-research-2026-04-02.md
workflowType: 'architecture'
project_name: 'RentEasyV2'
user_name: 'Kiril'
date: '2026-04-06'
---

# Architecture Decision Document

_This document builds collaboratively through step-by-step discovery. Sections are appended as we work through each architectural decision together._

## Starter Template Evaluation

### Primary Technology Domain

Full-stack web application — two independently scaffolded projects (Next.js 16 frontend + ASP.NET Core 10 Web API backend) under a monorepo root.

### Selected Scaffolding

**Frontend — Next.js 16**

```bash
npx create-next-app@latest renteasy-web
# ✅ TypeScript  ✅ ESLint  ✅ Tailwind CSS  ✅ App Router  ✅ src/  ✅ Turbopack
```

**Architectural decisions provided by scaffold:**
- TypeScript (strict mode throughout)
- Tailwind CSS 4.x (utility-first)
- App Router (required for RSC, layouts, server components)
- `src/` directory structure
- ESLint + Next.js config pre-wired
- Turbopack for fast dev HMR

**Backend — ASP.NET Core 10 Web API**

```bash
dotnet new webapi --use-controllers --use-program-main -o renteasy-api
```

**Architectural decisions provided by scaffold:**
- Controller-based API (not Minimal APIs — required for multi-role RBAC with consistent cross-cutting policies)
- Explicit `Program.cs` (not top-level statements)
- OpenAPI/Swagger included
- `appsettings.json` / `appsettings.Development.json` environment config

**Monorepo structure:**
```
RentEasyV2/
├── renteasy-web/        ← Next.js 16 frontend
├── renteasy-api/        ← ASP.NET Core 10 Web API
├── .github/workflows/   ← CI/CD
└── README.md
```

**Additional packages post-scaffold:**

| Package | Layer | Purpose |
|---|---|---|
| `next-intl` | Frontend | BG/EN i18n |
| `qrcode` | Frontend | Client-side QR for IRIS Pay / Revolut |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Backend | EF Core + Neon PostgreSQL |
| `Azure.Storage.Blobs` | Backend | Azure Blob Storage (files + PDFs) |
| `QuestPDF` | Backend | PDF generation |
| `resend-dotnet` | Backend | Transactional email |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | Backend | Auth |

**Note:** Project initialization using these commands is the first implementation story.

## Core Architectural Decisions

### Decision Priority Analysis

**Critical decisions (block implementation):**
- Auth token strategy with instant revocation via `TokenValidFrom`
- Multi-tenant EF Core query filters on all landlord-scoped entities
- Background job scheduler (`IHostedService`) for email nudges
- Azure Blob signed URL serving for all files and PDFs

**Important decisions (shape architecture):**
- RFC 7807 Problem Details for all API errors
- URL-prefix i18n routing (`/bg/`, `/en/`)
- GitHub Actions CI/CD, two environments only

**Deferred (post-MVP):**
- Caching (no caching in V1 — Neon handles load at this scale)
- Refresh token flow (deferred — M1 `TokenValidFrom` revocation is sufficient)

### Data Architecture

| Decision | Choice | Rationale |
|---|---|---|
| EF Core migrations | Manual `dotnet ef database update` from dev machine | Solo dev V1; no team workflow complexity needed |
| Neon connection strings | Two in `appsettings.json`: pooled (runtime) + direct (migrations) | Required by Neon/PgBouncer — direct for migrations, pooled for runtime queries |
| Caching | None in V1 | Neon handles load at this scale; keep it simple |
| Multi-tenancy | EF Core `HasQueryFilter` scoped to `LandlordId` on all tenant-scoped entities | Single DB with discriminator column. `DbContext` registered as Scoped. All `LandlordId` columns indexed. |

### Authentication & Security

| Decision | Choice | Rationale |
|---|---|---|
| JWT lifetime | 7 days, no refresh token | Solo dev V1; acceptable risk at this scale |
| Token revocation | `TokenValidFrom` datetime column on user table | Instant state-change enforcement without refresh token complexity. API validates `JWT.iat >= TokenValidFrom` on every request. Updated on move-out and any state transition. |
| Account state | Baked into JWT claim, re-issued on state change | Simple; works correctly with `TokenValidFrom` revocation |
| Auth pattern | Next.js BFF — JWT stored in HttpOnly cookie on SWA domain, forwarded as Bearer header to API | Required: Azure SWA and App Service are on different root domains; cross-domain cookies are not viable |

### API & Communication Patterns

| Decision | Choice | Rationale |
|---|---|---|
| Error handling | RFC 7807 `ProblemDetails` | Built into ASP.NET Core; standard format; consistent across all endpoints |
| Background jobs | `IHostedService` + EF Core polling | No extra infrastructure; runs in-process; sufficient for Day 3/7/14 nudge volume at V1 scale |
| API style | Controller-based REST | Consistent cross-cutting policies (auth, logging, error handling) via filter pipeline |

### Frontend Architecture

| Decision | Choice | Rationale |
|---|---|---|
| State management | Server Components + `useState`/`useReducer` only | No heavy client state needed per PRD; no global store overhead |
| i18n routing | URL prefix — `/bg/[route]`, `/en/[route]` | SEO required on showcase page (Bulgarian primary); next-intl default pattern |

### Infrastructure & Deployment

| Decision | Choice | Rationale |
|---|---|---|
| CI/CD | GitHub Actions | Free; official Azure deploy + Vercel deploy actions; zero cost |
| Environments | Development (local) + Production only | Solo dev V1; staging adds overhead without benefit at this scale |

### Decision Impact Analysis

**Implementation sequence:**
1. Scaffold monorepo (both projects)
2. Database schema + EF Core migrations (Neon, two connection strings)
3. ASP.NET Core Identity + JWT auth with `TokenValidFrom` column
4. Multi-tenancy `HasQueryFilter` on all scoped entities
5. Azure Blob Storage integration
6. `IHostedService` background job scheduler
7. Next.js BFF auth layer
8. i18n routing structure (`/bg/`, `/en/`)

**Cross-component dependencies:**
- `TokenValidFrom` must be set on account creation and updated on every state transition (move-out, manual revocation)
- `IHostedService` scheduler depends on EF Core being set up first (polls DB for pending nudge records)
- BFF auth layer must be in place before any authenticated Next.js page is built

## Implementation Patterns & Consistency Rules

### Naming Patterns

**Database (PostgreSQL via EF Core):**
- Tables: `snake_case` plural — `landlords`, `properties`, `tenancies`, `condition_report_items`, `bill_periods`, `maintenance_requests`, `email_nudge_jobs`
- Columns: `snake_case` — `landlord_id`, `created_at`, `token_valid_from`
- Foreign keys: `{referenced_table_singular}_id` — `landlord_id`, `tenancy_id`
- Indexes: `ix_{table}_{column(s)}` — `ix_properties_landlord_id`
- C# entity classes: `PascalCase` singular — `Property`, `Tenancy`, `BillPeriod`, `ConditionReportItem`

**API endpoints:**
- Resources: plural kebab-case — `/api/properties`, `/api/bill-periods`, `/api/condition-reports`
- Route params: `{id}` style — `/api/properties/{id}/tenancies/{tenancyId}`
- Query params: `camelCase` — `?landlordId=...`
- All resource IDs: UUIDs — never sequential integers in URLs

**C# code:**
- Classes/Methods: `PascalCase`
- Variables/Parameters: `camelCase`
- Private fields: `_camelCase`
- Constants: `PascalCase` (not `ALL_CAPS`)
- Async methods: suffix `Async` — `GetPropertyAsync`, `GeneratePdfAsync`

**TypeScript/React code:**
- Components: `PascalCase` — `BillingScreen`, `ConditionReportItem`
- Files: `PascalCase` matching component — `BillingScreen.tsx`
- Hooks: `camelCase` prefixed `use` — `useBillingPeriod`
- Utilities: `camelCase` — `formatCurrency`, `buildSignedUrl`
- API client functions: `camelCase` verb-noun — `fetchBillingPeriod`, `confirmPayment`

### Structure Patterns

**Backend project organisation:**
```
renteasy-api/
├── Controllers/          ← One controller per resource domain
├── Domain/
│   ├── Entities/         ← EF Core entity classes
│   ├── Enums/            ← AccountState, BillCategory, ConditionReportStatus
│   └── Interfaces/
├── Application/
│   ├── Services/         ← Business logic per domain
│   └── DTOs/             ← Request/response DTOs per domain
├── Infrastructure/
│   ├── Data/             ← DbContext, migrations, query filters
│   ├── Storage/          ← Azure Blob client wrapper
│   ├── Email/            ← Resend wrapper
│   └── Jobs/             ← IHostedService background jobs
└── Common/
    ├── Extensions/
    └── Middleware/       ← Auth validation, error handling
```

**Frontend project organisation:**
```
renteasy-web/src/
├── app/
│   └── [locale]/
│       ├── (public)/     ← Showcase page (static, unauthenticated)
│       ├── (auth)/       ← Login, password change
│       ├── (landlord)/   ← Landlord dashboard routes
│       └── (tenant)/     ← Tenant portal routes
├── api/                  ← Next.js Route Handlers (BFF layer)
│   └── auth/
├── components/
│   ├── ui/               ← Generic reusable components
│   ├── landlord/         ← Landlord-specific components
│   └── tenant/           ← Tenant-specific components
├── lib/                  ← API client, utilities, helpers
└── messages/             ← next-intl translation files (bg.json, en.json)
```

**Tests:** Co-located with source — `PropertyService.test.cs` next to `PropertyService.cs`; `BillingScreen.test.tsx` next to `BillingScreen.tsx`

### Format Patterns

**API responses:**
- Success: direct data, no wrapper
- All errors: RFC 7807 `ProblemDetails` — `type`, `title`, `status`, `detail`
- HTTP 200 success, 201 creation, 204 deletion, 400 bad request, 401 unauthenticated, 403 forbidden, 404 not found
- Never return HTTP 200 with an error body

**JSON field naming:**
- All fields: `camelCase` (ASP.NET Core `JsonNamingPolicy.CamelCase`)
- Dates: ISO 8601 strings — `"2026-04-06T14:30:00Z"` (never Unix timestamps)
- Money: `decimal` as JSON number — `764.90` (never `float`, `double`, or strings)
- Currency: EUR only in V1 — no currency field needed
- Enums: string values — `"Active"`, `"ReadOnly"`, `"Expired"` (never integers)

**Azure Blob path structure:** `{tenancyId}/{category}/{uuid}.{ext}` — e.g. `{tenancyId}/bills/{uuid}.pdf`, `{tenancyId}/condition-report/{uuid}.jpg`

**Signed URL expiry:** 1 hour for inline viewing; 24 hours for download links

### Process Patterns

**Error handling:**
- API: `ProblemDetails` only — never expose stack traces in responses
- Frontend: error boundaries at route level; inline field errors on forms; toast notifications for async action failures
- PDF generation failures: log + retry — never silently fail (NFR-R3)
- Email failures: log + Resend auto-retry; in-app status as secondary confirmation

**Loading states:**
- All data-fetching screens: skeleton screens (never blank/white — NFR-P3)
- Skeleton components named `{ScreenName}Skeleton` — `BillingScreenSkeleton`
- "I've paid" button: optimistic update — status changes immediately on tap, rolls back on API error

**File uploads:**
- Validate MIME type + magic bytes (file signature) server-side on every upload — never trust client-declared type
- Accepted: `image/jpeg`, `image/png`, `application/pdf` only
- Max file size: single constant, never hardcoded per endpoint
- Files stored to Azure Blob, never executed or served directly from app server

**Background jobs (`IHostedService`):**
- Each job type tracked in dedicated DB table — `email_nudge_jobs`
- All jobs idempotent — safe to run twice without duplicate sends
- Polling interval: 5 minutes

### Enforcement Guidelines

**All agents MUST:**
- Use UUIDs for all resource IDs — never sequential integers in URLs
- Run per-resource authorization check on every endpoint (role check alone is insufficient)
- Apply `HasQueryFilter` tenant scoping — never query across tenant boundaries
- Validate `JWT.iat >= TokenValidFrom` on every authenticated request
- Validate file MIME type + magic bytes on every upload endpoint
- Return `ProblemDetails` for all error responses — never custom error envelopes
- Use `camelCase` for all JSON fields
- Store all monetary amounts as `decimal` — never `float` or `double`

## Project Structure & Boundaries

### Complete Project Directory Structure

```
RentEasyV2/
├── .github/
│   └── workflows/
│       ├── api-deploy.yml          ← Deploy renteasy-api to Azure App Service B1
│       └── web-deploy.yml          ← Deploy renteasy-web to Azure Static Web Apps
├── renteasy-api/                   ← ASP.NET Core 10 Web API
│   ├── renteasy-api.csproj
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   ├── Controllers/
│   │   ├── AuthController.cs               ← FR1–FR5
│   │   ├── PropertiesController.cs         ← FR6–FR9
│   │   ├── TenantsController.cs            ← FR10–FR15
│   │   ├── ConditionReportsController.cs   ← FR16–FR26
│   │   ├── BillPeriodsController.cs        ← FR27–FR35
│   │   ├── MaintenanceController.cs        ← FR36–FR39
│   │   ├── ShowcaseController.cs           ← FR56–FR58 (waitlist)
│   │   └── FilesController.cs              ← Signed URL generation
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── ApplicationUser.cs          ← Identity + TokenValidFrom + AccountState
│   │   │   ├── Property.cs
│   │   │   ├── Tenancy.cs
│   │   │   ├── WelcomePack.cs
│   │   │   ├── ConditionReport.cs
│   │   │   ├── ConditionReportItem.cs
│   │   │   ├── BillPeriod.cs
│   │   │   ├── Bill.cs
│   │   │   ├── Payment.cs
│   │   │   ├── MaintenanceRequest.cs
│   │   │   ├── WaitlistEntry.cs
│   │   │   └── EmailNudgeJob.cs            ← Background job tracking
│   │   ├── Enums/
│   │   │   ├── AccountState.cs             ← Active, ReadOnly, Expired
│   │   │   ├── BillCategory.cs             ← Rent, Electricity, Water, BuildingMaintenance
│   │   │   ├── ConditionReportStatus.cs    ← InProgress, Agreed, Unresolved
│   │   │   ├── MaintenanceStatus.cs        ← Received, InProgress, Resolved
│   │   │   ├── NudgeType.cs                ← ConditionReportDay3/7/14, PaymentDueDay3
│   │   │   └── PaymentStatus.cs            ← Unpaid, PendingConfirmation, Confirmed
│   │   └── Interfaces/
│   │       ├── IPdfService.cs
│   │       ├── IEmailService.cs
│   │       └── IStorageService.cs
│   ├── Application/
│   │   ├── Services/
│   │   │   ├── PropertyService.cs
│   │   │   ├── TenancyService.cs
│   │   │   ├── ConditionReportService.cs
│   │   │   ├── BillingService.cs
│   │   │   ├── MaintenanceService.cs
│   │   │   ├── PdfService.cs               ← QuestPDF: receipts, condition reports, bundles
│   │   │   ├── StorageService.cs           ← Azure Blob: upload, signed URLs
│   │   │   └── EmailService.cs             ← Resend: all transactional email
│   │   └── DTOs/
│   │       ├── Auth/
│   │       ├── Properties/
│   │       ├── Tenancies/
│   │       ├── ConditionReports/
│   │       ├── BillPeriods/
│   │       └── Maintenance/
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs             ← EF Core DbContext + HasQueryFilter setup
│   │   │   └── Migrations/
│   │   ├── Storage/
│   │   │   └── AzureBlobStorageClient.cs
│   │   ├── Email/
│   │   │   └── ResendEmailClient.cs
│   │   └── Jobs/
│   │       ├── NudgeSchedulerJob.cs        ← IHostedService, polls every 5 min
│   │       └── TenancyExpiryJob.cs         ← IHostedService, 12-month auto-expiry
│   └── Common/
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       ├── Middleware/
│       │   ├── TokenValidFromMiddleware.cs  ← Validates JWT.iat >= TokenValidFrom
│       │   └── ErrorHandlingMiddleware.cs   ← ProblemDetails for unhandled exceptions
│       └── Constants.cs                    ← MaxFileSize, SignedUrlExpiry, NudgeIntervals
│
└── renteasy-web/                   ← Next.js 16 frontend
    ├── next.config.ts
    ├── tailwind.config.ts
    ├── tsconfig.json
    ├── package.json
    ├── .env.local
    ├── .env.example
    └── src/
        ├── middleware.ts               ← next-intl locale routing middleware
        ├── app/
        │   └── [locale]/
        │       ├── layout.tsx
        │       ├── (public)/
        │       │   └── page.tsx               ← FR54–FR59: Showcase page
        │       ├── (auth)/
        │       │   ├── login/page.tsx         ← FR1–FR2
        │       │   └── change-password/page.tsx ← FR3–FR4
        │       ├── (landlord)/
        │       │   ├── layout.tsx             ← Landlord auth guard
        │       │   ├── dashboard/page.tsx
        │       │   ├── properties/
        │       │   │   ├── new/page.tsx       ← FR6–FR8
        │       │   │   └── [id]/
        │       │   │       ├── page.tsx       ← FR9
        │       │   │       ├── tenants/new/page.tsx      ← FR10–FR11
        │       │   │       ├── condition-report/page.tsx ← FR16, FR21, FR26
        │       │   │       ├── bills/new/page.tsx        ← FR27
        │       │   │       ├── bills/[periodId]/page.tsx ← FR32
        │       │   │       └── maintenance/page.tsx      ← FR37–FR38
        │       └── (tenant)/
        │           ├── layout.tsx             ← Tenant auth guard + AccountState check
        │           ├── welcome/page.tsx       ← FR12: welcome pack
        │           ├── condition-report/page.tsx ← FR17–FR20, FR25
        │           ├── billing/page.tsx       ← FR28–FR31
        │           ├── history/page.tsx       ← FR34–FR35
        │           └── maintenance/new/page.tsx ← FR36
        ├── api/                        ← Next.js Route Handlers (BFF layer)
        │   └── auth/
        │       ├── login/route.ts      ← Calls API, sets HttpOnly JWT cookie
        │       ├── logout/route.ts     ← Clears cookie
        │       └── me/route.ts         ← Returns current user from cookie
        ├── components/
        │   ├── ui/                     ← Button, Input, Skeleton, Modal, Toast
        │   ├── landlord/
        │   │   ├── BillUploadForm.tsx
        │   │   ├── PaymentConfirmation.tsx
        │   │   ├── ConditionReportEditor.tsx
        │   │   └── MaintenanceList.tsx
        │   └── tenant/
        │       ├── BillingScreen.tsx
        │       ├── BillingScreenSkeleton.tsx
        │       ├── PaymentMethodDisplay.tsx   ← IBAN, IRIS Pay QR, Revolut QR
        │       ├── ConditionReportViewer.tsx
        │       └── MaintenanceForm.tsx
        ├── lib/
        │   ├── api.ts                  ← API client (reads JWT cookie, forwards as Bearer)
        │   ├── qr.ts                   ← Client-side QR generation (NFR-P5)
        │   └── utils.ts                ← formatCurrency, formatDate
        └── messages/
            ├── bg.json                 ← Bulgarian (primary)
            └── en.json                 ← English

```

### Architectural Boundaries

**Request flow:**
```
Browser → Next.js BFF (/api/auth/*) → ASP.NET Core API → Neon PostgreSQL
                                                        → Azure Blob Storage
Browser → Next.js pages (authenticated) → API client (lib/api.ts) → ASP.NET Core API
API → Azure Blob (upload) → returns signed URL → Browser fetches file directly from Blob
```

**Auth boundary:** Browser never calls ASP.NET Core directly. All auth flows through Next.js BFF Route Handlers which set/clear HttpOnly cookies.

**Data boundary:** EF Core `HasQueryFilter` on all landlord-scoped entities. `TokenValidFromMiddleware` validates JWT freshness on every authenticated API request. `(tenant)/layout.tsx` enforces `AccountState` routing client-side.

**GDPR boundary:** `ApplicationUser` holds deletable profile data. All tenancy record tables (`tenancies`, `condition_reports`, `bill_periods`, `payments`, `maintenance_requests`) retained regardless of profile deletion.

### FR Category → Location Mapping

| FR Category | Backend | Frontend |
|---|---|---|
| Auth (FR1–FR5) | `AuthController` + `ApplicationUser` | `(auth)/` + `/api/auth/` BFF |
| Property Management (FR6–FR9) | `PropertiesController` + `PropertyService` | `(landlord)/properties/` |
| Tenant Management (FR10–FR15) | `TenantsController` + `TenancyService` | `(landlord)/properties/[id]/tenants/` |
| Condition Reports (FR16–FR26) | `ConditionReportsController` + `ConditionReportService` + `PdfService` | `(landlord)/.../condition-report/` + `(tenant)/condition-report/` |
| Billing & Payment (FR27–FR35) | `BillPeriodsController` + `BillingService` + `PdfService` | `(landlord)/.../bills/` + `(tenant)/billing/` |
| Maintenance (FR36–FR39) | `MaintenanceController` + `MaintenanceService` | `(tenant)/maintenance/` + `(landlord)/.../maintenance/` |
| Notifications (FR40–FR49) | `EmailService` + `NudgeSchedulerJob` | N/A (email only in V1) |
| GDPR / Data (FR50–FR53) | `TenancyService` + `TenancyExpiryJob` | N/A |
| Showcase (FR54–FR59) | `ShowcaseController` (waitlist) | `(public)/page.tsx` |
| i18n (FR60) | N/A | `messages/bg.json`, `messages/en.json`, `middleware.ts` |

## Architecture Validation Results

### Coherence Validation ✅

**Decision compatibility:** All technology choices are compatible and verified. ASP.NET Core 10 + EF Core + Npgsql + Neon is a well-documented combination. Next.js 16 + next-intl + Tailwind 4 is the current recommended stack. Azure App Service B1 + Azure Static Web Apps is the intended deployment topology. QuestPDF and Resend are independent libraries with no conflicts.

**Pattern consistency:** `ProblemDetails` on the API + route-level error boundaries on the frontend form a consistent error strategy. `HasQueryFilter` + `TokenValidFromMiddleware` are complementary security layers. `IHostedService` polling every 5 minutes aligns with the Day 3/7/14 nudge requirement — no job will be more than 5 minutes late.

**Structure alignment:** Route group layouts (`(landlord)/layout.tsx`, `(tenant)/layout.tsx`) cleanly enforce role-based access at the Next.js level, complementing API-level role checks. BFF layer at `/api/auth/` is correctly isolated from app routes.

### Requirements Coverage Validation ✅

All 60 functional requirements are mapped to specific controllers, services, and frontend routes. All 20 NFRs are architecturally addressed.

**Implementation notes (not gaps):**
- `EmailNudgeJob`: the `email_nudge_jobs` table must track a `sent_at` timestamp per nudge to ensure idempotency across 5-minute polling cycles
- `WaitlistEntry` is not a landlord-scoped entity — `HasQueryFilter` must not apply to it; `ShowcaseController` waitlist count endpoint is public (unauthenticated)

### Architecture Completeness Checklist

**Requirements Analysis**
- [x] Project context thoroughly analyzed — 60 FRs, 9 categories, all NFRs mapped
- [x] Scale and complexity assessed — Medium; solo dev V1
- [x] Technical constraints identified — Neon dual connection strings, Azure cross-domain auth, Azure Blob confirmed over R2
- [x] Cross-cutting concerns mapped — 9 concerns fully addressed

**Architectural Decisions**
- [x] Critical decisions documented — 10 decisions + M1 token revocation mitigation
- [x] Technology stack fully specified with current versions (.NET 10, Next.js 16)
- [x] Starter scaffolding commands defined
- [x] Integration patterns defined — BFF, HasQueryFilter, TokenValidFrom

**Implementation Patterns**
- [x] Naming conventions — database, API, C#, TypeScript
- [x] Structure patterns — backend layers, frontend route groups
- [x] Process patterns — error handling, loading states, file uploads, background jobs
- [x] Enforcement rules — 8 mandatory rules for all agents

**Project Structure**
- [x] Complete directory tree defined — all files mapped to FRs
- [x] Component boundaries established
- [x] Integration points mapped — request flow, auth, data, GDPR
- [x] Requirements-to-structure mapping complete

### Architecture Readiness Assessment

**Overall Status: READY FOR IMPLEMENTATION**

**Confidence level: High**

**Key strengths:**
- Stack fully researched with versions verified — no open technology questions remain
- Every FR mapped to a specific file and controller
- Security is layered: JWT validation + per-resource auth + query filters + file signature validation
- GDPR data split enforced at schema level, not just application logic
- `TokenValidFrom` + account state machine covers all tenant lifecycle transitions cleanly

**Areas for future enhancement (V2+):**
- Viber notification channel (parallel to email — pending Viber Business API partner agreement)
- Refresh token flow (currently deferred — M1 revocation is sufficient for V1)
- Staging environment
- Evrotrust QES integration for legally binding condition report sign-off
- Self-serve landlord registration

### Implementation Handoff

**AI Agent Guidelines:**
- Follow all architectural decisions exactly as documented
- Use implementation patterns consistently across all components
- Respect project structure and boundaries defined in this document
- Refer to this document for all architectural questions before making implementation choices

**First implementation priority:**
```bash
# Step 1 — Scaffold monorepo
npx create-next-app@latest renteasy-web
dotnet new webapi --use-controllers --use-program-main -o renteasy-api

# Step 2 — Database + auth foundation
# Add Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.AspNetCore.Identity.EntityFrameworkCore
# Create AppDbContext with HasQueryFilter, ApplicationUser with TokenValidFrom
# Configure two Neon connection strings (pooled + direct)
# Run first migration

# Step 3 — BFF auth layer
# Implement AuthController (JWT issuance)
# Implement TokenValidFromMiddleware
# Implement Next.js /api/auth/ Route Handlers
```

## Project Context Analysis

### Requirements Overview

**Functional Requirements:**

60 FRs across 9 categories — Authentication & Account Management (FR1–FR5), Property Management (FR6–FR9), Tenant Management (FR10–FR15), Condition Reports (FR16–FR26), Bill Management & Payment Flow (FR27–FR35), Maintenance Requests (FR36–FR39), Notifications & Communications (FR40–FR49), Data Management & GDPR (FR50–FR53), Public Showcase Page (FR54–FR59), and Internationalisation (FR60).

The most architecturally significant FRs are:
- **FR22/FR23/FR25** — Condition report iterative dispute flow (up to 3 rounds), full dispute history in PDF, round/status visibility for both parties. Most complex stateful workflow in the system.
- **FR14/FR15** — Tenant account state machine: Active → Read-Only → Expired (12-month auto-expiry). Must be enforced at the authorization layer.
- **FR33/FR49/FR50** — PDF generation is a critical-path dependency: receipt PDF on every payment, condition report PDF on sign-off, final bundle at move-out. 100% reliability required.
- **FR41/FR44/FR45/FR46** — Background email scheduling (Day 3/7/14 nudges). Requires background job infrastructure (not just synchronous email dispatch).
- **FR19/FR46** — Timestamped PDF with tenant identity explicitly recorded — legally significant, not just a technical output.

**Non-Functional Requirements:**

NFRs that directly drive architectural decisions:
- **NFR-S3/S4** — UUID resource IDs everywhere; per-resource authorization on every endpoint. Not just role-based — resource ownership must be checked.
- **NFR-S5** — MIME type + file signature validation on all uploads. Not just extension checking.
- **NFR-SC1** — Multi-tenant data model from day one (Landlord → Properties → Tenancies). V1 UI surfaces one landlord; architecture must not be single-tenant.
- **NFR-R3** — 100% PDF generation success rate; failures must be logged and retried.
- **NFR-P4** — PDFs served as signed URLs; never embedded in page payloads.
- **NFR-P5** — QR code generation client-side in under 100ms.
- **NFR-I1** — Transactional email retries automatically on failure.

**Scale & Complexity:**

- Primary domain: Full-stack web application (Next.js frontend + ASP.NET Core 8 API)
- Complexity level: Medium (multi-role, multi-tenant, iterative stateful workflow, PDF generation pipeline, GDPR data split, i18n, background scheduling)
- Estimated architectural components: 8 (auth/identity, property/tenancy domain, condition report workflow, billing & payment, maintenance, notifications, PDF pipeline, file storage)

### Technical Constraints & Dependencies

**Confirmed stack (from technical research — all decisions resolved):**

| Layer | Technology | Cost |
|---|---|---|
| Backend | ASP.NET Core 8 Web API (Controllers + EF Core) | — |
| Frontend | Next.js 15 + App Router + next-intl | — |
| Database | Neon serverless PostgreSQL | Free |
| File Storage | Cloudflare R2 (S3-compatible) | Free forever |
| PDF Generation | QuestPDF (free <$1M revenue) | Free |
| Email | Resend (3,000/month free, EU region) | Free |
| Auth | ASP.NET Core Identity + JWT (BFF pattern) | Free |
| Backend Hosting | Azure App Service B1 | ~$13/mo |
| Frontend Hosting | Azure Static Web Apps (Free) | Free |

**Known technical constraints:**
- Azure Static Web Apps + Azure App Service run on separate root domains pre-custom-domain — cross-domain cookie sharing is not possible. Auth must use Next.js BFF pattern: JWT stored in HttpOnly cookie on SWA domain, forwarded as Bearer header to API.
- Neon requires two connection strings: pooled (runtime queries) and direct/non-pooled (EF Core migrations). Using pooled endpoint for migrations causes failures.
- Azure App Service F1 (free) has 60 CPU-minutes/day quota and mandatory sleep — unsuitable for real users. B1 is the minimum viable production tier.
- Cloudflare R2 requires AWS SDK for .NET with `DisablePayloadSigning = true` and `DisableDefaultChecksumValidation = true`.
- QuestPDF: use `Lazy` element for memory-efficient processing of photo-heavy condition report PDFs.

**GDPR data split constraint (architectural, not just policy):**
- Profile data (name, email, phone, credentials) — deletable on request (FR52)
- Tenancy record data (condition reports, payment history, receipts, bill PDFs) — retained under Art. 17(3)(b) regardless of deletion requests (FR53)
- This split must be enforced at the data model level, not just in deletion logic

### Cross-Cutting Concerns Identified

1. **Multi-tenancy** — EF Core global query filters (`HasQueryFilter`) scoped to `LandlordId` on all tenant-scoped entities. `DbContext` registered as Scoped. All `LandlordId`/`TenantId` columns indexed.
2. **Per-resource authorization** — Role checks alone are insufficient. Every endpoint must verify the authenticated user owns the resource being accessed (NFR-S4). Tenant users must never access another tenant's data.
3. **GDPR data split** — Data model must separate deletable profile fields from retained tenancy record fields at the schema level.
4. **PDF generation pipeline** — Synchronous for payment receipts and condition report sign-offs (user waits for result); background/retry for auto-resolution at Day 14. All failures logged and retried (NFR-R3).
5. **Background job scheduling** — Email nudges at Day 3/7/14 require a scheduler. Options: Azure Functions timer trigger, Hangfire, or hosted `IHostedService` with a cron-like loop. Must be evaluated in architectural decisions.
6. **File upload validation** — MIME type + file signature (magic bytes) validation on every upload. JPEG, PNG, PDF only. Files stored to R2, never executed or served directly from the app server.
7. **i18n** — next-intl for all UI strings; language in URL prefix (`/bg/`, `/en/`). User-generated content stored as-entered; no server-side translation.
8. **Signed URL serving** — All PDFs and uploaded files served via Cloudflare R2 presigned URLs with appropriate expiry. Expiry strategy to be decided (short-lived with regeneration on access vs. long-lived).
9. **Tenant account state machine** — Account state (Active / ReadOnly / Expired) enforced at authorization middleware, not just in business logic. Auto-expiry at 12 months post-move-out must be handled by a background job.
