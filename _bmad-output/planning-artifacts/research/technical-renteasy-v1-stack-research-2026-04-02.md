---
stepsCompleted: [1, 2, 3, 4, 5, 6]
inputDocuments:
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md
  - _bmad-output/planning-artifacts/research/domain-private-landlord-tools-software-research-2026-04-02.md
workflowType: 'research'
lastStep: 2
research_type: 'technical'
research_topic: 'Tech stack selection for RentEasy V1 — .NET/Azure-aligned, free-tier-first'
research_goals: 'Identify the right technologies across backend, frontend, database, file storage, PDF generation, email, auth, and hosting for RentEasy V1 — leveraging .NET/C#/Azure/React skills with free-tier constraints'
user_name: 'Kiril'
date: '2026-04-02'
web_research_enabled: true
source_verification: true
---

# Research Report: Tech Stack Selection for RentEasy V1

**Date:** 2026-04-02
**Author:** Kiril
**Research Type:** Technical

---

## Executive Summary

RentEasy V1 requires a technology stack that leverages an existing .NET/C#/Azure/React skillset, operates at zero or near-zero cost during the pre-launch phase, and scales cleanly to V2 without architectural rewrites. This research evaluated eight technology decision areas through parallel web research across 14 dedicated searches, all verified against current public sources (2025–2026).

**The core finding:** a fully capable, production-ready V1 stack can be assembled entirely from free tiers at launch, rising to approximately $13/month when a custom domain is purchased and the App Service is upgraded. No architectural changes are required at that transition — only configuration updates.

**Confirmed stack:**

| Layer | Technology | Cost |
|---|---|---|
| Backend | ASP.NET Core 8 Web API | — |
| Frontend | React + Next.js 15 (static export) + next-intl | — |
| Database | Neon serverless PostgreSQL | Free |
| File Storage | Azure Blob Storage | Free 12mo → ~$0.15/mo |
| PDF Generation | QuestPDF | Free (<$1M revenue) |
| Email | Resend | Free (3,000/mo) |
| Auth | ASP.NET Core Identity + JWT | Free |
| Backend Hosting | Azure App Service F1 → B1 | $0 → ~$13/mo |
| Frontend Hosting | Azure Static Web Apps Free | Free |
| Monitoring | Azure Application Insights | Free (5 GB/mo) |
| **Total at launch** | | **$0/month** |

**Key decisions resolved by this research:**
- **iTextSharp is abandoned** — QuestPDF is the .NET PDF winner, free at startup scale, zero infrastructure overhead
- **MediatR went commercial in 2024** — use CQRS by convention with direct handler injection; no mediator library needed
- **Generic repository over EF Core is an anti-pattern** — inject `DbContext` directly; use Specification pattern for complex queries
- **Azure SWA hybrid SSR and linked App Service backend are mutually exclusive** — static export mode gives zero-CORS linked proxy; showcase page pre-renders at build time (sufficient for SEO)
- **Neon vs Supabase:** Supabase pauses the entire project after 7 days of inactivity; Neon only suspends compute
- **Auth across Azure default domains:** ASP.NET Core issues JWTs; Next.js stores them in its own HttpOnly cookie and forwards as Bearer headers — no cross-domain cookie complexity

---

## Research Overview

This research covers the full technology stack for RentEasy V1 — a mobile-first, bilingual (Bulgarian/English) rental management web application for private landlords in Bulgaria. The research is anchored to the developer's existing skills (.NET/C#, SQL, Azure, Angular, React) and a hard constraint of free-tier or low-cost services.

Fourteen parallel web research agents were executed across six phases: technology stack selection, integration patterns, architectural patterns, and implementation approaches. All findings are sourced from current public documentation and community evidence (2025–2026), with multi-source validation on critical claims.

---

## Table of Contents

