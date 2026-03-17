using Application.Contact;
using Application.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace Infrastructure.Contact;

public class GraphEmailService(IOptions<GraphOptions> graphOptions, ILogger<GraphEmailService> logger) : IContactService
{
    private readonly ILogger<GraphEmailService> _logger = logger;
    private readonly GraphOptions _graphOptions = graphOptions.Value;
    private const string SenderEmail = "daniel@rayson.dev";
    private const string RecipientEmail = "daniel@rayson.dev";
    private const string RecipientName = "Daniel Rayson";

    private GraphServiceClient GetGraphClient()
    {
        var credential = new ClientSecretCredential(
            _graphOptions.TenantId,
            _graphOptions.ClientId,
            _graphOptions.ClientSecret
        );

        return new GraphServiceClient(credential);
    }

    public async Task<ServiceResponse> SendContactEmailAsync(ContactRequest request)
    {
        try
        {
            var graphClient = GetGraphClient();

            var subject = $"[Contact Form] {request.Subject}";
            var body = $@"
Name: {request.Name}
Email: {request.Email}
Subject: {request.Subject}

Message:
{request.Message}
";

            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = body
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = RecipientEmail,
                            Name = RecipientName
                        }
                    }
                }
            };

            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = false
            };

            await graphClient.Users[SenderEmail].SendMail.PostAsync(requestBody);

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
