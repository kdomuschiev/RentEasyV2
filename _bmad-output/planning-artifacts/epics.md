---
stepsCompleted: ["step-01-validate-prerequisites", "step-02-design-epics", "step-03-create-stories", "step-04-final-validation"]
workflowStatus: complete
completedAt: "2026-04-07"
inputDocuments:
  - _bmad-output/planning-artifacts/prd.md
  - _bmad-output/planning-artifacts/architecture.md
  - _bmad-output/planning-artifacts/ux-design-specification.md
---

# RentEasyV2 - Epic Breakdown

## Overview

This document provides the complete epic and story breakdown for RentEasyV2, decomposing the requirements from the PRD, UX Design Specification, and Architecture Decision Document into implementable stories.

## Requirements Inventory

### Functional Requirements

**Authentication & Account Management**
- FR1: Landlord can log in with email and password
- FR2: Tenant can log in with email and password
- FR3: System prevents tenant from accessing any feature before changing their temporary password on first login
- FR4: Any authenticated user can change their password at any time from account settings
- FR4b: Any user can request a password reset from the login screen via email link, without requiring an active session
- FR5: System enforces role-based access — Landlord and Tenant roles have separate, non-overlapping capability sets

**Property Management**
- FR6: Landlord can create a property with name, address, size, floor, and applicable bill categories
- FR7: Landlord can configure payment methods per property: IBAN, IRIS Pay phone number, and Revolut.me link
- FR8: Landlord can create and edit the property welcome pack (apartment manual, utility provider contacts, building management contact, WiFi credentials, garbage collection schedule, emergency numbers)
- FR9: Landlord can view and edit any property detail at any time

**Tenant Management**
- FR10: Landlord can create a tenant account for a property by providing tenant name and email address
- FR11: System sends a tenant invitation email containing login credentials and the property welcome pack upon account creation
- FR12: Tenant can view the welcome pack for their property at any time after first login
- FR13: Landlord can trigger the move-out flow for an active tenancy
- FR14: System automatically restricts a former tenant account to read-only access to their own tenancy data immediately upon move-out
- FR15: System automatically revokes former tenant access 12 months after move-out with no manual action required

**Condition Reports**
- FR16: Landlord can pre-load condition report items (photos + notes per item) for a property before tenant first login
- FR17: Tenant can view landlord-pre-loaded condition report items as read-only content on first login
- FR18: Tenant can add their own condition report items (photos + notes) on top of the landlord baseline
- FR19: Tenant can agree to the condition report, triggering timestamped PDF generation with tenant identity explicitly recorded, and email delivery to both parties
- FR20: Tenant can disagree with the condition report and submit disputed items with photos and written descriptions
- FR21: Landlord can review disputed items and re-request tenant sign-off with or without accepting the disputed items
- FR22: System enforces a maximum of 3 dispute rounds; after 3 rounds without agreement, the dispute is recorded as unresolved with both parties' positions permanently documented
- FR23: System generates a condition report PDF containing the full dispute history — all rounds, all photos, all notes, all timestamps from both parties, and final status (agreed or unresolved)
- FR24: System stores condition report PDFs permanently and makes them accessible to both landlord and tenant
- FR25: Both landlord and tenant can see the current dispute round number, which party's action is pending, and what disputed items remain outstanding
- FR26: Landlord can complete a move-out condition report using the same shared condition report editor as move-in

**Bill Management & Payment Flow**
- FR27: Landlord can upload a utility bill PDF and enter a total amount per bill category for a billing period
- FR28: Tenant can view an itemised breakdown of all charges for the current billing period with each bill PDF accessible inline
- FR29: Tenant can open and download any attached bill PDF directly from the billing screen
- FR30: Tenant can view all available payment methods for their property (IBAN with one-tap clipboard copy, IRIS Pay QR code and phone number, Revolut.me QR code and link)
- FR31: Tenant can mark a payment as made, changing the billing status to "Payment pending confirmation"
- FR32: Landlord can confirm a tenant payment, recording the amount confirmed as received
- FR33: System generates a receipt PDF upon payment confirmation, bundling the total confirmed amount with all bill PDFs for that billing period, and emails it to the tenant
- FR34: Tenant can view their complete payment history across all past billing periods
- FR35: Tenant can download any historical receipt or bill PDF at any time during and after their tenancy (within the read-only access window)

**Maintenance Requests**
- FR36: Tenant can submit a maintenance request with a title, description, and one or more photos
- FR36b: System sends email to landlord when tenant submits a new maintenance request
- FR37: Landlord can view all maintenance requests for a property, each showing current status and submission details
- FR37b: Landlord can view the move-in condition report completion status for each active tenancy (not started / in progress / complete)
- FR38: Landlord can update the status of a maintenance request (received / in progress / resolved)
- FR39: System notifies tenant by email when the landlord updates the status of their maintenance request

**Notifications & Communications**
- FR40: System sends email to tenant when landlord uploads bills for a billing period
- FR41: System sends a payment due reminder email to tenant 3 days after bills are uploaded if no payment has been marked as made
- FR42: System sends email to landlord when tenant marks a payment as made
- FR43: System sends email to tenant with receipt PDF attached when landlord confirms a payment
- FR44: System sends email nudge to tenant at Day 3 and Day 7 if move-in condition report is not completed
- FR45: System sends email reminder to both tenant and landlord at Day 14 if move-in condition report is not completed
- FR46: If the move-in condition report is not completed by Day 14, the system auto-resolves it as incomplete, generates a PDF documenting all items recorded to that point, and emails it to both tenant and landlord. Tenant retains full platform access.
- FR48: System emails condition report PDFs to both landlord and tenant upon sign-off
- FR49: System emails the final move-out PDF bundle (all receipts + both condition report PDFs) to the former tenant upon move-out completion

**Data Management & GDPR**
- FR50: System generates a final PDF bundle containing all payment receipts and both condition report PDFs upon move-out
- FR51: Former tenant can view and download their own payment history and condition reports during the 12-month read-only access period
- FR52: Landlord can delete a tenant's profile data (name, email, phone, credentials) on request
- FR53: System retains all tenancy record data (condition reports, payment history, receipts, bill PDFs) regardless of profile deletion requests, per legal retention requirements

**Public Showcase Page**
- FR54: Any visitor can view the public apartment showcase page without authentication
- FR55: Visitor can toggle the showcase page between Bulgarian and English
- FR56: Visitor can submit their email address to join the apartment waitlist
- FR57: Showcase page displays the current waitlist subscriber count
- FR58: Visitor can submit their email via a landlord interest call-to-action (stored separately from the tenant waitlist)
- FR59: Visitor can access the tenant login page via a ghost login link on the showcase page

**Internationalisation**
- FR60: Authenticated users can toggle the application language between Bulgarian and English

### NonFunctional Requirements

**Performance**
- NFR-P1: All application screens load within 2 seconds on a standard Bulgarian mobile connection (LTE/4G, ~20 Mbps) at a 390px viewport
- NFR-P2: The public showcase page achieves First Contentful Paint within 2 seconds under the same connection baseline
- NFR-P3: Skeleton screens are displayed on any screen that requires a data fetch — no blank or white loading states
- NFR-P4: Bill PDFs and condition report PDFs are served as externally-hosted signed URLs — PDF content is never embedded in page payloads
- NFR-P5: QR code generation for IRIS Pay and Revolut.me payment methods completes client-side in under 100ms

**Security**
- NFR-S1: All data is transmitted over HTTPS; no HTTP endpoints in production
- NFR-S2: Database content is encrypted at rest
- NFR-S3: All sensitive resource identifiers use UUIDs — no sequential or guessable IDs on tenancies, bills, condition reports, or uploaded files
- NFR-S4: Every API endpoint enforces per-resource authorisation — authenticated users can only access resources they own; authentication alone is not sufficient
- NFR-S5: All file uploads are validated by MIME type and file signature; only JPEG, PNG, and PDF files are accepted; uploaded files are never executed
- NFR-S6: All user-generated text rendered in the UI is output-encoded to prevent XSS
- NFR-S7: All text input fields sanitize input to prevent SQL injection and script injection
- NFR-S8: Tenancy data is strictly scoped — a user can never access another user's data, including former tenants accessing current tenant data

**Reliability**
- NFR-R1: System uptime is ≥ 99.5% per calendar month, measured excluding scheduled maintenance windows
- NFR-R2: Transactional email delivery rate is ≥ 99% — no missed credential deliveries, payment confirmations, condition report PDFs, or legally significant documents
- NFR-R3: PDF generation succeeds 100% of the time for all triggered events; generation failures must be logged and retried
- NFR-R4: In-app document status (payment confirmed, condition report signed) serves as a secondary confirmation channel alongside email delivery

**Accessibility**
- NFR-A1: All interactive elements have a minimum tap target size of 44×44px
- NFR-A2: Colour contrast meets WCAG 2.1 AA minimums: 4.5:1 for normal text, 3:1 for large text
- NFR-A3: All form inputs have associated `<label>` elements; no placeholder-only labelling
- NFR-A4: Error states communicate meaning through text, not colour alone
- NFR-A5: All meaningful images carry descriptive `alt` text; decorative images use `alt=""`
- NFR-A6: The `<html>` element carries the correct `lang` attribute (`bg` or `en`) for the active language
- NFR-A7: The interface is fully navigable without hover interactions — all functionality is available via tap or click

**Scalability**
- NFR-SC1: The data model supports multiple landlords, multiple properties per landlord, and multiple tenancies per property from day one
- NFR-SC2: All infrastructure scaling upgrades require configuration changes only — no code or architectural changes

**Integration Reliability**
- NFR-I1: Transactional email delivery retries automatically on failure; critical documents confirmed via in-app status as secondary signal
- NFR-I2: Signed document URLs generated with expiry windows ≥ 4 hours; URLs regenerated on access if expired
- NFR-I3: PDF generation executes entirely server-side with no external service calls — no network failure mode

### Additional Requirements

*From Architecture — technical requirements with implementation impact:*

