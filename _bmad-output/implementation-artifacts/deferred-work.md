# Deferred Work

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
