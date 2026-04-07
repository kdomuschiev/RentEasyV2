# Story 1.1: Monorepo Scaffold & CI/CD Pipeline

Status: ready-for-dev

## Story

As a developer,
I want a fully scaffolded monorepo with both projects initialized and deployed to production via CI/CD,
So that the team has a working production baseline from day one and all subsequent stories can be merged and deployed automatically.

## Acceptance Criteria

1. **Given** the monorepo root at `RentEasyV2/`
   **When** `npx create-next-app@latest renteasy-web` is run with TypeScript, ESLint, Tailwind CSS, App Router, src/, and Turbopack selected
   **Then** the `renteasy-web/` directory contains a working Next.js 16 application that builds successfully with `npm run build`

2. **Given** the monorepo root
   **When** `dotnet new webapi --use-controllers --use-program-main -o renteasy-api` is run
   **Then** the `renteasy-api/` directory contains a working ASP.NET Core 10 Web API with controller support, explicit `Program.cs`, and `dotnet build` passes

3. **Given** both projects are scaffolded
   **When** all additional packages are installed
   **Then** both projects build without errors

4. **Given** code is pushed to the `main` branch
   **When** GitHub Actions runs
   **Then** `api-deploy.yml` builds and deploys `renteasy-api` to Azure App Service B1
   **And** `web-deploy.yml` builds and deploys `renteasy-web` to Azure Static Web Apps

5. **Given** the production deployment has completed
   **When** an HTTP GET is made to the API's health/status endpoint
   **Then** a 200 response is returned

6. **Given** the production deployment has completed
   **When** a browser visits the Azure Static Web App URL
   **Then** the Next.js application loads without errors

## Tasks / Subtasks

- [ ] Scaffold monorepo root structure (AC: 1, 2)
  - [ ] Create `RentEasyV2/` root with `.gitignore`, `README.md`
  - [ ] Run `npx create-next-app@latest renteasy-web` — select: TypeScript ✅, ESLint ✅, Tailwind CSS ✅, App Router ✅, src/ ✅, Turbopack ✅
  - [ ] Run `dotnet new webapi --use-controllers --use-program-main -o renteasy-api`
  - [ ] Verify `npm run build` passes in `renteasy-web/`
  - [ ] Verify `dotnet build` passes in `renteasy-api/`

- [ ] Install all additional packages (AC: 3)
  - [ ] Frontend: `npm install next-intl qrcode` and `npm install -D @types/qrcode`
  - [ ] Backend: `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL`
  - [ ] Backend: `dotnet add package Azure.Storage.Blobs`
  - [ ] Backend: `dotnet add package QuestPDF`
  - [ ] Backend: `dotnet add package resend-dotnet`
  - [ ] Backend: `dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - [ ] Verify both projects still build after package installs

- [ ] Set up folder structure (AC: 2)
  - [ ] Create backend folder skeleton: `Controllers/`, `Domain/Entities/`, `Domain/Enums/`, `Domain/Interfaces/`, `Application/Services/`, `Application/DTOs/`, `Infrastructure/Data/`, `Infrastructure/Storage/`, `Infrastructure/Email/`, `Infrastructure/Jobs/`, `Common/Extensions/`, `Common/Middleware/`
  - [ ] Create frontend folder skeleton: `src/app/[locale]/(public)/`, `src/app/[locale]/(auth)/`, `src/app/[locale]/(landlord)/`, `src/app/[locale]/(tenant)/`, `src/api/auth/`, `src/components/ui/`, `src/components/landlord/`, `src/components/tenant/`, `src/lib/`, `src/messages/`

- [ ] Create CI/CD GitHub Actions workflows (AC: 4)
  - [ ] Create `.github/workflows/api-deploy.yml` — build `renteasy-api`, deploy to Azure App Service B1
  - [ ] Create `.github/workflows/web-deploy.yml` — build `renteasy-web`, deploy to Azure Static Web Apps
  - [ ] Add required GitHub Secrets: `AZURE_WEBAPP_PUBLISH_PROFILE`, `AZURE_STATIC_WEB_APPS_API_TOKEN`

- [ ] Add health endpoint to API (AC: 5)
  - [ ] Add `GET /api/health` returning 200 `{ "status": "ok" }` in a `HealthController`

- [ ] Verify production deployments (AC: 5, 6)
  - [ ] Push to `main`, confirm both workflows pass
  - [ ] Confirm `GET /api/health` returns 200 on production URL
  - [ ] Confirm Next.js SWA URL loads without errors

## Dev Notes

### Exact Scaffold Commands

```bash
# From RentEasyV2/ root
npx create-next-app@latest renteasy-web
# Answers: TypeScript=Yes, ESLint=Yes, Tailwind=Yes, src/=Yes, App Router=Yes, Turbopack=Yes, import alias=No

