using System.ComponentModel.DataAnnotations;

namespace renteasy_api.Application.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Token { get; init; }

    [Required]
    [MinLength(8)]
    public required string NewPassword { get; init; }
}
