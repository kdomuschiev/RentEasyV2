---
stepsCompleted: ["step-01-init", "step-02-discovery", "step-02b-vision", "step-02c-executive-summary", "step-03-success", "step-04-journeys", "step-05-domain", "step-06-innovation", "step-07-project-type", "step-08-scoping", "step-09-functional", "step-10-nonfunctional", "step-11-polish", "step-12-complete", "step-e-01-discovery", "step-e-02-review", "step-e-03-edit"]
workflowStatus: complete
completedAt: "2026-04-04"
lastEdited: "2026-04-06"
editHistory:
  - date: "2026-04-06"
    changes: "FR3 rewritten to capability format; FR41 timing trigger added (3 days); FR47 removed (duplicate of FR39); NFR-P4/P5/SC2/I1/I2/I3 implementation leakage cleaned up and metrics made concrete; Development Strategy section marked non-normative"
inputDocuments:
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2.md
  - _bmad-output/planning-artifacts/research/domain-private-landlord-tools-software-research-2026-04-02.md
  - _bmad-output/planning-artifacts/research/technical-renteasy-v1-stack-research-2026-04-02.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-01-now.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-01-tenant-ux.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-02-edge-cases.md
workflowType: 'prd'
classification:
  projectType: web_app
  domain: PropTech / Private Rental Management
  complexity: medium
  projectContext: greenfield
  notes: Multi-role (landlord/tenant), multi-tenant architecture from day one; no subscription management in V1
briefCount: 2
researchCount: 2
brainstormingCount: 3
projectDocsCount: 0
---

# Product Requirements Document - RentEasyV2

**Author:** Kiril
**Date:** 2026-04-02

## Executive Summary

RentEasy (renteasy.bg) is a mobile-first web application for private landlords and their tenants in Bulgaria. It replaces the opaque, friction-filled experience of paying rent and utility bills with the first transparency-first rental management platform in the Bulgarian market: itemised billing with attached utility PDFs, structured digital tenancy workflows, and a professional experience that neither side has had access to before.

The Bulgarian rental market is at an inflection point. Bulgaria joined the eurozone on January 1, 2026 — forcing landlords and tenants to re-denominate contracts and re-examine billing clarity. IRIS Pay A2A transactions grew tenfold in 2025; the digital payment infrastructure is mature. Sofia rents grew 16% in 2024. The infrastructure for a seamless digital rental experience is ready — yet no product treats billing transparency as a first-class feature.

RentEasy starts as a personal tool for one landlord and one apartment in Sofia, architected from the start to support multiple landlords and properties without rewrites. V1 covers the complete tenancy lifecycle: a public apartment showcase page, digital tenant onboarding with welcome pack, room-by-room move-in/move-out condition reports with iterative sign-off, monthly billing with itemised breakdowns and attached utility PDFs, three manual payment channels (IBAN, IRIS Pay, Revolut), landlord payment confirmation with auto-generated receipts, and maintenance request tracking. All documents are permanently stored; all notifications are email-delivered in V1.

**Target users:** Private landlords with 1–5 properties in Bulgaria who want to run their rentals professionally without it becoming a second job. Tenants aged 25–45, digitally comfortable, who have never been given a clear breakdown of what they're paying for.

### What Makes This Special

Rentila — the most mature Bulgarian landlord tool — does offer tenant accounts covering rent tracking, payments, receipts, and reminders. But it is built from the landlord's workflow outward: the tenant experience is a utility layer, not a designed product. What it lacks is billing transparency — no itemised charge breakdown, no attached utility bill PDFs, no condition report flow, no designed onboarding. Tenants can log in and see a payment status; they cannot understand what they're paying for or why.

RentEasy's differentiator is not the presence of a tenant portal. It's the depth and intent of it. The positioning that no competitor in the Bulgarian market occupies: the landlord who treats billing transparency as the product, not an afterthought.

Tenants who understand what they're paying for don't send dispute messages, don't question charges, and don't leave early. The product's core value to the landlord is zero payment-related messages per month. The value to the tenant is renting from someone who treats them as an adult.

**V2 to explore:** Inspired by Second Nature's resident onboarding platform — interactive welcome pack (apartment manual, contacts, rules as a designed mobile flow rather than a file attachment) and a move-in task checklist with automated reminders. Both deepen the onboarding experience beyond V1's email-delivered welcome pack.

## Project Classification

- **Project Type:** Web Application — mobile-first, multi-role (Landlord / Tenant), multi-tenant architecture from day one; no subscription management in V1
- **Domain:** PropTech / Private Rental Management
- **Complexity:** Medium — GDPR data-split retention policy, iterative condition report sign-off flow, multi-role authorization, PDF generation, dual-language (BG/EN)
- **Project Context:** Greenfield

## Success Criteria

### User Success

- Tenant pays every month without sending a single "what am I paying for?" message
- 100% of new tenants complete the move-in condition report within 14 days of first login
- All bill documentation — utility PDFs, receipts, condition reports — lives in the app permanently; zero files delivered via Viber or email
- Landlord spends less than 15 minutes per month on billing administration
- Tenant can access any historical receipt or bill PDF within 3 taps from any screen

### Business Success

- V1 (personal use): core billing loop proven — bill uploaded → tenant notified → tenant pays → receipt generated → zero confusion
- Platform milestone: first non-owner landlord successfully onboarded without direct assistance
- Tenant NPS measurably higher than industry baseline (measurement method TBD at scale)
- Month-over-month growth in properties under management once platform opens

