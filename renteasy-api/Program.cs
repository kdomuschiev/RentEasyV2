
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using renteasy_api.Application.Services;
using renteasy_api.Common.Middleware;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;
using renteasy_api.Infrastructure.Data;
using Scalar.AspNetCore;

namespace renteasy_api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add IHttpContextAccessor (required by AppDbContext for HasQueryFilter)
        builder.Services.AddHttpContextAccessor();

        // Add EF Core + Neon (pooled connection string for runtime)
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        // Add ASP.NET Core Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT authentication
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSection["Key"];
        if (!string.IsNullOrEmpty(jwtKey))
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });
        }

        // CORS
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Application services
        builder.Services.AddScoped<AuthService>();

        var app = builder.Build();

        // Seed roles and landlord account
        await SeedDataAsync(app.Services);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseMiddleware<TokenValidFromMiddleware>();
        app.UseMiddleware<RequiresPasswordChangeMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Create roles
        string[] roles = ["Landlord", "Tenant"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
            }
        }

        // Seed landlord account (idempotent)
        var landlordEmail = configuration["Seed:LandlordEmail"];
        var landlordPassword = configuration["Seed:LandlordPassword"];

        if (!string.IsNullOrEmpty(landlordEmail) && !string.IsNullOrEmpty(landlordPassword))
        {
            var existingUser = await userManager.FindByEmailAsync(landlordEmail);
            if (existingUser == null)
            {
                var landlord = new ApplicationUser
                {
                    UserName = landlordEmail,
                    Email = landlordEmail,
                    EmailConfirmed = true,
                    AccountState = AccountState.Active
                };

                var result = await userManager.CreateAsync(landlord, landlordPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(landlord, "Landlord");
                }
            }
        }
    }
}
