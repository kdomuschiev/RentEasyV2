using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Domain.Entities;

public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LandlordId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal? SizeSqm { get; set; }
    public int? Floor { get; set; }
    public List<BillCategory> BillCategories { get; set; } = [];
    public string? Iban { get; set; }
    public string? IrisPayPhoneNumber { get; set; }
    public string? RevolutMeLink { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ApplicationUser Landlord { get; set; } = null!;
    public List<Tenancy> Tenancies { get; set; } = [];
}
