namespace Motel.Domain.Exceptions;

/// <summary>
/// Base exception for business logic errors in the Motel Management System
/// </summary>
public class MotelBusinessException : Exception
{
    public string ErrorCode { get; }

    public MotelBusinessException(string message, string errorCode = "MOTEL_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public MotelBusinessException(string message, Exception innerException, string errorCode = "MOTEL_ERROR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : MotelBusinessException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID '{id}' was not found", "ENTITY_NOT_FOUND")
    {
    }

    public EntityNotFoundException(string entityName, string identifier)
        : base($"{entityName} '{identifier}' was not found", "ENTITY_NOT_FOUND")
    {
    }
}

/// <summary>
/// Exception thrown when a room is not available for booking
/// </summary>
public class RoomNotAvailableException : MotelBusinessException
{
    public Guid RoomId { get; }
    public DateOnly CheckInDate { get; }
    public DateOnly CheckOutDate { get; }

    public RoomNotAvailableException(Guid roomId, DateOnly checkIn, DateOnly checkOut)
        : base($"Room is not available from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd}", "ROOM_NOT_AVAILABLE")
    {
        RoomId = roomId;
        CheckInDate = checkIn;
        CheckOutDate = checkOut;
    }
}

/// <summary>
/// Exception thrown when trying to perform an operation on a reservation in invalid state
/// </summary>
public class InvalidReservationStateException : MotelBusinessException
{
    public Guid ReservationId { get; }
    public string CurrentState { get; }
    public string RequiredState { get; }

    public InvalidReservationStateException(Guid reservationId, string currentState, string requiredState)
        : base($"Cannot perform this operation. Reservation is in '{currentState}' state, but requires '{requiredState}'", "INVALID_RESERVATION_STATE")
    {
        ReservationId = reservationId;
        CurrentState = currentState;
        RequiredState = requiredState;
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : MotelBusinessException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred", "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when a duplicate entry is detected
/// </summary>
public class DuplicateEntityException : MotelBusinessException
{
    public string EntityName { get; }
    public string FieldName { get; }
    public string Value { get; }

    public DuplicateEntityException(string entityName, string fieldName, string value)
        : base($"{entityName} with {fieldName} '{value}' already exists", "DUPLICATE_ENTITY")
    {
        EntityName = entityName;
        FieldName = fieldName;
        Value = value;
    }
}

/// <summary>
/// Exception thrown when trying to delete an entity that has dependent records
/// </summary>
public class EntityHasDependenciesException : MotelBusinessException
{
    public string EntityName { get; }
    public Guid EntityId { get; }
    public string DependentEntity { get; }

    public EntityHasDependenciesException(string entityName, Guid entityId, string dependentEntity)
        : base($"Cannot delete {entityName}. It has existing {dependentEntity} records", "ENTITY_HAS_DEPENDENCIES")
    {
        EntityName = entityName;
        EntityId = entityId;
        DependentEntity = dependentEntity;
    }
}

/// <summary>
/// Exception thrown when a concurrency conflict occurs
/// </summary>
public class ConcurrencyException : MotelBusinessException
{
    public ConcurrencyException()
        : base("The record you attempted to update was modified by another user. Please refresh and try again", "CONCURRENCY_CONFLICT")
    {
    }
}
