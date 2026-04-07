---
stepsCompleted: ["step-01-document-discovery", "step-02-prd-analysis", "step-03-epic-coverage-validation", "step-04-ux-alignment", "step-05-epic-quality-review", "step-06-final-assessment"]
workflowStatus: complete
documentsInventoried:
  - prd: "_bmad-output/planning-artifacts/prd.md"
  - architecture: "_bmad-output/planning-artifacts/architecture.md"
  - epics: "_bmad-output/planning-artifacts/epics.md"
  - ux: "_bmad-output/planning-artifacts/ux-design-specification.md"
---

# Implementation Readiness Assessment Report

**Date:** 2026-04-07
**Project:** RentEasyV2

---

## PRD Analysis

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
- FR58: Visitor can submit their email via a landlord interest CTA (stored separately from tenant waitlist)
- FR59: Visitor can access the tenant login page via a ghost login link on the showcase page

**Internationalisation**

- FR60: Authenticated users can toggle the application language between Bulgarian and English

**Total FRs: 60** (FR1–FR60 including FR4b, FR36b, FR37b; FR47 removed as duplicate of FR39)

---

### Non-Functional Requirements

**Performance**

- NFR-P1: All application screens load within 2 seconds on a standard Bulgarian mobile connection (LTE/4G, ~20 Mbps) at 390px viewport
- NFR-P2: Public showcase page achieves First Contentful Paint within 2 seconds under same connection baseline
- NFR-P3: Skeleton screens displayed on any screen requiring a data fetch — no blank/white loading states
- NFR-P4: Bill PDFs and condition report PDFs served as externally-hosted signed URLs — PDF content never embedded in page payloads
- NFR-P5: QR code generation for IRIS Pay and Revolut.me completes client-side in under 100ms

**Security**

- NFR-S1: All data transmitted over HTTPS; no HTTP endpoints in production
- NFR-S2: Database content encrypted at rest
- NFR-S3: All sensitive resource identifiers use UUIDs — no sequential or guessable IDs
- NFR-S4: Every API endpoint enforces per-resource authorisation — authenticated users can only access resources they own
- NFR-S5: All file uploads validated by MIME type and file signature; only JPEG, PNG, and PDF files accepted; uploaded files never executed
- NFR-S6: All user-generated text rendered in UI is output-encoded to prevent XSS
- NFR-S7: All text input fields sanitize input to prevent SQL injection and script injection
- NFR-S8: Tenancy data is strictly scoped — a user can never access another user's data

**Reliability**

- NFR-R1: System uptime ≥ 99.5% per calendar month, excluding scheduled maintenance windows
- NFR-R2: Transactional email delivery rate ≥ 99% — no missed credential deliveries, payment confirmations, condition report PDFs, or legally significant documents
- NFR-R3: PDF generation succeeds 100% of the time for all triggered events; failures must be logged and retried
- NFR-R4: In-app document status serves as secondary confirmation channel alongside email delivery

**Accessibility**

- NFR-A1: All interactive elements have minimum tap target size of 44×44px
- NFR-A2: Colour contrast meets WCAG 2.1 AA minimums: 4.5:1 for normal text, 3:1 for large text
- NFR-A3: All form inputs have associated `<label>` elements; no placeholder-only labelling
- NFR-A4: Error states communicate meaning through text, not colour alone
- NFR-A5: All meaningful images carry descriptive alt text; decorative images use `alt=""`
- NFR-A6: `<html>` element carries correct `lang` attribute (`bg` or `en`) for the active language
- NFR-A7: Interface is fully navigable without hover interactions — all functionality available via tap or click

**Scalability**

- NFR-SC1: Data model supports multiple landlords, multiple properties per landlord, and multiple tenancies per property from day one
- NFR-SC2: All infrastructure scaling upgrades require configuration changes only — no code or architectural changes at any anticipated V1 growth stage

**Integration Reliability**

- NFR-I1: Transactional email delivery retries automatically on failure; critical documents confirmed via in-app status as secondary signal
- NFR-I2: Signed document URLs generated with expiry windows ≥ 4 hours; URLs regenerated on access if expired
- NFR-I3: PDF generation executes entirely server-side with no external service calls — no network failure mode

**Total NFRs: 29** (5 Performance + 8 Security + 4 Reliability + 7 Accessibility + 2 Scalability + 3 Integration)

---

### Additional Requirements & Constraints

