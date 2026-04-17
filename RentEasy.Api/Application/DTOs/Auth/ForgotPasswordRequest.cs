using System.ComponentModel.DataAnnotations;

namespace RentEasy.Api.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}
