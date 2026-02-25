using System;

namespace Presentation.Endpoints.Auth;

public class RequestPasswordResetRequest
{
    public required string Email { get; set; }
    public required string WebsiteUrl { get; set; }
}
