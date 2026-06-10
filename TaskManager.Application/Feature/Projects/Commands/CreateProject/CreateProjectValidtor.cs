using FluentValidation;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Common.Localizations;
using TaskManager.Application.Common.Extensions;
using TaskManager.Application.Common.Interfaces.Repositories;

namespace TaskManager.Application.Feature.Projects.Commands.CreateProject
{
    public class CreateProjectValidtor : AbstractValidator<CreateProject>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public CreateProjectValidtor(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ResultMessage.Required)
                .MustContainArabicLocalization(ResultMessage.ArabicLanguageRequired);
            RuleForEach(x => x.Name).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ResultMessage.Required);
            RuleForEach(x => x.Description).SetValidator(new LocalizationDtoValidator());

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage(ResultMessage.GreaterThan);

            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    var requestNames = dto.Name
                        .Select(x => (x.Value ?? "").Trim().ToLower())
                        .ToList();

                    var exists = await _projectRepository.IsExist(
                        p => p.CreatedById == dto.UserId &&
                             p.NameSet.Localization.Any(l => requestNames.Contains(l.Value.ToLower())),
                        cancellationToken);

                    return !exists;
                })
                .WithMessage(ResultMessage.ProjectDuplicateName);
        }
    }
}
