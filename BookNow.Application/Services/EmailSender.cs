using BookNow.Application.Interfaces;
using BookNow.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;


namespace BookNow.Utility 
{
    public class EmailSender : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
           
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));

           
            message.To.Add(new MailboxAddress(email, email));

            message.Subject = subject;

           
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    _logger.LogInformation("Connecting to SMTP {Host}:{Port}", _emailSettings.SmtpHost, _emailSettings.SmtpPort);

                    await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}