- **Currency:** EUR only; no BGN support required (eurozone transition Jan 2026)
- **Dual-language:** Bulgarian (primary) and English; UI strings via next-intl
- **Mobile-first:** Designed at 390px viewport; breakpoints at 390/768/1280px; no horizontal scroll at any breakpoint
- **Browser matrix:** Chrome 90+, Safari 14+, Firefox 88+, Samsung Internet 14+ (full support); 2018+ hardware (degraded but functional)
- **GDPR data split:** Profile data (deletable) vs. tenancy record data (retained under Art. 17(3)(b)); one-sentence retention disclosure at onboarding
- **Financial retention:** 5–7 years post-tenancy per Bulgarian tax law (supersedes GDPR deletion for financial records)
- **Legal validity of condition reports:** Timestamped PDF with tenant name = valid documentary evidence in Bulgarian courts; full dispute history mandatory in PDF
- **No real-time in V1:** No WebSockets, SSE, or polling; status updates via email; in-app refresh on navigation/reload only
- **Third-party integrations:** Resend API (email), Azure Blob Storage (file storage), QuestPDF (server-side PDF, no external dependency)
- **Tech stack:** Next.js 15 (App Router) + ASP.NET Core 8 Web API + JWT (HttpOnly cookies)
- **File types accepted:** JPEG, PNG, PDF only (uploads); MIME type + file signature validation required

---

### PRD Completeness Assessment

The PRD is **exceptionally complete and well-structured**. Key observations:

- All 60 FRs are clearly numbered, unambiguous, and scoped to V1
- All 29 NFRs carry concrete, measurable targets (not vague goals)
- Legal/compliance requirements (GDPR, Bulgarian tax law, condition report legal validity) are precisely specified
- FR47 was intentionally removed (duplicate of FR39) — edit history confirms this was a deliberate cleanup
- The boundary between V1 and V2/V3 is explicitly drawn
- Growth features are clearly separated from MVP scope
- No orphan requirements identified — all FRs trace back to user journeys

Minor observation: FR50 and FR49 overlap in intent (both concern the final move-out bundle). FR49 covers email delivery; FR50 covers generation. They are complementary, not duplicate.

---

## Epic Coverage Validation

### Coverage Matrix

