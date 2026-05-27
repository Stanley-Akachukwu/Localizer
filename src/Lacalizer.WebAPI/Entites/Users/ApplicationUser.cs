using Microsoft.AspNetCore.Identity;

namespace Lacalizer.WebAPI.Entites.Users;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
