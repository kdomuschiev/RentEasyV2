using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using renteasy_api.Common.Middleware;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Tests.Common.Middleware;

public class TokenValidFromMiddlewareTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _userStoreMock = new();
    private bool _nextCalled;

    [Fact]
    public async Task InvokeAsync_UnauthenticatedRequest_PassesThrough()
    {
        var middleware = new TokenValidFromMiddleware(_ => { _nextCalled = true; return Task.CompletedTask; });
        var context = new DefaultHttpContext();
        var userManager = CreateUserManager();

        await middleware.InvokeAsync(context, userManager);

        Assert.True(_nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ValidToken_PassesThrough()
    {
        var userId = Guid.NewGuid();
        var iat = DateTimeOffset.UtcNow;
        var user = new ApplicationUser
        {
            Id = userId,
            TokenValidFrom = iat.AddMinutes(-5) // token issued AFTER TokenValidFrom
        };

        SetupFindById(user);

        var middleware = new TokenValidFromMiddleware(_ => { _nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(userId, iat);
        var userManager = CreateUserManager();

        await middleware.InvokeAsync(context, userManager);

        Assert.True(_nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_InvalidatedToken_Returns401()
    {
        var userId = Guid.NewGuid();
        var iat = DateTimeOffset.UtcNow.AddHours(-1);
        var user = new ApplicationUser
        {
            Id = userId,
            TokenValidFrom = DateTimeOffset.UtcNow // TokenValidFrom is AFTER iat
        };

        SetupFindById(user);

        var middleware = new TokenValidFromMiddleware(_ => { _nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(userId, iat);
        context.Response.Body = new MemoryStream();
        var userManager = CreateUserManager();

        await middleware.InvokeAsync(context, userManager);

        Assert.False(_nextCalled);
        Assert.Equal(401, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(body);
        Assert.Equal("Token has been invalidated.", body.Detail);
    }

    [Fact]
    public async Task InvokeAsync_UserNotFound_Returns401()
    {
        var userId = Guid.NewGuid();
        var iat = DateTimeOffset.UtcNow;

        _userStoreMock.Setup(s => s.FindByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var middleware = new TokenValidFromMiddleware(_ => { _nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(userId, iat);
        context.Response.Body = new MemoryStream();
        var userManager = CreateUserManager();

        await middleware.InvokeAsync(context, userManager);

        Assert.False(_nextCalled);
        Assert.Equal(401, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateAuthenticatedContext(Guid userId, DateTimeOffset iat)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Iat, iat.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        return context;
    }

    private void SetupFindById(ApplicationUser user)
    {
        _userStoreMock.Setup(s => s.FindByIdAsync(user.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    private UserManager<ApplicationUser> CreateUserManager()
    {
        return new UserManager<ApplicationUser>(
            _userStoreMock.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
