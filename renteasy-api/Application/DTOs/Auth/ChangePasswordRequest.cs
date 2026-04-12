using System.ComponentModel.DataAnnotations;

namespace renteasy_api.Application.DTOs.Auth;

public class ChangePasswordRequest
{
    [Required]
    public required string CurrentPassword { get; init; }

    [Required]
    [MinLength(8)]
    public required string NewPassword { get; init; }
}
