namespace Motel.Infrastructure.Notifications;

public interface ISmsProvider
{
    Task SendAsync(string to, string message);
}
