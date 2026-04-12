using System.ComponentModel.DataAnnotations;

namespace renteasy_api.Application.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
