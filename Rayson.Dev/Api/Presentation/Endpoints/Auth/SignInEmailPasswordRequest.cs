namespace Presentation.Endpoints.Auth;

public class SignInEmailPasswordRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
