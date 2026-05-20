using FluentValidation;

namespace TaskManager.Application.Feature.Projects.Commands.UpdateProject
{
    public class UpdateProjectValidtor : AbstractValidator<UpdateProject>
    {
        public UpdateProjectValidtor()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid project ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
