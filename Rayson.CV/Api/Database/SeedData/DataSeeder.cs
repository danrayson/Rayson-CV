using Database.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Database.SeedData;

public class DataSeeder(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IDataSeeder
{
    private const string TestUserEmail = "testuser@test.com";
    private const string TestUserPassword = "TestPassword123!";
    private const string TestUserDisplayName = "Test User";

    public async Task SeedDatabase()
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

        if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(TestUserEmail);
        if (existingUser != null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            Email = TestUserEmail,
            UserName = TestUserEmail,
            DisplayName = TestUserDisplayName
        };

        var result = await userManager.CreateAsync(user, TestUserPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create test user: {errors}");
        }
    }
}
