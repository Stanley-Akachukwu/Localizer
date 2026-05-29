using Lacalizer.WebAPI.Entites.Users;

namespace Lacalizer.WebAPI.Infrastructure.Seeds;

public static class UserSeed
{
    public static readonly ApplicationUser SystemUser = new ApplicationUser
    {
        Id = "01SYSTEMUSER00000000000001",
        UserName = "system@localizer.com",
        NormalizedUserName = "SYSTEM@LOCALIZER.COM",
        Email = "system@localizer.com",
        NormalizedEmail = "SYSTEM@LOCALIZER.COM",
        EmailConfirmed = true,

        FirstName = "System",
        LastName = "User",
        ProfileImageUrl = null,
        CreatedAt = DateTime.UtcNow
    };

    public static ApplicationUser[] GetUsers()
    {
        return new[]
        {
            SystemUser
        };
    }
}