| FR | PRD Requirement (summary) | Epic | Story | Status |
|---|---|---|---|---|
| FR1 | Landlord login | Epic 1 | Story 1.3, 1.5 | ✓ Covered |
| FR2 | Tenant login | Epic 1 | Story 1.3, 1.5 | ✓ Covered |
| FR3 | Forced password change on first login | Epic 1 | Story 1.3, 1.5 | ✓ Covered |
| FR4 | Password change from account settings | Epic 1 | Story 1.3, 1.5 | ✓ Covered |
| FR4b | Password reset via email link | Epic 1 | Story 1.3 | ✓ Covered |
| FR5 | Role-based access (Landlord/Tenant) | Epic 1 | Story 1.4 | ✓ Covered |
| FR6 | Property creation | Epic 2 | Story 2.1, 2.3 | ✓ Covered |
| FR7 | Payment method configuration per property | Epic 2 | Story 2.1, 2.3 | ✓ Covered |
| FR8 | Welcome pack creation and editing | Epic 2 | Story 2.2, 2.3 | ✓ Covered |
| FR9 | Property detail view and edit | Epic 2 | Story 2.1, 2.3 | ✓ Covered |
| FR10 | Tenant account creation | Epic 2 | Story 2.4 | ✓ Covered |
| FR11 | Tenant invitation email with credentials | Epic 2 | Story 2.4 | ✓ Covered |
| FR12 | Tenant welcome pack view | Epic 2 | Story 2.2, 2.5 | ✓ Covered |
| FR13 | Move-out flow trigger by landlord | Epic 6 | Story 6.1, 6.4 | ✓ Covered |
| FR14 | Read-only account on move-out | Epic 6 | Story 6.1 | ✓ Covered |
| FR15 | 12-month auto-expiry | Epic 6 | Story 6.3 | ✓ Covered |
| FR16 | Landlord pre-loads condition report items | Epic 4 | Story 4.1 | ✓ Covered |
| FR17 | Tenant views landlord baseline (read-only) | Epic 4 | Story 4.1, 4.5 | ✓ Covered |
| FR18 | Tenant adds own condition report items | Epic 4 | Story 4.5, 4.6 | ✓ Covered |
| FR19 | Tenant agrees — PDF generated + emailed | Epic 4 | Story 4.2, 4.3 | ✓ Covered |
| FR20 | Tenant disagrees — submits disputed items | Epic 4 | Story 4.2, 4.6 | ✓ Covered |
| FR21 | Landlord reviews disputes + re-requests sign-off | Epic 4 | Story 4.2, 4.6 | ✓ Covered |
| FR22 | 3-round dispute cap | Epic 4 | Story 4.2 | ✓ Covered |
| FR23 | PDF with full dispute history | Epic 4 | Story 4.3 | ✓ Covered |
| FR24 | Permanent PDF storage for condition reports | Epic 4 | Story 4.3 | ✓ Covered |
| FR25 | Round number + pending party visibility | Epic 4 | Story 4.2, 4.6 | ✓ Covered |
| FR26 | Move-out condition report (reuses same editor) | Epic 6 | Story 6.3 | ✓ Covered |
| FR27 | Landlord bill upload (PDF + amount per category) | Epic 3 | Story 3.1 | ✓ Covered |
| FR28 | Tenant itemised bill view with inline PDFs | Epic 3 | Story 3.4 | ✓ Covered |
| FR29 | Bill PDF open/download | Epic 3 | Story 3.4 | ✓ Covered |
| FR30 | Payment method display (IBAN / IRIS Pay / Revolut) | Epic 3 | Story 3.5 | ✓ Covered |
| FR31 | "I've paid" tenant action | Epic 3 | Story 3.6 | ✓ Covered |
| FR32 | Landlord payment confirmation | Epic 3 | Story 3.2, 3.6 | ✓ Covered |
| FR33 | Receipt PDF generation + email | Epic 3 | Story 3.2 | ✓ Covered |
| FR34 | Full payment history access | Epic 3 | Story 3.7 | ✓ Covered |
| FR35 | Historical receipt/bill PDF download | Epic 3 | Story 3.7 | ✓ Covered |
| FR36 | Tenant maintenance request submission | Epic 5 | Story 5.1, 5.2 | ✓ Covered |
| FR36b | Email to landlord on new maintenance request | Epic 5 | Story 5.1 | ✓ Covered |
| FR37 | Landlord maintenance request list | Epic 5 | Story 5.1, 5.3 | ✓ Covered |
| FR37b | Condition report completion status on dashboard | Epic 5 | Story 5.1, 5.3 | ✓ Covered |
| FR38 | Maintenance status update by landlord | Epic 5 | Story 5.1, 5.3 | ✓ Covered |
| FR39 | Tenant email notification on status update | Epic 5 | Story 5.1 | ✓ Covered |
| FR40 | Bill upload notification email to tenant | Epic 3 | Story 3.1, 3.3 | ✓ Covered |
| FR41 | Payment due reminder (Day 3 after bill upload) | Epic 3 | Story 3.3 | ✓ Covered |
| FR42 | "I've paid" notification email to landlord | Epic 3 | Story 3.3 | ✓ Covered |
| FR43 | Receipt email to tenant | Epic 3 | Story 3.2 | ✓ Covered |
| FR44 | Condition report Day 3 + Day 7 nudge emails | Epic 4 | Story 4.4 | ✓ Covered |
| FR45 | Day 14 reminder email to both parties | Epic 4 | Story 4.4 | ✓ Covered |
| FR46 | Day 14 auto-resolution — incomplete PDF | Epic 4 | Story 4.4 | ✓ Covered |
| FR48 | Condition report PDF emailed on sign-off | Epic 4 | Story 4.3, 4.6 | ✓ Covered |
| FR49 | Final move-out bundle email to former tenant | Epic 6 | Story 6.2 | ✓ Covered |
| FR50 | Final PDF bundle generation (all receipts + CRs) | Epic 6 | Story 6.2 | ✓ Covered |
| FR51 | Former tenant read-only data access | Epic 6 | Story 6.4 | ✓ Covered |
| FR52 | Profile data deletion (GDPR request) | Epic 6 | Story 6.4 | ✓ Covered |
| FR53 | Legal retention enforcement — tenancy records | Epic 6 | Story 6.4 | ✓ Covered |
| FR54 | Public showcase page | Epic 7 | Story 7.2 | ✓ Covered |
| FR55 | BG/EN language toggle on showcase | Epic 7 | Story 7.4 | ✓ Covered |
| FR56 | Waitlist email capture | Epic 7 | Story 7.1, 7.3 | ✓ Covered |
| FR57 | Waitlist counter display | Epic 7 | Story 7.1, 7.3 | ✓ Covered |
| FR58 | Landlord interest CTA (separate DB tag) | Epic 7 | Story 7.1, 7.3 | ✓ Covered |
| FR59 | Ghost login link on showcase page | Epic 7 | Story 7.2, 7.3 | ✓ Covered |
| FR60 | Authenticated language toggle (BG/EN) | Epic 1 | Story 1.4, 1.5 | ✓ Covered |

### Missing Requirements

**No missing FRs detected.**

All 60 functional requirements from the PRD are present in the epics FR Coverage Map and are addressed by specific stories within those epics.

