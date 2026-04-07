---
title: "Product Brief: RentEasyV2"
status: "complete"
created: "2026-03-31"
updated: "2026-03-31T23:59:00"
inputs: ["user-discovery session", "web research: Bulgarian rental market", "web research: PropTech Europe 2024-2025", "competitor analysis: rentila.bg, rentpackage.com, immotech-app.com", "web research: GDPR for private landlords Bulgaria", "web research: Evrotrust digital signatures Bulgaria"]
---

# Product Brief: RentEasy

## Executive Summary

RentEasy (renteasy.bg) is a modern web application for private landlords and their tenants in Bulgaria. It replaces the opaque, friction-filled experience of paying rent and utility bills with something tenants have never seen in the Bulgarian market: complete clarity, a smooth digital process, and the feeling of renting from someone who actually cares.

The Bulgarian rental market is at an inflection point. Sofia rents grew 16% in 2024. Bulgaria joined the eurozone on January 1, 2026, forcing landlords and tenants to re-denominate contracts and re-clarify all charges. IRIS Pay A2A transactions grew tenfold in 2025. The infrastructure for a seamless digital rental experience is ready — yet no Bulgarian-built tenant-facing platform exists. Rentila — the most mature Bulgarian landlord tool — has no tenant portal at all. RentPackage targets agencies with opaque pricing. Immotech is a broken-website consumer app. The tenant-facing experience is entirely unoccupied.

RentEasy starts as a personal tool for one landlord and two apartments in Sofia. It is architected from the start to grow into a multi-landlord SaaS platform — the product private landlords across Bulgaria didn't know they needed.

---

## The Problem

Paying rent and bills in Bulgaria is an act of faith. Tenants receive a monthly total — sometimes via Viber message, sometimes a handwritten note — with little or no explanation of what they're paying for. Is the electricity estimate or actual? What does the building maintenance fee cover? Why did the water bill spike? Nobody explains. Tenants pay anyway, because there's no better option.

Professional property management companies dominate the market but solve this poorly. Their apps, where they exist at all, offer a single function: enter your payment. No bill breakdown. No attached utility scans. No history. No communication channel. The tenant experience is an afterthought.

The cost of the status quo is eroded trust. Tenants who don't understand their charges either disengage, question everything via phone and Viber, or quietly resent their landlord. For a private landlord who wants long-term, reliable tenants, this is exactly the wrong dynamic.

---

## The Solution

RentEasy gives tenants a clear, professional window into their rental relationship. Each month, the landlord uploads the utility bills and enters the totals — Electricity: 19.58 EUR, Water: 10.32 EUR, Building maintenance: 35 EUR, Rent: 700 EUR. The actual bill PDFs are attached and viewable in the app. The tenant receives a Viber notification, opens a clean summary, pays via IBAN, IRIS Pay, or Revolut — whichever they prefer — and the landlord confirms receipt. Done.

Beyond bill payment, RentEasy covers the full lifecycle of a professional tenancy:

- **Digital onboarding** — contract signed externally via real estate agent; landlord then creates the tenant account. Tenant receives credentials by email and a digital welcome pack: apartment manual, utility contacts, building rules, WiFi, garbage collection schedule.
- **Condition reports** — tenant completes a room-by-room photo move-in check on first login, timestamped and locked. Mirrored at move-out. The physical contract and deposit stay external; RentEasy holds the digital evidence trail.
- **Maintenance requests** — tenants submit issues with photos; landlord tracks resolution. No more Viber threads with no record.
- **Payment archive** — tenants can download all receipts and bills as PDFs anytime, useful for accountants and expense reports.
- **Automated reminders** — rent due, bill uploaded, maintenance resolved — all pushed via Viber.

The primary language is Bulgarian. English is fully supported.

---

## What Makes This Different

**Transparency as the product.** Every competitor treats the payment as the product and the breakdown as optional. RentEasy inverts this: the clarity *is* the experience. Tenants who understand what they're paying for don't send support messages, don't dispute charges, and don't leave.

**Built for the private landlord who gives a damn.** Professional management companies are optimizing for portfolio scale. RentEasy is optimized for the landlord who wants to be the best — modern, responsive, trustworthy — without it consuming their life. Automation handles the routine; the relationship stays human.

**Right time, right market.** The eurozone transition is forcing every Bulgarian landlord to revisit how they communicate charges. RentEasy arrives at the exact moment tenants are expecting more financial clarity and landlords are looking for a better way to provide it.

**Architected to grow.** Multi-landlord, multi-property from day one. A private landlord today; a platform for thousands of private landlords tomorrow.

---

## Who This Serves

