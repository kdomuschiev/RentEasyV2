# RentEasyV2 — Claude Instructions

## Project Overview

RentEasy (`renteasy.bg`) is a mobile-first rental management platform for private landlords and their tenants in Bulgaria. Its core value proposition is **billing transparency** — itemised charges with attached utility bill PDFs, structured digital tenancy workflows, and professional documentation that neither side has had access to before in the Bulgarian market.

**Target users:**
- **Landlords:** Private Bulgarian landlords (1–5 properties). V1 is built for one landlord (Kiril) and one apartment in Sofia, but the architecture supports multi-landlord from day one.
- **Tenants:** Bulgarian renters aged 25–45, digitally comfortable. May include expats (English fully supported).

**V1 scope (MVP):** Public showcase page, tenant onboarding with welcome pack, room-by-room condition reports with iterative sign-off (max 3 dispute rounds), monthly billing with itemised breakdowns and attached PDFs, three manual payment channels (IBAN, IRIS Pay, Revolut), landlord payment confirmation with auto-generated receipts, maintenance request tracking, email notifications, GDPR-compliant data retention, BG/EN i18n throughout.

**V2+ roadmap (do not build for V1):** Viber notifications, interactive welcome pack, Stripe embedded payments (requires EOOD registration), Evrotrust QES digital signatures, self-serve landlord registration, automated utility bill ingestion, lease generation.

## Architecture & Tech Stack

| Layer | Technology | Rationale |
|---|---|---|
| Backend | ASP.NET Core 10 Web API (Controllers + EF Core) | Controller-based for consistent cross-cutting policies (auth, logging, error handling) via filter pipeline |
| Frontend | Next.js 16 + App Router + TypeScript (strict) | RSC, layouts, server components; Turbopack for dev |
| Database | Neon PostgreSQL (EU Frankfurt) | Free tier; requires two connection strings — pooled (runtime) + direct (migrations) |
| File Storage | Azure Blob Storage (`files` container) | Signed URLs for serving; path: `{tenancyId}/{category}/{uuid}.{ext}` |
| PDF Generation | QuestPDF | Free <$1M revenue; server-side, no external dependency |
| Email | Resend (EU region, 3,000/month free) | Transactional email with PDF attachments |
| Auth | ASP.NET Core Identity + JWT (7-day lifetime) | BFF pattern: JWT in HttpOnly cookie on SWA domain, forwarded as Bearer to API |
| CSS | Tailwind CSS 4.x + shadcn/ui (authenticated app) | Custom Tailwind for showcase page; shadcn/ui (Radix primitives) for app |
| i18n | next-intl | URL-prefix routing (`/bg/`, `/en/`); Bulgarian is default |
| Backend Hosting | Azure App Service B1 (~$13/mo) | Minimum viable production tier (F1 unsuitable — 60 CPU-min/day quota) |
| Frontend Hosting | Azure Static Web Apps (Free) | |
| CI/CD | GitHub Actions | `api-deploy.yml` → App Service; `web-deploy.yml` → SWA |

**Key architectural patterns:**

- **Auth:** Next.js BFF pattern — browser never calls ASP.NET Core directly for auth. Route Handlers at `/api/auth/*` set/clear HttpOnly cookies. Required because Azure SWA and App Service are on different root domains (cross-domain cookies impossible).
- **Token revocation:** `TokenValidFrom` datetime column on user table. API validates `JWT.iat >= TokenValidFrom` on every request via `TokenValidFromMiddleware`. Updated on move-out and state transitions.
- **Account state machine:** `Active` → `ReadOnly` (immediate on move-out) → `Expired` (12-month auto-expiry). Baked into JWT claim, re-issued on state change.
- **Multi-tenancy:** EF Core `HasQueryFilter` scoped to `LandlordId` on all landlord-scoped entities. Single DB with discriminator column. `WaitlistEntry` is NOT landlord-scoped.
- **Background jobs:** `IHostedService` + EF Core polling every 5 minutes. Two jobs: `NudgeSchedulerJob` (Day 3/7/14 condition report + Day 3 payment nudges) and `TenancyExpiryJob` (12-month auto-expiry). All jobs idempotent.
- **File serving:** All PDFs and uploads served via Azure Blob signed URLs. 1 hour expiry for inline viewing, 24 hours for downloads. ⛔ Never serve files directly from the app server.
- **Error handling:** RFC 7807 `ProblemDetails` for all API errors. ⛔ Never custom error envelopes.

**Environments:** Development (local) + Production only. No staging in V1.