- Monorepo scaffold: `npx create-next-app@latest renteasy-web` (TypeScript, ESLint, Tailwind CSS, App Router, src/, Turbopack) + `dotnet new webapi --use-controllers --use-program-main -o renteasy-api` — **this is the first implementation story (Epic 1, Story 1)**
- Post-scaffold packages: `next-intl`, `qrcode` (frontend); `Npgsql.EntityFrameworkCore.PostgreSQL`, `Azure.Storage.Blobs`, `QuestPDF`, `resend-dotnet`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (backend)
- Database: Neon PostgreSQL with two connection strings in `appsettings.json` — pooled (runtime) and direct (migrations only); manual `dotnet ef database update` workflow
- Auth: ASP.NET Core Identity + JWT (7-day lifetime); `TokenValidFrom` datetime column on user table for instant revocation; validated via `TokenValidFromMiddleware` on every authenticated request; `AccountState` enum (Active, ReadOnly, Expired) baked into JWT claim and re-issued on state change
- Multi-tenancy: EF Core `HasQueryFilter` scoped to `LandlordId` on all landlord-scoped entities; `WaitlistEntry` is NOT landlord-scoped
- File storage: Azure Blob Storage; path structure `{tenancyId}/{category}/{uuid}.{ext}`; signed URLs: 1 hour for inline viewing, 24 hours for downloads; files never served from app server
- Background jobs: `IHostedService` polling every 5 minutes; `email_nudge_jobs` table tracks all jobs with idempotency; two jobs: `NudgeSchedulerJob` (Day 3/7/14 condition report + Day 3 payment) and `TenancyExpiryJob` (12-month auto-expiry)
- BFF auth pattern: Next.js Route Handlers at `/api/auth/*` set/clear HttpOnly JWT cookie; browser never calls ASP.NET Core API directly for auth
- i18n: URL prefix routing (`/bg/[route]`, `/en/[route]`) via next-intl; Bulgarian is default; translation files at `messages/bg.json` and `messages/en.json`
- API errors: RFC 7807 `ProblemDetails` on all error responses — never custom error envelopes
- CI/CD: GitHub Actions; two environments only (local development + production); `api-deploy.yml` → Azure App Service B1; `web-deploy.yml` → Azure Static Web Apps
- Naming: DB tables `snake_case` plural; C# entities `PascalCase` singular; API endpoints plural kebab-case; all resource IDs UUIDs; async methods suffixed `Async`; JSON fields `camelCase`; money as `decimal`; enums as strings; dates as ISO 8601

### UX Design Requirements

- UX-DR1: Implement the complete design token system — CSS variables for the full colour palette (`--color-bg: #F8F4EE`, `--color-surface: #FFFFFF`, `--color-primary: #4A6172`, `--color-accent: #C8952A`, `--color-oak: #C4955A`, `--color-text: #1E1E1E`, `--color-muted: #6B7280`, `--color-success: #3D7A5F`, `--color-pending: #B87A1A`, `--color-error: #C0392B`, `--color-stone: #3A3D42`) defined in a single Tailwind config applied to both showcase page and authenticated app
- UX-DR2: Implement typography system — Playfair Display (editorial serif with Cyrillic, showcase headings only) and Inter (UI font throughout, full Cyrillic); type scale from Display (48–72px/700) to Micro (12px/400); minimum body text 14px on all screens; line height 1.5 for body, 1.2 for display
- UX-DR3: Implement `BillLineItem` custom component — category label (left) + amount (right, bold) + optional PDF icon (far right); states: default, PDF-attached variant, no-PDF variant (Rent); accessible PDF link with `aria-label="Open [category] bill PDF"`; row not tappable to prevent accidental navigation
- UX-DR4: Implement `PaymentMethodPanel` custom component — three tabs (IBAN, IRIS Pay, Revolut) inside shadcn/ui Tabs; one-tap clipboard copy via `navigator.clipboard.writeText()`; inline label change to "Copied ✓" for 2 seconds then reverts (no toast, no modal); copy buttons with specific `aria-label` per method; QR codes with descriptive `alt` text
- UX-DR5: Implement `PaymentStatusBadge` custom component — three distinct visual states: Unpaid (`#B87A1A` on amber tint), Pending confirmation (same amber), Paid ✓ (`#3D7A5F` on green tint); always paired with text label, never colour-only
- UX-DR6: Implement `ConditionReportItem` custom component — contributor label ("Landlord" / "You"), photo thumbnails, description, timestamp; states: landlord item (read-only for tenant), tenant item (editable during active round); compact list view and expanded detail view variants
- UX-DR7: Implement `ConditionReportRoundBanner` custom component — round indicator ("Round X of 3"), actor label ("Your turn" / "Waiting for Kiril"); states: tenant's turn, landlord's turn, waiting, unresolved (calm, not error state); `role="status"` for screen reader announcements
- UX-DR8: Implement `SafetyIntroScreen` custom component — full-screen condition report intro; large `<h1>` headline ("This report protects you.") in landlord's voice; 2–3 body sentences; single primary "Start the report" button; full contrast on warm background; no auto-advance; shown on first login to condition report only
- UX-DR9: Implement `WelcomePackSection` custom component — section title + content (text, key-value pairs for WiFi/contacts, or list); variants: text section, key-value section, list section; copy buttons with `aria-label`; collapsed sections use `aria-expanded`
- UX-DR10: Implement `FileUploadArea` custom component — dashed-border drop zone, instruction text, "Choose file" button, attached file preview + remove button; states: default, drag-over (desktop), file attached, upload error (inline, never modal); variants: photo (JPEG/PNG, multiple files) and document (PDF, single file); `accept` attribute on input for client-side filtering; server-side MIME + magic bytes validation
- UX-DR11: Implement `PendingActionBanner` custom component — persistent non-dismissible condition report reminder; day indicator ("Day 7 of 14") + "Complete now" action button; states: Day 3–6 (soft tone), Day 7–13 (firmer), Day 14 (soft-block, replaces normal content); `role="alert"` on Day 14 version
- UX-DR12: Implement `QRCodeDisplay` custom component — client-side rendering via `qrcode` npm package (zero server round-trip, no network dependency); `role="img"` with descriptive `aria-label`; used inside `PaymentMethodPanel`
- UX-DR13: Implement `ShowcaseWaitlistForm` custom component (showcase page only) — live counter display, email input, submit button; states: default, submitting (button disabled), submitted (counter increments, inline confirmation replaces form, no redirect); variants: tenant waitlist (primary, top) and landlord interest (footer, different label + separate DB tag)
- UX-DR14: Implement landlord portal responsive layout — mobile: single column, top header, hamburger + Sheet drawer (Dashboard, Bills, Condition Report, Maintenance, Property Settings); desktop (≥1024px): persistent 220px left sidebar + content area, content max-width ~800px within right pane
- UX-DR15: Implement tenant portal responsive layout — mobile: single column, top header, contextual back links (no hamburger); tablet (≥768px): centred max-width ~640px; desktop (≥1024px): horizontal top navigation (Current Bill, History, Welcome Pack, Condition Report), centred single column max-width ~680px
- UX-DR16: Implement showcase page responsive layout — mobile (390px): full-bleed hero photo, single column, scroll-driven narrative, 64–96px section spacing; desktop (1280px): max-content ~960px centred with generous margins; Playfair Display headings 64–80px; parallax and scroll-driven section transitions
- UX-DR17: Implement "I've paid" button as full-width mustard (`#C8952A`), minimum 52px height, largest/most visually prominent action on billing screen, always reachable without scrolling on mobile; optimistic UI update — status changes immediately on tap, rolls back on API error
- UX-DR18: Implement skeleton screens as `{ScreenName}Skeleton` named components for every data-fetching screen; never blank/white loading states
- UX-DR19: Implement skip-to-content link as first focusable element on every page; all interactive elements reachable via Tab key following visual reading order; focus ring via `focus-visible:ring-2 focus-visible:ring-[#4A6172]`; never `outline: none` without replacement focus indicator; Radix UI handles focus trapping in modals/sheets
- UX-DR20: Implement ARIA live regions for toast/status notifications (`role="status" aria-live="polite"`); `aria-busy="true"` on loading containers; page titles update on route change via Next.js `metadata` export; icon-only buttons include `aria-label`

### Cross-Cutting Constraints

These constraints apply to **every story** in this document. Dev agents must read and apply these before implementing any story — they are not repeated in individual story ACs.

**NFR-S6 — XSS Output Encoding**
All user-generated content rendered in the UI must be output-encoded. Next.js JSX handles this by default — never use `dangerouslySetInnerHTML` with user-supplied content under any circumstances.

**NFR-S7 — SQL Injection & Input Sanitization**
All database queries must use EF Core parameterised queries. Never interpolate user input directly into SQL strings. All text inputs must be sanitized before processing.

**NFR-S8 — Cross-Tenant Data Isolation**
Every new API endpoint must be verified: a request from User A must never return data belonging to User B. The `HasQueryFilter` on EF Core handles landlord-scoped entities automatically — verify it is active on any new entity. For tenant-scoped access, per-resource authorisation checks are mandatory on every endpoint (role check alone is insufficient). Return 404, never 403, when a resource belongs to another user (no resource enumeration).

**NFR-P1 — App Screen Performance**
All authenticated app screens must load within 2 seconds on a standard Bulgarian mobile connection (LTE/4G, ~20 Mbps) at 390px viewport. Use skeleton screens on every data-fetching screen. Avoid blocking sequential data fetches — parallelise where possible. Never embed PDF content in page payloads; always use signed Azure Blob URLs.

**NFR-S1 — HTTPS Only**
No HTTP endpoints in production. Azure App Service must be configured to redirect HTTP → HTTPS. Azure Static Web Apps enforces HTTPS by default — verify this is not disabled.

**NFR-S2 — Database Encryption at Rest**
Neon PostgreSQL encryption at rest must be confirmed enabled in the Neon dashboard during Story 1.2 setup. This is a one-time configuration check, not a code concern.

**NFR-R1 / NFR-R2 — Uptime & Email Delivery**
Uptime (≥ 99.5%) and email delivery rate (≥ 99%) are operational SLA targets, not testable in stories. They are met through Azure App Service B1 SLA and Resend's delivery infrastructure + automatic retry. Monitor Resend delivery dashboard from day one.

**NFR-SC2 — Scaling via Configuration Only**
All infrastructure scaling (compute tier, database connections, email volume) must be achievable through Azure portal / Neon dashboard / Resend plan upgrades — no code or architecture changes required. This is an architectural constraint enforced by the chosen stack, not a per-story concern.

### FR Coverage Map

