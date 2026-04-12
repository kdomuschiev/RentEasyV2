using Microsoft.AspNetCore.Identity;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset TokenValidFrom { get; set; } = DateTimeOffset.UtcNow;
    public AccountState AccountState { get; set; } = AccountState.Active;
}
