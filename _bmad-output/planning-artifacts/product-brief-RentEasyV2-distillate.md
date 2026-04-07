---
title: "Product Brief Distillate: RentEasyV2"
type: llm-distillate
source: "product-brief-RentEasyV2.md"
created: "2026-03-31"
purpose: "Token-efficient context for downstream PRD creation"
---

# RentEasy — Detail Pack

## Project Fundamentals

- Product name: RentEasy, domain: renteasy.bg
- Owner: Kiril, private landlord, Sofia, Bulgaria
- Current state: 1 apartment ready to rent imminently, 2nd apartment available next year, 3rd planned for purchase year after
- BMad project root: /Users/kdomuschiev/Projects/RentEasyV2
- Primary language: Bulgarian. Secondary: English. Both must be fully supported in the UI.
- Mobile-first is non-negotiable — must work perfectly on mobile AND desktop. Not just responsive, genuinely mobile-first design.

---

## Users & Roles

- **Landlord role**: Kiril (and future: other private Bulgarian landlords). Manages properties, uploads bills, confirms payments, reviews maintenance, manages condition reports.
- **Tenant role**: Local Bulgarian renters, 25–45, digitally comfortable, Viber users. May include expats (hence English support).
- **Future role**: Platform landlords — other private owners who self-onboard via a future public registration flow. Architecture must support multi-landlord from day one even if V1 UI is single-landlord.
- No agency or property management company targeting — product is for private landlords only.

---

## Core V1 Feature: Bill Payment

- Landlord workflow: upload utility bill PDF per category + enter total amount (e.g. Electricity: 19.58 EUR). No line-item calculation or kWh breakdown in V1.
- Bill categories expected: Rent, Electricity, Water, Building maintenance fee (такса поддръжка). Others possible.
- Tenant sees: itemized list of all charges for the month, each with its total and a viewable/downloadable PDF attached.
- Tenant pays externally via one of three methods (all manual — no embedded payment processor):
  - **IBAN bank transfer** — landlord displays IBAN + payment reference
  - **IRIS Pay** — landlord displays phone number + QR code (A2A instant transfer via Bulgarian banking apps; all major BG banks support it: DSK, UniCredit, Fibank, Postbank)
  - **Revolut.me link** — display revolut.me/username + QR code; tenant enters amount manually
- Tenant enters amount themselves in all three methods (no auto-fill — no company/merchant account)
- Landlord receives bank notification via their banking app → logs into RentEasy → manually marks payment as paid
- On landlord confirming payment: system generates a PDF receipt (total paid + all utility bill PDFs included) → saved in app → available for tenant download
- Payment history: all past months accessible, all PDFs downloadable, permanently stored in app

---

## Payment Provider Analysis (resolved decisions)

- **Stripe**: rejected for V1 — requires Bulgarian registered company (EOOD/OOD). Planned for V2 when company is registered.
- **ePay.bg / Borica**: not pursued — also require merchant registration.
- **PayPal personal**: rejected — high fees (~3.4%), low Bulgarian adoption, poor UX.
- **IRIS Pay**: chosen — A2A instant, works with personal bank account, no integration needed, show phone number + QR. Grew 10x in 2025 in Bulgaria.
- **Revolut.me**: chosen — personal account, display link + QR, trivial to add. Tenant uses any card via Revolut interface.
- **IBAN**: chosen — classic, universal fallback.
- Future V2: Register EOOD → integrate Stripe for embedded card payments with auto-reconciliation.

---

## Tenant Onboarding Flow

1. Physical contract signed externally via real estate agent (not in app)
2. Deposit: 2 months rent, held by landlord externally (not tracked financially in app — optional: display "Deposit held: X EUR" as informational record)
3. Landlord creates tenant account in RentEasy after contract signing
4. Tenant receives email with login credentials + digital welcome pack
5. Welcome pack contents: apartment manual, utility provider contacts, building rules, WiFi credentials, garbage collection schedule, emergency numbers
6. On first login: tenant is directed to complete the move-in condition report

