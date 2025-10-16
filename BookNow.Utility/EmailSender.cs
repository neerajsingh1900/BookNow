using BookNow.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;


namespace BookNow.Utility 
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
                    await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}