namespace renteasy_api.Application.DTOs.Auth;

public class LoginResponse
{
    public required string Token { get; init; }
    public required string Role { get; init; }
    public required string AccountState { get; init; }
}
