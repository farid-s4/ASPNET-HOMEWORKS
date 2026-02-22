using FluentValidation;
using InvoiceManager.Models;

namespace InvoiceManager.Validators;

public class CreateInvoiceValidator : AbstractValidator<Invoice>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.Comment).NotEmpty().WithMessage("Comment is required");
        RuleFor(x=>x.CustomerId).NotEmpty().WithMessage("Customer id required")
            .LessThan(0).WithMessage("Customer id must be greater than 0");
    }
}