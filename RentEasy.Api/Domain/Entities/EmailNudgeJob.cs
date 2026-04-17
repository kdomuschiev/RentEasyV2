using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Domain.Entities;

// ⛔ NOT landlord-scoped — do NOT add HasQueryFilter
public class EmailNudgeJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenancyId { get; set; }
    public NudgeType NudgeType { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
