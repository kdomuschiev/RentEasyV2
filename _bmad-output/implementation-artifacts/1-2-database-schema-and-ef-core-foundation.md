# Story 1.2: Database Schema & EF Core Foundation

**Status:** review
**Epic:** 1 — Deployable Foundation
**Story Key:** 1-2-database-schema-and-ef-core-foundation

---

## Story

As a developer,
I want the core database schema established with multi-tenancy and the identity foundation in place,
So that all subsequent stories can persist data against a correctly structured, multi-tenant-safe database.

---

## Acceptance Criteria

**AC1 — Identity extension**
**Given** a Neon PostgreSQL database with two connection strings configured in `appsettings.json`
**When** `dotnet ef database update --connection "<direct_connection_string>"` is run
**Then** the `AspNetUsers` table exists with two additional columns: `token_valid_from` (timestamptz, not null) and `account_state` (varchar, not null, default `'Active'`)

**AC2 — Properties table**
**Given** the initial migration is applied
**When** the `properties` table schema is inspected
**Then** it exists with: `id` (UUID PK), `landlord_id` (UUID FK → AspNetUsers, indexed), `name`, `address`, `size_sqm`, `floor`, `created_at`, `updated_at`

**AC3 — HasQueryFilter enforced**
**Given** `AppDbContext` is configured
**When** a LINQ query is executed for any landlord-scoped entity
**Then** `HasQueryFilter` automatically filters to the current `landlord_id` claim from the JWT
**And** querying without an authenticated context throws an `UnauthorizedAccessException` (never silently returns all records)

**AC4 — Cross-landlord isolation**
**Given** the multi-tenancy filter is active
**When** a landlord queries properties
**Then** only their own properties are returned — never another landlord's records

**AC5 — Runtime uses pooled connection string**
**Given** the application starts
**When** EF Core establishes a database connection
**Then** the pooled Neon connection string (`ConnectionStrings:DefaultConnection`) is used — not the direct migration string

---

## Tasks / Subtasks

- [x] Install EF Core CLI tool (AC: 1)
  - [x] `dotnet tool install --global dotnet-ef` (or confirm already installed with `dotnet ef --version`)

- [x] Create all enums in `Domain/Enums/` (AC: 1, 2)
  - [x] `AccountState.cs` — Active, ReadOnly, Expired
  - [x] `BillCategory.cs` — Rent, Electricity, Water, BuildingMaintenance
  - [x] `ConditionReportStatus.cs` — InProgress, Agreed, Unresolved
  - [x] `MaintenanceStatus.cs` — Received, InProgress, Resolved
  - [x] `NudgeType.cs` — ConditionReportDay3, ConditionReportDay7, ConditionReportDay14, PaymentDueDay3
  - [x] `PaymentStatus.cs` — Unpaid, PendingConfirmation, Confirmed

- [x] Create `Domain/Entities/ApplicationUser.cs` (AC: 1)
  - [x] Extend `IdentityUser<Guid>`
  - [x] Add `TokenValidFrom` (DateTimeOffset, not null) → `token_valid_from`
  - [x] Add `AccountState` (AccountState enum, not null) → `account_state`

- [x] Create all domain entities in `Domain/Entities/` (AC: 2)
  - [x] `Property.cs`
  - [x] `Tenancy.cs`
  - [x] `WelcomePack.cs`
  - [x] `ConditionReport.cs`
  - [x] `ConditionReportItem.cs`
  - [x] `BillPeriod.cs`
  - [x] `Bill.cs`
  - [x] `Payment.cs`
  - [x] `MaintenanceRequest.cs`
  - [x] `WaitlistEntry.cs` (NOT landlord-scoped)
  - [x] `EmailNudgeJob.cs` (NOT landlord-scoped)

- [x] Create `Infrastructure/Data/AppDbContext.cs` (AC: 3, 4, 5)
  - [x] Extend `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`
  - [x] Add `DbSet<T>` for every entity
  - [x] Inject `IHttpContextAccessor` to read `landlord_id` claim
  - [x] Apply `HasQueryFilter` on all landlord-scoped entities
  - [x] Throw `UnauthorizedAccessException` when no auth context
  - [x] Configure snake_case table/column naming via EF Core conventions
  - [x] Configure enums as strings (`HasConversion<string>()`)
  - [x] Configure `LandlordId` index on each landlord-scoped entity

