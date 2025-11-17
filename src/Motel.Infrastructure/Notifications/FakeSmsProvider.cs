using Microsoft.Extensions.Logging;

namespace Motel.Infrastructure.Notifications;

/// <summary>
/// Fake SMS provider for development. In production, replace with Twilio or other SMS gateway.
/// </summary>
public class FakeSmsProvider : ISmsProvider
{
    private readonly ILogger<FakeSmsProvider> _logger;

    public FakeSmsProvider(ILogger<FakeSmsProvider> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string message)
    {
        _logger.LogInformation(
            "FAKE SMS: To={To}, Message={Message}",
            to, message);

        // In production, implement actual SMS sending via Twilio, etc.
        return Task.CompletedTask;
    }
}
