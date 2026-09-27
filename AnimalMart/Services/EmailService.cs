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
            // Validate config up front, with a specific error per missing/invalid
            // key, instead of letting a null flow into MailKit and surface as a
            // confusing NullReferenceException/FormatException deep in the call.
            var fromAddress = _configuration["Email:FromAddress"]
                ?? throw new InvalidOperationException("Missing configuration value 'Email:FromAddress'.");
            var smtpHost = _configuration["Email:SmtpHost"]
                ?? throw new InvalidOperationException("Missing configuration value 'Email:SmtpHost'.");
            var smtpPortRaw = _configuration["Email:SmtpPort"]
                ?? throw new InvalidOperationException("Missing configuration value 'Email:SmtpPort'.");
            if (!int.TryParse(smtpPortRaw, out var smtpPort))
            {
                throw new InvalidOperationException(
                    $"Configuration value 'Email:SmtpPort' is not a valid port number: '{smtpPortRaw}'.");
            }
            var smtpPassword = _configuration["Email:SmtpPassword"]
                ?? throw new InvalidOperationException("Missing configuration value 'Email:SmtpPassword'.");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "My Razor App",
                fromAddress));

            message.To.Add(MailboxAddress.Parse(recipient));

            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };
            //SmtpClient is called - no error handling currently
            using var smtp = new SmtpClient();
            //no error handling required, since we validated our values up front
            await smtp.ConnectAsync(
                smtpHost,
                smtpPort,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                fromAddress,
                smtpPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
