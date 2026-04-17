using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using renteasy_api.Application.DTOs.Properties;
using renteasy_api.Application.Services;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;
using renteasy_api.Infrastructure.Data;

namespace renteasy_api.Tests.Application.Services;

public class PropertyServiceTests
{
    private static AppDbContext CreateDbContext(string dbName, Guid landlordId)
    {
        var httpContextMock = new Mock<IHttpContextAccessor>();
        var claims = new List<Claim>
        {
            new Claim("landlord_id", landlordId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, landlordId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        httpContextMock.Setup(a => a.HttpContext).Returns(httpContext);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options, httpContextMock.Object);
    }

    private static PropertyService CreateService(AppDbContext db) => new(db);

    [Fact]
    public async Task CreatePropertyAsync_CreatesPropertyWithCorrectLandlordId()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var request = new CreatePropertyRequest
        {
            Name = "Sofia Apartment",
            Address = "1 Vitosha Blvd",
            SizeSqm = 65.5m,
            Floor = 3,
            BillCategories = [BillCategory.Rent, BillCategory.Electricity]
        };

        var dto = await service.CreatePropertyAsync(landlordId, request);

        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Sofia Apartment", dto.Name);
        Assert.Equal("1 Vitosha Blvd", dto.Address);
        Assert.Equal(65.5m, dto.SizeSqm);
        Assert.Equal(3, dto.Floor);
        Assert.Contains("Rent", dto.BillCategories);
        Assert.Contains("Electricity", dto.BillCategories);
        Assert.True(dto.CreatedAt > DateTimeOffset.MinValue);
        Assert.True(dto.UpdatedAt > DateTimeOffset.MinValue);

        // Verify persisted with correct LandlordId
        var saved = await db.Properties.FirstAsync(p => p.Id == dto.Id);
        Assert.Equal(landlordId, saved.LandlordId);
    }

    [Fact]
    public async Task GetPropertyAsync_ReturnsProperty_ForOwningLandlord()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var created = await service.CreatePropertyAsync(landlordId, new CreatePropertyRequest
        {
            Name = "My Flat",
            Address = "2 Main St",
            BillCategories = [BillCategory.Rent]
        });

        var dto = await service.GetPropertyAsync(landlordId, created.Id);

