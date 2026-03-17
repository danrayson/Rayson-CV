using Application.Contact;
using Application.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Contact;

public class OutlookEmailService(IOptions<OutlookOptions> outlookOptions, ILogger<OutlookEmailService> logger) : IContactService
{
    private readonly ILogger<OutlookEmailService> _logger = logger;
    private const string SenderEmail = "danrayson@raysondev.onmicrosoft.com";
    private const string DisplayEmail = "daniel@rayson.dev";
    private const string DisplayName = "Daniel Rayson";
    private const string SmtpHost = "smtp.office365.com";
    private const int SmtpPort = 587;
    private readonly string _appPassword = outlookOptions.Value.AppPassword ?? throw new ArgumentException("Outlook:AppPassword not configured");

    public async Task<ServiceResponse> SendContactEmailAsync(ContactRequest request)
    {
        try
        {
            var subject = $"[Contact Form] {request.Subject}";
            var body = $@"
Name: {request.Name}
Email: {request.Email}
Subject: {request.Subject}

Message:
{request.Message}
";

            using var client = new SmtpClient(SmtpHost, SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderEmail, _appPassword),
                Timeout = 30000
            };

            var message = new MailMessage
            {
                From = new MailAddress(DisplayEmail, DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(DisplayEmail);
            message.ReplyToList.Add(new MailAddress(request.Email, request.Name));

            await client.SendMailAsync(message);

            _logger.LogInformation("Contact email sent successfully from {Email}", request.Email);
            return ServiceResponse.Succeed();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email from {Email}", request.Email);
            return ServiceResponse.Fail("Failed to send email. Please try again later.");
        }
    }
}