---

## Condition Report Flow (detailed)

**Move-in:**
1. Tenant completes room-by-room condition report (photos + notes per room/item)
2. Two buttons presented: **Agree** / **Disagree**
3. **Agree path**: App generates timestamped PDF with tenant name/profile explicitly recorded as accepting conditions → saves PDF in app → emails PDF to both tenant and landlord → flow complete
4. **Disagree path**: Tenant uploads photos + written descriptions of disputed items → app adds to conditions list → Viber notification to landlord + email confirmation to tenant
5. Landlord logs in, reviews disputed items → if agrees with tenant's additions → re-requests tenant sign-off
6. Tenant reviews updated list → clicks Agree → same PDF generation + email delivery as step 3
7. Iterative: steps 4–6 can repeat until agreement reached

**Move-out:**
- Mirrors move-in flow exactly
- Both move-in and move-out PDFs stored permanently in app as tenancy record

**Notifications split:**
- Viber: workflow nudges (notify, remind, prompt action)
- Email: all PDF document delivery (permanent, retrievable, legally defensible)

**V1 sign-off method**: Email confirmation with timestamped photos = legally valid documentary evidence in Bulgarian courts. Zero cost.
**V2 sign-off upgrade**: Evrotrust QES integration (requires company registration first; Evrotrust API is B2B only).

---

## Maintenance Requests

- Tenant submits issue: title + description + photo(s)
- Landlord notified via Viber
- Landlord updates status (received / in progress / resolved)
- Tenant notified on status changes via Viber
- Full history stored in app — replaces unstructured Viber/phone threads
- No third-party contractor management in V1

---

## Public Showcase Page

- **Primary design priority** — highest visual quality in the product
- Design inspiration provided by user: bunsa.studio and essiewine.com (aesthetic reference only, not to copy)
- Must showcase the apartment beautifully: photos, description, key details, location
- **Waitlist/interest form**: if apartment is currently occupied, visitor can leave their email to be notified when it becomes available. Exact copy TBD by Kiril.
- Multiple properties supported (each with own showcase page as portfolio grows)
- This page is public-facing — no login required

---

## Notifications Strategy

- **Email only for V1** — Viber integration deferred to V2 (partner agreement process is a launch blocker with unknown timeline)
- WhatsApp explicitly rejected
- **V1 email notification list:**
  - Bill uploaded — "Your monthly bills are ready"
  - Payment due reminder
  - Payment confirmed + receipt attached
  - Condition report nudges (Day 3, Day 7, Day 14)
  - Maintenance request status updates
  - Credential delivery + welcome pack
  - PDF documents — condition reports, receipts, final move-out bundle
- **V2:** Add Viber as parallel channel once partner agreement is in place. Email stays permanently — never removed.
- Channel split by intent: conversational nudges → Viber (V2). Permanent records and credentials → email (always).

---

## GDPR / Data Compliance (private individual, no company)

- GDPR applies (operating a purposefully built web app for commercial rental activity falls outside household exemption)
- **No registration with КЗЛД required** — mandatory registration was abolished May 2018
- **No DPO required** at this scale
- Practical minimum requirements:
  - One-page privacy notice given to tenants at contract signing — must include retention periods and legal bases in plain language
  - Internal record of processing activities (private document, never filed anywhere)
  - HTTPS + encrypted database (at rest and in transit)
  - Data breach plan: notify КЗЛД within 72h if breach poses risk to tenants
- Legal basis for processing: Art. 6(1)(b) — contract performance covers most data; Art. 6(1)(f) — legitimate interest for utility/damage documentation
- Data retention: active tenancy + 5–10 years post-tenancy for financial/legal records (Bulgarian tax law)
- No consent needed for standard rental data — contractual basis is stronger
- **GDPR deletion requests — explicit data split:**
  - *Profile data* (name, email, phone, credentials) — deletable on request
  - *Tenancy record data* (condition reports, payment history, receipts, bill PDFs) — retained under Art. 17(3)(b) exemption (legal claims defence) + Bulgarian tax law. Not deletable on request.
  - On deletion request: written response within 30 days stating what was deleted and what was retained and why. Template this response before launch.
  - Plain sentence shown to tenant during onboarding: "Your tenancy records are kept for 7 years after move-out as required by Bulgarian law."