## Project Structure

```
RentEasyV2/
├── .github/workflows/
│   ├── api-deploy.yml              ← Azure App Service B1
│   └── web-deploy.yml              ← Azure Static Web Apps
├── RentEasy.Api/                   ← ASP.NET Core 10 Web API
│   ├── Controllers/                ← One controller per resource domain
│   ├── Domain/
│   │   ├── Entities/               ← EF Core entity classes (PascalCase singular)
│   │   ├── Enums/                  ← AccountState, BillCategory, PaymentStatus, etc.
│   │   └── Interfaces/             ← IPdfService, IEmailService, IStorageService
│   ├── Application/
│   │   ├── Services/               ← Business logic per domain
│   │   └── DTOs/                   ← Request/response DTOs, organised by domain subfolder
│   ├── Infrastructure/
│   │   ├── Data/                   ← AppDbContext, migrations, HasQueryFilter setup
│   │   ├── Storage/                ← Azure Blob client wrapper
│   │   ├── Email/                  ← Resend wrapper
│   │   └── Jobs/                   ← IHostedService background jobs
│   └── Common/
│       ├── Extensions/
│       ├── Middleware/             ← TokenValidFromMiddleware, ErrorHandlingMiddleware
│       └── Constants.cs            ← MaxFileSize, SignedUrlExpiry, NudgeIntervals
├── RentEasy.Web/                   ← Next.js 16 frontend
│   └── src/
│       ├── app/[locale]/
│       │   ├── (public)/           ← Showcase page (static, unauthenticated)
│       │   ├── (auth)/             ← Login, password change
│       │   ├── (landlord)/         ← Landlord dashboard routes (auth guard in layout)
│       │   └── (tenant)/           ← Tenant portal routes (auth guard + AccountState in layout)
│       ├── api/auth/               ← BFF Route Handlers (login/logout/me)
│       ├── components/
│       │   ├── ui/                 ← shadcn/ui + custom: Button, Skeleton, Toast, etc.
│       │   ├── landlord/           ← BillUploadForm, PaymentConfirmation, etc.
│       │   └── tenant/             ← BillingScreen, PaymentMethodDisplay, etc.
│       ├── lib/                    ← api.ts (API client), qr.ts, utils.ts
│       └── messages/               ← bg.json, en.json (next-intl translation files)
├── _bmad/                          ← BMad module config (do not modify during implementation)
├── _bmad-output/                   ← Planning artifacts (PRD, architecture, epics, UX spec)
└── CLAUDE.md
```

**Tests:** Co-located with source — `PropertyService.test.cs` next to `PropertyService.cs`; `BillingScreen.test.tsx` next to `BillingScreen.tsx`.

## Naming Conventions

**Database (PostgreSQL via EF Core):**
- Tables: `snake_case` plural — `landlords`, `properties`, `tenancies`, `bill_periods`, `email_nudge_jobs`
- Columns: `snake_case` — `landlord_id`, `created_at`, `token_valid_from`
- Foreign keys: `{referenced_table_singular}_id` — `landlord_id`, `tenancy_id`
- Indexes: `ix_{table}_{column(s)}` — `ix_properties_landlord_id`

**API endpoints:**
- Resources: plural kebab-case — `/api/properties`, `/api/bill-periods`, `/api/condition-reports`
- Route params: `{id}` style — `/api/properties/{id}/tenancies/{tenancyId}`
- Query params: `camelCase` — `?landlordId=...`
- ⛔ All resource IDs: UUIDs — never sequential integers in URLs

**C# code:**
- Classes/Methods: `PascalCase`; entity classes are singular — `Property`, `Tenancy`, `BillPeriod`
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
- Skeleton components: `{ScreenName}Skeleton` — `BillingScreenSkeleton`

**JSON:**
- All fields: `camelCase` (ASP.NET Core `JsonNamingPolicy.CamelCase`)
- Dates: ISO 8601 strings — `"2026-04-06T14:30:00Z"` (never Unix timestamps)
- Money: `decimal` as JSON number — `764.90` (⛔ never `float`, `double`, or strings)
- Enums: string values — `"Active"`, `"ReadOnly"`, `"Expired"` (⛔ never integers)
- Currency: EUR only in V1 — no currency field needed

## Coding Standards & Patterns

**API responses:**
- Success: direct data, no wrapper. HTTP 200 success, 201 creation, 204 deletion.
- Errors: RFC 7807 `ProblemDetails` — `type`, `title`, `status`, `detail`. HTTP 400/401/403/404.
- ⛔ Never return HTTP 200 with an error body.
- Return 404 (not 403) when a resource belongs to another user — prevent resource enumeration.

