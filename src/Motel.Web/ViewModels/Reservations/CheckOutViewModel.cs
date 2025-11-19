using System.ComponentModel.DataAnnotations;

namespace Motel.Web.ViewModels.Reservations;

/// <summary>
/// ViewModel for check-out process
/// </summary>
public class CheckOutViewModel
{
    public Guid ReservationId { get; set; }

    // Reservation Details
    public string ClientName { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public DateTime CheckInTime { get; set; }
    public DateOnly ScheduledCheckOutDate { get; set; }
    public int NumberOfNights { get; set; }

    // Check-out Information
    [Required(ErrorMessage = "وقت تسجيل الخروج مطلوب")]
    [Display(Name = "وقت تسجيل الخروج")]
    public DateTime ActualCheckOutTime { get; set; } = DateTime.Now;

    [Display(Name = "حالة الغرفة")]
    public string? RoomCondition { get; set; }

    [Display(Name = "ملاحظات تسجيل الخروج")]
    [MaxLength(500)]
    public string? CheckOutNotes { get; set; }

    // Additional Charges
    [Display(Name = "رسوم إضافية")]
    [Range(0, double.MaxValue)]
    public decimal AdditionalCharges { get; set; } = 0;

    [Display(Name = "سبب الرسوم الإضافية")]
    public string? AdditionalChargesReason { get; set; }

    public List<ExtraChargeItem> ExtraCharges { get; set; } = new();

    // Validation
    public bool IsLateCheckOut { get; set; }
    public TimeSpan LateCheckOutDuration { get; set; }
    public decimal LateCheckOutFee { get; set; }

    // Mini-bar / Services
    [Display(Name = "استهلاك المشروبات")]
    public decimal MiniBarCharges { get; set; } = 0;

    [Display(Name = "خدمات إضافية")]
    public decimal ServiceCharges { get; set; } = 0;

    // Damage
    [Display(Name = "أضرار")]
    public decimal DamageCharges { get; set; } = 0;

    [Display(Name = "وصف الأضرار")]
    public string? DamageDescription { get; set; }

    // Summary
    public decimal BaseAmount { get; set; }
    public decimal TotalExtraCharges => AdditionalCharges + MiniBarCharges + ServiceCharges + DamageCharges + LateCheckOutFee;
    public decimal FinalAmount => BaseAmount + TotalExtraCharges;

    // Payment Status
    public bool IsPaid { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue => FinalAmount - AmountPaid;
}

public class ExtraChargeItem
{
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime ChargedAt { get; set; }
}
