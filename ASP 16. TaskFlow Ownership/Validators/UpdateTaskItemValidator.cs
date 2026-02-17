using ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;
using ASP_16._TaskFlow_Ownership.Models;
using FluentValidation;
using TaskStatus = ASP_16._TaskFlow_Ownership.Models.TaskStatus;

namespace ASP_16._TaskFlow_Ownership.Validators;

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
                .NotEmpty().WithMessage("TaskItem Title is required")
                    .MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters long");           

        RuleFor(x => x.Priority)           
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");

        RuleFor(x => x.Status)           
            .Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s))
            .WithMessage("Status must be one of: 0(ToDo), 1(InProgress), 2(Done)");
    }
}