**Loading states:**
- All data-fetching screens: skeleton screens (⛔ never blank/white loading states).
- "I've paid" button: optimistic update — status changes immediately on tap, rolls back on API error.

**File uploads:**
- ⛔ Validate MIME type + magic bytes (file signature) server-side on every upload — never trust client-declared type.
- Accepted: `image/jpeg`, `image/png`, `application/pdf` only.
- Max file size: single constant in `Constants.cs`, never hardcoded per endpoint.
- Files stored to Azure Blob, never executed or served directly from app server.

**Background jobs (`IHostedService`):**
- Each job type tracked in dedicated DB table — `email_nudge_jobs`.
- ⛔ All jobs must be idempotent — safe to run twice without duplicate sends.
- `sent_at` timestamp per nudge ensures idempotency across 5-minute polling cycles.

**Security (all non-negotiable pre-launch):**
- ⛔ Run per-resource authorization check on every endpoint (role check alone is insufficient).
- ⛔ Apply `HasQueryFilter` tenant scoping — never query across tenant boundaries.
- ⛔ Validate `JWT.iat >= TokenValidFrom` on every authenticated request.
- ⛔ Use parameterised EF Core queries only — never interpolate user input into SQL.
- ⛔ Never use `dangerouslySetInnerHTML` with user-supplied content.
- ⛔ HTTPS everywhere — no HTTP endpoints in production.
- ⛔ Store all monetary amounts as `decimal` — never `float` or `double`.

**General principles:**
- Clean Architecture layer boundaries: Controllers → Application (Services/DTOs) → Domain (Entities/Interfaces/Enums) → Infrastructure. Dependencies point inward.
- Server Components + `useState`/`useReducer` only — no heavy client-side state management, no global store.
- No caching in V1 — Neon handles load at this scale.
- No real-time (WebSockets/SSE/polling) in V1 — status refreshes on navigation or manual reload.
- Neon connection strings: pooled for runtime, direct for migrations. ⛔ Never use pooled endpoint for `dotnet ef database update`.

**Azure Blob path structure:** `{tenancyId}/{category}/{uuid}.{ext}` — e.g. `{tenancyId}/bills/{uuid}.pdf`, `{tenancyId}/condition-report/{uuid}.jpg`.

**Signed URL expiry:** 1 hour for inline viewing; 24 hours for download links. Regenerate on access if expired.

## Domain Concepts & Terminology

**Core entities and relationships:**
- `Landlord` (ApplicationUser with Landlord role) → owns many `Property` → each has many `Tenancy`
- `Tenancy` → has `ConditionReport` (move-in + move-out) → has many `ConditionReportItem` (contributed by either party)
- `Tenancy` → has many `BillPeriod` → each has many `Bill` (one per category) → each has `Payment`
- `Tenancy` → has many `MaintenanceRequest`
- `Property` → has `WelcomePack` (apartment manual, contacts, WiFi, etc.)
- `WaitlistEntry` — not landlord-scoped; public showcase page email capture

**Account state machine:**
- `Active` — full platform access (default on creation)
- `ReadOnly` — immediate on move-out; tenant can view/download own data only
- `Expired` — 12 months after move-out; access fully revoked; no landlord action required

**Condition report flow (most complex workflow):**
1. Landlord pre-loads 10–15 dispute-prone items with photos before tenant first login
2. Tenant sees Safety Intro Screen ("This report protects you"), then reviews landlord baseline (read-only)
3. Tenant adds own items on top (photos + notes)
4. Tenant chooses: **Agree** → timestamped PDF generated and emailed to both | **Disagree** → uploads disputed items
5. Landlord reviews, accepts/rejects, re-requests sign-off
6. Max 3 dispute rounds. After 3 without agreement → documented as unresolved with full history
7. Day 3/7 email nudges; Day 14 auto-resolution — PDF generated with all items recorded to that point
8. PDF includes **full dispute history** — all rounds, all photos, all notes, all timestamps, final status

**Billing flow:**
1. Landlord uploads PDF per bill category (Rent, Electricity, Water, Building Maintenance) + enters total amount
2. Tenant views itemised breakdown with inline PDFs
3. Tenant pays externally (IBAN / IRIS Pay QR / Revolut QR), then taps "I've paid" → status: "Pending confirmation"
4. Landlord verifies bank receipt, confirms in app → receipt PDF auto-generated and emailed to tenant
5. No automated reconciliation — wrong amounts handled outside app

