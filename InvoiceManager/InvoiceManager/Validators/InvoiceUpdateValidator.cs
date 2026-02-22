using FluentValidation;
using InvoiceManager.DTO.InvoiceDTOs;

namespace InvoiceManager.Validators;

public class InvoiceUpdateValidator : AbstractValidator<InvoiceUpdateDTO>
{
    public InvoiceUpdateValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment required");
    }   
}