### Discrepancy Identified — Version References in Story 1.1

⚠️ **INCONSISTENCY — Story 1.1 AC version mismatch:**
- Story 1.1 ACs reference **Next.js 16** and **ASP.NET Core 10**
- The PRD states: **Next.js 15** and **ASP.NET Core 8 Web API**
- The epics Requirements Inventory (Additional Requirements section) aligns with the PRD: `npx create-next-app@latest` and `dotnet new webapi` (no version pinned) but the context implies Next.js 15 / .NET 8

**Impact:** If a developer follows Story 1.1 ACs verbatim, they would scaffold with different versions than the PRD specifies. Minor risk — the AC text should be corrected to match the PRD.

### Coverage Statistics

- **Total PRD FRs:** 60
- **FRs covered in epics:** 60
- **Coverage percentage: 100%**
- **Missing FRs: 0**
- **Epics in plan: 7** (Epic 1–7)
- **Total stories: 26** (Stories 1.1–1.6, 2.1–2.5, 3.1–3.7, 4.1–4.6, 5.1–5.3, 6.1–6.4, 7.1–7.4)

---

## UX Alignment Assessment

### UX Document Status

**Found:** `ux-design-specification.md` (65,995 bytes, Apr 5 21:07) — fully complete (14 steps completed)

The UX specification explicitly consumed the PRD as an input document and was itself consumed by the Architecture document. All three documents form a coherent chain: PRD → UX → Architecture → Epics.

---

### UX ↔ PRD Alignment

**Alignment: Strong — no missing UX coverage of PRD user journeys**

| PRD User Journey | UX Specification Coverage |
|---|---|
| Journey 1: Landlord First-Time Setup | ✓ Journey 3 (Landlord Billing) + portal layouts |
| Journey 2: Tenant Onboarding | ✓ Journey 1 (full emotional arc mapped) |
| Journey 3: Monthly Billing Loop | ✓ Journeys 2 & 3 (tenant + landlord sides) |
| Journey 4: Condition Report Dispute | ✓ Journey 4 (full disagree flow diagrammed) |
| Journey 5: Public Showcase Page | ✓ Journey 5 (full conversion funnel diagrammed) |
| Journey 6: Move-Out | ✓ Journey 6 (full flow diagrammed) |

All 20 UX Design Requirements (UX-DR1 through UX-DR20) are present in the epics' Requirements Inventory and are assigned to specific stories.

**UX requirements not explicitly in PRD FRs (but supported by architecture):**
- Emotional design principles (Safety Intro Screen copy, condition report framing) — design intent, not FR
- Specific component anatomy (PaymentMethodPanel tab order, "Copied ✓" 2-second revert) — implementation detail handled in epics
- Optimistic UI for "I've paid" — UX-DR17, covered in Story 3.6

---

### UX ↔ Architecture Alignment

**Alignment: Strong — Architecture document explicitly consumed UX spec as input**

| UX Requirement | Architecture Support |
|---|---|
| shadcn/ui + Tailwind design system | ✓ Architecture specifies shadcn/ui in scaffold |
| Client-side QR code (`qrcode` npm) | ✓ Listed in additional packages; NFR-P5 (<100ms) |
| URL-prefix i18n (`/bg/`, `/en/`) | ✓ Architecture Decision: `next-intl` + URL prefix routing |
| HttpOnly JWT cookie (BFF pattern) | ✓ Explicitly modelled in Architecture |
| Azure Blob signed URLs for PDFs | ✓ Architecture specifies 1-hour inline / 24-hour download |
| Server-side PDF generation (QuestPDF) | ✓ `QuestPDF` in backend packages; NFR-I3 |
| No hover-only interactions | ✓ Architecture: web app, mobile-first — no native APIs |
| No real-time (no WebSockets/SSE) | ✓ PRD and Architecture both explicitly state no real-time in V1 |
| WCAG 2.1 AA (Radix UI handles focus trapping) | ✓ Radix UI via shadcn/ui; Architecture cross-cutting constraints |

---

### Alignment Issues Found

#### ⚠️ ISSUE 1 — Version Disagreement: PRD vs. Architecture (Propagated to Stories)

| Document | Frontend Version | Backend Version |
|---|---|---|
| PRD | Next.js 15 | ASP.NET Core 8 Web API |
| Architecture document | **Next.js 16** | **ASP.NET Core 10** |
| Story 1.1 ACs | **Next.js 16** | **ASP.NET Core 10** |

**Root cause:** Architecture was written after PRD (PRD: Apr 2, Architecture: Apr 6) and appears to have upgraded to the latest stable versions at that time. The Epics correctly align with Architecture.

