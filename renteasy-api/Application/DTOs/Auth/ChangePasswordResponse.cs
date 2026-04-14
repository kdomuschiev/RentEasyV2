namespace renteasy_api.Application.DTOs.Auth;

public class ChangePasswordResponse
{
    public required string Token { get; init; }
    public required string Role { get; init; }
    public required string AccountState { get; init; }
}
