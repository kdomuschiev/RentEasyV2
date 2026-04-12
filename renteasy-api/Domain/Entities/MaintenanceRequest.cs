using renteasy_api.Domain.Enums;

namespace renteasy_api.Domain.Entities;

public class MaintenanceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid TenancyId { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Received;
    public string PhotoBlobPaths { get; set; } = "[]";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Tenancy Tenancy { get; set; } = null!;
    public ApplicationUser Tenant { get; set; } = null!;
}
