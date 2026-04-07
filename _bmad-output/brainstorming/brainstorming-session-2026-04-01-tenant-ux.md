---
stepsCompleted: [1, 2, 3, 4]
inputDocuments:
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-01-now.md
session_topic: 'RentEasy Tenant UX — onboarding, first impressions, and ongoing experience'
session_goals: 'Define the tenant journey from first login to settled tenant — delight moments, trust signals, and flows that make tenants feel professionally hosted'
selected_approach: 'ai-recommended'
techniques_used: ['Role Playing', 'Reversal Inversion', 'Question Storming']
ideas_generated: 24
session_active: false
workflow_completed: true
context_file: '_bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md'
---

# Brainstorming Session: RentEasy Tenant UX

**Facilitator:** Kiril
**Date:** 2026-04-01
**Topic:** Tenant UX — onboarding, first impressions, and ongoing experience
**Techniques:** Role Playing → Reversal Inversion → Question Storming

---

## Session Overview

**Topic:** RentEasy Tenant UX — onboarding, first impressions, and ongoing experience
**Goals:** Define the tenant journey from first login to settled tenant — delight moments, trust signals, and flows that make tenants feel professionally hosted

**Key context established during session:**
- Trust is established offline — tenant already knows RentEasy from apartment viewing and contract signing. Credential email arrives expected, not cold.
- Showcase page launches with phone shots before July, professional shots replace them in July. Page is never unfinished — just on version one of visuals.
- 90% of apartment is brand new — condition report baseline is minimal (10-15 photos, dispute-prone items only)
- Soft block for incomplete condition report at 14 days (revised from 21)

---

## Technique Selection

**Approach:** AI-Recommended Techniques

**Techniques used:**
- **Role Playing:** Inhabited the tenant at each stage — first email, first login, condition report, billing cycle. Surfaced emotional states the product brief didn't capture.
- **Reversal Inversion:** Designed the worst possible experience at each touchpoint, then inverted every element into the design spec.
- **Question Storming:** Generated 35 open questions across 6 domains — surfaced V1 requirements hiding as unknowns, security gaps, and legal tensions.

---

## Complete Idea Inventory

### Theme 1: Trust & First Contact
*Solving the credibility problem before the app even opens*

