using FluentValidation;
using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Models;

namespace InvoiceManager.Validators;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDTO>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.Comment).NotEmpty().WithMessage("Comment is required");
        RuleFor(x=>x.CustomerId).GreaterThan(0).WithMessage("ProjectId must be greater than 0");
    }
}