- [x] Create `Common/Constants.cs`
  - [x] `MaxFileSizeBytes` = 10 * 1024 * 1024 (10MB)
  - [x] `SignedUrlInlineExpiryHours` = 1
  - [x] `SignedUrlDownloadExpiryHours` = 24
  - [x] `NudgeJobPollingIntervalMinutes` = 5
  - [x] `MaxConditionReportDisputeRounds` = 3
  - [x] `TenancyExpiryMonths` = 12

- [x] Update `appsettings.json` with connection string keys (AC: 5)

- [x] Update `Program.cs` to register DbContext, Identity, IHttpContextAccessor (AC: 3, 5)

- [x] Run migration and verify (AC: 1, 2)
  - [x] `dotnet ef migrations add InitialCreate --project renteasy-api` (from repo root) using direct connection string
  - [x] `dotnet ef database update --connection "<direct_connection_string>"`
  - [x] Verify `AspNetUsers` has `token_valid_from` and `account_state` columns
  - [x] Verify `properties` table exists with correct schema
  - [x] Verify `dotnet build` still passes after changes

---

## Dev Notes

### What Already Exists (from Story 1.1 — DO NOT RE-CREATE)

Packages already installed in `renteasy-api.csproj`:
- `Npgsql.EntityFrameworkCore.PostgreSQL 10.0.1` ✅
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.5` ✅
- `Azure.Storage.Blobs 12.27.0` ✅
- `QuestPDF 2026.2.4` ✅
- `Resend 0.2.2` ✅
- `Scalar.AspNetCore 2.13.22` ✅

Folder skeletons exist with `.gitkeep` files (replace them):
- `Domain/Entities/`, `Domain/Enums/`, `Domain/Interfaces/`
- `Infrastructure/Data/`, `Infrastructure/Storage/`, `Infrastructure/Email/`, `Infrastructure/Jobs/`
- `Common/Extensions/`, `Common/Middleware/`

`Program.cs` already configured with `JsonNamingPolicy.CamelCase`.

### Neon Connection Strings

Add to `appsettings.json` (placeholder values — real values are in `appsettings.Development.json` which is git-ignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "",
    "MigrationConnection": ""
  }
}
```

`DefaultConnection` = pooled (PgBouncer) endpoint — used at runtime.
`MigrationConnection` = direct endpoint — used ONLY for `dotnet ef database update`.

**NEVER use `DefaultConnection` for migrations** — Neon's pooled endpoint does not support EF Core migration operations and will fail silently or with cryptic errors.

To run migrations with the direct connection string:
```bash
# From repo root
dotnet ef database update --project renteasy-api --connection "Host=ep-divine-base-algj081h.c-3.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=<password>; SSL Mode=VerifyFull; Channel Binding=Require;"
```

### Entity Column Specifications

**Naming rule:** All EF Core entities use `PascalCase` C# properties. EF Core naming convention configured to map them to `snake_case` in PostgreSQL.

#### ApplicationUser (extends `IdentityUser<Guid>`)
Table: `AspNetUsers` (managed by Identity — do not rename)

```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset TokenValidFrom { get; set; }
    public AccountState AccountState { get; set; } = AccountState.Active;
}
```

Extra columns added: `token_valid_from` (timestamptz NOT NULL), `account_state` (varchar NOT NULL, default 'Active')

#### Property
Table: `properties`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | `Guid.NewGuid()` |
| `LandlordId` | `landlord_id` | UUID FK | → AspNetUsers, indexed |
| `Name` | `name` | varchar | not null |
| `Address` | `address` | varchar | not null |
| `SizeSqm` | `size_sqm` | decimal | nullable |
| `Floor` | `floor` | int | nullable |
| `CreatedAt` | `created_at` | timestamptz | not null |
| `UpdatedAt` | `updated_at` | timestamptz | not null |

Navigation: `Landlord` (ApplicationUser), `Tenancies` (List<Tenancy>)

#### Tenancy
Table: `tenancies`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `PropertyId` | `property_id` | UUID FK | → properties |
| `TenantId` | `tenant_id` | UUID FK | → AspNetUsers, nullable (null before tenant created) |
| `StartDate` | `start_date` | timestamptz | not null |
| `MoveOutDate` | `move_out_date` | timestamptz | nullable |
| `CreatedAt` | `created_at` | timestamptz | not null |
| `UpdatedAt` | `updated_at` | timestamptz | not null |