**[Trust #1]: Trust Established Offline**
_Concept:_ By the time the credential email arrives, the tenant has already met Kiril in person, visited renteasy.bg, understood the payment flow, and chosen to sign the contract knowing all of this. The email is expected, the brand is familiar, the purpose is known.
_Novelty:_ The onboarding flow doesn't need to sell RentEasy — it just needs to fulfil the expectation already set in person.

**[Onboarding #1]: Frictionless Credential Email**
_Concept:_ The email does one job — delivers credentials and gets the tenant to first login. No marketing copy, no feature explanations, no long welcome text. Name, apartment address, single button: "Set up your account."
_Novelty:_ Respects that the tenant already knows why they're here. Most SaaS welcome emails try to do too much.

**[Trust #5]: Credential Email Inversion — Design Spec**
_Concept:_ Sender: "Kiril via RentEasy." Subject: "Your apartment at [address] is ready." Single large CTA button. No expiry pressure. No GDPR walls. First line names landlord and specific property. Designed by inverting every element of a phishing email.
_Novelty:_ Anti-phishing design logic applied to a welcome email.

**[Trust #3]: Landlord-Anchored Sender**
_Concept:_ Every element connects to something the tenant already knows — the landlord's name, the apartment address. Unknown brand anchored to trusted person immediately.
_Novelty:_ Personalisation as anti-phishing trust signal, not marketing tactic.

---

### Theme 2: Auth & Account Security

**[Auth #1]: Password Setup Flow**
_Concept:_ First login forces password change before anything else. Rules: 6+ characters, letters and numbers. Single entry field with show/hide eye icon. Email confirmation on save. Forgot password on login screen from day one.
_Novelty:_ Single field with eye icon skips the confirmation field — small UX kindness most apps ignore.

**[Auth #2]: Show Password Option**
_Concept:_ Eye icon instead of confirmation field. Tenant verifies visually rather than typing twice. Reduces lockout risk without adding friction.
_Novelty:_ Cleaner than two fields, safer than blind single entry.

---

### Theme 3: Condition Report UX
*The most legally important and emotionally risky moment in the tenancy*

**[Onboarding #2]: Tenant-First Framing**
_Concept:_ The condition report is introduced as "your protection for the entire tenancy — anything you document now cannot be held against you at move-out." Tenant is active documenter, not respondent to landlord's version of reality.
_Novelty:_ Same functionality, completely different emotional positioning.

**[Onboarding #3]: The Safety Intro Screen**
_Concept:_ Before the report begins, one full screen: "This report protects you. Anything you photograph and note here is permanently recorded. You can add anything you find, disagree with anything you see, and nothing is locked until you're ready. Take your time." Signed by Kiril.
_Novelty:_ Converts the most anxiety-inducing moment into the most trust-building one. The landlord proactively tells the tenant how to protect themselves against the landlord.

**[Onboarding #4]: No Time Pressure**
_Concept:_ No deadline shown on first login. Draft saved indefinitely. App explicitly tells tenant: "Some things only show up after a few days — a leaky tap, a squeaky door. Add them when you find them."
_Novelty:_ Removes urgency that makes tenants feel trapped.

**[Onboarding #5]: Escalation Sequence (14-day)**
_Concept:_ Day 3: gentle Viber nudge — "When you're ready, no rush." Day 7: Viber + in-app banner. Day 14: Viber to tenant + Viber to landlord. Soft block at 14 days — calm screen, "this protects you, 15 minutes," one button.
_Novelty:_ Starts with patience, ends with a wall. Firm without hostile.

**[Onboarding #6]: Landlord Visibility into Completion**
_Concept:_ Kiril sees completion status in dashboard — not started / in progress / complete. Can send personal Viber nudge if stuck. Human touch beats automated reminders.
_Novelty:_ System reminders feel like spam. A message from a person feels like a relationship.

**[Onboarding #7]: Soft Block Framing**
_Concept:_ When app soft-blocks at day 14, screen is calm not punitive. "To access your bills and payment history, we need to complete one thing first. This protects you for your entire tenancy. It takes about 15 minutes." One button: "Start now."
_Novelty:_ The block is a nudge, not a punishment. Copy keeps the relationship intact.

**[Onboarding #8]: Landlord Pre-Loaded Condition Baseline**
_Concept:_ Before tenant's first login, Kiril uploads photos and descriptions of dispute-prone items. Tenant opens the condition report and finds it already partially populated — reviewing and confirming rather than building from blank.
_Novelty:_ Inverts the cognitive burden. Instead of "photograph everything," the tenant's job is "check if this is accurate."

**[Onboarding #9]: Pre-Load as Trust Signal**
_Concept:_ The fact that the landlord already documented everything — including imperfections — before the tenant moved in signals fairness, not entrapment. "Kiril photographed the small scratch on the kitchen cabinet before you arrived."
_Novelty:_ Transparency about imperfections converts skeptical tenants immediately.

**[Onboarding #10]: Tenant Layer on Top**
_Concept:_ Tenant adds photos and notes on top of landlord baseline. Landlord content is read-only — tenant can't delete it, only add. Both layers preserved in final PDF. A joint record, not one party's version.
_Novelty:_ Condition report as conversation, not monologue. Legally stronger and relationally healthier.

**[Onboarding #12]: Minimal Baseline for Renovated Properties**
_Concept:_ For this apartment: 10-15 photos only. Older floor section, renovated furniture piece, appliance interiors (oven, fridge), front door and locks, windows. Everything brand new needs no documentation — brevity signals confidence.
_Novelty:_ A short report on a renovated apartment is more credible than an exhaustive one. Less says more.

---

### Theme 4: Billing & Payment UX
*The core monthly interaction — money must feel safe*

**[Billing #1]: The One-Screen Bill**
_Concept:_ The monthly bill view does everything on a single screen. Itemised breakdown at top, attached PDFs inline, payment options below, "I've paid" button at the bottom. No jumping between sections, no hunting for information.
_Novelty:_ Most billing UIs separate these into multiple screens. One screen removes every moment of "where do I find this?"

**[Billing #2]: One-Tap IBAN Copy**
_Concept:_ IBAN, Revolut link, and IRIS phone number each have a single copy button. Tap once — copied to clipboard, ready to paste into banking app. Zero manual typing of financial details.
_Novelty:_ Typing an IBAN manually is error-prone and anxiety-inducing. One tap eliminates both.

**[Billing #3]: PDF Permanence Promise**
_Concept:_ Every uploaded bill PDF and every generated receipt is permanently stored and permanently accessible. Full payment history from day one of tenancy. Everything downloadable anytime.
_Novelty:_ Permanence is a feature. The app is the legal record.

**[Billing #4]: The Receipt Moment**
_Concept:_ Landlord confirms payment → immediate Viber notification → PDF receipt generated automatically with all bills bundled. "Payment confirmed — your receipt is ready." The emotional payoff of the monthly cycle.
_Novelty:_ The receipt notification is the moment the tenant feels the product working. Repeated every month.

**[Billing #5]: Payment History as Peace of Mind**
_Concept:_ Every month, every amount, every status visible. Every receipt downloadable. Full tenancy financial record in 5 seconds. Replaces Viber thread archaeology that currently passes for payment history in Bulgarian rentals.
_Novelty:_ History as trust infrastructure, not just a log.

---

### Theme 5: Design & Performance

**[Design #1]: Speed as a Feature**
_Concept:_ Sub-2-second load on standard Bulgarian mobile connection. Skeleton screens where data must load. No blank white screens ever.
_Novelty:_ In Bulgaria's rental app landscape, slow is the norm. Speed alone is differentiating.

**[Design #2]: Visual Language Continuity**
_Concept:_ Same fonts, palette, and care as the showcase page. The app feels like one coherent product — not a different system behind a pretty website.
_Novelty:_ Most rental apps look nothing like their marketing sites. RentEasy is consistent end-to-end.

**[Mobile #1]: Designed at 390px First**
_Concept:_ Every screen designed and tested on smallest common smartphone screen. No horizontal scroll, no tiny tap targets, no desktop tables squeezed onto mobile.
_Novelty:_ "Mobile-responsive" is how every competitor describes their product. "Mobile-first" is what RentEasy actually delivers.

**[Mobile #2]: Tap Target Standard**
_Concept:_ Every interactive element minimum 44px tap target height. Usable while standing in a kitchen without zooming.
_Novelty:_ Small detail, enormous daily UX difference.

---

### Theme 6: Showcase Page Visual Update

**[Production #6]: Two-Phase Visual Launch**
_Concept:_ Launch showcase page with high-quality phone shots before July — photographer not available until July. Professional shots replace them then. The page is never unfinished, just on version one of visuals. Three rules for phone shots: natural light only, obsessively clean space, lead with detail shots over wide rooms.
_Novelty:_ Separates page launch from photography quality. Waiting for the photographer means losing months of waitlist collection.

---

## Open Questions — Prioritized by V1 Impact

### Must answer before building V1

| Question | Why it matters |
|---|---|
| Old tenant still has credentials after move-out — who deactivates and when? | Security and data protection |
| Can GDPR deletion be requested during an active tenancy? | Conflicts with contract performance legal basis |
| If tenant data is deleted, what happens to the condition report protecting the landlord? | Fundamental legal tension to resolve |
| How long must financial records be kept under Bulgarian tax law? | Data retention policy can't contradict tax law |
| Two tenants sharing one apartment — one account or two? | Auth and billing model decision |
| What if Viber is down on bill upload day? | Notification fallback needed |
| What if the tenant changes their phone number — does Viber break? | Account management flow needed |
| What is the minimum supported browser and device? | Defines front-end technical scope |

### Answer before V1 launch

| Question | Why it matters |
|---|---|
| Tenant pays wrong amount — over or under? | Landlord confirmation flow edge case |
| Landlord doesn't confirm payment for 3 days — what does tenant see? | Needs a status state and timeout |
| Maintenance request no response for a week — what happens? | Needs escalation or at minimum a visible status |
| Who decides "resolved" on maintenance — landlord or tenant? | Product decision with relationship implications |
| Tenant wants to give notice through the app? | Needs a graceful "not here" answer |

### Security — answer before launch

| Question | Why it matters |
|---|---|
| SQL injection / XSS in text fields | Standard input sanitization — must be implemented |
| Tenant guessing another tenant's URL to access their data | Authorization checks on all data endpoints |
| Attacker attempting to break or probe the app | Basic security audit before launch |

### V2 / future

- Card payment flow and disputes
- Two-person tenancy accounts
- Contract and notice management in app
- GDPR deletion of waitlist emails when product evolves
- Old tenant data retention policy post-tenancy

---

## Prioritization Results

**Top 3 high-impact ideas:**
1. **Tenant-First Framing + Safety Intro Screen [Onboarding #2, #3]** — reframes the most anxiety-inducing moment into the most trust-building one
2. **Landlord Pre-Loaded Baseline + Tenant Layer on Top [Onboarding #8, #10]** — removes blank-form terror, signals landlord fairness, creates a joint legal record
3. **The One-Screen Bill + Receipt Moment [Billing #1, #4]** — the monthly cycle done right, start to finish

**Quick wins:**
- Show password eye icon — one field change, significant UX improvement
- Forgot password on login screen — must exist from day one, build it first
- One-tap IBAN copy — trivial to implement, removes financial anxiety
- Skeleton screens — perceived performance improvement with low build cost

**Breakthrough concepts:**
- **14-Day Soft Block** — patience with a wall. The escalation sequence that's firm without being hostile
- **Tenant Layer on Top of Landlord Baseline** — condition report as conversation not monologue. Legally stronger, relationally healthier
- **PDF Permanence Promise** — storage as a trust feature, not just a technical requirement

---

## Action Planning

### Before any code is written
- Resolve GDPR tension: deletion request vs. condition report retention — legal clarity needed
- Decide: two tenants sharing one apartment — one account or two?
- Define minimum supported browser and device
- Write the Safety Intro Screen copy — Kiril's voice, not legal language
- Decide notification fallback when Viber is unavailable

### Early in build
- Implement authorization checks on all data endpoints — no tenant accesses another tenant's data by guessing URLs
- Input sanitization on all text fields — no exceptions
- Implement skeleton screens from the start — retrofitting is harder
- Build old tenant deactivation flow — who triggers it, when, what access remains

### Before launch
- Define and document data retention policy — active tenancy + 5-10 years financial records
- Test full flow on a 6-year-old Android on a slow connection
- Basic security audit of all input fields and data endpoints

---

## Session Summary

**Total ideas generated:** 24 across 3 techniques
**Open questions surfaced:** 35 across 6 domains
**Themes identified:** 6 (Trust/First Contact, Auth, Condition Report, Billing, Design/Performance, Production)

**Key creative breakthroughs:**
- The condition report's hidden emotional problem: it looks like a trap, not a shield. Solved by tenant-first framing and landlord pre-loading the baseline.
- The credential email structurally resembles a phishing attack — solved offline, not by design.
- "Every piece of friction in this app should feel like it's protecting the tenant, never like it's serving the landlord." — the design principle that emerged from the session and governs all future UX decisions.
- The billing experience's worst-case is entirely about money feeling unsafe. Every fix is about making money feel visible, certain, and confirmed.

---

*Session completed: 2026-04-02*
*Facilitated using BMAD Brainstorming Workflow*