- **Post move-out access:**
  - Move-out flow triggered by landlord — automatically downgrades tenant account to read-only
  - Former tenant retains read-only access to their own payment history and condition reports only for 12 months
  - At move-out: system generates final PDF bundle (all receipts + condition reports) and emails to former tenant
  - Access auto-expires after 12 months — no landlord action required
  - Former tenant can never see new tenant's data under any circumstances — data strictly scoped by tenancy period

---

## Competitor Intelligence

| Competitor | Origin | Tenant portal | Key gap |
|---|---|---|---|
| Rentila (rentila.bg) | Bulgarian-built | None | Entirely landlord-side; 4.8★, freemium €0–9.90/mo; most mature local tool |
| RentPackage (rentpackage.com) | Foreign, BG-localized | Yes (billing + maintenance) | Pricing opaque; agency-focused; no confirmed BG regulatory compliance |
| Immotech (immotech-app.com) | German company, BG phone | Yes (consumer app) | Website broken/empty; consumer household app not landlord SaaS; 23 reviews |
| Flat Manager (flatmanager.bg) | Bulgarian service co. | None | Human-service business, no digital product |
| Rentify (rentify.bg) | Bulgarian service co. | None | Tech-enabled service, no tenant portal; 115k+ check-ins |
| Управление БГ | Bulgarian service co. | None | Zero digital product, phone/email only; 1,400+ properties |
| Bidrento (bidrento.com) | Estonian PropTech | Partial | EU expansion play; not localized for BG; targets institutional landlords |

**Key insight**: No Bulgarian-built tenant-facing rental payment platform exists. Rentila is the strongest local competitor but has zero tenant portal — their gap is exactly RentEasy's core value proposition.

---

## Market Context

- Sofia rental prices +16% in 2024; 2025–2026 growth 5–8% annually
- Bulgaria joined eurozone January 1, 2026 — forcing re-denomination of contracts and bill clarification. Natural forcing function for transparent payment platform.
- IRIS Pay A2A transactions grew 10x in 2025 — digital payment infrastructure mature
- Average Sofia 2BR rent ~€700/month; rental yields ~4.19% (Q3 2025)
- Growing expat/digital nomad segment in Sofia → demand for app-based management
- EU regulatory digitization pressure increasing (rental registration, GDPR, VAT)
- PropTech Europe market ~€12.82B (2025), growing ~19% YoY

---

## Technical Constraints & Preferences

- Web application only in V1 — no native mobile apps
- Mobile-first responsive design — designed at 390px first, adapted up to desktop
- Minimum supported: Chrome 90+ (Android), Safari 14+ (iPhone/iOS 14+), any phone 2018 or newer. Older devices get degraded but functional experience.
- Minimum tap target: 44px on all interactive elements
- Sub-2-second page load on standard Bulgarian mobile connection. Skeleton screens where data must load.
- Must support multi-landlord, multi-property data architecture from day one (even if V1 UI is single-landlord)
- PDF generation required: condition reports, payment receipts
- File storage required: utility bill PDFs, condition report photos, receipt PDFs
- **Email-only notifications in V1** — Viber Business API deferred to V2
- No payment gateway integration in V1 — display payment instructions only
- Bulgarian and English i18n from the start
- **Single account per apartment in V1** — one set of credentials per tenancy. Multi-occupant shared credentials is tenant's private arrangement.
- All data endpoints must have proper authorization checks — no tenant can access another tenant's data by guessing URLs
- Input sanitization required on all text fields — XSS and SQL injection prevention

