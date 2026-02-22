using FluentValidation;
using InvoiceManager.DTO.CustomerDTOs;

namespace InvoiceManager.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDTO>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(20).WithMessage("Name must have 20 characters");
        RuleFor(x => x.Email)
            .EmailAddress().NotEmpty().WithMessage("Email is required");
        RuleFor(x=>x.Phone)
            .NotEmpty().WithMessage("Phone is required");
    }
}