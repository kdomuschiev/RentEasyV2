---
stepsCompleted: [1, 2, 3, 4]
inputDocuments:
  - _bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-01-now.md
  - _bmad-output/brainstorming/brainstorming-session-2026-04-01-tenant-ux.md
session_topic: 'RentEasy Edge Cases — unresolved questions, failure modes, and boundary conditions'
session_goals: 'Surface and resolve the open questions from previous sessions, identify failure modes before build, make decisions that unblock development'
selected_approach: 'ai-recommended'
techniques_used: ['Six Thinking Hats', 'Constraint Mapping', 'Failure Analysis']
ideas_generated: 18
session_active: false
workflow_completed: true
context_file: '_bmad-output/planning-artifacts/product-brief-RentEasyV2-distillate.md'
---

# Brainstorming Session: RentEasy Edge Cases

**Facilitator:** Kiril
**Date:** 2026-04-02
**Topic:** Edge cases, failure modes, and unresolved decisions
**Techniques:** Six Thinking Hats → Constraint Mapping → Failure Analysis

---

## Session Overview

**Goal:** Convert open questions from previous sessions into firm decisions that unblock PRD and architecture work. No new ideas — resolution only.

**All decisions from this session have been applied to `product-brief-RentEasyV2-distillate.md`.**

---

## Decisions Made

### Legal & GDPR

