using Application.Core;

namespace Application.Auth;

public interface IAuthService
{
    Task<ServiceResponse> SignUp(string displayName, string email, string password);
    Task<ServiceResponse<SignInResponse>> SignIn(string email, string password);
    Task<ServiceResponse> SendEmailPasswordResetConfirmationEmail(string email, string redirectUrl);
    Task<ServiceResponse<string>> ChangePassword(string email, string token, string password, string passwordCheck);
}
