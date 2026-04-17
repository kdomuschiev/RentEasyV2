using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentEasy.Api.Common.Middleware;

namespace RentEasy.Api.Tests.Common.Middleware;

public class RequiresPasswordChangeMiddlewareTests
{
    private bool _nextCalled;

    [Fact]
    public async Task InvokeAsync_UnauthenticatedRequest_PassesThrough()
    {
        var middleware = CreateMiddleware();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        Assert.True(_nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ActiveAccount_PassesThrough()
    {
        var middleware = CreateMiddleware();
        var context = CreateContextWithAccountState("Active", "/api/properties", "GET");

        await middleware.InvokeAsync(context);

        Assert.True(_nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_RequiresPasswordChange_ChangePasswordEndpoint_PassesThrough()
    {
        var middleware = CreateMiddleware();
        var context = CreateContextWithAccountState("RequiresPasswordChange", "/api/auth/change-password", "POST");

        await middleware.InvokeAsync(context);

        Assert.True(_nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_RequiresPasswordChange_OtherEndpoint_Returns403()
    {
        var middleware = CreateMiddleware();
        var context = CreateContextWithAccountState("RequiresPasswordChange", "/api/properties", "GET");
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.False(_nextCalled);
        Assert.Equal(403, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(body);
        Assert.Equal("Forbidden", body.Title);
        Assert.Equal(403, body.Status);
    }

    [Fact]
    public async Task InvokeAsync_RequiresPasswordChange_GetChangePassword_Returns403()
    {
        // GET to /api/auth/change-password should still be blocked — only POST is allowed
        var middleware = CreateMiddleware();
        var context = CreateContextWithAccountState("RequiresPasswordChange", "/api/auth/change-password", "GET");
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.False(_nextCalled);
        Assert.Equal(403, context.Response.StatusCode);
    }

    private RequiresPasswordChangeMiddleware CreateMiddleware()
    {
        return new RequiresPasswordChangeMiddleware(_ => { _nextCalled = true; return Task.CompletedTask; });
    }

    private static DefaultHttpContext CreateContextWithAccountState(string accountState, string path, string method)
    {
        var claims = new List<Claim>
        {
            new("account_state", accountState)
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };
        context.Request.Path = path;
        context.Request.Method = method;
        return context;
    }
}