### Technical Success

- Sub-2-second page load on a standard Bulgarian mobile connection (measured at 390px viewport)
- Monthly uptime ≥ 99.5% — no outages during bill upload or payment confirmation windows
- PDF generation reliability: 100% — every payment confirmation and every condition report produces a valid, downloadable PDF with no failures
- Email delivery rate ≥ 99% — no missed payment confirmations, condition report nudges, credential deliveries, or receipt emails

### Measurable Outcomes

| Metric | Target |
|---|---|
| Tenant billing questions per cycle | 0 |
| Documents delivered outside the app | 0 |
| Landlord billing admin time per month | < 15 min |
| Condition report completion rate (14 days) | 100% |
| PDF generation failure rate | 0% |
| Email delivery rate | ≥ 99% |
| Monthly uptime | ≥ 99.5% |
| Page load (mobile, standard BG connection) | < 2 seconds |

## Product Scope

### MVP — Minimum Viable Product

Both components ship together as a single release:

**Showcase Page**
- Public apartment showcase page at `renteasy.bg` — editorial design, mobile-first, Bulgarian and English
- Tenant waitlist form (email capture with seeded counter)
- Landlord interest CTA in footer (separate tag, same email collection mechanic)
- Ghost login link (top-right, text-only)

**Tenant Portal + Landlord Dashboard**
- Two-role authentication: Landlord and Tenant (JWT, ASP.NET Core Identity)
- Tenant onboarding: landlord creates account → email credential delivery + welcome pack → forced password change on first login
- Condition reports: landlord pre-loads 10–15 dispute-prone items → tenant adds layer on top → iterative sign-off (max 3 rounds) → timestamped PDF generated and emailed to both parties → full dispute history included in PDF
- Move-out condition report: mirrors move-in flow; both PDFs stored permanently as tenancy record
- Bill management: landlord uploads utility PDF + enters total per category (Rent, Electricity, Water, Building maintenance) → tenant sees itemised breakdown with attached PDFs inline
- Payment flow: IBAN, IRIS Pay (QR + phone number), Revolut.me (QR + link) displayed; tenant marks "I've paid" → status becomes "Pending confirmation" → landlord confirms → receipt PDF auto-generated bundling all bill PDFs
- Payment history: all months, all receipts and bill PDFs, permanently stored and downloadable
- Maintenance requests: tenant submits with photos → landlord notified by email → landlord updates status → tenant notified on resolution
- Post move-out: landlord triggers move-out → final PDF bundle emailed to former tenant → account downgrades to read-only → auto-expires after 12 months
- Email notifications: full V1 list (bill uploaded, payment due reminder, "I've paid" trigger to landlord, payment confirmed + receipt, condition report nudges Day 3/7/14, maintenance updates, credential delivery, all PDF documents)
- GDPR: data-split retention (profile data deletable; tenancy record data retained under Art. 17(3)(b)), one-sentence retention disclosure at onboarding, 30-day deletion response template
- Bulgarian + English i18n throughout; primary language Bulgarian
- Mobile-first responsive design — designed at 390px, adapted to desktop; 44px minimum tap targets; skeleton screens on data load
- Security: UUID resource IDs, per-resource authorization checks, file upload validation (JPEG/PNG/PDF only), input sanitization, output encoding, HTTPS, encrypted database at rest

### Growth Features (Post-MVP / V2)

- **Viber notifications** as parallel channel alongside email (pending Viber Business API partner agreement)
- **Interactive welcome pack** — Second Nature-inspired: apartment manual, contacts, rules as a designed mobile-first flow rather than email attachments
- **Move-in task checklist** — Second Nature-inspired: guided move-in tasks with automated reminders (set up electricity account, save emergency numbers, etc.)
- **Self-serve landlord registration** — public onboarding flow for other private landlords to join the platform
- **Stripe embedded payments** — requires EOOD company registration; auto-reconciliation replaces manual confirmation flow
- **Evrotrust QES digital signatures** — legally binding condition report sign-off; requires B2B commercial contract with Evrotrust
- **Multiple property showcase pages** — `renteasy.bg/apartments/[name]` structure introduced when second property arrives

### Vision (V3+)

- Lease and contract generation with e-signing
- Automated utility bill ingestion from providers
- Tax summary export for landlord income declarations (Bulgarian tax law)
- Tenant references and scoring
- Freemium model with paid automation tier
- Geographic expansion: Plovdiv, Varna
- Tenant screening and background checks

## User Journeys

### Journey 1: Landlord — First-Time Setup (Kiril)

**The situation:** The app is deployed. Kiril's landlord account is seeded. He logs in for the first time with his credentials. The dashboard is empty — no properties, no tenants. Before anything else can happen, he needs to set up his apartment.

**Rising action:** Kiril clicks "Add property." He enters the apartment name ("Lyulin"), the full address, size (65 m²), floor (3rd), and the three payment methods his tenants will use: his IBAN, his IRIS Pay phone number, and his Revolut.me link. He sets up the bill categories that apply: Rent, Electricity, Water, Building Maintenance Fee. Then he builds the welcome pack — apartment manual, utility provider contacts, building management phone number, WiFi name and password, garbage collection schedule, emergency numbers. He saves.

**Climax:** Kiril opens the Condition Report section and pre-loads the baseline. He photographs and notes the 12 items that matter: the small scratch on the kitchen cabinet, the older section of flooring in the hallway, the interior of the oven and fridge, the front door and lock, the windows, the renovated furniture piece. He marks each with a photo and a short note. The apartment is documented before a single tenant has stepped through the door.