Navigation: `Property`, `Landlord`, `Tenant` (ApplicationUser?), `WelcomePack`, `ConditionReports` (List<ConditionReport>), `BillPeriods` (List<BillPeriod>), `MaintenanceRequests` (List<MaintenanceRequest>)

#### WelcomePack
Table: `welcome_packs`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `TenancyId` | `tenancy_id` | UUID FK | unique (one per tenancy) |
| `Content` | `content` | text | JSON structure stored as text |
| `CreatedAt` | `created_at` | timestamptz | |
| `UpdatedAt` | `updated_at` | timestamptz | |

#### ConditionReport
Table: `condition_reports`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `TenancyId` | `tenancy_id` | UUID FK | |
| `Status` | `status` | varchar | ConditionReportStatus as string |
| `CurrentRound` | `current_round` | int | default 1, max 3 |
| `PdfBlobPath` | `pdf_blob_path` | varchar | nullable; blob path (not signed URL — generate on access) |
| `CreatedAt` | `created_at` | timestamptz | |
| `UpdatedAt` | `updated_at` | timestamptz | |

Navigation: `Items` (List<ConditionReportItem>), `Tenancy`

#### ConditionReportItem
Table: `condition_report_items`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `ConditionReportId` | `condition_report_id` | UUID FK | |
| `ContributedByTenant` | `contributed_by_tenant` | bool | false = landlord item |
| `Title` | `title` | varchar | not null |
| `Notes` | `notes` | text | nullable |
| `PhotoBlobPaths` | `photo_blob_paths` | text | JSON array of blob paths (not signed URLs) |
| `RoundNumber` | `round_number` | int | which dispute round this item belongs to |
| `IsDisputed` | `is_disputed` | bool | default false |
| `CreatedAt` | `created_at` | timestamptz | |

#### BillPeriod
Table: `bill_periods`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `TenancyId` | `tenancy_id` | UUID FK | |
| `PeriodLabel` | `period_label` | varchar | e.g. "April 2026" |
| `CreatedAt` | `created_at` | timestamptz | |
| `UpdatedAt` | `updated_at` | timestamptz | |

Navigation: `Bills` (List<Bill>), `Payment` (Payment?)

#### Bill
Table: `bills`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `BillPeriodId` | `bill_period_id` | UUID FK | |
| `Category` | `category` | varchar | BillCategory as string |
| `Amount` | `amount` | decimal(18,2) | NEVER float/double |
| `PdfBlobPath` | `pdf_blob_path` | varchar | nullable; blob path |
| `CreatedAt` | `created_at` | timestamptz | |

Navigation: `BillPeriod`

#### Payment
Table: `payments`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `BillPeriodId` | `bill_period_id` | UUID FK | unique (one per period) |
| `Status` | `status` | varchar | PaymentStatus as string; default Unpaid |
| `AmountConfirmed` | `amount_confirmed` | decimal(18,2) | nullable |
| `PaidAt` | `paid_at` | timestamptz | nullable (when tenant tapped "I've paid") |
| `ConfirmedAt` | `confirmed_at` | timestamptz | nullable (when landlord confirmed) |
| `ReceiptPdfBlobPath` | `receipt_pdf_blob_path` | varchar | nullable; blob path |
| `CreatedAt` | `created_at` | timestamptz | |
| `UpdatedAt` | `updated_at` | timestamptz | |

#### MaintenanceRequest
Table: `maintenance_requests`

| C# Property | Column | Type | Notes |
|---|---|---|---|
| `Id` | `id` | UUID PK | |
| `LandlordId` | `landlord_id` | UUID FK | indexed |
| `TenancyId` | `tenancy_id` | UUID FK | |
| `TenantId` | `tenant_id` | UUID FK | |
| `Title` | `title` | varchar | not null |
| `Description` | `description` | text | not null |
| `Status` | `status` | varchar | MaintenanceStatus as string; default Received |
| `PhotoBlobPaths` | `photo_blob_paths` | text | JSON array of blob paths |
| `CreatedAt` | `created_at` | timestamptz | |
| `UpdatedAt` | `updated_at` | timestamptz | |

