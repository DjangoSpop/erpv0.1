using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Motel.Web.ViewModels.Reservations;

/// <summary>
/// Multi-step wizard for creating reservations
/// </summary>
public class ReservationWizardViewModel
{
    // Step 1: Client Selection
    [Required(ErrorMessage = "يجب اختيار نزيل")]
    [Display(Name = "النزيل")]
    public Guid? ClientId { get; set; }
    public SelectList? AvailableClients { get; set; }
    public string? SelectedClientName { get; set; }
    public string? SelectedClientPhone { get; set; }

    // Step 2: Dates Selection
    [Required(ErrorMessage = "تاريخ تسجيل الدخول مطلوب")]
    [Display(Name = "تاريخ الوصول")]
    public DateOnly? CheckInDate { get; set; }

    [Required(ErrorMessage = "تاريخ تسجيل الخروج مطلوب")]
    [Display(Name = "تاريخ المغادرة")]
    public DateOnly? CheckOutDate { get; set; }

    [Required(ErrorMessage = "عدد النزلاء مطلوب")]
    [Range(1, 20, ErrorMessage = "عدد النزلاء يجب أن يكون بين 1 و 20")]
    [Display(Name = "عدد النزلاء")]
    public int Guests { get; set; } = 1;

    // Computed
    public int NumberOfNights => CheckInDate.HasValue && CheckOutDate.HasValue
        ? Math.Max(1, CheckOutDate.Value.DayNumber - CheckInDate.Value.DayNumber)
        : 0;

    // Step 3: Room Selection
    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new();

    [Required(ErrorMessage = "يجب اختيار غرفة")]
    [Display(Name = "الغرفة")]
    public Guid? RoomId { get; set; }
    public string? SelectedRoomNumber { get; set; }
    public decimal SelectedRoomRate { get; set; }

    // Step 4: Pricing & Extras
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
    [Display(Name = "السعر لليلة")]
    public decimal NightlyRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "الخصم لا يمكن أن يكون سالباً")]
    [Display(Name = "الخصم")]
    public decimal DiscountAmount { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "الرسوم الإضافية لا يمكن أن تكون سالبة")]
    [Display(Name = "رسوم إضافية")]
    public decimal ExtraCharges { get; set; } = 0;

    [Display(Name = "ملاحظات")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Step 5: Summary & Confirmation
    public decimal Subtotal => (NightlyRate * NumberOfNights) + ExtraCharges - DiscountAmount;
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount => Subtotal + TaxAmount;

    // Wizard State
    public int CurrentStep { get; set; } = 1;
    public int TotalSteps => 5;
    public bool CanGoNext { get; set; }
    public bool CanGoPrevious => CurrentStep > 1;
    public bool IsLastStep => CurrentStep >= TotalSteps;
}

public class AvailableRoomViewModel
{
    public Guid Id { get; set; }
    public string Number { get; set; } = default!;
    public string TypeDisplay { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BaseNightlyRate { get; set; }
    public string FormattedRate { get; set; } = default!;
    public string? Features { get; set; }
    public bool IsRecommended { get; set; }
}
