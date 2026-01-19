using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Database.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class TokenService(IOptions<AuthOptions> authOptions, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : ITokenService
{
    private readonly TimeSpan _expiration = TimeSpan.FromHours(12);
    private readonly AuthOptions authOptions = authOptions.Value;
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly RoleManager<ApplicationRole> roleManager = roleManager;

    public async Task<string> CreateTokenAsync(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.Add(_expiration);
        var token = CreateJwtToken(
            await CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials,
        DateTime expiration)
    {
        return new(
            authOptions.Issuer,
            authOptions.Audience,
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
    }

    private async Task<List<Claim>> CreateClaims(ApplicationUser identityUser)
    {
        var roles = await userManager.GetRolesAsync(identityUser);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                //Role no longer exists
                continue;
            }
            var claims = await roleManager.GetClaimsAsync(role);
            roleClaims.AddRange(claims);
        }
        try
        {
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new(ClaimTypes.NameIdentifier, identityUser.Id.ToString()),
                    new(ClaimTypes.Name, identityUser.DisplayName),
                    new(ClaimTypes.Email, identityUser.Email!),
                    new(ClaimTypes.Role, roles.FirstOrDefault() ?? ""),
                    // new(CustomClaimTypes.IsRegistered, user.IsABender.ToString())
                };

            //TODO: Add permissions here when implemented

            return claims;
        }
        catch
        {
            throw;
        }
    }
    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authOptions.IssuerSigningKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}