**Primary — Tenant (local Bulgarian, 25–45):** Renting in Sofia, digitally comfortable, probably using Viber daily. Pays bills without complaint when they understand them. Values transparency and a landlord who communicates professionally. The "aha moment": opening the first bill breakdown and seeing exactly what they're paying for, with the actual utility scans attached.

**Primary — Landlord (private owner, 1–5 properties):** Has a real job and wants their rental properties to run smoothly without constant Viber messages. Wants to be a good landlord — professional, trustworthy — but doesn't want to build a second career around it. The "aha moment": the first month with zero tenant payment questions.

**Future — Platform Landlord:** Other private landlords in Bulgaria who discover RentEasy and want the same experience for their properties. The same product, extended to multi-user onboarding.

---

## Success Criteria

**V1 success (personal use):**
- Tenant pays rent and bills through the app every month without sending a single "what am I paying for?" message
- All bill documentation lives in the app — zero files sent via Viber or email
- Landlord spends less than 15 minutes per month on billing administration

**Platform success (future):**
- First non-owner landlord successfully onboarded
- Tenant NPS measurably higher than industry baseline
- Month-over-month growth in properties under management

---

## Scope

**V1 — In scope:**
- Public-facing apartment showcase page (primary design priority — high visual quality, mobile-first)
- Interest/waitlist form on showcase page: if apartment is occupied, visitor leaves email to be notified when it becomes available
- Authentication with two roles: landlord and tenant
- Bill management: landlord uploads utility bill PDFs and enters total amount per bill type (e.g., Electricity: 19.58 EUR). No line-item calculation required.
- Tenant payment interface: itemized summary of all charges with attached PDFs viewable/downloadable in-app
- Payment via three channels (no company required): IBAN bank transfer, IRIS Pay (QR + phone number display), Revolut.me link (QR + link display). Tenant enters amount manually; landlord manually confirms receipt in the app.
- On payment confirmation: system generates a PDF receipt covering total payment + all utility bills, available for download
- Viber notifications and reminders (bill uploaded, payment due, maintenance updates)
- Digital tenant onboarding: contract signed externally → landlord creates tenant account → tenant receives email credentials + welcome pack
- Maintenance request submission and tracking with photo evidence
- Move-in/move-out condition reports with iterative sign-off flow:
  1. Tenant completes condition report on first login (room-by-room, with photos)
  2. **Agree** → app generates timestamped PDF with tenant identity explicitly recorded, saves it in the app, and delivers it via email to both tenant and landlord. Flow complete.
  3. **Disagree** → tenant uploads photos and descriptions of disputed items. App adds them to the conditions list and notifies landlord (Viber) and tenant (email confirmation).
  4. Landlord logs in, reviews disputed items, and if in agreement re-requests tenant sign-off.
  5. Tenant reviews updated conditions → clicks Agree → same PDF generation and email delivery as step 2.
  - Viber used for workflow nudges (notify, remind, prompt action). Email used for all PDF document delivery — permanent, retrievable, legally defensible record.
  - Generated PDFs stored in the app for the full duration of the tenancy and beyond.
- Mobile-first responsive design — optimised for both mobile and desktop
- Bulgarian (primary) and English (secondary) language support
- Multi-landlord, multi-property data architecture (even if UI starts single-landlord)

**V1 — Out of scope:**
- Direct bank integration or automated payment reconciliation (landlord manually confirms receipt)
- Lease/contract generation or e-signing
- Automated utility bill fetching from providers
- Tenant screening or background checks
- Mobile native apps (web-first, mobile-responsive)
- Public landlord marketplace or multi-landlord discovery

**Compliance notes:**
- GDPR applies to this web app even without a registered company. No registration with КЗЛД is required. Practical requirements: one-page privacy notice for tenants at contract signing, internal processing record (private document), HTTPS + encrypted database, data breach plan. No DPO needed at this scale.
- Condition report sign-off: email confirmation with timestamped photos (V1). Evrotrust QES integration planned for V2.

---

## Roadmap Thinking

**Year 1 — Personal tool:** One landlord, one apartment (second apartment added mid-year). Prove the core loop: bill uploaded → tenant notified → tenant pays → zero confusion.

**Year 2 — Platform foundations:** Second apartment live. Begin onboarding other private landlords in Sofia. Add self-serve landlord registration. Introduce freemium model with paid tier for automation features. Register company (EOOD) → upgrade to Stripe for embedded card payments with auto-reconciliation. Integrate Evrotrust QES for legally binding condition report sign-off.

**Year 3+ — Product expansion:** Lease management and e-signing, automated utility bill ingestion, tax summary export for landlord income declarations, tenant references and scoring, expansion to Plovdiv and Varna. Monetization: freemium with paid automation tier.

The long-term vision: the go-to platform for the Bulgarian private landlord who wants to offer a professional, modern rental experience without becoming a property management company.