## URL Structure & Showcase Page

- **`renteasy.bg`** = the apartment showcase page directly. No subdirectory, no listing layer in V1.
- Migration path: when second property arrives, introduce `renteasy.bg/apartments/[name]` structure at that point. Any shared links or QR codes will need updating then.
- **Two-phase visual launch:** Phone shots at initial launch (before July 2026). Professional photographer shots replace them July 2026+. Page structure, copy, and UX are complete from day one — only photography upgrades.
- Showcase page is public-facing, no login required
- Ghost login: "Tenant login →" text link top-right corner only — no prominent navigation

---

## Rejected Ideas / Out of Scope (do not re-propose for V1)

- **Card payment integration (Stripe/Mollie)**: requires registered company → V2
- **Evrotrust API integration**: requires B2B commercial contract → V2
- **DocuSign/BoldSign AES**: valid option but superseded by email confirmation decision for V1
- **WhatsApp notifications**: explicitly rejected
- **Viber notifications in V1**: deferred to V2 — partner agreement timeline is a launch blocker. Email-only in V1.
- **Automated utility bill fetching**: V2+
- **Lease/contract generation or e-signing**: V2+
- **Tenant screening / background checks**: future
- **Native mobile apps**: future
- **Public landlord marketplace / multi-landlord discovery**: future
- **Tax summary export**: V3+
- **Tenant references and scoring**: V3+
- **kWh / unit-level bill breakdown**: explicitly rejected — total per bill type only in V1
- **Deposit financial management in app**: deposit held externally, optional informational display only
- **Third-party contractor management**: future

---

## Open Questions (unresolved at brief completion)

- Exact waitlist form copy for showcase page (Kiril will specify later)
- Viber Business API: deferred to V2 — no investigation needed before V1 launch
- Whether to display deposit amount as informational record in the app (decided: optional)
- Monetization pricing tiers for future platform (freemium + paid automation tier — details TBD)
- Whether condition report Disagree flow has a maximum iteration limit or escalation path if landlord and tenant cannot agree
- Future: how other landlords will discover RentEasy (no acquisition channel defined yet)
- Minimum supported browser/device: Chrome 90+, Safari 14+, 2018+ hardware (resolved — see Technical Constraints)

## Decisions Log (post-brief brainstorming sessions, 2026-04-01/02)

- **Notifications V1:** Email only. Viber deferred to V2.
- **URL structure:** `renteasy.bg` is the showcase page directly. No subdirectory in V1.
- **Single account per apartment:** One set of credentials per tenancy in V1.
- **Post move-out access:** Read-only for 12 months, final PDF bundle on move-out, auto-expiry.
- **GDPR deletion:** Profile data deletable. Tenancy record data retained under Art. 17(3)(b) + Bulgarian tax law.
- **Condition report baseline:** Landlord pre-loads 10-15 dispute-prone items before tenant first login. Tenant adds on top. Joint record.
- **Condition report escalation:** Day 3 nudge, Day 7 reminder, Day 14 soft block. 14 days total.
- **Condition report dispute cap:** 3 rounds maximum. After 3 rounds without agreement, dispute stays open as unresolved record — both positions documented. No escalation path in V1.
- **Condition report PDF:** Includes full dispute history — all rounds, all photos, all notes, all timestamps from both parties. Not just the final agreed state.
- **Two-phase visual launch:** Phone shots at launch, professional photographer shots in July 2026.
- **Showcase page:** renteasy.bg is the apartment. Waitlist counter (seeded warm). Landlord interest CTA in footer (separate audience, same email collection mechanic). Ghost login top-right.
- **Payment flow:** Tenant marks "I've paid" → status becomes "Payment pending confirmation" → Kiril gets email notification → Kiril verifies bank receipt → Kiril confirms in app → receipt generated. No automated reconciliation. Wrong amounts handled outside app.
- **Maintenance resolution:** Landlord marks resolved. Tenant gets email notification. No tenant confirmation step in V1.
