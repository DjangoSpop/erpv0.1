using Motel.Application.DTOs;
using Motel.Domain.Enums;
using Motel.Web.ViewModels.Reservations;

namespace Motel.Web.Extensions;

/// <summary>
/// Extension methods for mapping Reservation DTOs to ViewModels
/// </summary>
public static class ReservationMappingExtensions
{
    public static ReservationListItemViewModel ToListItemViewModel(this ReservationDto dto)
    {
        return new ReservationListItemViewModel
        {
            Id = dto.Id,
            ClientName = dto.ClientName,
            ClientPhone = dto.ClientPhone ?? "",
            RoomNumber = dto.RoomNumber,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            CheckInDateFormatted = dto.CheckInDate.ToString("dd/MM/yyyy"),
            CheckOutDateFormatted = dto.CheckOutDate.ToString("dd/MM/yyyy"),
            NumberOfNights = dto.NumberOfNights,
            Guests = dto.Guests,
            NightlyRate = dto.NightlyRate,
            TotalAmount = dto.TotalAmount,
            TotalAmountFormatted = $"{dto.TotalAmount:N2} ج.م",
            Status = dto.Status,
            StatusDisplay = GetReservationStatusDisplay(dto.Status),
            StatusBadgeClass = GetReservationStatusBadgeClass(dto.Status),
            CreatedAt = dto.CreatedAt,
            CreatedAtFormatted = dto.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            ActualCheckInTime = dto.ActualCheckInTime,
            ActualCheckOutTime = dto.ActualCheckOutTime,
            CanCheckIn = dto.Status == ReservationStatus.Confirmed,
            CanCheckOut = dto.Status == ReservationStatus.CheckedIn,
            CanCancel = dto.Status is ReservationStatus.Pending or ReservationStatus.Confirmed
        };
    }

    public static ReservationDetailViewModel ToDetailViewModel(this ReservationDto dto)
    {
        var listItem = dto.ToListItemViewModel();

        return new ReservationDetailViewModel
        {
            Id = listItem.Id,
            ClientName = listItem.ClientName,
            ClientPhone = listItem.ClientPhone,
            RoomNumber = listItem.RoomNumber,
            CheckInDate = listItem.CheckInDate,
            CheckOutDate = listItem.CheckOutDate,
            CheckInDateFormatted = listItem.CheckInDateFormatted,
            CheckOutDateFormatted = listItem.CheckOutDateFormatted,
            NumberOfNights = listItem.NumberOfNights,
            Guests = listItem.Guests,
            NightlyRate = listItem.NightlyRate,
            TotalAmount = listItem.TotalAmount,
            TotalAmountFormatted = listItem.TotalAmountFormatted,
            Status = listItem.Status,
            StatusDisplay = listItem.StatusDisplay,
            StatusBadgeClass = listItem.StatusBadgeClass,
            CreatedAt = listItem.CreatedAt,
            CreatedAtFormatted = listItem.CreatedAtFormatted,
            ActualCheckInTime = listItem.ActualCheckInTime,
            ActualCheckOutTime = listItem.ActualCheckOutTime,
            ActualCheckInTimeFormatted = listItem.ActualCheckInTime?.ToString("dd/MM/yyyy HH:mm"),
            ActualCheckOutTimeFormatted = listItem.ActualCheckOutTime?.ToString("dd/MM/yyyy HH:mm"),
            ExtraCharges = dto.ExtraCharges,
            ExtraChargesFormatted = $"{dto.ExtraCharges:N2} ج.م",
            FinalTotal = dto.TotalAmount + dto.ExtraCharges,
            FinalTotalFormatted = $"{(dto.TotalAmount + dto.ExtraCharges):N2} ج.م",
            Notes = dto.Notes,
            CanCheckIn = listItem.CanCheckIn,
            CanCheckOut = listItem.CanCheckOut,
            CanCancel = listItem.CanCancel,
            CanEdit = dto.Status is ReservationStatus.Pending or ReservationStatus.Confirmed,
            CanIssueInvoice = dto.Status == ReservationStatus.CheckedOut
        };
    }

    public static CheckInViewModel ToCheckInViewModel(this ReservationDto dto)
    {
        return new CheckInViewModel
        {
            ReservationId = dto.Id,
            ClientName = dto.ClientName,
            RoomNumber = dto.RoomNumber,
            ScheduledCheckInDate = dto.CheckInDate,
            ScheduledCheckInTime = new TimeOnly(14, 0), // Default check-in time
            ActualCheckInTime = DateTime.Now,
            ActualGuests = dto.Guests,
            IsEarlyCheckIn = DateTime.Now.Date < dto.CheckInDate.ToDateTime(TimeOnly.MinValue).Date,
            EarlyCheckInFee = 0,
            SpecialRequests = new List<string>(),
            RequiredDocuments = new List<string> { "بطاقة الهوية/جواز السفر" },
            DocumentsCollected = false
        };
    }

    public static CheckOutViewModel ToCheckOutViewModel(this ReservationDto dto)
    {
        var isLate = DateTime.Now.TimeOfDay > new TimeSpan(12, 0, 0); // After 12 PM

        return new CheckOutViewModel
        {
            ReservationId = dto.Id,
            ClientName = dto.ClientName,
            RoomNumber = dto.RoomNumber,
            ScheduledCheckOutDate = dto.CheckOutDate,
            ScheduledCheckOutTime = new TimeOnly(12, 0), // Default checkout time
            ActualCheckOutTime = DateTime.Now,
            IsLateCheckOut = isLate,
            LateCheckOutFee = isLate ? 50 : 0, // 50 EGP late fee
            RoomCondition = "جيدة",
            MiniBarCharges = 0,
            ServiceCharges = 0,
            DamageCharges = 0,
            AdditionalCharges = 0,
            DiscountAmount = 0,
            FinalPaymentStatus = "معلق"
        };
    }

    private static string GetReservationStatusDisplay(ReservationStatus status)
    {
        return status switch
        {
            ReservationStatus.Pending => "قيد الانتظار",
            ReservationStatus.Confirmed => "مؤكد",
            ReservationStatus.CheckedIn => "تم تسجيل الدخول",
            ReservationStatus.CheckedOut => "تم تسجيل الخروج",
            ReservationStatus.Cancelled => "ملغي",
            ReservationStatus.NoShow => "لم يحضر",
            _ => status.ToString()
        };
    }

    private static string GetReservationStatusBadgeClass(ReservationStatus status)
    {
        return status switch
        {
            ReservationStatus.Pending => "bg-secondary",
            ReservationStatus.Confirmed => "bg-info",
            ReservationStatus.CheckedIn => "bg-success",
            ReservationStatus.CheckedOut => "bg-primary",
            ReservationStatus.Cancelled => "bg-danger",
            ReservationStatus.NoShow => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }
}
