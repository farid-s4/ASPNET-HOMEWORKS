using ASP_16._TaskFlow_Ownership.DTOs;
using FluentValidation;

namespace ASP_16._TaskFlow_Ownership.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Project Name is required")
                    .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
