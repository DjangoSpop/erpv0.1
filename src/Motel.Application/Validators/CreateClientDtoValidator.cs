using FluentValidation;
using Motel.Application.DTOs;

namespace Motel.Application.Validators;

public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("اسم النزيل مطلوب")
            .MaximumLength(200).WithMessage("الاسم يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.NationalIdOrPassport)
            .NotEmpty().WithMessage("رقم الهوية أو جواز السفر مطلوب")
            .MaximumLength(50).WithMessage("رقم الهوية يجب ألا يتجاوز 50 حرف");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب")
            .MaximumLength(20).WithMessage("رقم الهاتف يجب ألا يتجاوز 20 رقم");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("البريد الإلكتروني غير صحيح");
    }
}