| FR | Epic | Description |
|---|---|---|
| FR1 | Epic 1 | Landlord login |
| FR2 | Epic 1 | Tenant login |
| FR3 | Epic 1 | Forced password change on first login |
| FR4 | Epic 1 | Password change from account settings |
| FR4b | Epic 1 | Password reset via email link |
| FR5 | Epic 1 | Role-based access enforcement (Landlord / Tenant) |
| FR6 | Epic 2 | Property creation |
| FR7 | Epic 2 | Payment method configuration per property |
| FR8 | Epic 2 | Welcome pack creation and editing |
| FR9 | Epic 2 | Property detail view and edit |
| FR10 | Epic 2 | Tenant account creation |
| FR11 | Epic 2 | Tenant invitation email with credentials |
| FR12 | Epic 2 | Tenant welcome pack view |
| FR13 | Epic 6 | Move-out flow trigger by landlord |
| FR14 | Epic 6 | Read-only account on move-out |
| FR15 | Epic 6 | 12-month auto-expiry |
| FR16 | Epic 4 | Landlord pre-loads condition report items |
| FR17 | Epic 4 | Tenant views landlord baseline (read-only) |
| FR18 | Epic 4 | Tenant adds own condition report items |
| FR19 | Epic 4 | Tenant agrees — PDF generated + emailed |
| FR20 | Epic 4 | Tenant disagrees — submits disputed items |
| FR21 | Epic 4 | Landlord reviews disputes + re-requests sign-off |
| FR22 | Epic 4 | 3-round dispute cap |
| FR23 | Epic 4 | PDF with full dispute history |
| FR24 | Epic 4 | Permanent PDF storage for condition reports |
| FR25 | Epic 4 | Round number + pending party visibility |
<!-- Note: FR19, FR23, FR24 were previously split across Stories 4.2 and 4.3. These are now merged into Story 4.2. -->
| FR26 | Epic 6 | Move-out condition report (reuses same editor) |
| FR27 | Epic 3 | Landlord bill upload (PDF + amount per category) |
| FR28 | Epic 3 | Tenant itemised bill view with inline PDFs |
| FR29 | Epic 3 | Bill PDF open/download |
| FR30 | Epic 3 | Payment method display (IBAN / IRIS Pay / Revolut) |
| FR31 | Epic 3 | "I've paid" tenant action |
| FR32 | Epic 3 | Landlord payment confirmation |
| FR33 | Epic 3 | Receipt PDF generation + email |
| FR34 | Epic 3 | Full payment history access |
| FR35 | Epic 3 | Historical receipt/bill PDF download |
| FR36 | Epic 5 | Tenant maintenance request submission |
| FR36b | Epic 5 | Email to landlord on new maintenance request |
| FR37 | Epic 5 | Landlord maintenance request list |
| FR37b | Epic 5 | Condition report completion status on dashboard |
| FR38 | Epic 5 | Maintenance status update by landlord |
| FR39 | Epic 5 | Tenant email notification on status update |
| FR40 | Epic 3 | Bill upload notification email to tenant |
| FR41 | Epic 3 | Payment due reminder (Day 3 after bill upload) |
| FR42 | Epic 3 | "I've paid" notification email to landlord |
| FR43 | Epic 3 | Receipt email to tenant |
| FR44 | Epic 4 | Condition report Day 3 + Day 7 nudge emails |
| FR45 | Epic 4 | Day 14 reminder email to both parties |
| FR46 | Epic 4 | Day 14 auto-resolution — incomplete PDF |
| FR48 | Epic 4 | Condition report PDF emailed on sign-off |
| FR49 | Epic 6 | Final move-out bundle email to former tenant |
| FR50 | Epic 6 | Final PDF bundle generation (all receipts + CRs) |
| FR51 | Epic 6 | Former tenant read-only data access |
| FR52 | Epic 6 | Profile data deletion (GDPR request) |
| FR53 | Epic 6 | Legal retention enforcement — tenancy records |
| FR54 | Epic 7 | Public showcase page |
| FR55 | Epic 7 | BG/EN language toggle on showcase |
| FR56 | Epic 7 | Waitlist email capture |
| FR57 | Epic 7 | Waitlist counter display |
| FR58 | Epic 7 | Landlord interest CTA (separate DB tag) |
| FR59 | Epic 7 | Ghost login link on showcase page |
| FR60 | Epic 1 | Authenticated language toggle (BG/EN) |

## Epic List

### Epic 1: Deployable Foundation — Production-Ready Application Shell
The application is deployed to production. Kiril can log in with his seeded landlord account, change password, access the (empty) dashboard, and toggle between Bulgarian and English. All infrastructure is operational: Neon database, ASP.NET Core Identity + JWT with `TokenValidFrom` revocation, Next.js BFF auth layer, Azure Blob connection, `IHostedService` bootstrap, next-intl URL-prefix routing, design token system, GitHub Actions CI/CD, and both environments live.
**FRs covered:** FR1, FR2, FR3, FR4, FR4b, FR5, FR60

### Epic 2: Property Setup & Tenant Onboarding
The landlord creates and configures a property (name, address, size, floor, bill categories, payment methods, welcome pack), invites a tenant by name and email, and the tenant receives their credentials, sets up their password, and can view their welcome pack. All portal navigation scaffolding (landlord sidebar + tenant top nav) is in place.
**FRs covered:** FR6, FR7, FR8, FR9, FR10, FR11, FR12

### Epic 3: Monthly Billing Loop — Core Product Value
The complete billing cycle works end-to-end: landlord uploads utility PDFs + enters amounts per category → bills sent → tenant views itemised breakdown with inline PDFs → tenant selects payment method (IBAN / IRIS Pay / Revolut) → one-tap clipboard copy or QR → tenant marks "I've paid" → landlord confirms receipt → receipt PDF auto-generated and emailed. Payment history permanently accessible. All billing-related email notifications operational.
**FRs covered:** FR27, FR28, FR29, FR30, FR31, FR32, FR33, FR34, FR35, FR40, FR41, FR42, FR43

### Epic 4: Condition Reports — Joint Legal Record
The complete move-in condition report flow: landlord pre-loads dispute-prone items with photos before tenant first login → tenant sees Safety Intro Screen → tenant reviews landlord baseline → tenant adds their own items with photos → agree (PDF generated, timestamped, emailed to both) or disagree (dispute flow, max 3 rounds, unresolved state documented). Day 3/7/14 nudge system operational. Auto-resolution at Day 14.
**FRs covered:** FR16, FR17, FR18, FR19, FR20, FR21, FR22, FR23, FR24, FR25, FR44, FR45, FR46, FR48
**Stories:** 4.1, 4.2 (merged from original 4.2 + 4.3), 4.3 (renumbered from 4.4), 4.4 (renumbered from 4.5), 4.5 (renumbered from 4.6)

### Epic 5: Maintenance Requests
Tenants can submit maintenance requests with a title, description, and photos. Landlords see all requests per property with their current status and can update it (Received → In Progress → Resolved). Tenant receives an email notification on each status update. Landlord dashboard surfaces condition report completion status alongside maintenance.
**FRs covered:** FR36, FR36b, FR37, FR37b, FR38, FR39

### Epic 6: Move-Out & End of Tenancy
The complete end-of-tenancy flow: landlord triggers move-out (confirmation modal) → move-out condition report initiated (pre-loaded with agreed move-in baseline) → tenant completes report → final PDF bundle (both condition reports + all receipts) auto-generated and emailed → account immediately transitions to read-only → 12-month auto-expiry. GDPR data-split enforced: profile data deletable on request, tenancy records retained.
**FRs covered:** FR13, FR14, FR15, FR26, FR49, FR50, FR51, FR52, FR53

### Epic 7: Public Showcase Page
The public showcase page is live at `renteasy.bg` — a scroll-driven editorial experience (Playfair Display headings, full-bleed photos, parallax transitions). BG/EN language toggle. Apartment waitlist with live counter. Landlord interest CTA at footer with separate DB tag. Ghost login link. Sub-2-second FCP. Open Graph, structured data, and SEO metadata in place.
**FRs covered:** FR54, FR55, FR56, FR57, FR58, FR59

---

## Epic 1: Deployable Foundation — Production-Ready Application Shell

The application is deployed to production. Kiril can log in with his seeded landlord account, change password, access the (empty) dashboard, and toggle between Bulgarian and English. All infrastructure is operational: Neon database, ASP.NET Core Identity + JWT with `TokenValidFrom` revocation, Next.js BFF auth layer, Azure Blob connection, `IHostedService` bootstrap, next-intl URL-prefix routing, design token system, GitHub Actions CI/CD, and both environments live.

### Story 1.1: Monorepo Scaffold & CI/CD Pipeline

As a developer,
I want a fully scaffolded monorepo with both projects initialized and deployed to production via CI/CD,
So that the team has a working production baseline from day one and all subsequent stories can be merged and deployed automatically.

**Acceptance Criteria:**

**Given** the monorepo root at `RentEasyV2/`
**When** `npx create-next-app@latest renteasy-web` is run with TypeScript, ESLint, Tailwind CSS, App Router, src/, and Turbopack selected
**Then** the `renteasy-web/` directory contains a working Next.js 16 application that builds successfully with `npm run build`

**Given** the monorepo root
**When** `dotnet new webapi --use-controllers --use-program-main -o renteasy-api` is run
**Then** the `renteasy-api/` directory contains a working ASP.NET Core 10 Web API with controller support, explicit `Program.cs`, and `dotnet build` passes

**Given** both projects are scaffolded
**When** all additional packages are installed (frontend: `next-intl`, `qrcode`; backend: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Azure.Storage.Blobs`, `QuestPDF`, `resend-dotnet`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`)
**Then** both projects build without errors

**Given** code is pushed to the `main` branch
**When** GitHub Actions runs
**Then** `api-deploy.yml` builds and deploys `renteasy-api` to Azure App Service B1
**And** `web-deploy.yml` builds and deploys `renteasy-web` to Azure Static Web Apps

**Given** the production deployment has completed
**When** an HTTP GET is made to the API's health/status endpoint
**Then** a 200 response is returned

**Given** the production deployment has completed
**When** a browser visits the Azure Static Web App URL
**Then** the Next.js application loads without errors

---

### Story 1.2: Database Schema & EF Core Foundation

As a developer,
I want the core database schema established with multi-tenancy and the identity foundation in place,
So that all subsequent stories can persist data against a correctly structured, multi-tenant-safe database.

**Acceptance Criteria:**

