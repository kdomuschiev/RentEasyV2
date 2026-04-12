# Deferred Work

## Deferred from: code review of 1-2-database-schema-and-ef-core-foundation (2026-04-11)

- **Payment.TenancyId absent** — when a landlord has multiple tenancies, service code must traverse Payment→BillPeriod→Tenancy to verify cross-tenant access; no direct FK shortcut. Address when billing endpoints are built (Story 3.x).
- **Empty connection string startup validation** — `appsettings.json` has empty `DefaultConnection`/`MigrationConnection` placeholders with no startup guard. A `builder.Configuration.GetConnectionString("DefaultConnection") ?? throw` guard would give a clearer startup error. Low-priority improvement.
- **WelcomePack.Content XSS risk** — `Content` stored as unbounded text with no declared format. If rendered as HTML in the frontend, sanitisation must happen at the UI layer. Address in Welcome Pack UI story.
- **EmailNudgeJob.TenancyId cascade delete** — `TenancyId` is a bare Guid with no navigation property or `OnDelete` cascade; orphaned rows will remain if a tenancy is ever deleted. Intentional per spec; revisit if tenancy deletion is ever added.
- **HasQueryFilter + background jobs contract** — `GetCurrentLandlordId()` throws in non-HTTP contexts. Background jobs (Stories 3.3+, 4.4) must use `IgnoreQueryFilters()` explicitly when querying landlord-scoped entities. Document this constraint in the background job story specs.
- **ConditionReport.ReportType field** — no move-in/move-out discriminator on `ConditionReport`. Add `ReportType` enum and `(TenancyId, ReportType)` unique composite index in Story 4.1 (Condition Report Data Model).
- **CORS policy absent in Program.cs** — no `AddCors`/`UseCors` call; cross-origin requests from Azure SWA will be blocked. Address in Story 1.3 (auth API) when the allowed SWA origin is known.
- **MigrationConnection not wired to IDesignTimeDbContextFactory** — `dotnet ef` CLI uses `DefaultConnection` (pooled) unless `--connection` flag is passed manually. An `IDesignTimeDbContextFactory<AppDbContext>` implementation would automate this. Workaround documented in dev notes; defer until it causes friction.
