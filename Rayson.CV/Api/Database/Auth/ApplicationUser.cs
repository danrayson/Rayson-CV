using Microsoft.AspNetCore.Identity;

namespace Database.Auth;

public class ApplicationUser : IdentityUser<int>
{
    public required string DisplayName { get; set; }
}
