using ASP_NET_23._TaskFlow_CQRS.Application.Validators;
using FluentValidation;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.RequestDto.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Password();
    }
}