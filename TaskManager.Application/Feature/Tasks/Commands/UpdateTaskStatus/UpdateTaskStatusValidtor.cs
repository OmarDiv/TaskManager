using FluentValidation;

namespace TaskManager.Application.Feature.Tasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusValidtor : AbstractValidator<UpdateTaskStatus>
    {
        public UpdateTaskStatusValidtor()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid task ID.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status.");
        }
    }
}
