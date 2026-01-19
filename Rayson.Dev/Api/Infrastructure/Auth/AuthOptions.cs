namespace Infrastructure.Auth;

public class AuthOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string IssuerSigningKey { get; set; }
}
