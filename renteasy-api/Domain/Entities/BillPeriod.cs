namespace renteasy_api.Domain.Entities;

public class BillPeriod
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid TenancyId { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Tenancy Tenancy { get; set; } = null!;
    public List<Bill> Bills { get; set; } = [];
    public Payment? Payment { get; set; }
}
