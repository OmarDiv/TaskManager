using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;

namespace TaskManager.Application.Feature.Projects.Commands.CreateProject
{
    public class CreateProjectValidtor : AbstractValidator<CreateProject>
    {
        public CreateProjectValidtor()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .MustContainArabicLocalization(ResultMessage.ArabicLanguageRequired);
            RuleForEach(x => x.Name).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ResultMessage.Required);
            RuleForEach(x => x.Description).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage(ResultMessage.GreaterThan);
        }
    }
}