**Resolution:** The property is ready. Kiril's dashboard shows one apartment, fully configured, awaiting its first tenant. When the contract is signed with the real estate agent next week, one click is all it takes to start the tenancy.

**Capabilities revealed:** Property creation and management, payment method configuration per property, bill category management, welcome pack editor (property-level), landlord-side condition report baseline upload, account settings with password change.

---

### Journey 2: Tenant Onboarding — Iva's First Days

**The situation:** Iva, 31, has just signed the rental contract with the real estate agent and handed over two months' deposit. She's moving in on Saturday. She's used to Viber messages from previous landlords — a total amount, no breakdown, sent whenever. She doesn't expect anything different.

**Rising action:** On Thursday evening, her phone shows an email from "Kiril via RentEasy." Subject: "Your apartment at [address] is ready." The sender name anchors her immediately — she knows Kiril. She opens it. One button: "Set up your account." She taps it. On first login she's prompted to set a new password — single field with an eye icon, no confirmation field. She saves. The app opens on a welcome screen.

**Climax:** She finds the welcome pack waiting — the apartment manual, the building management contact, the WiFi credentials, the garbage collection schedule. Everything she would have had to ask for over Viber is already here. Then she sees the condition report. The intro screen is unexpected: "This report protects you. Anything you photograph and note here is permanently recorded." She sees that Kiril has already documented 12 items — including a scratch on the kitchen cabinet she hadn't even noticed yet. She adds three things she spotted during move-in: a mark on the bathroom wall, a stiff window handle, a chip on the kitchen tiles. She hits Agree. A PDF is generated, timestamped with her name, emailed to both her and Kiril.

**Resolution:** Three days into her tenancy, Iva has a complete digital record of the apartment's condition at move-in — jointly documented, permanently stored, signed off without friction. She's renting from someone different, and she already knows it.

**Capabilities revealed:** Tenant invitation flow (landlord creates tenant account, sends invitation email), forced password change on first login, password change at any point (account settings), welcome pack display, condition report with landlord baseline + tenant layer, Agree flow, PDF generation and email delivery.

---

### Journey 3: The Monthly Billing Cycle (Core Loop)

**The situation:** It's the 1st of the month. Kiril has received the electricity and water bills in his mailbox. He opens the RentEasy dashboard on his phone.

**Rising action:** He uploads the electricity bill PDF, enters 19.58 EUR. Uploads the water bill, enters 10.32 EUR. Adds rent (700 EUR) and building maintenance fee (35 EUR) as fixed entries. He hits "Send bills." Iva receives an email: "Your monthly bills are ready." She opens RentEasy. One screen: four line items with totals, each with its PDF attached inline. She taps the electricity PDF — it opens. She taps IRIS Pay: Kiril's phone number and a QR code appear. She opens her bank app, scans the QR, enters 764.90 EUR manually, confirms. Back in RentEasy she taps "I've paid." Status changes to "Payment pending confirmation."

**Climax:** Kiril's bank app pings with a payment notification. He opens RentEasy, checks the amount matches, taps "Confirm payment." The app generates a PDF receipt — total paid, all four bill PDFs bundled — and emails it to Iva instantly. Her status updates to "Paid."

**Resolution:** The whole cycle took Kiril 8 minutes. Iva has a receipt in her inbox and the bill PDFs permanently in her payment history. She sent zero messages asking what she was paying for. This repeats every month.

**Capabilities revealed:** Bill upload (PDF + amount per category), tenant billing view with inline PDFs, payment method display (IBAN/IRIS Pay/Revolut), "I've paid" tenant action, landlord payment confirmation, receipt PDF generation and email, payment history.

---

### Journey 4: Condition Report Dispute — Edge Case

**The situation:** New tenancy, new tenant. The tenant opens the condition report on first login, reviews Kiril's 12 pre-loaded items, and disagrees. There's a damp patch on the bedroom wall that Kiril's photos don't capture.

**Rising action:** The tenant clicks "Disagree." The app prompts them to upload photos and a written description of disputed items. They photograph the damp patch, write two sentences. The app adds their items to the conditions list alongside Kiril's baseline, and sends Kiril an email notification. Kiril logs in, reviews the photos. He agrees the patch exists — it's minor but real. He accepts the addition and re-requests tenant sign-off. The tenant reviews the updated list — their item is now included — and clicks Agree. PDF generated. Both sides emailed. Round 1, resolved.

**Alternative climax (3 rounds, no agreement):** After three rounds of documented disagreement, the app stops the loop. Both positions are preserved in full — all photos, all notes, all timestamps, all rounds. The PDF records the unresolved state explicitly. No escalation path in V1; if it reaches a court, the paper trail speaks for itself.

**Resolution:** Either the dispute is resolved and both parties have a jointly agreed PDF, or the dispute is permanently documented with complete transparency. Either outcome is legally defensible.

**Capabilities revealed:** Disagree flow (tenant photo upload + description), landlord review of disputed items, re-request sign-off, round counter and 3-round cap, unresolved state documentation, full dispute history in final PDF, round/status visibility for both parties.

---

### Journey 5: Public Visitor — The Showcase Page

**The situation:** Ana is 28, looking for a 2BR apartment in Sofia. A friend mentions Lyulin — cheaper than centre, new metro station coming. She searches and finds renteasy.bg.