**[Legal #1]: GDPR Deletion Policy**
_Decision:_ Split data into two explicit categories:
- **Profile data** (name, email, phone, credentials) — deletable on request
- **Tenancy record data** (condition reports, payment history, receipts, bill PDFs) — retained under Art. 17(3)(b) GDPR exemption (legal claims defence) + Bulgarian tax law. Not deletable on request.

_Implementation:_
- Privacy notice states this split in plain language
- One sentence shown during tenant onboarding: "Your tenancy records are kept for 7 years after move-out as required by Bulgarian law."
- Written response template within 30 days of any deletion request — stating what was deleted, what was retained, why. Template before launch, not after.

_Legal basis:_ Art. 17(3)(b) directly covers this. The exemption is clear. Low risk if privacy notice is well-written.

---

**[Legal #2]: Post Move-Out Access Policy**
_Decision:_
- New tenant = completely separate account. Zero overlap, zero shared data visibility. Ever.
- Former tenant account auto-downgrades to read-only at move-out. Access: own payment history and condition reports only.
- At move-out: system generates final PDF bundle (all receipts + condition reports) and emails to former tenant.
- Read-only access auto-expires 12 months after move-out. No landlord action required.
- Landlord triggers move-out flow manually — starts the clock, sends the bundle, downgrades access.

---

### Notifications

**[Notifications #2]: Email-Only for V1**
_Decision:_ All notifications in V1 are email only. Viber deferred to V2.

_V1 email notification list:_
- Bill uploaded
- Payment due reminder
- Tenant marks "I've paid" → triggers email to Kiril
- Payment confirmed by Kiril → receipt attached
- Condition report nudges (Day 3, Day 7, Day 14)
- Maintenance request status updates
- Credential delivery + welcome pack
- PDF documents — condition reports, receipts, final move-out bundle

_V2:_ Viber added as parallel channel once partner agreement is in place. Email stays permanently.

---

### Account & Auth

**[Product #1]: Single Account Per Apartment — V1**
_Decision:_ One account per apartment. If two people share, they share credentials — their private arrangement. No multi-occupant support in V1.

**[Auth #3]: Minimum Supported Devices**
_Decision:_ Chrome 90+ (Android), Safari 14+ (iPhone/iOS 14+), any phone 2018 or newer. Older devices: degraded but functional, not broken.

---

### Payment Flow

**[Payment #3]: Tenant Marks "I've Paid" — Option A**
_Decision:_ Tenant taps "I've paid" after making bank transfer. Sets status to "Payment pending confirmation." Triggers email to Kiril. Kiril verifies bank receipt and confirms in app. Receipt generated on confirmation.

_Rationale:_ Psychological closure for tenant. Timestamp for records. Action trigger for Kiril. No financial logic — purely a status and notification trigger.

**[Payment #1]: Manual Amount Confirmation**
_Decision:_ App records what Kiril confirms as received — not what was owed. If amount is wrong, resolved outside the app via bank statements. No reconciliation logic in V1.

**[Payment #2]: Payment Pending Status**
_Decision:_ After tenant marks "I've paid," bill shows "Payment pending confirmation." No timeout, no auto-confirmation. Honest status until Kiril acts.

---

### Maintenance

**[Maintenance #1]: Landlord Resolves**
_Decision:_ Kiril marks maintenance requests as resolved. Tenant gets email notification. No tenant confirmation step in V1.

---

### Condition Report

**[ConditionReport #1]: 3-Round Dispute Cap**
_Decision:_ Disagree loop caps at 3 rounds. After 3 rounds without agreement, dispute stays open as an unresolved record — both parties' positions documented. No escalation path in V1. The paper trail exists if it reaches a legal dispute.

**[ConditionReport #2]: Full Dispute History in Final PDF**
_Decision:_ The final condition report PDF includes the complete dispute history — all rounds, all photos, all notes from both parties, all timestamps. Not just the agreed final state.
_Rationale:_ A condition report that shows how agreement was reached is legally stronger than one showing only the outcome.

**[ConditionReport #3]: Status Visibility for Both Parties**
_Decision:_ Both Kiril and the tenant see current round number, whose turn it is to act, and what the outstanding disputed items are. Clear, unambiguous process state at all times.

---

### Security

**[Security #1]: Pre-Launch Security Checklist**
_Non-negotiable before V1 launch:_
- Authorization checks on every data endpoint — logged-in user must own the resource, not just be authenticated
- No sequential/guessable IDs on sensitive resources — use UUIDs
- File upload validation — mime type + file signature, JPEG/PNG/PDF only. Never execute uploaded files.
- Output encoding on all user-generated content rendered in the UI
- Input sanitization on all text fields
- HTTPS everywhere
- Encrypted database at rest

---

## Constraint Map — Final State

| Question | Classification | Resolution |
|---|---|---|
| GDPR deletion vs. condition report | Real constraint + Product decision | Art. 17(3)(b) exemption applies. Data split implemented. |
| Post move-out access | Product decision | 12 months read-only, final PDF bundle, auto-expiry |
| Two tenants sharing | Product decision | One account V1, their arrangement |
| Viber fallback | Product decision | Email-only V1, Viber V2 |
| Minimum device support | Real constraint | Chrome 90+, Safari 14+, 2018+ hardware |
| Tenant pays wrong amount | Product decision | Manual confirmation, handled outside app |
| Payment confirmation delay | Product decision | "Pending" status, no timeout |
| Who resolves maintenance | Product decision | Landlord resolves |
| Condition report loop limit | Product decision | 3 rounds, then open record |
| SQL injection / XSS / URL guessing | Real constraint | Build requirement, not a decision |
| Tenant marks "I've paid" | Product decision | Option A — tenant action, Kiril confirms |

---

## Failure Scenarios — Analysis Results

### Scenario 1: Payment Dispute
_Risk:_ Tenant claims they paid, Kiril's bank shows different amount.
_What holds:_ App records what Kiril confirms as received. Tenant's "I've paid" tap is timestamped. Both have a record. Resolution happens outside the app via bank statements.
_What doesn't exist in V1:_ Automated reconciliation, partial payment handling, in-app dispute flow. All handled manually.
_Verdict:_ Acceptable for V1 scale. One landlord, one apartment.

### Scenario 2: Security Incident
_Risk:_ Malicious user probing URLs, injecting scripts, uploading malicious files.
_What holds:_ UUID resource IDs, authorization checks per resource, file validation, input sanitization, output encoding.
_Pre-launch requirement:_ Security checklist above must be fully implemented. Not optional, not V2.
_Verdict:_ Manageable with standard security practices applied consistently from day one.

### Scenario 3: Condition Report Escalation
_Risk:_ Tenant and landlord disagree, dispute runs multiple rounds, no agreement.
_What holds:_ 3-round cap. Full dispute history in PDF. Both positions documented. Status visibility for both parties throughout.
_What doesn't exist in V1:_ Formal escalation path, third-party mediation, legal integration.
_Verdict:_ The paper trail is sufficient for V1. If it reaches a court, the full documented history is in the PDF.

---

## Session Summary

**Technique 1 — Six Thinking Hats:** Resolved GDPR deletion, post move-out access, single account decision, notification channel.

**Technique 2 — Constraint Mapping:** Cleared 11 open questions — labeled each as real constraint, product decision, or V2 problem. All resolved.

**Technique 3 — Failure Analysis:** Walked 3 highest-risk V1 scenarios. Identified what holds, what's missing, what's acceptable at V1 scale.

**Core insight from this session:**
Most of the open questions weren't actually hard — they just needed a decision. The only genuinely complex one was GDPR deletion, and the law itself provides the answer. Everything else was Kiril's call, and the calls were all straightforward once framed clearly.

**RentEasy V1 is now fully unblocked for PRD and architecture work.**

---

*Session completed: 2026-04-02*
*Facilitated using BMAD Brainstorming Workflow*

