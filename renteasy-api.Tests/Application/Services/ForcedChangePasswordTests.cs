using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using renteasy_api.Application.Services;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Tests.Application.Services;

public class ForcedChangePasswordTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<AuthService>> _loggerMock;

    public ForcedChangePasswordTests()
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
        storeMock.As<IUserEmailStore<ApplicationUser>>();
        storeMock.As<IUserRoleStore<ApplicationUser>>();
        storeMock.As<IUserPasswordStore<ApplicationUser>>();

        var userManager = new UserManager<ApplicationUser>(
            storeMock.Object,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            null!,
            null!,
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());

        var service = new AuthService(userManager, _configuration, _loggerMock.Object);
        return (service, storeMock);
    }

    private static void SetupFindById(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser? user)
    {
        storeMock.Setup(s => s.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    private static void SetupPasswordStore(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser user)
    {
        var passwordStore = storeMock.As<IUserPasswordStore<ApplicationUser>>();
        string? currentHash = "OldHashValue";

        // Simulate RemovePassword: sets hash to null
        // Simulate AddPassword: reads hash (null after remove means HasPassword = false) then sets new hash
        passwordStore
            .Setup(s => s.SetPasswordHashAsync(user, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback<ApplicationUser, string?, CancellationToken>((_, hash, _) => currentHash = hash)
            .Returns(Task.CompletedTask);

        passwordStore
            .Setup(s => s.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => currentHash);
    }

    private static void SetupUpdate(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser user)
    {
        storeMock.Setup(s => s.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
    }

    private static void SetupGetRoles(Mock<IUserStore<ApplicationUser>> storeMock, ApplicationUser user, IList<string> roles)
    {
        storeMock.As<IUserRoleStore<ApplicationUser>>()
            .Setup(s => s.GetRolesAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
    }

    [Fact]
    public async Task ForcedChangePasswordAsync_PasswordValidationFails_ReturnsFailureWithoutRemovingPassword()
    {
        // Create a service with a real PasswordValidator so short passwords are rejected
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        storeMock.As<IUserEmailStore<ApplicationUser>>();
        storeMock.As<IUserRoleStore<ApplicationUser>>();
        storeMock.As<IUserPasswordStore<ApplicationUser>>();

        var userManager = new UserManager<ApplicationUser>(
            storeMock.Object,
            Options.Create(new IdentityOptions { Password = { RequiredLength = 8, RequireDigit = false, RequireLowercase = false, RequireUppercase = false, RequireNonAlphanumeric = false } }),
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            new IPasswordValidator<ApplicationUser>[] { new PasswordValidator<ApplicationUser>() },
            new UpperInvariantLookupNormalizer(),
            null!,
            null!,
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());

        var service = new AuthService(userManager, _configuration, _loggerMock.Object);
        var user = CreateTestUser("tenant@test.com", AccountState.RequiresPasswordChange);
        SetupFindById(storeMock, user);
        // Do NOT set up password store — pre-validation should reject before RemovePasswordAsync is called

        var (success, error, response) = await service.ForcedChangePasswordAsync(user.Id, "ab");

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Null(response);
        // Verify RemovePasswordAsync was never called (store.UpdateAsync should not have been invoked)
        storeMock.Verify(
            s => s.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ForcedChangePasswordAsync_UserNotFound_ReturnsFailure()
    {
        var (service, storeMock) = CreateService();
        SetupFindById(storeMock, null);

        var (success, error, response) = await service.ForcedChangePasswordAsync(Guid.NewGuid(), "NewPassword1");

        Assert.False(success);
        Assert.Equal("User not found.", error);
        Assert.Null(response);
    }

    [Fact]
    public async Task ForcedChangePasswordAsync_Success_SetsAccountStateToActive()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("tenant@test.com", AccountState.RequiresPasswordChange);
        SetupFindById(storeMock, user);
        SetupPasswordStore(storeMock, user);
        SetupUpdate(storeMock, user);
        SetupGetRoles(storeMock, user, ["Tenant"]);

        var (success, error, response) = await service.ForcedChangePasswordAsync(user.Id, "NewPassword1");

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(response);
        Assert.Equal(AccountState.Active.ToString(), response.AccountState);
    }

    [Fact]
    public async Task ForcedChangePasswordAsync_Success_ReturnsNewJwt()
    {
        var (service, storeMock) = CreateService();
        var user = CreateTestUser("tenant@test.com", AccountState.RequiresPasswordChange);
        SetupFindById(storeMock, user);
        SetupPasswordStore(storeMock, user);
        SetupUpdate(storeMock, user);
        SetupGetRoles(storeMock, user, ["Tenant"]);

        var (success, _, response) = await service.ForcedChangePasswordAsync(user.Id, "NewPassword1");

        Assert.True(success);
        Assert.NotNull(response);
        Assert.NotEmpty(response.Token);
        Assert.Equal("Tenant", response.Role);
    }

    [Fact]
    public async Task ForcedChangePasswordAsync_Success_UpdatesTokenValidFrom()
    {
        var (service, storeMock) = CreateService();
        var originalTokenValidFrom = DateTimeOffset.UtcNow.AddDays(-1);
        var user = CreateTestUser("tenant@test.com", AccountState.RequiresPasswordChange);
        user.TokenValidFrom = originalTokenValidFrom;
        SetupFindById(storeMock, user);
        SetupPasswordStore(storeMock, user);
        SetupUpdate(storeMock, user);
        SetupGetRoles(storeMock, user, ["Tenant"]);

        await service.ForcedChangePasswordAsync(user.Id, "NewPassword1");

        Assert.True(user.TokenValidFrom > originalTokenValidFrom);
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
