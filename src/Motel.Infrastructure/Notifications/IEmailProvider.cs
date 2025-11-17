namespace Motel.Infrastructure.Notifications;

public interface IEmailProvider
{
    Task SendAsync(string to, string subject, string body);
}