#### WaitlistEntry — NOT LANDLORD-SCOPED
Table: `waitlist_entries`

```csharp
// ⛔ DO NOT add HasQueryFilter to this entity
public class WaitlistEntry
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsLandlordInterest { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
}
```

#### EmailNudgeJob — NOT LANDLORD-SCOPED
Table: `email_nudge_jobs`

```csharp
// ⛔ DO NOT add HasQueryFilter to this entity
public class EmailNudgeJob
{
    public Guid Id { get; set; }
    public Guid TenancyId { get; set; }
    public NudgeType NudgeType { get; set; }
    public DateTimeOffset? SentAt { get; set; }  // null = not yet sent; set = idempotency guard
    public DateTimeOffset CreatedAt { get; set; }
}
```

### AppDbContext Implementation Pattern

```csharp
// Infrastructure/Data/AppDbContext.cs
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenancy> Tenancies => Set<Tenancy>();
    public DbSet<WelcomePack> WelcomePacks => Set<WelcomePack>();
    public DbSet<ConditionReport> ConditionReports => Set<ConditionReport>();
    public DbSet<ConditionReportItem> ConditionReportItems => Set<ConditionReportItem>();
    public DbSet<BillPeriod> BillPeriods => Set<BillPeriod>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();
    public DbSet<EmailNudgeJob> EmailNudgeJobs => Set<EmailNudgeJob>();

    private Guid GetCurrentLandlordId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("No authenticated user context available for query filter.");

        // JWT will contain "landlord_id" claim for both landlord and tenant roles (set in Story 1.3)
        // - Landlord: landlord_id = their own user ID
        // - Tenant: landlord_id = their associated landlord's user ID
        var landlordIdClaim = user.FindFirst("landlord_id")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(landlordIdClaim, out var landlordId))
            throw new UnauthorizedAccessException("Cannot determine landlord context from token.");

        return landlordId;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply snake_case naming convention
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()!.ToSnakeCase());
            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(fk.GetConstraintName()!.ToSnakeCase());
            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()!.ToSnakeCase());
        }

        // Enums as strings
        builder.Entity<ApplicationUser>()
            .Property(u => u.AccountState)
            .HasConversion<string>();

        builder.Entity<ConditionReport>()
            .Property(r => r.Status)
            .HasConversion<string>();

        builder.Entity<Bill>()
            .Property(b => b.Category)
            .HasConversion<string>();

        builder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>()
            .HasDefaultValue(PaymentStatus.Unpaid);

        builder.Entity<MaintenanceRequest>()
            .Property(m => m.Status)
            .HasConversion<string>()
            .HasDefaultValue(MaintenanceStatus.Received);

        builder.Entity<EmailNudgeJob>()
            .Property(j => j.NudgeType)
            .HasConversion<string>();

        // Decimal precision for money — NEVER float/double
        builder.Entity<Bill>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(p => p.AmountConfirmed)
            .HasPrecision(18, 2);

        // Unique constraints
        builder.Entity<WelcomePack>()
            .HasIndex(w => w.TenancyId)
            .IsUnique();

        builder.Entity<Payment>()
            .HasIndex(p => p.BillPeriodId)
            .IsUnique();

        // LandlordId indexes on all landlord-scoped entities
        builder.Entity<Property>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_properties_landlord_id");
        builder.Entity<Tenancy>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_tenancies_landlord_id");
        builder.Entity<WelcomePack>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_welcome_packs_landlord_id");
        builder.Entity<ConditionReport>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_condition_reports_landlord_id");
        builder.Entity<ConditionReportItem>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_condition_report_items_landlord_id");
        builder.Entity<BillPeriod>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_bill_periods_landlord_id");
        builder.Entity<Bill>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_bills_landlord_id");
        builder.Entity<Payment>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_payments_landlord_id");
        builder.Entity<MaintenanceRequest>().HasIndex(e => e.LandlordId).HasDatabaseName("ix_maintenance_requests_landlord_id");

        // HasQueryFilter — landlord-scoped entities only
        // ⛔ WaitlistEntry and EmailNudgeJob must NOT have this filter
        builder.Entity<Property>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<Tenancy>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<WelcomePack>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<ConditionReport>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<ConditionReportItem>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<BillPeriod>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<Bill>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<Payment>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
        builder.Entity<MaintenanceRequest>().HasQueryFilter(e => e.LandlordId == GetCurrentLandlordId());
    }
}
```

