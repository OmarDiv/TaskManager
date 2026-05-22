using FluentValidation;

namespace TaskManager.Application.Feature.Projects.Commands.CreateProject
{
    public class CreateProjectValidtor : AbstractValidator<CreateProject>
    {
        public CreateProjectValidtor()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Invalid creator user ID.");
        }
    }
}
