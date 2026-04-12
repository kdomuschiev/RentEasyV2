using System.ComponentModel.DataAnnotations;

namespace renteasy_api.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}