**Important:** `GetCurrentLandlordId()` will throw during migration (no HTTP context). This is expected and correct — migrations run outside any request context. EF Core tools bypass query filters during migrations.

### snake_case Extension Method

EF Core does not have built-in snake_case conversion. Add a simple extension:

```csharp
// Common/Extensions/StringExtensions.cs
public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Concat(input.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString())).ToLower();
    }
}
```

Alternatively, use `EFCore.NamingConventions` NuGet package (`dotnet add package EFCore.NamingConventions`) and call `.UseSnakeCaseNamingConvention()` in `UseNpgsql`. This is the recommended approach — less custom code.

### Program.cs Additions

Add these registrations in `Program.cs`:

```csharp
// Add IHttpContextAccessor (required by AppDbContext for HasQueryFilter)
builder.Services.AddHttpContextAccessor();

// Add EF Core + Neon (pooled connection string for runtime)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
```

### appsettings.json Update

Add connection string keys (empty values — real values in git-ignored `appsettings.Development.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "",
    "MigrationConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Constants.cs

```csharp
// Common/Constants.cs
namespace renteasy_api.Common;

public static class Constants
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    public const int SignedUrlInlineExpiryHours = 1;
    public const int SignedUrlDownloadExpiryHours = 24;
    public const int NudgeJobPollingIntervalMinutes = 5;
    public const int MaxConditionReportDisputeRounds = 3;
    public const int TenancyExpiryMonths = 12;
}
```

### Running Migrations

```bash
# From repo root — add migration
dotnet ef migrations add InitialCreate --project renteasy-api

# Apply to Neon using DIRECT (non-pooled) connection string
dotnet ef database update --project renteasy-api \
  --connection "Host=ep-divine-base-algj081h.c-3.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=<password>; SSL Mode=VerifyFull; Channel Binding=Require;"