**Given** a new Neon PostgreSQL database with two connection strings configured in `appsettings.json`
**When** `dotnet ef database update` is run using the direct (non-pooled) connection string
**Then** the `AspNetUsers` table is created with two additional columns: `token_valid_from` (timestamptz, not null) and `account_state` (varchar, not null, default `'Active'`)

**Given** the initial migration is applied
**When** the `properties` table schema is inspected
**Then** it exists with columns: `id` (UUID PK), `landlord_id` (UUID FK → AspNetUsers, indexed), `name`, `address`, `size_sqm`, `floor`, `created_at`, `updated_at`

**Given** the `AppDbContext` is configured
**When** a LINQ query is executed for any landlord-scoped entity
**Then** `HasQueryFilter` automatically filters results to only the current user's `LandlordId`
**And** querying without an authenticated context throws an exception (not silently returns all records)

**Given** the multi-tenancy filter is active
**When** a landlord user queries properties
**Then** only their own properties are returned, never any other landlord's records

**Given** the application starts at runtime
**When** EF Core establishes a database connection
**Then** the pooled Neon connection string is used (not the direct migration string)

---

### Story 1.3: Authentication API (Login, Password Change, Password Reset)

As a landlord or tenant,
I want to log in, change my password, and reset a forgotten password via email,
So that my account is secure and I can always regain access.

**Acceptance Criteria:**

**Given** a user with valid credentials
**When** `POST /api/auth/login` is called with correct email and password
**Then** a JWT is returned containing role claim (`Landlord` or `Tenant`), `account_state` claim, and 7-day expiry
**And** the JWT's `iat` (issued at) is ≥ the user's `token_valid_from` value

**Given** a user submits a login request with an incorrect password
**When** `POST /api/auth/login` is processed
**Then** a 401 Unauthorized response with an RFC 7807 `ProblemDetails` body is returned

**Given** an authenticated user
**When** `POST /api/auth/change-password` is called with the correct current password and a valid new password
**Then** the password is updated and `token_valid_from` is set to the current UTC timestamp
**And** any previously issued JWT is immediately invalid (iat < new token_valid_from)

**Given** `TokenValidFromMiddleware` is registered in the request pipeline
**When** any authenticated API request is processed
**Then** the JWT's `iat` is compared to the user's current `token_valid_from` in the database
**And** if `iat < token_valid_from`, a 401 Unauthorized ProblemDetails response is returned

**Given** a user with `account_state = 'RequiresPasswordChange'`
**When** any API request other than `POST /api/auth/change-password` is made
**Then** a 403 Forbidden ProblemDetails response is returned

**Given** a valid registered email address
**When** `POST /api/auth/forgot-password` is called with that email
**Then** a password reset email is sent containing a time-limited link (valid 1 hour)
**And** if the email is not found, a 200 OK response is still returned (no email enumeration)

**Given** a valid, unexpired reset token
**When** `POST /api/auth/reset-password` is called with the token and a new password
**Then** the password is updated, `token_valid_from` is refreshed, and the token is invalidated

---

### Story 1.4: Next.js BFF Auth Layer & i18n Routing

As a developer,
I want the Next.js BFF auth layer and i18n URL-prefix routing in place,
So that the browser communicates securely via HttpOnly cookies and all app routes are available in Bulgarian and English.

**Acceptance Criteria:**

**Given** a user submits credentials via the login form
**When** the Next.js BFF `POST /api/auth/login` Route Handler is called
**Then** it forwards the credentials to the ASP.NET Core API
**And** on success, stores the JWT in an HttpOnly, Secure, SameSite=Strict cookie on the SWA domain
**And** returns the user's role and `account_state` to the client (not the raw JWT)

**Given** an authenticated page component calls `lib/api.ts`
**When** a request is made to the ASP.NET Core API
**Then** the JWT is read from the HttpOnly cookie server-side and forwarded as a `Bearer` header
**And** the browser never has direct JavaScript access to the JWT string

**Given** a user calls `/api/auth/logout`
**When** the Route Handler executes
**Then** the HttpOnly JWT cookie is cleared and the user is redirected to `/[locale]/login`

**Given** next-intl is configured with `bg` as the default locale
**When** a user visits the root path `/`
**Then** `middleware.ts` redirects them to `/bg/`

**Given** a user is on a `/bg/` route
**When** they switch the language toggle to EN
**Then** they are navigated to the equivalent `/en/` route without a full page reload

**Given** any authenticated route (`/[locale]/(landlord)/*` or `/[locale]/(tenant)/*`)
**When** an unauthenticated user attempts to access it
**Then** the route group layout redirects them to `/[locale]/login`

---

### Story 1.5: Login & Authentication UI (Both Roles)

As a landlord or tenant,
I want a login screen, a forced first-login password change screen, and account settings password change,
So that I can securely access my account and manage my credentials from any device.

**Acceptance Criteria:**

**Given** an unauthenticated user visits any authenticated route
**When** the route guard evaluates their session
**Then** they are redirected to `/[locale]/login`

**Given** the login page at `/[locale]/login`
**When** a user enters valid credentials and submits
**Then** they are redirected to their role-appropriate default screen: landlord → `/[locale]/dashboard`, tenant → `/[locale]/billing`

**Given** a user with `account_state = 'RequiresPasswordChange'` successfully logs in
**When** they are redirected post-login
**Then** they land on `/[locale]/change-password` and cannot navigate to any other route until the password is changed

**Given** the forced password change screen
**When** the user enters a new password meeting the minimum requirements (8+ characters, letters + numbers)
**Then** it is submitted successfully and they are redirected to their dashboard
**And** the password field uses a single field with a show/hide eye icon — no confirmation field

**Given** the account settings page for an authenticated user
**When** they submit a password change with the correct current password and a valid new password
**Then** the password is updated and an inline success message is shown (no page reload)

**Given** the language toggle is present in the authenticated app header
**When** a user taps BG or EN
**Then** the active locale switches instantly without a reload or re-authentication
**And** the `<html lang="...">` attribute updates to match the active language

**Given** any login or password form field has a validation error
**When** the user moves focus away from the field (blur event)
**Then** an inline error message appears beneath the field in text (not colour alone)

---

### Story 1.6: Design System Foundation

As a developer,
I want the complete design system established — tokens, typography, shadcn/ui theming, and accessibility baseline,
So that all subsequent UI stories can be built on a consistent, accessible visual foundation without repeating setup work.

**Acceptance Criteria:**

**Given** the Tailwind config is set up
**When** CSS custom properties are inspected on any page
**Then** all 11 colour tokens are defined as CSS variables: `--color-bg: #F8F4EE`, `--color-surface: #FFFFFF`, `--color-primary: #4A6172`, `--color-accent: #C8952A`, `--color-oak: #C4955A`, `--color-text: #1E1E1E`, `--color-muted: #6B7280`, `--color-success: #3D7A5F`, `--color-pending: #B87A1A`, `--color-error: #C0392B`, `--color-stone: #3A3D42`

**Given** the typography system is configured
**When** Inter and Playfair Display font files are loaded
**Then** both include their Cyrillic character subsets
**And** Inter is set as the default `font-sans` applied globally
**And** Playfair Display is available as `font-display` for showcase page use

**Given** shadcn/ui components are generated into `/components/ui/`
**When** Button, Input, Form, Tabs, Badge, Dialog, AlertDialog, Sheet, Skeleton, and Separator are installed
**Then** their CSS variable overrides apply the RentEasy colour tokens (primary CTA button → `--color-accent`, focus rings → `--color-primary`)

**Given** any page component that fetches data
**When** data is loading
**Then** a `{ScreenName}Skeleton` component renders (never a blank white screen)

**Given** any interactive element (button, link, input, tab)
**When** it receives keyboard focus via Tab key
**Then** a visible focus ring renders using `focus-visible:ring-2 focus-visible:ring-[#4A6172]`
**And** `outline: none` is never applied without a replacement focus indicator

**Given** any page renders
**When** the DOM is inspected
**Then** a skip-to-content `<a>` link is the first focusable element, pointing to `#main-content`

**Given** any interactive element
**When** its computed tap target is measured
**Then** it is at least 44×44px (achieved via `min-h-[44px]` and appropriate padding)

---

## Epic 2: Property Setup & Tenant Onboarding

The landlord creates and configures a property (name, address, size, floor, bill categories, payment methods, welcome pack), invites a tenant by name and email, and the tenant receives their credentials, sets up their password, and can view their welcome pack. All portal navigation scaffolding (landlord sidebar + tenant top nav) is in place.

### Story 2.1: Property & Payment Method API

As a landlord,
I want to create and configure a property with its payment methods via the API,
So that properties are stored securely, scoped to my account, and ready for tenants.

**Acceptance Criteria:**

**Given** an authenticated landlord
**When** `POST /api/properties` is called with name, address, size_sqm, floor, and a list of bill categories
**Then** a new property is created with a UUID `id`, `landlord_id` set to the current user
**And** a 201 Created response with the property DTO is returned

**Given** an authenticated landlord
**When** `PUT /api/properties/{id}/payment-methods` is called with IBAN, IRIS Pay phone number, and Revolut.me link
**Then** the payment methods are saved against the property

**Given** a landlord attempts to access a property belonging to another landlord
**When** `GET /api/properties/{id}` is called
**Then** a 404 Not Found ProblemDetails response is returned (never 403 — no resource enumeration)

**Given** an authenticated landlord
**When** `GET /api/properties/{id}` is called for their own property
**Then** the full property detail including payment methods and bill categories is returned

**Given** an authenticated landlord
**When** `PUT /api/properties/{id}` is called with updated property details
**Then** the property is updated and a 200 response with the updated DTO is returned

---

### Story 2.2: Welcome Pack API

As a landlord,
I want to create and edit the welcome pack for a property via the API,
So that tenants receive all property information they need on first login.

**Acceptance Criteria:**

**Given** an authenticated landlord with an existing property
**When** `PUT /api/properties/{id}/welcome-pack` is called with welcome pack sections (apartment manual, utility contacts, building management contact, WiFi credentials, garbage collection schedule, emergency numbers)
**Then** the welcome pack is saved against the property and a 200 response is returned

**Given** a tenant authenticated to a specific property
**When** `GET /api/properties/{id}/welcome-pack` is called
**Then** the full welcome pack content for their property is returned
**And** a tenant from a different property cannot access this welcome pack (returns 404)

