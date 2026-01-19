using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;

        public EmailService()
        {
            _smtpHost = "fakehost";
            _smtpPort = 1234;
            _smtpUsername = "fakename";
            _smtpPassword = "fakepassword";
            _enableSsl = true;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            Console.WriteLine($"Sending email to {email} with subject '{subject}' and body '{body}'");
            return;
            // using (var client = new SmtpClient(_smtpHost, _smtpPort))
            // {
            //     client.EnableSsl = _enableSsl;
            //     client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            //     var message = new MailMessage
            //     {
            //         From = new MailAddress(_smtpUsername),
            //         Subject = subject,
            //         Body = body,
            //         IsBodyHtml = true // Set to false if you are sending plain text
            //     };
            //     message.To.Add(email);

            //     try
            //     {
            //         await client.SendMailAsync(message);
            //     }
            //     catch (Exception ex)
            //     {
            //         throw new Exception("Failed to send email", ex);
            //     }
            // }
        }
    }
}