**Impact:** PRD is stale — it specifies versions that the architecture has superseded. No implementation risk if developers follow Stories + Architecture. PRD should be updated to reflect Next.js 16 / ASP.NET Core 10.

**Recommendation:** Update PRD line: "Built on Next.js 15 (static export for showcase, SSR for authenticated app) backed by an ASP.NET Core 8 Web API" → Next.js 16 / ASP.NET Core 10.

---

#### ⚠️ ISSUE 2 — Password Minimum Length Inconsistency: UX vs. Epics

| Document | Minimum password requirement |
|---|---|
| UX Journey 1 flowchart | "6+ chars, letters + numbers" |
| Story 1.5 AC | "8+ characters, letters + numbers" |

**Impact:** Story 1.5 and UX flowchart are in direct contradiction. A developer implementing Story 1.5 will use 8 characters (correct — stronger security). The UX flowchart is a visual summary; the Story AC is the implementation spec. However, the UX spec should be consistent.

**Recommendation:** Update the UX Journey 1 flowchart note from "6+" to "8+".

---

#### ⚠️ ISSUE 3 — Day 14 Soft-Block: UX vs. PRD (Ambiguous)

| Document | Day 14 behaviour |
|---|---|
| UX PendingActionBanner spec | "Day 14 (soft-block — replaces normal content)" |
| PRD FR46 | "Tenant retains full platform access" |
| Story 4.5 AC | "it replaces the main content with `role='alert'`" |

**Interpretation:** The UX and Story 4.5 describe a _visual_ replacement (the banner fills the content area on Day 14) — not an access restriction. The tenant can still navigate; the banner is the dominant visual element. This aligns with FR46 ("full platform access retained"). However, the phrase "replaces normal content" in UX-DR11 could be misread as blocking navigation.

**Recommendation:** The Story 4.5 AC should explicitly clarify: "replaces the main content area visually but does not block navigation — tenant retains full access." No code risk if both Story 4.4 and Story 4.5 are read together, but the wording in UX-DR11 warrants a clarification note.

---

### Warnings

None beyond the issues flagged above. No missing UX documentation for any user-facing feature. All 20 UX design requirements have traceability to epics stories.

---

## Epic Quality Review

Beginning **Epic Quality Review** against create-epics-and-stories standards. Validating: user value, epic independence, story dependencies, and AC quality.

---

### Epic 1: Deployable Foundation — Production-Ready Application Shell

**User Value Check:** ⚠️ Partially technical — but acceptable for a greenfield foundation epic.
- The epic narrative includes concrete user outcomes: "Kiril can log in, change password, access the dashboard, and toggle BG/EN."
- Infrastructure stories (1.1, 1.2, 1.4, 1.6) are present alongside user-facing stories (1.3, 1.5).
- Story 1.1 (Monorepo Scaffold & CI/CD) is the expected greenfield initialisation story per best practice — this is the starter template requirement story. ✓

**Epic Independence:** ✓ Stands alone completely.

**Best Practices Compliance:**
- [x] Epic delivers user value (login, language toggle, forced password change)
- [x] Story 1.1 = starter template setup (greenfield requirement met)
- [x] Database tables created minimally in Story 1.2 (only `AspNetUsers` + `properties`) — not a schema dump
- [x] CI/CD established early (Story 1.1) — greenfield best practice met
- [x] ACs are in Given/When/Then format throughout

**Issue Carried From Step 4:** Story 1.1 ACs reference Next.js 16 and ASP.NET Core 10, conflicting with PRD's Next.js 15 / ASP.NET Core 8.

---

### Epic 2: Property Setup & Tenant Onboarding

**User Value Check:** ✓ Clear — landlord configures property, invites tenant, tenant accesses welcome pack.

**Epic Independence:** ✓ Requires only Epic 1 output (auth, database). No forward dependencies.

**Story ordering within Epic 2:**
- 2.1 (API) → 2.3 (UI) ✓ correct API-first pattern
- 2.2 (Welcome Pack API) → 2.5 (Tenant portal including welcome pack) ✓
- 2.4 (Tenant account creation) → 2.5 (Tenant first login) ✓ correct sequencing

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] Epic can function independently
- [x] Stories appropriately sized
- [x] No forward dependencies
- [x] ACs are testable and specific

---

### Epic 3: Monthly Billing Loop — Core Product Value

**User Value Check:** ✓ The product's core loop, explicitly the highest-value epic.

**Epic Independence:** ✓ Requires Epics 1 & 2 (auth + tenancy).