**Payment methods (display only, no gateway in V1):**
- IBAN: account number + one-tap clipboard copy
- IRIS Pay: phone number + QR code (A2A instant via Bulgarian banking apps)
- Revolut.me: link + QR code; tenant enters amount manually

**GDPR data split policy:**
- **Profile data** (name, email, phone, credentials) — deletable on GDPR request
- **Tenancy record data** (condition reports, payments, receipts, bill PDFs) — ⛔ retained under Art. 17(3)(b) + Bulgarian tax law (5–7 years post-tenancy); not deletable
- One-sentence disclosure at onboarding: "Your tenancy records are kept for 7 years after move-out as required by Bulgarian law."
- Deletion request response template must exist before launch

**Enums:**
- `AccountState`: Active, ReadOnly, Expired
- `BillCategory`: Rent, Electricity, Water, BuildingMaintenance
- `ConditionReportStatus`: InProgress, Agreed, Unresolved
- `MaintenanceStatus`: Received, InProgress, Resolved
- `PaymentStatus`: Unpaid, PendingConfirmation, Confirmed
- `NudgeType`: ConditionReportDay3, ConditionReportDay7, ConditionReportDay14, PaymentDueDay3

## Do's and Don'ts

**Do:**
- Use UUIDs for all resource IDs
- Return `ProblemDetails` for every error response
- Use `HasQueryFilter` on every new landlord-scoped entity
- Validate file uploads with MIME type + magic bytes
- Use skeleton screens on every data-fetching screen
- Use `decimal` for all monetary amounts
- Keep all infrastructure scaling as config-only changes (Azure portal / Neon dashboard / Resend plan)
- Use `Lazy` element in QuestPDF for photo-heavy condition report PDFs
- Log + retry all PDF generation failures (⛔ never silently fail)
- Use `camelCase` for all JSON fields

**Don't:**
- ⛔ Don't commit directly to `main`
- ⛔ Don't use sequential/guessable IDs in URLs
- ⛔ Don't embed PDF content in page payloads — always use signed Azure Blob URLs
- ⛔ Don't use `dangerouslySetInnerHTML` with user-supplied content
- ⛔ Don't use `float` or `double` for money
- ⛔ Don't use integer enum values in JSON — always string
- ⛔ Don't use Unix timestamps — always ISO 8601
- ⛔ Don't use pooled Neon connection string for migrations
- ⛔ Don't apply `HasQueryFilter` to `WaitlistEntry` (it's not landlord-scoped)
- ⛔ Don't add hover-only interactions — all functionality must be available via tap
- ⛔ Don't use placeholder-only form labelling — all inputs need `<label>` elements
- ⛔ Don't build Viber notifications, Stripe payments, Evrotrust QES, or any V2+ features
- ⛔ Don't add caching, refresh tokens, or staging environment in V1
- ⛔ Don't use custom error envelopes — RFC 7807 `ProblemDetails` only

## Testing Approach

- Unit + integration tests co-located with source files
- Backend: `dotnet test` with SQLite in-memory for fast tests
- ⛔ PDF generation: build and test early — not last (highest-risk feature for complexity)
- NFR-R3: every PDF generation path must be verified — receipts, condition report sign-offs, auto-resolutions, move-out bundles
- Cross-tenant isolation: verify that User A's request never returns User B's data on every new endpoint
- File upload validation: test MIME + magic bytes for JPEG, PNG, PDF; reject everything else
- Background job idempotency: verify no duplicate sends when job runs twice

## Branching Strategy

- `main` is the stable branch — never commit directly to it
- Before starting any piece of work, create a branch from `main`:
  - `feat-[brief-description]` for new functionality (e.g. `feat-property-setup`)
  - `fix-[brief-description]` for bug fixes (e.g. `fix-jwt-expiry`)
  - `bmad-[brief-description]` when changes are only in `_bmad/` or `_bmad-output/` (e.g. `bmad-update-prd`)

## Merging

- When the user says a PR has been merged, run `git checkout main && git pull` to sync the local main branch
- If there are uncommitted changes when switching to main, stash them first with `git stash` before checking out

## Pull Requests

- After every commit, create a PR from the current branch into `main` using `gh pr create` — only if a PR for that branch does not already exist
- PR titles must follow the format: `[feat] Short description`, `[fix] Short description`, or `[bmad] Short description` (matching the branch type)
