namespace TaskManager.Application.Common.Localizations
{
    public class LocalizationDtoValidator : AbstractValidator<LocalizationDto>
    {
        public LocalizationDtoValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty();

            RuleFor(x => x.Value)
                .NotEmpty();
        }
    }
}
