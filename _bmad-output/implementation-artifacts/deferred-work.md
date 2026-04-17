# Deferred Work

## Deferred from: code review of 1-6-design-system-foundation (2026-04-14)

- **JWT auth-guard layouts decode payload without signature verification** — pre-existing (Stories 1.3–1.5); actual enforcement is at the API via `TokenValidFromMiddleware`. No change needed unless client-side trust boundary becomes a concern.
- **LanguageToggle drops query strings and hash fragments on locale switch** — pre-existing (Story 1.5); `segments[1] = newLocale` only replaces the locale path segment. Revisit when query-string–dependent pages exist.
- **`sameSite: 'strict'` on JWT cookie blocks external-link cross-site navigation** — pre-existing (Story 1.3); `lax` is the OWASP recommendation for auth cookies that must survive top-level navigations. Revisit before email-link flows are introduced.
- **`autoComplete` attributes missing on PasswordInput** — `PasswordInput` has no `autoComplete` prop in its interface; login/change-password forms cannot signal `current-password` or `new-password` to password managers. Address when `PasswordInput` interface is next extended.
- **AppHeader: failed logout silently redirects without clearing cookie** — pre-existing (Story 1.5); on network error the JWT cookie is not cleared but the user is redirected to login, where they are immediately bounced back to the dashboard. Add error state UI when AppHeader is further developed.
- **PasswordChangeForm success flag not cleared at start of re-submit** — pre-existing (Story 1.5); success and apiError can briefly coexist. `setSuccess(false)` should be called at the top of `handleSubmit`.
- **LoginPage: stale API error not cleared on subsequent client-side validation failure** — pre-existing (Story 1.5); `setErrors({})` is only called when both validators pass, leaving a prior API error visible alongside new field errors.
- **HTML `<title>` and `<meta description>` not locale-aware** — pre-existing (Story 1.4); static `metadata` export in `[locale]/layout.tsx` always renders English strings. Migrate to `generateMetadata` when SEO/localisation becomes a priority.
- **`aria-busy` missing on submit buttons during loading state** — not in AC scope; screen readers don't announce that a form is processing. Add `aria-busy={loading}` when accessibility pass is done.
- **Tenant layout does not guard `ReadOnly` account state** — pre-existing (Story 1.4); a tenant in `ReadOnly` state bypasses any read-only restriction in the layout redirect logic. Address in the move-out flow (Epic 6).
- **Focus rings use hardcoded hex `#4A6172` instead of CSS variable** — low-severity style concern; `focus-visible:ring-[#4A6172]` is functionally correct but won't update if `--color-primary` changes. Replace with `focus-visible:ring-[--color-primary]` when doing a token-cleanup pass.

## Deferred from: code review of 1-5-login-and-authentication-ui (2026-04-14)

- **Stub pages (dashboard, billing) hardcoded English strings** — Story 2.3 and 3.4 will implement fully; stubs are intentionally minimal.
- **`RequiresPasswordChangeMiddleware` also whitelists regular `/api/auth/change-password`** — pre-existing behavior from Story 1.4; `RequiresPasswordChange` users could call the regular change-password endpoint if they know their temp password. Low risk since temp passwords are system-generated.
- **BFF handlers forward raw request body to API without field filtering** — API validates all inputs; extra fields are ignored by the deserializer. Defense-in-depth concern only.
- **Settings/layout JWT parsing without signature verification** — consistent pattern from Story 1.4 layouts; actual protection is at the API level via `TokenValidFromMiddleware`.
- **Duplicate password validation in controller and Identity** — intentional: controller check rejects invalid passwords before `RemovePasswordAsync` runs, preventing account corruption in the typical case.

## Deferred from: code review of 1-4-nextjs-bff-auth-layer-and-i18n-routing (2026-04-12)

- **Language toggle (AC5)** — LanguageToggle UI component not implemented in Story 1.4; routing infrastructure is ready. Implement in Story 1.5 or 1.6 (Design System Foundation).
- **JWT decode logic duplicated** — `decodeJwtPayload` in `me/route.ts` and inline `Buffer.from` decode in both layout guards are independent implementations of the same operation; a shape change to JWT claims will diverge silently. Extract to a shared utility when convenient.
- **`/api/auth/me` returns HTTP 200 with undefined claims on missing JWT fields** — if a structurally valid JWT without `role`/`account_state` claims is present, route returns `{ role: undefined, accountState: undefined }` with 200 instead of 401. Low risk in practice since all tokens are issued by the API.
- **`apiRequest` sets `Content-Type: application/json` on GET requests** — technically invalid for bodyless requests; some proxies strip or reject it. Fix when API client is extended in a later story.

## Deferred from: code review of 1-3-authentication-api (2026-04-12)

- **landlord_id claim missing from Tenant JWTs** — `AppDbContext.GetCurrentLandlordId()` throws `UnauthorizedAccessException` if the `landlord_id` claim is absent. The comment in AppDbContext already states tenants need this claim pointing to their landlord's ID. `AuthService.GenerateJwt()` only adds it for Landlords. Must be completed in Story 2.4 (Tenant Account Creation) when the tenant→landlord association is established and the claim can be populated.
- **DB call per authenticated request in TokenValidFromMiddleware** — every request incurs an extra round-trip to load the user for `TokenValidFrom` validation. Acceptable at V1 Neon free-tier load, but note as a known scaling constraint. Revisit if query volume becomes a concern post-launch.

## Deferred from: code review of 1-2-database-schema-and-ef-core-foundation (2026-04-11)

- **Payment.TenancyId absent** — when a landlord has multiple tenancies, service code must traverse Payment→BillPeriod→Tenancy to verify cross-tenant access; no direct FK shortcut. Address when billing endpoints are built (Story 3.x).
- **Empty connection string startup validation** — `appsettings.json` has empty `DefaultConnection`/`MigrationConnection` placeholders with no startup guard. A `builder.Configuration.GetConnectionString("DefaultConnection") ?? throw` guard would give a clearer startup error. Low-priority improvement.
- **WelcomePack.Content XSS risk** — `Content` stored as unbounded text with no declared format. If rendered as HTML in the frontend, sanitisation must happen at the UI layer. Address in Welcome Pack UI story.
- **EmailNudgeJob.TenancyId cascade delete** — `TenancyId` is a bare Guid with no navigation property or `OnDelete` cascade; orphaned rows will remain if a tenancy is ever deleted. Intentional per spec; revisit if tenancy deletion is ever added.
- **HasQueryFilter + background jobs contract** — `GetCurrentLandlordId()` throws in non-HTTP contexts. Background jobs (Stories 3.3+, 4.4) must use `IgnoreQueryFilters()` explicitly when querying landlord-scoped entities. Document this constraint in the background job story specs.
- **ConditionReport.ReportType field** — no move-in/move-out discriminator on `ConditionReport`. Add `ReportType` enum and `(TenancyId, ReportType)` unique composite index in Story 4.1 (Condition Report Data Model).
- **CORS policy absent in Program.cs** — no `AddCors`/`UseCors` call; cross-origin requests from Azure SWA will be blocked. Address in Story 1.3 (auth API) when the allowed SWA origin is known.
- **MigrationConnection not wired to IDesignTimeDbContextFactory** — `dotnet ef` CLI uses `DefaultConnection` (pooled) unless `--connection` flag is passed manually. An `IDesignTimeDbContextFactory<AppDbContext>` implementation would automate this. Workaround documented in dev notes; defer until it causes friction.