**Rising action:** The page loads in under 2 seconds. A full-screen editorial photo — the renovated kitchen in soft north light. She scrolls. Room by room the apartment reveals itself. A before/after of the renovation. A short note from Kiril in his own voice. Honest specs: "65 m² — enough space for two people who value having their own room." A paragraph about the neighbourhood that names Lyulin directly, then explains why it's a forward bet. The new metro station. The Ring Road.

**Climax:** She reaches the waitlist section. A counter shows 7 people have already signed up. The copy reads: "The apartment is currently rented. Leave your email — you'll be the first to know when it's available." She enters her email. One field, one tap.

**Resolution:** Ana is on the list. Kiril's database has one more warm lead with zero effort on his part. At the footer, a separate quiet line: "Are you a landlord in Sofia?" — she ignores it, but the next person who lands on this page is a landlord who's been looking for exactly this.

**Capabilities revealed:** Public showcase page (editorial layout, BG/EN toggle, ghost login), waitlist email capture with seeded counter, landlord interest CTA (separate audience tag), no login required.

---

### Journey 6: Move-Out — End of Tenancy

**The situation:** Iva's tenancy ends. Contract has concluded externally. Kiril logs into the dashboard and triggers the move-out flow.

**Rising action:** Kiril clicks "End tenancy" on Iva's property. The app initiates the move-out condition report — same structure as move-in, pre-loaded with the agreed move-in state as baseline. Iva receives an email prompting her to complete it. She documents the apartment room by room: mostly matching move-in, one noted difference — the stiff window handle was never fixed. She hits Agree. The app generates the move-out PDF.

**Climax:** The system auto-generates the final PDF bundle — move-in condition report, move-out condition report, and all payment receipts for the full tenancy — and emails it to Iva's address. Her account is immediately downgraded to read-only. She can view and download her own payment history and condition reports for 12 months. After 12 months, access auto-expires with no action required from Kiril.

**Resolution:** The tenancy is closed cleanly. Both parties have the complete documentary record. Kiril's dashboard shows the property as vacant, ready for the next tenancy setup. Iva's last interaction with RentEasy is receiving a comprehensive PDF bundle — a professional close to a professional tenancy.

**Capabilities revealed:** Move-out trigger (landlord), move-out condition report (mirrors move-in), final PDF bundle generation (all receipts + both condition reports), post-move-out read-only access, 12-month auto-expiry.

## Domain-Specific Requirements

### Compliance & Regulatory

**GDPR (EU Regulation 2016/679)**
- Legal bases: Art. 6(1)(b) — contract performance (covers standard rental data); Art. 6(1)(f) — legitimate interest (utility and damage documentation)
- No consent required for standard rental data — contractual basis is stronger and more appropriate
- Data split with different retention treatment:
  - *Profile data* (name, email, phone, credentials) — deletable on request
  - *Tenancy record data* (condition reports, payment history, receipts, bill PDFs) — retained under Art. 17(3)(b) exemption (legal claims defence) + Bulgarian tax law; not deletable on request
- Privacy notice: one-page notice given to tenant at contract signing (before app access), covering retention periods and legal bases in plain language
- One sentence shown during tenant onboarding: "Your tenancy records are kept for 7 years after move-out as required by Bulgarian law."
- Deletion request response: written response template within 30 days, stating what was deleted and what was retained and why — template must exist before launch
- Internal record of processing activities (private document, never filed)
- No registration with КЗЛД required (mandatory registration abolished May 2018)
- No DPO required at this scale
- Data breach plan: notify КЗЛД within 72 hours if breach poses risk to tenants

**Bulgarian Tax Law — Data Retention**
- Financial records (receipts, payment history, bill PDFs) retained for active tenancy + minimum 5–7 years post-tenancy
- This retention period supersedes any GDPR deletion request for financial records

**Condition Report — Legal Validity**
- V1 sign-off method: email delivery of timestamped PDF with tenant name explicitly recorded = legally valid documentary evidence in Bulgarian courts
- Full dispute history (all rounds, all photos, all notes, all timestamps) must be included in the final PDF — not just the agreed outcome
- Unresolved disputes (after 3 rounds) remain as open records with both positions fully documented

**Eurozone Transition (January 2026)**
- All amounts displayed in EUR; no BGN support required
- Existing contracts re-denominated externally; app operates in EUR from day one

### Technical Constraints

**Security** — all security requirements are non-negotiable pre-launch. The app handles personal data, financial records, and legally significant documents. Full measurable specifications are in the Non-Functional Requirements section (NFR-S1 through NFR-S8).

**File storage** — permanent storage of all uploaded and generated files is a product promise, not merely a technical requirement. Utility bill PDFs, condition report photos, receipts, and condition report PDFs must remain accessible for the full legal retention period. All files encrypted at rest (Azure Blob Storage).

**PDF generation** — every generated PDF (receipts, condition reports, move-out bundles) must be valid, downloadable, and contain the full content specified in each FR. Condition report PDFs must include complete dispute history — all rounds, all photos, all notes, all timestamps, and final status. QuestPDF, server-side, no external dependency.

**Email delivery** — all legally significant documents (condition reports, receipts, final move-out bundle) are delivered via email as the permanent, retrievable record. Email is not a notification channel — it is the delivery mechanism for legal documents. Resend API, 3,000/month free tier (sufficient for V1 scale).

### Integration Requirements

**Payment display (no gateway integration in V1)**
- IBAN: account number + payment reference displayed; one-tap clipboard copy
- IRIS Pay: landlord phone number + QR code displayed; tenant pays via their banking app
- Revolut.me: link + QR code displayed; tenant enters amount manually
- No automated reconciliation — landlord manually confirms receipt after bank notification

