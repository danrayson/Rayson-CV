using System;

namespace Presentation.Endpoints.Auth;

public class ChangePasswordRequest
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string Password { get; set; }
    public required string PasswordCheck { get; set; }
}
