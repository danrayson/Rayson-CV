using System;

namespace Infrastructure.Auth;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
}
