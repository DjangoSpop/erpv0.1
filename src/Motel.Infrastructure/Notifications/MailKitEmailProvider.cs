using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Motel.Infrastructure.Notifications;

public class MailKitEmailProvider : IEmailProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailKitEmailProvider> _logger;

    public MailKitEmailProvider(IConfiguration configuration, ILogger<MailKitEmailProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"] ?? "نظام إدارة الموتيل",
                _configuration["Email:FromAddress"] ?? "noreply@motel.local"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { TextBody = body };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            // For development, you might want to skip SSL verification
            // In production, remove this and configure proper SSL
            var host = _configuration["Email:SmtpHost"];
            var port = _configuration.GetValue<int>("Email:SmtpPort", 587);

            if (string.IsNullOrEmpty(host))
            {
                _logger.LogWarning("Email SMTP host not configured. Email not sent.");
                return;
            }

            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);

            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
