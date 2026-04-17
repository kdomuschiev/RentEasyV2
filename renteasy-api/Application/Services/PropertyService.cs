using Microsoft.EntityFrameworkCore;
using renteasy_api.Application.DTOs.Properties;
using renteasy_api.Domain.Entities;
using renteasy_api.Infrastructure.Data;

namespace renteasy_api.Application.Services;

public class PropertyService
{
    private readonly AppDbContext _db;

    public PropertyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PropertyDto> CreatePropertyAsync(Guid landlordId, CreatePropertyRequest request)
    {
        var now = DateTimeOffset.UtcNow;
        var property = new Property
        {
            LandlordId = landlordId,
            Name = request.Name,
            Address = request.Address,
            SizeSqm = request.SizeSqm,
            Floor = request.Floor,
            BillCategories = request.BillCategories.Distinct().ToList(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        _db.Properties.Add(property);
        await _db.SaveChangesAsync();

        return ToDto(property);
    }

    public async Task<PropertyDto?> GetPropertyAsync(Guid landlordId, Guid id)
    {
        var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.LandlordId == landlordId);
        return property == null ? null : ToDto(property);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(Guid landlordId, Guid id, UpdatePropertyRequest request)
    {
        var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.LandlordId == landlordId);
        if (property == null)
            return null;

        property.Name = request.Name;
        property.Address = request.Address;
        property.SizeSqm = request.SizeSqm;
        property.Floor = request.Floor;
        property.BillCategories = request.BillCategories.Distinct().ToList();
        property.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(property);
    }

    public async Task<PropertyDto?> UpdatePaymentMethodsAsync(Guid landlordId, Guid id, UpdatePaymentMethodsRequest request)
    {
        var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.LandlordId == landlordId);
        if (property == null)
            return null;

        property.Iban = request.Iban;
        property.IrisPayPhoneNumber = request.IrisPayPhoneNumber;
        property.RevolutMeLink = request.RevolutMeLink;
        property.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(property);
    }

    private static PropertyDto ToDto(Property p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Address = p.Address,
        SizeSqm = p.SizeSqm,
        Floor = p.Floor,
        BillCategories = p.BillCategories.Select(c => c.ToString()).ToList(),
        Iban = p.Iban,
        IrisPayPhoneNumber = p.IrisPayPhoneNumber,
        RevolutMeLink = p.RevolutMeLink,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
    };
}
