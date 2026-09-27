using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using AnimalMart.Interfaces;

namespace AnimalMart.Services
{
        
public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(
            string recipient,
            string subject,
            string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "My Razor App",
                _configuration["Email:Address"]!));

            message.To.Add(MailboxAddress.Parse(recipient));

            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["Email:SmtpServer"],
                int.Parse(_configuration["Email:Port"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["Email:Address"],
                _configuration["Email:Password"]);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
