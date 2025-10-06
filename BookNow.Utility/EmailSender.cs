using BookNow.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
// Note: Removed redundant and conflicting using statements like System.Net.Mail

namespace BulkyBook.Utility // Use your actual namespace
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();

            // Sender address from configuration
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));

            // Recipient address provided by Identity
            message.To.Add(new MailboxAddress(email, email));

            message.Subject = subject;

            // Set the body content as HTML
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            try
            {
                // The SmtpClient is now unambiguously identified as MailKit.Net.Smtp.SmtpClient
                using (var client = new SmtpClient())
                {
                    // Connect to the SMTP server
                    await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    // Authenticate with credentials
                    await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

                    // Send the message
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // In a production app, use ILogger for logging
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}