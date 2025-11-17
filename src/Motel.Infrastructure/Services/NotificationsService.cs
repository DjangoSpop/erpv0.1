using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Infrastructure.Data;
using Motel.Infrastructure.Notifications;

namespace Motel.Infrastructure.Services;

public class NotificationsService : INotificationsService
{
    private readonly AppDbContext _context;
    private readonly IEmailProvider _emailProvider;
    private readonly ISmsProvider _smsProvider;
    private readonly ILogger<NotificationsService> _logger;

    public NotificationsService(
        AppDbContext context,
        IEmailProvider emailProvider,
        ISmsProvider smsProvider,
        ILogger<NotificationsService> logger)
    {
        _context = context;
        _emailProvider = emailProvider;
        _smsProvider = smsProvider;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(Guid reservationId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) return;

        var subject = "تأكيد حجز غرفة";
        var body = $@"
عزيزنا {reservation.Client.FullName},

تم تأكيد حجزك بنجاح.

تفاصيل الحجز:
- الغرفة: {reservation.Room.Number}
- تاريخ الوصول: {reservation.CheckInDate:yyyy-MM-dd}
- تاريخ المغادرة: {reservation.CheckOutDate:yyyy-MM-dd}
- عدد النزلاء: {reservation.Guests}
- السعر لليلة: {reservation.NightlyRate:N2} جنيه

نتطلع لاستقبالكم!
";

        if (!string.IsNullOrEmpty(reservation.Client.Email))
        {
            await SendEmailAsync(reservation.Client.Email, subject, body, reservationId);
        }

        // Also send SMS if phone available
        var smsMessage = $"تم تأكيد حجزك في الغرفة {reservation.Room.Number} من {reservation.CheckInDate:yyyy-MM-dd} إلى {reservation.CheckOutDate:yyyy-MM-dd}";
        await SendSmsAsync(reservation.Client.Phone, smsMessage, reservationId);
    }

    public async Task SendCheckInNoticeAsync(Guid reservationId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) return;

        var subject = "تسجيل الدخول";
        var body = $@"
عزيزنا {reservation.Client.FullName},

تم تسجيل دخولك بنجاح في الغرفة {reservation.Room.Number}.

تاريخ المغادرة: {reservation.CheckOutDate:yyyy-MM-dd}

نتمنى لك إقامة سعيدة!
";

        if (!string.IsNullOrEmpty(reservation.Client.Email))
        {
            await SendEmailAsync(reservation.Client.Email, subject, body, reservationId);
        }
    }

    public async Task ScheduleCheckoutReminderAsync(Guid reservationId, DateTime whenUtc)
    {
        // In a real application, you would use a background job scheduler like Hangfire
        // For this implementation, we'll just log the scheduled reminder
        _logger.LogInformation(
            "تذكير بالمغادرة مجدول للحجز {ReservationId} في {WhenUtc}",
            reservationId, whenUtc);

        // For now, we could store this in the database and have a background service check periodically
        // This is a simplified implementation
    }

    public async Task SendEmailAsync(string to, string subject, string body, Guid? reservationId = null)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            ReservationId = reservationId,
            Channel = "Email",
            Recipient = to,
            Subject = subject,
            Body = body,
            SentAtUtc = DateTime.UtcNow
        };

        try
        {
            await _emailProvider.SendAsync(to, subject, body);
            log.Success = true;
        }
        catch (Exception ex)
        {
            log.Success = false;
            log.Error = ex.Message;
            _logger.LogError(ex, "فشل إرسال البريد الإلكتروني إلى {To}", to);
        }

        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task SendSmsAsync(string to, string message, Guid? reservationId = null)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            ReservationId = reservationId,
            Channel = "SMS",
            Recipient = to,
            Subject = "SMS",
            Body = message,
            SentAtUtc = DateTime.UtcNow
        };

        try
        {
            await _smsProvider.SendAsync(to, message);
            log.Success = true;
        }
        catch (Exception ex)
        {
            log.Success = false;
            log.Error = ex.Message;
            _logger.LogError(ex, "فشل إرسال الرسالة النصية إلى {To}", to);
        }

        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