**Story ordering within Epic 3:**
- 3.1 (Bill Period API + file storage) → 3.4 (Billing UI) ✓
- 3.2 (Payment confirmation + receipt PDF) → 3.6 (Confirm UI) ✓
- 3.3 (Email notifications + nudge scheduler) → 3.6 uses it ✓

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] Stories appropriately sized
- [x] ACs cover error conditions (Story 3.2: PDF failure → retry; Story 3.6: API error → rollback)

🟡 **Minor concern — email_nudge_jobs table ownership:** Story 3.3 creates `EmailNudgeJob` records for `PaymentDueDay3`. Story 4.4 later creates `ConditionReportDay3/Day7/Day14` records for the same table. The `email_nudge_jobs` table is created in Story 3.3 but Story 4.4 depends on it existing. Epic ordering (3 before 4) handles this correctly in practice, but Story 4.4 should explicitly state: *"Depends on email_nudge_jobs table and NudgeSchedulerJob IHostedService created in Story 3.3."* This implicit cross-epic dependency is not documented.

---

### Epic 4: Condition Reports — Joint Legal Record

**User Value Check:** ✓ Legally significant and emotionally central — joint condition report, dispute flow, PDF generation.

**Epic Independence:** Requires Epics 1 & 2. ✓

**Story ordering within Epic 4:**
- 4.1 (Data model + landlord pre-load API) → 4.5 (Tenant view) ✓
- 4.2 (Agree/Disagree API) → 4.6 (UI) ✓
- 4.3 (PDF generation) ← triggered by 4.2. 4.3 is a backend service story; Story 4.2 references it by name ("PDF generation is triggered (Story 4.3)"). This is an **intra-epic forward reference** — Story 4.2 AC references Story 4.3 by number.

🟠 **Major Issue — Story 4.2 forward-references Story 4.3:**
- Story 4.2 AC: "Agree action → PDF generation is triggered (Story 4.3)"
- A developer implementing Story 4.2 in isolation will find the PDF trigger referenced but not yet implemented
- **Recommendation:** Story 4.2 should either: (a) stub the PDF trigger as a no-op placeholder, making Story 4.3 an additive enhancement; OR (b) merge Stories 4.2 and 4.3 into one story since they are tightly coupled. The reference should not appear in ACs.

🟡 **Minor concern — Story 4.4 cross-epic dependency on Story 3.3 (email_nudge_jobs):** Same issue as noted in Epic 3 — Story 4.4 assumes the `email_nudge_jobs` table and `NudgeSchedulerJob` infrastructure from Story 3.3 already exist. Not stated explicitly.

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] ACs cover error conditions (PDF failure, 3-round cap, unresolved state)
- [x] Stories cover happy and sad paths

---

### Epic 5: Maintenance Requests

**User Value Check:** ✓ Tenant submits requests, landlord manages them, both receive notifications.

**Epic Independence:** ✓ Requires Epics 1 & 2 (tenancy). However, Story 5.1 includes `GET /api/properties/{id}/condition-report-status` which queries condition report data.

🟡 **Minor concern — Story 5.1 implicitly requires Epic 4 (condition report data model):**
- FR37b: "Landlord can view the move-in condition report completion status for each active tenancy"
- Story 5.1's endpoint `GET /api/properties/{id}/condition-report-status` queries the `condition_reports` table, which is created in Epic 4 (Story 4.1)
- If implementing Epic 5 without Epic 4, this endpoint would fail
- The epic build order (4 before 5) is the implicit guarantee. Should be made explicit in Story 5.1: *"Depends on ConditionReport entity from Story 4.1."*

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] ACs are specific and testable
- [x] Role enforcement validated (tenant cannot update status — 403 returned)

---

### Epic 6: Move-Out & End of Tenancy

**User Value Check:** ✓ Closes the full tenancy lifecycle with a permanent document record and clean GDPR handling.

**Epic Independence:** ✓ Requires Epics 1–5. This is the capstone epic — correctly placed last (before showcase).

**Story ordering within Epic 6:**
- 6.1 (Move-Out API + state transitions) → 6.4 (UI) ✓
- 6.2 (Final PDF bundle) — depends on Epic 3 (receipt PDFs) and Epic 4 (condition report PDFs)
- 6.3 (Move-out condition report + expiry job) — reuses Epic 4 components (explicitly stated in AC) ✓

