using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;

namespace TaskManager.Application.Feature.Tasks.Commands.UpdateTask
{
    public class UpdateTaskValidtor : AbstractValidator<UpdateTask>
    {
        public UpdateTaskValidtor()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(ResultMessage.GreaterThan);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .MustContainArabicLocalization(ResultMessage.ArabicLanguageRequired);
            RuleForEach(x => x.Title).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ResultMessage.Required);
            RuleForEach(x => x.Description).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid task priority.");
        }
    }
}
