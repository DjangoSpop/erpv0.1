using System.ComponentModel.DataAnnotations;

namespace Motel.Web.ViewModels.Reservations;

/// <summary>
/// ViewModel for check-in process
/// </summary>
public class CheckInViewModel
{
    public Guid ReservationId { get; set; }

    // Reservation Details
    public string ClientName { get; set; } = default!;
    public string ClientPhone { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public DateOnly ScheduledCheckInDate { get; set; }
    public DateOnly ScheduledCheckOutDate { get; set; }
    public int Guests { get; set; }

    // Check-in Information
    [Required(ErrorMessage = "وقت تسجيل الدخول مطلوب")]
    [Display(Name = "وقت تسجيل الدخول")]
    public DateTime ActualCheckInTime { get; set; } = DateTime.Now;

    [Display(Name = "عدد النزلاء الفعلي")]
    [Range(1, 20)]
    public int ActualGuests { get; set; }

    [Display(Name = "رقم الهوية/جواز السفر")]
    [MaxLength(50)]
    public string? DocumentNumber { get; set; }

    [Display(Name = "نوع الوثيقة")]
    public string? DocumentType { get; set; }

    [Display(Name = "ملاحظات تسجيل الدخول")]
    [MaxLength(500)]
    public string? CheckInNotes { get; set; }

    // Validation
    public bool IsEarlyCheckIn { get; set; }
    public TimeSpan EarlyCheckInDuration { get; set; }
    public decimal EarlyCheckInFee { get; set; }

    // Special Requests
    [Display(Name = "طلبات خاصة")]
    public List<string> SpecialRequests { get; set; } = new();

    [Display(Name = "طلب خاص جديد")]
    public string? NewSpecialRequest { get; set; }
}
