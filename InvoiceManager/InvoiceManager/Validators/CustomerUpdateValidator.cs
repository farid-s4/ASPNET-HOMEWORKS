using FluentValidation;
using InvoiceManager.DTO.CustomerDTOs;

namespace InvoiceManager.Validators;

public class CustomerUpdateValidator : AbstractValidator<CustomerUpdateDTO>
{
    public CustomerUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name required")
            .MinimumLength(3).WithMessage("Customer name must be at least 3 characters long");
    }
}