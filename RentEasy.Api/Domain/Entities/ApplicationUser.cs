using Microsoft.AspNetCore.Identity;
using RentEasy.Api.Domain.Enums;

namespace RentEasy.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset TokenValidFrom { get; set; } = DateTimeOffset.UtcNow;
    public AccountState AccountState { get; set; } = AccountState.Active;
}
