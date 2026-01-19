using Database.Auth;

namespace Infrastructure.Auth;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}
