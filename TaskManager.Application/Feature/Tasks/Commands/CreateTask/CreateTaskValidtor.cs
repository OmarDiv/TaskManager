using FluentValidation;

namespace TaskManager.Application.Feature.Tasks.Commands.CreateTask
{
    public class CreateTaskValidtor : AbstractValidator<CreateTask>
    {
        public CreateTaskValidtor()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(150).WithMessage("Task title cannot exceed 150 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid task priority.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Invalid project ID.");
        }
    }
}
