namespace Motel.Application.Interfaces;

public interface INotificationsService
{
    Task SendBookingConfirmationAsync(Guid reservationId);
    Task SendCheckInNoticeAsync(Guid reservationId);
    Task ScheduleCheckoutReminderAsync(Guid reservationId, DateTime whenUtc);
    Task SendEmailAsync(string to, string subject, string body, Guid? reservationId = null);
    Task SendSmsAsync(string to, string message, Guid? reservationId = null);
}
