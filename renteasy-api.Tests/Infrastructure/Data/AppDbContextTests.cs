using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using renteasy_api.Domain.Entities;
using renteasy_api.Infrastructure.Data;

namespace renteasy_api.Tests.Infrastructure.Data;

public class AppDbContextTests
{
    private static AppDbContext CreateContext(Guid? landlordId = null)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        if (landlordId.HasValue)
        {
            var claims = new List<Claim>
            {
                new Claim("landlord_id", landlordId.Value.ToString()),
                new Claim(ClaimTypes.NameIdentifier, landlordId.Value.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);
        }
        else
        {
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, httpContextAccessorMock.Object);
    }

    private static async Task SeedPropertiesDirectly(AppDbContext context, Guid landlordA, Guid landlordB)
    {
        // Bypass query filter by using EF Core change tracker directly
        var propA = new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordA,
            Name = "Property A",
            Address = "Address A",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var propB = new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordB,
            Name = "Property B",
            Address = "Address B",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Properties.Add(propA);
        context.Properties.Add(propB);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task QueryFilter_WithNoAuthContext_ThrowsUnauthorizedAccessException()
    {
        // Arrange — context with no authenticated user
        var context = CreateContext(landlordId: null);

        // Seed a property by bypassing the context (using a separate seeded context)
        var landlordId = Guid.NewGuid();
        var seedContext = CreateContext(landlordId);
        var property = new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordId,
            Name = "Test Property",
            Address = "123 Test St",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        seedContext.Properties.Add(property);
        await seedContext.SaveChangesAsync();

        // Act & Assert — querying without auth should throw
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => context.Properties.ToListAsync());
    }

    [Fact]
    public async Task QueryFilter_WithAuthenticatedLandlord_ReturnsOnlyOwnProperties()
    {
        // Arrange
        var landlordA = Guid.NewGuid();
        var landlordB = Guid.NewGuid();

        // Use a seeding context with landlord A auth to add landlord A's property
        var seedContextA = CreateContext(landlordA);
        var propertyA = new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordA,
            Name = "Landlord A Property",
            Address = "1 Landlord Ave",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        seedContextA.Properties.Add(propertyA);
        await seedContextA.SaveChangesAsync();

        // A shared in-memory database is needed — use a named database for both contexts
        var dbName = Guid.NewGuid().ToString();

        var httpContextA = new Mock<IHttpContextAccessor>();
        httpContextA.Setup(a => a.HttpContext).Returns(CreateHttpContext(landlordA));

        var httpContextB = new Mock<IHttpContextAccessor>();
        httpContextB.Setup(a => a.HttpContext).Returns(CreateHttpContext(landlordB));

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var ctxA = new AppDbContext(options, httpContextA.Object);
        var ctxB = new AppDbContext(options, httpContextB.Object);

        // Seed both properties bypassing filters (context with landlord A adds its property,
        // context with landlord B adds its property)
        var propA = new Property { Id = Guid.NewGuid(), LandlordId = landlordA, Name = "A", Address = "A", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        var propB = new Property { Id = Guid.NewGuid(), LandlordId = landlordB, Name = "B", Address = "B", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };

        ctxA.Properties.Add(propA);
        await ctxA.SaveChangesAsync();

        ctxB.Properties.Add(propB);
        await ctxB.SaveChangesAsync();

        // Act
        var resultA = await ctxA.Properties.ToListAsync();
        var resultB = await ctxB.Properties.ToListAsync();

        // Assert — each landlord only sees their own properties
        Assert.Single(resultA);
        Assert.Equal(landlordA, resultA[0].LandlordId);

        Assert.Single(resultB);
        Assert.Equal(landlordB, resultB[0].LandlordId);
    }

    [Fact]
    public async Task WaitlistEntry_HasNoQueryFilter_ReturnsAllEntries()
    {
        // Arrange — WaitlistEntry is not landlord-scoped, no HasQueryFilter applied
        var dbName = Guid.NewGuid().ToString();

        // Use a context with no auth for seeding (WaitlistEntry has no filter, so no auth needed for inserts)
        var httpContextMock = new Mock<IHttpContextAccessor>();
        httpContextMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var ctx = new AppDbContext(options, httpContextMock.Object);

        ctx.WaitlistEntries.Add(new WaitlistEntry { Id = Guid.NewGuid(), Email = "a@example.com", CreatedAt = DateTimeOffset.UtcNow });
        ctx.WaitlistEntries.Add(new WaitlistEntry { Id = Guid.NewGuid(), Email = "b@example.com", CreatedAt = DateTimeOffset.UtcNow });
        await ctx.SaveChangesAsync();

        // Act — no auth context, should still return all entries (no query filter)
        var entries = await ctx.WaitlistEntries.ToListAsync();

        // Assert
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public async Task EmailNudgeJob_HasNoQueryFilter_ReturnsAllJobs()
    {
        // Arrange
        var httpContextMock = new Mock<IHttpContextAccessor>();
        httpContextMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ctx = new AppDbContext(options, httpContextMock.Object);

        ctx.EmailNudgeJobs.Add(new EmailNudgeJob { Id = Guid.NewGuid(), TenancyId = Guid.NewGuid(), NudgeType = Domain.Enums.NudgeType.ConditionReportDay3, CreatedAt = DateTimeOffset.UtcNow });
        await ctx.SaveChangesAsync();

        // Act
        var jobs = await ctx.EmailNudgeJobs.ToListAsync();

        // Assert
        Assert.Single(jobs);
    }

    private static HttpContext CreateHttpContext(Guid landlordId)
    {
        var claims = new List<Claim>
        {
            new Claim("landlord_id", landlordId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, landlordId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        return new DefaultHttpContext { User = principal };
    }
}
