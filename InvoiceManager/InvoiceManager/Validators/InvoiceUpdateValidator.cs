using FluentValidation;
using InvoiceManager.DTO;

namespace InvoiceManager.Validators;

public class InvoiceUpdateValidator : AbstractValidator<InvoiceUpdateDTO>
{
    public InvoiceUpdateValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment required");
    }   
}