```

**If `dotnet ef` is not found:** `dotnet tool install --global dotnet-ef`

### Landlord_id Claim in JWT (cross-story contract)

Story 1.3 (Auth API) must issue a `landlord_id` claim in the JWT:
- **Landlord user** → `landlord_id = user.Id` (their own ID)
- **Tenant user** → `landlord_id = tenancy.LandlordId` (their landlord's ID)

This claim is what `AppDbContext.GetCurrentLandlordId()` reads. Story 1.2 defines the contract; Story 1.3 fulfils it.

### Blob Path Pattern (for reference — used in later stories)

Stored paths in DB are always blob paths, never signed URLs:
```
{tenancyId}/{category}/{uuid}.{ext}
```
Examples:
- `{tenancyId}/bills/{uuid}.pdf`
- `{tenancyId}/condition-report/{uuid}.jpg`
- `{tenancyId}/receipts/{uuid}.pdf`

Signed URLs are generated on-demand (1hr inline, 24hr download). This is why columns are named `*_blob_path`, not `*_url`.

### QuestPDF Licence (set in Program.cs — not this story, but don't miss it in 1.3+)

```csharp
QuestPDF.Settings.License = LicenseType.Community;
```

### Cross-Cutting Constraints (apply to this story)

- All IDs are `Guid` — never `int` or sequential ID
- All monetary columns use `decimal` with `HasPrecision(18,2)` — never `float` or `double`
- Enum values stored as strings — never integers
- Dates stored as `DateTimeOffset` (timestamptz in PostgreSQL) — never `DateTime` or Unix timestamps
- `WaitlistEntry` and `EmailNudgeJob` MUST NOT have `HasQueryFilter`

---

## Dev Agent Record

### Agent Model Used
claude-sonnet-4-6

### Completion Notes
- Confirmed `dotnet ef` v10.0.5 already installed — no action needed.
- Created 6 enums in `Domain/Enums/`: AccountState, BillCategory, ConditionReportStatus, MaintenanceStatus, NudgeType, PaymentStatus.
- Created `ApplicationUser` extending `IdentityUser<Guid>` with `TokenValidFrom` (DateTimeOffset) and `AccountState` (enum) columns.
- Created 11 domain entities: Property, Tenancy, WelcomePack, ConditionReport, ConditionReportItem, BillPeriod, Bill, Payment, MaintenanceRequest, WaitlistEntry, EmailNudgeJob. WaitlistEntry and EmailNudgeJob explicitly excluded from HasQueryFilter.
- Created `AppDbContext` extending `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>` with: all DbSets, IHttpContextAccessor injection, GetCurrentLandlordId() throwing UnauthorizedAccessException when no auth context, HasQueryFilter on all 9 landlord-scoped entities, enum-as-string conversions, decimal precision (18,2) for money, unique indexes on WelcomePack.TenancyId and Payment.BillPeriodId, LandlordId indexes on all scoped entities.
- Used `EFCore.NamingConventions` (`.UseSnakeCaseNamingConvention()`) for automatic snake_case naming — recommended approach from Dev Notes.
- Created `Common/Constants.cs` with all 6 constants.
- Updated `appsettings.json` with `DefaultConnection` and `MigrationConnection` placeholder keys.
- Updated `Program.cs`: added `AddHttpContextAccessor()`, `AddDbContext<AppDbContext>` with pooled connection string + snake_case convention, `AddIdentity<ApplicationUser, IdentityRole<Guid>>()` with EF stores + token providers, added `UseAuthentication()` middleware.
- Added `Microsoft.EntityFrameworkCore.Design` package to enable EF Core migration tooling.
- Ran `dotnet ef migrations add InitialCreate` — migration generated successfully.
- Ran `dotnet ef database update` against Neon direct endpoint — all 18 tables + indexes created. AspNetUsers has `token_valid_from` (timestamptz) and `account_state` (text) columns. `properties` table has correct schema.
- Created `renteasy-api.Tests` xUnit project with 4 tests covering AC3 (UnauthorizedAccessException without auth), AC4 (cross-landlord isolation), and non-scoped entities (WaitlistEntry, EmailNudgeJob). All 4 tests pass.

### File List
- `renteasy-api/Domain/Enums/AccountState.cs` (new)
- `renteasy-api/Domain/Enums/BillCategory.cs` (new)
- `renteasy-api/Domain/Enums/ConditionReportStatus.cs` (new)
- `renteasy-api/Domain/Enums/MaintenanceStatus.cs` (new)
- `renteasy-api/Domain/Enums/NudgeType.cs` (new)
- `renteasy-api/Domain/Enums/PaymentStatus.cs` (new)
- `renteasy-api/Domain/Entities/ApplicationUser.cs` (new)
- `renteasy-api/Domain/Entities/Property.cs` (new)
- `renteasy-api/Domain/Entities/Tenancy.cs` (new)
- `renteasy-api/Domain/Entities/WelcomePack.cs` (new)
- `renteasy-api/Domain/Entities/ConditionReport.cs` (new)
- `renteasy-api/Domain/Entities/ConditionReportItem.cs` (new)
- `renteasy-api/Domain/Entities/BillPeriod.cs` (new)
- `renteasy-api/Domain/Entities/Bill.cs` (new)
- `renteasy-api/Domain/Entities/Payment.cs` (new)
- `renteasy-api/Domain/Entities/MaintenanceRequest.cs` (new)
- `renteasy-api/Domain/Entities/WaitlistEntry.cs` (new)
- `renteasy-api/Domain/Entities/EmailNudgeJob.cs` (new)
- `renteasy-api/Infrastructure/Data/AppDbContext.cs` (new)
- `renteasy-api/Common/Constants.cs` (new)
- `renteasy-api/appsettings.json` (modified)
- `renteasy-api/Program.cs` (modified)
- `renteasy-api/renteasy-api.csproj` (modified — added EFCore.NamingConventions, Microsoft.EntityFrameworkCore.Design)
- `renteasy-api/Migrations/20260411173802_InitialCreate.cs` (new)
- `renteasy-api/Migrations/20260411173802_InitialCreate.Designer.cs` (new)
- `renteasy-api/Migrations/AppDbContextModelSnapshot.cs` (new)
- `renteasy-api.Tests/renteasy-api.Tests.csproj` (new)
- `renteasy-api.Tests/Infrastructure/Data/AppDbContextTests.cs` (new)
- `RentEasyV2.sln` (modified — test project added)

## Change Log
- 2026-04-11: Story created by bmad-create-story
- 2026-04-11: Story implemented by dev agent (claude-sonnet-4-6) — all tasks complete, 4 tests passing, migration applied to Neon