        Assert.NotNull(dto);
        Assert.Equal(created.Id, dto.Id);
        Assert.Equal("My Flat", dto.Name);
    }

    [Fact]
    public async Task GetPropertyAsync_ReturnsNull_ForDifferentLandlord()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordA = Guid.NewGuid();
        var landlordB = Guid.NewGuid();

        var dbA = CreateDbContext(dbName, landlordA);
        var serviceA = CreateService(dbA);
        var created = await serviceA.CreatePropertyAsync(landlordA, new CreatePropertyRequest
        {
            Name = "A Property",
            Address = "A Street",
            BillCategories = [BillCategory.Rent]
        });

        var dbB = CreateDbContext(dbName, landlordB);
        var serviceB = CreateService(dbB);
        var result = await serviceB.GetPropertyAsync(landlordB, created.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePropertyAsync_UpdatesFieldsAndRefreshesUpdatedAt()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var created = await service.CreatePropertyAsync(landlordId, new CreatePropertyRequest
        {
            Name = "Old Name",
            Address = "Old Address",
            BillCategories = [BillCategory.Rent]
        });

        var before = DateTimeOffset.UtcNow;
        var updated = await service.UpdatePropertyAsync(landlordId, created.Id, new UpdatePropertyRequest
        {
            Name = "New Name",
            Address = "New Address",
            SizeSqm = 80m,
            Floor = 5,
            BillCategories = [BillCategory.Electricity, BillCategory.Water]
        });
        var after = DateTimeOffset.UtcNow;

        Assert.NotNull(updated);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal("New Address", updated.Address);
        Assert.Equal(80m, updated.SizeSqm);
        Assert.Equal(5, updated.Floor);
        Assert.Contains("Electricity", updated.BillCategories);
        Assert.Contains("Water", updated.BillCategories);
        Assert.True(updated.UpdatedAt >= before && updated.UpdatedAt <= after);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ReturnsNull_ForNonExistentProperty()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var result = await service.UpdatePropertyAsync(landlordId, Guid.NewGuid(), new UpdatePropertyRequest
        {
            Name = "Attempt",
            Address = "Attempt",
            BillCategories = [BillCategory.Rent]
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ReturnsNull_ForDifferentLandlord()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordA = Guid.NewGuid();
        var landlordB = Guid.NewGuid();

        var dbA = CreateDbContext(dbName, landlordA);
        var serviceA = CreateService(dbA);
        var created = await serviceA.CreatePropertyAsync(landlordA, new CreatePropertyRequest
        {
            Name = "A Property",
            Address = "A Street",
            BillCategories = [BillCategory.Rent]
        });

        var dbB = CreateDbContext(dbName, landlordB);
        var serviceB = CreateService(dbB);
        var result = await serviceB.UpdatePropertyAsync(landlordB, created.Id, new UpdatePropertyRequest
        {
            Name = "Hijack",
            Address = "Hijack St",
            BillCategories = [BillCategory.Rent]
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePaymentMethodsAsync_SavesAndClearsPaymentMethodFields()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var created = await service.CreatePropertyAsync(landlordId, new CreatePropertyRequest
        {
            Name = "Flat",
            Address = "Flat St",
            BillCategories = [BillCategory.Rent]
        });

        var withMethods = await service.UpdatePaymentMethodsAsync(landlordId, created.Id, new UpdatePaymentMethodsRequest
        {
            Iban = "BG80BNBG96611020345678",
            IrisPayPhoneNumber = "+359888123456",
            RevolutMeLink = "https://revolut.me/kiril"
        });

        Assert.NotNull(withMethods);
        Assert.Equal("BG80BNBG96611020345678", withMethods.Iban);
        Assert.Equal("+359888123456", withMethods.IrisPayPhoneNumber);
        Assert.Equal("https://revolut.me/kiril", withMethods.RevolutMeLink);

        // Clear IBAN — null clears it
        var cleared = await service.UpdatePaymentMethodsAsync(landlordId, created.Id, new UpdatePaymentMethodsRequest
        {
            Iban = null,
            IrisPayPhoneNumber = "+359888123456",
            RevolutMeLink = "https://revolut.me/kiril"
        });

        Assert.NotNull(cleared);
        Assert.Null(cleared.Iban);
        Assert.Equal("+359888123456", cleared.IrisPayPhoneNumber);
    }

    [Fact]
    public async Task UpdatePaymentMethodsAsync_ReturnsNull_ForDifferentLandlord()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordA = Guid.NewGuid();
        var landlordB = Guid.NewGuid();

        var dbA = CreateDbContext(dbName, landlordA);
        var serviceA = CreateService(dbA);
        var created = await serviceA.CreatePropertyAsync(landlordA, new CreatePropertyRequest
        {
            Name = "A Property",
            Address = "A Street",
            BillCategories = [BillCategory.Rent]
        });

        var dbB = CreateDbContext(dbName, landlordB);
        var serviceB = CreateService(dbB);
        var result = await serviceB.UpdatePaymentMethodsAsync(landlordB, created.Id, new UpdatePaymentMethodsRequest
        {
            Iban = "BG00FAKE00000000000000"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePropertyAsync_DeduplicatesBillCategories()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();
        var db = CreateDbContext(dbName, landlordId);
        var service = CreateService(db);

        var dto = await service.CreatePropertyAsync(landlordId, new CreatePropertyRequest
        {
            Name = "Flat",
            Address = "Flat St",
            BillCategories = [BillCategory.Rent, BillCategory.Rent, BillCategory.Electricity]
        });

        Assert.Equal(2, dto.BillCategories.Count);
        Assert.Contains("Rent", dto.BillCategories);
        Assert.Contains("Electricity", dto.BillCategories);
    }
}
