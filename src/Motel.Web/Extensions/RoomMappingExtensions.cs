using Motel.Application.DTOs;
using Motel.Domain.Enums;
using Motel.Web.ViewModels.Rooms;

namespace Motel.Web.Extensions;

/// <summary>
/// Extension methods for mapping Room DTOs to ViewModels
/// </summary>
public static class RoomMappingExtensions
{
    public static RoomItemViewModel ToItemViewModel(this RoomDto dto)
    {
        return new RoomItemViewModel
        {
            Id = dto.Id,
            Number = dto.Number,
            Type = dto.Type,
            TypeDisplay = GetRoomTypeDisplay(dto.Type),
            Status = dto.Status,
            StatusDisplay = GetRoomStatusDisplay(dto.Status),
            StatusBadgeClass = GetRoomStatusBadgeClass(dto.Status),
            Capacity = dto.Capacity,
            BaseNightlyRate = dto.BaseNightlyRate,
            FormattedRate = $"{dto.BaseNightlyRate:N2} ج.م",
            Notes = dto.Notes,
            IsAvailable = dto.Status == RoomStatus.Available,
            ActiveReservations = 0 // Will be populated by service if needed
        };
    }

    public static RoomListViewModel ToListViewModel(
        this IEnumerable<RoomDto> rooms,
        string? searchTerm = null,
        RoomType? filterByType = null,
        RoomStatus? filterByStatus = null,
        int? minCapacity = null,
        decimal? maxPrice = null,
        DateOnly? availabilityFromDate = null,
        DateOnly? availabilityToDate = null,
        int? requiredGuests = null)
    {
        var roomItems = rooms.Select(r => r.ToItemViewModel()).ToList();

        return new RoomListViewModel
        {
            Rooms = roomItems,
            SearchTerm = searchTerm,
            FilterByType = filterByType,
            FilterByStatus = filterByStatus,
            MinCapacity = minCapacity,
            MaxPrice = maxPrice,
            AvailabilityFromDate = availabilityFromDate,
            AvailabilityToDate = availabilityToDate,
            RequiredGuests = requiredGuests,
            TotalRooms = roomItems.Count,
            AvailableRooms = roomItems.Count(r => r.Status == RoomStatus.Available),
            OccupiedRooms = roomItems.Count(r => r.Status == RoomStatus.Occupied),
            ReservedRooms = roomItems.Count(r => r.Status == RoomStatus.Reserved)
        };
    }

    private static string GetRoomTypeDisplay(RoomType type)
    {
        return type switch
        {
            RoomType.Single => "فردية",
            RoomType.Double => "مزدوجة",
            RoomType.Twin => "توأم",
            RoomType.Triple => "ثلاثية",
            RoomType.Family => "عائلية",
            RoomType.Suite => "جناح",
            RoomType.Chalet => "شاليه",
            RoomType.Dorm => "عنبر",
            _ => type.ToString()
        };
    }

    private static string GetRoomStatusDisplay(RoomStatus status)
    {
        return status switch
        {
            RoomStatus.Available => "متاحة",
            RoomStatus.Occupied => "مشغولة",
            RoomStatus.Reserved => "محجوزة",
            RoomStatus.OutOfService => "خارج الخدمة",
            RoomStatus.Maintenance => "صيانة",
            _ => status.ToString()
        };
    }

    private static string GetRoomStatusBadgeClass(RoomStatus status)
    {
        return status switch
        {
            RoomStatus.Available => "bg-success",
            RoomStatus.Occupied => "bg-danger",
            RoomStatus.Reserved => "bg-warning text-dark",
            RoomStatus.OutOfService => "bg-secondary",
            RoomStatus.Maintenance => "bg-info",
            _ => "bg-secondary"
        };
    }
}