**Given** a landlord updates the welcome pack for a property
**When** a tenant subsequently fetches the welcome pack
**Then** they see the latest version

---

### Story 2.3: Property Setup UI (Landlord)

As a landlord,
I want to create a property, configure its payment methods and bill categories, and edit property details from a responsive dashboard,
So that my property is fully configured before any tenant is invited.

**Acceptance Criteria:**

**Given** the landlord dashboard with no properties
**When** the landlord clicks "Add property"
**Then** a property creation form is shown with fields: name, address, size (m²), floor, and bill category checkboxes (Rent, Electricity, Water, Building Maintenance Fee)

**Given** all required fields are filled
**When** the landlord submits the form
**Then** the property is created and the landlord is taken to the property detail page

**Given** the property detail page
**When** the landlord navigates to payment methods settings
**Then** they can enter and save their IBAN, IRIS Pay phone number, and Revolut.me link

**Given** the property detail page
**When** the landlord navigates to the welcome pack editor
**Then** they can create and edit all welcome pack sections (apartment manual, utility contacts, building management contact, WiFi credentials, garbage collection schedule, emergency numbers)

**Given** the landlord portal on mobile (390px)
**When** any screen is rendered
**Then** a single-column layout is used with a hamburger button that opens a Sheet drawer containing navigation links (Dashboard, Bills, Condition Report, Maintenance, Property Settings)

**Given** the landlord portal on desktop (≥1024px)
**When** any screen is rendered
**Then** a persistent 220px left sidebar displays navigation links and the content area fills the remaining width with max-width ~800px

---

### Story 2.4: Tenant Account Creation & Invitation Email

As a landlord,
I want to create a tenant account by entering their name and email, triggering an invitation email with credentials,
So that the tenant can set up their account and access the platform immediately.

**Acceptance Criteria:**

**Given** an authenticated landlord with a configured property
**When** `POST /api/properties/{id}/tenants` is called with tenant name and email address
**Then** a new tenant user is created with `account_state = 'RequiresPasswordChange'` and a temporary password
**And** a tenancy record linking the tenant to the property is created with status `Active`
**And** a 201 response with the tenant DTO is returned

**Given** a tenant account has been created
**When** the invitation email is sent via Resend API
**Then** the email contains the tenant's login credentials (email + temporary password) and a "Set up your account" CTA link
**And** the sender name is "Kiril via RentEasy" and subject is "Your apartment at [address] is ready"

**Given** an invitation email fails to deliver
**When** Resend reports a delivery failure
**Then** the failure is logged and Resend's automatic retry is relied upon

**Given** a landlord attempts to invite a tenant to a property they do not own
**When** the API request is processed
**Then** a 404 Not Found ProblemDetails response is returned

---

### Story 2.5: Tenant First Login, Welcome Pack & Portal Navigation

As a tenant,
I want to set up my account on first login and immediately access my welcome pack from a clean, mobile-first portal,
So that I have all the information I need about my new apartment from day one.

**Acceptance Criteria:**

**Given** a tenant with `account_state = 'RequiresPasswordChange'` enters their temporary credentials
**When** they log in
**Then** they are redirected to the forced password change screen and cannot navigate elsewhere

**Given** the tenant has set a new password
**When** they are redirected to their portal
**Then** they land on a welcome screen showing their apartment address with CTAs for "View welcome pack" and "Start condition report"

**Given** the tenant navigates to the welcome pack
**When** the page renders
**Then** each section displays using the `WelcomePackSection` component in the appropriate variant (text, key-value for WiFi/contacts, list)
**And** any contact or credential entries have a one-tap copy button with `aria-label`

**Given** the tenant portal on mobile (390px)
**When** any screen is rendered
**Then** a single-column layout is used with a top header and contextual back links (no hamburger menu)

**Given** the tenant portal on desktop (≥1024px)
**When** any screen is rendered
**Then** a horizontal top navigation bar displays: Current Bill, History, Welcome Pack, Condition Report
**And** content is centred in a single column at max-width ~680px

**Given** the tenant accesses the welcome pack at any point after first login
**When** they navigate to `/[locale]/welcome`
**Then** the welcome pack content reflects the latest version from the landlord

---

## Epic 3: Monthly Billing Loop — Core Product Value

The complete billing cycle works end-to-end: landlord uploads utility PDFs + enters amounts per category → bills sent → tenant views itemised breakdown with inline PDFs → tenant selects payment method (IBAN / IRIS Pay / Revolut) → one-tap clipboard copy or QR → tenant marks "I've paid" → landlord confirms receipt → receipt PDF auto-generated and emailed. Payment history permanently accessible. All billing-related email notifications operational.

### Story 3.1: Bill Period & File Storage API

As a landlord,
I want to upload utility bill PDFs and enter amounts per category via the API, with files stored securely in Azure Blob,
So that tenants can access itemised bills and their underlying PDFs at any time.

**Acceptance Criteria:**

**Given** an authenticated landlord
**When** `POST /api/properties/{id}/bill-periods` is called with a billing month/year
**Then** a new `BillPeriod` record is created with UUID `id`, linked to the property, with `payment_status = 'Unpaid'`

**Given** a bill period exists
**When** `POST /api/bill-periods/{id}/bills` is called with a bill category, amount (decimal), and an optional PDF file attachment
**Then** the amount is stored on the `Bill` record
**And** if a PDF is attached, it is validated (MIME type + file signature must be `application/pdf`)
**And** the validated PDF is uploaded to Azure Blob at path `{tenancyId}/bills/{uuid}.pdf`
**And** the blob path is stored on the `Bill` record

**Given** a file upload with an invalid MIME type or file signature
**When** the upload endpoint processes it
**Then** a 400 Bad Request ProblemDetails response is returned and no file is stored

**Given** an authenticated tenant
**When** `GET /api/bill-periods/current` is called
**Then** the current billing period is returned with all bill line items (category, amount, and a signed Azure Blob URL for any attached PDF, valid for 1 hour)

**Given** a signed URL has expired
**When** a tenant requests the bill period
**Then** a fresh signed URL (1-hour expiry) is generated and returned

**Given** an authenticated landlord
**When** `POST /api/bill-periods/{id}/send` is called
**Then** the bill period status is set to `Sent` and the tenant notification email is triggered (FR40)

---

### Story 3.2: Payment Confirmation & Receipt PDF Generation

As a landlord,
I want to confirm a tenant's payment and have a receipt PDF auto-generated and emailed to the tenant,
So that every payment has a permanent, professional record.

**Acceptance Criteria:**

**Given** a bill period with `payment_status = 'PendingConfirmation'`
**When** `POST /api/bill-periods/{id}/confirm-payment` is called with the amount confirmed as received
**Then** the `payment_status` is updated to `'Confirmed'`
**And** the `confirmed_amount` and `confirmed_at` timestamp are stored

**Given** payment is confirmed
**When** `PdfService.GenerateReceiptAsync` is called
**Then** a PDF receipt is generated containing: total confirmed amount, confirmation date, all bill line items with amounts, and all bill PDFs bundled as attachments
**And** the receipt PDF is uploaded to Azure Blob at `{tenancyId}/receipts/{uuid}.pdf`
**And** the receipt email (FR43) is sent to the tenant via Resend with the PDF attached

**Given** PDF generation fails for any reason
**When** the failure is detected
**Then** the error is logged with full context
**And** generation is retried (up to 3 attempts) before the failure is surfaced

**Given** an authenticated tenant
**When** `GET /api/bill-periods/{id}/receipt` is called for a confirmed period
**Then** a signed Azure Blob URL (24-hour expiry) for the receipt PDF is returned

---

### Story 3.3: Billing Email Notifications & Payment Nudge Scheduler

As a system,
I want all billing email notifications sent reliably and the Day 3 payment nudge scheduled automatically,
So that tenants are informed at every step of the billing cycle without landlord intervention.

**Acceptance Criteria:**

**Given** a landlord sends bills (`POST /api/bill-periods/{id}/send`)
**When** the send action completes
**Then** a "Your [month] bills are ready" email (FR40) is sent to the tenant via Resend
**And** an `EmailNudgeJob` record of type `PaymentDueDay3` is created with `scheduled_for = sent_at + 3 days`

**Given** a tenant marks payment as made (`POST /api/bill-periods/{id}/mark-paid`)
**When** the action completes
**Then** the bill period `payment_status` is set to `'PendingConfirmation'`
**And** the "I've paid" notification email (FR42) is sent to the landlord
**And** any pending `PaymentDueDay3` nudge job for this period is cancelled (`cancelled = true`)

**Given** the `NudgeSchedulerJob` `IHostedService` polls every 5 minutes
**When** a `PaymentDueDay3` job is found with `scheduled_for <= now`, `sent_at IS NULL`, and `cancelled = false`
**Then** the payment due reminder email (FR41) is sent to the tenant
**And** the job's `sent_at` is set to prevent duplicate sends

**Given** a nudge job's `sent_at` is already set
**When** the scheduler polls and finds the same job
**Then** no duplicate email is sent (idempotency guaranteed)

---

### Story 3.4: Tenant Billing Screen (Itemised View, PDFs, Status Badge)

As a tenant,
I want to see an itemised breakdown of my monthly bills with each PDF accessible in one tap, and always know my current payment status,
So that I understand exactly what I owe without asking a single question.

**Acceptance Criteria:**

**Given** a tenant navigates to `/[locale]/billing`
**When** the page loads
**Then** the `BillingScreenSkeleton` renders immediately while data is fetching (never a blank white screen)

**Given** the current billing period has been sent
**When** the billing screen fully loads
**Then** a `PaymentStatusBadge` is displayed at the top: "Unpaid" (amber), "Payment pending confirmation" (amber), or "Paid ✓" (green) — always with a text label, never colour alone

**Given** the billing screen is displaying the current period
**When** the tenant scrolls the bill list
**Then** each charge renders as a `BillLineItem`: category name (left), amount in EUR (right, bold), and a PDF icon (far right) for any category with an attached PDF

**Given** a bill line item has an attached PDF
**When** the tenant taps the PDF icon
**Then** the bill PDF opens in a new browser tab using the native PDF viewer

**Given** a bill line item has no attached PDF (e.g. Rent)
**Then** no PDF icon is shown for that line item

**Given** the billing screen is in "Payment pending confirmation" state
**When** the status badge renders
**Then** an inline note reads "We'll notify you when [landlord name] confirms"

---