**Third-party services**
- Email: Resend API (transactional email, PDF attachments, nudges)
- File storage: Azure Blob Storage SDK (upload, retrieve, signed URL serving)
- PDF generation: QuestPDF library (server-side, no external dependency)

### Risk Mitigations

| Risk | Mitigation |
|---|---|
| Tenant data accessed by wrong user via URL manipulation | UUID resource IDs + per-resource authorization checks on every endpoint |
| Uploaded file is malicious (not an image/PDF) | MIME type + file signature validation; files never executed |
| GDPR deletion request conflicts with legal retention obligation | Data split policy documented; template response prepared before launch; privacy notice states split clearly |
| Condition report PDF challenged in court | Full dispute history in PDF; tenant name and timestamp explicitly recorded; email delivery creates independent paper trail |
| Email not delivered (missed receipt, missed nudge) | Resend delivery monitoring; retry on failure; critical documents (receipts, condition report PDFs) confirmed via in-app status as well as email |
| Payment amount wrong (tenant over/under pays) | App records what landlord confirms as received, not what was owed; wrong amounts resolved outside app via bank statements; no automated reconciliation |

## Innovation & Novel Patterns

### Detected Innovation Areas

**1. Transparency as the product (market positioning innovation)**
Every competitor in the Bulgarian private rental market — and the overwhelming majority globally — builds billing as a mechanism: enter your payment here. RentEasy inverts this: the itemised breakdown with attached utility bill PDFs *is* the experience. The payment method is secondary. No product in the Bulgarian market has made "understanding what you're paying for" the core UX, not a feature.

**2. Condition report as joint record (UX approach innovation)**
The standard condition report model is adversarial: landlord documents the apartment, tenant signs it (or disputes it with no structure). RentEasy reframes it as a joint record — the landlord pre-loads dispute-prone items before the tenant's first login, the tenant adds their layer on top, and neither party can delete the other's contributions. The final PDF is a conversation, not a monologue. This is structurally different from any condition report flow in existing tools and is both legally stronger and relationally healthier.

**3. Dual-audience single-page conversion (growth strategy innovation)**
The showcase page simultaneously serves two completely different audiences — potential tenants (waitlist) and potential future platform landlords (interest capture) — from a single public page with no landing page infrastructure. The tenant showcase page is secretly the B2B sales page. Both funnels collect email with different tags, validating two sides of a future marketplace before any marketplace is built.

**4. Trust as explicit UX design principle**
The design rule that emerged from the tenant UX brainstorming session: "Every piece of friction in this app should feel like it's protecting the tenant, never like it's serving the landlord." This is not a tagline — it's an architectural constraint that governs feature decisions. The Safety Intro Screen on the condition report, the landlord pre-loading items before the tenant moves in, the PDF permanence promise — all are expressions of this principle. It is unusual enough in the landlord software space to constitute a genuine UX innovation.

### Market Context & Competitive Landscape