dotnet new webapi --use-controllers --use-program-main -o renteasy-api
```

### Package Versions (latest stable as of 2026-04)

**Frontend (`renteasy-web/`):**
- `next` — 16.x (installed by create-next-app)
- `tailwindcss` — 4.x (installed by create-next-app)
- `next-intl` — install latest; used for BG/EN i18n with URL-prefix routing
- `qrcode` + `@types/qrcode` — used in Epic 3 for IRIS Pay / Revolut QR codes; install now so it's available

**Backend (`renteasy-api/`):**
- `Npgsql.EntityFrameworkCore.PostgreSQL` — EF Core provider for Neon PostgreSQL
- `Azure.Storage.Blobs` — Azure Blob Storage SDK
- `QuestPDF` — PDF generation (synchronous for receipts, background for auto-resolution)
- `resend-dotnet` — Transactional email via Resend (EU region, 3,000/month free)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` — ASP.NET Core Identity + EF Core integration

### Monorepo Layout

```
RentEasyV2/
├── renteasy-web/
├── renteasy-api/
├── .github/workflows/
│   ├── api-deploy.yml
│   └── web-deploy.yml
└── README.md
```

### Backend Folder Structure

```
renteasy-api/
├── Controllers/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── Application/
│   ├── Services/
│   └── DTOs/
├── Infrastructure/
│   ├── Data/
│   ├── Storage/
│   ├── Email/
│   └── Jobs/
├── Common/
│   ├── Extensions/
│   └── Middleware/
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

### Frontend Folder Structure

```
renteasy-web/src/
├── app/
│   └── [locale]/
│       ├── (public)/
│       ├── (auth)/
│       ├── (landlord)/
│       └── (tenant)/
├── api/
│   └── auth/
├── components/
│   ├── ui/
│   ├── landlord/
│   └── tenant/
├── lib/
└── messages/
    ├── bg.json
    └── en.json
```

### CI/CD Notes

- GitHub repo: `kdomuschiev/RentEasyV2`
- Trigger: push to `main` branch only
- Two separate workflow files — one per project
- `api-deploy.yml`: uses `azure/webapps-deploy` action targeting App Service B1
- `web-deploy.yml`: uses `Azure/static-web-apps-deploy` action
- Required secrets to configure in GitHub repo settings:
  - `AZURE_WEBAPP_PUBLISH_PROFILE` — download from Azure Portal → App Service → Deployment Center
  - `AZURE_STATIC_WEB_APPS_API_TOKEN` — available in Azure Portal → Static Web App → Manage deployment token

### Health Endpoint

Place in `Controllers/HealthController.cs`:
```csharp
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}
```
No auth required on this endpoint.

### Naming Conventions (apply from day 1)

- C# classes/methods: `PascalCase`; variables/params: `camelCase`; private fields: `_camelCase`; async methods: `Async` suffix
- TypeScript components: `PascalCase` files matching component name; hooks: `use` prefix; utilities: `camelCase`
- DB tables: `snake_case` plural; columns: `snake_case`; FK: `{table_singular}_id`

### Global Rules (enforce from story 1)

- All resource IDs: UUIDs — never sequential integers
- All API errors: RFC 7807 `ProblemDetails`
- JSON field names: `camelCase` (`JsonNamingPolicy.CamelCase` in `Program.cs`)
- Monetary amounts: `decimal` — never `float` or `double`

### Project Structure Notes

- This story creates the canonical folder structure — all subsequent stories must place files in these locations
- `src/messages/bg.json` and `src/messages/en.json` should be created as empty objects `{}` now; they'll be populated per story
- `appsettings.Development.json` should be git-ignored (contains local secrets); `appsettings.json` has placeholder keys only

### References

- [Source: architecture.md — Monorepo Structure]
- [Source: architecture.md — Tech Stack Overview]
- [Source: architecture.md — Repository Layout & Folder Structure]
- [Source: architecture.md — Naming Conventions]
- [Source: architecture.md — Global Enforcement Rules]
- [Source: architecture.md — CI/CD Deployment Patterns]
- [Source: epics.md — Epic 1, Story 1.1]

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

### Completion Notes List

### File List
