namespace renteasy_api.Domain.Entities;

public class Tenancy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid? TenantId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? MoveOutDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Property Property { get; set; } = null!;
    public ApplicationUser Landlord { get; set; } = null!;
    public ApplicationUser? Tenant { get; set; }
    public WelcomePack? WelcomePack { get; set; }
    public List<ConditionReport> ConditionReports { get; set; } = [];
    public List<BillPeriod> BillPeriods { get; set; } = [];
    public List<MaintenanceRequest> MaintenanceRequests { get; set; } = [];
}
