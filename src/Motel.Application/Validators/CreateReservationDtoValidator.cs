using FluentValidation;
using Motel.Application.DTOs;

namespace Motel.Application.Validators;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("يجب اختيار نزيل");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("يجب اختيار غرفة");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("تاريخ تسجيل الدخول مطلوب");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("تاريخ تسجيل الخروج مطلوب")
            .GreaterThan(x => x.CheckInDate).WithMessage("تاريخ المغادرة يجب أن يكون بعد تاريخ الوصول");

        RuleFor(x => x.Guests)
            .GreaterThan(0).WithMessage("عدد النزلاء يجب أن يكون أكبر من صفر");

        RuleFor(x => x.NightlyRate)
            .GreaterThan(0).WithMessage("سعر الليلة يجب أن يكون أكبر من صفر");
    }
}
