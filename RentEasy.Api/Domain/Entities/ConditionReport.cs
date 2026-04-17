using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Domain.Entities;

public class ConditionReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid TenancyId { get; set; }
    public ConditionReportStatus Status { get; set; } = ConditionReportStatus.InProgress;
    public int CurrentRound { get; set; } = 1;
    public string? PdfBlobPath { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Tenancy Tenancy { get; set; } = null!;
    public List<ConditionReportItem> Items { get; set; } = [];
}