- Global trend confirmed by research: "Tenant Experience" has only just been named as a product category in 2025 (Second Nature's "Resident Experience Platform"). The market has barely begun recognising this layer.
- Bulgarian incumbent (Rentila) has tenant accounts but no designed tenant experience — the gap is depth of intent, not feature presence.
- The tenant-centric quadrant in Bulgarian private rental management is uncontested. The innovation is not just building something better — it's building in a space no one has deliberately occupied.

### Validation Approach

- **Billing transparency**: V1 success metric is zero "what am I paying for?" messages from tenant per billing cycle. Binary, measurable from month one.
- **Condition report as joint record**: Validated by tenant completing the report within 14 days (100% target) and no legal disputes arising from ambiguous documentation.
- **Dual-audience showcase**: Validated by landlord interest emails collected before any platform features are built. If no landlords sign up from the footer CTA, the market-side hypothesis needs revisiting.
- **Trust as UX principle**: Validated qualitatively — does the tenant recommend the app to others? Would they choose a different apartment if it used RentEasy vs. a landlord with no system?

### Risk Mitigation

| Innovation Risk | Mitigation |
|---|---|
| Transparency approach doesn't reduce tenant questions | Billing loop is measurable from month one; if messages continue, investigate whether PDFs are actually being opened/read |
| Condition report joint model creates more disputes, not fewer | 3-round cap prevents infinite loops; unresolved state is still a documented outcome; full history protects both parties |
| Dual-audience page confuses primary audience (tenants) | Landlord CTA is footer-only, visually separated, quiet tone; tenant journey is never interrupted |
| "Trust-first" UX principle slows landlord workflows | Every friction introduced must pass the test: does it protect the tenant, or just slow the landlord? If only the latter, remove it |

## Web Application Specific Requirements

### Project-Type Overview

RentEasy is a multi-page web application with two distinct rendering contexts:

- **Showcase page** (`renteasy.bg`) — publicly accessible, statically pre-rendered at build time for SEO and performance. No authentication required.
- **Application** (tenant portal + landlord dashboard) — server-rendered, behind authentication. No SEO requirement. Mobile-first design throughout.

Built on Next.js 16 (static export for showcase, SSR for authenticated app) backed by an ASP.NET Core 10 Web API. No native mobile apps in V1 — the web app is the product on all devices.

### Browser Matrix

| Browser | Minimum Version | Platform | Support Level |
|---|---|---|---|
| Chrome | 90+ | Android | Full support |
| Safari | 14+ (iOS 14+) | iPhone / iPad | Full support |
| Chrome | 90+ | Desktop | Full support |
| Safari | 14+ | macOS | Full support |
| Firefox | 88+ | Desktop | Full support |
| Samsung Internet | 14+ | Android | Full support |
| Other browsers (2018+ hardware) | — | Any | Degraded but functional — no broken layouts, no inaccessible features |

JavaScript is required. No progressive enhancement for JS-disabled environments.

### Responsive Design

- **Design-first viewport:** 390px (iPhone 14 / common Android baseline) — every screen designed and tested here first
- **Breakpoints:** 390px (mobile) → 768px (tablet) → 1280px (desktop)
- **No horizontal scroll** at any breakpoint
- **Minimum tap target:** 44×44px on all interactive elements (buttons, links, form fields)
- **Touch-friendly:** no hover-only interactions; all UI functions available via tap
- **Showcase page:** scroll-driven editorial layout — designed as a narrative sequence, not a grid. Parallax and fade transitions between sections. No global navigation — header contains logo, language toggle, and ghost login link only; no menu, no links, no distractions.
- **App screens:** single-column mobile layout; wider layouts on tablet/desktop where it aids readability (billing history, condition report)

### Performance Targets

- **Showcase page:** sub-2-second First Contentful Paint on standard Bulgarian mobile connection (LTE/4G, ~20 Mbps)
- **App screens:** sub-2-second load on same connection baseline
- **Skeleton screens:** displayed on any screen where data must be fetched — no blank white screens
- **Images:** responsive sizing with `srcset`; lazy loading below the fold; WebP with JPEG fallback
- **PDF serving:** streamed from Azure Blob via signed URLs — no PDF content embedded in page payloads

### SEO Strategy

**Showcase page only** (app is behind authentication — no SEO requirement):

- Static pre-rendering at build time (Next.js static export) — full HTML delivered to crawlers
- `<title>` and `<meta description>` in Bulgarian (primary) and English (secondary) via next-intl
- Open Graph tags for social sharing (apartment photo, description, title)
- Structured data: `schema.org/RealEstateListing` for apartment details
- Canonical URL: `renteasy.bg` (root domain)
- Sitemap: single-page sitemap (`renteasy.bg` only in V1)
- Performance is SEO: sub-2s FCP directly supports Core Web Vitals ranking signals
- No SEO requirements for `/login`, `/dashboard`, or any authenticated routes — these should be `noindex`

### Accessibility Level

- **Target:** WCAG 2.1 AA (practical compliance, not certification)
- **Colour contrast:** minimum 4.5:1 for normal text, 3:1 for large text
- **Focus indicators:** visible keyboard focus on all interactive elements
- **Form labels:** all inputs have associated `<label>` elements; no placeholder-only labelling
- **Error messages:** descriptive, not just colour-coded (accessible to colour-blind users)
- **Images:** `alt` text on all meaningful images; decorative images marked `alt=""`
- **Language:** `lang="bg"` or `lang="en"` set on `<html>` per active language
- **Screen readers:** semantic HTML throughout; ARIA attributes only where native semantics are insufficient
- **No accessibility audit required pre-launch** — practical AA compliance is the standard; formal audit is a V2 consideration

### Implementation Considerations

**Frontend stack:**
- Next.js 16 with App Router
- next-intl for Bulgarian/English i18n — type-safe, single build artifact, runtime language switching
- React component library: to be decided during architecture phase (options: shadcn/ui, Radix UI, or custom)
- No heavy client-side state management needed in V1 — server components + minimal client state

**Backend integration:**
- ASP.NET Core 10 Web API — REST, JSON
- Authentication: ASP.NET Core Identity + JWT; Next.js stores token in HttpOnly cookie, forwards as Bearer header to API
- File uploads: direct to ASP.NET Core API endpoint → proxied to Azure Blob Storage
- PDF serving: signed Azure Blob URLs returned by API; client opens/downloads directly from Blob

**No real-time requirements in V1:**
- No WebSockets, no Server-Sent Events, no polling
- All status updates (payment confirmation, maintenance resolution) delivered via email
- In-app status refreshes on page navigation or manual reload only

**i18n architecture:**
- Language stored in URL prefix (`/bg/`, `/en/`) or user preference cookie — TBD during architecture
- All user-generated content stored as-entered (no server-side translation)
- UI strings translated via next-intl message files; Bulgarian is default

## Development Strategy & Risk Mitigation

*This section is informational for V1 solo dev execution — not a product requirement.*

**MVP approach:** Experience MVP — a complete, polished tenancy loop for one landlord and one apartment. Validation: "does this experience make the tenant stop sending payment questions?" Measurable from month one. Solo developer (Kiril), free-tier-first, vertical slice build order: billing loop → condition report → showcase page. Each slice is independently testable.

**Build order rationale:** Billing loop delivers core value fastest. Condition report is the most complex feature. Showcase page can be built in parallel once core app is stable.

### Risk Mitigation Strategy

**Technical risks:**

| Risk | Likelihood | Mitigation |
|---|---|---|
| Condition report PDF complexity (multi-round dispute history, photos, timestamps) | Medium | QuestPDF handles complex layouts well; build and test PDF generation early in development — not last |
| Azure Blob signed URL expiry causing broken PDF links | Low | Set generous expiry windows; regenerate on access if needed |
| Email delivery failures for legally significant documents | Low | Resend monitoring + retry; in-app status as secondary confirmation |
| Solo dev bandwidth — MVP scope too large to ship in one piece | Medium | Build in vertical slices: billing loop first (highest value), condition report second, showcase page third — each slice is shippable and testable independently |

**Market risks:**

| Risk | Mitigation |
|---|---|
| Tenant doesn't engage with condition report despite 14-day escalation | Soft block at Day 14 is the hard stop; landlord dashboard shows completion status so Kiril can send a personal nudge before the wall hits |
| Tenant ignores billing emails, still asks questions via Viber | In-app billing view is the primary experience; if Viber questions continue after month 1, investigate whether email is being filtered or PDFs aren't opening |
| Showcase page generates no waitlist signups | No conversion target set — page serves awareness and landlord interest validation. Low-risk in V1. |

**Resource risks:**

| Risk | Mitigation |
|---|---|
| Solo dev — feature gets stuck or takes longer than expected | Vertical slice approach allows partial launch; billing loop alone delivers core value |
| Free tier limits hit earlier than expected (Resend 3,000/month, Neon, Azure Blob) | Monitoring from day one; upgrade paths are configuration-only, no architecture changes needed |

## Functional Requirements

### Authentication & Account Management

- **FR1:** Landlord can log in with email and password
- **FR2:** Tenant can log in with email and password
- **FR3:** System prevents tenant from accessing any feature before changing their temporary password on first login
- **FR4:** Any authenticated user can change their password at any time from account settings
- **FR4b:** Any user can request a password reset from the login screen via email link, without requiring an active session
- **FR5:** System enforces role-based access — Landlord and Tenant roles have separate, non-overlapping capability sets

### Property Management

- **FR6:** Landlord can create a property with name, address, size, floor, and applicable bill categories
- **FR7:** Landlord can configure payment methods per property: IBAN, IRIS Pay phone number, and Revolut.me link
- **FR8:** Landlord can create and edit the property welcome pack (apartment manual, utility provider contacts, building management contact, WiFi credentials, garbage collection schedule, emergency numbers)
- **FR9:** Landlord can view and edit any property detail at any time

### Tenant Management

- **FR10:** Landlord can create a tenant account for a property by providing tenant name and email address
- **FR11:** System sends a tenant invitation email containing login credentials and the property welcome pack upon account creation
- **FR12:** Tenant can view the welcome pack for their property at any time after first login
- **FR13:** Landlord can trigger the move-out flow for an active tenancy
- **FR14:** System automatically restricts a former tenant account to read-only access to their own tenancy data immediately upon move-out
- **FR15:** System automatically revokes former tenant access 12 months after move-out with no manual action required

### Condition Reports

- **FR16:** Landlord can pre-load condition report items (photos + notes per item) for a property before tenant first login
- **FR17:** Tenant can view landlord-pre-loaded condition report items as read-only content on first login
- **FR18:** Tenant can add their own condition report items (photos + notes) on top of the landlord baseline
- **FR19:** Tenant can agree to the condition report, triggering timestamped PDF generation with tenant identity explicitly recorded, and email delivery to both parties
- **FR20:** Tenant can disagree with the condition report and submit disputed items with photos and written descriptions
- **FR21:** Landlord can review disputed items and re-request tenant sign-off with or without accepting the disputed items
- **FR22:** System enforces a maximum of 3 dispute rounds; after 3 rounds without agreement, the dispute is recorded as unresolved with both parties' positions permanently documented
- **FR23:** System generates a condition report PDF containing the full dispute history — all rounds, all photos, all notes, all timestamps from both parties, and final status (agreed or unresolved)
- **FR24:** System stores condition report PDFs permanently and makes them accessible to both landlord and tenant
- **FR25:** Both landlord and tenant can see the current dispute round number, which party's action is pending, and what disputed items remain outstanding
- **FR26:** Landlord can complete a move-out condition report using the same shared condition report editor as move-in

### Bill Management & Payment Flow

- **FR27:** Landlord can upload a utility bill PDF and enter a total amount per bill category for a billing period
- **FR28:** Tenant can view an itemised breakdown of all charges for the current billing period with each bill PDF accessible inline
- **FR29:** Tenant can open and download any attached bill PDF directly from the billing screen
- **FR30:** Tenant can view all available payment methods for their property (IBAN with one-tap clipboard copy, IRIS Pay QR code and phone number, Revolut.me QR code and link)
- **FR31:** Tenant can mark a payment as made, changing the billing status to "Payment pending confirmation"
- **FR32:** Landlord can confirm a tenant payment, recording the amount confirmed as received
- **FR33:** System generates a receipt PDF upon payment confirmation, bundling the total confirmed amount with all bill PDFs for that billing period, and emails it to the tenant
- **FR34:** Tenant can view their complete payment history across all past billing periods
- **FR35:** Tenant can download any historical receipt or bill PDF at any time during and after their tenancy (within the read-only access window)

### Maintenance Requests

- **FR36:** Tenant can submit a maintenance request with a title, description, and one or more photos
- **FR36b:** System sends email to landlord when tenant submits a new maintenance request
- **FR37:** Landlord can view all maintenance requests for a property, each showing current status and submission details
- **FR37b:** Landlord can view the move-in condition report completion status for each active tenancy (not started / in progress / complete)
- **FR38:** Landlord can update the status of a maintenance request (received / in progress / resolved)
- **FR39:** System notifies tenant by email when the landlord updates the status of their maintenance request

### Notifications & Communications

- **FR40:** System sends email to tenant when landlord uploads bills for a billing period
- **FR41:** System sends a payment due reminder email to tenant 3 days after bills are uploaded if no payment has been marked as made
- **FR42:** System sends email to landlord when tenant marks a payment as made
- **FR43:** System sends email to tenant with receipt PDF attached when landlord confirms a payment
- **FR44:** System sends email nudge to tenant at Day 3 and Day 7 if move-in condition report is not completed
- **FR45:** System sends email reminder to both tenant and landlord at Day 14 if move-in condition report is not completed
- **FR46:** If the move-in condition report is not completed by Day 14, the system auto-resolves it as incomplete, generates a PDF documenting all items recorded to that point, and emails it to both tenant and landlord. Tenant retains full platform access.
- **FR48:** System emails condition report PDFs to both landlord and tenant upon sign-off
- **FR49:** System emails the final move-out PDF bundle (all receipts + both condition report PDFs) to the former tenant upon move-out completion

### Data Management & GDPR

- **FR50:** System generates a final PDF bundle containing all payment receipts and both condition report PDFs upon move-out
- **FR51:** Former tenant can view and download their own payment history and condition reports during the 12-month read-only access period
- **FR52:** Landlord can delete a tenant's profile data (name, email, phone, credentials) on request
- **FR53:** System retains all tenancy record data (condition reports, payment history, receipts, bill PDFs) regardless of profile deletion requests, per legal retention requirements

### Public Showcase Page

- **FR54:** Any visitor can view the public apartment showcase page without authentication
- **FR55:** Visitor can toggle the showcase page between Bulgarian and English
- **FR56:** Visitor can submit their email address to join the apartment waitlist
- **FR57:** Showcase page displays the current waitlist subscriber count
- **FR58:** Visitor can submit their email via a landlord interest call-to-action (stored separately from the tenant waitlist)
- **FR59:** Visitor can access the tenant login page via a ghost login link on the showcase page

### Internationalisation

- **FR60:** Authenticated users can toggle the application language between Bulgarian and English

## Non-Functional Requirements

### Performance

- **NFR-P1:** All application screens load within 2 seconds on a standard Bulgarian mobile connection (LTE/4G, ~20 Mbps) at a 390px viewport
- **NFR-P2:** The public showcase page achieves First Contentful Paint within 2 seconds under the same connection baseline
- **NFR-P3:** Skeleton screens are displayed on any screen that requires a data fetch — no blank or white loading states
- **NFR-P4:** Bill PDFs and condition report PDFs are served as externally-hosted signed URLs — PDF content is never embedded in page payloads
- **NFR-P5:** QR code generation for IRIS Pay and Revolut.me payment methods completes client-side in under 100ms

### Security

- **NFR-S1:** All data is transmitted over HTTPS; no HTTP endpoints in production
- **NFR-S2:** Database content is encrypted at rest
- **NFR-S3:** All sensitive resource identifiers use UUIDs — no sequential or guessable IDs on tenancies, bills, condition reports, or uploaded files
- **NFR-S4:** Every API endpoint enforces per-resource authorisation — authenticated users can only access resources they own; authentication alone is not sufficient
- **NFR-S5:** All file uploads are validated by MIME type and file signature; only JPEG, PNG, and PDF files are accepted; uploaded files are never executed
- **NFR-S6:** All user-generated text rendered in the UI is output-encoded to prevent XSS
- **NFR-S7:** All text input fields sanitize input to prevent SQL injection and script injection
- **NFR-S8:** Tenancy data is strictly scoped — a user can never access another user's data, including former tenants accessing current tenant data

### Reliability

- **NFR-R1:** System uptime is ≥ 99.5% per calendar month, measured excluding scheduled maintenance windows
- **NFR-R2:** Transactional email delivery rate is ≥ 99% — no missed credential deliveries, payment confirmations, condition report PDFs, or legally significant documents
- **NFR-R3:** PDF generation succeeds 100% of the time for all triggered events — payment confirmations, condition report sign-offs, auto-resolutions, and move-out bundles; generation failures must be logged and retried
- **NFR-R4:** In-app document status (payment confirmed, condition report signed) serves as a secondary confirmation channel alongside email delivery

### Accessibility

- **NFR-A1:** All interactive elements have a minimum tap target size of 44×44px
- **NFR-A2:** Colour contrast meets WCAG 2.1 AA minimums: 4.5:1 for normal text, 3:1 for large text
- **NFR-A3:** All form inputs have associated `<label>` elements; no placeholder-only labelling
- **NFR-A4:** Error states communicate meaning through text, not colour alone
- **NFR-A5:** All meaningful images carry descriptive `alt` text; decorative images use `alt=""`
- **NFR-A6:** The `<html>` element carries the correct `lang` attribute (`bg` or `en`) for the active language
- **NFR-A7:** The interface is fully navigable without hover interactions — all functionality is available via tap or click

### Scalability

- **NFR-SC1:** The data model supports multiple landlords, multiple properties per landlord, and multiple tenancies per property from day one — V1 UI surfaces one landlord's data; the architecture is not single-tenant
- **NFR-SC2:** All infrastructure scaling upgrades (compute, database, email) require configuration changes only — no code or architectural changes at any anticipated V1 growth stage

### Integration Reliability

- **NFR-I1:** Transactional email delivery retries automatically on failure; critical documents (credentials, receipts, condition report PDFs) are confirmed via in-app status as a secondary signal
- **NFR-I2:** Signed document URLs are generated with expiry windows ≥ 4 hours; URLs are regenerated on access if expired
- **NFR-I3:** PDF generation executes entirely server-side with no external service calls — no network failure mode
