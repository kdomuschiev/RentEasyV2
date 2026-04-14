namespace renteasy_api.Application.DTOs.Auth;

public class ForcedChangePasswordRequest
{
    public required string NewPassword { get; init; }
}
