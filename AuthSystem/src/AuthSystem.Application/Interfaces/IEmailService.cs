namespace AuthSystem.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendTwoFactorCodeAsync(string email, string code, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
}
