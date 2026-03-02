using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Validators;
using FluentValidation;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        {
            RuleFor(x => x.RequestDto.FirstName)
                .NotEmpty().WithMessage("Firstname is required")
                .MinimumLength(2).WithMessage("Firstname must be at least 2 characters long");

            RuleFor(x => x.RequestDto.LastName)
                .NotEmpty().WithMessage("Lastname is required")
                .MinimumLength(2).WithMessage("Lastname must be at least 2 characters long");

            RuleFor(x => x.RequestDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.RequestDto.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Password();

            RuleFor(x => x.RequestDto.ConfirmPassword)
                .NotEmpty().WithMessage("Confirmed password is required")
                .Equal(x => x.RequestDto.Password).WithMessage("Passwords do not match");
        }
    }
}