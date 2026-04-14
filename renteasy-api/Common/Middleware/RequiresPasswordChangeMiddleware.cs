using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace renteasy_api.Common.Middleware;

public class RequiresPasswordChangeMiddleware
{
    private readonly RequestDelegate _next;

    public RequiresPasswordChangeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var accountState = context.User.FindFirst("account_state")?.Value;

            if (accountState == "RequiresPasswordChange")
            {
                var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
                var method = context.Request.Method;

                if (!(method == "POST" && (path == "/api/auth/change-password" || path == "/api/auth/forced-change-password")))
                {
                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    var problem = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807",
                        Title = "Forbidden",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "You must change your password before accessing other resources."
                    };

                    var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await context.Response.WriteAsync(json);
                    return;
                }
            }
        }

        await _next(context);
    }
}
