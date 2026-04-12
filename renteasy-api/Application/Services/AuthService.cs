using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using renteasy_api.Application.DTOs.Auth;
using renteasy_api.Common;
using renteasy_api.Domain.Entities;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Application.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return null;

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return null;

        // Block expired accounts — access fully revoked (ReadOnly users can still log in to view data)
        if (user.AccountState == AccountState.Expired)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Tenant";

        var token = GenerateJwt(user, role);

        return new LoginResponse
        {
            Token = token,
            Role = role,
            AccountState = user.AccountState.ToString()
        };
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, "User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errors);
        }

        // Truncate to Unix-second boundary so iat >= TokenValidFrom holds for freshly issued tokens
        user.TokenValidFrom = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        if (user.AccountState == AccountState.RequiresPasswordChange)
            user.AccountState = AccountState.Active;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var updateErrors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            return (false, updateErrors);
        }

        return (true, null);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // No email enumeration — silently return
            _logger.LogInformation("Forgot password requested for non-existent email: {Email}", request.Email);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetUrl = $"{_configuration["App:FrontendUrl"]}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        // Log reset URL in development — actual email sending is wired up in a later story
        _logger.LogInformation("Password reset URL for {Email}: {ResetUrl}", user.Email, resetUrl);
    }

    public async Task<(bool Success, string? Error)> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return (false, "Invalid or expired reset token.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
            return (false, "Invalid or expired reset token.");

        // Truncate to Unix-second boundary so iat >= TokenValidFrom holds for freshly issued tokens
        user.TokenValidFrom = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var updateErrors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            return (false, updateErrors);
        }

        return (true, null);
    }

    public async Task<string?> GenerateNewTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Tenant";

        return GenerateJwt(user, role);
    }

    private string GenerateJwt(ApplicationUser user, string role)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role),
            new("account_state", user.AccountState.ToString())
        };

        // landlord_id claim is CRITICAL for HasQueryFilter in AppDbContext
        if (role == "Landlord")
        {
            claims.Add(new Claim("landlord_id", user.Id.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            notBefore: now,
            expires: now.AddDays(Constants.JwtExpiryDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
