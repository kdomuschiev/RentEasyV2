using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentEasy.Api.Domain.Entities;

namespace RentEasy.Api.Common.Middleware;

public class TokenValidFromMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidFromMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var iatClaim = context.User.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;
            var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (iatClaim != null && userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                var iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatClaim));
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null || iat < user.TokenValidFrom)
                {
                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var problem = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807",
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Token has been invalidated."
                    };

                    var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await context.Response.WriteAsync(json);
                    return;
                }
            }
            else
            {
                // Authenticated token is missing required iat or sub claims — treat as invalid
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                var problem = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Token is missing required claims."
                };

                var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
                return;
            }
        }

        await _next(context);
    }
}