1. [Technical Research Scope Confirmation](#technical-research-scope-confirmation)
2. [Technology Stack Analysis](#technology-stack-analysis) — Backend, Frontend, Database, Storage, PDF, Email, Auth, Hosting
3. [Integration Patterns Analysis](#integration-patterns-analysis) — API design, multi-tenancy, Next.js ↔ ASP.NET Core, Azure Blob, file uploads, email
4. [Architectural Patterns and Design](#architectural-patterns-and-design) — Modular monolith, Clean Architecture, CQRS, security, CI/CD
5. [Implementation Approaches and Technology Adoption](#implementation-approaches-and-technology-adoption) — Local dev, testing, migrations, monitoring, risk register

---

## Technical Research Scope Confirmation

**Research Topic:** Tech stack selection for RentEasy V1 — .NET/Azure-aligned, free-tier-first
**Research Goals:** Identify the right technologies across backend, frontend, database, file storage, PDF generation, email, auth, and hosting for RentEasy V1 — leveraging .NET/C#/Azure/React skills with free-tier constraints

**Technical Research Scope:**

- Architecture Analysis — design patterns, frameworks, system architecture
- Implementation Approaches — development methodologies, coding patterns
- Technology Stack — languages, frameworks, tools, platforms
- Integration Patterns — APIs, protocols, interoperability
- Performance Considerations — scalability, optimization, patterns

**Research Methodology:**

- Current web data with rigorous source verification
- Multi-source validation for critical technical claims
- Confidence level framework for uncertain information
- Comprehensive technical coverage with architecture-specific insights

**Scope Confirmed:** 2026-04-02

---

## Technology Stack Analysis

### Backend: ASP.NET Core Web API

**Recommended: ASP.NET Core 8 (LTS)**

ASP.NET Core 8 is the current LTS release (supported until November 2026) with .NET 9 available for non-LTS targets. For a production V1, .NET 8 LTS is the right choice — stability over cutting-edge features.

**Architecture pattern for RentEasy:**
- Minimal APIs or Controller-based REST API — both viable; Controllers give more structure for a multi-role app
- Multi-tenant data model from day one (landlord → properties → tenancies) as specified in the product brief
- Role-based authorization via ASP.NET Core's built-in `[Authorize(Roles = "Landlord,Tenant")]` and policy-based auth for finer-grained rules
- Cookie authentication for the web app (simpler, built-in CSRF protection, no token refresh complexity for a pure web app)

_Source: [ASP.NET Core 8 LTS Docs](https://learn.microsoft.com/en-us/aspnet/core/)_

---

### Frontend: React + Next.js vs Angular

**Recommended: React + Next.js 15 + next-intl**

Both React and Angular have full official support in ASP.NET Core project templates (Visual Studio 2022 17.11+, both use Vite). However, for RentEasy's specific requirements, React + Next.js pulls ahead:

**Why React + Next.js wins for RentEasy:**

| Factor | React + Next.js | Angular |
|---|---|---|
| Mobile-first bundle | ~45 KB base + code splitting | ~140 KB base |
| i18n developer experience | next-intl: type-safe, single build, runtime switching | Built-in requires separate build per locale; use Transloco instead |
| SSR / streaming | Next.js PPR + RSC streaming — strong for mobile first-load | Angular Universal improved but behind Next.js |
| Solo dev maintenance | Flexible, choose your own state management | Opinionated — fewer decisions |
| .NET mental model | Less aligned | More aligned (DI, decorators, modules mirror .NET) |

**Angular remains a valid choice** if you prefer the opinionated "batteries included" model and are comfortable with Transloco for i18n. Angular's Signals + Zoneless Change Detection (stable in Angular 18–20) have genuinely modernized the framework — it is not a dead-end choice.

**i18n recommendation:** If React → **next-intl** (type-safe, single build artifact, ICU plurals). If Angular → **Transloco** (not ngx-translate, which has stalled).

**Stack Overflow 2025:** React 46.9% vs Angular 19.8% professional dev usage. Angular job postings grew 47% YoY in 2025.

_Sources: [React vs Angular 2026 — Pagepro](https://pagepro.co/blog/react-vs-angular-comparison/), [Microsoft Learn — React with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa/react), [next-intl GitHub](https://github.com/amannn/next-intl)_

---

### Database: PostgreSQL via Neon

**Recommended: Neon (serverless PostgreSQL) — Free tier**

Neon is the strongest free-tier PostgreSQL option for a .NET project. Following Databricks' ~$1B acquisition (May 2025), the free tier was expanded significantly.

**Neon Free Tier Limits (2026):**

| Resource | Limit |
|---|---|
| Storage | 0.5 GB per project |
| Compute | 100 CU-hours/month (doubled in Oct 2025) |
| Projects | 100 |
| Connections | 10,000 (pooled via PgBouncer) |
| Egress | 5 GB/month |
| Auto-suspend | After 5 min inactivity (scale-to-zero) |
| Backups (PITR) | Up to 6 hours |
| Branching | Unlimited (instant copy-on-write) |

**100 CU-hours at 0.25 CU = ~400 hours continuous compute/month** — ample for a low-traffic app.

**Neon + .NET / Entity Framework Core:**
- Official NuGet packages: `Npgsql.EntityFrameworkCore.PostgreSQL` (v10.0.1)
- Official Neon guides for ASP.NET Core + EF Core exist at `neon.com/docs/guides/dotnet-entity-framework`
- **Critical:** Use the **direct (non-pooled)** connection string for EF Core migrations; use the **pooled** connection string for runtime queries. Running migrations through PgBouncer causes failures.
- Set `Max Auto Prepare = 0` on the pooled endpoint to avoid prepared statement conflicts with PgBouncer.
- SSL required: include `SSL Mode=Require;Trust Server Certificate=true` in connection strings.

**Known issue:** Cold start latency of ~500ms–2s on first query after scale-to-zero. Cannot disable on free tier. Mitigated by PgBouncer keeping a warm connection.

**Neon vs. Supabase PostgreSQL:**

| | Neon Free | Supabase Free |
|---|---|---|
| Storage | 0.5 GB | 500 MB (similar) |
| Inactivity behavior | Compute suspends; data persists | Entire project paused after **7 days** of inactivity |
| Branching | Unlimited CoW | Migration-based, not CoW |
| Best for | Pure PostgreSQL + .NET | Full BaaS (auth, storage, realtime) |

Supabase's 7-day full project pause is a significant risk for a low-traffic early-stage app. Neon's compute-only suspend (data always persists) is more reliable.

_Sources: [Neon Plans & Pricing](https://neon.com/docs/introduction/plans), [EF Core + Neon Docs](https://neon.com/docs/guides/dotnet-entity-framework), [Neon vs Supabase — SQLFlash](https://sqlflash.ai/article/20251011_neon_vs_supabase/)_

---

### File Storage: Cloudflare R2

**Recommended: Cloudflare R2 — Free tier**

R2 is the strongest free-tier object storage option for RentEasy's use case (utility bill PDFs, condition report photos, receipt PDFs — ~50–100 files/month).

**Cloudflare R2 Free Tier (permanent, no expiry):**

| Resource | Limit |
|---|---|
| Storage | 10 GB/month |
| Egress (downloads) | Free / unlimited |
| Class A ops (writes) | 1 million/month |
| Class B ops (reads) | 10 million/month |

**Zero egress cost** is R2's headline differentiator versus Azure Blob Storage and AWS S3.

**.NET Usage:**
R2 is S3-compatible. Use the **AWS SDK for .NET** (`AWSSDK.S3` NuGet) with two required config overrides:
- `DisablePayloadSigning = true`
- `DisableDefaultChecksumValidation = true`

This is documented in Cloudflare's official .NET example. Community SDK `Cloudflare.NET` also exists for native DI support.

**Comparison:**

| | Cloudflare R2 | Supabase Storage | Azure Blob |
|---|---|---|---|
| Free storage | 10 GB (permanent) | 1 GB (permanent) | 5 GB (12 months only) |
| Egress | Free forever | 5 GB/month | $0.087/GB |
| .NET SDK | AWS SDK workaround | Official C# SDK | Official Azure SDK |
| Best fit | RentEasy (best free) | If using Supabase stack | Azure-first shops |

Azure Blob's free tier expires after 12 months — it is not a long-term free option.

_Sources: [Cloudflare R2 Pricing](https://developers.cloudflare.com/r2/pricing/), [R2 AWS SDK .NET Example](https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/)_

---

### PDF Generation: QuestPDF

**Recommended: QuestPDF — Free for revenue under $1M/year**

QuestPDF is the strongest .NET PDF generation library for RentEasy's use case (condition reports with photos, payment receipts).

**QuestPDF Licensing (2026):**
- **Community (MIT):** Free for organizations with annual gross revenue **under $1M USD**, open-source projects, charities, and evaluation
- Professional / Enterprise: Required above $1M revenue
- All tiers have identical features — no feature gating

**Why QuestPDF for RentEasy:**
- Code-first fluent C# API — no HTML templates required
- Excellent image support: JPEG, PNG, WebP, SVG with FitWidth/FitHeight/FitArea
- Auto-pagination, flexible tables/grids, container-based layout
- 2025.1 added `Lazy` element for memory-efficient documents with many photos
- Zero infrastructure overhead — pure .NET, runs on Azure App Service without Docker
- Active development: v2026.2.4 on NuGet as of April 2026

**Alternatives ruled out:**

| Library | Verdict |
|---|---|
| iTextSharp (v5) | **Abandoned** — do not use. No maintenance, was LGPL. |
| iText 7 | ~$10K–45K/year commercial license. AGPL otherwise (must open-source your app). |
| PuppeteerSharp | Free (MIT), HTML-to-PDF via headless Chrome. **Requires Docker** — unreliable on Azure App Service Linux without containerization. |
| WkHtmlToPdf / DinkToPdf | **Security critical** — CVSS 9.8 SSRF (CVE-2022-35583), abandoned since 2020. Hard no. |
| Aspose.PDF | ~$1,175/dev/year. Enterprise only. |
| IronPDF | $799–$2,399+. HTML-to-PDF with managed Chromium, but overkill and expensive. |

_Sources: [QuestPDF License](https://www.questpdf.com/license/), [NuGet QuestPDF](https://www.nuget.org/packages/QuestPDF), [iTextSharp abandoned — Medium 2026](https://medium.com/@win_98496/itextsharp-is-abandoned-what-every-net-developer-needs-to-know-in-2025-99fbfcdb7c84)_

---

### Email: Resend

**Recommended: Resend — Free tier (3,000 emails/month)**

Resend is the right choice for RentEasy V1 email notifications.

**Resend Free Tier:**
- 3,000 emails/month, 1 custom domain
- No credit card required
- Paid plans from ~$20/month for 50K emails

**RentEasy email volume estimate:** ~10 tenants × ~15 notifications/month = ~150 emails/month. Well within the free tier.

**Resend + .NET:**
- **Official .NET SDK:** `resend-dotnet` on NuGet (repo: [github.com/resend/resend-dotnet](https://github.com/resend/resend-dotnet))
- Standard DI integration via `IResend` interface
- **PDF attachment support confirmed** — pass base64 bytes with filename in `Attachments` property on `EmailMessage`
- [TextControl Jan 2025 blog post](https://www.textcontrol.com/blog/2025/01/09/create-and-send-pdf-documents-as-email-attachments-with-net-c-sharp/) specifically covers .NET + Resend + PDF attachments

**EU / Bulgaria sending:**
- Resend offers `eu-west-1` (Ireland) as sending region — good for Bulgarian inbox placement
- **GDPR caveat:** Account data, metadata, and logs are stored in the **US** (no EU data residency for Resend account data as of 2026). For logs containing personal data (email addresses), this is a potential GDPR consideration. Mitigate by keeping log retention minimal (1 day on free tier) and noting this in the internal processing record.
- If strict EU data residency is required: **Brevo** (EU-based company) or **Mailgun** (EU-hosted infrastructure) are alternatives.

_Sources: [Resend Pricing](https://resend.com/docs/knowledge-base/account-quotas-and-limits), [Resend .NET SDK](https://github.com/resend/resend-dotnet), [Resend EU Region Docs](https://resend.com/docs/dashboard/domains/regions)_

---

### Authentication: ASP.NET Core Identity

**Recommended: ASP.NET Core Identity (self-hosted)**

For RentEasy's two-role model (Landlord + Tenant), ASP.NET Core Identity is the zero-cost, full-control path.

**Why self-hosted Identity for RentEasy:**
- Already included in the .NET ecosystem — no additional service or cost
- Role-based authorization via `[Authorize(Roles = "Landlord")]` or `[Authorize(Roles = "Tenant")]`
- Cookie authentication is appropriate for a server-rendered or SPA web app (no CSRF complexity when using Next.js with API routes)
- Credentials-based login matches the spec exactly (landlord creates tenant account, tenant receives credentials)
- Data stored in your own Neon PostgreSQL — no third-party data residency concerns
- Full control over tenant account lifecycle (read-only post-move-out, 12-month expiry)

**Auth pattern for RentEasy:**
- Two roles: `Landlord`, `Tenant`
- Landlord creates tenant accounts (no self-registration for tenants in V1)
- Cookie auth for the web app, with potential for JWT Bearer added later if a mobile app is built
- Tenant account state: `Active` → `ReadOnly` (post move-out) → `Expired` (12 months)

**Managed auth alternatives (if self-hosting feels like too much setup):**

| Service | Free MAU | .NET SDK | Notes |
|---|---|---|---|
| Clerk | 10,000 | Official (Jan 2025) | Best DX, `AddClerkAuthentication()` |
| Auth0 | 25,000 (B2C) | Official | Steeper price past free tier ($0.07/MAU) |
| Supabase Auth | 50,000 | Community only | Most generous free tier; DIY role claims |

**Verdict:** ASP.NET Core Identity is sufficient and recommended. The only reason to use a managed service would be if you want social login (Google, Facebook) — which is not in scope for V1.

_Sources: [ASP.NET Core Role-Based Auth](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles), [Clerk .NET SDK](https://github.com/clerk/clerk-sdk-csharp), [Auth vs Clerk vs Supabase — DesignRevision](https://designrevision.com/blog/auth-providers-compared)_

---

### Hosting & Deployment

**Recommended: Azure App Service B1 (~$13/month) for API + Vercel Free for frontend**

**Backend — Azure App Service B1:**

| Platform | Free? | RAM | Cold Start | .NET Native | Est. Cost |
|---|---|---|---|---|---|
| Azure F1 | Yes | 1 GB shared | ~20 min idle | Yes | $0 |
| **Azure B1** | No | **1.75 GB dedicated** | **None (Always On)** | Yes | **~$13/mo** |
| Railway Hobby | No | Up to 8 GB | 1–5 sec (if serverless on) | Via Docker | $5/mo |
| Fly.io | No | 512 MB | ~300ms (scale-to-zero) | Via Docker | ~$3–4/mo |
| Render | Partial (750 hrs) | 512 MB | **15–50 sec mandatory** | Via Docker | $0 / $7/mo |

**Why Azure B1 for RentEasy:**
- **No cold starts** — Always On available from B1 and above. Critical for a landlord/tenant app where delayed first responses break trust.
- **No Docker required** — deploy directly from Visual Studio or GitHub Actions with the official Azure App Service .NET deployment pipeline
- **1.75 GB RAM** — sufficient for ASP.NET Core 8 at low traffic (idle ~80–120 MB)
- **Your existing Azure familiarity** — Azure portal, App Service, deployment slots at Standard+
- Azure F1 (free) is tempting but the 60 CPU-minutes/day quota is easily burned and the mandatory sleep makes it unsuitable for real users

**Note on free alternatives:**
- **Fly.io at ~$3–4/mo** is the best budget alternative if $13/mo is too much — fast cold starts (~300ms on scale-to-zero), Docker-based
- **Render free** is effectively unusable for a real API (30–50 second mandatory cold starts)

**Frontend — Vercel Free:**
- Next.js is made by Vercel — zero-config deployment, instant CDN, automatic preview deployments on PRs
- Free tier: 100 GB bandwidth/month, unlimited deployments
- For a low-traffic V1, Vercel free is permanent (not time-limited)
- Alternative: **Azure Static Web Apps** (free tier) if you want everything in Azure

**Estimated monthly cost at launch:**
| Service | Cost |
|---|---|
| Azure App Service B1 | ~$13/mo |
| Neon PostgreSQL | $0 (free tier) |
| Cloudflare R2 | $0 (free tier) |
| Resend email | $0 (free tier) |
| Vercel (frontend) | $0 (free tier) |
| **Total** | **~$13/month** |

_Sources: [Azure App Service Pricing](https://azure.microsoft.com/en-us/pricing/details/app-service/windows/), [Fly.io Resource Pricing](https://fly.io/docs/about/pricing/), [Render Free Tier Docs](https://render.com/docs/free)_

---

## Technology Adoption Trends

**ASP.NET Core:** Consistently top 3 in Stack Overflow's most-loved frameworks. .NET 8 LTS with native AOT, minimal APIs, and first-class OpenAPI support positions it well through 2026.

**React + Next.js:** React remains the dominant frontend framework (46.9% professional dev usage). Next.js 15 with Partial Prerendering and React Server Components is the de facto React meta-framework. The React Compiler (stable 2025) reduces manual memoization.

**Serverless PostgreSQL:** Neon's acquisition by Databricks signals a long-term commitment. The serverless/scale-to-zero model is the direction the industry is moving for small/medium applications.

**Cloudflare R2:** Zero-egress pricing is pressuring AWS S3 and Azure Blob on the cost dimension. Growing developer adoption for file storage in indie/startup projects.

**QuestPDF:** Has become the community default for .NET PDF generation following iTextSharp's abandonment. 2025–2026 releases have focused on performance with large photo-heavy documents.

**Resend:** Part of a wave of developer-focused transactional email services (alongside Postmark, Brevo) that have displaced Mailchimp/SendGrid for technical builders. Growing fast; official .NET SDK shipped 2024.

---

## Summary Recommendation

### Recommended Stack for RentEasy V1

| Layer | Technology | Cost | Rationale |
|---|---|---|---|
| **Backend** | ASP.NET Core 8 Web API | — | Your core skill; LTS stability |
| **Frontend** | React + Next.js 15 | — | Mobile-first bundle, next-intl i18n, strong SSR |
| **i18n** | next-intl | Free | Type-safe, single build, BG+EN |
| **Database** | Neon (PostgreSQL) | Free | Best free-tier Postgres, EF Core + Npgsql supported |
| **File Storage** | Azure Blob Storage | ~$0/12mo then ~$0.15/mo | Official Azure SDK, familiar tooling, ~$0.10–0.15/mo after free period |
| **PDF Generation** | QuestPDF | Free (<$1M) | Best .NET PDF lib, photo support, zero infra |
| **Email** | Resend | Free (3K/mo) | Official .NET SDK, PDF attachments, EU region |
| **Auth** | ASP.NET Core Identity | Free | Full control, fits 2-role model exactly |
| **Backend Hosting** | Azure App Service F1 → B1 | $0 → ~$13/mo | F1 free at launch; upgrade to B1 when domain is purchased |
| **Frontend Hosting** | Azure Static Web Apps (Free) | Free | Permanent free tier, CDN, GitHub Actions deploy |
| **Total** | | **$0/month at launch → ~$13/month when domain is purchased** | |

### Key Decisions This Research Resolves

1. **Frontend:** React + Next.js over Angular — mobile-first bundle and next-intl i18n DX are the deciding factors for this specific project
2. **Database:** Neon over Supabase — Supabase's 7-day project pause is a real risk; Neon compute-only suspension is safer
3. **Storage:** Cloudflare R2 over Azure Blob — Azure free tier is time-limited; R2 is permanently free with zero egress
4. **PDF:** QuestPDF over all others — iTextSharp is abandoned, WkHtmlToPdf has critical CVEs, PuppeteerSharp requires Docker
5. **Auth:** ASP.NET Core Identity over managed services — no external cost or data residency concern; matches spec exactly
6. **Hosting:** Azure B1 over free tiers — Always On eliminates cold starts; $13/mo is the right tradeoff for real users
7. **Email:** Resend — 3K free emails/month far exceeds V1 needs; official .NET SDK confirmed working with PDF attachments

### Open Architecture Questions (for PRD/Architecture phase)

- **Frontend architecture decision:** Confirm React + Next.js (or Angular). This determines project scaffolding.
- **Deployment topology:** Single Azure App Service for API + Vercel for frontend (recommended) vs. unified hosting
- **Resend GDPR note:** Log retention set to 1 day (free tier default). Document in internal processing record that Resend metadata is US-stored. Include in tenant privacy notice if email logs contain personal data.
- **Neon migration workflow:** Confirm two connection strings in `appsettings.json` (pooled for runtime, direct for migrations) before first database setup.

---

---

## Integration Patterns Analysis

### API Design: Controllers vs Minimal APIs

**Recommended: Controller-based APIs for RentEasy**

The 2025 community consensus: both are production-ready, but the choice is a governance question, not a performance one. Performance differences are negligible in real workloads dominated by DB and network I/O.

**Why Controllers for RentEasy:**
- `[ApiController]` conventions, automatic model validation, filter pipelines built in
- Multi-tenant SaaS with RBAC benefits from consistent cross-cutting policies (auth, logging, error handling)
- Controllers reduce sprawl as the codebase grows — clear separation by resource
- Minimal APIs require intentional modular file structure from day one to avoid `Program.cs` becoming unmanageable

Minimal APIs are viable if you commit to a strict file-per-feature structure, but Controllers are the pragmatic default for a medium-complexity app.

_Source: [Microsoft Learn — Choose between APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis), [BoldSign — Controllers vs Minimal API .NET 8](https://boldsign.com/blogs/controllers-vs-minimal-api-dotnet-8/)_

---

### Multi-Tenant Data Isolation: EF Core Global Query Filters

**Recommended: Single DB + tenant discriminator column + `HasQueryFilter`**

RentEasy's data model is naturally multi-tenant (landlord → properties → tenancies). The canonical EF Core approach:

```csharp
// Scoped tenant service reads LandlordId from claims
public class AppDbContext : DbContext
{
    private readonly string _tenantId;
    public AppDbContext(DbContextOptions opts, ITenantService tenantService)
        : base(opts) => _tenantId = tenantService.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>()
            .HasQueryFilter(p => p.LandlordId == _tenantId);
        modelBuilder.Entity<Tenancy>()
            .HasQueryFilter(t => t.LandlordId == _tenantId);
        // Apply to all tenant-scoped entities
    }
}
```

**Critical implementation rules:**
- Register `DbContext` as **Scoped** (not Singleton) — the tenant ID is per-request
- **Index all `TenantId`/`LandlordId` columns** — the filter adds a `WHERE` clause to every query; missing indexes = full table scans
- `FromSqlRaw` / `ExecuteSqlRaw` bypass global filters — treat these as admin-only operations
- Use `IgnoreQueryFilters()` sparingly and only in explicitly admin-scoped services
- EF Core 10 introduces named filters — useful if combining tenant isolation with soft-delete filters

_Source: [Microsoft Learn — EF Core Multi-tenancy](https://learn.microsoft.com/en-us/ef/core/miscellaneous/multitenancy), [Milan Jovanovic — Multi-Tenant with EF Core](https://www.milanjovanovic.tech/blog/multi-tenant-applications-with-ef-core)_

---

### Next.js ↔ ASP.NET Core Integration

**Deployment topology (pre-custom-domain — Azure default domains):**
- Frontend: `*.azurestaticapps.net` (Azure Static Web Apps)
- Backend: `*.azurewebsites.net` (Azure App Service)

These are completely separate root domains — cookie sharing via `Domain=".renteasy.bg"` is not possible. **Third-party cookie restrictions** in modern browsers also make `SameSite=None` cookies across unrelated domains increasingly unreliable.

**Auth pattern for separate Azure domains: JWT Bearer tokens via BFF**

The recommended approach is a **Next.js BFF (Backend For Frontend)** layer:
1. User submits login form → Next.js Route Handler calls ASP.NET Core `/auth/login`
2. ASP.NET Core validates credentials, returns a JWT
3. Next.js Route Handler stores the JWT in an **HttpOnly cookie on the Static Web Apps domain** (same-origin to the browser — first-party cookie, no third-party issues)
4. All subsequent API calls from Next.js (Server Components or Route Handlers) read the HttpOnly cookie server-side and forward the JWT as an `Authorization: Bearer {token}` header to the ASP.NET Core API

The browser only ever talks to the Next.js domain. The ASP.NET Core API receives JWT Bearer tokens from the Next.js server — it never issues cookies directly to the browser.

**ASP.NET Core: JWT Bearer auth (instead of cookie auth):**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

**CORS configuration — still needed for any direct browser calls:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsFrontend", policy =>
    {
        policy.WithOrigins("https://{your-app}.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
        // No AllowCredentials() needed — JWT in Authorization header, not cookies
    });
});
```

**Calling the API from Next.js:**

| Caller | Pattern |
|---|---|
| Server Components / Server Actions | Read HttpOnly cookie server-side → attach as `Authorization: Bearer` header to ASP.NET Core fetch |
| Client Components | Call Next.js Route Handler (BFF) — never call ASP.NET Core directly from browser |
| Next.js Middleware | Read HttpOnly cookie → validate JWT (or call `/auth/validate`) → redirect to login if invalid |

**When `renteasy.bg` is purchased:** Switch to subdomain topology (`renteasy.bg` + `api.renteasy.bg`). Update CORS origin and optionally switch back to cookie auth with `SameSite=Lax` (same-site becomes much simpler). No architectural change required — just config updates.

**Note on CORS origin:** Drive `WithOrigins()` from an environment variable (`FRONTEND_ORIGIN`) so you can update it when the domain changes without a code deployment.

_Sources: [Microsoft Learn — CORS in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors), [Next.js — BFF Guide](https://nextjs.org/docs/app/guides/backend-for-frontend), [Next.js — Authentication Guide](https://nextjs.org/docs/app/guides/authentication)_

---

### Azure Blob Storage Integration

**Auth pattern: Managed Identity (production) + DefaultAzureCredential**

Microsoft's unambiguous recommendation — no secrets in code, auto-rotating tokens, works identically in dev and production:

```csharp
// Program.cs — register once, inject everywhere
builder.Services.AddSingleton(x =>
    new BlobServiceClient(
        new Uri($"https://{storageAccountName}.blob.core.windows.net"),
        new DefaultAzureCredential()));
```

Azure App Service setup: Enable System Assigned Managed Identity → assign **Storage Blob Data Contributor** role on the storage account.

**Container naming for multi-tenant isolation:**
```
Container: tenant-{landlordId}         (e.g., tenant-abc123)
Blob path:  {year}/{month}/{filename}  (e.g., 2025/04/invoice-001.pdf)
```

Per-tenant containers allow container-scoped SAS tokens that naturally restrict a tenant to their own files.

**Upload pattern:**
```csharp
var blobClient = containerClient.GetBlobClient($"{year}/{month}/{Guid.NewGuid()}-{originalName}");
var uploadOptions = new BlobUploadOptions
{
    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
};
await blobClient.UploadAsync(file.OpenReadStream(), uploadOptions);
```

**Tenant file access — short-lived SAS URLs (15–60 minutes):**
```csharp
// User Delegation SAS (preferred — works with Managed Identity)
var delegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
    DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

var sasBuilder = new BlobSasBuilder
{
    BlobContainerName = containerName,
    BlobName = blobName,
    Resource = "b",
    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-15),  // clock skew buffer
    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
};
sasBuilder.SetPermissions(BlobSasPermissions.Read);
var sasUri = new BlobUriBuilder(blobClient.Uri)
    { Sas = sasBuilder.ToSasQueryParameters(delegationKey, accountName) }.ToUri();
```

Tenants never get direct access to blob containers — always request a fresh SAS URL through your API, which enforces authorization first.

_Sources: [Azure Blob Upload .NET — Microsoft Learn](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload), [SAS Overview — Microsoft Learn](https://learn.microsoft.com/en-us/azure/storage/common/storage-sas-overview), [Multitenancy and Azure Storage — Azure Architecture Center](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/service/storage)_

---

### File Upload Endpoints

**Pattern: `IFormFile` for small files; streaming (`MultipartReader`) for PDFs and photos**

| Approach | Threshold | Use Case |
|---|---|---|
| `IFormFile` (buffered) | Under ~5–10 MB | Small thumbnails, profile images |
| `MultipartReader` (streaming) | PDFs, condition report photos | Avoids memory exhaustion under concurrent load |

**Security checklist — apply to all upload endpoints:**
1. Never use `IFormFile.FileName` for blob naming — always generate with `Guid.NewGuid()`
2. Validate file extension AND magic bytes (file signature), not just `Content-Type` header
3. Apply `[RequestSizeLimit]` per endpoint
4. Store files in Azure Blob only — never the web root
5. Raise `MaxRequestBodySize` explicitly for endpoints accepting large files

---

### OpenAPI / Swagger

**On .NET 8: Swashbuckle 6.x (current default)**

Swashbuckle was removed from .NET 9+ project templates — the community maintainer did not publish a .NET 8 release ([GitHub issue #54599](https://github.com/dotnet/aspnetcore/issues/54599)). .NET 9 introduced native `Microsoft.AspNetCore.OpenApi` + **Scalar** as the UI replacement.

**Recommendation for RentEasy:**
- Stay on .NET 8 LTS with Swashbuckle 6.x for V1
- When upgrading to .NET 9+: switch to `Microsoft.AspNetCore.OpenApi` + `Scalar.AspNetCore` (better UX, multi-language code samples, actively maintained)

_Source: [C# Corner — Why Scalar is Replacing Swagger in .NET 9/10](https://www.c-sharpcorner.com/article/why-scalar-is-replacing-swagger-in-net-9-and-10/)_

---

### Email Integration: Resend from ASP.NET Core

Standard DI pattern:
```csharp
// Program.cs
builder.Services.AddResend(options => 
    options.ApiToken = builder.Configuration["Resend:ApiKey"]);

// In a service
public class EmailService(IResend resend)
{
    public async Task SendReceiptAsync(string to, byte[] pdfBytes)
    {
        var message = new EmailMessage
        {
            From = "noreply@renteasy.bg",
            To = [to],
            Subject = "Your payment receipt",
            HtmlBody = "<p>Please find your receipt attached.</p>",
            Attachments = [new Attachment
            {
                Filename = "receipt.pdf",
                Content = Convert.ToBase64String(pdfBytes)
            }]
        };
        await resend.EmailSendAsync(message);
    }
}
```

_Source: [Resend .NET SDK](https://github.com/resend/resend-dotnet), [TextControl — PDF Attachments with Resend + C# (Jan 2025)](https://www.textcontrol.com/blog/2025/01/09/create-and-send-pdf-documents-as-email-attachments-with-net-c-sharp/)_

---

---

## Architectural Patterns and Design

### System Architecture: Modular Monolith

**Recommended: Modular monolith for RentEasy V1**

RentEasy is a small, well-scoped domain with a solo developer. Microservices would introduce unnecessary operational complexity. A **modular monolith** — single deployable unit with clear internal module boundaries — is the right architecture for V1 and scales to V2 without a rewrite.

Modules aligned to the domain:
- `Properties` — property and tenancy management
- `Billing` — bill uploads, payment tracking, receipts
- `ConditionReports` — move-in/move-out reports, dispute flow
- `Maintenance` — request tracking and status
- `Notifications` — email dispatch (Resend integration)
- `Storage` — Azure Blob wrapper
- `Identity` — ASP.NET Core Identity, JWT issuance

Each module owns its own EF Core entities, service interfaces, and Application layer handlers. Infrastructure is shared (one DbContext, one blob client, one email client).

---

### Project Structure: Clean Architecture (4-project solution)

**Recommended: Clean Architecture with feature-slice organization inside Application layer**

The 2025 community consensus (ardalis + Jason Taylor templates, now on .NET 8/9/10) is Clean Architecture as the outer scaffold with Vertical Slice organization inside the Application layer:

```
RentEasy.sln
├── src/
│   ├── RentEasy.Domain          ← Entities, value objects, domain events (zero external deps)
│   ├── RentEasy.Application     ← Use cases organized by feature
│   │   └── Features/
│   │       ├── Billing/
│   │       │   ├── UploadBill/
│   │       │   ├── ConfirmPayment/
│   │       │   └── GetBillingHistory/
│   │       ├── ConditionReports/
│   │       ├── Maintenance/
│   │       └── Properties/
│   ├── RentEasy.Infrastructure  ← EF Core DbContext + migrations, Blob Storage, Resend, QuestPDF
│   └── RentEasy.API             ← Controllers, DI registration, middleware
└── tests/
    ├── RentEasy.UnitTests
    └── RentEasy.IntegrationTests
```

**Dependency rules (enforced via project references):**
- `Domain` → no dependencies
- `Application` → `Domain` only
- `Infrastructure` → `Application` + `Domain`
- `API` → `Application` + `Infrastructure` (Infrastructure for DI wiring only)

_Sources: [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture), [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)_

---

### CQRS and MediatR

**Recommended: CQRS pattern without MediatR library**

CQRS as a pattern (separate command and query objects) is still well-suited for RentEasy's medium complexity. However:

**MediatR went commercial in late 2024/early 2025** — Jimmy Bogard moved it to a paid commercial license for commercial use. This triggered a significant community re-evaluation.

**Free alternatives:**
- **Wolverine** — most feature-rich free alternative, MIT licensed, also handles messaging
- **LiteBus** — lightweight, MIT, CQS-focused
- **Roll your own** — a minimal mediator is ~30 lines of C#; many teams prefer this for small-medium apps

**For RentEasy:** Given the small team (solo) and bounded scope, injecting handlers directly via DI without a mediator library is the cleanest approach. CQRS by convention (separate `Command`/`Query` classes, separate `Handler` classes) without the MediatR pipeline overhead.

_Sources: [You Don't Need MediatR — juliocasal.com](https://juliocasal.com/blog/you-don-t-need-mediatr), [Stop Conflating CQRS and MediatR — Milan Jovanovic](https://www.milanjovanovic.tech/blog/stop-conflating-cqrs-and-mediatr)_

---

### Repository Pattern

**Recommended: Use DbContext directly — no generic repository wrapper**

The generic repository pattern over EF Core is broadly considered an anti-pattern in 2025. `DbContext` is already a Unit of Work and `DbSet<T>` is already a repository. Wrapping it adds a leaky abstraction with no real benefit.

- Inject `AppDbContext` directly into handlers and services
- Use `ardalis/Specification` NuGet for complex, reusable query objects (e.g., "all unpaid bills for tenant X in the last 3 months")
- Use `DbContext.SaveChangesAsync()` natively — no Unit of Work wrapper needed
- `FromSqlRaw` with parameters if raw SQL is ever needed — never string concatenation

_Source: [The Repository Pattern is Dead — emonarafat.github.io](https://emonarafat.github.io/2025/04/21/the-repository-pattern-is-dead-ef-core-killed-it.html)_

---

### Security Architecture

**Two-layer authorization:** ASP.NET Core Identity role checks (coarse) + `IAuthorizationHandler` resource checks (fine-grained).

```csharp
// Coarse: role check on controller
[Authorize(Roles = "Landlord")]
public class PropertiesController : ControllerBase { }

// Fine-grained: resource-based check in handler
var authResult = await _authService.AuthorizeAsync(User, tenancy, "SameTenantPolicy");
if (!authResult.Succeeded) return Forbid();
```

**EF Core global query filters as second layer** — defense in depth so even if authorization is misconfigured, a tenant's query physically cannot return another tenant's data.

**JWT claim design:**
```json
{
  "sub": "user-uuid",
  "tenant_id": "landlord-uuid",
  "role": "Landlord | Tenant",
  "jti": "random-guid",
  "exp": 900
}
```

- Access token TTL: **15 minutes**
- Refresh token TTL: **7–30 days**, stored hashed in DB, returned via `HttpOnly` cookie, rotated on every use
- Never read `tenant_id` from request body — always from JWT claims

**Input validation:** DataAnnotations for DTO-level constraints + FluentValidation for business rules. EF Core parameterizes all LINQ queries automatically (SQL injection protection). Add `HtmlSanitizer` NuGet if any field stores rich text.

**Security headers (production):** `UseHttpsRedirection()` + `UseHsts()` on API. In `next.config.js` headers: `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: strict-origin-when-cross-origin`. Roll out CSP in `Content-Security-Policy-Report-Only` mode first.

_Sources: [Resource-Based Authorization — Code Maze](https://code-maze.com/aspnetcore-resource-based-authorization/), [Microsoft Learn — Enforce HTTPS](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl)_

---

### ⚠️ Azure Static Web Apps — Critical Architectural Constraint

**Key finding: Azure Static Web Apps hybrid mode (SSR/App Router) and the linked App Service backend feature are mutually exclusive.**

The linked backend proxy (`/api/*` → App Service) only works in **static export mode** (pure CSR/SSG). If Next.js runs in hybrid mode (SSR, Server Components, Route Handlers), the linked backend is not available — you must call the ASP.NET Core API directly across origins (CORS required).

| Deployment option | Next.js SSR | Linked App Service backend | CORS needed |
|---|---|---|---|
| SWA static export + linked backend | No (CSR/SSG only) | Yes — zero CORS config | No |
| SWA hybrid mode | Yes | No | Yes |
| App Service (Linux) for Next.js | Yes | N/A | Yes |

**Additional SWA concerns (2025):**
- Hybrid Next.js on SWA has been in preview since 2022 with limited progress
- Max bundle size: 250 MB (must use `output: 'standalone'`)
- Max function execution time: 45 seconds
- Community observation: SWA GitHub repo shows limited activity in 2025 — uncertain roadmap

**Decision required for RentEasy:**

RentEasy's showcase page benefits from SSR (SEO for a public-facing property page). The tenant portal is behind login so SSR is less critical there. This creates a real trade-off:

**Option A — SWA static export + linked App Service:**
- Next.js as pure SPA/SSG. Showcase page pre-rendered at build time (good enough for SEO).
- Zero CORS config. Clean `/api` proxy. Free tier for frontend.
- No Server Components, no Route Handlers, no BFF pattern — all API calls are client-side.

**Option B — SWA hybrid + CORS:**
- Full Next.js App Router SSR. Server Components. BFF pattern possible.
- Must configure CORS on the API. Uses a preview feature.
- Risk: uncertain SWA SSR roadmap.

**Option C — Azure App Service B1 for Next.js (additional ~$13/mo):**
- Full SSR, no restrictions, standard CORS. Total cost ~$26/month.
- Most flexible, no preview-feature risk.

_Source: [Next.js on Azure Static Web Apps — Microsoft Learn](https://learn.microsoft.com/en-us/azure/static-web-apps/nextjs), [API support in SWA — Microsoft Learn](https://learn.microsoft.com/en-us/azure/static-web-apps/apis-app-service)_

---

### CI/CD: GitHub Actions Monorepo

Two workflow files with `paths:` filters to prevent cross-triggering:

**SWA workflow** uses `Azure/static-web-apps-deploy@v1`:
- `app_location: "frontend"` (relative to repo root)
- `api_location: ""` (leave blank — using linked App Service backend)
- `output_location: ".next"` or `"out"` for static export
- Set `NEXT_PUBLIC_*` vars in the workflow `env:` block at build time

**App Service workflow** uses `azure/webapps-deploy@v3` with OIDC (preferred over publish profiles):
- `dotnet restore` → `dotnet build` → `dotnet publish` → deploy
- OIDC requires: Entra app registration + federated credential + `Website Contributor` role on the App Service

**⚠️ Gotcha:** If `api_location: ""` is in the SWA workflow, each pipeline run resets the linked backend. Use `az staticwebapp backend link` in the pipeline to re-assert the link after each deploy if needed.

**Configuration management across both services:**
- `NEXT_PUBLIC_*` vars → set in GitHub Actions `env:` block (baked into Next.js build by Oryx)
- Server-side Next.js vars → SWA application settings (Azure Portal or CLI)
- ASP.NET Core vars → App Service application settings (override `appsettings.json`)
- Shared secrets (e.g., JWT key) → Azure Key Vault, referenced from both via Key Vault references

_Source: [Build configuration for Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/build-configuration), [Deploy via GitHub Actions — App Service](https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions)_

---

---

## Implementation Approaches and Technology Adoption

### Local Development Setup

**Recommended: VS Code compound launch + `dotnet watch` + `next dev`**

Monorepo structure:
```
RentEasy/
├── backend/        ← ASP.NET Core solution
├── frontend/       ← Next.js app
└── .vscode/
    ├── tasks.json
    └── launch.json
```

Run both simultaneously with a VS Code compound launch configuration, or from any terminal using `concurrently`:
```bash
npx concurrently "dotnet watch run --project backend" "next dev --prefix frontend"
```

- `dotnet watch` provides hot reload without full restart for most code changes
- `next dev` provides Fast Refresh for instant frontend updates
- Add `http://localhost:3000` to CORS allowed origins in `appsettings.Development.json`
- Store Neon connection strings (both pooled and direct) in `dotnet user-secrets` — never in `appsettings.json`

_Source: [dotnet watch — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch)_

---

### Testing Strategy

**Framework: xUnit** — the .NET community default. Used in all official ASP.NET Core samples, templates, and documentation. Better test isolation than NUnit (`IDisposable`/`IAsyncLifetime` instead of `[SetUp]`/`[TearDown]`). Runs tests in parallel by default.

**Do not use the EF Core InMemory provider** — Microsoft's own docs explicitly discourage it. It lacks relational constraints, doesn't enforce foreign keys, and diverges from real database behavior.

**Two-tier testing approach:**

| Tier | Tool | When to use |
|---|---|---|
| Unit tests | SQLite in-memory (`Microsoft.Data.Sqlite`) | Testing handler/service logic that touches the DB; fast, no Docker |
| Integration tests | Testcontainers + PostgreSQL | Full HTTP stack tests; highest fidelity, matches Neon/prod behavior |

**SQLite in-memory (unit tests):**
```csharp
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection).Options;
using var context = new AppDbContext(options, mockTenantService);
context.Database.EnsureCreated();
// test logic here
```

**Testcontainers + Respawn (integration tests):**
```csharp
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services => {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(_db.GetConnectionString()));
        });

    public async Task InitializeAsync() => await _db.StartAsync();
    public new async Task DisposeAsync() => await _db.StopAsync();
}
```

Use **Respawn** (`Respawn` NuGet) to reset database state between tests — much faster than recreating the schema, respects FK order.

**Testing auth / multi-tenancy:**
```csharp
// Fake auth handler in ConfigureTestServices — swap claims per test scenario
services.AddAuthentication("Test")
    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
```

Test cross-tenant isolation explicitly: seed data for Tenant A and Tenant B, confirm Tenant A's token cannot access Tenant B's resources at the HTTP level.

**Authorization handler unit tests** — no mocking framework needed:
```csharp
var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
    new Claim("tenant_id", "landlord-abc"),
    new Claim(ClaimTypes.Role, "Landlord")
}, "TestAuth"));
var context = new AuthorizationHandlerContext(
    new[] { new SameTenantRequirement() }, user, resource);
await handler.HandleAsync(context);
Assert.True(context.HasSucceeded);
```

_Sources: [Testcontainers Best Practices — Milan Jovanovic](https://www.milanjovanovic.tech/blog/testcontainers-best-practices-dotnet-integration-testing), [EF Core Testing Strategy — Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy)_

---

### EF Core Migrations with Neon

**Rule: always use the non-pooled (direct) connection string for migrations.** Neon's pooled string routes through PgBouncer (transaction-mode), which blocks the `SET` statements and advisory locks EF Core migrations require.

Store two connection strings in secrets:
- `ConnectionStrings__Default` — pooled (runtime queries)
- `ConnectionStrings__Migrations` — direct (migrations only, port 5432)

**CI/CD migration pattern (recommended: Migration Bundles):**
```yaml
# Build stage — generate self-contained bundle
- run: dotnet ef migrations bundle --output efbundle

# Deploy stage — run against Neon direct connection
- run: ./efbundle --connection "${{ secrets.NEON_DIRECT_CONNECTION }}"
```

Migration bundles are Microsoft's recommended CI/CD approach — no need to install `dotnet-ef` on the deployment agent.

**Never run `database update` in application startup code in production.** Always as an explicit pipeline step before the app deployment.

**Use Neon branches for staging migrations:** create a branch from main, test the migration, then run against the production branch once verified.

_Source: [EF Core Migrations with Neon — Neon Docs](https://neon.com/docs/guides/entity-migrations), [Migration Bundles — .NET Blog](https://devblogs.microsoft.com/dotnet/introducing-devops-friendly-ef-core-migration-bundles/)_

---

### Monitoring: Azure Application Insights

**Free tier: 5 GB ingestion/month per Log Analytics workspace** — shared across all resources in the workspace. Beyond that: ~$2.30–2.76/GB.

For RentEasy at launch (1-2 users, very low traffic): a small .NET API with adaptive sampling generates **~0.1–1 GB/month** — well within the free 5 GB.

**Setup:**
- Works with both App Service F1 and B1 — no plan restriction
- Add `Microsoft.ApplicationInsights.AspNetCore` NuGet to the API project
- Set the `APPLICATIONINSIGHTS_CONNECTION_STRING` in App Service application settings
- Enable **adaptive sampling** (on by default) to keep volume low
- Set a **daily ingestion cap** (e.g., 1 GB/day) as a safety net against accidental telemetry floods

**What to monitor for RentEasy:**
- Failed requests (4xx, 5xx)
- Dependency failures (Neon DB, Azure Blob, Resend)
- Exception telemetry
- Slow requests (PDF generation, file uploads)

_Source: [Azure Monitor Pricing — Microsoft](https://azure.microsoft.com/en-us/pricing/details/monitor/)_

---

### Development Workflow Summary

**Day-to-day:**
1. `dotnet user-secrets set` for all local secrets (Neon connections, Resend API key, JWT key, Blob connection)
2. `npx concurrently` or VS Code compound launch to run both backend and frontend
3. Feature branches → PRs → GitHub Actions runs tests on PR
4. Merge to `main` → GitHub Actions deploys backend to App Service + frontend to SWA automatically

**Before going live (domain purchased):**
1. Upgrade App Service F1 → B1 (enable Always On)
2. Add custom domain + SSL on both App Service and SWA
3. Update CORS origins from Azure default domains to `api.renteasy.bg` / `renteasy.bg`
4. Set up Azure Blob Storage lifecycle policies (move old files to Cool tier after 90 days)
5. Verify Application Insights daily cap is set

---

### Risk Register

| Risk | Likelihood | Mitigation |
|---|---|---|
| App Service F1 CPU quota hit | Low (low traffic at launch) | Monitor with App Insights; upgrade to B1 if needed |
| Neon cold start latency | Medium (scale-to-zero) | PgBouncer pooled connection masks most cold starts; acceptable for V1 |
| SWA static export limits Next.js SSR | Accepted | Showcase page pre-renders at build time; sufficient for SEO |
| Azure Blob free tier expires 12 months | Known | Budget ~$0.15/mo after year 1; negligible at this scale |
| Resend US data residency | Low (logs only, 1-day retention) | Document in internal GDPR processing record |
| MediatR commercial license | Avoided | Using CQRS by convention, direct handler injection — no MediatR dependency |
| iTextSharp abandoned | Avoided | Using QuestPDF |

---

_Research completed: 2026-04-02. All claims verified against current public sources. Sources cited inline per section._
