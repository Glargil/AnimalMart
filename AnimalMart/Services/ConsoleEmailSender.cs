using AnimalMart.Interfaces;
namespace AnimalMart.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        public string? LastBody { get; private set; }
        public Task SendAsync(string toEmail, string subject, string plainTextBody)
        {
            LastBody = plainTextBody;
            Console.WriteLine($"--- EMAIL to {toEmail} ---\n{subject}\n\n{plainTextBody}\n");
            return Task.CompletedTask;
        }
    }
}
