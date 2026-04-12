using renteasy_api.Domain.Enums;

namespace renteasy_api.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public Guid BillPeriodId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
    public decimal? AmountConfirmed { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public string? ReceiptPdfBlobPath { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public BillPeriod BillPeriod { get; set; } = null!;
}