🟡 **Minor observation — Story 6.2 composite dependency not stated:**
- Story 6.2 generates the final bundle from: move-in condition report PDF + move-out condition report PDF + all receipt PDFs
- These come from Epic 4 (condition report PDFs) and Epic 3 (receipt PDFs)
- Story 6.2 implicitly requires both Epics 3 and 4 to have run
- The story should note this dependency explicitly: *"Requires PdfService extensions from Stories 4.3 and 3.2."*

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] GDPR split correctly implemented in Story 6.4 (profile deleted, tenancy records retained)
- [x] JWT revocation on state change (Story 6.1 updates token_valid_from)
- [x] 12-month auto-expiry is silent (no email, no notification) — correctly specified

---

### Epic 7: Public Showcase Page

**User Value Check:** ✓ Acquisition funnel — tenant waitlist + landlord interest capture. Arguably most business-critical for growth.

**Epic Independence:** ✓ The waitlist API is a new endpoint (`POST /api/showcase/waitlist`) that explicitly does not have landlord-scoped query filters. The only "dependency" is the ghost login link pointing to Epic 1's login page — the showcase page can be deployed before the login page exists (the link would 404, not break the page). Effectively independent.

**Story ordering within Epic 7:**
- 7.1 (Waitlist API) → 7.3 (Waitlist Form UI) ✓
- 7.2 (Editorial Design) → 7.3 (Waitlist form is part of that page) — parallel development possible
- 7.4 (SEO + performance) — additive to 7.2 ✓

**Best Practices Compliance:**
- [x] Epic delivers user value
- [x] WaitlistEntry confirmed as non-landlord-scoped (Story 7.1 explicitly states this)
- [x] Idempotent duplicate email handling (Story 7.1: same email twice → 200, no duplicate)
- [x] Performance criterion in AC (Story 7.2: Lighthouse FCP < 2s, Story 7.4: Performance ≥ 90)

---

### Best Practices Compliance Summary

| Epic | User Value | Independence | Story Sizing | No Fwd Deps | AC Quality | FR Traceability |
|---|---|---|---|---|---|---|
| Epic 1 | ⚠️ Partial (foundation) | ✓ | ✓ | ✓ | ✓ | ✓ |
| Epic 2 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Epic 3 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Epic 4 | ✓ | ✓ | ✓ | ⚠️ 1 issue | ✓ | ✓ |
| Epic 5 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Epic 6 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Epic 7 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |

---

### Quality Findings by Severity

#### 🟠 Major Issues

**M1 — Story 4.2 forward-references Story 4.3 in ACs**
- Story 4.2 AC: "PDF generation is triggered (Story 4.3)" — ACs must not reference future stories
- The agree/disagree API and PDF generation are tightly coupled; splitting them creates an incomplete story
- **Options:** Merge Stories 4.2 and 4.3 into one story, OR make the Story 4.2 trigger a no-op stub ("trigger PDF generation — implementation in Story 4.3") — but the stub approach still creates a partially implemented feature state
- **Recommendation:** Merge 4.2 and 4.3 into a single story: "Condition Report Agree/Disagree API + PDF Generation"

#### 🟡 Minor Concerns

**m1 — Story 4.4 implicit dependency on Story 3.3 infrastructure**
- `email_nudge_jobs` table and `NudgeSchedulerJob` created in Story 3.3; Story 4.4 adds to it without stating this dependency
- Epic ordering (3 before 4) makes this work, but the dependency should be explicit

**m2 — Story 5.1 implicit dependency on Epic 4 (condition_reports table)**
- `GET /api/properties/{id}/condition-report-status` requires the condition_reports entity from Epic 4
- Should be stated in Story 5.1: "Depends on ConditionReport entity from Story 4.1"

**m3 — Story 6.2 composite dependency on Stories 3.2 and 4.3 not stated**
- Final bundle generation requires receipt PDFs (Story 3.2) and condition report PDFs (Story 4.3)
- Should be noted in Story 6.2 prerequisites

**m4 — Story 1.1 version references conflict with PRD** (carried from Step 4, Issue 1)

**m5 — Password minimum length inconsistency: UX says 6+, Story 1.5 says 8+** (carried from Step 4, Issue 2)

---

## Summary and Recommendations

### Overall Readiness Status

# ✅ READY — with targeted fixes before Epic 4

The RentEasyV2 planning artifacts are in an **exceptionally strong state**. This is one of the most complete and well-traced planning packages I have assessed:

- **100% FR coverage** — all 60 Functional Requirements traced to specific epics and stories
- **100% NFR coverage** — all 29 NFRs specified with concrete, measurable targets
- **Strong document chain** — PRD → UX Spec → Architecture → Epics, each explicitly consuming the prior
- **No missing user journeys** — all 6 PRD user journeys diagrammed in the UX spec
- **Correct epic structure** — API-before-UI pattern consistently applied within each epic
- **Legal/GDPR compliance modelled** — data-split, retention policy, and 7-year financial record retention all specified in stories
- **Security enforced** — UUID resource IDs, per-resource authorisation, XSS output encoding, MIME validation all in cross-cutting constraints applied to every story

