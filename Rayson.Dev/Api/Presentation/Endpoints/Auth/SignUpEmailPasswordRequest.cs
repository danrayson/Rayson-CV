namespace Presentation.Endpoints.Auth;

public class SignUpEmailPasswordRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
}
