using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using renteasy_api.Application.DTOs.Auth;
using renteasy_api.Application.Services;

namespace renteasy_api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Invalid email or password."
            });
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Invalid token."
            });
        }

        var (success, error) = await _authService.ChangePasswordAsync(userId, request);
        if (!success)
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error
            });
        }

        // Return new token so client can use it immediately
        var newToken = await _authService.GenerateNewTokenAsync(userId);
        if (newToken == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Failed to generate new token."
            });
        }

        return Ok(new ChangePasswordResponse { Token = newToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request);
        // Always return 200 regardless of whether email exists (no enumeration)
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var (success, error) = await _authService.ResetPasswordAsync(request);
        if (!success)
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error
            });
        }

        return Ok();
    }
}
