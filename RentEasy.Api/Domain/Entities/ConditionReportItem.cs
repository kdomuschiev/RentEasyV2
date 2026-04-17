namespace RentEasy.Api.Domain.Entities;

public class ConditionReportItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid ConditionReportId { get; set; }
    public bool ContributedByTenant { get; set; } = false;
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string PhotoBlobPaths { get; set; } = "[]";
    public int RoundNumber { get; set; }
    public bool IsDisputed { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }

    public ConditionReport ConditionReport { get; set; } = null!;
}
