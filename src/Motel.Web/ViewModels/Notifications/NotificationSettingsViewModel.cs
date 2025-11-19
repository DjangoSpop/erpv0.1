using System.ComponentModel.DataAnnotations;

namespace Motel.Web.ViewModels.Notifications;

/// <summary>
/// ViewModel for configuring notification settings
/// </summary>
public class NotificationSettingsViewModel
{
    // Email Settings
    [Display(Name = "تفعيل إشعارات البريد الإلكتروني")]
    public bool EmailNotificationsEnabled { get; set; } = true;

    [Display(Name = "تفعيل إشعارات SMS")]
    public bool SmsNotificationsEnabled { get; set; } = false;

    // Booking Confirmations
    [Display(Name = "إرسال تأكيد الحجز")]
    public bool SendBookingConfirmation { get; set; } = true;

    [Display(Name = "إرسال إشعار تسجيل الدخول")]
    public bool SendCheckInNotification { get; set; } = true;

    // Checkout Reminders
    [Display(Name = "تذكير قبل الخروج بـ 24 ساعة")]
    public bool SendCheckout24hReminder { get; set; } = true;

    [Display(Name = "تذكير قبل الخروج بـ 2 ساعة")]
    public bool SendCheckout2hReminder { get; set; } = true;

    [Display(Name = "تذكير في وقت الخروج")]
    public bool SendCheckoutTimeReminder { get; set; } = true;

    // Checkout Time Configuration
    [Required]
    [Display(Name = "وقت الخروج الافتراضي")]
    public TimeOnly DefaultCheckoutTime { get; set; } = new TimeOnly(12, 0);

    [Required]
    [Display(Name = "وقت الدخول الافتراضي")]
    public TimeOnly DefaultCheckInTime { get; set; } = new TimeOnly(14, 0);

    // Payment Reminders
    [Display(Name = "تذكير بالدفع عند الإصدار")]
    public bool SendPaymentReminder { get; set; } = true;

    [Display(Name = "تذكير بالدفع المتأخر")]
    public bool SendOverduePaymentReminder { get; set; } = true;

    [Display(Name = "عدد أيام التأخير قبل التذكير")]
    [Range(1, 30)]
    public int OverdueDays { get; set; } = 3;

    // Email Template
    [Display(Name = "توقيع البريد الإلكتروني")]
    [MaxLength(500)]
    public string EmailSignature { get; set; } = "شكراً لاختياركم موتيل دهب";

    // SMS Template
    [Display(Name = "اسم المرسل (SMS)")]
    [MaxLength(11)]
    public string SmsSenderName { get; set; } = "DahabMotel";
}

/// <summary>
/// ViewModel for notification history
/// </summary>
public class NotificationHistoryViewModel
{
    public List<NotificationLogItem> Notifications { get; set; } = new();

    // Filtering
    public string? FilterChannel { get; set; }
    public bool? FilterSuccess { get; set; }
    public DateOnly? FilterFromDate { get; set; }
    public DateOnly? FilterToDate { get; set; }

    // Statistics
    public int TotalSent { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public decimal SuccessRate { get; set; }
}

public class NotificationLogItem
{
    public Guid Id { get; set; }
    public string Channel { get; set; } = default!;
    public string Recipient { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public string SentAtFormatted { get; set; } = default!;
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? ReservationId { get; set; }
    public string? ReservationReference { get; set; }
}
