using FluentValidation;
using Motel.Application.DTOs;

namespace Motel.Application.Validators;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("رقم الغرفة مطلوب")
            .MaximumLength(20).WithMessage("رقم الغرفة يجب ألا يتجاوز 20 حرف");

        RuleFor(x => x.BaseNightlyRate)
            .GreaterThan(0).WithMessage("سعر الليلة يجب أن يكون أكبر من صفر");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("سعة الغرفة يجب أن تكون أكبر من صفر")
            .LessThanOrEqualTo(20).WithMessage("سعة الغرفة يجب ألا تتجاوز 20 شخص");
    }
}
