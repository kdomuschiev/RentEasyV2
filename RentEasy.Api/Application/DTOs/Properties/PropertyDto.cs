namespace RentEasy.Api.Application.DTOs.Properties;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal? SizeSqm { get; set; }
    public int? Floor { get; set; }
    public List<string> BillCategories { get; set; } = [];
    public string? Iban { get; set; }
    public string? IrisPayPhoneNumber { get; set; }
    public string? RevolutMeLink { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