### Story 3.5: Payment Method Display (IBAN / IRIS Pay / Revolut + QR)

As a tenant,
I want to see all payment method details with one-tap clipboard copy and QR codes,
So that I can pay using my preferred method without manually typing any financial details.

**Acceptance Criteria:**

**Given** the billing screen is displaying an unpaid period
**When** the payment methods section renders
**Then** a tab switcher (`PaymentMethodPanel`) displays three tabs: IBAN (selected by default), IRIS Pay, Revolut

**Given** the IBAN tab is active
**When** the tenant views it
**Then** the full IBAN account number is displayed with a copy button labelled "Copy IBAN"

**Given** the IRIS Pay tab is active
**When** the tenant views it
**Then** the landlord's phone number is displayed with a copy button
**And** a QR code is rendered client-side via the `qrcode` package with no server round-trip
**And** QR code generation completes in under 100ms

**Given** the Revolut tab is active
**When** the tenant views it
**Then** the Revolut.me link is displayed with a copy button
**And** a QR code is rendered client-side

**Given** any copy button is tapped
**When** `navigator.clipboard.writeText()` executes
**Then** the button label changes to "Copied ✓" for 2 seconds then reverts (no toast, no modal)

**Given** all QR codes
**When** rendered
**Then** they each have `role="img"` and a descriptive `aria-label`

---

### Story 3.6: "I've Paid" Flow & Landlord Payment Confirmation UI

As a tenant and landlord,
I want the "I've paid" action and landlord confirmation to be simple, immediate, and unambiguous,
So that the payment cycle closes cleanly every month with a single tap from each side.

**Acceptance Criteria:**

**Given** the billing screen is showing an unpaid period
**When** the tenant views it on mobile
**Then** the "I've paid" button is full-width, mustard `#C8952A`, minimum 52px height, and reachable without scrolling

**Given** the tenant taps "I've paid"
**When** the action is submitted
**Then** the status badge updates immediately to "Payment pending confirmation" (optimistic UI)
**And** the `POST /api/bill-periods/{id}/mark-paid` API call is made in the background

**Given** the API call fails
**When** the error is received
**Then** the status badge rolls back to "Unpaid" and an inline error message is shown

**Given** the landlord receives the "I've paid" notification email and opens the app
**When** they navigate to the property's bills
**Then** the bill period shows "Payment pending confirmation" with the tenant's self-reported payment indicator

**Given** the landlord taps "Confirm payment"
**When** the confirmation UI renders
**Then** they enter the amount received (decimal, EUR) and submit — no other fields required

**Given** payment is confirmed
**When** the landlord dashboard refreshes
**Then** the bill period status shows "Paid ✓" with the confirmation date

---

### Story 3.7: Payment History View

As a tenant,
I want to view my complete payment history with all receipts and bill PDFs permanently accessible,
So that I have a full financial record of my tenancy at any time.

**Acceptance Criteria:**

**Given** an authenticated tenant navigates to `/[locale]/history`
**When** the page loads
**Then** all past billing periods are listed in reverse chronological order, each showing: month/year, total amount, and payment status badge

**Given** a billing period in the history list has a confirmed payment
**When** the tenant taps on that period
**Then** they see the full itemised breakdown and a "Download receipt" link

**Given** the tenant taps "Download receipt"
**When** the signed URL request is made
**Then** the receipt PDF opens via a signed Azure Blob URL with 24-hour expiry

**Given** any bill PDF in a historical period
**When** the tenant taps the PDF icon
**Then** the utility bill PDF opens via a signed Azure Blob URL

**Given** a former tenant in read-only access (post move-out, within 12 months)
**When** they access `/[locale]/history`
**Then** their complete payment history remains visible and all PDFs remain downloadable

---

## Epic 4: Condition Reports — Joint Legal Record

The complete move-in condition report flow: landlord pre-loads dispute-prone items with photos before tenant first login → tenant sees Safety Intro Screen → tenant reviews landlord baseline → tenant adds their own items with photos → agree (PDF generated, timestamped, emailed to both) or disagree (dispute flow, max 3 rounds, unresolved state documented). Day 3/7/14 nudge system operational. Auto-resolution at Day 14.

### Story 4.1: Condition Report Data Model & Landlord Pre-load API

As a landlord,
I want to pre-load condition report items (photos + notes) for a property before my tenant's first login,
So that the tenant sees a jointly-documented baseline from the moment they open the app.

**Acceptance Criteria:**

**Given** an authenticated landlord with a property
**When** `POST /api/condition-reports/{tenancyId}/items` is called with a description and one or more photo files
**Then** each photo is validated (MIME type + file signature must be `image/jpeg` or `image/png`)
**And** validated photos are uploaded to Azure Blob at `{tenancyId}/condition-report/{uuid}.jpg`
**And** a `ConditionReportItem` record is created with: UUID `id`, `contributor = 'Landlord'`, description, blob paths, timestamp, and `round = 0` (baseline)

**Given** a photo file upload with an invalid MIME type or file signature
**When** the upload endpoint processes it
**Then** a 400 Bad Request ProblemDetails response is returned and no file is stored

**Given** an authenticated landlord
**When** `GET /api/condition-reports/{tenancyId}` is called
**Then** all pre-loaded items are returned with signed Azure Blob URLs (1-hour expiry) for each photo

**Given** an authenticated tenant
**When** `GET /api/condition-reports/{tenancyId}` is called
**Then** all condition report items are returned, distinguishing landlord items (`contributor = 'Landlord'`) from tenant items (`contributor = 'Tenant'`)

**Given** a tenant attempts to access a condition report for a tenancy they are not part of
**When** the request is processed
**Then** a 404 Not Found ProblemDetails response is returned

---

### Story 4.2: Condition Report Agree/Disagree API + PDF Generation

As a tenant or landlord,
I want the agree/disagree flow enforced by the API with a 3-round cap, permanent dispute history, and a condition report PDF generated on every outcome,
So that the condition report result is legally defensible and permanently stored regardless of whether agreement is reached.

**Acceptance Criteria:**

**Given** a condition report in `InProgress` status and it is the tenant's turn
**When** `POST /api/condition-reports/{tenancyId}/agree` is called by the tenant
**Then** the condition report status is set to `Agreed`
**And** the `agreed_at` timestamp is recorded
**And** `PdfService.GenerateConditionReportAsync` is called immediately

**Given** `PdfService.GenerateConditionReportAsync` is called on an `Agreed` or `Unresolved` condition report
**When** PDF generation runs
**Then** the generated PDF contains: tenancy address, tenant name, all items from all rounds (landlord and tenant), all photos embedded, all timestamps, and the final status (Agreed or Unresolved)
**And** the PDF is uploaded to Azure Blob at `{tenancyId}/condition-report/{uuid}.pdf`
**And** the blob path is stored on the `ConditionReport` record

**Given** the condition report status is `Unresolved`
**When** the PDF is generated
**Then** both parties' positions are fully documented with all rounds, photos, notes, and timestamps
**And** the unresolved state is explicitly noted — documented as an outcome, not a failure

**Given** PDF generation fails
**When** the failure is detected
**Then** the error is logged with full context and generation is retried up to 3 times

**Given** a condition report in `InProgress` status and it is the tenant's turn
**When** `POST /api/condition-reports/{tenancyId}/disagree` is called with disputed item photos and descriptions
**Then** new `ConditionReportItem` records are created with `contributor = 'Tenant'` and the current `round` number
**And** the `pending_action` is set to `'Landlord'`
**And** a dispute notification email is sent to the landlord

**Given** it is the landlord's turn and the current round is ≤ 3
**When** `POST /api/condition-reports/{tenancyId}/landlord-response` is called with optional accepted items and response notes
**Then** the `pending_action` is set to `'Tenant'`
**And** `round` is incremented
**And** a sign-off request email is sent to the tenant

**Given** the tenant disagrees again and the dispute has reached round 3
**When** `POST /api/condition-reports/{tenancyId}/disagree` is called for a third time
**Then** the condition report status is set to `Unresolved`
**And** `PdfService.GenerateConditionReportAsync` is called with both parties' full positions documented
**And** no further dispute actions are accepted on this report

**Given** `GET /api/condition-reports/{tenancyId}` is called by either party
**When** a dispute is in progress
**Then** the response includes: current `round` number, `pending_action` (Tenant or Landlord), and all items from all rounds with contributor labels and timestamps

**Given** `GET /api/condition-reports/{tenancyId}/pdf` is called by either landlord or tenant
**When** a PDF exists
**Then** a signed Azure Blob URL (24-hour expiry) is returned

---

### Story 4.4: Condition Report Nudge Scheduler (Day 3/7/14 + Auto-resolution)

**Depends on:** Story 3.3 — requires the `email_nudge_jobs` table and `NudgeSchedulerJob` `IHostedService` created in Story 3.3. This story adds `ConditionReportDay3/Day7/Day14` job types to that existing infrastructure.

As a system,
I want the Day 3, Day 7, and Day 14 nudge emails sent automatically, with auto-resolution at Day 14 if the report is still incomplete,
So that the condition report is completed within the first two weeks without requiring manual landlord intervention.

**Acceptance Criteria:**

**Given** a new tenancy is created
**When** the tenant account is created
**Then** three `EmailNudgeJob` records are created: `ConditionReportDay3`, `ConditionReportDay7`, and `ConditionReportDay14`, with `scheduled_for` set to tenancy start + 3, 7, and 14 days respectively

**Given** the `NudgeSchedulerJob` polls and finds a `ConditionReportDay3` or `ConditionReportDay7` job due
**When** the condition report is still in `InProgress` status
**Then** a nudge email (FR44) is sent to the tenant and the job's `sent_at` is recorded

**Given** the scheduler finds a `ConditionReportDay14` job due
**When** the condition report is still in `InProgress` status
**Then** a reminder email (FR45) is sent to both tenant and landlord
**And** the condition report is auto-resolved: status set to `AutoResolved`, PDF generated with all items recorded to that point (FR46)
**And** the tenant's full platform access is retained (no restriction)

**Given** the condition report reaches `Agreed` or `Unresolved` before Day 14
**When** the scheduler finds any remaining nudge jobs for this tenancy
**Then** all pending nudge jobs are cancelled (`cancelled = true`) and no further emails are sent

**Given** any nudge job already has `sent_at` set
**When** the scheduler polls and finds the same job
**Then** no duplicate email is sent (idempotency guaranteed)

---