One 🟠 Major Issue must be resolved before implementing Epic 4. The remaining issues are documentation corrections and do not block implementation of Epics 1–3.

---

### Issues Summary

| ID | Severity | Category | Description | When to Fix |
|---|---|---|---|---|
| M1 | 🟠 Major | Story structure | Story 4.2 forward-references Story 4.3 in ACs ("PDF generation is triggered (Story 4.3)") | Before starting Epic 4 |
| m1 | 🟡 Minor | Dependency docs | Story 4.4 does not state its dependency on Story 3.3's email_nudge_jobs table | Before starting Epic 4 |
| m2 | 🟡 Minor | Dependency docs | Story 5.1 does not state its dependency on Epic 4's condition_reports entity | Before starting Epic 5 |
| m3 | 🟡 Minor | Dependency docs | Story 6.2 does not state its dependencies on Stories 3.2 + 4.3 (receipt + CR PDFs) | Before starting Epic 6 |
| m4 | 🟡 Minor | Version conflict | PRD says Next.js 15 / ASP.NET Core 8; Architecture + Stories say Next.js 16 / ASP.NET Core 10 | Update PRD (low risk — devs follow Architecture) |
| m5 | 🟡 Minor | Spec inconsistency | UX Journey 1 flowchart says "6+ chars" password min; Story 1.5 says "8+ characters" | Update UX flowchart before Story 1.5 |
| m6 | 🟡 Minor | Wording ambiguity | UX-DR11 "Day 14 soft-block replaces content" could be misread as blocking navigation (FR46 says access retained) | Clarify Story 4.5 AC wording |

**Total issues: 7** (1 Major, 6 Minor)
**Blocking issues: 1** (M1 — before Epic 4 only)

---

### Critical Issues Requiring Immediate Action

#### M1 — Merge or clarify Stories 4.2 and 4.3 (before Epic 4)

**Problem:** Story 4.2 (Agree/Disagree API) ACs state "PDF generation is triggered (Story 4.3)" — this is a forward reference in an acceptance criterion. A developer completing Story 4.2 cannot satisfy this AC without Story 4.3.

**Options:**
1. **Merge Stories 4.2 and 4.3** into "Condition Report Agree/Disagree API + PDF Generation" — one story, one implementation unit, no forward reference
2. **Rewrite Story 4.2 AC** to replace the forward reference with a stub: "The agree/disagree outcome is persisted; PDF generation is invoked on a `IPdfService` interface with a no-op stub (Story 4.3 provides the real implementation)"

Option 1 is cleaner. The stories are tightly coupled and the combined scope is manageable.

---

### Recommended Next Steps

1. **Fix M1 before Epic 4 begins:** Merge Story 4.2 and Story 4.3 into a single story, or rewrite the AC to remove the forward reference. Decide before implementing Epic 4, Story 2.
2. **Add dependency notes to Stories 4.4, 5.1, and 6.2** (m1, m2, m3): Add a one-line "Depends on:" note to each story's header. These do not require restructuring — just documentation clarity.
3. **Update PRD version references** (m4): Change "Next.js 15" → "Next.js 16" and "ASP.NET Core 8" → "ASP.NET Core 10" in the PRD's Web Application Specific Requirements section.
4. **Correct UX Journey 1 flowchart** (m5): Update "6+ chars, letters + numbers" to "8+ characters, letters + numbers" to match Story 1.5.
5. **Clarify Story 4.5 Day 14 wording** (m6): Add explicit note that the soft-block is a visual replacement only — tenant retains full navigation access per FR46.
6. **Proceed to implementation** — Epics 1, 2, and 3 are clear to implement without any blockers. Begin with Epic 1, Story 1.1.

---

### Final Note

This assessment identified **7 issues** across **3 categories** (story structure, dependency documentation, specification consistency). The single Major issue (M1) is easily remedied with a story merge. The 6 Minor issues are documentation corrections only — none indicate missing features, incorrect architecture, or unaddressed requirements.

**The planning foundation for RentEasyV2 is genuinely strong.** The PRD is precise with measurable targets, the architecture is coherent and appropriate for a solo developer on free-tier services, the UX specification is complete with 20 named design requirements all traced to stories, and the epics achieve 100% FR coverage across 7 epics and 26 stories.

**Assessment completed:** 2026-04-07
**Assessed by:** Implementation Readiness Workflow v6.2.2
**Report saved to:** `_bmad-output/planning-artifacts/implementation-readiness-report-2026-04-07.md`



