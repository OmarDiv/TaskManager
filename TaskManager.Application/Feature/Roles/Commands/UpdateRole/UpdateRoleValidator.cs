using FluentValidation;

namespace TaskManager.Application.Feature.Roles.Commands.UpdateRole
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRole>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0).WithMessage("Invalid role ID.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Permissions)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Permissions)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You can't add duplicated permissions for the same role")
                .When(x => x.Permissions != null);
        }
    }
}