### Story 4.5: Safety Intro Screen & Tenant Condition Report View

As a tenant,
I want a full-screen safety introduction before my first condition report, followed by a clear view of all items from both the landlord and myself,
So that the condition report feels like protection, not a trap.

**Acceptance Criteria:**

**Given** a tenant opens the condition report for the first time
**When** the condition report route renders
**Then** the `SafetyIntroScreen` component is shown full-viewport: large `<h1>` "This report protects you.", 2–3 sentences in landlord's voice explaining the joint record, and a single "Start the report" button
**And** the Safety Intro Screen is shown on first access only — subsequent visits go directly to the report

**Given** the tenant taps "Start the report"
**When** they are taken to the condition report items view
**Then** items load with a skeleton screen (never blank)

**Given** the condition report items are loaded
**When** the tenant views the list
**Then** each item renders as a `ConditionReportItem`: contributor label ("Landlord" or "You"), photo thumbnail(s), description, and timestamp
**And** landlord items are displayed as read-only for the tenant

**Given** the condition report is not yet completed
**When** the tenant visits any app screen
**Then** the `PendingActionBanner` shows a day indicator ("Day 7 of 14") and a "Complete now" CTA
**And** from Day 3–6 the tone is soft; Day 7–13 it is firmer; Day 14 the banner visually replaces the main content area with `role="alert"` — the tenant retains full navigation access and platform functionality (per FR46); only the content area of the current screen is replaced, not the navigation

---

### Story 4.6: Tenant Agree/Disagree & Dispute UI + Landlord Review UI

As a tenant and landlord,
I want the agree/disagree flow and dispute rounds clearly presented with round tracking and photo upload,
So that both parties can complete or document the condition report without confusion.

**Acceptance Criteria:**

**Given** the tenant has reviewed all condition report items
**When** they tap "Agree"
**Then** the agree API call is made, PDF generation is triggered, and a confirmation screen shows "Condition report complete. A PDF has been sent to your email."

**Given** the tenant taps "Disagree"
**When** the disagree screen renders
**Then** they can add disputed items: each with a description text field and a `FileUploadArea` (photo variant — JPEG/PNG, multiple files)
**And** a "Submit disputed items" button is available once at least one item is added

**Given** a dispute is in progress
**When** either party views the condition report
**Then** the `ConditionReportRoundBanner` displays at the top: "Round X of 3" and whose turn it is ("Your turn" or "Waiting for [name]")
**And** the banner uses `role="status"` and neutral styling (never red or alarming)

**Given** the landlord receives a dispute notification and opens the app
**When** they view the condition report
**Then** disputed tenant items are clearly labelled "Tenant added" with photos and descriptions
**And** the landlord can respond and re-request sign-off via a "Request sign-off" button

**Given** the condition report reaches `Unresolved` after round 3
**When** either party views the report
**Then** the status shows "Unresolved — fully documented" with a calm, factual message (not an error state)
**And** a "Download PDF" link is available for the complete dispute record

**Given** a condition report PDF is generated and emailed (FR48)
**When** either party taps "Download PDF"
**Then** the PDF opens via a signed Azure Blob URL (24-hour expiry)

---

## Epic 5: Maintenance Requests

Tenants can submit maintenance requests with a title, description, and photos. Landlords see all requests per property with their current status and can update it (Received → In Progress → Resolved). Tenant receives an email notification on each status update. Landlord dashboard surfaces condition report completion status alongside maintenance.

### Story 5.1: Maintenance Request API

**Depends on:** Story 4.1 — the `GET /api/properties/{id}/condition-report-status` endpoint (FR37b) queries the `condition_reports` table created in Story 4.1. Epic 4 must be complete before this story is implemented.

As a tenant or landlord,
I want maintenance requests created, tracked, and status-updated via the API, with email notifications at each stage,
So that maintenance issues have a clear record and both parties stay informed without external messaging.

**Acceptance Criteria:**

**Given** an authenticated tenant
**When** `POST /api/maintenance-requests` is called with a title, description, and one or more photo files
**Then** each photo is validated (MIME type + file signature must be `image/jpeg` or `image/png`)
**And** validated photos are uploaded to Azure Blob at `{tenancyId}/maintenance/{uuid}.jpg`
**And** a `MaintenanceRequest` record is created with UUID `id`, `status = 'Received'`, tenancy link, title, description, and blob paths
**And** a notification email (FR36b) is sent to the landlord via Resend

**Given** a file upload with an invalid MIME type or file signature
**When** the upload endpoint processes it
**Then** a 400 Bad Request ProblemDetails response is returned and no file is stored

**Given** an authenticated landlord
**When** `GET /api/properties/{id}/maintenance-requests` is called
**Then** all maintenance requests for that property are returned, each showing: title, description, current status, submission date, and photo signed URLs (1-hour expiry)
**And** only requests belonging to the landlord's own properties are returned

**Given** an authenticated landlord
**When** `PUT /api/maintenance-requests/{id}/status` is called with a new status (`InProgress` or `Resolved`)
**Then** the status is updated and a notification email (FR39) is sent to the tenant

**Given** an authenticated tenant attempts to update a maintenance request status
**When** the request is processed
**Then** a 403 Forbidden ProblemDetails response is returned

**Given** `GET /api/properties/{id}/condition-report-status` is called by an authenticated landlord
**When** an active tenancy exists
**Then** the response includes the condition report completion status: `NotStarted`, `InProgress`, or `Complete`

---

### Story 5.2: Tenant Maintenance Request Submission UI

As a tenant,
I want to submit a maintenance request with photos from a simple form,
So that my landlord is notified and the issue is on record without needing to send a Viber message.

**Acceptance Criteria:**

**Given** an authenticated tenant navigates to `/[locale]/maintenance/new`
**When** the page renders
**Then** a form is shown with fields: title (text input), description (textarea), and a `FileUploadArea` (photo variant — JPEG/PNG, multiple files)

**Given** the tenant completes all required fields and attaches at least one photo
**When** they submit the form
**Then** the request is submitted, the landlord is notified by email, and the tenant sees an inline confirmation ("Your request has been submitted. Kiril has been notified.")

**Given** the tenant submits the form without a title or description
**When** validation runs on blur
**Then** inline error messages appear beneath each required field in text (not colour alone)

**Given** a file upload fails (wrong type or network error)
**When** the error occurs
**Then** an inline error message appears within the `FileUploadArea` with a retry option (never a modal)

**Given** the form is submitted successfully
**When** the tenant later views their maintenance section
**Then** their submitted request is listed with its current status badge: "Received", "In Progress", or "Resolved"

---

### Story 5.3: Landlord Maintenance Management UI

As a landlord,
I want to view all maintenance requests for my property and update their status,
So that I can manage issues efficiently and tenants always know what's happening.

**Acceptance Criteria:**

**Given** an authenticated landlord navigates to the maintenance section for a property
**When** the page renders
**Then** all maintenance requests are listed, each showing: title, submission date, current status badge, and a thumbnail of the first photo

**Given** the landlord taps a maintenance request
**When** the detail view renders
**Then** they see the full description, all photo thumbnails (tappable to view full-size via signed URL), and the current status

**Given** a maintenance request is in `Received` or `InProgress` status
**When** the landlord selects a new status and confirms
**Then** the status is updated and the tenant receives a notification email (FR39)

**Given** the landlord dashboard renders
**When** an active tenancy exists
**Then** the condition report completion status (FR37b) is displayed alongside the property card: "Condition report: Not started / In progress / Complete"

**Given** a maintenance request photo thumbnail is tapped
**When** the signed URL is fetched
**Then** the full-size photo opens in a new browser tab

---

## Epic 6: Move-Out & End of Tenancy

The complete end-of-tenancy flow: landlord triggers move-out (confirmation modal) → move-out condition report initiated (pre-loaded with agreed move-in baseline) → tenant completes report → final PDF bundle (both condition reports + all receipts) auto-generated and emailed → account immediately transitions to read-only → 12-month auto-expiry. GDPR data-split enforced: profile data deletable on request, tenancy records retained.

### Story 6.1: Move-Out API & Account State Transitions

As a landlord,
I want to trigger the move-out flow via the API, immediately transitioning the tenant's account to read-only with revoked active-session access,
So that the tenancy closes securely and instantly the moment move-out is triggered.

**Acceptance Criteria:**

**Given** an authenticated landlord with an active tenancy
**When** `POST /api/tenancies/{id}/move-out` is called
**Then** the tenancy `status` is set to `MoveOutInProgress`
**And** the tenant's `account_state` is set to `ReadOnly`
**And** the tenant's `token_valid_from` is updated to the current UTC timestamp, immediately invalidating their active JWT
**And** a move-out condition report is initiated (status `InProgress`, pre-loaded with all items from the agreed move-in report as the baseline)
**And** a notification email is sent to the tenant prompting them to complete the move-out condition report

**Given** a tenant with `account_state = 'ReadOnly'` attempts to perform a write action (mark paid, submit maintenance, add condition report items outside the move-out flow)
**When** the API request is processed
**Then** a 403 Forbidden ProblemDetails response is returned

**Given** a tenant with `account_state = 'ReadOnly'`
**When** they access `GET` endpoints for their own tenancy data (payment history, condition reports, welcome pack)
**Then** all read requests succeed and data is returned normally

**Given** a landlord attempts to trigger move-out on a tenancy they do not own
**When** `POST /api/tenancies/{id}/move-out` is processed
**Then** a 404 Not Found ProblemDetails response is returned

---

### Story 6.2: Final PDF Bundle Generation & Move-Out Emails

**Depends on:** Story 3.2 (receipt PDFs stored in Azure Blob) and Story 4.2 (condition report PDFs stored in Azure Blob). The final bundle is assembled from artifacts created by these two stories. Both Epic 3 and Epic 4 must be complete before this story is implemented.

As a system,
I want the final PDF bundle auto-generated upon move-out completion and emailed to the former tenant,
So that they leave with a complete, permanent record of their entire tenancy.

**Acceptance Criteria:**

**Given** the move-out condition report reaches `Agreed` or `Unresolved`
**When** `PdfService.GenerateFinalBundleAsync` is called
**Then** a final PDF bundle is generated containing: the move-in condition report PDF, the move-out condition report PDF, and all payment receipt PDFs for the full tenancy duration

