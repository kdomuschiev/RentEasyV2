using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RentEasy.Api.Common.Middleware;

namespace RentEasy.Api.Tests.Common.Middleware;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoException_PassesThrough()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => Task.CompletedTask,
            Mock.Of<ILogger<ErrorHandlingMiddleware>>());

        var context = new DefaultHttpContext();
        await middleware.InvokeAsync(context);

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Exception_Returns500ProblemDetails()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new InvalidOperationException("Test error"),
            Mock.Of<ILogger<ErrorHandlingMiddleware>>());

        var context = CreateContextWithEnvironment("Production");
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(body);
        Assert.Equal(500, body.Status);
        Assert.Equal("Internal Server Error", body.Title);
        Assert.Equal("An unexpected error occurred.", body.Detail);
    }

    [Fact]
    public async Task InvokeAsync_Exception_InDevelopment_ExposesMessage()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new InvalidOperationException("Detailed error message"),
            Mock.Of<ILogger<ErrorHandlingMiddleware>>());

        var context = CreateContextWithEnvironment("Development");
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(body);
        Assert.Equal("Detailed error message", body.Detail);
    }

    private static DefaultHttpContext CreateContextWithEnvironment(string environmentName)
    {
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns(environmentName);

        var services = new ServiceCollection();
        services.AddSingleton(envMock.Object);

        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        return context;
    }
}
