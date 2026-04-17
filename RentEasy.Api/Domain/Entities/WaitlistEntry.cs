namespace RentEasy.Api.Domain.Entities;

// ⛔ NOT landlord-scoped — do NOT add HasQueryFilter
public class WaitlistEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public bool IsLandlordInterest { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
}