**Given** the final bundle PDF is generated
**When** generation completes successfully
**Then** it is uploaded to Azure Blob at `{tenancyId}/final-bundle/{uuid}.pdf`
**And** the tenancy `status` is updated to `Completed`
**And** the final bundle email (FR49) is sent to the former tenant via Resend with the PDF attached

**Given** PDF generation fails for any reason
**When** the failure is detected
**Then** the error is logged and generation is retried up to 3 times before surfacing the failure

**Given** an authenticated former tenant
**When** `GET /api/tenancies/{id}/final-bundle` is called within the 12-month read-only window
**Then** a signed Azure Blob URL (24-hour expiry) for the final bundle PDF is returned

---

### Story 6.3: Move-Out Condition Report & Tenancy Expiry Job

As a tenant and system,
I want to complete the move-out condition report using the same familiar flow, and have my account automatically expire after 12 months with no action required from the landlord,
So that the tenancy closes with a documented record and access ends cleanly.

**Acceptance Criteria:**

**Given** a move-out has been triggered and the move-out condition report is initiated
**When** the tenant opens the condition report
**Then** the pre-loaded baseline is the agreed move-in condition report items displayed as read-only
**And** the tenant can add new move-out items (photos + descriptions) using the same `FileUploadArea` and `ConditionReportItem` components from Epic 4

**Given** the tenant agrees to the move-out condition report
**When** `POST /api/condition-reports/{tenancyId}/agree` is called on the move-out report
**Then** the move-out condition report PDF is generated
**And** the final bundle generation is triggered (Story 6.2)

**Given** the `TenancyExpiryJob` `IHostedService` runs daily
**When** it finds a tenancy with `status = 'Completed'` and `completed_at <= now - 12 months` and tenant `account_state = 'ReadOnly'`
**Then** the tenant's `account_state` is set to `Expired`
**And** their `token_valid_from` is updated to the current UTC timestamp, revoking any remaining session
**And** no email or notification is sent — expiry is silent

**Given** a tenant with `account_state = 'Expired'`
**When** they attempt to log in
**Then** a 401 Unauthorized ProblemDetails response is returned indicating access has expired

---

### Story 6.4: Move-Out UI (Landlord Trigger + Former Tenant Read-Only Access & GDPR)

As a landlord and former tenant,
I want the move-out triggered with a clear confirmation modal, the former tenant able to access their read-only data, and the landlord able to delete profile data on GDPR request,
So that end of tenancy is handled professionally and compliantly for both parties.

**Acceptance Criteria:**

**Given** an authenticated landlord with an active tenancy
**When** they tap "End tenancy" on the property dashboard
**Then** a confirmation `AlertDialog` appears: "This begins the move-out process for [tenant name]. This cannot be undone." with Confirm / Cancel buttons

**Given** the landlord confirms move-out
**When** the modal is dismissed
**Then** the dashboard updates to show tenancy status as "Move-out in progress"
**And** the landlord can see that a move-out condition report is pending completion by the tenant

**Given** a former tenant with `account_state = 'ReadOnly'` logs in
**When** they access any screen
**Then** a persistent read-only banner is displayed: "Your tenancy has ended. You have read-only access until [expiry date]."
**And** all write actions (buttons, forms) are hidden or disabled

**Given** the former tenant navigates to payment history or condition reports
**When** the pages render
**Then** all historical data, receipts, and PDFs remain accessible via signed URLs (24-hour expiry)

**Given** a landlord receives a GDPR profile deletion request and calls `DELETE /api/tenants/{id}/profile`
**When** the request is processed
**Then** the tenant's profile data (name, email, phone, credentials) is deleted from `AspNetUsers`
**And** all tenancy record data (condition reports, bill periods, payments, maintenance requests) is retained
**And** a 204 No Content response is returned

**Given** profile data has been deleted
**When** tenancy records are queried by the landlord
**Then** tenancy records remain intact and queryable (orphaned profile link handled gracefully)

---

## Epic 7: Public Showcase Page

The public showcase page is live at `renteasy.bg` — a scroll-driven editorial experience (Playfair Display headings, full-bleed photos, parallax transitions). BG/EN language toggle. Apartment waitlist with live counter. Landlord interest CTA at footer with separate DB tag. Ghost login link. Sub-2-second FCP. Open Graph, structured data, and SEO metadata in place.

### Story 7.1: Waitlist API & Database

As a visitor,
I want to submit my email to the waitlist or landlord interest list and see a live counter of signups,
So that I can register my interest without friction.

**Acceptance Criteria:**

**Given** an unauthenticated visitor
**When** `POST /api/showcase/waitlist` is called with a valid email address and `type = 'tenant'`
**Then** a `WaitlistEntry` record is created with UUID `id`, email, `type = 'tenant'`, and `created_at`
**And** a 201 response is returned

**Given** `POST /api/showcase/waitlist` is called with `type = 'landlord'`
**Then** a `WaitlistEntry` record is created with `type = 'landlord'` (stored separately, never mixed with tenant entries)
**And** a 201 response is returned

**Given** the same email is submitted twice for the same type
**When** the duplicate is processed
**Then** a 200 OK response is returned (no error, no duplicate record — idempotent)

**Given** an invalid email address is submitted
**When** validation runs
**Then** a 400 Bad Request ProblemDetails response is returned

**Given** an unauthenticated visitor calls `GET /api/showcase/waitlist/count`
**When** the request is processed
**Then** the total tenant waitlist count is returned as a JSON number with no authentication required

**Given** the `WaitlistEntry` table
**When** `AppDbContext` query filters are applied
**Then** `HasQueryFilter` is NOT applied to `WaitlistEntry` — it is a public entity with no landlord scoping

---

### Story 7.2: Showcase Page Editorial Design

As a visitor,
I want to experience a scroll-driven editorial page that reveals the apartment room by room, with a host note and renovation story,
So that I understand the apartment and feel the landlord's care within the first scroll.

**Acceptance Criteria:**

**Given** a visitor arrives at `renteasy.bg`
**When** the page loads
**Then** First Contentful Paint occurs within 2 seconds on a simulated LTE/4G connection (Lighthouse mobile preset)
**And** the page is statically pre-rendered at build time (Next.js static export)

**Given** the hero section renders
**When** the visitor first sees the page
**Then** a full-bleed editorial photo fills the viewport
**And** the header contains only: logo (left), `BG / EN` language toggle, and "Tenant login →" ghost link (right) — no other navigation

**Given** the visitor scrolls
**When** they move through the page
**Then** the scroll sequence reveals: hero → living room editorial shots → bedroom shots → host note (Kiril's voice, ~4 sentences) → human specs (each number paired with its human meaning) → renovation story (before/after) → waitlist section

**Given** the page renders on desktop (≥1280px)
**When** section headings are displayed
**Then** Playfair Display is used at 64–80px for display headings
**And** parallax and fade transitions occur between sections

**Given** the page renders on mobile (390px)
**When** any section is displayed
**Then** all sections use full-width single-column layout with 64–96px vertical spacing between sections
**And** no parallax (performance constraint on mobile)

**Given** all images on the showcase page
**When** they are rendered
**Then** they use `next/image` with explicit `width`/`height`, `sizes` prop for responsive sizing, and lazy loading below the fold
**And** all meaningful images have descriptive `alt` text; decorative images use `alt=""`

---

### Story 7.3: Waitlist Form, Landlord Interest CTA & Ghost Login

As a visitor,
I want to submit my email to the waitlist and see the counter increment, and as a landlord visitor submit my interest separately,
So that I can register without friction and the page serves both audiences cleanly.

**Acceptance Criteria:**

**Given** the visitor scrolls to the waitlist section
**When** the section renders
**Then** the `ShowcaseWaitlistForm` displays the current signup count fetched from `GET /api/showcase/waitlist/count`
**And** a single email input and submit button are shown

**Given** the visitor enters a valid email and submits
**When** the form is submitted
**Then** the button is disabled during submission (no double-submit)
**And** on success: the counter increments by 1, the form is replaced by inline confirmation "You're on the list. We'll be in touch." — no page redirect, no scroll jump

**Given** the visitor enters an invalid email and submits
**When** client-side validation runs
**Then** an inline error message appears beneath the field in text (not colour alone)

**Given** the visitor scrolls to the footer
**When** the footer renders
**Then** a visually separated landlord CTA section shows: "Are you a landlord in Sofia?" with a separate email input
**And** submission calls `POST /api/showcase/waitlist` with `type = 'landlord'`
**And** its confirmation and counter are independent from the tenant waitlist

**Given** the showcase page header renders on desktop
**When** the page is viewed
**Then** "Tenant login →" is displayed as a text-only sticky link (12px Inter, no button treatment)
**And** on mobile it is present in the hero area

**Given** the visitor taps "Tenant login →"
**When** they are navigated
**Then** they are taken to `/[locale]/login`

---

### Story 7.4: SEO, Performance & Language Toggle

As a visitor and search engine,
I want the showcase page to be fully indexed, fast, and available in Bulgarian and English,
So that it ranks for relevant searches and serves both Bulgarian and international visitors.

**Acceptance Criteria:**

**Given** the showcase page is built
**When** Next.js runs a static export build
**Then** full HTML is generated at build time and delivered to crawlers without JavaScript execution required

**Given** the page is rendered for the Bulgarian locale
**When** the `<head>` is inspected
**Then** `<title>` and `<meta name="description">` are in Bulgarian
**And** `<html lang="bg">` is set
**And** Open Graph tags are present: `og:title`, `og:description`, `og:image` (apartment hero photo), `og:url`
**And** `schema.org/RealEstateListing` structured data is present with apartment details

**Given** the page is rendered for the English locale
**When** the `<head>` is inspected
**Then** `<title>` and `<meta name="description">` are in English
**And** `<html lang="en">` is set

**Given** all authenticated routes (`/login`, `/dashboard`, `/billing`, etc.)
**When** their `<head>` is inspected
**Then** `<meta name="robots" content="noindex">` is present

**Given** the visitor taps the `BG / EN` language toggle
**When** they switch language
**Then** the page content re-renders in the selected language
**And** the URL updates to the appropriate locale prefix (`/bg/` or `/en/`)

**Given** a sitemap is generated
**When** it is inspected
**Then** it contains only `renteasy.bg` — no authenticated routes

**Given** a Lighthouse mobile audit is run on the showcase page
**When** results are reviewed
**Then** Performance score is ≥ 90, Accessibility score is ≥ 90, and LCP is < 2 seconds
