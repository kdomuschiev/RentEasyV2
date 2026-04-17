using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Domain.Entities;

public class Bill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid BillPeriodId { get; set; }
    public BillCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? PdfBlobPath { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public BillPeriod BillPeriod { get; set; } = null!;
}
