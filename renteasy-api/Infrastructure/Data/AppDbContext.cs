using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Infrastructure.Data;

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

        // JWT must contain "landlord_id" claim for both landlord and tenant roles (set in Story 1.3)
        // - Landlord: landlord_id = their own user ID
        // - Tenant: landlord_id = their associated landlord's user ID
        var landlordIdClaim = user.FindFirst("landlord_id")?.Value;

        if (string.IsNullOrWhiteSpace(landlordIdClaim) || !Guid.TryParse(landlordIdClaim, out var landlordId))
            throw new UnauthorizedAccessException("Cannot determine landlord context from token — missing or invalid landlord_id claim.");

        return landlordId;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Enums as strings with DB-level defaults where applicable
        builder.Entity<ApplicationUser>()
            .Property(u => u.AccountState)
            .HasConversion<string>()
            .HasDefaultValue(AccountState.Active);

        builder.Entity<ConditionReport>()
            .Property(r => r.Status)
            .HasConversion<string>()
            .HasDefaultValue(ConditionReportStatus.InProgress);

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

        // Decimal precision — NEVER float/double
        builder.Entity<Bill>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(p => p.AmountConfirmed)
            .HasPrecision(18, 2);

        builder.Entity<Property>()
            .Property(p => p.SizeSqm)
            .HasPrecision(18, 2);

        builder.Entity<Property>()
            .Property(p => p.BillCategories)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<BillCategory>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<BillCategory>()
            )
            .HasColumnType("text")
            .HasDefaultValueSql("'[]'")
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<BillCategory>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

        // Max-length for JSON blob path arrays
        builder.Entity<ConditionReportItem>()
            .Property(e => e.PhotoBlobPaths)
            .HasMaxLength(8192);

        builder.Entity<MaintenanceRequest>()
            .Property(e => e.PhotoBlobPaths)
            .HasMaxLength(8192);

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
