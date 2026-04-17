using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RentEasy.Api.Application.DTOs.Auth;
using RentEasy.Api.Application.Services;
using RentEasy.Api.Domain.Entities;
using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Tests.Application.Services;

public class AuthServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<AuthService>> _loggerMock;

    public AuthServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsATestSigningKeyThatIsLongEnoughForHmacSha256!",
                ["Jwt:Issuer"] = "renteasy-api",
                ["Jwt:Audience"] = "renteasy-web",
                ["Jwt:ExpiryDays"] = "7",
                ["App:FrontendUrl"] = "http://localhost:3000"
            })
            .Build();
        _loggerMock = new Mock<ILogger<AuthService>>();
    }

    private (AuthService service, Mock<IUserStore<ApplicationUser>> storeMock) CreateService()
    {
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        // Register additional interfaces BEFORE accessing .Object
        storeMock.As<IUserEmailStore<ApplicationUser>>();
        storeMock.As<IUserRoleStore<ApplicationUser>>();
        storeMock.As<IUserPasswordStore<ApplicationUser>>();

        var userManager = new UserManager<ApplicationUser>(
            storeMock.Object,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            null!, null!, new UpperInvariantLookupNormalizer(), null!, null!,
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());

        var service = new AuthService(userManager, _configuration, _loggerMock.Object);
        return (service, storeMock);
    }

    private static void SetupFindByEmail(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser? user)
    {
        storeMock.As<IUserEmailStore<ApplicationUser>>()
            .Setup(s => s.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    private static void SetupGetRoles(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser user, IList<string> roles)
    {
        storeMock.As<IUserRoleStore<ApplicationUser>>()
            .Setup(s => s.GetRolesAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
    }

    private static void SetupCheckPassword(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser user, bool valid)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        // For valid: hash the actual password so verification succeeds
        // For invalid: hash a different password so verification fails
        var hash = hasher.HashPassword(user, valid ? "Password123" : "CompletelyDifferentPassword!");
        storeMock.As<IUserPasswordStore<ApplicationUser>>()
            .Setup(s => s.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hash);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("landlord@test.com", AccountState.Active);
        SetupFindByEmail(storeMock, user);
        SetupCheckPassword(storeMock, user, true);
        SetupGetRoles(storeMock, user, ["Landlord"]);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "landlord@test.com",
            Password = "Password123"
        });

        Assert.NotNull(result);
        Assert.Equal("Landlord", result.Role);
        Assert.Equal("Active", result.AccountState);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
    {
        var (service, storeMock) = CreateService();
        SetupFindByEmail(storeMock, null);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "Password123"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("user@test.com", AccountState.Active);
        SetupFindByEmail(storeMock, user);
        SetupCheckPassword(storeMock, user, false);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@test.com",
            Password = "WrongPassword"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_JwtContainsRequiredClaims()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("landlord@test.com", AccountState.Active);
        SetupFindByEmail(storeMock, user);
        SetupCheckPassword(storeMock, user, true);
        SetupGetRoles(storeMock, user, ["Landlord"]);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "landlord@test.com",
            Password = "Password123"
        });

        Assert.NotNull(result);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("landlord@test.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Active", jwt.Claims.First(c => c.Type == "account_state").Value);
        Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == "landlord_id").Value);
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat));
    }

    [Fact]
    public async Task LoginAsync_TenantRole_DoesNotIncludeLandlordIdClaim()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("tenant@test.com", AccountState.Active);
        SetupFindByEmail(storeMock, user);
        SetupCheckPassword(storeMock, user, true);
        SetupGetRoles(storeMock, user, ["Tenant"]);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "tenant@test.com",
            Password = "Password123"
        });

        Assert.NotNull(result);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        Assert.Null(jwt.Claims.FirstOrDefault(c => c.Type == "landlord_id"));
    }

    [Fact]
    public async Task LoginAsync_JwtExpiresIn7Days()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("user@test.com", AccountState.Active);
        SetupFindByEmail(storeMock, user);
        SetupCheckPassword(storeMock, user, true);
        SetupGetRoles(storeMock, user, ["Landlord"]);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@test.com",
            Password = "Password123"
        });

        Assert.NotNull(result);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        var expiry = jwt.ValidTo;
        var expectedExpiry = DateTime.UtcNow.AddDays(7);
        Assert.InRange(expiry, expectedExpiry.AddMinutes(-1), expectedExpiry.AddMinutes(1));
    }

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_DoesNotThrow()
    {
        var (service, storeMock) = CreateService();
        SetupFindByEmail(storeMock, null);

        var exception = await Record.ExceptionAsync(() =>
            service.ForgotPasswordAsync(new ForgotPasswordRequest { Email = "unknown@test.com" }));

        Assert.Null(exception);
    }

    private static ApplicationUser CreateTestUser(string email, AccountState state)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            AccountState = state,
            TokenValidFrom = DateTimeOffset.UtcNow.AddDays(-1),
            EmailConfirmed = true
        };
    }
}
