using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using renteasy_api.Domain.Entities;
using renteasy_api.Infrastructure.Data;

namespace renteasy_api.Tests.Infrastructure.Data;

public class AppDbContextTests
{
    private static AppDbContext CreateContext(string dbName, Guid? landlordId = null)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        if (landlordId.HasValue)
        {
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(CreateHttpContext(landlordId.Value));
        }
        else
        {
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options, httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task QueryFilter_WithNoAuthContext_ThrowsUnauthorizedAccessException()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordId = Guid.NewGuid();

        // Seed a property using an authenticated context
        var seedContext = CreateContext(dbName, landlordId);
        seedContext.Properties.Add(new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordId,
            Name = "Test Property",
            Address = "123 Test St",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        await seedContext.SaveChangesAsync();

        // Query with no auth context — should throw
        var unauthContext = CreateContext(dbName, landlordId: null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => unauthContext.Properties.ToListAsync());
    }

    [Fact]
    public async Task QueryFilter_WithAuthenticatedLandlord_ReturnsOnlyOwnProperties()
    {
        var dbName = Guid.NewGuid().ToString();
        var landlordA = Guid.NewGuid();
        var landlordB = Guid.NewGuid();

        // Seed both landlords' properties into the same DB
        var ctxA = CreateContext(dbName, landlordA);
        var ctxB = CreateContext(dbName, landlordB);

        ctxA.Properties.Add(new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordA,
            Name = "Landlord A Property",
            Address = "1 Landlord Ave",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        await ctxA.SaveChangesAsync();

        ctxB.Properties.Add(new Property
        {
            Id = Guid.NewGuid(),
            LandlordId = landlordB,
            Name = "Landlord B Property",
            Address = "2 Landlord Blvd",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        await ctxB.SaveChangesAsync();

        // Query — each landlord should only see their own property
        var resultA = await ctxA.Properties.ToListAsync();
        var resultB = await ctxB.Properties.ToListAsync();

        Assert.Single(resultA);
        Assert.Equal(landlordA, resultA[0].LandlordId);

        Assert.Single(resultB);
        Assert.Equal(landlordB, resultB[0].LandlordId);
    }

    [Fact]
    public async Task WaitlistEntry_HasNoQueryFilter_ReturnsAllEntries()
    {
        var dbName = Guid.NewGuid().ToString();

        var httpContextMock = new Mock<IHttpContextAccessor>();
        httpContextMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var ctx = new AppDbContext(options, httpContextMock.Object);

        ctx.WaitlistEntries.Add(new WaitlistEntry { Id = Guid.NewGuid(), Email = "a@example.com", CreatedAt = DateTimeOffset.UtcNow });
        ctx.WaitlistEntries.Add(new WaitlistEntry { Id = Guid.NewGuid(), Email = "b@example.com", CreatedAt = DateTimeOffset.UtcNow });
        await ctx.SaveChangesAsync();

        var entries = await ctx.WaitlistEntries.ToListAsync();

        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public async Task EmailNudgeJob_HasNoQueryFilter_ReturnsAllJobs()
    {
        var httpContextMock = new Mock<IHttpContextAccessor>();
        httpContextMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ctx = new AppDbContext(options, httpContextMock.Object);

        ctx.EmailNudgeJobs.Add(new EmailNudgeJob { Id = Guid.NewGuid(), TenancyId = Guid.NewGuid(), NudgeType = Domain.Enums.NudgeType.ConditionReportDay3, CreatedAt = DateTimeOffset.UtcNow });
        await ctx.SaveChangesAsync();

        var jobs = await ctx.EmailNudgeJobs.ToListAsync();

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
