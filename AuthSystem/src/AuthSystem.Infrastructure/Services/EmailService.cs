using AuthSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);
        _logger.LogInformation("Email body: {Body}", body);
        
        await Task.CompletedTask;
    }

    public async Task SendTwoFactorCodeAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        var subject = "Your Two-Factor Authentication Code";
        var body = $@"
            <h2>Two-Factor Authentication</h2>
            <p>Your verification code is: <strong>{code}</strong></p>
            <p>This code will expire in 10 minutes.</p>
            <p>If you didn't request this code, please ignore this email.</p>
        ";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to AuthSystem!";
        var body = $@"
            <h2>Welcome {firstName}!</h2>
            <p>Thank you for registering with AuthSystem.</p>
            <p>Your account has been successfully created.</p>
            <p>You can now log in and start using our services.</p>
        ";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }
}
